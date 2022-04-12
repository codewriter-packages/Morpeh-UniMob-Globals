namespace Morpeh.Globals {
    using System.Collections.Generic;

    namespace ECS {
        internal sealed class ProcessEventsSystem : ILateSystem {
            public World World { get; set; }

            public string name => "MorpehProcessEventsSystem";

            public void OnAwake() {
            }

            public void OnUpdate(float deltaTime) {
                if (this.World != World.Default) {
                    return;
                }

                var list = GlobalsUpdater.DispatchedEvents;
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
        public static List<IGlobalUpdateListener> DispatchedEvents { get; set; } = new List<IGlobalUpdateListener>();
        public static List<IGlobalUpdateListener> ExecutingEvents  { get; set; } = new List<IGlobalUpdateListener>();
    }
}

namespace Morpeh {
    public static class GlobalsWorldExtensions {
        public static void InitializeGlobals(this World world) {
            var sg = world.CreateSystemsGroup();
            sg.AddSystem(new Morpeh.Globals.ECS.ProcessEventsSystem());
            world.AddSystemsGroup(99999, sg);
        }
    }
}