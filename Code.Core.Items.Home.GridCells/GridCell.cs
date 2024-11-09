using System;
using UnityEngine;

namespace Code.Core.Items.Home.GridCells;

public abstract class GridCell
{
	public static GridCell Parse(string gridCell)
	{
		string text;
		try
		{
			if (gridCell.StartsWith("F"))
			{
				FloorGridCell floorGridCell = new FloorGridCell();
				string[] array = gridCell.Substring(1).Split(":".ToCharArray());
				floorGridCell.Col = int.Parse(array[0]);
				floorGridCell.Row = int.Parse(array[1]);
				return floorGridCell;
			}
			if (gridCell.StartsWith("W"))
			{
				WallGridCell wallGridCell = new WallGridCell();
				string[] array2 = gridCell.Substring(1).Split(":".ToCharArray());
				wallGridCell.WallIndex = int.Parse(array2[0]);
				wallGridCell.Tile = int.Parse(array2[1]);
				return wallGridCell;
			}
			if (gridCell.StartsWith("A"))
			{
				AttachNode attachNode = new AttachNode();
				string[] array3 = gridCell.Substring(1).Split(":".ToCharArray());
				attachNode.FurnitureId = int.Parse(array3[0]);
				attachNode.NodeIndex = short.Parse(array3[1]);
				return attachNode;
			}
			text = "Unknown prefix (valid prefixes are 'F' for Floor, 'W' for Wall, and 'A' for Attached).";
		}
		catch (FormatException ex)
		{
			text = "FormatException: " + ex.Message;
		}
		catch (ArgumentOutOfRangeException ex2)
		{
			text = "ArgumentOutOfRangeException: " + ex2.Message;
		}
		Debug.LogWarning("Trying to create invalid grid cell " + gridCell + ": " + text);
		throw new ArgumentException("Invalid grid cell " + gridCell + ": " + text);
	}
}
