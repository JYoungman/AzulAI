using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    class PureRandom : Player
    {
        //Main AI method. Determine which of the legally availible moves the player should take on their current turn.
        public override Move PerformMove(List<Move> availibleMoves)
        {
            return availibleMoves[gameManager.randNumGen.Next(0, availibleMoves.Count)];
        }

        //Courtesy function for displaying information about match and results.
        public override string DisplayName()
        {
            return "Pure Random AI";
        }
    }
}
