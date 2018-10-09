using System;
using System.Collections;
using System.Collections.Generic;

namespace AzulAI
{
    public class GameManager
    {
        List<Player> players;
        Player startingPlayer;

        List<Tile> bag;
        List<Tile> box;
        List<Tile> pool;

        Tile[][] tileKey;

        List<Factory> factories;

        bool lastRound = false;
        uint roundCount = 0;
        bool movesAvailible = true;

        public Random randNumGen;

        bool verbose = false;

        public GameManager(List<Player> playerList)
        {
            //Allocate lists
            bag = new List<Tile>();
            box = new List<Tile>();
            pool = new List<Tile>();
            factories = new List<Factory>();

            //Generate tiles
            for (int i = 0; i < 20; i++)
            {
                bag.Add(new Tile(TileColor.blue));
                bag.Add(new Tile(TileColor.red));
                bag.Add(new Tile(TileColor.white));
                bag.Add(new Tile(TileColor.yellow));
                bag.Add(new Tile(TileColor.black));
            }

            pool.Add(new Tile(TileColor.penalty));

            players = playerList;
            startingPlayer = players[0];

            //Generate Factories
            int factoryCount = 5;
            if (players.Count == 3)
                factoryCount = 7;
            else if (players.Count == 4)
                factoryCount = 9;

            for (int i = 0; i < factoryCount; i++ )
            {
                factories.Add(new Factory());
            }

            //Generate tile key
            tileKey = new Tile[5][];
            for (int i = 0; i < 5; i++ )
            {
                tileKey[i] = new Tile[5];
                switch (i)
                {
                    case 0:
                        {
                            tileKey[i][0] = new Tile(TileColor.blue);
                            tileKey[i][1] = new Tile(TileColor.yellow);
                            tileKey[i][2] = new Tile(TileColor.red);
                            tileKey[i][3] = new Tile(TileColor.black);
                            tileKey[i][4] = new Tile(TileColor.white);
                            break;
                        }
                    case 1:
                        {
                            tileKey[i][1] = new Tile(TileColor.blue);
                            tileKey[i][2] = new Tile(TileColor.yellow);
                            tileKey[i][3] = new Tile(TileColor.red);
                            tileKey[i][4] = new Tile(TileColor.black);
                            tileKey[i][0] = new Tile(TileColor.white);
                            break;
                        }
                    case 2:
                        {
                            tileKey[i][2] = new Tile(TileColor.blue);
                            tileKey[i][3] = new Tile(TileColor.yellow);
                            tileKey[i][4] = new Tile(TileColor.red);
                            tileKey[i][0] = new Tile(TileColor.black);
                            tileKey[i][1] = new Tile(TileColor.white);
                            break;
                        }
                    case 3:
                        {
                            tileKey[i][3] = new Tile(TileColor.blue);
                            tileKey[i][4] = new Tile(TileColor.yellow);
                            tileKey[i][0] = new Tile(TileColor.red);
                            tileKey[i][1] = new Tile(TileColor.black);
                            tileKey[i][2] = new Tile(TileColor.white);
                            break;
                        }
                    case 4:
                        {
                            tileKey[i][4] = new Tile(TileColor.blue);
                            tileKey[i][0] = new Tile(TileColor.yellow);
                            tileKey[i][1] = new Tile(TileColor.red);
                            tileKey[i][2] = new Tile(TileColor.black);
                            tileKey[i][3] = new Tile(TileColor.white);
                            break;
                        }
                }

            }

            //Provide all players with a link
            foreach(Player p in playerList)
            {
                p.gameManager = this;
                p.GameSetup();
            }

            //Create random number generator
            randNumGen = new Random();

            int bagCount = bag.Count;
        }

        //-----------------------------------------------------------------
        //              Core Gameplay Functions
        //-----------------------------------------------------------------

        //Main game loop. Returns the outcome of the game.
        public GameResults PlayGame()
        {
            while(!lastRound)
            {
                GameRound();
            }
            DisplayText("Game over");
            GameResults results = new GameResults(players, GetWinningPlayers(), roundCount);
            return results;
        }

