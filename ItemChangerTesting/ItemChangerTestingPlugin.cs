using BepInEx;
using BepInEx.Configuration;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Events;
using ItemChanger.Silksong;
using Silksong.ModMenu.Elements;
using Silksong.ModMenu.Models;
using Silksong.ModMenu.Plugin;
using Silksong.ModMenu.Screens;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChangerTesting
{
    [BepInDependency(ItemChangerPlugin.Id)]
    [BepInAutoPlugin(id: "io.github.testing.silksong.itemchanger")]
    public partial class ItemChangerTestingPlugin : BaseUnityPlugin, IModMenuCustomMenu
    {
        public required ConfigEntry<int> cfgSaveSlot;
        public required ConfigEntry<TestFolder> cfgTestFolder;
        public required ConfigEntry<int> cfgTestIndex;
        public required ConfigEntry<bool> cfgAutoRun;
        public required ConfigEntry<float> cfgAutoRunDelay;

        public static ItemChangerTestingPlugin Instance { get; private set; } = null!;
        public new BepInEx.Logging.ManualLogSource Logger => base.Logger;

        private void Awake()
        {
            Instance = this;
            cfgSaveSlot = Config.Bind(configDefinition: new ConfigDefinition(section: "Menu", key: "Save Slot"), defaultValue: 1,
                configDescription: new ConfigDescription("The save slot to use for the test.", acceptableValues: new AcceptableValueRange<int>(1, 100)));
            cfgTestFolder = Config.Bind(configDefinition: new ConfigDefinition(section: "Menu", key: "Test Folder"), defaultValue: (TestFolder)default,
                configDescription: new ConfigDescription("The test folder to search."));
            cfgTestIndex = Config.Bind(configDefinition: new ConfigDefinition(section: "Menu", key: "Test Index"), defaultValue: (int)default,
                configDescription: new ConfigDescription("The index of the test to launch, within its folder."));
            cfgAutoRun = Config.Bind(configDefinition: new ConfigDefinition(section: "AutoRun", key: "Enabled"), defaultValue: false,
                configDescription: new ConfigDescription("If true, auto-fire the test selected by Test Folder + Test Index when the title screen is ready."));
            cfgAutoRunDelay = Config.Bind(configDefinition: new ConfigDefinition(section: "AutoRun", key: "Delay Seconds"), defaultValue: 2.0f,
                configDescription: new ConfigDescription("Seconds to wait after the title screen is ready before firing the auto-run test."));

            LogLifecycleEvents();


            ItemChangerHost.Singleton.LifecycleEvents.OnEnterGame += () =>
            {
                inGame = true;
                testMethods?.VisibleSelf = true;
            };
            ItemChangerHost.Singleton.LifecycleEvents.OnLeaveGame += () =>
            {
                inGame = false;
                testMethods?.VisibleSelf = false;
            };

            if (cfgAutoRun.Value) SceneManager.sceneLoaded += OnSceneLoadedForAutoRun;
        }

        private bool autoRunFired;

        private void OnSceneLoadedForAutoRun(Scene scene, LoadSceneMode mode)
        {
            if (autoRunFired) return;
            if (scene.name != "Menu_Title") return;
            autoRunFired = true;
            SceneManager.sceneLoaded -= OnSceneLoadedForAutoRun;
            StartCoroutine(AutoRunCoroutine());
        }

        private System.Collections.IEnumerator AutoRunCoroutine()
        {
            yield return new WaitForSeconds(Mathf.Max(0f, cfgAutoRunDelay.Value));

            TestFolder folder = cfgTestFolder.Value;
            int index = cfgTestIndex.Value;
            if (!Test.TestGroups.TryGetValue(folder, out var tests) || index < 0 || index >= tests.Count)
            {
                Logger.LogWarning($"[TestingAutoRun] no test at folder={folder} index={index}; aborting auto-run.");
                AutoRunResult.Write($"{folder}[{index}]", "missing", null, "Folder/index out of range.");
                yield break;
            }

            Test test = tests[index];
            string menuName = test.GetMetadata().MenuName;
            Logger.LogInfo($"[TestingAutoRun] starting {menuName}");
            try
            {
                TestDispatcher.StartTest(test);
            }
            catch (Exception e)
            {
                Logger.LogError($"[TestingAutoRun] test {menuName} failed: {e}");
                AutoRunResult.Write(menuName, "failed", null, e.ToString());
            }
        }

        private bool inGame = false;
        private TextButton? testMethods;

        public AbstractMenuScreen BuildCustomMenu()
        {
            SimpleMenuScreen screen = new("ItemChangerTesting");
            MenuElementGenerators.CreateIntSliderGenerator()(cfgSaveSlot, out MenuElement? saveSlotSelector);
            ConfigEntryFactory.GenerateEnumChoiceElement(cfgTestFolder, out MenuElement? testFolderSelector);

            ListChoiceModel<Test> model = new([.. Test.TestGroups[cfgTestFolder.Value]])
            {
                DisplayFn = (_, t) => t.GetMetadata().MenuName
            };
            if (cfgTestIndex.Value is int savedIndex && savedIndex >= 0 && savedIndex < model.Values.Count) model.Index = savedIndex;
            model.OnValueChanged += _ => cfgTestIndex.Value = model.Index;
            DynamicDescriptionChoiceElement<Test> testSelector = new("Test", model, "The test to launch.", t => t.GetMetadata().MenuDescription);

            TextButton run = new("Erase save slot and launch test.");
            run.OnSubmit += Run;

            testMethods = new("Test Methods");
            testMethods.OnSubmit += ShowTestMethods;
            testMethods.VisibleSelf = inGame;

            screen.Add(saveSlotSelector!);
            screen.Add(testFolderSelector!);
            screen.Add(testSelector!);
            screen.Add(run);
            screen.Add(testMethods);

            void UpdateFolder(object sender, EventArgs args) => model.UpdateValues([.. Test.TestGroups[cfgTestFolder.Value]], 0);
            cfgTestFolder.SettingChanged += UpdateFolder;
            screen.OnDispose += () => cfgTestFolder.SettingChanged -= UpdateFolder;

            return screen;

            void Run()
            {
                UIManager.instance.HideMenuInstant(screen.MenuScreen);
                try
                {
                    TestDispatcher.StartTest(testSelector.Value);
                }
                catch (Exception e)
                {
                    Logger.LogError($"Error starting test: {e}");
                }
            }
        }

        private Test? lastLoadedTest;
        private AbstractMenuScreen? testMethodsScreen;

        void ShowTestMethods()
        {
            var activeTest = ItemChangerHost.Singleton.ActiveProfile?.Modules.Get<Test>();
            if (activeTest == null)
            {
                Logger.LogError("No active Test module.");
                return;
            }

            if (lastLoadedTest == activeTest)
            {
                MenuScreenNavigation.Show(testMethodsScreen!);
                return;
            }

            List<(string, Action)> hooks = [.. activeTest.TestMethods()];
            if (hooks.Count == 0)
            {
                Logger.LogError($"Test '{activeTest.GetMetadata().MenuName}' has no test methods.");
                return;
            }

            PaginatedMenuScreenBuilder builder = new($"{activeTest.GetMetadata().MenuName} Test Methods");
            foreach (var (name, hook) in hooks)
            {
                TextButton button = new(name);
                button.OnSubmit += hook;
                builder.Add(button);
            }

            testMethodsScreen?.Dispose();
            testMethodsScreen = builder.Build();
            testMethodsScreen.OnDispose += () =>
            {
                testMethodsScreen = null;
                lastLoadedTest = null;
            };

            lastLoadedTest = activeTest;
            MenuScreenNavigation.Show(testMethodsScreen);
        }

        // TODO - this probably ought to be in ItemChanger.Core
        private void LogLifecycleEvents()
        {
            SilksongHost.Instance.LifecycleEvents.OnLeaveGame += () => Logger.LogInfo("Invoked " + nameof(LifecycleEvents.OnLeaveGame));
            SilksongHost.Instance.LifecycleEvents.OnEnterGame += () => Logger.LogInfo("Invoked " + nameof(LifecycleEvents.OnEnterGame));
            SilksongHost.Instance.LifecycleEvents.OnSafeToGiveItems += () => Logger.LogInfo("Invoked " + nameof(LifecycleEvents.OnSafeToGiveItems));
            SilksongHost.Instance.LifecycleEvents.OnItemChangerHook += () => Logger.LogInfo("Invoked " + nameof(LifecycleEvents.OnItemChangerHook));
            SilksongHost.Instance.LifecycleEvents.OnItemChangerUnhook += () => Logger.LogInfo("Invoked " + nameof(LifecycleEvents.OnItemChangerUnhook));
            SilksongHost.Instance.LifecycleEvents.BeforeStartNewGame += () => Logger.LogInfo("Invoked " + nameof(LifecycleEvents.BeforeStartNewGame));
            SilksongHost.Instance.LifecycleEvents.BeforeContinueGame += () => Logger.LogInfo("Invoked " + nameof(LifecycleEvents.BeforeContinueGame));
            SilksongHost.Instance.LifecycleEvents.AfterStartNewGame += () => Logger.LogInfo("Invoked " + nameof(LifecycleEvents.AfterStartNewGame));
            SilksongHost.Instance.LifecycleEvents.AfterContinueGame += () => Logger.LogInfo("Invoked " + nameof(LifecycleEvents.AfterContinueGame));
        }

        public LocalizedText ModMenuName() => "ItemChangerTesting";
    }
}
