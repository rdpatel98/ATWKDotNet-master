using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.OBJECTPOOL
{
    public class QueueStore<TType> : Queue<TType>, IItemStore<TType>
    {
        //FIFO
        public QueueStore(int capacity):base(capacity)
        {

        }

        public TType Fetch()
        {
            return this.Dequeue();
        }

        public void Store(TType item)
        {
            this.Enqueue(item);
        }
    }
}
