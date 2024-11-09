using System.Collections.Generic;
using Code.Core.Emote;
using Code.Core.Utility;
using Code.World.Level;
using Code.World.Level.LevelObject;
using UnityEngine;

namespace Code.Core.Avatar;

public class MilMo_SuperAlivenessManager
{
	public enum LodLevel
	{
		Enabled,
		Disabled
	}

	private enum State
	{
		Event,
		Prio,
		Iterate,
		Idle
	}

	public enum Prio
	{
		AggroCritter = 60,
		PlayerSocial = 40,
		DangerousCritter = 20,
		Player = 18,
		Pickup = 16,
		HarmlessCritter = 14,
		ContainerCritter = 12,
		Npc = 10
	}

	public const float RANGE_SQUARED = 36f;

	private const float MIN_LOOK_AT_TIME = 1f;

	private const int CRITTER_UPDATES_PER_FRAME = 10;

	private const int PICKUP_UPDATES_PER_FRAME = 10;

	private const int PLAYER_UPDATES_PER_FRAME = 10;

	private const int NPC_UPDATES_PER_FRAME = 3;

	private const float EYE_LOOK_ANGLE = 30f;

	public const float HALF_FIELD_OF_VIEW = 100f;

	private MilMo_Avatar _avatar;

	private HeadLookController _headLookController;

	private MilMo_SuperAlivenessTarget _currentTarget;

	private State _state = State.Prio;

	private readonly Dictionary<object, MilMo_SuperAlivenessTarget> _targetsInRange = new Dictionary<object, MilMo_SuperAlivenessTarget>();

	private int _critterUpdateIndex;

	private int _pickupUpdateIndex;

	private int _playerUpdateIndex;

	private int _npcUpdateIndex;

	private const float EYE_RESPONSIVENESS = 0.5f;

	private const float MAX_EYE_U = 0.015f;

	private const float MIN_EYE_U = -0.015f;

	private const float MAX_EYE_V = 0.01f;

	private const float MIN_EYE_V = -0.0075f;

	private const float EYE_ANGLE_SCALE_FACTOR_U = 0.015f;

	private const float EYE_ANGLE_SCALE_FACTOR_V = 0.01f;

	private bool _useEyeLook = true;

	private LodLevel _lodLevel;

	private LodLevel _currentLodLevel;

	private bool _forcedDisable;

	public bool UseEyeLook
	{
		set
		{
			_useEyeLook = value;
		}
	}

	public MilMo_SuperAlivenessManager()
	{
		_lodLevel = LodLevel.Disabled;
		_currentLodLevel = LodLevel.Disabled;
	}

	public void ForceDisable()
	{
		_forcedDisable = true;
		Disable();
	}

	public void Disable()
	{
		if (_avatar != null && _avatar.EmoteManager != null)
		{
			_avatar.EmoteManager.SetEyeLook(Vector2.zero, Vector2.zero);
			_currentTarget = null;
			_currentLodLevel = LodLevel.Disabled;
		}
	}

	public void ForceEnable()
	{
		_forcedDisable = false;
		Enable();
	}

	public void Enable()
	{
		if (!_forcedDisable)
		{
			_currentLodLevel = _lodLevel;
		}
	}

	public void DisableHeadLookController()
	{
		if (!(_headLookController == null))
		{
			_headLookController.enabled = false;
		}
	}

	public void EnableHeadLookController()
	{
		if ((bool)_headLookController)
		{
			_headLookController.enabled = true;
		}
	}

	public void SetLodLevelEnabled()
	{
		_lodLevel = LodLevel.Enabled;
		Enable();
	}

	public void SetLodLevelDisabled()
	{
		_lodLevel = LodLevel.Disabled;
		Disable();
	}

	public void CritterAggro(MilMo_MovableObject critter)
	{
		if (_targetsInRange.ContainsKey(critter))
		{
			_targetsInRange[critter].ChangePrioClass(Prio.AggroCritter, resetTime: true);
		}
	}

	public void CritterNoAggro(MilMo_MovableObject critter)
	{
		if (_targetsInRange.ContainsKey(critter))
		{
			_targetsInRange[critter].ChangePrioClass(Prio.DangerousCritter, resetTime: false);
		}
	}

