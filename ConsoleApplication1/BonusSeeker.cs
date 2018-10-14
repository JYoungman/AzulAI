﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    class BonusSeeker : Player
    {
        public override Move PerformMove(List<Move> availibleMoves)
        {
            //First priority: Color sets
            List<KeyValuePair<TileColor, int>> placedTiles = gameManager.PlacedTilesByColor(this);

            //We aren't worried about colors we have already filled or have none of
            IEnumerable <KeyValuePair<TileColor, int>> comboColors = placedTiles.Where(x => x.Value != 0);
            comboColors = comboColors.Where(x => x.Value != 5);

            if(comboColors.Count() != 0)
            {
                foreach (KeyValuePair<TileColor, int> kvp in comboColors)
                {
                    List<Move> comboMoves = availibleMoves.Where(x => x.color == kvp.Key).ToList<Move>();
                    if (comboMoves.Count > 0)
                    {
                        comboMoves.Sort((a, b) => gameManager.ExpectedMoveValue(a, this).CompareTo(gameManager.ExpectedMoveValue(b, this)));
                        return comboMoves[comboMoves.Count-1];
                    }
                }
            }

            //Second priority: Columns
            List<KeyValuePair<int, int>> columnCompletion = new List<KeyValuePair<int, int>>();
            for(int i = 0; i < 5; i++)
            {
                int tilesInColumn = 0;
                for(int y = 0; y < 5; y++)
                {
                    if (tileGrid[i][y] != null)
                        tilesInColumn++;
                }
                columnCompletion.Add(new KeyValuePair<int, int>(i, tilesInColumn));
            }

            IEnumerable<KeyValuePair<int, int>> completeColumns = columnCompletion.Where(x => x.Value != 0);

            if(completeColumns.Count() != 0)
            {
                List<Move> columnMoves = new List<Move>();
                foreach (KeyValuePair<int, int> kvp in completeColumns)
                {
                    //Determine open tiles in partially complete columns
                    List<KeyValuePair<TileColor,int>> availibleColors = new List<KeyValuePair<TileColor, int>>();
                    for(int y = 0; y < 5; y++)
                    {
                        if(tileGrid[kvp.Key][y] == null)
                        {
                            availibleColors.Add(new KeyValuePair<TileColor,int>(gameManager.TileColorAtLocation(kvp.Key, y), y));
                        }
                    }

                    //Determine moves that help fill partially complete columns
                    foreach(KeyValuePair<TileColor, int> desiredLocation in availibleColors)
                    {
                        foreach(Move m in availibleMoves)
                        {
                            if (m.color == desiredLocation.Key && m.rowIdx == desiredLocation.Value)
                                columnMoves.Add(m);
                        }
                    }
                }

                //Determine and execute most valuable move
                if (columnMoves.Count > 0)
                {
                    columnMoves.Sort((a, b) => gameManager.ExpectedMoveValue(a, this).CompareTo(gameManager.ExpectedMoveValue(b, this)));
                    return columnMoves[columnMoves.Count-1];
                }
            }

            //Third priority: Rows. Note that these combos will only be attempted explicitly during the last round
            if(gameManager.IsLastRound())
            {
                //Find partially complete rows
                List<KeyValuePair<int, int>> openRows = new List<KeyValuePair<int, int>>();
                for(int i = 0; i < 5; i++)
                {
                    int openSlots = 0;
                    for(int x = 0; x < 5; x++)
                    {
                        if(tileGrid[x][i] == null)
                        {
                            openSlots++;
                        }
                    }
                    openRows.Add(new KeyValuePair<int, int>(i, openSlots));
                }

                openRows = openRows.Where(x => x.Value != 0).ToList<KeyValuePair<int, int>>();
                openRows = openRows.Where(x => x.Value != 5).ToList<KeyValuePair<int, int>>();

                //Try to fill partially completed rows
                if (openRows.Count != 0)
                {
                    openRows.Sort((x, y) => x.Value.CompareTo(y.Value));
                    List<Move> rowMoves = new List<Move>();

                    foreach(KeyValuePair<int, int> kvp in openRows)
                    {
                        foreach(Move m in availibleMoves)
                        {
                            if (m.rowIdx == kvp.Key)
                                rowMoves.Add(m);
                        }
                    }

                    //Determine and execute the most valuable move
                    if (rowMoves.Count > 0)
                    {
                        rowMoves.Sort((a, b) => gameManager.ExpectedMoveValue(a, this).CompareTo(gameManager.ExpectedMoveValue(b, this)));
                        return rowMoves[rowMoves.Count-1];
                    }
                }

            }

            //Otherwise, do something reasonable.
            availibleMoves.Sort((a, b) => gameManager.ExpectedMoveValue(a, this).CompareTo(gameManager.ExpectedMoveValue(b, this)));
            return availibleMoves[availibleMoves.Count-1];
        }

        public override string DisplayName()
        {
            return "Bonus Seeker AI";
        }
    }
}