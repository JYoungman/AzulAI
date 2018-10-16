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

        public Tile[,] TileGrid { get; set; }
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

            TileGrid = new Tile[5, 5];

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

        //Returns the sum of adjacency chains starting at row, col in tileGrid. Used for scoring.
        public int AdjacentTiles(int row, int col)
        {
            int combo = 0;

            //Vertical checks
            for (var r = row + 1; r < 5; r++)
            {
                if (TileGrid[r, col] != null)
                {
                    combo++;
                }
                else
                {
                    break;
                }
            }
            for (var r = row - 1; r >= 0; r--)
            {
                if (TileGrid[r, col] != null)
                {
                    combo++;
                }
                else
                {
                    break;
                }
            }

            //Horizontal checks
            for (var c = col + 1; c < 5; c++)
            {
                if (TileGrid[row, c] != null)
                {
                    combo++;
                }
                else
                {
                    break;
                }
            }
            for (var c = col - 1; c >= 0; c--)
            {
                if (TileGrid[row, c] != null)
                {
                    combo++;
                }
                else
                {
                    break;
                }
            }

            return combo;
        }

        public int FullColumnCount()
        {
            int fullColumns = 0;

            for (int col = 0; col < 5; col++)
            {
                bool isFull = true;
                for (int row = 0; row < 5; row++ )
                {
                    if(TileGrid[row, col] == null)
                    {
                        isFull = false;
                        break;
                    }
                }

                if (isFull)
                    fullColumns++;
            }

            return fullColumns;
        }

        public int FullRowCount()
        {
            int fullRows = 0;

            for (int row = 0; row < 5; row++)
            {
                bool isFull = true;

                for (int col = 0; col < 5; col++)
                {
                    if(TileGrid[row, col] == null)
                    {
                        isFull = false;
                        break;
                    }
                }

                if (isFull)
                    fullRows++;
            }

                return fullRows;
        }

        public int FullSetCount()
        {
            int fullSets = 0;
            int[] colorCounts = new int[5];

            for (int row = 0; row < 5; row++ )
            {
                for(int col = 0; col < 5; col++)
                {
                    if(TileGrid[row, col] != null)
                    {
                        switch(TileGrid[row, col].color)
                        {
                            case TileColor.blue:
                                {
                                    colorCounts[0]++;
                                    break;
                                }
                            case TileColor.yellow:
                                {
                                    colorCounts[1]++;
                                    break;
                                }
                            case TileColor.red:
                                {
                                    colorCounts[2]++;
                                    break;
                                }
                            case TileColor.white:
                                {
                                    colorCounts[3]++;
                                    break;
                                }
                            case TileColor.black:
                                {
                                    colorCounts[4]++;
                                    break;
                                }
                        }
                    }
                }
            }

            for (int k = 0; k < 5; k++ )
            {
                if (colorCounts[k] == 5)
                    fullSets++;
            }

            return fullSets;
        }
    }
}