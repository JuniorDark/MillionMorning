using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.Core.Items.Home;
using Code.Core.Network;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Code.World.Player;
using Core;
using Core.Interaction;
using UI;
using UI.Marker.Object;
using UnityEngine;

namespace Code.World.Chat.ChatRoom;

public class MilMo_SitPoint : IHasInteraction
{
	private const float M_CAMERA_FOCUS_TIME = 5f;

	private const float INTERACTION_RADIUS = 0.7f;

	private GameObject _sitNode;

	private bool _cameraFocus;

	private MilMo_TimerEvent _timeoutEvent;

	private float _sqrDistanceToPlayer;

	private readonly MilMo_SitPointTemplate _template;

	private readonly MilMo_ChatRoom _room;

	private ObjectMarker _marker;

	public bool Occupied => Occupant != null;

	public MilMo_Avatar Occupant { get; private set; }

	public short Id => _template.Id;

	public bool CameraFocus
	{
		get
		{
			if (_cameraFocus)
			{
				return Occupied;
			}
			return false;
		}
	}

	private bool PlayerInRange { get; set; }

	private MilMo_Player PlayerInstance => MilMo_Player.Instance;

	private MilMo_Avatar AvatarInstance => PlayerInstance?.Avatar;

	public MilMo_SitPoint(MilMo_ChatRoom room, MilMo_SitPointTemplate template)
	{
		_room = room;
		_template = template;
	}

	public void Update()
	{
		if ((bool)_sitNode && AvatarInstance != null && (bool)AvatarInstance.GameObject)
		{
			_sqrDistanceToPlayer = Vector3.SqrMagnitude(AvatarInstance.Position - GetPosition());
			PlayerInRange = _sqrDistanceToPlayer <= 0.7f;
		}
	}

	public void OccupantChatted()
	{
		_cameraFocus = true;
		MilMo_EventSystem.Instance.PostEvent("sitpoint_camera_focus", this);
		MilMo_EventSystem.RemoveTimerEvent(_timeoutEvent);
		_timeoutEvent = MilMo_EventSystem.At(5f, CameraFocusTimeout);
	}

	public void CameraFocusTimeout()
	{
		_cameraFocus = false;
		MilMo_EventSystem.Instance.PostEvent("sitpoint_camera_release_focus", this);
	}

	public void Destroy()
	{
		if ((bool)_marker)
		{
			_marker.Remove();
		}
	}

	public int GetPrio()
	{
		return 2;
	}

	public void UseReaction()
	{
		if (!Occupied)
		{
			Singleton<GameNetwork>.Instance.RequestEnterChatRoom(_room.Id, _template.Id);
		}
	}

	public Vector3 GetPosition()
	{
		if (!(_sitNode != null))
		{
			return Vector3.zero;
		}
		return _sitNode.transform.position;
	}

	public void RefreshMarkerPosition()
	{
		if ((bool)_marker)
		{
			_marker.SetPosition(GetPosition());
		}
	}

	public Vector3 GetMarkerOffset()
	{
		return new Vector3(0f, 0.5f, 0f);
	}

	public string GetInteractionVerb()
	{
		return MilMo_Localization.GetLocString("Interact_Sit")?.String;
	}

	public Interactable.InteractionType GetInteractionType()
	{
		return Interactable.InteractionType.PickDown;
	}

	public void Sit(MilMo_Avatar avatar)
	{
		Occupant = avatar;
		Occupant.Sit(_sitNode.transform, "Root", _template.Pose);
		_marker.Disable(isDisabled: true);
	}

	public void Leave()
	{
		if (Occupant != null)
		{
			_cameraFocus = false;
			Occupant.StopSitting();
			Occupant = null;
			_marker.Disable(isDisabled: false);
		}
	}

	public void SetupPlayerController(Vector3 posBeforeEnter)
	{
		Vector3 forward = posBeforeEnter - _sitNode.transform.position;
		forward.y = 0f;
		MilMo_PlayerControllerChat.ExitPoint = new MilMo_Transform(posBeforeEnter, Quaternion.LookRotation(forward).eulerAngles);
	}

	public void AddNodesToProp(Transform parent)
	{
		if (_template != null)
		{
			string name = "Chat" + _template.Pose + ((_template.Id > 9) ? _template.Id.ToString() : ("0" + _template.Id));
			_sitNode = new GameObject(name);
			_sitNode.transform.localPosition = _template.Position;
			_sitNode.transform.localRotation = Quaternion.Euler(_template.Rotation);
			_sitNode.transform.SetParent(parent, !_template.RelativeTransform);
			SetupObjectMarker();
		}
	}

	private void SetupObjectMarker()
	{
		MilMo_LocString objectName = new MilMo_LocString("", removeTags: false);
		_marker = WorldSpaceManager.GetWorldSpaceObject<ObjectMarker>(ObjectMarker.AddressableAddressCapsule);
		if (_marker != null)
		{
			_marker.Initialize(this, objectName, 0.7f);
		}
	}
}
