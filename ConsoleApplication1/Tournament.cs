using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    public class Tournament
    {
        public Tournament(List<Player> players, int rounds)
        {
            Players = players;
            Rounds = rounds;
        }

        public List<Player> Players { get; }
        public int Rounds { get; }

        public TournamentResults PlayTournament()
        {
            var wins = new int[Players.Count];
            var totalScores = new int[Players.Count];
            var totalEarnedPenalties = new int[Players.Count];
            var totalAppliedPenalties = new int[Players.Count];
            var heatMap = new int[5, 5];
            var totalGameRounds = 0u;
            var ties = 0;

            var timer = new Stopwatch();
            timer.Start();

            for (int i = 0; i < Rounds; i++)
            {
                // Clone the list so it's not affected by turn order
                GameManager gm = new GameManager(new List<Player>(Players));
                GameResults gameResult = gm.PlayGame();

                if (gameResult.winners.Count > 1)
                {
                    ties++;
                }

                foreach (var winner in gameResult.winners)
                {
                    wins[Players.IndexOf(winner)]++;
                    UpdateHeatMap(heatMap, winner.Wall);
                }

                for (int j = 0; j < Players.Count; j++)
                {
                    // players comes back in a different order than it started
                    var player = gameResult.players[j];
                    var playerIndex = Players.IndexOf(player);
                    totalScores[playerIndex] += player.score;
                    totalEarnedPenalties[playerIndex] += player.totalEarnedPenalties;
                    totalAppliedPenalties[playerIndex] += player.totalAppliedPenalties;
                }

                totalGameRounds += gameResult.roundCount;
            }
            timer.Stop();

            var tournamentResults = new TournamentResults()
            {
                Ties = ties,
                Time = timer.Elapsed,
            };

            for (int i = 0; i < Players.Count; i++)
            {
                tournamentResults.WinPercentages.Add(wins[i] / (double)Rounds);
                tournamentResults.AverageScores.Add(totalScores[i] / (double)Rounds);
                tournamentResults.AverageEarnedPenalties.Add(totalEarnedPenalties[i] / (double)Rounds);
                tournamentResults.AverageAppliedPenalties.Add(totalAppliedPenalties[i] / (double)Rounds);
            }

            tournamentResults.AverageRounds = totalGameRounds / (double)Rounds;

            tournamentResults.HeatMap = CompileHeatMap(heatMap);

            return tournamentResults;
        }

        private void UpdateHeatMap(int[,] heatMap, Tile[,] tileGrid)
        {
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    if (tileGrid[row, col] != null)
                    {
                        heatMap[row, col]++;
                    }
                }
            }
        }

        private double[,] CompileHeatMap(int[,] heatMap)
        {
            var compiled = new double[5, 5];
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    compiled[row, col] = heatMap[row, col] / (double)Rounds;
                }
            }

            return compiled;
        }
    }

    public class TournamentResults
    {
        public List<double> WinPercentages { get; } = new List<double>();

        public List<double> AverageScores { get; } = new List<double>();
        public List<double> AverageEarnedPenalties { get; } = new List<double>();
        public List<double> AverageAppliedPenalties { get; } = new List<double>();

        public double AverageRounds { get; set; }

        public int Ties { get; set; }

        public TimeSpan Time { get; set; }

        public double[,] HeatMap { get; set; }
    }
}
