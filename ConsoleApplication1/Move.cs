using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    public class Move
    {
        public int factoryIdx { get; private set; }     //Value of -1 indicates tile pool
        public int rowIdx { get; private set; }         //Value of -1 indicates penalty row
        public TileColor color { get; private set; }
        public bool hasPenalty { get; private set; }    //Value of true indicates penalty tile selected

        public Move(int factoryIndex, int targetRow, TileColor colorToTake, bool penalty)
        {
            factoryIdx = factoryIndex;
            rowIdx = targetRow;
            color = colorToTake;
            hasPenalty = penalty;
        }
    }
}