	public void PlayerSocialized(MilMo_Avatar player)
	{
		if (_targetsInRange.ContainsKey(player))
		{
			_targetsInRange[player].ChangePrioClass(Prio.PlayerSocial, resetTime: true);
		}
	}

	public void PlayerStopSocialize(MilMo_Avatar player)
	{
		if (_targetsInRange.ContainsKey(player))
		{
			_targetsInRange[player].ChangePrioClass(Prio.Player, resetTime: false);
		}
	}

	public void Init(MilMo_Avatar avatar, LodLevel lodLevel)
	{
		_avatar = avatar;
		_lodLevel = lodLevel;
		_currentLodLevel = lodLevel;
		bool flag = _avatar.HeadStart != null && _avatar.HeadEnd != null;
		bool flag2 = _avatar.SpineStart != null && _avatar.SpineEnd != null;
		if (flag || flag2)
		{
			_headLookController = _avatar.GameObject.AddComponent<HeadLookController>();
			if (_headLookController != null)
			{
				_headLookController.enabled = false;
				_headLookController.segments = new BendingSegment[(!(flag && flag2)) ? 1 : 2];
				_headLookController.rootNode = _avatar.GameObject.transform;
				_headLookController.nonAffectedJoints = new NonAffectedJoints[((_avatar.LeftShoulder != null) ? 1 : 0) + ((_avatar.RightShoulder != null) ? 1 : 0)];
				int num = 0;
				if (_avatar.RightShoulder != null)
				{
					_headLookController.nonAffectedJoints[num] = new NonAffectedJoints
					{
						joint = _avatar.RightShoulder,
						effect = 0f,
						child = MilMo_Utility.GetChildBone(_avatar.Renderer, _avatar.RightShoulder)
					};
					num++;
				}
				if (_avatar.LeftShoulder != null)
				{
					_headLookController.nonAffectedJoints[num] = new NonAffectedJoints
					{
						joint = _avatar.LeftShoulder,
						effect = 0f,
						child = MilMo_Utility.GetChildBone(_avatar.Renderer, _avatar.LeftShoulder)
					};
				}
				num = 0;
				if (flag)
				{
					_headLookController.segments[num] = new BendingSegment
					{
						firstTransform = _avatar.HeadStart,
						lastTransform = _avatar.HeadEnd,
						thresholdAngleDifference = 30f,
						bendingMultiplier = 0.6f,
						maxAngleDifference = 30f,
						maxBendingAngle = 35f,
						responsiveness = 2.5f
					};
					num++;
				}
				if (flag2)
				{
					_headLookController.segments[num] = new BendingSegment
					{
						firstTransform = _avatar.SpineStart,
						lastTransform = _avatar.SpineEnd,
						thresholdAngleDifference = 65f,
						bendingMultiplier = 0.6f,
						maxAngleDifference = 65f,
						maxBendingAngle = 35f,
						responsiveness = 4f
					};
				}
				_headLookController.Initialize();
			}
		}
		_state = State.Prio;
	}

	public void FixedUpdate()
	{
		if (!_headLookController)
		{
			return;
		}
		if (_currentTarget != null && !_currentTarget.Transform)
		{
			if (_targetsInRange.ContainsKey(_currentTarget))
			{
				_targetsInRange.Remove(_currentTarget);
			}
			_currentTarget = null;
		}
		if (_currentTarget != null)
		{
			_headLookController.rotationMultiplier = ((!MilMo_Utility.IsClose(_headLookController.rotationMultiplier, 1f, 0.01f)) ? Mathf.Lerp(_headLookController.rotationMultiplier, 1f, Time.deltaTime * 2f) : 1f);
			_headLookController.target = _currentTarget.Position;
			if (!_headLookController.enabled)
			{
				_headLookController.Reset();
				_headLookController.rotationMultiplier = 1f;
				_headLookController.enabled = true;
			}
		}
		else if (_headLookController.enabled)
		{
			if (MilMo_Utility.IsClose(_headLookController.rotationMultiplier, 0f, 0.01f))
			{
				_headLookController.target = Vector3.zero;
				_headLookController.enabled = false;
			}
			else
			{
				_headLookController.rotationMultiplier = Mathf.Lerp(_headLookController.rotationMultiplier, 0f, Time.deltaTime * 2f);
			}
		}
		FixedUpdateEyes();
	}

