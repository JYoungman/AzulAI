using System;
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
            if(GetNextOpenSpaceInTileStoreRow(4) == 0)
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

            //Second priority: Fourth row
            if (GetNextOpenSpaceInTileStoreRow(3) == 0)
            {
                IEnumerable<Move> lastRowMoves = availibleMoves.Where(x => x.rowIdx == 4);
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

            //Third priority: First row
            if (GetNextOpenSpaceInTileStoreRow(0) == 0)
            {
                //Get moves that exactly fill row
                List<Move> rowFillMoves = new List<Move>();
                foreach(Move m in availibleMoves)
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
            if (GetNextOpenSpaceInTileStoreRow(1) == 0)
            {
                //Get moves that exactly fill row
                List<Move> rowFillMoves = new List<Move>();
                foreach (Move m in availibleMoves)
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
            availibleMoves.Sort((a, b) => gameManager.ExpectedMoveValue(a, this).CompareTo(gameManager.ExpectedMoveValue(b, this)));
            return availibleMoves[0];
        }

    }
}
