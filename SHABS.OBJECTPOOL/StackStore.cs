using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.OBJECTPOOL
{
    public class StackStore<TType>:Stack<TType>,IItemStore<TType>
    {
        //LIFO
        public StackStore(int capacity):base(capacity)
        {

        }

        public TType Fetch()
        {
            return this.Pop();
        }

        public void Store(TType item)
        {
            this.Push(item);
        }
    }
}