	public void Update()
	{
		if (_currentLodLevel != LodLevel.Disabled)
		{
			switch (_state)
			{
			case State.Event:
				UpdateStateEvent();
				break;
			case State.Prio:
				UpdateStatePrio();
				break;
			case State.Iterate:
				UpdateStateIterate();
				break;
			case State.Idle:
				UpdateStateIdle();
				break;
			}
		}
	}

	private void FixedUpdateEyes()
	{
		if (_currentLodLevel == LodLevel.Disabled || !_useEyeLook || !_avatar.HeadTip || _avatar.EmoteManager == null)
		{
			return;
		}
		if (_currentTarget == null)
		{
			_avatar.EmoteManager.SetEyeLook(Vector2.zero, Vector2.zero);
			return;
		}
		Vector3 vector = _currentTarget.Position - _avatar.HeadTip.position;
		if (!(vector == Vector3.zero))
		{
			vector.Normalize();
			Quaternion quaternion = Quaternion.FromToRotation(_avatar.HeadTip.forward, vector);
			float value = ((quaternion.eulerAngles.y <= 180f) ? quaternion.eulerAngles.y : (quaternion.eulerAngles.y - 360f)) * 0.5f;
			vector = _avatar.HeadTip.TransformDirection(vector);
			vector.Normalize();
			float value2 = Mathf.Asin(vector.y) * 57.29578f * 0.5f;
			float num = Mathf.Clamp(value, -30f, 30f);
			value2 = Mathf.Clamp(value2, -30f, 30f);
			float value3 = num / 30f * 0.015f;
			float num2 = value2 / 30f * 0.01f;
			value3 = Mathf.Clamp(value3, -0.015f, 0.015f);
			num2 = Mathf.Clamp(0f - num2, -0.0075f, 0.01f);
			Vector2 uvOffsetRight = new Vector2(0f - value3, num2);
			Vector2 uvOffsetLeft = new Vector2(value3, num2);
			_avatar.EmoteManager.SetEyeLook(uvOffsetRight, uvOffsetLeft);
		}
	}

	private void UpdateStateEvent()
	{
	}

	private void UpdateStatePrio()
	{
		UpdateTargetsInRange();
		MilMo_SuperAlivenessTarget milMo_SuperAlivenessTarget = UpdatePrios();
		if (milMo_SuperAlivenessTarget != _currentTarget && (_currentTarget == null || Time.time - _currentTarget.StartTime > 1f))
		{
			_avatar.PlayEmoteLocal(MilMo_EmoteSystem.GetEmoteByName("Emotes.RaiseEyebrows"), isBlink: true);
			_currentTarget = milMo_SuperAlivenessTarget;
		}
	}

	private void UpdateStateIterate()
	{
		if (UpdateTargetsInRange())
		{
			_state = State.Prio;
		}
	}

	private void UpdateStateIdle()
	{
		if (UpdateTargetsInRange())
		{
			_state = State.Prio;
		}
	}

	private MilMo_SuperAlivenessTarget UpdatePrios()
	{
		MilMo_SuperAlivenessTarget milMo_SuperAlivenessTarget = null;
		List<object> list = new List<object>();
		foreach (KeyValuePair<object, MilMo_SuperAlivenessTarget> item in _targetsInRange)
		{
			item.Value.UpdateTotalPrio(_avatar.GameObject.transform, out var inRange);
			if (!inRange)
			{
				list.Add(item.Key);
			}
			else if (milMo_SuperAlivenessTarget == null || item.Value.TotalPrio > milMo_SuperAlivenessTarget.TotalPrio)
			{
				milMo_SuperAlivenessTarget = item.Value;
			}
		}
		foreach (object item2 in list)
		{
			if (_currentTarget == item2)
			{
				_currentTarget = null;
			}
			_targetsInRange.Remove(item2);
		}
		return milMo_SuperAlivenessTarget;
	}

