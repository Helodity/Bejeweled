using System.Collections.Generic;
using System.Linq;

public struct Match
{
    public List<Tile> Tiles;
    public TileContents.ContentType Content;
    public bool IsCornerMatch;
    public Match(TileContents.ContentType type, List<Tile> tiles) {
        Tiles = tiles;
        Content = type;

        List<int> xCoords = new List<int>();
        List<int> yCoords = new List<int>(); ;
        foreach(Tile t in tiles) {
            xCoords.Add(t.Position.x);
            yCoords.Add(t.Position.y);
        }
        bool multipleX = xCoords.Distinct().Count() > 1;
        bool multipleY = yCoords.Distinct().Count() > 1;
        IsCornerMatch = multipleX && multipleY;
    }
}
