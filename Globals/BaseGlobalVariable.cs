namespace Morpeh.Globals {
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UniMob;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;

#endif

    public abstract class BaseGlobalVariable<TData> : BaseGlobal, IGlobalUpdateListener, IGlobalVariable {
        [PropertySpace]
        [Title("Default Data")]
        [SerializeField]
        [PropertyOrder(10)]
        [DelayedProperty]
        [HideLabel]
        [DisableInPlayMode]
        private TData value;

        [NonSerialized]
        [ShowInInspector]
        [PropertySpace]
        [Title("Runtime Data")]
        [PropertyOrder(11)]
        [DelayedProperty]
        [HideLabel]
        [HideInEditorMode]
        [OnValueChanged(nameof(InvalidateRuntimeValue))]
        [OnInspectorGUI(nameof(OnRuntimeValueInspector), append: true)]
        private TData runtimeValue;

        private const string COMMON_KEY = "MORPEH__GLOBALS_VARIABLES_";

        [HideInInlineEditors]
        [PropertyOrder(1)]
        [ShowIf("@" + nameof(AutoSave) + " && " + nameof(CanBeAutoSaved))]
        [SerializeField]
        private string customKey;

        // ReSharper disable once InconsistentNaming
        private string __internalKey;

        private string Key {
            get {
                if (string.IsNullOrEmpty(this.__internalKey)) {
                    this.__internalKey = COMMON_KEY + this.customKey;
                }

                return this.__internalKey;
            }
        }

        public string CustomKey => this.customKey;

        public virtual bool CanBeAutoSaved => true;

        [Header("Saving Settings")]
        [HideInInlineEditors]
        [PropertyOrder(0)]
        [ShowIf(nameof(CanBeAutoSaved))]
        // ReSharper disable once InconsistentNaming
        public bool AutoSave;

        public  bool HasPlayerPrefsValue            => PlayerPrefs.HasKey(this.Key);
        private bool HasPlayerPrefsValueAndAutoSave => PlayerPrefs.HasKey(this.Key) && this.AutoSave;

        internal event Action<TData> Callback;

        [NonSerialized] private bool isLoaded;
        [NonSerialized] private bool isPublished;

        [NonSerialized] private MutableAtom<TData> runtimeValueHolder;

        public Atom<TData> AtomValue => this.runtimeValueHolder;

        public TData DefaultValue {
            get => this.value;
            set => this.value = value;
        }

        public TData Value {
            get {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    return this.DefaultValue;
                }
#endif

                return this.runtimeValueHolder.Value;
            }
            set {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    this.DefaultValue = value;
                }
                else
#endif
                {
                    this.runtimeValueHolder.Value = value;
                }
            }
        }

        public abstract TData  Deserialize(string serializedData);
        public abstract string Serialize(TData data);

        public virtual TData GetDefaultValue() => this.value;

        public IDisposable Subscribe(Action<TData> callback) {
            return new Subscription(this, callback);
        }

        public void ModifyValue(Action<TData> edit) {
            edit(this.Value);
            
            this.runtimeValueHolder.Invalidate();

            if (!this.isPublished) {
                GlobalsUpdater.DispatchedEvents.Add(this);
            }

            this.isPublished = true;
        }

        private void InvalidateRuntimeValue()
        {
            this.runtimeValueHolder?.Invalidate();
        }

        private TData GetRuntimeValue() {
            return this.runtimeValue;
        }
        
        private void SetRuntimeValue(TData newValue)
        {
            this.runtimeValue = newValue;

            if (this.isPublished) {
                return;
            }

            this.isPublished = true;

            GlobalsUpdater.DispatchedEvents.Add(this);
        }

        private void OnRuntimeValueInspector()
        {
#if UNITY_EDITOR
            Sirenix.Utilities.Editor.GUIHelper.RequestRepaint();
#endif
        }

        internal class Subscription : IDisposable {
            private readonly BaseGlobalVariable<TData> variable;
            private readonly Action<TData>             callback;

            public Subscription(BaseGlobalVariable<TData> variable, Action<TData> callback) {
                this.variable = variable;
                this.callback = callback;

                this.variable.Callback += this.callback;
            }

            public void Dispose() {
                this.variable.Callback -= this.callback;
            }
        }

        protected virtual void OnEnable() {
            this.__internalKey = null;

            GlobalsSaveControl.SaveRequested += this.SaveData;

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += this.OnPlayModeStateChanged;

            if (string.IsNullOrEmpty(this.customKey)) {
                this.GenerateCustomKey();
            }
#endif
            this.LoadData();
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange state) {
            if (state == PlayModeStateChange.EnteredPlayMode) {
                this.LoadData();
            }
        }
#endif

        [Button]
        [PropertyOrder(3)]
        [ShowIf("@" + nameof(AutoSave))]
        [HideInInlineEditors]
        private void GenerateCustomKey() {
            this.__internalKey = null;
            this.customKey     = Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

        private void LoadData() {
            if (this.isLoaded) {
                return;
            }

            this.isLoaded = true;

            this.runtimeValue = this.AutoSave && PlayerPrefs.HasKey(this.Key)
                ? this.Deserialize(PlayerPrefs.GetString(this.Key))
                : this.GetDefaultValue();

            this.runtimeValueHolder = Atom.Computed(Lifetime.Eternal,
                pull: this.GetRuntimeValue,
                push: this.SetRuntimeValue,
                debugName: this.name);
        }

        internal void SaveData() {
            if (this.AutoSave) {
                PlayerPrefs.SetString(this.Key, this.Serialize(this.runtimeValueHolder.Value));
            }
        }


        #region EDITOR

#if UNITY_EDITOR
        [HideInInlineEditors]
        [ShowIf("@" + nameof(HasPlayerPrefsValueAndAutoSave))]
        [PropertyOrder(4)]
        [Button]
        internal void ResetPlayerPrefsValue() {
            if (this.HasPlayerPrefsValue) {
                PlayerPrefs.DeleteKey(this.Key);
            }
        }
#endif

        #endregion

        public void OnFrameEnd() {
            if (this.isPublished) {
                this.Callback?.Invoke(this.runtimeValueHolder.Value);
                this.isPublished = false;
            }
        }
    }

    public interface IGlobalVariable {
        string CustomKey { get; }
    }
}