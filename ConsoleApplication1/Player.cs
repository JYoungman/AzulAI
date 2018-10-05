using System;
using System.Collections;
using System.Collections.Generic;

namespace AzulAI
{
    public class Player
    {
        public int score = 0;
        public int penaltyAccruedThisRound = 0;
        public bool legalMovesAvailible = true;

        public Tile[][] tileGrid { get; set; }
        public Tile[][] tileStores { get; set; }
        public Tile[] penaltyRow { get; set; }

        public GameManager gameManager;

        public Player()
        {
            tileGrid = new Tile[5][];
            for (int i = 0; i < 5; i++ )
            {
                tileGrid[i] = new Tile[5];
            }

            tileStores = new Tile[5][];
            for (int i = 0; i < 5; i++)
            {
                tileStores[i] = new Tile[i+1];
            }

            penaltyRow = new Tile[7];
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

        public int GetNextOpenSpaceInTileStoreRow(int rowIdx)
        {
            for(int i = 0; i < tileStores[rowIdx].Length; i++)
            {
                if (tileStores[rowIdx][i] == null)
                    return i;
            }

            return -1;
        }

        public int GetNextOpenSpaceInPenaltyRow()
        {
            for(int i = 0; i < penaltyRow.Length; i++)
            {
                if (penaltyRow[i] == null)
                    return i;
            }

            return -1;
        }

        //Returns the sum of adjacency chains starting at x, y in tileGrid. Used for scoring.
        public int AdjacentTiles(int x, int y)
        {
            int combo = 0;

            //Horizontal checks
            if(x > 0 && tileGrid[x-1][y] != null)
            {
                combo++;

                if(x > 1 && tileGrid[x-2][y] != null)
                {
                    combo++;
                }
            }

            if(x < 4 && tileGrid[x+1][y] != null)
            {
                combo++;
                
                if (x < 3 && tileGrid[x+2][y] != null)
                {
                    combo++;
                }
            }

            //Vertical checks
            if (y > 0 && tileGrid[x][y - 1] != null)
            {
                combo++;

                if (y > 1 && tileGrid[x][y - 2] != null)
                {
                    combo++;
                }
            }

            if (y < 4 && tileGrid[x][y + 1] != null)
            {
                combo++;

                if (y < 3 && tileGrid[x][y + 2] != null)
                {
                    combo++;
                }
            }

            return combo;
        }

        public int FullColumnCount()
        {
            int fullColumns = 0;

            for (int i = 0; i < 5; i++ )
            {
                bool isFull = true;
                for(int j = 0; j < 5; j++)
                {
                    if(tileGrid[i][j] == null)
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

            for (int j = 0; j < 5; j++)
            {
                bool isFull = true;

                for(int i = 0; i < 5; i++)
                {
                    if(tileGrid[i][j] == null)
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

            for (int i = 0; i < 5; i++ )
            {
                for(int j = 0; j < 5; j++)
                {
                    if(tileGrid[i][j] != null)
                    {
                        switch(tileGrid[i][j].color)
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

        //Returns true if a store row is full, meaning that a tile will be added to the main grid at the end of the round
        public bool TileStoreRowComplete(int rowIdx)
        {
            for (int i = 0; i < tileStores[rowIdx].Length; i++)
            {
                if(tileStores[rowIdx][i] == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}