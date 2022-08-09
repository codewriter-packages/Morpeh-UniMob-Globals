using System.Runtime.CompilerServices;
using UnityEngine;

[assembly:InternalsVisibleTo("CodeWriter.Morpeh.UniMob.Globals")]

namespace Morpeh
{
     partial class WorldExtensions {
        [RuntimeInitializeOnLoadMethod]
        private static void InitializeGlobalsOnLoad() {
            UnityRuntimeHelper.onApplicationFocusLost += GlobalsSaveControl.Save;
        }

        static partial void InitializeGlobals(this World world) {
            var sg = world.CreateSystemsGroup();
            sg.AddSystem(new Morpeh.Globals.ECS.ProcessUniMobEventsSystem());
            world.AddSystemsGroup(99999, sg);
        }
    }
}