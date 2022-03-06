using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHABS.COMMON
{
    public abstract class SingletonBase<TType> where TType : class
    {
        #region Members

        /// <summary>
        /// Static instance. Needs to use lambda expression
        /// to construct an instance (since constructor is private).
        /// </summary>
        private static readonly Lazy<TType> sInstance = new Lazy<TType>(() => CreateInstanceOfT());

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance of this singleton.
        /// </summary>
        public static TType Current { get { return sInstance.Value; } }

        #endregion

        #region Methods

        /// <summary>
        /// Creates an instance of T via reflection since T's constructor is expected to be private.
        /// </summary>
        /// <returns></returns>
        private static TType CreateInstanceOfT()
        {
            return Activator.CreateInstance(typeof(TType), true) as TType;
        }

        #endregion
    }
}
