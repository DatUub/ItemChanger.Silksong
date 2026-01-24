using UnityEngine;

namespace ItemChanger.Silksong.Util;

public static class MessageUtil
{
    private static readonly Queue<ColorMsg> messages = [];

    public static void EnqueueMessage(string text, Sprite? sprite = null, Color? modifyTextColor = null)
    {
        messages.Enqueue(new(new ICMsg(text, sprite ?? SpriteUtil.Empty), modifyTextColor ?? Color.white));
    }

    public static void EnqueueMessage(ICollectableUIMsgItem msg, Color? modifyTextColor = null)
    {
        messages.Enqueue(new(msg, modifyTextColor ?? Color.white));
    }

    public static void Error()
    {
        EnqueueMessage("Error: see LogOutput.log for details.", sprite: null, Color.red);
    }

    internal static void Setup()
    {
        GameObject obj = new("MessageController parent");
        obj.AddComponent<MessageController>();
        UObject.DontDestroyOnLoad(obj);
    }

    internal static void Clear()
    {
        MessageController.Instance.Reset();
    }

    private class MessageController : MonoBehaviour
    {
        private static bool canSendMessage = true;

        public static MessageController Instance 
        { 
            get => field ?? throw new NullReferenceException("MessageController was not instantiated!");
            private set;
        }

        public void Awake() => Instance = this;

        public void Update()
        {
            if (canSendMessage && messages.TryDequeue(out ColorMsg msg))
            {
                try
                {
                    CollectableUIMsg.Spawn(msg.Message, msg.Color, replacing: null, forceReplacingEffect: false);
                    canSendMessage = false;
                    this.ExecuteDelayed(3f, () => canSendMessage = true); // messages have a normal duration of ~4.75f
                }
                catch (Exception e)
                {
                    GlobalRefs.Logger.LogError($"Error spawning message: {e}");
                    canSendMessage = true;
                }
            }
        }

        public void Reset()
        {
            StopAllCoroutines();
            messages.Clear();
            canSendMessage = true; 
        }
    }

    private readonly struct ColorMsg(ICollectableUIMsgItem message, Color color)
    {
        public ColorMsg(ICollectableUIMsgItem message) : this(message, Color.white) { }
        public ICollectableUIMsgItem Message { get; } = message;
        public Color Color { get; } = color;
    }

    private class ICMsg(string message, Sprite sprite) : ICollectableUIMsgItem
    {
        public string Message { get; } = message;
        public Sprite Sprite { get; } = sprite;

        public UObject GetRepresentingObject()
        {
            return MessageController.Instance;
        }

        public float GetUIMsgIconScale() => 1f;

        public string GetUIMsgName() => Message;

        public Sprite GetUIMsgSprite() => Sprite;

        public bool HasUpgradeIcon() => false;
    }
}


