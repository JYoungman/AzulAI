using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    public class Factory : IEnumerable<Tile>
    {
        public List<Tile> Tiles { get; }
        public bool Availible => Tiles.Any();

        public int CountOf(TileColor color)
        {
            return Tiles.Where(tile => tile.Color == color).Count();
        }

        public Factory()
        {
            Tiles = new List<Tile>(4);
        }

        internal void Add(Tile tile)
        {
            Debug.Assert(Tiles.Count < 4);
            Tiles.Add(tile);
        }

        public IEnumerable<TileColor> GetColors()
        {
            return Tiles.Select(t => t.Color).Distinct();
        }

        public IEnumerator<Tile> GetEnumerator() => Tiles.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Tiles.GetEnumerator();
    }
}
