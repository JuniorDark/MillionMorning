namespace Code.Core.Items.Home.GridCells;

public class AttachNode : GridCell
{
	public long FurnitureId;

	public short NodeIndex;

	public AttachNode()
	{
	}

	public AttachNode(long furnitureId, short nodeIndex)
	{
		FurnitureId = furnitureId;
		NodeIndex = nodeIndex;
	}

	public override string ToString()
	{
		return "A" + FurnitureId + ":" + NodeIndex;
	}
}
