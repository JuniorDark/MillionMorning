using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Avatar;
using Code.Core.Global;
using Code.Core.Items.Home;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.Chat.ChatRoom;

public class MilMo_ChatRoom
{
	private MilMo_Avatar _currentTalker;

	public long Id { get; }

	public bool HasThePlayer { get; set; }

	public List<MilMo_Transform> MainCameraPositions { get; }

	public List<MilMo_SitPoint> SitPoints { get; } = new List<MilMo_SitPoint>();


	public MilMo_Avatar LastTalker { get; private set; }

	public float AverageHeadHeight { get; private set; }

	public GameObject GameObject { get; private set; }

	public MilMo_ChatRoom(MilMo_ChatRoomTemplate template, GameObject gameObject)
		: this(template.Id, template.SitPoints, template.MainCameraPositions, gameObject)
	{
	}

	public MilMo_ChatRoom(long id, List<MilMo_SitPointTemplate> sitPoints, GameObject gameObject)
		: this(id, sitPoints, new List<MilMo_Transform>(), gameObject)
	{
	}

	private MilMo_ChatRoom(long id, List<MilMo_SitPointTemplate> sitPoints, List<MilMo_Transform> cameraPositions, GameObject gameObject)
	{
		Id = id;
		MainCameraPositions = cameraPositions;
		foreach (MilMo_SitPointTemplate sitPoint in sitPoints)
		{
			SitPoints.Add(new MilMo_SitPoint(this, sitPoint));
		}
		SetGameObject(gameObject);
	}

	public void ParticipantChatted(MilMo_Avatar talker)
	{
		if (!HasThePlayer)
		{
			return;
		}
		using IEnumerator<MilMo_SitPoint> enumerator = SitPoints.Where((MilMo_SitPoint sitPoint) => sitPoint.Occupied && sitPoint.Occupant.Id == talker.Id).GetEnumerator();
		if (enumerator.MoveNext())
		{
			enumerator.Current.OccupantChatted();
			if (_currentTalker != null && _currentTalker.Id != talker.Id)
			{
				LastTalker = _currentTalker;
			}
			_currentTalker = talker;
		}
	}

	public void Destroy()
	{
		MainCameraPositions.Clear();
		foreach (MilMo_SitPoint sitPoint in SitPoints)
		{
			sitPoint.Destroy();
		}
	}

	public void Enter(short sitPointId, MilMo_Avatar avatar)
	{
		using (IEnumerator<MilMo_SitPoint> enumerator = SitPoints.Where((MilMo_SitPoint sitPoint) => sitPoint.Id == sitPointId).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				enumerator.Current.Sit(avatar);
				CalculateAverageHeadHeight();
				return;
			}
		}
		Debug.LogWarning($"Couldn't find sit point {sitPointId} in chat room {Id}");
	}

	public void Leave(MilMo_Avatar avatar)
	{
		using (IEnumerator<MilMo_SitPoint> enumerator = SitPoints.Where((MilMo_SitPoint sitPoint) => sitPoint.Occupied && sitPoint.Occupant.Id == avatar.Id).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				enumerator.Current.Leave();
				CalculateAverageHeadHeight();
				if (_currentTalker != null && avatar.Id == _currentTalker.Id)
				{
					_currentTalker = null;
				}
				if (LastTalker != null && avatar.Id == LastTalker.Id)
				{
					LastTalker = null;
				}
				return;
			}
		}
		Debug.LogWarning($"Couldn't find avatar {avatar.Name} in chat room {Id}");
	}

	public bool HasPlayer(string playerId)
	{
		return SitPoints.Any((MilMo_SitPoint sitPoint) => sitPoint.Occupied && sitPoint.Occupant.Id == playerId);
	}

	private void SetGameObject(GameObject gameObject)
	{
		GameObject = gameObject;
		if (!(GameObject == null))
		{
			ClearNodesFromProp();
			AddNodesToProp();
		}
	}

	private void ClearNodesFromProp()
	{
		Transform[] componentsInChildren = GameObject.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (transform != null && transform.name.StartsWith("Chat", StringComparison.InvariantCultureIgnoreCase))
			{
				MilMo_Global.Destroy(transform.gameObject);
			}
		}
		foreach (MilMo_SitPoint sitPoint in SitPoints)
		{
			sitPoint.Destroy();
		}
	}

	private void AddNodesToProp()
	{
		Transform transform = GameObject.GetComponentsInChildren<Transform>(includeInactive: true).FirstOrDefault((Transform child) => child.name.Equals("Mesh"));
		if (transform == null)
		{
			Debug.LogWarning($"{GameObject.name}: Failed to add nodes for chat room with id {Id}." + "No child object named \"Mesh\" was found in the game object.");
			return;
		}
		foreach (MilMo_Transform mainCameraPosition in MainCameraPositions)
		{
			if (mainCameraPosition != null)
			{
				GameObject gameObject = new GameObject("ChatCamera");
				gameObject.transform.position = mainCameraPosition.Position;
				gameObject.transform.rotation = Quaternion.Euler(mainCameraPosition.EulerRotation);
				gameObject.transform.parent = transform;
			}
		}
		foreach (MilMo_SitPoint sitPoint in SitPoints)
		{
			sitPoint.AddNodesToProp(transform);
		}
	}

	private void CalculateAverageHeadHeight()
	{
		AverageHeadHeight = 0f;
		int num = 0;
		foreach (MilMo_SitPoint item in SitPoints.Where((MilMo_SitPoint point) => point.Occupied && point.Occupant.Head != null))
		{
			AverageHeadHeight += item.Occupant.Head.position.y;
			num++;
		}
		if (num > 0)
		{
			AverageHeadHeight /= num;
		}
	}
}
