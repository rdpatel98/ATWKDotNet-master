using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.OBJECTPOOL
{
    public interface IItemStore<TType>
    {
        TType Fetch();
        void Store(TType item);
        int Count { get; }
    }
}
