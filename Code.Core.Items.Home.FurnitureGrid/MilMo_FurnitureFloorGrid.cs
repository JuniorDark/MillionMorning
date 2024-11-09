using Code.Core.Network.types;

namespace Code.Core.Items.Home.FurnitureGrid;

public sealed class MilMo_FurnitureFloorGrid : MilMo_FurnitureGrid
{
	public byte[][] Tiles { get; private set; }

	public int Rows { get; private set; }

	public int Columns { get; private set; }

	public float PivotRow { get; private set; }

	public float PivotCol { get; private set; }

	public MilMo_FurnitureFloorGrid()
		: this(1, 1, 0.5f, 0.5f)
	{
		Tiles[0][0] = 1;
	}

	private MilMo_FurnitureFloorGrid(int rows, int columns, float pivotRow, float pivotColumn)
	{
		Rows = rows;
		Columns = columns;
		Tiles = new byte[rows][];
		for (int i = 0; i < Tiles.Length; i++)
		{
			Tiles[i] = new byte[columns];
		}
		PivotRow = pivotRow;
		PivotCol = pivotColumn;
	}

	public MilMo_FurnitureFloorGrid(Code.Core.Network.types.FurnitureGrid grid)
		: this(grid.GetBytes().GetArray().Length / grid.GetBytes().GetColumns(), grid.GetBytes().GetColumns(), grid.GetPivot().GetY(), grid.GetPivot().GetX())
	{
		int num = 0;
		for (int i = 0; i < Rows; i++)
		{
			for (int j = 0; j < Columns; j++)
			{
				Tiles[i][j] = grid.GetBytes().GetArray()[num];
				num++;
			}
		}
	}

	public MilMo_FurnitureFloorGrid GetRotatedCopy()
	{
		MilMo_FurnitureFloorGrid milMo_FurnitureFloorGrid = new MilMo_FurnitureFloorGrid(Columns, Rows, PivotCol, (float)Rows - PivotRow);
		for (int num = Rows - 1; num >= 0; num--)
		{
			for (int i = 0; i < Columns; i++)
			{
				milMo_FurnitureFloorGrid.Tiles[i][Rows - 1 - num] = Tiles[num][i];
			}
		}
		return milMo_FurnitureFloorGrid;
	}
}
