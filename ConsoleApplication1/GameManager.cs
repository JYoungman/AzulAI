using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AzulAI
{
    public class GameManager
    {
        List<Player> players;
        Player startingPlayer;

        List<Tile> bag;
        List<Tile> box;
        List<Tile> pool;

        Tile[,] tileKey;

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

            pool.Add(new Tile(TileColor.FirstPlayer));

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
            tileKey = new Tile[5, 5];
            for (int row = 0; row < 5; row++ )
            {
                switch (row)
                {
                    case 0:
                        {
                            tileKey[row, 0] = new Tile(TileColor.blue);
                            tileKey[row, 1] = new Tile(TileColor.yellow);
                            tileKey[row, 2] = new Tile(TileColor.red);
                            tileKey[row, 3] = new Tile(TileColor.black);
                            tileKey[row, 4] = new Tile(TileColor.white);
                            break;
                        }
                    case 1:
                        {
                            tileKey[row, 1] = new Tile(TileColor.blue);
                            tileKey[row, 2] = new Tile(TileColor.yellow);
                            tileKey[row, 3] = new Tile(TileColor.red);
                            tileKey[row, 4] = new Tile(TileColor.black);
                            tileKey[row, 0] = new Tile(TileColor.white);
                            break;
                        }
                    case 2:
                        {
                            tileKey[row, 2] = new Tile(TileColor.blue);
                            tileKey[row, 3] = new Tile(TileColor.yellow);
                            tileKey[row, 4] = new Tile(TileColor.red);
                            tileKey[row, 0] = new Tile(TileColor.black);
                            tileKey[row, 1] = new Tile(TileColor.white);
                            break;
                        }
                    case 3:
                        {
                            tileKey[row, 3] = new Tile(TileColor.blue);
                            tileKey[row, 4] = new Tile(TileColor.yellow);
                            tileKey[row, 0] = new Tile(TileColor.red);
                            tileKey[row, 1] = new Tile(TileColor.black);
                            tileKey[row, 2] = new Tile(TileColor.white);
                            break;
                        }
                    case 4:
                        {
                            tileKey[row, 4] = new Tile(TileColor.blue);
                            tileKey[row, 0] = new Tile(TileColor.yellow);
                            tileKey[row, 1] = new Tile(TileColor.red);
                            tileKey[row, 2] = new Tile(TileColor.black);
                            tileKey[row, 3] = new Tile(TileColor.white);
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
            var progressMade = true;
            while(!lastRound && progressMade)
            {
                progressMade = GameRound();
            }
            DisplayText("Game over");
            GameResults results = new GameResults(players, GetWinningPlayers(), roundCount);
            return results;
        }

        //Play a round of the game. The game is a series of these. Should probably be several, smaller functions.
        bool GameRound()
        {
            //Reset tiles
            if (roundCount != 0)
            {
                Debug.Assert(!pool.Any(), "There shouldn't be any tiles left.");
                pool.Clear();
                //Dump excess factory tiles into box
                foreach(Factory f in factories)
                {
                    for(int i = 0; i < 4; i++)
                    {
                        if(f.tiles[i] != null)
                        {
                            Debug.Assert(false, "There shouldn't be any tiles left.");
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
                    if (t.color != TileColor.FirstPlayer)
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
                var moveCheck = GenerateLegalMoves(p);
                if(moveCheck.Any())
                {
                    p.legalMovesAvailible = true;
                    if (moveCheck.Where(move => move.rowIdx != -1).Any())
                    {
                        movesAvailible = true;
                    }
                }
                else
                {
                    Debug.Assert(false, "No legal moves.");
                }
            }

            //If no player has legal moves, end the game
            if(movesAvailible == false)
            {
                lastRound = true;
            }

            var progressMade = false;

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
                            var move = p.PerformMove(legalMoves);
                            ExecuteMove(move, p);
                            progressMade |= move.rowIdx != -1;
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
                                if (pCheck.legalMovesAvailible)
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
                for (int row = 0; row < 5; row++ )
                {
                    var line = p.PatternLines[row];
                    if (line.IsFull)
                    {
                        pointsEarned++;

                        var col = ColumnOfTileColor(row, line.Color);

                        var tiles = line.Clear();
                        p.TileGrid[row, col] = tiles.First();

                        box.AddRange(tiles.Skip(1));

                        //Check for adjacency bonuses
                        pointsEarned += p.AdjacentTiles(row, col);
                    }
                }
                p.score += pointsEarned;

                //Deduct penalties, not going below zero
                var appliedPenalty = Math.Min(p.penaltyAccruedThisRound, p.score);
                p.score -= appliedPenalty;
                p.totalAppliedPenalties += appliedPenalty;
                p.totalEarnedPenalties += p.penaltyAccruedThisRound;
                p.penaltyAccruedThisRound = 0;

                box.AddRange(p.FloorLine.Clear());

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
            return progressMade;
        }

        //Returns a list of all of the moves that the player can legally perform this turn
        public List<Move> GenerateLegalMoves(Player activePlayer)
        {
            List<Move> legalMoves = new List<Move>();

            //Iterate through Factories, creating moves for each row its colors can be placed in
            for (int f = 0; f < factories.Count; f++)
            {
                if (factories[f].availible)
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

                            if (addColor)
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

                        legalMoves.Add(new Move(f, -1, tc, false));
                    }
                }
            }

            //Create moves for pulling from the pool
            bool hasFirstPlayerPenalty = false;
            
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
                        if (t.color == TileColor.FirstPlayer)
                        {
                            hasFirstPlayerPenalty = true;
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
                    legalMoves.Add(new Move(-1, row, tc, hasFirstPlayerPenalty));
                }

                //Generate moves for moving directly to penalty row
                if (tc != TileColor.FirstPlayer)
                {
                    legalMoves.Add(new Move(-1, -1, tc, hasFirstPlayerPenalty));
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
                var factory = factories[move.factoryIdx];
                factory.availible = false;

                for (int i = 0; i < 4; i++)
                {
                    //Assign requested tiles to the appropriate row on the player's board
                    var tile = factory.tiles[i];
                    if (tile != null && tile.color == move.color)
                    {
                        if (move.rowIdx >= 0)
                        {
                            var line = activePlayer.PatternLines[move.rowIdx];

                            if (!line.IsFull)
                            {
                                line.Add(tile);
                            }
                            //If there isn't room in the desired row, move remaining tiles to the penalty row
                            else
                            {
                                TileToFloorLine(tile, activePlayer);
                            }
                        }
                        //Negative row index in move indicates penalty row as target row
                        else
                        {
                            TileToFloorLine(tile, activePlayer);
                        }
                    }
                    //Move remaining tiles into the pool
                    else if (tile != null)
                    {
                        pool.Add(tile);
                    }
                }
                
                //Empty factory
                for (int j = 0; j < 4; j++)
                    factory.tiles[j] = null;
            }
            //A negative factoryIdx value indicates player has chosen to pull from the pool
            else
            {
                //Take the penalty tile if present
                if (move.hasFirstPlayerPenalty)
                {
                    var firstPlayerTile = pool.Find(tile => tile.color == TileColor.FirstPlayer);
                    TileToFloorLine(firstPlayerTile, activePlayer);
                    pool.Remove(firstPlayerTile);
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
                            var line = activePlayer.PatternLines[move.rowIdx];
                            if (!line.IsFull)
                            {
                                line.Add(t);
                            }
                            //If there isn't room in the desired row, move remaining tiles to the penalty row
                            else
                            {
                                TileToFloorLine(t, activePlayer);
                            }
                        }
                        //Negative row index in move indicates penalty row as target row
                        else
                        {
                            TileToFloorLine(t, activePlayer);
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
            for (int row = 0; row < 5; row++)
            {
                int fullTiles = 0;
                for (int col = 0; col < 5; col++)
                {
                    if (p.TileGrid[row, col] != null)
                    {
                        fullTiles++;
                    }
                }

                //Rows with four filled tiles are candidates for becoming full rows this game round
                if (fullTiles == 4)
                {
                    //If tile store row of a grid row with four filled spaces is filled, that row in the tile grid will fill at the end of the round
                    if (p.PatternLines[row].IsFull)
                        return true;
                }
            }

            return false;
        }

        //Returns list of rows in which the player can legally place a tile
        List<int> LegalRowsForColor(TileColor c, Player p)
        {
            List<int> rows = new List<int>();
            if (c == TileColor.FirstPlayer)
            {
                return rows;
            }

            for (int row = 0; row < 5; row++)
            {
                bool canMatchInRow = true;

                //Check grid to see if tile of color c is already placed on this row
                if(p.PatternLines[row].IsEmpty)
                {
                    var col = ColumnOfTileColor(row, c);
                    if(p.TileGrid[row, col] != null)
                    {
                        canMatchInRow = false;
                    }
                }
                //Check if there is space remaining in stores containing tiles of color c
                else if(p.PatternLines[row].Color == c)
                {
                    canMatchInRow = !p.PatternLines[row].IsFull;
                }
                //Full rows or empty rows with appropriate grid space cannot recieve tiles of color c
                else
                {
                    canMatchInRow = false;
                }

                if(canMatchInRow)
                {
                    rows.Add(row);
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
                for(int row = 0; row < 5; row++)
                {
                    for(int col = 0; col < 5; col++)
                    {
                        if (p.TileGrid[row, col] != null)
                            knownTiles++;
                    }
                }

                for(int row = 0; row < 5; row++)
                {
                    knownTiles += p.PatternLines[row].Load;
                }

                knownTiles += p.FloorLine.Load;
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

            // TODO: This is not a shallow copy. You're modifying the real table.
            Tile[,] tempGrid = p.TileGrid;

            //Add tiles that will be put on tile grid at end of turn to temporary grid
            int row = 0;
            foreach (var line in p.PatternLines)
            {
                if (line.IsFull)
                {
                    TileColor tc = line.Color;
                    for (int col = 0; col < 5; col++)
                    {
                        if (tileKey[col, row].color == tc)
                        {
                            tempGrid[col, row] = line.Peak();
                            break;
                        }
                    }
                }
                row++;
            }

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
                var line = p.PatternLines[m.rowIdx];
                var availability = line.Availability;
                if (tiles >= availability)
                {
                    value++;
                    tiles -= availability;

                    //Determine grid location where tile will be placed
                    int placedColumn = 0;
                    for (int x = 0; x < 5; x++)
                    {
                        if (tileKey[x, m.rowIdx].color == m.color)
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
                        if(tempGrid[x, m.rowIdx] == null && tileKey[x, m.rowIdx].color != m.color)
                        {
                            horizontalBonus = false;
                            break;
                        }
                    }

                    bool verticalBonus = true;
                    for (int y = 0; y < 5; y++)
                    {
                        if (tempGrid[placedColumn, y] == null && tileKey[placedColumn, y].color != m.color)
                        {
                            verticalBonus = false;
                            break;
                        }
                    }

                    //Determine if set bonus would be gained
                    bool setBonus = m.color != TileColor.FirstPlayer;
                    var keyCoords = new List<KeyValuePair<int, int>>(5);
                    for(int i = 0; i < 5; i++)
                    {
                        for(int j = 0; j < 5; j++)
                        {
                            if(tileKey[i, j].color == m.color)
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
                        if(tempGrid[coords.Key, coords.Value] == null)
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

            //Send tiles to the floor line
            int penaltyIdx = p.FloorLine.Load;

            if(m.hasFirstPlayerPenalty)
            {
                if(!p.FloorLine.IsFull)
                {
                    value -= p.FloorLine.PenaltyAtIndex(penaltyIdx);
                    penaltyIdx++;
                }
            }
            if(tiles > 0)
            {
                int penaltyTiles = Math.Min(tiles, p.FloorLine.Availability);

                for (int i = 0; i < penaltyTiles; i++)
                {
                    value -= p.FloorLine.PenaltyAtIndex(penaltyIdx);
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

            for(int row = 0; row < 5; row++)
            {
                for(int col = 0; col < 5; col++)
                {
                    if(p.TileGrid[row, col] != null)
                    {
                        switch(p.TileGrid[row, col].color)
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

        //Returns the color of tile that gets placed at the provided coordinates
        public TileColor TileColorAtLocation(int row, int col)
        {
            return tileKey[row, col].color;
        }

        //Returns the column the color belongs in for the given row
        public int ColumnOfTileColor(int row, TileColor color)
        {
            for (int col = 0; col < 5; col++)
            {
                if (tileKey[row, col].color == color)
                {
                    return col;
                }
            }
            throw new InvalidOperationException(color.ToString());
        }

        //Returns whether or not the last round condition has been met
        public bool IsLastRound()
        {
            return lastRound;
        }

        //-----------------------------------------------------------------
        //              Tile Management Functions
        //-----------------------------------------------------------------

        //Moves specified tile to the penalty row of the specified player
        void TileToFloorLine(Tile t, Player activePlayer)
        {
            if (!activePlayer.FloorLine.IsFull)
            {
                activePlayer.FloorLine.Add(t);

                //Note player penalty accrued for end of round scoring
                activePlayer.penaltyAccruedThisRound += activePlayer.FloorLine.NextPenalty();
            }
            //If the penalty row is full, add tile to box instead
            else
            {
                box.Add(t);
            }
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