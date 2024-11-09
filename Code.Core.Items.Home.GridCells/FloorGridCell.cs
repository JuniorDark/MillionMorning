namespace Code.Core.Items.Home.GridCells;

public class FloorGridCell : GridCell
{
	public int Col;

	public int Row;

	public FloorGridCell()
	{
	}

	public FloorGridCell(int row, int col)
	{
		Row = row;
		Col = col;
	}

	public FloorGridCell(FloorGridCell c)
	{
		Col = c.Col;
		Row = c.Row;
	}

	public override string ToString()
	{
		return "F" + Col + ":" + Row;
	}
}
