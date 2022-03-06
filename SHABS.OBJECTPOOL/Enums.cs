using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.OBJECTPOOL
{
    public enum AccessMode { FIFO, LIFO, Circular };

    public enum LoadingMode { Eager, Lazy, LazyExpanding };
}
