namespace Morpeh {
    using System;
    using UnityEngine;

    public static class GlobalsSaveControl {
        public static event Action SaveRequested = delegate { };

        public static void Save() {
            SaveRequested.Invoke();
            PlayerPrefs.Save();
        }
    }
}