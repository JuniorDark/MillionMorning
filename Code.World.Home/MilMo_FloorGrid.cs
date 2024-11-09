using System;
using System.Collections.Generic;
using Code.Core.Items.Home.FurnitureGrid;
using Code.Core.Items.Home.GridCells;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;
using Code.World.Player;
using UnityEngine;

namespace Code.World.Home;

public class MilMo_FloorGrid
{
	public class Tile
	{
		public MilMo_Furniture CarpetSlot;

		public MilMo_Furniture FloorSlot;

		public void Add(MilMo_Furniture furniture)
		{
			if (furniture.Template.IsCarpet)
			{
				if (CarpetSlot != null && CarpetSlot.Id != furniture.Id)
				{
					throw new MilMo_Home.FurnitureGridException(CarpetSlot, "Attempting to add a carpet to a tile where there is already another carpet.");
				}
				CarpetSlot = furniture;
			}
			else
			{
				if (FloorSlot != null && FloorSlot.Id != furniture.Id)
				{
					throw new MilMo_Home.FurnitureGridException(FloorSlot, "Attempting to add a furniture to the floor slot of a tile where the floor slot is already occupied.");
				}
				FloorSlot = furniture;
			}
		}

		public void Remove(MilMo_Furniture furniture)
		{
			if (CarpetSlot != null && CarpetSlot.Id == furniture.Id)
			{
				CarpetSlot = null;
			}
			if (FloorSlot != null && FloorSlot.Id == furniture.Id)
			{
				FloorSlot = null;
			}
		}

		public bool HasFurniture(MilMo_Furniture furniture)
		{
			if (CarpetSlot != null && CarpetSlot.Id == furniture.Id)
			{
				return true;
			}
			if (FloorSlot != null && FloorSlot.Id == furniture.Id)
			{
				return true;
			}
			return false;
		}

		public MilMo_Furniture TestCollision(MilMo_Furniture furniture)
		{
			if (furniture.Template.IsCarpet)
			{
				if (CarpetSlot != null && CarpetSlot.Id != furniture.Id)
				{
					return CarpetSlot;
				}
				return null;
			}
			if (FloorSlot != null && FloorSlot.Id != furniture.Id)
			{
				return FloorSlot;
			}
			return null;
		}
	}

	public int Rows { get; }

	public int Columns { get; }

	public Tile[][] Tiles { get; }

	public int[][] DebugGrid { get; }

	public MilMo_FloorGrid(int rows, int columns)
	{
		Tiles = new Tile[rows][];
		for (int i = 0; i < Tiles.Length; i++)
		{
			Tiles[i] = new Tile[columns];
		}
		for (int j = 0; j < Tiles.Length; j++)
		{
			for (int k = 0; k < Tiles[j].Length; k++)
			{
				Tiles[j][k] = new Tile();
			}
		}
		DebugGrid = new int[rows][];
		for (int l = 0; l < DebugGrid.Length; l++)
		{
			DebugGrid[l] = new int[columns];
		}
		Rows = rows;
		Columns = columns;
	}

	public bool TileHasFurniture(int row, int column)
	{
		if (row >= Rows || column >= Columns || row < 0 || column < 0)
		{
			throw new ArgumentOutOfRangeException("column", "Attempting to test for furniture on a cell outside the floor grid");
		}
		return Tiles[row][column].FloorSlot != null;
	}

	public void Add(MilMo_AttachableFurniture furniture)
	{
		if (furniture != null && furniture.IsOnFloor)
		{
			FloorGridCell floorTile = furniture.FloorTile;
			MilMo_FurnitureFloorGrid furnitureGrid = new MilMo_FurnitureFloorGrid();
			Add(furniture, furnitureGrid, floorTile);
		}
	}

	public void Add(MilMo_FloorFurniture furniture)
	{
		if (furniture != null)
		{
			Add(furniture, furniture.Grid, furniture.Tile);
		}
	}

	private void Add(MilMo_Furniture furniture, MilMo_FurnitureFloorGrid furnitureGrid, FloorGridCell tile)
	{
		if (furniture == null)
		{
			return;
		}
		int num = tile.Col - Mathf.FloorToInt(furnitureGrid.PivotCol);
		int num2 = tile.Row - Mathf.FloorToInt(furnitureGrid.PivotRow);
		int num3 = num;
		for (int i = 0; i < furnitureGrid.Rows; i++)
		{
			if (num2 >= Rows)
			{
				break;
			}
			if (num2 < 0)
			{
				continue;
			}
			for (int j = 0; j < furnitureGrid.Columns; j++)
			{
				if (num3 >= Columns)
				{
					break;
				}
				if (num3 >= 0)
				{
					if (furnitureGrid.Tiles[i][j] != 0)
					{
						Tiles[num2][num3].Add(furniture);
					}
					num3++;
				}
			}
			num2++;
			num3 = num;
		}
	}