	private bool UpdateTargetsInRange()
	{
		if (MilMo_Instance.CurrentInstance == null)
		{
			return false;
		}
		bool result = false;
		List<MilMo_MovableObject> creatures = MilMo_Instance.CurrentInstance.Creatures;
		List<MilMo_Avatar> avatars = MilMo_Instance.CurrentInstance.Avatars;
		List<MilMo_LevelItem> items = MilMo_Instance.CurrentInstance.Items;
		List<MilMo_LevelNpc> nPCs = MilMo_Instance.CurrentInstance.NPCs;
		int num = 0;
		int num2 = num;
		if (_currentLodLevel == LodLevel.Enabled)
		{
			if (creatures != null && creatures.Count > 0)
			{
				num = _critterUpdateIndex % creatures.Count;
				for (int i = 0; i < 10; i++)
				{
					num2 = (num + i) % creatures.Count;
					if (num2 == num && i > 0)
					{
						break;
					}
					float sqrMagnitude = (creatures[num2].Position - _avatar.Position).sqrMagnitude;
					if (sqrMagnitude <= 36f && !_targetsInRange.ContainsKey(creatures[num2]))
					{
						Prio prio = (creatures[num2].IsContainer() ? Prio.ContainerCritter : ((!creatures[num2].IsDangerous()) ? Prio.HarmlessCritter : Prio.DangerousCritter));
						_targetsInRange.Add(creatures[num2], new MilMo_SuperAlivenessTarget(creatures[num2].GameObject.transform, prio, sqrMagnitude, useBoundingBoxForHeight: true));
						result = true;
					}
				}
				_critterUpdateIndex = (num2 + 1) % creatures.Count;
			}
			if (items != null && items.Count > 0)
			{
				num = _pickupUpdateIndex % items.Count;
				for (int j = 0; j < 10; j++)
				{
					num2 = (num + j) % items.Count;
					if (num2 == num && j > 0)
					{
						break;
					}
					float sqrMagnitude2 = (items[num2].Position - _avatar.Position).sqrMagnitude;
					if (sqrMagnitude2 <= 36f && !_targetsInRange.ContainsKey(items[num2]))
					{
						_targetsInRange.Add(items[num2], new MilMo_SuperAlivenessTarget(items[num2].GameObject.transform, Prio.Pickup, sqrMagnitude2, useBoundingBoxForHeight: true));
						result = true;
					}
				}
				_pickupUpdateIndex = (num2 + 1) % items.Count;
			}
			if (nPCs != null && nPCs.Count > 0)
			{
				num = _npcUpdateIndex % nPCs.Count;
				for (int k = 0; k < 3; k++)
				{
					num2 = (num + k) % nPCs.Count;
					if (num2 == num && k > 0)
					{
						break;
					}
					float sqrMagnitude3 = ((nPCs[num2].Head ? nPCs[num2].Head.position : nPCs[num2].Position) - _avatar.Position).sqrMagnitude;
					if (sqrMagnitude3 <= 36f && !_targetsInRange.ContainsKey(nPCs[num2]))
					{
						_targetsInRange.Add(nPCs[num2], new MilMo_SuperAlivenessTarget(nPCs[num2].Head ? nPCs[num2].Head : nPCs[num2].GameObject.transform, Prio.Npc, sqrMagnitude3, useBoundingBoxForHeight: false));
						result = true;
					}
				}
				_npcUpdateIndex = (num2 + 1) % nPCs.Count;
			}
		}
		if (avatars == null || avatars.Count <= 0)
		{
			return result;
		}
		num = _playerUpdateIndex % avatars.Count;
		for (int l = 0; l < 10; l++)
		{
			num2 = (num + l) % avatars.Count;
			if (num2 == num && l > 0)
			{
				break;
			}
			if (avatars[num2].Enabled)
			{
				float sqrMagnitude4 = (avatars[num2].Position - _avatar.Position).sqrMagnitude;
				if (!(avatars[num2].Id == _avatar.Id) && sqrMagnitude4 <= 36f && !_targetsInRange.ContainsKey(avatars[num2]))
				{
					_targetsInRange.Add(avatars[num2], new MilMo_SuperAlivenessTarget(avatars[num2].HeadTip ? avatars[num2].HeadTip : avatars[num2].GameObject.transform, Prio.Player, sqrMagnitude4, useBoundingBoxForHeight: false));
					result = true;
				}
			}
		}
		_playerUpdateIndex = (num2 + 1) % avatars.Count;
		return result;
	}
}
