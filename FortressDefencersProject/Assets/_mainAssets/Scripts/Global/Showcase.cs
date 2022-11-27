using System;
using System.Collections.Generic;

namespace FD.Global
{
    public class Showcase
    {
        public const int EMPTY_SLOT_ID = -1;

        /// <summary>
        /// Units IDs.
        /// 'ShowCase.EMPTY_SLOT_ID' - empty slot
        /// </summary>
        private readonly List<int> _slots;


        public Showcase()
        {
            _slots = new();
        }


        public List<int> Slots => _slots;


        public void Set(IList<int> v)
        {
            SetSize(v.Count);

            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i] != v[i])
                {
                    _slots[i] = v[i];
                }
            }

        }


        public void RemoveAtSlot(int slotID)
        {
            _slots[slotID] = EMPTY_SLOT_ID;
        }


        private void SetSize(int slotsAmount)
        {
            int c = _slots.Count;

            if (slotsAmount == c)
                return;

            int dif = slotsAmount - c;

            if (dif > 0)
            {
                int[] empties = new int[dif];
                Array.Fill(empties, EMPTY_SLOT_ID);
                _slots.AddRange(empties);
            }
            else
            {
                _slots.RemoveRange(slotsAmount, -dif);
            }
        }




    }
}
