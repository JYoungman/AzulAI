using System;
using System.Collections;
using System.Collections.Generic;

namespace AzulAI
{
    public class Player
    {
        public int score = 0;
        public int penaltyAccruedThisRound = 0;
        public int totalEarnedPenalties = 0;
        public int totalAppliedPenalties = 0; // Possibly lower as your not allowed to go negative.
        public bool legalMovesAvailible = true;

        public Wall Wall { get; set; }
        public PatternLine[] PatternLines { get; set; }
        public FloorLine FloorLine { get; set; }

        public GameManager gameManager;

        public Player()
        {
        }

        public void GameSetup()
        {
            score = 0;
            penaltyAccruedThisRound = 0;
            totalAppliedPenalties = 0;
            totalEarnedPenalties = 0;
            legalMovesAvailible = true;

            Wall = new Wall();

            PatternLines = new PatternLine[5];
            for (int i = 0; i < 5; i++)
            {
                PatternLines[i] = new PatternLine(i + 1);
            }

            FloorLine = new FloorLine();
        }
        
        //Main AI method. Determine which of the legally availible moves the player should take on their current turn.
        public virtual Move PerformMove(List<Move> availibleMoves)
        {
            return availibleMoves[0];
        }

        //Courtesy function for displaying information about match and results.
        public virtual string DisplayName()
        {
            return "Default AI";
        }

        // Like Wall.AdjacentTiles but accounts for full Pattern Lines what will get installed this round
        internal int TilesThatWillBeAdjacent(int row, int col)
        {
            // Rows aren't any different, you can only add one item to a row per round.
            var total = Wall.AdjacentRowTiles(row, col);
            
            // Vertical checks, account for full pattern lines
            for (var r = row + 1; r < 5; r++)
            {
                if (Wall[r, col] != null
                    || (PatternLines[r].IsFull && PatternLines[r].Color == Wall.TileKey[r, col]))
                {
                    total++;
                }
                else
                {
                    break;
                }
            }
            for (var r = row - 1; r >= 0; r--)
            {
                if (Wall[r, col] != null
                    || (PatternLines[r].IsFull && PatternLines[r].Color == Wall.TileKey[r, col]))
                {
                    total++;
                }
                else
                {
                    break;
                }
            }

            return total;
        }
    }
}