using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SHABS.OBJECTPOOL
{
    public class ObjectPool<TType> : IDisposable
    {
        private bool isDisposed;
        private Func<ObjectPool<TType>, TType> factory;
        private LoadingMode loadingMode;
        private IItemStore<TType> itemStore;
        private int size;
        private int count;
        private Semaphore sync;




        #region constrcutors

        public ObjectPool(int size, Func<ObjectPool<TType>, TType> factory)
            : this(size, factory, LoadingMode.Lazy, AccessMode.FIFO)
        {
        }

        public ObjectPool(int size, Func<ObjectPool<TType>, TType> factory, LoadingMode loadingMode, AccessMode accessMode)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException("size", (object)size, "Argument 'size' must be greater than zero.");
            }
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }
            this.size = size;
            this.factory = factory;
            this.sync = new Semaphore(size, size);
            this.loadingMode = loadingMode;
            this.itemStore = this.CreateItemStore(accessMode, size);
            if (loadingMode == LoadingMode.Eager)
            {
                this.PreloadItems();
            }
        }

        private void PreloadItems()
        {
            for (int cntr = 0; cntr < this.size; cntr++)
            {
                TType t = this.factory(this);
                this.itemStore.Store(t);
            }
            this.count = this.size;
        }

        #endregion

        #region acquire
        public TType Acquire()
        {
            TType t;
            this.sync.WaitOne();
            switch (this.loadingMode)
            {
                case LoadingMode.Eager:
                    {
                        t = this.AcquireEager();
                        break;
                    }
                case LoadingMode.Lazy:
                    {
                        t = this.AcquireLazy();
                        break;
                    }
                default:
                    {
                        t = this.AcquireLazyExpanding();
                        break;
                    }
            }
            return t;
        }

        private TType AcquireEager()
        {
            TType t;
            lock (this.itemStore)
            {
                t = this.itemStore.Fetch();
            }
            return t;
        }

        private TType AcquireLazy()
        {
            TType t;
            lock (this.itemStore)
            {
                if (this.itemStore.Count > 0)
                {
                    t = this.itemStore.Fetch();
                    return t;
                }
            }
            Interlocked.Increment(ref this.count);
            t = this.factory(this);
            return t;
        }

        private TType AcquireLazyExpanding()
        {
            TType t;
            bool shouldExpand = false;
            if (this.count < this.size)
            {
                if (Interlocked.Increment(ref this.count) > this.size)
                {
                    // Another thread took the last spot - use the store instead
                    Interlocked.Decrement(ref this.count);
                }
                else
                {
                    shouldExpand = true;
                }
            }
            if (!shouldExpand)
            {
                lock (this.itemStore)
                {
                    t = this.itemStore.Fetch();
                }
            }
            else
            {
                t = this.factory(this);
            }
            return t;
        }

        #endregion

        private IItemStore<TType> CreateItemStore(AccessMode accessMode, int capacity)
        {
            switch (accessMode)
            {
                case AccessMode.FIFO:
                    return new QueueStore<TType>(capacity);
                case AccessMode.LIFO:
                    return new StackStore<TType>(capacity);
                case AccessMode.Circular:
                default:
                    return new CircularStore<TType>(capacity);
            }
        }

        public void Release(TType item)
        {
            lock (this.itemStore)
            {
                this.itemStore.Store(item);
            }
            this.sync.Release();
        }

        public bool IsDisposed
        {
            get
            {
                return this.isDisposed;
            }
        }

        #region IDisposable
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;
                if (typeof(IDisposable).IsAssignableFrom(typeof(TType)))
                {
                    lock (this.itemStore)
                    {
                        while (this.itemStore.Count > 0)
                        {
                            ((IDisposable)(object)this.itemStore.Fetch()).Dispose();
                        }
                    }
                }
                this.sync.Close();
            }
        }
        #endregion
    }
}
