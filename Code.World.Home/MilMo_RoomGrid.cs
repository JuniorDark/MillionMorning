using System;
using System.Collections.Generic;
using Code.Core.Items.Home.FurnitureGrid;
using Code.Core.Items.Home.GridCells;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;
using Code.World.Player;
using UnityEngine;

namespace Code.World.Home;

public class MilMo_RoomGrid
{
	public class Collision
	{
		public readonly MilMo_Furniture CollidedFurniture;

		public readonly bool WallCollision;

		public readonly bool FurnitureCollision;

		public bool CustomCollision;

		public bool CurtainWithoutWindow;

		public Collision(MilMo_Furniture collidedFurniture, bool wallCollision, bool customCollision)
			: this(collidedFurniture, wallCollision, customCollision, curtainWithoutWindow: false)
		{
		}

		public Collision(MilMo_Furniture collidedFurniture, bool wallCollision, bool customCollision, bool curtainWithoutWindow)
		{
			CollidedFurniture = collidedFurniture;
			FurnitureCollision = collidedFurniture != null;
			WallCollision = wallCollision;
			CustomCollision = customCollision;
			CurtainWithoutWindow = curtainWithoutWindow;
		}
	}

	private readonly Vector3 _offsetFromOrigo;

	public MilMo_FloorGrid Floor { get; }

	public MilMo_WallGrid Walls { get; }

	public int Rows => Floor.Rows;

	public int Columns => Floor.Columns;

	public MilMo_RoomGrid(int rows, int columns, Vector3 offsetFromOrigo)
	{
		Floor = new MilMo_FloorGrid(rows, columns);
		Walls = new MilMo_WallGrid(rows, columns);
		_offsetFromOrigo = offsetFromOrigo;
	}

	public void Add(MilMo_Furniture furniture)
	{
		if (!(furniture is MilMo_FloorFurniture furniture2))
		{
			if (!(furniture is MilMo_WallFurniture furniture3))
			{
				if (furniture is MilMo_AttachableFurniture { IsOnFloor: not false } milMo_AttachableFurniture)
				{
					Floor.Add(milMo_AttachableFurniture);
				}
			}
			else
			{
				Walls.Add(furniture3);
			}
		}
		else
		{
			Floor.Add(furniture2);
		}
	}

	public void Remove(MilMo_Furniture furniture)
	{
		if (!(furniture is MilMo_FloorFurniture furniture2))
		{
			if (!(furniture is MilMo_WallFurniture furniture3))
			{
				if (furniture is MilMo_AttachableFurniture { IsOnFloor: not false } milMo_AttachableFurniture)
				{
					Floor.Remove(milMo_AttachableFurniture);
				}
			}
			else
			{
				Walls.Remove(furniture3);
			}
		}
		else
		{
			Floor.Remove(furniture2);
		}
	}

	public Collision TestCollision(FloorGridCell tile)
	{
		return Floor.TestCollision(tile);
	}

	public Collision TestCollision(MilMo_FloorFurniture furniture, FloorGridCell tile, float rotation)
	{
		return Floor.TestCollision(furniture, tile, rotation);
	}

	public Collision TestCollision(MilMo_WallFurniture furniture, WallGridCell tile)
	{
		return Walls.TestCollision(furniture, tile);
	}

	public MilMo_PlayerControllerHome.TargetTile GetClosestFreeTileNextToFurniture(MilMo_FloorFurniture furniture, FloorGridCell closeToTile)
	{
		return GetClosestFreeTileNextToFurniture(furniture, closeToTile, ignoreCollision: false);
	}

	public MilMo_PlayerControllerHome.TargetTile GetClosestFreeTileNextToFurniture(MilMo_AttachableFurniture furniture, FloorGridCell closeToTile)
	{
		return GetClosestFreeTileNextToFurniture(furniture, closeToTile, ignoreCollision: false);
	}

	public MilMo_PlayerControllerHome.TargetTile GetClosestTileNextToFurniture(MilMo_FloorFurniture furniture, FloorGridCell closeToTile)
	{
		return GetClosestFreeTileNextToFurniture(furniture, closeToTile, ignoreCollision: true);
	}

	public MilMo_PlayerControllerHome.TargetTile GetClosestTileNextToFurniture(MilMo_AttachableFurniture furniture, FloorGridCell closeToTile)
	{
		return GetClosestFreeTileNextToFurniture(furniture, closeToTile, ignoreCollision: true);
	}

	private MilMo_PlayerControllerHome.TargetTile GetClosestFreeTileNextToFurniture(MilMo_FloorFurniture furniture, FloorGridCell closeToTile, bool ignoreCollision)
	{
		return Floor.GetClosestFreeTileNextToFurniture(furniture, closeToTile, ignoreCollision);
	}

	private MilMo_PlayerControllerHome.TargetTile GetClosestFreeTileNextToFurniture(MilMo_AttachableFurniture furniture, FloorGridCell closeToTile, bool ignoreCollision)
	{
		return Floor.GetClosestFreeTileNextToFurniture(furniture, closeToTile, ignoreCollision);
	}

