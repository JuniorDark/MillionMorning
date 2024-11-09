using System;
using System.Runtime.InteropServices;
using Code.Core.Items.Home.FurnitureGrid;
using Code.Core.Items.Home.GridCells;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;
using UnityEngine;

namespace Code.World.Home;

public class MilMo_WallGrid
{
	public class Tile
	{
		public MilMo_WallFurniture DefaultSlot;

		public MilMo_WallFurniture CurtainSlot;

		public void Add(MilMo_WallFurniture furniture)
		{
			if (furniture.Template.IsCurtain)
			{
				if (CurtainSlot != null && CurtainSlot.Id != furniture.Id)
				{
					throw new MilMo_Home.FurnitureGridException(CurtainSlot, "Attempting to add a curtain to a tile where there is already another curtain.");
				}
				CurtainSlot = furniture;
			}
			else
			{
				if (DefaultSlot != null && DefaultSlot.Id != furniture.Id)
				{
					throw new MilMo_Home.FurnitureGridException(DefaultSlot, "Attempting to add a wall furniture to an occupied tile.");
				}
				DefaultSlot = furniture;
			}
		}

		public void Remove(MilMo_WallFurniture furniture)
		{
			if (CurtainSlot != null && CurtainSlot.Id == furniture.Id)
			{
				CurtainSlot = null;
			}
			if (DefaultSlot != null && DefaultSlot.Id == furniture.Id)
			{
				DefaultSlot = null;
				CurtainSlot = null;
			}
		}

