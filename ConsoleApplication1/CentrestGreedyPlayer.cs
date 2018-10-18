using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    public class CentrestGreedyPlayer : Player
    {
        // Main AI method. Determine which of the legally availible moves the player should take on their current turn.
        public override Move PerformMove(List<Move> availibleMoves)
        {
            var scoredMoves = availibleMoves.Select(move =>
                new KeyValuePair<Move, double>(move,
                    gameManager.ExpectedMoveValue(move, this) + CalculateCentrality(move)))
                .ToList();
            scoredMoves.Sort((a, b) => a.Value.CompareTo(b.Value));

            return scoredMoves[scoredMoves.Count-1].Key;
        }

        // Give a subpoint preference to the central columns
        private double CalculateCentrality(Move move)
        {
            if (move.Color == TileColor.FirstPlayer)
            {
                return 0;
            }
            if (move.RowIdx == -1)
            {
                return 0;
            }

            var column = Wall.ColumnOfTileColor(move.RowIdx, move.Color);
            switch (column)
            {
                case 0:
                    return 0.1;  // Slightly preferred over 4 in hopes of clustering moves in one round to a single column
                case 1:
                    return 0.25; // Slightly preferred over 3 in hopes of clustering moves in one round to a single column
                case 2:
                    return 0.75; // Provides the most future flexibility
                case 3:
                    return 0.2;
                case 4:
                    return 0;
                default:
                    throw new NotImplementedException(column.ToString());
            }
        }

        //Courtesy function for displaying information about match and results.
        public override string DisplayName()
        {
            return "Centrest Greedy AI";
        }
    }
}
