using System;
using System.Collections.Generic;

namespace AzulAI
{
    public class PatternLine
    {
        private List<Tile> Slots { get; set; }
        public int Capacity { get; }
        public int Load => Slots.Count;
        public int Availability => Capacity - Load;
        public bool IsFull => Load == Capacity;
        public bool IsEmpty => Load == 0;

        public PatternLine(int capacity)
        {
            Capacity = capacity;
            Slots = new List<Tile>(Capacity);
        }

        public TileColor Color
        {
            get
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException("The pattern line is empty.");
                }

                return Slots[0].color;
            }
        }

        public void Add(Tile tile)
        {
            if (!IsEmpty && tile.color != Color)
            {
                throw new InvalidOperationException("Color mismatch.");
            }
            if (IsFull)
            {
                throw new InvalidOperationException("The line is full.");
            }

            Slots.Add(tile);
        }

        public Tile Peak()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("The pattern line is empty.");
            }

            return Slots[0];
        }

        public IEnumerable<Tile> Clear()
        {
            var copy = new List<Tile>(Slots);
            Slots.Clear();
            return copy;
        }
    }
}
