using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    class QuickEndPlayer : Player
    {
        //Main AI method. Determine which of the legally availible moves the player should take on their current turn.
        public override Move PerformMove(List<Move> availibleMoves)
        {
            //Try to fill in the top row as quickly as possible
            foreach(Move m in availibleMoves)
            {
                if (m.rowIdx == 0)
                    return m;
            }
            //Failing that, favor getting more tiles onto the board
            for (int y = 1; y < 5; y++ )
            {
                if(tileStores[y][0] != null)
                {
                    foreach(Move m in availibleMoves)
                    {
                        if(m.color == tileStores[y][0].color)
                        {
                            return m;
                        }
                    }
                }
            }

            return availibleMoves[gameManager.randNumGen.Next(0, availibleMoves.Count)];
        }

        //Courtesy function for displaying information about match and results.
        public override string DisplayName()
        {
            return "End Game Quick AI";
        }
    }
}
