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
        public int rowIdx { get; private set; }         //Value of -1 indicates floor line
        public TileColor color { get; private set; }
        public bool hasFirstPlayerPenalty { get; private set; }    //Value of true indicates the first player tile selected

        public Move(int factoryIndex, int targetRow, TileColor colorToTake, bool hasFirstPlayerPenalty)
        {
            factoryIdx = factoryIndex;
            rowIdx = targetRow;
            color = colorToTake;
            this.hasFirstPlayerPenalty = hasFirstPlayerPenalty;
        }
    }
}