	public void Remove(MilMo_Furniture furniture)
	{
		if (furniture == null)
		{
			return;
		}
		Tile[][] tiles = Tiles;
		foreach (Tile[] array in tiles)
		{
			for (int j = 0; j < array.Length; j++)
			{
				array[j].Remove(furniture);
			}
		}
	}

	public FloorGridCell GetClosestGridCellForFurniture(MilMo_FloorFurniture furniture, FloorGridCell closeToTile)
	{
		if (furniture == null || closeToTile == null)
		{
			return null;
		}
		return GetClosestGridCellForFurniture(furniture, furniture.Grid, furniture.Tile, closeToTile);
	}

	private FloorGridCell GetClosestGridCellForFurniture(MilMo_Furniture furniture, MilMo_FurnitureFloorGrid furnitureGrid, FloorGridCell furnitureTile, FloorGridCell closeToTile)
	{
		FloorGridCell result = null;
		float num = float.MaxValue;
		int num2 = furnitureTile.Col - Mathf.FloorToInt(furnitureGrid.PivotCol);
		int num3 = furnitureTile.Row - Mathf.FloorToInt(furnitureGrid.PivotRow);
		int num4 = num2;
		for (int i = 0; i < furnitureGrid.Rows; i++)
		{
			for (int j = 0; j < furnitureGrid.Columns; j++)
			{
				if (num3 < Rows && num4 < Columns && num3 >= 0 && num4 >= 0 && Tiles[num3][num4].HasFurniture(furniture))
				{
					float num5 = Vector2.SqrMagnitude(new Vector2(num3 - closeToTile.Row, num4 - closeToTile.Col));
					if (num5 < num)
					{
						result = new FloorGridCell(num3, num4);
						num = num5;
					}
				}
				num4++;
			}
			num3++;
			num4 = num2;
		}
		return result;
	}

	public MilMo_PlayerControllerHome.TargetTile GetClosestFreeTileNextToFurniture(MilMo_FloorFurniture furniture, FloorGridCell closeToTile, bool ignoreCollision)
	{
		if (furniture == null || closeToTile == null)
		{
			return null;
		}
		return GetClosestFreeTileNextToFurniture(furniture, furniture.Grid, furniture.Tile, closeToTile, ignoreCollision);
	}

	public MilMo_PlayerControllerHome.TargetTile GetClosestFreeTileNextToFurniture(MilMo_AttachableFurniture furniture, FloorGridCell closeToTile, bool ignoreCollision)
	{
		if (furniture == null || closeToTile == null || !furniture.IsOnFloor)
		{
			return null;
		}
		return GetClosestFreeTileNextToFurniture(furniture, new MilMo_FurnitureFloorGrid(), furniture.FloorTile, closeToTile, ignoreCollision);
	}

	private MilMo_PlayerControllerHome.TargetTile GetClosestFreeTileNextToFurniture(MilMo_Furniture furniture, MilMo_FurnitureFloorGrid furnitureGrid, FloorGridCell furnitureTile, FloorGridCell closeToTile, bool ignoreCollision)
	{
		List<MilMo_PlayerControllerHome.TargetTile> list = new List<MilMo_PlayerControllerHome.TargetTile>();
		int num = furnitureTile.Col - Mathf.FloorToInt(furnitureGrid.PivotCol);
		int num2 = furnitureTile.Row - Mathf.FloorToInt(furnitureGrid.PivotRow);
		int num3 = num;
		for (int i = 0; i < furnitureGrid.Rows; i++)
		{
			for (int j = 0; j < furnitureGrid.Columns; j++)
			{
				if (num2 < Rows && num3 < Columns && num2 >= 0 && num3 >= 0 && furnitureGrid.Tiles[i][j] != 0)
				{
					int num4 = num3;
					int num5 = num2 - 1;
					if (num5 >= 0 && !Tiles[num5][num4].HasFurniture(furniture) && (ignoreCollision || Tiles[num5][num4].FloorSlot == null))
					{
						list.Add(new MilMo_PlayerControllerHome.TargetTile(num5, num4, 180f));
					}
					num4 = num3 + 1;
					num5 = num2;
					if (num4 < Columns && !Tiles[num5][num4].HasFurniture(furniture) && (ignoreCollision || Tiles[num5][num4].FloorSlot == null))
					{
						list.Add(new MilMo_PlayerControllerHome.TargetTile(num5, num4, 270f));
					}
					num4 = num3;
					num5 = num2 + 1;
					if (num5 < Rows && !Tiles[num5][num4].HasFurniture(furniture) && (ignoreCollision || Tiles[num5][num4].FloorSlot == null))
					{
						list.Add(new MilMo_PlayerControllerHome.TargetTile(num5, num4, 0f));
					}
					num4 = num3 - 1;
					num5 = num2;
					if (num4 >= 0 && !Tiles[num5][num4].HasFurniture(furniture) && (ignoreCollision || Tiles[num5][num4].FloorSlot == null))
					{
						list.Add(new MilMo_PlayerControllerHome.TargetTile(num5, num4, 90f));
					}
				}
				num3++;
			}
			num2++;
			num3 = num;
		}
		if (list.Count == 0)
		{
			return null;
		}
		float num6 = float.MaxValue;
		MilMo_PlayerControllerHome.TargetTile result = null;
		foreach (MilMo_PlayerControllerHome.TargetTile item in list)
		{
			float num7 = Vector2.SqrMagnitude(new Vector2(item.Row - closeToTile.Row, item.Col - closeToTile.Col));
			if (num7 < num6)
			{
				result = item;
				num6 = num7;
			}
		}
		return result;
	}

