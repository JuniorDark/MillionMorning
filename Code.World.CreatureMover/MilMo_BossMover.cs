using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.ObjectEffectSystem;
using Code.Core.Utility;
using Code.World.Level;
using Code.World.Level.LevelObject;
using UnityEngine;

namespace Code.World.CreatureMover;

public class MilMo_BossMover : MilMo_MovableObjectMover
{
	private float _realRotationY;

	private float _targetRotationY;

	private MilMo_BossModeTemplate _currentMode;

	private readonly List<MilMo_ObjectEffect> _effects = new List<MilMo_ObjectEffect>();

	private Transform _turnToTarget;

	public MilMo_LevelBossTemplate Template;

	public MilMo_BossModeTemplate CurrentMode
	{
		get
		{
			return _currentMode;
		}
		set
		{
			_currentMode = value;
			if (_currentMode == null)
			{
				return;
			}
			RotationSpeed = _currentMode.TurnSpeed;
			PlayAnimation(_currentMode.LoopingAnimation, 0.3f, WrapMode.Loop);
			if (!MilMo_World.Instance.enabled || VisualRep == null || VisualRep.GameObject == null)
			{
				return;
			}
			foreach (string enterEffect in _currentMode.EnterEffects)
			{
				MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(VisualRep.GameObject, enterEffect);
				if (objectEffect != null)
				{
					_effects.Add(objectEffect);
				}
			}
		}
	}

	public Transform TurnToTarget
	{
		get
		{
			return _turnToTarget;
		}
		set
		{
			_turnToTarget = value;
			if (_turnToTarget != null)
			{
				TurnTo(_turnToTarget.position);
			}
		}
	}

	public override void Update()
	{
		Update(Time.deltaTime);
	}

	protected override void Update(float time)
	{
		if (!base.IsReady)
		{
			return;
		}
		for (int num = _effects.Count - 1; num >= 0; num--)
		{
			if (!_effects[num].Update())
			{
				_effects.RemoveAt(num);
			}
		}
		if (MilMo_Utility.Equals(base.Position, Target) && MilMo_Utility.IsClose(_realRotationY, _targetRotationY, 0.1f))
		{
			return;
		}
		float num2 = Velocity * time;
		Vector3 vector = new Vector3(Target.x, 0f, Target.z) - new Vector3(base.Position.x, 0f, base.Position.z);
		if (vector.sqrMagnitude <= num2 * num2)
		{
			if (!MilMo_Utility.Equals(base.Position, Target))
			{
				base.Position = Target;
			}
		}
		else
		{
			vector.Normalize();
			base.Position += vector * num2;
		}
		Vector3 normal = Vector3.zero;
		MilMo_LevelBossTemplate template = Template;
		if (template != null && !template.IsImmobile)
		{
			Vector3 position = base.Position;
			float walkableHeight = MilMo_Level.GetWalkableHeight(position, out normal);
			base.Position = new Vector3(position.x, walkableHeight, position.z);
		}
		if (!Dead)
		{
			if (_turnToTarget != null)
			{
				TurnTo(_turnToTarget.position);
			}
			if (MilMo_Utility.IsClose(_realRotationY, _targetRotationY, 0.1f))
			{
				_realRotationY = _targetRotationY;
			}
			else
			{
				_realRotationY = Mathf.LerpAngle(_realRotationY, _targetRotationY, time * RotationSpeed);
			}
			base.Rotation = Quaternion.Euler(0f, _realRotationY, 0f);
			if (!MilMo_Utility.Equals(normal, Vector3.zero))
			{
				base.Rotation = Quaternion.FromToRotation(Vector3.up, normal) * base.Rotation;
			}
		}
	}

	protected override void SetTargetRotation()
	{
		TurnTo(Target);
		_turnToTarget = null;
	}

	public override void TurnTo(Vector3 target)
	{
		Vector3 vector = new Vector3(target.x, 0f, target.z);
		Vector3 vector2 = new Vector3(base.Position.x, 0f, base.Position.z);
		float targetRotationY = ((!MilMo_Utility.Equals(vector, vector2)) ? Quaternion.LookRotation(vector - vector2).eulerAngles.y : base.Rotation.eulerAngles.y);
		_targetRotationY = targetRotationY;
	}

	public override void Aggro()
	{
		if (_currentMode == null || VisualRep == null || VisualRep.GameObject == null || !MilMo_World.Instance.enabled)
		{
			return;
		}
		foreach (string aggroEffect in _currentMode.AggroEffects)
		{
			MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(VisualRep.GameObject, aggroEffect);
			if (objectEffect != null)
			{
				_effects.Add(objectEffect);
			}
		}
	}

	public override void ReadUpdate(CreatureUpdate creatureUpdate)
	{
		base.ReadUpdate(creatureUpdate);
		if (_currentMode != null)
		{
			PlayAnimation(_currentMode.LoopingAnimation, 0.3f, WrapMode.Loop);
		}
	}

	public override void StartMoving(Vector3 target, float speed, float timeSinceCreationMsg)
	{
		base.StartMoving(target, speed, timeSinceCreationMsg);
		if (_currentMode != null)
		{
			PlayAnimation(_currentMode.LoopingAnimation, 0.3f, WrapMode.Loop);
		}
	}

	public override void NoAggro()
	{
	}

	public override void Kill()
	{
		Dead = true;
		Target = base.Position;
	}

	protected override bool IsMainAnimation(string anim)
	{
		if (_currentMode == null)
		{
			return false;
		}
		return anim == _currentMode.LoopingAnimation;
	}
}
