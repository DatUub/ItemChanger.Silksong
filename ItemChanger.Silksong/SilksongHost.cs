using ItemChanger.Containers;
using ItemChanger.Events;
using ItemChanger.Logging;
using ItemChanger.Modules;
using ItemChanger.Silksong.Modules;
using ItemChanger.Silksong.RawData;

namespace ItemChanger.Silksong
{
    public class SilksongHost : ItemChangerHost
    {
        internal SilksongHost() { }

        public override ILogger Logger { get; } = new PluginLogger();

        public override ContainerRegistry ContainerRegistry { get; } = new()
        {
            DefaultSingleItemContainer = Containers.ShinyContainer.Instance,
            DefaultMultiItemContainer = Containers.ChestContainer.Instance,
        };

        public override Finder Finder { get; } = CreateFinder();

        private static Finder CreateFinder()
        {
            var finder = new Finder();
            foreach (var item in BaseItemList.GetItems())
            {
                finder.DefineItem(item);
            }
            return finder;
        }

        public override IEnumerable<Module> BuildDefaultModules()
        {
            return [
                new PlayerDataEditModule(),
                ];
        }

        private LifecycleEvents.Invoker? lifecycleInvoker;
        private GameEvents.Invoker? gameInvoker;

        protected override void PrepareEvents(LifecycleEvents.Invoker lifecycleInvoker, GameEvents.Invoker gameInvoker)
        {
            this.lifecycleInvoker = lifecycleInvoker;
            this.gameInvoker = gameInvoker;

            On.GameManager.StartNewGame += BeforeStartNewGameHook;
            On.GameManager.ContinueGame += OnContinueGame;
            On.GameManager.BeginSceneTransition += TransitionHook;
            On.GameManager.ResetSemiPersistentItems += OnResetSemiPersistentItems;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        protected override void UnhookEvents(LifecycleEvents.Invoker lifecycleInvoker, GameEvents.Invoker gameInvoker)
        {
            this.lifecycleInvoker = null;
            this.gameInvoker = null;

            On.GameManager.StartNewGame -= BeforeStartNewGameHook;
            On.GameManager.ContinueGame -= OnContinueGame;
            On.GameManager.BeginSceneTransition -= TransitionHook;
            On.GameManager.ResetSemiPersistentItems -= OnResetSemiPersistentItems;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene from, UnityEngine.SceneManagement.Scene to)
        {
            if (to.name == "Menu_Title")
            {
                lifecycleInvoker?.NotifyOnLeaveGame();
                return;
            }

            gameInvoker?.NotifyPersistentUpdate();
        }

        private void OnResetSemiPersistentItems(On.GameManager.orig_ResetSemiPersistentItems orig, GameManager self)
        {
            gameInvoker?.NotifySemiPersistentUpdate();
            orig(self);
        }

        private void TransitionHook(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info)
        {
            string targetScene = info.SceneName;
            string targetGate = info.EntryGateName;

            gameInvoker?.NotifyBeforeNextSceneLoaded(new Events.Args.BeforeSceneLoadedEventArgs(targetScene)); // TODO: add gate info
            // TODO: transition overrides
            orig(self, info);
        }

        private void OnContinueGame(On.GameManager.orig_ContinueGame orig, GameManager self)
        {
            lifecycleInvoker?.NotifyBeforeContinueGame();
            lifecycleInvoker?.NotifyOnEnterGame();
            orig(self);
            lifecycleInvoker?.NotifyAfterContinueGame();
        }

        private void BeforeStartNewGameHook(On.GameManager.orig_StartNewGame orig, GameManager self, bool permadeathMode, bool bossRushMode)
        {
            lifecycleInvoker?.NotifyBeforeStartNewGame();
            
            // TODO: StartDef
            lifecycleInvoker?.NotifyOnEnterGame();
            orig(self, permadeathMode, bossRushMode);

            lifecycleInvoker?.NotifyAfterStartNewGame();
            lifecycleInvoker?.NotifyOnSafeToGiveItems(); // TODO: move
        }
    }

    file class PluginLogger : ILogger
    {
        void ILogger.LogError(string? message)
        {
            ItemChangerPlugin.Instance.Logger.LogError(message);
        }

        void ILogger.LogFine(string? message)
        {
            ItemChangerPlugin.Instance.Logger.LogDebug(message);
        }

        void ILogger.LogInfo(string? message)
        {
            ItemChangerPlugin.Instance.Logger.LogInfo(message);
        }

        void ILogger.LogWarn(string? message)
        {
            ItemChangerPlugin.Instance.Logger.LogWarning(message);
        }
    }
}