        //Play a round of the game. The game is a series of these. Should probably be several, smaller functions.
        void GameRound()
        {
            //Reset tiles
            if (roundCount != 0)
            {
                pool.Clear();
                //Dump excess factory tiles into box
                foreach(Factory f in factories)
                {
                    for(int i = 0; i < 4; i++)
                    {
                        if(f.tiles[i] != null)
                        {
                            box.Add(f.tiles[i]);
                            f.tiles[i] = null;
                        }
                    }
                }
            }

            if (box.Count != 0)
            {
                foreach (Tile t in box)
                {
                    if (t.color != TileColor.penalty)
                    {
                        bag.Add(t);
                    }
                    else
                    {
                        pool.Add(t);
                    }
                }
                box.Clear();
            }

            //Reset factories
            foreach (Factory f in factories)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (bag.Count > 0)
                    {
                        int randIdx = randNumGen.Next(0, bag.Count);
                        f.tiles[i] = bag[randIdx];
                        bag.RemoveAt(randIdx);
                        f.availible = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //Update turn order
            if(players[0] != startingPlayer)
            {
                int startingPlayerIdx = 0;
                for(int i = 0; i < players.Count; i++)
                {
                    if(players[i] == startingPlayer)
                    {
                        startingPlayerIdx = i;
                        break;
                    }
                }

                List<Player> temp = players.GetRange(startingPlayerIdx, players.Count - startingPlayerIdx);
                foreach(Player p in players)
                {
                    if(!temp.Contains(p))
                    {
                        temp.Add(p);
                    }
                }

                players = temp;
            }

            //Test for edge case when no legal moves are possible at the start of the round
            foreach(Player p in players)
            {
                List<Move> moveCheck = GenerateLegalMoves(p);
                if(moveCheck.Count > 0)
                {
                    p.legalMovesAvailible = true;
                    movesAvailible = true;
                }
            }

            //If no player has legal moves, end the game
            if(movesAvailible == false)
            {
                lastRound = true;
            }

            //Players take their turns until no legal moves remain
            while (movesAvailible)
            {
                foreach (Player p in players)
                {
                    if (p.legalMovesAvailible)
                    {
                        List<Move> legalMoves = GenerateLegalMoves(p);
                        
                        if (legalMoves.Count > 0)
                        {
                            ExecuteMove(p.PerformMove(legalMoves), p);
                            //Check for game end condition
                            lastRound = MoveEndsGame(p);

                        }
                        //If a player doesn't have legal moves availible, check to see if the round is over
                        else
                        {
                            p.legalMovesAvailible = false;
                            movesAvailible = false;
                            foreach (Player pCheck in players)
                            {
                                if (pCheck.legalMovesAvailible == true)
                                {
                                    movesAvailible = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //Score round
            foreach(Player p in players)
            {
                int pointsEarned = 0;

                //Move completed rows to grid
                for (int y = 0; y < 5; y++ )
                {
                    if(p.TileStoreRowComplete(y))
                    {
                        pointsEarned++;

                       for(int x = 0; x < 5; x++)
                        {
                            if(this.tileKey[x][y].color == p.tileStores[y][0].color)
                            {
                                p.tileGrid[x][y] = p.tileStores[y][0];
                                p.tileStores[y][0] = null;

                                //Return excess tiles to the box
                                for (int i = 1; i < p.tileStores[y].Length; i++ )
                                {
                                    box.Add(p.tileStores[y][i]);
                                    p.tileStores[y][i] = null;
                                }

                                //Check for adjacency bonuses
                                pointsEarned += p.AdjacentTiles(x, y);
                                break;
                            }
                       }
                    }
                }
                p.score += pointsEarned;

                //Deduct penalties, not going below zero
                p.score -= p.penaltyAccruedThisRound;
                p.score = Math.Max(p.score, 0);
                p.penaltyAccruedThisRound = 0;

                //Combo bonuses assigned on last round
                if(lastRound)
                {
                    int bonus = 0;
                    bonus += p.FullColumnCount() * 7;
                    bonus += p.FullRowCount() * 2;
                    bonus += p.FullSetCount() * 10;
                    p.score += bonus;
                }

                //DisplayText(p.DisplayName() + " score: " + p.score+"\n");
            }

            //Housekeeping
            roundCount++;
            //DisplayText(roundCount.ToString()+"\n");
        }

        //Returns a list of all of the moves that the player can legally perform this turn
        public List<Move> GenerateLegalMoves(Player activePlayer)
        {
            List<Move> legalMoves = new List<Move>();

            //Iterate through Factories, creating moves for each row its colors can be placed in
            for (int f = 0; f < factories.Count; f++)
            {
                if (factories[f].availible == true)
                {
                    //Generate list of availible colors
                    List<TileColor> availibleColors = new List<TileColor>();
                    for (int i = 0; i < 4; i++)
                    {
                        if (factories[f].tiles[i] != null)
                        {
                            bool addColor = true;
                            foreach (TileColor tc in availibleColors)
                            {
                                if (tc == factories[f].tiles[i].color)
                                {
                                    addColor = false;
                                }
                            }

                            if (addColor == true)
                            {
                                availibleColors.Add(factories[f].tiles[i].color);
                            }
                        }
                    }

                    //Look for legal places to place tiles of availible colors
                    foreach (TileColor tc in availibleColors)
                    {
                        List<int> availibleRows = LegalRowsForColor(tc, activePlayer);

                        //Generate move data
                        foreach (int row in availibleRows)
                        {
                            legalMoves.Add(new Move(f, row, tc, false));
                        }

                        //Generate moves for moving directly to penalty row
                        int tileCount = 0;
                        for (int i = 0; i < factories[f].tiles.Length; i++ )
                        {
                            if(factories[f].tiles[i] != null)
                            {
                                if (factories[f].tiles[i].color == tc)
                                    tileCount++;
                            }
                        }

                        if (activePlayer.GetNextOpenSpaceInPenaltyRow() >= tileCount)
                        {
                            legalMoves.Add(new Move(f, -1, tc, false));
                        }
                    }
                }
            }

            //Create moves for pulling from the pool
            bool hasPenalty = false;
            
            //Generate availible color list
            List<TileColor> colorsInPool = new List<TileColor>();
            foreach(Tile t in pool)
            {
                if (t != null)
                {
                    bool colorFound = false;
                    foreach (TileColor tc in colorsInPool)
                    {
                        if (tc == t.color)
                        {
                            colorFound = true;
                            break;
                        }
                    }

                    if (colorFound == false)
                    {
                        colorsInPool.Add(t.color);
                        //Flag if penalty tile is still in place
                        if (t.color == TileColor.penalty)
                        {
                            hasPenalty = true;
                        }
                    }
                }
            }

            //Look for legal places to place tiles of availible colors
            foreach(TileColor tc in colorsInPool)
            {
                List<int> availibleRows = LegalRowsForColor(tc, activePlayer);

                //Generate move data
                foreach (int row in availibleRows)
                {
                    legalMoves.Add(new Move(-1, row, tc, hasPenalty));
                }

                //Generate moves for moving directly to penalty row
                int tileCount = 0;
                foreach(Tile poolTile in pool)
                {
                    if (poolTile != null && poolTile.color == tc)
                        tileCount++;
                }

                if (activePlayer.GetNextOpenSpaceInPenaltyRow() >= tileCount)
                {
                    legalMoves.Add(new Move(-1, -1, tc, hasPenalty));
                }
            }

            //DisplayText("Legal moves: "+legalMoves.Count.ToString()+"\n");
            return legalMoves;
        }

        void ExecuteMove(Move move, Player activePlayer)
        {
            //Positive values represents a Factory to pull from
            if (move.factoryIdx >= 0)
            {
                factories[move.factoryIdx].availible = false;

                for (int i = 0; i < 4; i++)
                {
                    //Assign requested tiles to the appropriate row on the player's board
                    if (factories[move.factoryIdx].tiles[i] != null && factories[move.factoryIdx].tiles[i].color == move.color)
                    {
                        if (move.rowIdx >= 0)
                        {
                            int rowSlot = activePlayer.GetNextOpenSpaceInTileStoreRow(move.rowIdx);

                            if (rowSlot >= 0)
                            {
                                activePlayer.tileStores[move.rowIdx][rowSlot] = factories[move.factoryIdx].tiles[i];
                            }
                            //If there isn't room in the desired row, move remaining tiles to the penalty row
                            else
                            {
                                TileToPenaltyRow(factories[move.factoryIdx].tiles[i], activePlayer);
                            }
                        }
                        //Negative row index in move indicates penalty row as target row
                        else
                        {
                            TileToPenaltyRow(factories[move.factoryIdx].tiles[i], activePlayer);
                        }
                    }
                    //Move remaining tiles into the pool
                    else
                    {
                        pool.Add(factories[move.factoryIdx].tiles[i]);
                    }
                }

                
                //Empty factory
                for (int j = 0; j < 4; j++)
                    factories[move.factoryIdx].tiles[j] = null;
            }
            //A negative factoryIdx value indicates player has chosen to pull from the pool
            else
            {
                //Take the penalty tile if present
                if (move.hasPenalty)
                {
                    activePlayer.penaltyAccruedThisRound += 1;
                    TileToPenaltyRow(pool.Find(FindPenaltyTile), activePlayer);
                    pool.Remove(pool.Find(FindPenaltyTile));
                    startingPlayer = activePlayer;
                }

                //Place the selected tiles in the indicated row
                List<Tile> toRemove = new List<Tile>();
                foreach (Tile t in pool)
                {
                    if (t != null && t.color == move.color)
                    {
                        if (move.rowIdx >= 0)
                        {
                            int rowSlot = activePlayer.GetNextOpenSpaceInTileStoreRow(move.rowIdx);

                            if (rowSlot >= 0)
                            {
                                activePlayer.tileStores[move.rowIdx][rowSlot] = t;
                            }
                            //If there isn't room in the desired row, move remaining tiles to the penalty row
                            else
                            {
                                TileToPenaltyRow(t, activePlayer);
                            }
                        }
                        //Negative row index in move indicates penalty row as target row
                        else
                        {
                            TileToPenaltyRow(t, activePlayer);
                        }

                        //Remove the active tile from the pool
                        toRemove.Add(t);
                        //pool.Remove(t);
                    }
                }
                foreach(Tile t in toRemove)
                {
                    if (t != null)
                    {
                        foreach (Tile pt in pool)
                        {
                            if (pt != null && t.color == pt.color)
                            {
                                pool.Remove(pt);
                                break;
                            }
                        }
                    }
                }
            }

        }

        //-----------------------------------------------------------------
        //              Helper Functions
        //-----------------------------------------------------------------

        //Returns true if the player has made a move that will end the game this round
        bool MoveEndsGame(Player p)
        {
            for (int j = 0; j < 5; j++)
            {
                int fullTiles = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (p.tileGrid[i][j] != null)
                    {
                        fullTiles++;
                    }
                }

                //Rows with four filled tiles are candidates for becoming full rows this game round
                if (fullTiles == 4)
                {
                    bool willFill = true;

                    for (int k = 0; k < p.tileStores[j].Length; k++)
                    {
                        if (p.tileStores[j][k] == null)
                        {
                            willFill = false;
                            break;
                        }
                    }

                    //If tile store row of a grid row with four filled spaces is filled, that row in the tile grid will fill at the end of the round
                    if (willFill == true)
                        return true;
                }
            }

            return false;
        }

        //Returns list of rows in which the player can legally place a tile
        List<int> LegalRowsForColor(TileColor c, Player p)
        {
            List<int> rows = new List<int>();

            for (int j = 0; j < 5; j++ )
            {
                bool canMatchInRow = true;

                //Check grid to see if tile of color c is already placed on this row
                if(p.tileStores[j][0] == null)
                {
                    for(int i = 0; i < 5; i++)
                    {
                        if(p.tileGrid[i][j] != null && p.tileGrid[i][j].color == c)
                        {
                            canMatchInRow = false;
                            break;
                        }
                    }
                }
                //Check if there is space remaining in stores containing tiles of color c
                else if(p.tileStores[j][0].color == c)
                {
                    bool gapFound = false;
                    for(int i = 0; i < p.tileStores[j].Length; i++)
                    {
                        if(p.tileStores[j][i] == null)
                        {
                            gapFound = true;
                            break;
                        }
                    }

                    canMatchInRow = gapFound;
                }
                //Full rows or empty rows with appropriate grid space cannot recieve tiles of color c
                else
                {
                    canMatchInRow = false;
                }

                if(canMatchInRow == true)
                {
                    rows.Add(j);
                }
            }
            
            return rows;
        }

        void DisplayText(string str)
        {
            if (verbose)
                Console.Write(str);
        }

        //Debugging function
        int TilesInGame()
        {
            int knownTiles = 0;

            //Check factories
            foreach(Factory f in factories)
            {
                for(int j = 0; j < 4; j++)
                {
                    if (f.tiles[j] != null)
                        knownTiles++;
                }
            }

            //Check bag
            knownTiles += bag.Count;

            //Check pool
            knownTiles += pool.Count;

            //Check box
            knownTiles += box.Count;

            //Check player boards
            foreach(Player p in players)
            {
                for(int x = 0; x < 5; x++)
                {
                    for(int y = 0; y < 5; y++)
                    {
                        if (p.tileGrid[x][y] != null)
                            knownTiles++;
                    }
                }

                for(int i = 0; i < 5; i++)
                {
                    for(int j = 0; j < p.tileStores[i].Length; j++)
                    {
                        if (p.tileStores[i][j] != null)
                            knownTiles++;
                    }
                }

                for(int k = 0; k < p.penaltyRow.Length; k++)
                {
                    if (p.penaltyRow[k] != null)
                        knownTiles++;
                }
            }

            return knownTiles;
        }

        //-----------------------------------------------------------------
        //              Support Functions
        //-----------------------------------------------------------------

        //Returns players with the highest score
        public List<Player> GetWinningPlayers()
        {
            int curHighScore = 0;

            foreach (Player p in players)
            {
                if (p.score > curHighScore)
                    curHighScore = p.score;
            }

            List<Player> winners = new List<Player>();
            List<int> horCount = new List<int>();

            foreach (Player p in players)
            {
                if (p.score == curHighScore)
                    winners.Add(p);
            }

            //Attempt to break ties
            if(winners.Count > 1)
            {
                winners.Sort((x, y) => x.FullRowCount().CompareTo(y.FullRowCount()));
                
                int curMaxRows = winners[0].FullRowCount();
                int tieIdx = 0;

                for(int i = 0; i < winners.Count; i++)
                {
                    if(winners[i].FullRowCount() == curMaxRows)
                    {
                        tieIdx = i;
                        break;
                    }
                }

                winners = winners.GetRange(tieIdx, winners.Count - tieIdx);
            }

            return winners;
        }

        //Returns the expect point value of move m if performed by player p this turn
        public int ExpectedMoveValue(Move m, Player p)
        {
            int value = 0;
            int tiles = 0;

            //Determine count of tiles to be pulled. For moves pulling from a factory.
            if (m.factoryIdx >= 0)
            {
                var factory = factories[m.factoryIdx];
                for (int i = 0; i < 4; i++)
                {
                    if (factory.tiles[i]?.color == m.color)
                        tiles++;
                }
            }
            //For moves pulling from the pool
            else
            {
                foreach(Tile t in pool)
                {
                    if(t?.color == m.color)
                    {
                        tiles++;
                    }
                }
            }

            if (m.rowIdx >= 0)
            {
                var nextOpenSpace = p.tileStores[m.rowIdx].Length - p.GetNextOpenSpaceInTileStoreRow(m.rowIdx);
                if (tiles >= nextOpenSpace)
                {
                    value++;
                    tiles -= nextOpenSpace;

                    //Determine grid location where tile will be placed
                    int placedColumn = 0;
                    for (int x = 0; x < 5; x++)
                    {
                        if (tileKey[x][m.rowIdx].color == m.color)
                        {
                            value += p.AdjacentTiles(x, m.rowIdx);
                            placedColumn = x;
                            break;
                        }
                    }

                    //Determine if row or column bonus would be gained
                    bool horizontalBonus = true;
                    for(int x = 0; x < 5; x++)
                    {
                        if(p.tileGrid[x][m.rowIdx] == null && tileKey[x][m.rowIdx].color != m.color)
                        {
                            horizontalBonus = false;
                            break;
                        }
                    }

                    bool verticalBonus = true;
                    for (int y = 0; y < 5; y++)
                    {
                        if (p.tileGrid[placedColumn][y] == null && tileKey[placedColumn][y].color != m.color)
                        {
                            verticalBonus = false;
                            break;
                        }
                    }

                    //Determine if set bonus would be gained
                    bool setBonus = true;
                    var keyCoords = new List<KeyValuePair<int, int>>(5);
                    for(int i = 0; i < 5; i++)
                    {
                        for(int j = 0; j < 5; j++)
                        {
                            if(tileKey[i][j].color == m.color)
                            {
                                if(i != placedColumn && j != m.rowIdx)
                                {
                                    keyCoords.Add(new KeyValuePair<int, int>(i, j));
                                }
                            }
                        }
                    }

                    foreach(KeyValuePair<int, int> coords in keyCoords)
                    {
                        if(p.tileGrid[coords.Key][coords.Value] == null)
                        {
                            setBonus = false;
                            break;
                        }
                    }

                    if (horizontalBonus)
                        value += 2;

                    if (verticalBonus)
                        value += 7;

                    if (setBonus)
                        value += 10;
                }
            }

            //Send tiles to penalty row
            int penaltyIdx = p.GetNextOpenSpaceInPenaltyRow();

            if(m.hasPenalty)
            {
                value--;
                if(penaltyIdx < p.penaltyRow.Length)
                {
                    value -= PenaltyAtPenaltyRowLocation(penaltyIdx);
                    penaltyIdx++;
                }
            }
            if(tiles > 0)
            {
                int penaltyTiles = Math.Min(tiles, p.penaltyRow.Length - penaltyIdx);

                for(int i = 0; i < penaltyTiles; i++)
                {
                    value -= PenaltyAtPenaltyRowLocation(penaltyIdx);
                    penaltyIdx++;
                }
            }

            return value;
        }

        //Returns the amount of tiles of a given color found in specified factory
        public int TilesGainedByMove(Move m)
        {
            int tileCount = 0;

            if(m.factoryIdx == -1)
            {
                foreach(Tile t in pool)
                {
                    if (t?.color == m.color)
                        tileCount++;
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (factories[m.factoryIdx].tiles[i]?.color == m.color)
                        tileCount++;
                }
            }

            return tileCount;
        }

        //Return count of each tile color play has placed on their tile grid, in descending order
        public List<KeyValuePair<TileColor, int>> PlacedTilesByColor(Player p)
        {
            List<KeyValuePair<TileColor, int>> colorCounts = new List<KeyValuePair<TileColor,int>>();

            int blackCount = 0;
            int blueCount = 0;
            int redCount = 0;
            int whiteCount = 0;
            int yellowCount = 0;

            for(int x = 0; x < 5; x++)
            {
                for(int y = 0; y < 5; y++)
                {
                    if(p.tileGrid[x][y] != null)
                    {
                        switch(p.tileGrid[x][y].color)
                        {
                            case TileColor.black:
                                {
                                    blackCount++;
                                    break;
                                }
                            case TileColor.blue:
                                {
                                    blueCount++;
                                    break;
                                }
                            case TileColor.red:
                                {
                                    redCount++;
                                    break;
                                }
                            case TileColor.white:
                                {
                                    whiteCount++;
                                    break;
                                }
                            case TileColor.yellow:
                                {
                                    yellowCount++;
                                    break;
                                }
                        }
                    }
                }
            }

            colorCounts.Add(new KeyValuePair<TileColor, int>(TileColor.black, blackCount));
            colorCounts.Add(new KeyValuePair<TileColor, int>(TileColor.blue, blueCount));
            colorCounts.Add(new KeyValuePair<TileColor, int>(TileColor.red, redCount));
            colorCounts.Add(new KeyValuePair<TileColor, int>(TileColor.white, whiteCount));
            colorCounts.Add(new KeyValuePair<TileColor, int>(TileColor.yellow, yellowCount));

            colorCounts.Sort((x, y) => x.Value.CompareTo(y.Value));
            colorCounts.Reverse();

            return colorCounts;
        }

        //-----------------------------------------------------------------
        //              Tile Management Functions
        //-----------------------------------------------------------------

        //Moves specified tile to the penalty row of the specified player
        void TileToPenaltyRow(Tile t, Player activePlayer)
        {
            int penaltyRowIdx = activePlayer.GetNextOpenSpaceInPenaltyRow();

            if (penaltyRowIdx >= 0)
            {
                activePlayer.penaltyRow[penaltyRowIdx] = t;

                //Note player penalty accrued for end of round scoring
                activePlayer.penaltyAccruedThisRound += PenaltyAtPenaltyRowLocation(penaltyRowIdx);
            }
            //If the penalty row is full, add tile to box instead
            else
            {
                box.Add(t);
            }
        }

        //Used for finding the penalty tile in a list of tiles
        static bool FindPenaltyTile(Tile t)
        {
            if (t.color == TileColor.penalty)
                return true;

            return false;
        }

        //Determine points docked for placing a tile at a certain location on the penalty row
        int PenaltyAtPenaltyRowLocation(int column)
        {
            int penalty = 1;

            if(column > 1 && column < 5)
            {
                penalty = 2;
            }

            if(column >= 5)
            {
                penalty = 3;
            }

            return penalty;
        }

        //-----------------------------------------------------------------
        //              Player Data Acquisition Functions
        //-----------------------------------------------------------------
        

    }

    public class GameResults
    {
        public List<Player> players;
        public List<Player> winners;
        public uint roundCount = 0;

        public GameResults(List<Player> p, List<Player> w, uint rounds)
        {
            players = p;
            winners = w;
            roundCount = rounds;
        }
    }
}