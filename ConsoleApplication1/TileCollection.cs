using System;
using System.Collections;
using System.Collections.Generic;

namespace AzulAI
{
    public class TileCollection : IEnumerable<Tile>
    {
        protected List<Tile> BlueTiles { get; } = new List<Tile>(4);
        protected List<Tile> RedTiles { get; } = new List<Tile>(4);
        protected List<Tile> YellowTiles { get; } = new List<Tile>(4);
        protected List<Tile> WhiteTiles { get; } = new List<Tile>(4);
        protected List<Tile> BlackTiles { get; } = new List<Tile>(4);
        public bool Availible => BlueTiles.Count > 0 || RedTiles.Count > 0 || YellowTiles.Count > 0 || WhiteTiles.Count > 0 || BlackTiles.Count > 0;
        public int Count => BlueTiles.Count + RedTiles.Count + YellowTiles.Count + WhiteTiles.Count + BlackTiles.Count;

        protected List<Tile> SelectColor(TileColor color)
        {
            switch (color)
            {
                case TileColor.Blue:
                    return BlueTiles;
                case TileColor.Red:
                    return RedTiles;
                case TileColor.Yellow:
                    return YellowTiles;
                case TileColor.White:
                    return WhiteTiles;
                case TileColor.Black:
                    return BlackTiles;
                default:
                    throw new NotImplementedException(color.ToString());
            }
        }

        public int CountOf(TileColor color) => SelectColor(color).Count;

        public virtual void Add(Tile tile) => SelectColor(tile.Color).Add(tile);

        public IEnumerable<TileColor> GetColors()
        {
            if (BlueTiles.Count > 0)
            {
                yield return TileColor.Blue;
            }
            if (RedTiles.Count > 0)
            {
                yield return TileColor.Red;
            }
            if (YellowTiles.Count > 0)
            {
                yield return TileColor.Yellow;
            }
            if (WhiteTiles.Count > 0)
            {
                yield return TileColor.White;
            }
            if (BlackTiles.Count > 0)
            {
                yield return TileColor.Black;
            }
        }

        public IEnumerable<Tile> Take(TileColor color)
        {
            var list = SelectColor(color);
            var toReturn = new List<Tile>(list);
            list.Clear();
            return toReturn;
        }

        public void Clear()
        {
            BlueTiles.Clear();
            RedTiles.Clear();
            YellowTiles.Clear();
            WhiteTiles.Clear();
            BlackTiles.Clear();
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            if (BlueTiles.Count > 0)
            {
                foreach (var tile in BlueTiles)
                {
                    yield return tile;
                }
            }
            if (RedTiles.Count > 0)
            {
                foreach (var tile in RedTiles)
                {
                    yield return tile;
                }
            }
            if (YellowTiles.Count > 0)
            {
                foreach (var tile in YellowTiles)
                {
                    yield return tile;
                }
            }
            if (WhiteTiles.Count > 0)
            {
                foreach (var tile in WhiteTiles)
                {
                    yield return tile;
                }
            }
            if (BlackTiles.Count > 0)
            {
                foreach (var tile in BlackTiles)
                {
                    yield return tile;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
