namespace Morpeh.Globals {
    using System.Collections.Generic;

    namespace ECS {
        internal sealed class ProcessUniMobEventsSystem : ILateSystem {
            public World World { get; set; }

            public void OnAwake() {
            }

            public void OnUpdate(float deltaTime) {
                if (this.World != World.Default) {
                    return;
                }

                var list = GlobalsUpdater.DispatchedEvents;
                if (list.Count == 0) {
                    return;
                }

                GlobalsUpdater.DispatchedEvents = GlobalsUpdater.ExecutingEvents;
                GlobalsUpdater.ExecutingEvents  = list;

                foreach (var evt in list) {
                    evt.OnFrameEnd();
                }

                list.Clear();
            }

            public void Dispose() {
            }
        }
    }

    internal interface IGlobalUpdateListener {
        void OnFrameEnd();
    }

    internal static class GlobalsUpdater {
        public static List<IGlobalUpdateListener> DispatchedEvents = new List<IGlobalUpdateListener>();
        public static List<IGlobalUpdateListener> ExecutingEvents = new List<IGlobalUpdateListener>();
    }
}