namespace Code.Core.Items.Home.GridCells;

public class WallGridCell : GridCell
{
	public int WallIndex;

	public int Tile;

	public WallGridCell()
	{
	}

	public WallGridCell(int wall, int tile)
	{
		WallIndex = wall;
		Tile = tile;
	}

	public WallGridCell(WallGridCell c)
	{
		WallIndex = c.WallIndex;
		Tile = c.Tile;
	}

	public override string ToString()
	{
		return "W" + WallIndex + ":" + Tile;
	}
}
