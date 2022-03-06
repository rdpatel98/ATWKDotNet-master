using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHABS.COMMON
{
    public interface MyCloneAble<TType>
    {
        TType Clone();
        TType DeepClone();
       
    }
}
