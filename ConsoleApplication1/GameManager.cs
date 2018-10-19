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

        List<Tile> bag = new List<Tile>();
        List<Tile> box = new List<Tile>();
        CenterOfTable centerOfTable = new CenterOfTable();

        List<TileCollection> factories;

        bool lastRound = false;
        uint roundCount = 0;
        bool movesAvailible = true;

        public Random randNumGen;

        bool verbose = false;

        public GameManager(List<Player> playerList)
        {
            if (playerList.Count > 4 || playerList.Count < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(playerList));
            }

            //Allocate lists

            //Generate tiles
            for (int i = 0; i < 20; i++)
            {
                bag.Add(new Tile(TileColor.Blue));
                bag.Add(new Tile(TileColor.Red));
                bag.Add(new Tile(TileColor.White));
                bag.Add(new Tile(TileColor.Yellow));
                bag.Add(new Tile(TileColor.Black));
            }

            centerOfTable.Add(new Tile(TileColor.FirstPlayer));

            players = playerList;
            startingPlayer = players[0];

            //Generate Factories
            int factoryCount = players.Count * 2 + 1;
            factories = new List<TileCollection>(factoryCount);

            for (int i = 0; i < factoryCount; i++ )
            {
                factories.Add(new TileCollection());
            }

            //Provide all players with a link
            foreach(Player p in playerList)
            {
                p.gameManager = this;
                p.GameSetup();
            }

            //Create random number generator
            randNumGen = new Random();
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
                Debug.Assert(!centerOfTable.Availible, "There shouldn't be any tiles left.");
                //Dump excess factory tiles into box
                foreach(TileCollection f in factories)
                {
                    Debug.Assert(!f.Availible, "There shouldn't be any tiles left.");
                }
            }

            if (box.Count != 0)
            {
                foreach (Tile t in box)
                {
                    if (t.Color != TileColor.FirstPlayer)
                    {
                        bag.Add(t);
                    }
                    else
                    {
                        centerOfTable.Add(t);
                    }
                }
                box.Clear();
            }

            //Reset factories
            foreach (TileCollection f in factories)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (bag.Count > 0)
                    {
                        int randIdx = randNumGen.Next(0, bag.Count);
                        f.Add(bag[randIdx]);
                        bag.RemoveAt(randIdx);
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
                    if (moveCheck.Where(move => move.RowIdx != -1).Any())
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
                            progressMade |= move.RowIdx != -1;
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

                //Move completed rows to wall
                for (int row = 0; row < 5; row++ )
                {
                    var line = p.PatternLines[row];
                    if (line.IsFull)
                    {
                        pointsEarned++;

                        var col = p.Wall.ColumnOfTileColor(row, line.Color);

                        var tiles = line.Clear();
                        p.Wall[row, col] = tiles.First();

                        box.AddRange(tiles.Skip(1));

                        //Check for adjacency bonuses
                        pointsEarned += p.Wall.AdjacentTiles(row, col);
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
                    bonus += p.Wall.FullColumnCount() * 7;
                    bonus += p.Wall.FullRowCount() * 2;
                    bonus += p.Wall.FullSetCount() * 10;
                    p.score += bonus;
                }
            }

            //Housekeeping
            roundCount++;
            
            return progressMade;
        }

        //Returns a list of all of the moves that the player can legally perform this turn
        public List<Move> GenerateLegalMoves(Player activePlayer)
        {
            List<Move> legalMoves = new List<Move>();

            //Iterate through Factories, creating moves for each row its colors can be placed in
            for (int f = 0; f < factories.Count; f++)
            {
                if (factories[f].Availible)
                {
                    //Look for legal places to place tiles of availible colors
                    foreach (TileColor tc in factories[f].GetColors())
                    {
                        List<int> availibleRows = LegalRowsForColor(tc, activePlayer);

                        //Generate moves for moving directly to floor line
                        int tileCount = factories[f].CountOf(tc); // TODO add this count to the move

                        //Generate move data
                        foreach (int row in availibleRows)
                        {
                            legalMoves.Add(new Move(f, row, tc, tileCount, false));
                        }

                        legalMoves.Add(new Move(f, -1, tc, tileCount, false));
                    }
                }
            }

            //Create moves for pulling from the center of the table
            bool hasFirstPlayerPenalty = centerOfTable.HasFirstPlayerTile;

            //Look for legal places to place tiles of availible colors
            foreach(TileColor tc in centerOfTable.GetColors())
            {
                List<int> availibleRows = LegalRowsForColor(tc, activePlayer);

                int tileCount = centerOfTable.CountOf(tc);

                //Generate move data
                foreach (int row in availibleRows)
                {
                    legalMoves.Add(new Move(-1, row, tc, tileCount, hasFirstPlayerPenalty));
                }

                //Generate moves for moving directly to floor line
                if (tc != TileColor.FirstPlayer)
                {
                    legalMoves.Add(new Move(-1, -1, tc, tileCount, hasFirstPlayerPenalty));
                }
            }

            return legalMoves;
        }

        void ExecuteMove(Move move, Player activePlayer)
        {
            //Positive values represents a Factory to pull from
            if (move.FactoryIdx >= 0)
            {
                var factory = factories[move.FactoryIdx];

                var moveTiles = factory.Take(move.Color);

                //Assign requested tiles to the appropriate row on the player's board
                foreach (var tile in moveTiles)
                {
                    if (move.RowIdx >= 0)
                    {
                        var line = activePlayer.PatternLines[move.RowIdx];

                        if (!line.IsFull)
                        {
                            line.Add(tile);
                        }
                        //If there isn't room in the desired row, move remaining tiles to the floor line
                        else
                        {
                            TileToFloorLine(tile, activePlayer);
                        }
                    }
                    //Negative row index in move indicates floor line as target row
                    else
                    {
                        TileToFloorLine(tile, activePlayer);
                    }
                }

                //Move remaining tiles into the center of the table
                foreach (var tile in factory)
                {
                    centerOfTable.Add(tile);
                }

                //Empty factory
                factory.Clear();
            }
            //A negative factoryIdx value indicates player has chosen to pull from the center of the table
            else
            {
                //Take the first player tile if present
                if (move.HasFirstPlayerPenalty)
                {
                    TileToFloorLine(centerOfTable.TakeFirstPlayerTile(), activePlayer);
                    startingPlayer = activePlayer;
                }

                //Place the selected tiles in the indicated row
                foreach (Tile t in centerOfTable.Take(move.Color))
                {
                    if (move.RowIdx >= 0)
                    {
                        var line = activePlayer.PatternLines[move.RowIdx];
                        if (!line.IsFull)
                        {
                            line.Add(t);
                        }
                        //If there isn't room in the desired row, move remaining tiles to the floor line
                        else
                        {
                            TileToFloorLine(t, activePlayer);
                        }
                    }
                    //Negative row index in move indicates floor line as target row
                    else
                    {
                        TileToFloorLine(t, activePlayer);
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
                    if (p.Wall[row, col] != null)
                    {
                        fullTiles++;
                    }
                }

                //Rows with four filled tiles are candidates for becoming full rows this game round
                if (fullTiles == 4)
                {
                    //If pattern line of a wall row with four filled spaces is filled, that row in the wall will fill at the end of the round
                    if (p.PatternLines[row].IsFull)
                        return true;
                }
            }

            return false;
        }

        //Returns list of rows in which the player can legally place a tile of a given color
        List<int> LegalRowsForColor(TileColor c, Player p)
        {
            List<int> rows = new List<int>(5);
            if (c == TileColor.FirstPlayer)
            {
                return rows;
            }

            for (int row = 0; row < 5; row++)
            {
                bool canMatchInRow = true;

                //Check wall to see if tile of color c is already placed on this row
                if(p.PatternLines[row].IsEmpty)
                {
                    var col = p.Wall.ColumnOfTileColor(row, c);
                    if(p.Wall[row, col] != null)
                    {
                        canMatchInRow = false;
                    }
                }
                //Check if there is space remaining in pattern lines containing tiles of color c
                else if(p.PatternLines[row].Color == c)
                {
                    canMatchInRow = !p.PatternLines[row].IsFull;
                }
                //Full rows or empty rows with appropriate wall space cannot recieve tiles of color c
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
            foreach(TileCollection f in factories)
            {
                knownTiles += f.Count;
            }

            //Check bag
            knownTiles += bag.Count;

            //Check center of table
            knownTiles += centerOfTable.Count;

            //Check box
            knownTiles += box.Count;

            //Check player boards
            foreach(Player p in players)
            {
                for(int row = 0; row < 5; row++)
                {
                    for(int col = 0; col < 5; col++)
                    {
                        if (p.Wall[row, col] != null)
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
                winners.Sort((x, y) => x.Wall.FullRowCount().CompareTo(y.Wall.FullRowCount()));
                
                int curMaxRows = winners[0].Wall.FullRowCount();
                int tieIdx = 0;

                for(int i = 0; i < winners.Count; i++)
                {
                    if(winners[i].Wall.FullRowCount() == curMaxRows)
                    {
                        tieIdx = i;
                        break;
                    }
                }

                winners = winners.GetRange(tieIdx, winners.Count - tieIdx);
            }

            return winners;
        }

        //Returns the expected point value of move m if performed by player p this turn
        public int ExpectedMoveValue(Move m, Player p)
        {
            int value = 0;
            int tiles = m.Count;

            if (m.RowIdx >= 0)
            {
                var line = p.PatternLines[m.RowIdx];
                var availability = line.Availability;
                if (tiles >= availability)
                {
                    value++;
                    tiles -= availability;

                    //Determine grid location where tile will be placed
                    int placedColumn = p.Wall.ColumnOfTileColor(m.RowIdx, m.Color);

                    value += p.TilesThatWillBeAdjacent(m.RowIdx, placedColumn);

                    //Determine if row or column bonus would be gained
                    bool horizontalBonus = true;
                    for(int col = 0; col < 5; col++)
                    {
                        if(p.Wall[m.RowIdx, col] == null && p.Wall.TileKey[m.RowIdx, col] != m.Color)
                        {
                            horizontalBonus = false;
                            break;
                        }
                    }

                    bool verticalBonus = true;
                    for (int r = 0; r < 5; r++)
                    {
                        if (p.Wall[r, placedColumn] == null 
                            && (p.Wall.TileKey[r, placedColumn] != m.Color
                                || !p.PatternLines[r].IsFull))
                        {
                            verticalBonus = false;
                            break;
                        }
                    }

                    //Determine if set bonus would be gained
                    bool setBonus = m.Color != TileColor.FirstPlayer;
                    var keyCoords = new List<KeyValuePair<int, int>>(5);
                    for(int r = 0; r < 5; r++)
                    {
                        for(int c = 0; c < 5; c++)
                        {
                            if(p.Wall.TileKey[r, c] == m.Color)
                            {
                                if(r != m.RowIdx && c != placedColumn)
                                {
                                    keyCoords.Add(new KeyValuePair<int, int>(r, c));
                                }
                            }
                        }
                    }

                    foreach(KeyValuePair<int, int> coords in keyCoords)
                    {
                        if(p.Wall[coords.Key, coords.Value] == null
                            && !p.PatternLines[coords.Key].IsFull)
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

            if(m.HasFirstPlayerPenalty)
            {
                tiles++;
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

        //Returns whether or not the last round condition has been met
        public bool IsLastRound()
        {
            return lastRound;
        }

        //-----------------------------------------------------------------
        //              Tile Management Functions
        //-----------------------------------------------------------------

        //Moves specified tile to the floor line of the specified player
        void TileToFloorLine(Tile t, Player activePlayer)
        {
            if (!activePlayer.FloorLine.IsFull)
            {
                activePlayer.FloorLine.Add(t);

                //Note player penalty accrued for end of round scoring
                activePlayer.penaltyAccruedThisRound += activePlayer.FloorLine.NextPenalty();
            }
            //If the floor line is full, add tile to box instead
            else
            {
                box.Add(t);
            }
        }
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