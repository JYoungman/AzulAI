using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    public class CenterOfTable : IEnumerable<Tile>
    {
        public List<Tile> Tiles { get; } = new List<Tile>();
        public int Count => Tiles.Count;

        public bool HasFirstPlayerTile => Tiles.Any(tile => tile.Color == TileColor.FirstPlayer);

        public void Add(Tile tile)
        {
            Tiles.Add(tile);
        }

        public int CountOf(TileColor color)
        {
            return Tiles.Where(tile => tile.Color == color).Count();
        }

        public IEnumerable<Tile> Take(TileColor color)
        {
            var taken = Tiles.Where(tile => tile.Color == color).ToList();
            Tiles.RemoveAll(tile => tile.Color == color);
            return taken;
        }

        public IEnumerable<TileColor> GetColors()
        {
            return Tiles.Select(t => t.Color).Distinct();
        }

        public IEnumerator<Tile> GetEnumerator() => Tiles.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Tiles.GetEnumerator();
    }
}