	public MilMo_PlayerControllerHome.TargetTile GetClosestTileNextToTile(FloorGridCell tile, FloorGridCell closeToTile)
	{
		return GetClosestFreeTileNextToTile(tile, closeToTile, ignoreCollision: true);
	}

	public MilMo_PlayerControllerHome.TargetTile GetClosestFreeTileNextToTile(FloorGridCell tile, FloorGridCell closeToTile)
	{
		return GetClosestFreeTileNextToTile(tile, closeToTile, ignoreCollision: false);
	}

	private MilMo_PlayerControllerHome.TargetTile GetClosestFreeTileNextToTile(FloorGridCell tile, FloorGridCell closeToTile, bool ignoreCollision)
	{
		List<MilMo_PlayerControllerHome.TargetTile> list = new List<MilMo_PlayerControllerHome.TargetTile>();
		int row = tile.Row;
		int col = tile.Col;
		if (row < Rows && col < Columns && row >= 0 && col >= 0)
		{
			int num = col;
			int num2 = row - 1;
			if (num2 >= 0 && (ignoreCollision || Tiles[num2][num].FloorSlot == null))
			{
				list.Add(new MilMo_PlayerControllerHome.TargetTile(num2, num, 180f));
			}
			num = col + 1;
			num2 = row;
			if (num < Columns && (ignoreCollision || Tiles[num2][num].FloorSlot == null))
			{
				list.Add(new MilMo_PlayerControllerHome.TargetTile(num2, num, 270f));
			}
			num = col;
			num2 = row + 1;
			if (num2 < Rows && (ignoreCollision || Tiles[num2][num].FloorSlot == null))
			{
				list.Add(new MilMo_PlayerControllerHome.TargetTile(num2, num, 0f));
			}
			num = col - 1;
			num2 = row;
			if (num >= 0 && (ignoreCollision || Tiles[num2][num].FloorSlot == null))
			{
				list.Add(new MilMo_PlayerControllerHome.TargetTile(num2, num, 90f));
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		float num3 = float.MaxValue;
		MilMo_PlayerControllerHome.TargetTile result = null;
		foreach (MilMo_PlayerControllerHome.TargetTile item in list)
		{
			float num4 = Vector2.SqrMagnitude(new Vector2(item.Row - closeToTile.Row, item.Col - closeToTile.Col));
			if (num4 < num3)
			{
				result = item;
				num3 = num4;
			}
		}
		return result;
	}

	public MilMo_RoomGrid.Collision TestCollision(FloorGridCell tile)
	{
		if (tile == null)
		{
			return null;
		}
		if (tile.Row < 0 || tile.Col < 0 || tile.Row >= Rows || tile.Col >= Columns)
		{
			return new MilMo_RoomGrid.Collision(null, wallCollision: true, customCollision: false);
		}
		if (Tiles[tile.Row][tile.Col].FloorSlot != null)
		{
			return new MilMo_RoomGrid.Collision(Tiles[tile.Row][tile.Col].FloorSlot, wallCollision: false, customCollision: false);
		}
		return null;
	}

	public MilMo_RoomGrid.Collision TestCollision(MilMo_AttachableFurniture furniture, FloorGridCell tile)
	{
		if (tile == null || furniture == null)
		{
			return null;
		}
		return TestCollision(furniture, tile, new MilMo_FurnitureFloorGrid(), null);
	}

	public MilMo_RoomGrid.Collision TestCollision(MilMo_FloorFurniture furniture, FloorGridCell tile, float rotation)
	{
		if (tile == null || furniture == null)
		{
			return null;
		}
		return TestCollision(furniture, tile, furniture.Template.GetGridForRotation(rotation), null);
	}

	private MilMo_RoomGrid.Collision TestCollision(MilMo_Furniture furniture, FloorGridCell tile, MilMo_FurnitureFloorGrid furnitureGrid, FloorGridCell additionalCollisionTile)
	{
		int num = tile.Col - Mathf.FloorToInt(furnitureGrid.PivotCol);
		int num2 = tile.Row - Mathf.FloorToInt(furnitureGrid.PivotRow);
		int num3 = num;
		if (num2 < 0 || num3 < 0)
		{
			return new MilMo_RoomGrid.Collision(null, wallCollision: true, customCollision: false);
		}
		for (int i = 0; i < furnitureGrid.Rows; i++)
		{
			for (int j = 0; j < furnitureGrid.Columns; j++)
			{
				if (num2 >= Rows || num3 >= Columns)
				{
					return new MilMo_RoomGrid.Collision(null, wallCollision: true, customCollision: false);
				}
				if (furnitureGrid.Tiles[i][j] != 0)
				{
					MilMo_Furniture milMo_Furniture = Tiles[num2][num3].TestCollision(furniture);
					if (milMo_Furniture != null)
					{
						return new MilMo_RoomGrid.Collision(milMo_Furniture, wallCollision: false, customCollision: false);
					}
				}
				if (additionalCollisionTile != null && num2 == additionalCollisionTile.Row && num3 == additionalCollisionTile.Col && furnitureGrid.Tiles[i][j] != 0)
				{
					return new MilMo_RoomGrid.Collision(null, wallCollision: false, customCollision: true);
				}
				num3++;
			}
			num2++;
			num3 = num;
		}
		return null;
	}

	public FloorGridCell GetClosestSpaceForObject(FloorGridCell closeToPoint, MilMo_AttachableFurniture furniture, int preferredDirection)
	{
		if (furniture == null || closeToPoint == null)
		{
			return null;
		}
		return GetClosestSpaceForObject(closeToPoint, furniture, new MilMo_FurnitureFloorGrid(), preferredDirection);
	}

	public FloorGridCell GetClosestSpaceForObject(FloorGridCell closeToPoint, MilMo_FloorFurniture furniture, float rotation, int preferredDirection)
	{
		if (furniture == null || closeToPoint == null)
		{
			return null;
		}
		return GetClosestSpaceForObject(closeToPoint, furniture, furniture.Template.GetGridForRotation(rotation), preferredDirection);
	}

	public FloorGridCell GetClosestSpaceForObject(FloorGridCell closeToPoint, MilMo_Furniture furniture, MilMo_FurnitureFloorGrid furnitureGrid, int preferredDirection)
	{
		int num = closeToPoint.Row;
		int num2 = closeToPoint.Col;
		switch (preferredDirection)
		{
		case 0:
			num--;
			break;
		case 1:
			num2++;
			break;
		case 2:
			num++;
			break;
		case 3:
			num2--;
			break;
		}
		int num3 = preferredDirection - 1;
		if (num3 < 0)
		{
			num3 = 3;
		}
		int num4 = 0;
		int num5 = 1;
		while (num4 < Rows * Columns - 1)
		{
			if (num >= 0 && num < Rows && num2 >= 0 && num2 < Columns)
			{
				num4++;
				DebugGrid[num][num2] = num4;
				FloorGridCell floorGridCell = new FloorGridCell(num, num2);
				if (TestCollision(furniture, floorGridCell, furnitureGrid, closeToPoint) == null)
				{
					return floorGridCell;
				}
				if (num4 >= Rows * Columns - 1)
				{
					return null;
				}
			}
			int num6 = ((num3 == preferredDirection) ? 1 : 0);
			if (num3 == 0 && num == closeToPoint.Row - (num5 + num6))
			{
				num3 = 3;
			}
			else if ((num3 == 3 && num2 == closeToPoint.Col - (num5 + num6)) || (num3 == 2 && num == closeToPoint.Row + num5 + num6) || (num3 == 1 && num2 == closeToPoint.Col + num5 + num6))
			{
				num3--;
			}
			if (num6 == 1 && num3 != preferredDirection)
			{
				num5++;
			}
			switch (num3)
			{
			case 0:
				num--;
				break;
			case 1:
				num2++;
				break;
			case 2:
				num++;
				break;
			case 3:
				num2--;
				break;
			}
		}
		return null;
	}

	public List<string> GetPossiblePositionsForHomeDeliveryBox(MilMo_AttachableFurniture box)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < Rows; i++)
		{
			for (int j = 0; j < Columns; j++)
			{
				FloorGridCell floorGridCell = new FloorGridCell(i, j);
				if (TestCollision(box, floorGridCell) == null)
				{
					list.Add(floorGridCell.ToString());
				}
			}
		}
		return list;
	}
}
