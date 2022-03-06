using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.OBJECTPOOL
{
    public class CircularStore<TType> : IItemStore<TType>
    {
        //roundRobin

        private List<Slot<TType>> slots;
        private int freeSlotCount;
        private int position = -1;

        public CircularStore(int capacity)
        {
            slots = new List<Slot<TType>>(capacity);
        }

        public TType Fetch()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("The buffer is empty.");
            }

            int startPosition = position;
            do
            {
                Advance();
                Slot<TType> slot = slots[position];
                if (!slot.IsInUse)
                {
                    slot.IsInUse = true;
                    --freeSlotCount;
                    return slot.Item;
                }
            } while (startPosition != position);
            throw new InvalidOperationException("No free slots.");
        }

        private void Advance()
        {
            position = (position + 1) % slots.Count;
        }

        public void Store(TType item)
        {
            Slot<TType> slot = slots.Find(s => object.Equals(s.Item, item));
            if (slot == null)
            {
                slot = new Slot<TType>(item);
                slots.Add(slot);
            }
            slot.IsInUse = false;
            ++freeSlotCount;
        }

        public int Count
        {
            get { return freeSlotCount; }
        }
    }
}
