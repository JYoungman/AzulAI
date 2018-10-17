namespace AzulAI
{
    public class Tile
    {
        public TileColor Color { get; }

        public Tile(TileColor color)
        {
            Color = color;
        }
    }

    public enum TileColor
    {
        None,
        Blue,
        Red,
        Yellow,
        White,
        Black,
        FirstPlayer,
    }
}
