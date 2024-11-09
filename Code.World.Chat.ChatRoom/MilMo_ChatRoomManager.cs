using System.Collections.Generic;
using System.Linq;
using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.World.Player;
using Core;
using Core.Interaction;
using UnityEngine;

namespace Code.World.Chat.ChatRoom;

public class MilMo_ChatRoomManager
{
	private class PlayerInChatRoomInfo
	{
		public readonly MilMo_Avatar Avatar;

		public readonly long ChatRoomId;

		public readonly short SitPointId;

		public PlayerInChatRoomInfo(MilMo_Avatar avatar, long chatRoomId, short sitPointId)
		{
			Avatar = avatar;
			ChatRoomId = chatRoomId;
			SitPointId = sitPointId;
		}
	}

	private readonly List<MilMo_ChatRoom> _chatRooms = new List<MilMo_ChatRoom>();

	private readonly List<PlayerInChatRoomInfo> _playersInUnknownChatRooms = new List<PlayerInChatRoomInfo>();

	private MilMo_ChatRoom _currentPlayerChatroom;

	private MilMo_GenericReaction _jumpButtonReaction;

	private static MilMo_ChatRoomManager _instance;

	public static MilMo_ChatRoomManager Instance => _instance ?? (_instance = new MilMo_ChatRoomManager());

	private MilMo_Player PlayerInstance => MilMo_Player.Instance;

	private MilMo_Avatar AvatarInstance => PlayerInstance?.Avatar;

	private MilMo_ChatRoomManager()
	{
	}

	public void Update()
	{
		if (_currentPlayerChatroom != null)
		{
			return;
		}
		foreach (MilMo_SitPoint item in _chatRooms.SelectMany((MilMo_ChatRoom room) => room.SitPoints))
		{
			item.Update();
		}
	}

	public void AddChatRoom(MilMo_ChatRoom chatRoom)
	{
		_chatRooms.Add(chatRoom);
		for (int num = _playersInUnknownChatRooms.Count - 1; num >= 0; num--)
		{
			if (_playersInUnknownChatRooms[num].Avatar == null || _playersInUnknownChatRooms[num].Avatar.IsDestroyed)
			{
				_playersInUnknownChatRooms.RemoveAt(num);
			}
			else if (_playersInUnknownChatRooms[num].ChatRoomId == chatRoom.Id)
			{
				EnterChatRoom(_playersInUnknownChatRooms[num].ChatRoomId, _playersInUnknownChatRooms[num].SitPointId, _playersInUnknownChatRooms[num].Avatar);
				_playersInUnknownChatRooms.RemoveAt(num);
			}
		}
	}

	public void UnloadChatRooms()
	{
		foreach (MilMo_ChatRoom chatRoom in _chatRooms)
		{
			foreach (MilMo_SitPoint sitPoint in chatRoom.SitPoints)
			{
				if (sitPoint.Occupied)
				{
					MilMo_Avatar occupant = sitPoint.Occupant;
					chatRoom.Leave(occupant);
					occupant.PlayAnimation("Idle");
					if (occupant.Id == AvatarInstance?.Id)
					{
						PlayerLeftChatroom(chatRoom);
					}
				}
			}
			chatRoom.Destroy();
		}
		_chatRooms.Clear();
		_playersInUnknownChatRooms.Clear();
	}

	public void UnloadChatRoom(GameObject gameObject, bool savePlayersInChatroom)
	{
		for (int num = _chatRooms.Count - 1; num >= 0; num--)
		{
			if (!(_chatRooms[num].GameObject != gameObject))
			{
				foreach (MilMo_SitPoint sitPoint in _chatRooms[num].SitPoints)
				{
					if (sitPoint.Occupied)
					{
						MilMo_Avatar occupant = sitPoint.Occupant;
						_chatRooms[num].Leave(occupant);
						occupant.PlayAnimation("Idle");
						if (occupant.Id == AvatarInstance?.Id)
						{
							PlayerLeftChatroom(_chatRooms[num]);
						}
						if (savePlayersInChatroom)
						{
							_playersInUnknownChatRooms.Add(new PlayerInChatRoomInfo(occupant, _chatRooms[num].Id, sitPoint.Id));
						}
					}
				}
				_chatRooms[num].Destroy();
				_chatRooms.RemoveAt(num);
			}
		}
	}

