using Newtonsoft.Json;
using UnityEngine;

namespace ItemChangerTesting
{
    internal static class AutoRunResult
    {
        public static void Write(string testName, string status, string? scene, string? error)
        {
            try
            {
                string path = System.IO.Path.Combine(Application.persistentDataPath, "ic_testing_autorun_result.json");
                string json = JsonConvert.SerializeObject(new
                {
                    test = testName,
                    status,
                    scene,
                    error,
                    timestamp = DateTime.UtcNow.ToString("o"),
                }, Formatting.Indented);
                System.IO.File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                ItemChangerTestingPlugin.Instance.Logger.LogWarning($"[TestingAutoRun] failed to write result json: {e.Message}");
            }
        }
    }
}
