using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    class BonusGreedyPlayer : Player
    {
        //Main AI method. Determine which of the legally availible moves the player should take on their current turn.
        public override Move PerformMove(List<Move> availibleMoves)
        {
            //Isolate the moves with the highest immediate value
            var scoredMoves = availibleMoves.Select(move => new KeyValuePair<Move, int>(move, gameManager.ExpectedMoveValue(move, this))).ToList();
            scoredMoves.Sort((a, b) => a.Value.CompareTo(b.Value));
            int maxVal = scoredMoves[scoredMoves.Count - 1].Value;
            scoredMoves = scoredMoves.Where(x => x.Value == maxVal).ToList<KeyValuePair<Move, int>>();

            //Prefer moves that bring us closer to finishing color combos
            List<KeyValuePair<TileColor, int>> placedTiles = Wall.PlacedTilesByColor();

            //Remove empty and full color combos
            placedTiles = placedTiles.Where(x => x.Value != 0).ToList<KeyValuePair<TileColor, int>>();
            placedTiles = placedTiles.Where(x => x.Value != 5).ToList<KeyValuePair<TileColor, int>>();

            //If there are tiles on the wall, find the most plentiful color on the wall
            if (placedTiles.Count() != 0)
            {
                placedTiles.Sort((a, b) => a.Value.CompareTo(b.Value));
                int maxColors = placedTiles[placedTiles.Count - 1].Value;
                placedTiles = placedTiles.Where(x => x.Value == maxColors).ToList<KeyValuePair<TileColor, int>>();
                placedTiles.Reverse();

                //Attempt to find and perform a move with the most common incomplete color
                for(int idx = 0; idx < placedTiles.Count; idx++)
                {
                    foreach(KeyValuePair<Move, int> kvp in scoredMoves)
                    {
                        if (placedTiles[idx].Key == kvp.Key.Color)
                            return kvp.Key;
                    }
                }
            }

            //If color combos cannot be advanced, try to advance a column combo
            List<KeyValuePair<int, int>> columnCompletion = new List<KeyValuePair<int, int>>();
            for (int col = 0; col < 5; col++)
            {
                int tilesInColumn = 0;
                for (int row = 0; row < 5; row++)
                {
                    if (Wall[row, col] != null)
                        tilesInColumn++;
                }
                columnCompletion.Add(new KeyValuePair<int, int>(col, tilesInColumn));
            }

            //We only care about columns with some tiles, but that aren't full
            columnCompletion = columnCompletion.Where(pair => pair.Value != 0).ToList<KeyValuePair<int, int>>();
            columnCompletion = columnCompletion.Where(pair => pair.Value != 5).ToList<KeyValuePair<int, int>>();

            //Attempt to find and execute a move that fills the most full column
            if (columnCompletion.Count != 0)
            {
                columnCompletion.Sort((a, b) => a.Value.CompareTo(b.Value));
                columnCompletion.Reverse();

                for(int idx = 0; idx < columnCompletion.Count; idx++)
                {
                    var columnMoves = scoredMoves.Where(x => x.Key.RowIdx >= 0);
                    columnMoves = columnMoves.Where(x => Wall.ColumnOfTileColor(x.Key.RowIdx, x.Key.Color) == columnCompletion[idx].Key);
                    if (columnMoves.Count() != 0)
                        return columnMoves.First().Key;
                }
                
            }

            //If column combos cannot be advanced, try to advance a row combo
            List<KeyValuePair<int, int>> rowCompletion = new List<KeyValuePair<int, int>>();
            for (int row = 0; row < 5; row++)
            {
                int tilesInRow = 0;
                for (int col = 0; col < 5; col++)
                {
                    if (Wall[row, col] != null)
                    {
                        tilesInRow++;
                    }
                }
                rowCompletion.Add(new KeyValuePair<int, int>(row, tilesInRow));
            }

            rowCompletion = rowCompletion.Where(x => x.Value != 0).ToList<KeyValuePair<int, int>>();
            rowCompletion = rowCompletion.Where(x => x.Value != 5).ToList<KeyValuePair<int, int>>();

            //Attempt to find and execute a move that fills the most full row
            if (rowCompletion.Count != 0)
            {
                rowCompletion.Sort((a, b) => a.Value.CompareTo(b.Value));
                rowCompletion.Reverse();

                for (int idx = 0; idx < rowCompletion.Count; idx++)
                {
                    var rowMoves = scoredMoves.Where(x => x.Key.RowIdx == rowCompletion[idx].Key);
                    if (rowMoves.Count() != 0)
                        return rowMoves.First().Key;
                }
            }

            //If no combos can be advanced, pick an arbitrary high value move
            return scoredMoves[0].Key;
        }

        //Courtesy function for displaying information about match and results.
        public override string DisplayName()
        {
            return "Bonus Greedy AI";
        }
    }
}
