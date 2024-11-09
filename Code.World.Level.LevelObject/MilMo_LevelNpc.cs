using System;
using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Utility;
using Code.World.Player;
using Core;
using Core.Analytics;
using Core.Interaction;
using UI;
using UI.Marker.NPC;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public class MilMo_LevelNpc : MilMo_LevelObject, IHasInteraction
{
	public static class InteractionState
	{
		public const byte HAS_NEXT_QUEST = 0;

		public const byte QUEST_ACTIVE = 1;

		public const byte QUEST_TO_FINISH = 2;

		public const byte NO_MORE_QUESTS = 3;

		public const byte SHOP = 4;

		public const byte TRAVEL = 5;

		public static string GetInteractionType(int type)
		{
			return type switch
			{
				0 => "has_next_quest", 
				1 => "quest_active", 
				2 => "quest_to_finish", 
				3 => "no_more_quests", 
				4 => "shop", 
				5 => "travel", 
				_ => "unknown", 
			};
		}
	}

	public static float HeadScale = 1f;

	private const float NPC_TURN_TO_PLAYER_SPEED = 1.2f;

	private const float NPC_LOOK_AT_RANGE = 7f;

	private const string HEAD_TIP_BONE = "#HeadTip";

	private const string HEAD_SEGMENT_START = "#NeckBase";

	private const string HEAD_SEGMENT_END = "#NeckLo";

	private const string SPINE_SEGMENT_START = "#SpineLumbarLo";

	private const string SPINE_SEGMENT_END = "#SpineThoracicBase";

	private float _interactionRadius;

	private float _lookAtRange = 7f;

	private bool _inPlayerRange;

	private Transform _head;

	private byte _interactionState = 3;

	private MilMo_LocString _interactionVerb;

	private NPCMarker _interactionMarker;

	private Transform _player;

	private Transform _playerHead;

	private Quaternion _headRotation;

	private bool _useTurnToPlayer = true;

	private HeadLookController _lookAtController;

	private readonly MilMo_GenericReaction _mAvatarReloadedListener;

	private bool _turnToPlayer;

	public float Height { get; private set; }

	public Transform Head { get; private set; }

	public MilMo_LocString Name { get; private set; }

	public string TemplateIdentifier { get; private set; }

	public string GameObjectName { get; private set; }

	public int NumberOfQuests { get; private set; }

	public int CompletedQuests { get; set; }

	public bool IsFacingPlayer { get; }

	public bool HasExitArrow { get; private set; }

	public string ExitArrowIdentifier { get; private set; }

	public MilMo_LevelNpc(Transform player, Transform playerHead)
		: base("Content/Creatures/", useSpawnEffect: false)
	{
		Height = 1.5f;
		IsFacingPlayer = false;
		_player = player;
		_playerHead = playerHead;
		_mAvatarReloadedListener = MilMo_EventSystem.Listen("local_avatar_reloaded", LocalAvatarReloaded);
		_mAvatarReloadedListener.Repeating = true;
	}

	private void LocalAvatarReloaded(object avatarAsObject)
	{
		if (avatarAsObject is MilMo_Avatar milMo_Avatar)
		{
			_player = milMo_Avatar.GameObject.transform;
			_playerHead = milMo_Avatar.Head;
		}
	}

	public override void Unload()
	{
		MilMo_EventSystem.RemoveReaction(_mAvatarReloadedListener);
		if (_interactionMarker != null)
		{
			_interactionMarker.Remove();
		}
		base.Unload();
	}

	public override void Read(Code.Core.Network.types.LevelObject npc, OnReadDone callback)
	{
		Npc npc2 = npc as Npc;
		if (npc2 == null)
		{
			Debug.LogWarning("Got non NPC level object when creating NPC.");
			callback?.Invoke(success: false, null);
		}
		base.Read(npc, callback);
		if (npc2 != null)
		{
			_interactionRadius = (float)Math.Sqrt(npc2.GetSqrInteractionRange());
			_useTurnToPlayer = npc2.GetUseTurnToPlayer() != 0;
			_lookAtRange = Mathf.Max(_interactionRadius, 7f);
			SpawnRotation = new Vector3(npc2.GetRotation().GetX(), npc2.GetRotation().GetY(), npc2.GetRotation().GetZ());
			VisualRepName = npc2.GetVisualRep();
			Name = MilMo_Localization.GetLocString(npc2.GetName());
			TemplateIdentifier = npc2.GetTemplateIdentifier();
			NumberOfQuests = npc2.GetNrOfQuests();
			_interactionState = (byte)npc2.GetInteractionState();
			_interactionVerb = MilMo_Localization.GetLocString(npc2.GetInteractionVerb());
			ExitArrow exitArrow = npc2.GetExitArrow();
			if (exitArrow != null)
			{
				HasExitArrow = true;
				exitArrow.GetHeightOffset();
				ExitArrowIdentifier = exitArrow.GetIdentifier();
			}
		}
		FinishRead(null, timeOut: false);
	}

	protected override void FinishRead(MilMo_Template template, bool timeOut)
	{
		if (timeOut)
		{
			Debug.LogWarning("Failed to read " + Name);
		}
		else
		{
			base.FinishRead(template, timeOut: false);
		}
	}

	protected override bool FinishLoad()
	{
		if (!base.FinishLoad())
		{
			return false;
		}
		GameObjectName = MilMo_Utility.RemoveCloneFromName(base.GameObject.name);
		string text = GameObjectName;
		int num = text.IndexOf("Mod", StringComparison.Ordinal);
		if (num >= 0)
		{
			text = text.Substring(0, num);
		}
		Head = MilMo_Utility.GetChild(base.GameObject, "#HeadTip".Replace("#", text));
		if (Head != null)
		{
			Height = Head.transform.position.y - base.GameObject.transform.position.y;
		}
		else if (_useTurnToPlayer)
		{
			Debug.LogWarning("Failed to find bone " + text + "HeadTip for npc " + Name);
		}
		_interactionMarker = WorldSpaceManager.GetWorldSpaceObject<NPCMarker>(NPCMarker.AddressableAddress);
		_interactionMarker.Initialize(_player, this, Name, _interactionRadius, _interactionState);
		Transform child = MilMo_Utility.GetChild(base.GameObject, "#NeckBase".Replace("#", text));
		Transform child2 = MilMo_Utility.GetChild(base.GameObject, "#NeckLo".Replace("#", text));
		Transform child3 = MilMo_Utility.GetChild(base.GameObject, "#SpineLumbarLo".Replace("#", text));
		Transform child4 = MilMo_Utility.GetChild(base.GameObject, "#SpineThoracicBase".Replace("#", text));
		bool flag = child != null && child2 != null;
		bool flag2 = child3 != null && child4 != null;
		if (flag || flag2)
		{
			_lookAtController = base.GameObject.AddComponent<HeadLookController>();
			if (_lookAtController != null)
			{
				_lookAtController.enabled = false;
				_lookAtController.segments = new BendingSegment[(!(flag && flag2)) ? 1 : 2];
				_lookAtController.rootNode = base.GameObject.transform;
				_lookAtController.nonAffectedJoints = new NonAffectedJoints[0];
				int num2 = 0;
				if (flag)
				{
					_lookAtController.segments[num2] = new BendingSegment();
					_lookAtController.segments[num2].firstTransform = child;
					_lookAtController.segments[num2].lastTransform = child2;
					_lookAtController.segments[num2].thresholdAngleDifference = 0f;
					_lookAtController.segments[num2].bendingMultiplier = 0.6f;
					_lookAtController.segments[num2].maxAngleDifference = 90f;
					_lookAtController.segments[num2].maxBendingAngle = 40f;
					_lookAtController.segments[num2].responsiveness = 2.5f;
					num2++;
				}
				if (flag2)
				{
					_lookAtController.segments[num2] = new BendingSegment();
					_lookAtController.segments[num2].firstTransform = child3;
					_lookAtController.segments[num2].lastTransform = child4;
					_lookAtController.segments[num2].thresholdAngleDifference = 30f;
					_lookAtController.segments[num2].bendingMultiplier = 0.6f;
					_lookAtController.segments[num2].maxAngleDifference = 30f;
					_lookAtController.segments[num2].maxBendingAngle = 40f;
					_lookAtController.segments[num2].responsiveness = 4f;
				}
				_lookAtController.Initialize();
			}
		}
		else if (_useTurnToPlayer)
		{
			Debug.LogWarning("NPC " + FullPath + " has no head or spine bones for look at controller.");
		}
		_head = child2;
		AsyncGetIcon("0", MilMo_ResourceManager.Priority.Low, null);
		IsReady = true;
		return true;
	}

	public override void FixedUpdate()
	{
		if (Paused)
		{
			return;
		}
		base.FixedUpdate();
		if (!(base.GameObject == null) && !(_player == null))
		{
			Vector3 npcToPlayer = _player.position - base.GameObject.transform.position;
			float magnitude = npcToPlayer.magnitude;
			if (_useTurnToPlayer)
			{
				TurnToPlayer(magnitude, npcToPlayer);
			}
			if (_inPlayerRange && magnitude > _interactionRadius)
			{
				_turnToPlayer = false;
				_inPlayerRange = false;
				MilMo_EventSystem.Instance.PostEvent("npc_leave_range", this);
			}
			else if (!_inPlayerRange && magnitude < _interactionRadius)
			{
				_inPlayerRange = true;
				MilMo_EventSystem.Instance.PostEvent("npc_enter_range", this);
			}
		}
	}

	private void TurnToPlayer(float npcToPlayerDistance, Vector3 npcToPlayer)
	{
		Vector3 eulerAngles = base.GameObject.transform.eulerAngles;
		bool flag = MilMo_Utility.IsClose(eulerAngles.y, _player.rotation.eulerAngles.y, 0.02f);
		bool flag2 = MilMo_Utility.IsClose(eulerAngles.y, SpawnRotation.y, 0.02f);
		if (_turnToPlayer && !flag)
		{
			Vector3 vector = Vector3.Normalize(_player.position - base.GameObject.transform.position);
			Quaternion b = Quaternion.LookRotation(vector);
			b.eulerAngles = new Vector3(0f, b.eulerAngles.y, 0f);
			float num = Vector3.Angle(vector, base.GameObject.transform.forward) * (MathF.PI / 180f) / (MathF.PI / 2f);
			base.GameObject.transform.rotation = Quaternion.Lerp(base.GameObject.transform.rotation, b, 1.2f * Time.deltaTime * num);
		}
		else if (!flag2)
		{
			base.GameObject.transform.rotation = Quaternion.Lerp(base.GameObject.transform.rotation, Quaternion.Euler(SpawnRotation), 1.2f * Time.deltaTime);
		}
		if (_lookAtController == null)
		{
			return;
		}
		if (npcToPlayerDistance < _lookAtRange)
		{
			if (Vector3.Angle(new Vector3(npcToPlayer.x, 0f, npcToPlayer.z), base.GameObject.transform.forward) * (MathF.PI / 180f) < MathF.PI / 2f)
			{
				if (!(_lookAtController == null) && !(_playerHead == null))
				{
					_lookAtController.rotationMultiplier = ((!MilMo_Utility.IsClose(_lookAtController.rotationMultiplier, 1f, 0.01f)) ? Mathf.Lerp(_lookAtController.rotationMultiplier, 1f, Time.deltaTime * 2f) : 1f);
					Vector3 position = _playerHead.position;
					_lookAtController.target = new Vector3(position.x, position.y, position.z);
					if (!_lookAtController.enabled)
					{
						_lookAtController.Reset();
						_lookAtController.rotationMultiplier = 1f;
						_lookAtController.enabled = true;
					}
				}
			}
			else if (_lookAtController.enabled)
			{
				if (MilMo_Utility.IsClose(_lookAtController.rotationMultiplier, 0f, 0.01f))
				{
					_lookAtController.target = Vector3.zero;
					_lookAtController.enabled = false;
				}
				else
				{
					_lookAtController.rotationMultiplier = Mathf.Lerp(_lookAtController.rotationMultiplier, 0f, Time.deltaTime * 2f);
				}
			}
		}
		else if (_lookAtController.enabled)
		{
			if (MilMo_Utility.IsClose(_lookAtController.rotationMultiplier, 0f, 0.01f))
			{
				_lookAtController.target = Vector3.zero;
				_lookAtController.enabled = false;
			}
			else
			{
				_lookAtController.rotationMultiplier = Mathf.Lerp(_lookAtController.rotationMultiplier, 0f, Time.deltaTime * 2f);
			}
		}
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
		if (_head != null)
		{
			_head.localScale = Vector3.one * HeadScale;
		}
	}

	public void ChangeInteractionState(byte newInteractionState)
	{
		_interactionState = newInteractionState;
		if ((bool)_interactionMarker)
		{
			_interactionMarker.UpdateState(_interactionState);
		}
	}

	public int GetPrio()
	{
		return 3;
	}

	public void UseReaction()
	{
		MilMo_Player instance = MilMo_Player.Instance;
		if (instance == null || instance.IsTalking)
		{
			return;
		}
		MilMo_Avatar avatar = instance.Avatar;
		if (avatar != null && !avatar.InCombat)
		{
			_turnToPlayer = true;
			MilMo_Player.Instance.OnStopTalkingWithNPC += PlayerStopInteract;
			Singleton<GameNetwork>.Instance.SendTalkToNpc(base.Id, avatar.Position);
			if (instance.IsNewPlayer())
			{
				MilMoAnalyticsHandler.NpcInteraction(TemplateIdentifier);
			}
		}
	}

	public Vector3 GetPosition()
	{
		return base.GameObject.transform.position;
	}

	public Vector3 GetMarkerOffset()
	{
		Renderer renderer = base.VisualRep?.Renderer;
		if (renderer == null)
		{
			return new Vector3(0f, 0.5f, 0f);
		}
		Bounds bounds = renderer.bounds;
		Vector3 result = base.GameObject.transform.InverseTransformPoint(bounds.center);
		result.y += bounds.extents.y;
		return result;
	}

	public string GetInteractionVerb()
	{
		return _interactionVerb.String;
	}

	public Interactable.InteractionType GetInteractionType()
	{
		return Interactable.InteractionType.Silver;
	}

	private void PlayerStopInteract()
	{
		_turnToPlayer = false;
	}

	public string GetPortraitKey()
	{
		string text = MilMo_Utility.ExtractNameFromPath(VisualRepName);
		return "Icon" + text;
	}
}
