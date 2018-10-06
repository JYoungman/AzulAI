using System;
using System.Collections.Generic;
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
            var totalGameRounds = 0u;

            for (int i = 0; i < Rounds; i++)
            {
                GameManager gm = new GameManager(Players);
                GameResults gameResult = gm.PlayGame();

                foreach (var winner in gameResult.winners)
                {
                    wins[Players.IndexOf(winner)]++;
                }

                for (int j = 0; j < Players.Count; j++)
                {
                    // players comes back in a different order than it started
                    var player = gameResult.players[j];
                    totalScores[Players.IndexOf(player)] += player.score;
                }

                totalGameRounds += gameResult.roundCount;
            }

            var tournamentResults = new TournamentResults();

            for (int i = 0; i < Players.Count; i++)
            {
                tournamentResults.WinPercentages.Add(wins[i] / (double)Rounds);
                tournamentResults.AverageScores.Add(totalScores[i] / (double)Rounds);
            }

            tournamentResults.AverageRounds = totalGameRounds / (double)Rounds;

            return tournamentResults;
        }
    }

    public class TournamentResults
    {
        public List<double> WinPercentages { get; } = new List<double>();

        public List<double> AverageScores { get; } = new List<double>();

        public double AverageRounds { get; set; }
    }
}
