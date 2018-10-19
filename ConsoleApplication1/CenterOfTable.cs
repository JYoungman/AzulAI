using System.Diagnostics;

namespace AzulAI
{
    public class CenterOfTable : TileCollection
    {
        public bool HasFirstPlayerTile => FirstPlayerTile != null;

        private Tile FirstPlayerTile { get; set; }

        public Tile TakeFirstPlayerTile()
        {
            var tile = FirstPlayerTile;
            Debug.Assert(tile != null);
            FirstPlayerTile = null;
            return tile;
        }

        public override void Add(Tile tile)
        {
            if (tile.Color == TileColor.FirstPlayer)
            {
                FirstPlayerTile = tile;
            }
            else
            {
                base.Add(tile);
            }
        }
    }
}
