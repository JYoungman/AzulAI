using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    public class SimpleGreedyPlayer : Player
    {
        //Main AI method. Determine which of the legally availible moves the player should take on their current turn.
        public override Move PerformMove(List<Move> availibleMoves)
        {
            availibleMoves.Sort((a, b) => gameManager.ExpectedMoveValue(a, this).CompareTo(gameManager.ExpectedMoveValue(b, this)));
            return availibleMoves[availibleMoves.Count-1];
        }

        //Courtesy function for displaying information about match and results.
        public override string DisplayName()
        {
            return "Simple Greedy AI";
        }
    }
}
