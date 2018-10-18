using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    public class Move
    {
        public int FactoryIdx { get; }     //Value of -1 indicates tile pool
        public int RowIdx { get; }         //Value of -1 indicates floor line
        public TileColor Color { get; }
        public int Count { get; } // Count of tiles
        public bool HasFirstPlayerPenalty { get; }    //Value of true indicates this is the first move to pull from the center this round

        public Move(int factoryIndex, int targetRow, TileColor colorToTake, int count, bool hasFirstPlayerPenalty)
        {
            FactoryIdx = factoryIndex;
            RowIdx = targetRow;
            Color = colorToTake;
            Count = count;
            HasFirstPlayerPenalty = hasFirstPlayerPenalty;
        }
    }
}
