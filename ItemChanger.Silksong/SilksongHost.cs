using ItemChanger.Containers;
using ItemChanger.Events;
using ItemChanger.Logging;
using ItemChanger.Modules;

namespace ItemChanger.Silksong
{
    public class SilksongHost : ItemChangerHost
    {
        public override ILogger Logger => throw new NotImplementedException();

        public override ContainerRegistry ContainerRegistry => throw new NotImplementedException();

        public override Finder Finder => throw new NotImplementedException();

        public override IEnumerable<Module> BuildDefaultModules()
        {
            throw new NotImplementedException();
        }

        protected override void PrepareEvents(LifecycleEvents.Invoker lifecycleInvoker, GameEvents.Invoker gameInvoker)
        {
            throw new NotImplementedException();
        }

        protected override void UnhookEvents(LifecycleEvents.Invoker lifecycleInvoker, GameEvents.Invoker gameInvoker)
        {
            throw new NotImplementedException();
        }
    }
}
