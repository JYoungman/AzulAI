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
            var scoredMoves = availibleMoves.Select(move => new KeyValuePair<Move, int>(move, gameManager.ExpectedMoveValue(move, this))).ToList();
            scoredMoves.Sort((a, b) => a.Value.CompareTo(b.Value));
            return scoredMoves[scoredMoves.Count-1].Key;
        }

        //Courtesy function for displaying information about match and results.
        public override string DisplayName()
        {
            return "Simple Greedy AI";
        }
    }
}