		public bool TestCollision(MilMo_WallFurniture furniture, out MilMo_WallFurniture collidedFurniture)
		{
			collidedFurniture = null;
			if (furniture.Template.IsCurtain)
			{
				if (CurtainSlot == null || CurtainSlot.Id == furniture.Id)
				{
					return false;
				}
				collidedFurniture = CurtainSlot;
				return true;
			}
			if (DefaultSlot == null || DefaultSlot.Id == furniture.Id)
			{
				return false;
			}
			collidedFurniture = DefaultSlot;
			return true;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct WallIndex
	{
		public const byte NORTH = 0;

		public const byte EAST = 1;

		public const byte SOUTH = 2;

		public const byte WEST = 3;
	}

	private const byte DEFAULT = 1;

	private const byte CURTAIN = 2;

	private readonly Tile[][] _tiles = new Tile[4][];

	private readonly int[][] _debugGrid = new int[4][];

	public MilMo_WallGrid(int rows, int columns)
	{
		_tiles[0] = new Tile[columns];
		_tiles[1] = new Tile[rows];
		_tiles[2] = new Tile[columns];
		_tiles[3] = new Tile[rows];
		for (int i = 0; i < _tiles.GetLength(0); i++)
		{
			for (int j = 0; j < _tiles[i].Length; j++)
			{
				_tiles[i][j] = new Tile();
			}
		}
		_debugGrid[0] = new int[columns];
		_debugGrid[1] = new int[rows];
		_debugGrid[2] = new int[columns];
		_debugGrid[3] = new int[rows];
	}

	public void Add(MilMo_WallFurniture furniture)
	{
		if (furniture == null)
		{
			return;
		}
		MilMo_FurnitureWallGrid grid = furniture.Grid;
		int num = furniture.Tile.Tile - Mathf.FloorToInt(grid.Pivot);
		Tile[] array = _tiles[furniture.Tile.WallIndex];
		for (int i = 0; i < grid.Width; i++)
		{
			if (num >= array.Length)
			{
				break;
			}
			array[num].Add(furniture);
			num++;
		}
	}

	public void Remove(MilMo_WallFurniture furniture)
	{
		if (furniture == null)
		{
			return;
		}
		Tile[][] tiles = _tiles;
		foreach (Tile[] array in tiles)
		{
			for (int j = 0; j < array.Length; j++)
			{
				array[j].Remove(furniture);
			}
		}
	}

	public WallGridCell GetClosestSpaceForObject(WallGridCell preferredTile, MilMo_WallFurniture furniture)
	{
		int wallIndex = preferredTile.WallIndex;
		int tile = Math.Min(Math.Max(preferredTile.Tile, 0), _tiles[wallIndex].Length - 1);
		int num = 0;
		int num2 = _tiles[0].Length * 2 + _tiles[1].Length * 2;
		int num3 = 1;
		WallGridCell wallGridCell = new WallGridCell(wallIndex, tile);
		WallGridCell wallGridCell2 = new WallGridCell(wallIndex, tile);
		WallGridCell tile2 = wallGridCell;
		while (num < num2)
		{
			if (tile2.Tile >= 0 && tile2.Tile < _tiles[tile2.WallIndex].Length)
			{
				num++;
				_debugGrid[tile2.WallIndex][tile2.Tile] = num;
				if (TestCollision(furniture, tile2) == null)
				{
					return tile2;
				}
				if (num >= num2)
				{
					return null;
				}
				num3 = -num3;
				tile2 = ((num3 != 1) ? wallGridCell2 : wallGridCell);
				TileMoveNext(ref tile2, num3);
				continue;
			}
			throw new ApplicationException("Got invalid tile when iterating wall tiles. Visited tiles: " + num + ", current tile: " + tile2);
		}
		return null;
	}

	private void TileMoveNext(ref WallGridCell tile, int direction)
	{
		if (Mathf.Abs(direction) != 1)
		{
			throw new ArgumentException("Direction must be either -1 or 1");
		}
		int num = 1;
		if (tile.WallIndex > 1)
		{
			num = -1;
		}
		tile.Tile += num * direction;
		if (tile.Tile < 0 || tile.Tile >= _tiles[tile.WallIndex].Length)
		{
			int wallIndex = tile.WallIndex;
			tile.WallIndex += direction;
			if (tile.WallIndex > 3)
			{
				tile.WallIndex = 0;
			}
			else if (tile.WallIndex < 0)
			{
				tile.WallIndex = 3;
			}
			if (wallIndex == 0 || wallIndex == 3)
			{
				tile.Tile = 0;
			}
			else
			{
				tile.Tile = _tiles[tile.WallIndex].Length - 1;
			}
		}
	}

	public MilMo_RoomGrid.Collision TestCollision(MilMo_WallFurniture furniture, WallGridCell tile)
	{
		if (tile == null || furniture == null)
		{
			return null;
		}
		int num = tile.Tile - Mathf.FloorToInt(furniture.Grid.Pivot);
		Tile[] array = _tiles[tile.WallIndex];
		if (num < 0)
		{
			return new MilMo_RoomGrid.Collision(null, wallCollision: true, customCollision: false);
		}
		for (int i = 0; i < furniture.Grid.Width; i++)
		{
			if (num >= array.Length)
			{
				return new MilMo_RoomGrid.Collision(null, wallCollision: true, customCollision: false);
			}
			if (array[num].TestCollision(furniture, out var collidedFurniture))
			{
				if (collidedFurniture == null)
				{
					return new MilMo_RoomGrid.Collision(null, wallCollision: false, customCollision: false, curtainWithoutWindow: true);
				}
				return new MilMo_RoomGrid.Collision(collidedFurniture, wallCollision: false, customCollision: false);
			}
			num++;
		}
		return null;
	}

	public Tile[] GetWall(byte index)
	{
		if (index > _tiles.GetLength(0))
		{
			throw new IndexOutOfRangeException("Trying to get invalid wall " + index);
		}
		return _tiles[index];
	}

	public int[] GetWallDebugInfo(byte index)
	{
		if (index > _debugGrid.GetLength(0))
		{
			throw new IndexOutOfRangeException("Trying to get debug info for invalid wall " + index);
		}
		return _debugGrid[index];
	}
}
