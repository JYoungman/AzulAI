using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    public class Factory
    {
        public Tile[] tiles { get; set; }
        public bool availible = true;

        public Factory()
        {
            tiles = new Tile[4];
        }
    }
}
