namespace Code.Core.Items.Home.FurnitureGrid;

public sealed class MilMo_FurnitureWallGrid : MilMo_FurnitureGrid
{
	public byte Width { get; private set; }

	public float Pivot { get; private set; }

	public MilMo_FurnitureWallGrid(byte width, float pivot)
	{
		Width = width;
		Pivot = pivot;
	}
}
