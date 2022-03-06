using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.OBJECTPOOL
{
    public class Slot<TType>
    {
        public Slot(TType item)
        {
            this.Item = item;
        }

        public TType Item { get; private set; }
        public bool IsInUse { get; set; }
    }
}