	public void ChatMessageFromPlayer(MilMo_Avatar player)
	{
		foreach (MilMo_ChatRoom item in _chatRooms.Where((MilMo_ChatRoom chatRoom) => chatRoom.HasPlayer(player.Id)))
		{
			item.ParticipantChatted(player);
		}
	}

	public void EnterChatRoom(long roomId, short sitPointId, MilMo_Avatar avatar)
	{
		foreach (MilMo_ChatRoom chatRoom in _chatRooms)
		{
			if (chatRoom.Id == roomId)
			{
				Vector3 position = avatar.Position;
				chatRoom.Enter(sitPointId, avatar);
				if (avatar.Id == AvatarInstance?.Id)
				{
					PlayerEnteredChatroom(chatRoom, position);
				}
				return;
			}
		}
		_playersInUnknownChatRooms.Add(new PlayerInChatRoomInfo(avatar, roomId, sitPointId));
	}

	public void LeaveChatRoom(long roomId, MilMo_Avatar avatar)
	{
		for (int num = _playersInUnknownChatRooms.Count - 1; num >= 0; num--)
		{
			if (roomId == _playersInUnknownChatRooms[num].ChatRoomId && avatar == _playersInUnknownChatRooms[num].Avatar)
			{
				_playersInUnknownChatRooms.RemoveAt(num);
			}
		}
		using (IEnumerator<MilMo_ChatRoom> enumerator = _chatRooms.Where((MilMo_ChatRoom room) => room.Id == roomId).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				MilMo_ChatRoom current = enumerator.Current;
				current.Leave(avatar);
				avatar.PlayAnimation("Idle");
				if (avatar.Id == AvatarInstance?.Id)
				{
					PlayerLeftChatroom(current);
				}
				return;
			}
		}
		avatar.StopSitting();
	}

	private void PlayerEnteredChatroom(MilMo_ChatRoom room, Vector3 posBeforeEnter)
	{
		if (room == null)
		{
			return;
		}
		Singleton<InteractionManager>.Instance.Disable();
		room.HasThePlayer = true;
		MilMo_World.HudHandler.AddLeaveChatroomButton(RequestLeave);
		_jumpButtonReaction = MilMo_EventSystem.Listen("button_Jump", RequestLeave);
		_jumpButtonReaction.Repeating = true;
		foreach (MilMo_SitPoint sitPoint in room.SitPoints)
		{
			if (sitPoint.Occupied && sitPoint.Occupant.Id == AvatarInstance?.Id)
			{
				sitPoint.SetupPlayerController(posBeforeEnter);
			}
		}
		MilMo_World.Instance.Camera.HookupChatCam(room);
		_currentPlayerChatroom = room;
		AvatarInstance?.PlaySoundEffect("Content/Sounds/Batch01/Furniture/ChairSit");
		MilMo_EventSystem.Instance.PostEvent("tutorial_Sit", "");
	}

	private void PlayerLeftChatroom(MilMo_ChatRoom room)
	{
		if (room != null)
		{
			if (Singleton<InteractionManager>.Instance != null)
			{
				Singleton<InteractionManager>.Instance.Enable();
			}
			room.HasThePlayer = false;
			MilMo_World.HudHandler.RemoveLeaveChatroomButton();
			MilMo_EventSystem.RemoveReaction(_jumpButtonReaction);
			_jumpButtonReaction = null;
			MilMo_World.Instance.Camera.LeaveChatRoom();
			_currentPlayerChatroom = null;
			AvatarInstance?.PlaySoundEffect("Content/Sounds/Batch01/Furniture/ChairGetUp");
		}
	}

	public void RequestLeave(object obj)
	{
		if (_currentPlayerChatroom != null)
		{
			Singleton<GameNetwork>.Instance.RequestLeaveChatRoom(_currentPlayerChatroom.Id);
		}
	}
}
