using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace AzulAI
{
    public class Player
    {
        public int score = 0;
        public int penaltyAccruedThisRound = 0;
        public int totalEarnedPenalties = 0;
        public int totalAppliedPenalties = 0; // Possibly lower as you're not allowed to go negative.
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

        //Returns list of rows in which the player can legally place a tile of a given color
        public IEnumerable<int> LegalRowsForColor(TileColor c)
        {
            Debug.Assert(c != TileColor.FirstPlayer);

            for (int row = 0; row < 5; row++)
            {
                bool canMatchInRow = true;

                //Check wall to see if tile of color c is already placed on this row
                if (PatternLines[row].IsEmpty)
                {
                    var col = Wall.ColumnOfTileColor(row, c);
                    if (Wall[row, col] != null)
                    {
                        canMatchInRow = false;
                    }
                }
                //Check if there is space remaining in pattern lines containing tiles of color c
                else if (PatternLines[row].Color == c)
                {
                    canMatchInRow = !PatternLines[row].IsFull;
                }
                //Full rows or empty rows with appropriate wall space cannot recieve tiles of color c
                else
                {
                    canMatchInRow = false;
                }

                if (canMatchInRow)
                {
                    yield return row;
                }
            }
        }
    }
}