using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    public class Tile
    {
        public TileColor color { get; set; }

        public Tile(TileColor tc)
        {
            color = tc;
        }
    }

    public enum TileColor
    {
        blue,
        red,
        yellow,
        white,
        black,
        penalty,
        empty
    }
}
