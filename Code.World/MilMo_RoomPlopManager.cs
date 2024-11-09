using System.Collections.Generic;
using System.Linq;
using Code.Core.Avatar;
using Code.Core.BuddyBackend;
using Code.World.GUI;
using Core;
using UnityEngine;

namespace Code.World;

public sealed class MilMo_RoomPlopManager
{
	private sealed class MilMo_Door
	{
		public MilMo_DoorPlop Marker { get; private set; }

		public MilMo_Room ToRoom { get; private set; }

		public MilMo_Door(MilMo_Room fromRoom, MilMo_Room toRoom, Vector3 position)
		{
			ToRoom = toRoom;
			Marker = new MilMo_DoorPlop(MilMo_World.Instance.UI, position, (fromRoom != null) ? fromRoom.Name : "");
		}

		public void Destroy()
		{
			if (Marker != null)
			{
				Marker.Remove();
				Marker = null;
			}
		}
	}

	private sealed class MilMo_Room
	{
		public string Name { get; private set; }

		public List<MilMo_Avatar> PlayersInside { get; private set; }

		public List<MilMo_Door> Doors { get; private set; }

		public MilMo_Room(string name)
		{
			Doors = new List<MilMo_Door>();
			PlayersInside = new List<MilMo_Avatar>();
			Name = name;
		}

		public void PlayerEnter(MilMo_Avatar avatar)
		{
			if (!PlayersInside.Contains(avatar))
			{
				PlayersInside.Add(avatar);
			}
		}

		public void PlayerLeave(MilMo_Avatar avatar)
		{
			PlayersInside.Remove(avatar);
		}

		public bool HasFriendInside()
		{
			return PlayersInside.Any((MilMo_Avatar avatar) => Singleton<MilMo_BuddyBackend>.Instance.GetBuddy(avatar.Id) != null);
		}

		public void Destroy()
		{
			foreach (MilMo_Door door in Doors)
			{
				door.Destroy();
			}
			Doors.Clear();
		}
	}

	private readonly List<MilMo_Door> _parentDoors = new List<MilMo_Door>();

	private readonly Dictionary<string, MilMo_Room> _rooms = new Dictionary<string, MilMo_Room>();

	public static MilMo_RoomPlopManager Instance { get; private set; }

	private MilMo_RoomPlopManager()
	{
	}

	static MilMo_RoomPlopManager()
	{
		Instance = new MilMo_RoomPlopManager();
	}

	public void PlayerEnteredRoom(MilMo_Avatar avatar, string room)
	{
		if (!_rooms.ContainsKey(room))
		{
			_rooms.Add(room, new MilMo_Room(room));
		}
		_rooms[room].PlayerEnter(avatar);
		UpdatePlops();
	}

	public void PlayerLeftRoom(MilMo_Avatar avatar, string room)
	{
		if (!_rooms.ContainsKey(room))
		{
			_rooms.Add(room, new MilMo_Room(room));
		}
		_rooms[room].PlayerLeave(avatar);
		UpdatePlops();
	}

	public void AddDoor(string fromRoomName, string toRoomName, Vector3 position)
	{
		if (!string.IsNullOrEmpty(toRoomName))
		{
			if (!_rooms.ContainsKey(toRoomName))
			{
				_rooms.Add(toRoomName, new MilMo_Room(toRoomName));
			}
			if (!string.IsNullOrEmpty(fromRoomName) && !_rooms.ContainsKey(fromRoomName))
			{
				_rooms.Add(fromRoomName, new MilMo_Room(fromRoomName));
			}
			MilMo_Room value = null;
			if (!string.IsNullOrEmpty(fromRoomName))
			{
				_rooms.TryGetValue(fromRoomName, out value);
			}
			MilMo_Room toRoom = _rooms[toRoomName];
			MilMo_Door item = new MilMo_Door(value, toRoom, position);
			if (value == null)
			{
				_parentDoors.Add(item);
			}
			else
			{
				value.Doors.Add(item);
			}
			UpdatePlops();
		}
	}

	public void Clear()
	{
		foreach (MilMo_Door parentDoor in _parentDoors)
		{
			parentDoor.Destroy();
		}
		_parentDoors.Clear();
		foreach (MilMo_Room value in _rooms.Values)
		{
			value.Destroy();
		}
		_rooms.Clear();
	}

	private void UpdatePlops()
	{
		foreach (MilMo_Door parentDoor in _parentDoors)
		{
			List<MilMo_Room> visitedRooms = new List<MilMo_Room>();
			int totalPlayerCount = 0;
			bool friendInAnyRoom = false;
			VisitRoom(parentDoor.ToRoom, visitedRooms, ref totalPlayerCount, ref friendInAnyRoom);
			parentDoor.Marker.PlayersInsideCount = totalPlayerCount;
			parentDoor.Marker.FriendInside = friendInAnyRoom;
		}
	}

	private static void VisitRoom(MilMo_Room room, List<MilMo_Room> visitedRooms, ref int totalPlayerCount, ref bool friendInAnyRoom)
	{
		if (visitedRooms.Contains(room))
		{
			return;
		}
		totalPlayerCount += room.PlayersInside.Count;
		if (room.HasFriendInside())
		{
			friendInAnyRoom = true;
		}
		visitedRooms.Add(room);
		foreach (MilMo_Door door in room.Doors)
		{
			door.Marker.PlayersInsideCount = door.ToRoom.PlayersInside.Count;
			door.Marker.FriendInside = door.ToRoom.HasFriendInside();
			VisitRoom(door.ToRoom, visitedRooms, ref totalPlayerCount, ref friendInAnyRoom);
		}
	}
}
