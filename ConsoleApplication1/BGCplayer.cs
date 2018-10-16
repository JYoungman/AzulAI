﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    class BGCplayer : Player
    {
        public override string DisplayName()
        {
            return "Board Games Chronicle";
        }

        public override Move PerformMove(List<Move> availibleMoves)
        {
            //Strategy taken from article: https://theboardgameschronicle.com/2018/04/05/strategies-azul/

            //First priority: Fifth row
            if(PatternLines[4].IsEmpty)
            {
                IEnumerable<Move> lastRowMoves = availibleMoves.Where(x => x.rowIdx == 4);
                int highestCount = 0;
                
                foreach(Move m in lastRowMoves)
                {
                    if (m.factoryIdx > -1)
                    {
                        int tilesGained = gameManager.TilesGainedByMove(m);
                        highestCount = Math.Max(highestCount, tilesGained);

                        //In the rare case of a factory with four availible tiles, take it
                        if (tilesGained == 4)
                            return m;
                    }
                }

                //Attempt to pick a color that we already have on the board to move toward bonus
                List<Move> highValMoves = new List<Move>();

                foreach(Move m in lastRowMoves)
                {
                    if (gameManager.TilesGainedByMove(m) == highestCount)
                        highValMoves.Add(m);
                }
                highValMoves.Sort((x, y) => x.color.CompareTo(y.color));

                List<KeyValuePair<TileColor, int>> placedTiles = gameManager.PlacedTilesByColor(this);

                foreach(KeyValuePair<TileColor, int> kvp in placedTiles)
                {
                    foreach(Move m in highValMoves)
                    {
                        if (m.color == kvp.Key)
                            return m;
                    }
                }
            }
            else
            {
                int tilesNeeded = PatternLines[4].Availability;
                IEnumerable<Move> lastRowMoves = availibleMoves.Where(x => x.rowIdx == 4);
                lastRowMoves = lastRowMoves.Where(x => gameManager.TilesGainedByMove(x) == tilesNeeded);
                if (lastRowMoves.Any())
                {
                    return lastRowMoves.First();
                }
            }

            //Second priority: Fourth row
            if (PatternLines[3].IsEmpty)
            {
                IEnumerable<Move> lastRowMoves = availibleMoves.Where(x => x.rowIdx == 3);
                int highestCount = 0;

                foreach (Move m in lastRowMoves)
                {
                    if (m.factoryIdx > -1)
                    {
                        int tilesGained = gameManager.TilesGainedByMove(m);
                        highestCount = Math.Max(highestCount, tilesGained);

                        //In the rare case of a factory with four availible tiles, take it
                        if (tilesGained == 4)
                            return m;
                    }
                }

                //Attempt to pick a color that we already have on the board to move toward bonus
                List<Move> highValMoves = new List<Move>();

                foreach (Move m in lastRowMoves)
                {
                    if (gameManager.TilesGainedByMove(m) == highestCount)
                        highValMoves.Add(m);
                }
                highValMoves.Sort((x, y) => x.color.CompareTo(y.color));

                List<KeyValuePair<TileColor, int>> placedTiles = gameManager.PlacedTilesByColor(this);

                foreach (KeyValuePair<TileColor, int> kvp in placedTiles)
                {
                    foreach (Move m in highValMoves)
                    {
                        if (m.color == kvp.Key)
                            return m;
                    }
                }
            }
            else
            {
                int tilesNeeded = PatternLines[3].Availability;
                IEnumerable<Move> lastRowMoves = availibleMoves.Where(x => x.rowIdx == 3);
                lastRowMoves = lastRowMoves.Where(x => gameManager.TilesGainedByMove(x) == tilesNeeded);
                if (lastRowMoves.Any())
                {
                    return lastRowMoves.First();
                }
            }

            //Third priority: First row
            if (PatternLines[0].IsEmpty)
            {
                //Get moves that exactly fill row
                IEnumerable<Move> thisRowMoves = availibleMoves.Where(x => x.rowIdx == 0);
                List<Move> rowFillMoves = new List<Move>();
                foreach(Move m in thisRowMoves)
                {
                    if (gameManager.TilesGainedByMove(m) == 1)
                        rowFillMoves.Add(m);
                }

                //Prioritize colors already on the board
                List<KeyValuePair<TileColor, int>> placedTiles = gameManager.PlacedTilesByColor(this);

                foreach (KeyValuePair<TileColor, int> kvp in placedTiles)
                {
                    foreach (Move m in rowFillMoves)
                    {
                        if (m.color == kvp.Key)
                            return m;
                    }
                }
            }

            //Fourth priority: Second row
            if (PatternLines[0].IsEmpty)
            {
                //Get moves that exactly fill row
                IEnumerable<Move> thisRowMoves = availibleMoves.Where(x => x.rowIdx == 1);
                List<Move> rowFillMoves = new List<Move>();
                foreach (Move m in thisRowMoves)
                {
                    if (gameManager.TilesGainedByMove(m) == 2)
                        rowFillMoves.Add(m);
                }

                //Prioritize colors already on the board
                List<KeyValuePair<TileColor, int>> placedTiles = gameManager.PlacedTilesByColor(this);

                foreach (KeyValuePair<TileColor, int> kvp in placedTiles)
                {
                    foreach (Move m in rowFillMoves)
                    {
                        if (m.color == kvp.Key)
                            return m;
                    }
                }
            }

            //Fifth priority: Do something reasonable. In this case, highest expected short term value.
            var scoredMoves = availibleMoves.Select(move => new KeyValuePair<Move, int>(move, gameManager.ExpectedMoveValue(move, this))).ToList();
            scoredMoves.Sort((a, b) => a.Value.CompareTo(b.Value));
            return scoredMoves[scoredMoves.Count - 1].Key;
        }

    }
}
