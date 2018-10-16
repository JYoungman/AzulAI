using System;
using System.Collections.Generic;

namespace AzulAI
{
    public class FloorLine
    {
        public int Capacity { get; }
        private List<Tile> Slots { get; }
        public int Load => Slots.Count;
        public int Availability => Capacity - Load;
        public bool IsFull => Load == Capacity;
        public bool IsEmpty => Load == 0;

        public FloorLine()
        {
            Capacity = 7;
            Slots = new List<Tile>(Capacity);
        }

        public void Add(Tile tile)
        {
            if (IsFull)
            {
                throw new InvalidOperationException("The line is full.");
            }

            Slots.Add(tile);
        }

        public IEnumerable<Tile> Clear()
        {
            var copy = new List<Tile>(Slots);
            Slots.Clear();
            return copy;
        }

        // Determine points docked for placing a tile at a certain location on the penalty row
        public int PenaltyAtIndex(int column)
        {
            int penalty = 1;

            if (column > 1 && column < 5)
            {
                penalty = 2;
            }

            if (column >= 5)
            {
                penalty = 3;
            }

            return penalty;
        }

        // Gets the penalty for the next available space, or 0 if full.
        public int NextPenalty()
        {
            if (IsFull)
            {
                return 0;
            }

            return PenaltyAtIndex(Load);
        }
    }
}
