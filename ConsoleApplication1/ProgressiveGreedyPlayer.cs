using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    class ProgressiveGreedyPlayer : Player
    {
        //Main AI method. Determine which of the legally availible moves the player should take on their current turn.
        public override Move PerformMove(List<Move> availibleMoves)
        {
            var scoredMoves = availibleMoves.Select(move => new KeyValuePair<Move, int>(move, gameManager.ExpectedMoveValue(move, this))).ToList();
            scoredMoves.Sort((a, b) => a.Value.CompareTo(b.Value));

            //If there are points to be gained this turn, grab them
            if(scoredMoves[scoredMoves.Count - 1].Value > 0)
                return scoredMoves[scoredMoves.Count - 1].Key;

            //Failing that, pick a move that does as much to fill a Pattern Line as possible
            var tileEfficientMoves = scoredMoves.Select(move => new KeyValuePair<Move, float>(move.Key, TileEfficiency(move.Key))).ToList();
            tileEfficientMoves.Sort((a, b) => a.Value.CompareTo(b.Value));

            return tileEfficientMoves[tileEfficientMoves.Count - 1].Key;
        }

        //Courtesy function for displaying information about match and results.
        public override string DisplayName()
        {
            return "Progressive Greedy AI";
        }
    }
}
