using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    public class Wall
    {
        public Tile[,] Tiles { get; } = new Tile[5, 5];

        public TileColor[,] TileKey { get; } = new TileColor[5, 5];

        public Wall()
        {            
            TileKey[0, 0] = TileColor.Blue;
            TileKey[0, 1] = TileColor.Yellow;
            TileKey[0, 2] = TileColor.Red;
            TileKey[0, 3] = TileColor.Black;
            TileKey[0, 4] = TileColor.White;

            TileKey[1, 1] = TileColor.Blue;
            TileKey[1, 2] = TileColor.Yellow;
            TileKey[1, 3] = TileColor.Red;
            TileKey[1, 4] = TileColor.Black;
            TileKey[1, 0] = TileColor.White;

            TileKey[2, 2] = TileColor.Blue;
            TileKey[2, 3] = TileColor.Yellow;
            TileKey[2, 4] = TileColor.Red;
            TileKey[2, 0] = TileColor.Black;
            TileKey[2, 1] = TileColor.White;

            TileKey[3, 3] = TileColor.Blue;
            TileKey[3, 4] = TileColor.Yellow;
            TileKey[3, 0] = TileColor.Red;
            TileKey[3, 1] = TileColor.Black;
            TileKey[3, 2] = TileColor.White;

            TileKey[4, 4] = TileColor.Blue;
            TileKey[4, 0] = TileColor.Yellow;
            TileKey[4, 1] = TileColor.Red;
            TileKey[4, 2] = TileColor.Black;
            TileKey[4, 3] = TileColor.White;
        }

        public Tile this[int row, int col]
        {
            get => Tiles[row, col];
            set => Tiles[row, col] = value;
        }

        //Returns the color of tile that gets placed at the provided coordinates
        public TileColor TileColorAtLocation(int row, int col)
        {
            return TileKey[row, col];
        }

        //Returns the column the color belongs in for the given row
        public int ColumnOfTileColor(int row, TileColor color)
        {
            for (int col = 0; col < 5; col++)
            {
                if (TileKey[row, col] == color)
                {
                    return col;
                }
            }
            throw new InvalidOperationException(color.ToString());
        }

        public int FullColumnCount()
        {
            int fullColumns = 0;

            for (int col = 0; col < 5; col++)
            {
                bool isFull = true;
                for (int row = 0; row < 5; row++)
                {
                    if (Tiles[row, col] == null)
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
                    if (Tiles[row, col] == null)
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

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    if (Tiles[row, col] != null)
                    {
                        switch (Tiles[row, col].Color)
                        {
                            case TileColor.Blue:
                                {
                                    colorCounts[0]++;
                                    break;
                                }
                            case TileColor.Yellow:
                                {
                                    colorCounts[1]++;
                                    break;
                                }
                            case TileColor.Red:
                                {
                                    colorCounts[2]++;
                                    break;
                                }
                            case TileColor.White:
                                {
                                    colorCounts[3]++;
                                    break;
                                }
                            case TileColor.Black:
                                {
                                    colorCounts[4]++;
                                    break;
                                }
                        }
                    }
                }
            }

            for (int k = 0; k < 5; k++)
            {
                if (colorCounts[k] == 5)
                    fullSets++;
            }

            return fullSets;
        }

        //Returns the sum of adjacency chains starting at row, col in tileGrid. Used for scoring.
        public int AdjacentTiles(int row, int col) => AdjacentRowTiles(row, col) + AdjacentColTiles(row, col);

        //Returns the sum of adjacency chains starting at row, col in tileGrid. Used for scoring.
        public int AdjacentColTiles(int row, int col)
        {
            int combo = 0;

            //Vertical checks
            for (var r = row + 1; r < 5; r++)
            {
                if (Tiles[r, col] != null)
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
                if (Tiles[r, col] != null)
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

        //Returns the sum of adjacency chains starting at row, col in tileGrid. Used for scoring.
        public int AdjacentRowTiles(int row, int col)
        {
            int combo = 0;

            //Horizontal checks
            for (var c = col + 1; c < 5; c++)
            {
                if (Tiles[row, c] != null)
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
                if (Tiles[row, c] != null)
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

        //Returns count of each tile color play has placed on their wall, in descending order
        public List<KeyValuePair<TileColor, int>> PlacedTilesByColor()
        {
            List<KeyValuePair<TileColor, int>> colorCounts = new List<KeyValuePair<TileColor, int>>();

            int blackCount = 0;
            int blueCount = 0;
            int redCount = 0;
            int whiteCount = 0;
            int yellowCount = 0;

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    if (Tiles[row, col] != null)
                    {
                        switch (Tiles[row, col].Color)
                        {
                            case TileColor.Black:
                                {
                                    blackCount++;
                                    break;
                                }
                            case TileColor.Blue:
                                {
                                    blueCount++;
                                    break;
                                }
                            case TileColor.Red:
                                {
                                    redCount++;
                                    break;
                                }
                            case TileColor.White:
                                {
                                    whiteCount++;
                                    break;
                                }
                            case TileColor.Yellow:
                                {
                                    yellowCount++;
                                    break;
                                }
                        }
                    }
                }
            }

            colorCounts.Add(new KeyValuePair<TileColor, int>(TileColor.Black, blackCount));
            colorCounts.Add(new KeyValuePair<TileColor, int>(TileColor.Blue, blueCount));
            colorCounts.Add(new KeyValuePair<TileColor, int>(TileColor.Red, redCount));
            colorCounts.Add(new KeyValuePair<TileColor, int>(TileColor.White, whiteCount));
            colorCounts.Add(new KeyValuePair<TileColor, int>(TileColor.Yellow, yellowCount));

            colorCounts.Sort((x, y) => x.Value.CompareTo(y.Value));
            colorCounts.Reverse();

            return colorCounts;
        }
    }
}