	public MilMo_PlayerControllerHome.TargetTile GetClosestTileNextToFurniture(MilMo_WallFurniture furniture, FloorGridCell closeToTile)
	{
		return GetClosestFreeTileNextToFurniture(furniture, closeToTile, ignoreCollision: true);
	}

	public MilMo_PlayerControllerHome.TargetTile GetClosestFreeTileNextToFurniture(MilMo_WallFurniture furniture, FloorGridCell closeToTile)
	{
		return GetClosestFreeTileNextToFurniture(furniture, closeToTile, ignoreCollision: false);
	}

	private MilMo_PlayerControllerHome.TargetTile GetClosestFreeTileNextToFurniture(MilMo_WallFurniture furniture, FloorGridCell closeToTile, bool ignoreCollision)
	{
		if (furniture == null || closeToTile == null)
		{
			return null;
		}
		List<MilMo_PlayerControllerHome.TargetTile> list = new List<MilMo_PlayerControllerHome.TargetTile>();
		MilMo_FurnitureWallGrid grid = furniture.Grid;
		int wallIndex = furniture.Tile.WallIndex;
		int num;
		int num2;
		int num3;
		int num4;
		if (wallIndex == 0 || wallIndex == 2)
		{
			num = furniture.Tile.Tile - Mathf.FloorToInt(grid.Pivot);
			num2 = 1;
			num3 = 0;
			num4 = ((furniture.Tile.WallIndex != 0) ? (Floor.Rows - 1) : 0);
		}
		else
		{
			num4 = furniture.Tile.Tile - Mathf.FloorToInt(grid.Pivot);
			num2 = 0;
			num3 = 1;
			num = ((furniture.Tile.WallIndex != 3) ? (Floor.Columns - 1) : 0);
		}
		float rotation = furniture.Tile.WallIndex * 90;
		int num5 = num4;
		int num6 = num;
		for (int i = 0; i < grid.Width; i++)
		{
			if (num5 < Floor.Rows && num6 < Floor.Columns && num5 >= 0 && num6 >= 0 && (ignoreCollision || !Floor.TileHasFurniture(num5, num6)))
			{
				list.Add(new MilMo_PlayerControllerHome.TargetTile(num5, num6, rotation));
			}
			num6 += num2;
			num5 += num3;
		}
		if (list.Count == 0)
		{
			return null;
		}
		float num7 = float.MaxValue;
		MilMo_PlayerControllerHome.TargetTile result = null;
		foreach (MilMo_PlayerControllerHome.TargetTile item in list)
		{
			float num8 = Vector2.SqrMagnitude(new Vector2(item.Row - closeToTile.Row, item.Col - closeToTile.Col));
			if (num8 < num7)
			{
				result = item;
				num7 = num8;
			}
		}
		return result;
	}

	public FloorGridCell GetGridCellAtPosition(Vector3 pos)
	{
		return GetGridCellAtPosition(pos.x, pos.z);
	}

	public FloorGridCell GetGridCellAtPosition(float x, float z)
	{
		z -= _offsetFromOrigo.z;
		x -= _offsetFromOrigo.x;
		z = 0f - z;
		x = Mathf.Min(Mathf.Max(x, 0f), (float)Columns * 1f);
		z = Mathf.Min(Mathf.Max(z, 0f), (float)Rows * 1f);
		int col = Mathf.FloorToInt(x / 1f);
		return new FloorGridCell(Mathf.FloorToInt(z / 1f), col);
	}

	public WallGridCell GetWallGridCellAtPosition(Vector3 pos)
	{
		return GetWallGridCellAtPosition(pos.x, pos.z);
	}

	private WallGridCell GetWallGridCellAtPosition(float x, float z)
	{
		z -= _offsetFromOrigo.z;
		x -= _offsetFromOrigo.x;
		z = 0f - z;
		x = Mathf.Min(Mathf.Max(x, 0f), (float)Columns * 1f);
		z = Mathf.Min(Mathf.Max(z, 0f), (float)Rows * 1f);
		float num = 0.5f;
		int num2;
		if (x > 0f)
		{
			if (z < num)
			{
				num2 = 0;
			}
			else if (z > (float)Rows * 1f - num)
			{
				num2 = 2;
			}
			else
			{
				if (!(z > 0f) || !(x > (float)Columns * 1f - num))
				{
					return null;
				}
				num2 = 1;
			}
		}
		else
		{
			if (!(z > 0f - num))
			{
				return null;
			}
			num2 = 3;
		}
		int tile = ((num2 != 0 && num2 != 2) ? Math.Min(Math.Max(Mathf.FloorToInt(z / 1f), 0), Rows - 1) : Math.Min(Math.Max(Mathf.FloorToInt(x / 1f), 0), Columns - 1));
		return new WallGridCell(num2, tile);
	}

	public FloorGridCell GetClosestGridCellForFurniture(MilMo_FloorFurniture furniture, FloorGridCell closeToCell)
	{
		return Floor.GetClosestGridCellForFurniture(furniture, closeToCell);
	}
}
