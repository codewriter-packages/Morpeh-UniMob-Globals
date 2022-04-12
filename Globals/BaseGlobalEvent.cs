namespace Morpeh.Globals {
    using System;
    using System.Collections.Generic;
    using UniMob;
    using UnityEngine;

    public abstract class BaseGlobalEvent<TData> : BaseGlobal, IGlobalUpdateListener {
        [NonSerialized] private readonly List<TData> batchedChanges   = new List<TData>();
        [NonSerialized] private readonly List<TData> scheduledChanges = new List<TData>();

        [NonSerialized] private bool isPublished;
        [NonSerialized] private bool isScheduled;

        public List<TData> BatchedChanges   => this.batchedChanges;
        public List<TData> ScheduledChanges => this.scheduledChanges;

        public bool IsPublished => this.isPublished;
        public bool IsScheduled => this.isScheduled;

        internal event Action<List<TData>> Callback;

        public void Publish(TData data) {
            this.batchedChanges.Add(data);

            if (!this.isPublished && !this.isScheduled) {
                GlobalsUpdater.DispatchedEvents.Add(this);
            }

            this.isPublished = true;
        }

        public void NextFrame(TData data) {
            this.scheduledChanges.Add(data);

            if (!this.isPublished && !this.isScheduled) {
                GlobalsUpdater.DispatchedEvents.Add(this);
            }

            this.isScheduled = true;
        }

        public IDisposable Subscribe(Action<List<TData>> callback) {
            return new Subscription(this, callback);
        }
        
        public void Subscribe(Lifetime lifetime, Action<List<TData>> callback) {
            lifetime.Register(this.Subscribe(callback));
        }
        
        internal class Subscription : IDisposable {
            private readonly BaseGlobalEvent<TData> owner;
            private readonly Action<List<TData>>    callback;

            public Subscription(BaseGlobalEvent<TData> owner, Action<List<TData>> callback) {
                this.owner    = owner;
                this.callback = callback;

                this.owner.Callback += this.callback;
            }

            public void Dispose() {
                this.owner.Callback -= this.callback;
            }
        }

        public void OnFrameEnd() {
            if (this.isPublished) {
                this.Callback?.Invoke(this.batchedChanges);
                this.isPublished = false;
                this.batchedChanges.Clear();
            }

            if (this.isScheduled) {
                this.isPublished = true;

                EnsureCapacity(this.batchedChanges, this.batchedChanges.Count + this.scheduledChanges.Count);
                foreach (var evt in this.scheduledChanges) {
                    this.batchedChanges.Add(evt);
                }

                this.isScheduled = false;
                this.scheduledChanges.Clear();

                GlobalsUpdater.DispatchedEvents.Add(this);
            }
        }

        private static void EnsureCapacity(List<TData> list, int min) {
            var capacity = list.Capacity;

            if (capacity >= min) {
                return;
            }

            var newCapacity = capacity == 0 ? 4 : capacity * 2;
            if (newCapacity < min) {
                newCapacity = Mathf.NextPowerOfTwo(min);
            }

            list.Capacity = newCapacity;
        }
    }
}