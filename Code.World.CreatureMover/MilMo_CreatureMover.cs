using Code.Core.EventSystem;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.World.Level;
using Code.World.Level.LevelObject;
using UnityEngine;

namespace Code.World.CreatureMover;

public abstract class MilMo_CreatureMover : MilMo_MovableObjectMover
{
	private const float STUNNED_TIME = 0.2f;

	protected const float POSITIONAL_IMPULSE_FACTOR = 1f;

	protected const float ROTATIONAL_IMPULSE_FACTOR = 149f;

	protected const float SCALE_IMPULSE_FACTOR = 0.5f;

	protected const float REAL_IMPULSE_DECELERATION = 15f;

	private Transform _deathCollectNode;

	protected readonly MilMo_ObjectMover ImpactMover;

	protected readonly string ImpactType;

	protected bool HasImpulse;

	protected bool IsStunned;

	private bool _playAggroAnimOnUnStun;

	private float _lastStunTime;

	private Vector3 _deathImpulseFallTarget;

	private float _deathImpulseFallVelocity;

	private float _deathImpulseFadeOutSpeed;

	private float _deathImpulseFallStartTime;

	private static bool _impactMoverTweaker;

	private static float _impactMoverPullTweaker;

	private static float _impactMoverDragTweaker;

	private static float _impactMoverAnglePullTweaker;

	private static float _impactMoverAngleDragTweaker;

	private static float _impactMoverScalePullTweaker;

	private static float _impactMoverScaleDragTweaker;

	protected readonly MilMo_LevelCreatureTemplate Template;

	public bool HasDeathImpulse { get; private set; }

	public float DeathImpulseStartTime { get; private set; }

	public Transform DeathCollectNode
	{
		get
		{
			return _deathCollectNode;
		}
		set
		{
			_deathCollectNode = value;
			SetParent(_deathCollectNode);
		}
	}

	protected MilMo_CreatureMover(MilMo_LevelCreatureTemplate template)
	{
		Template = template;
		Velocity = template.Velocity;
		RotationSpeed = template.TurnSpeed;
		ImpactMover = new MilMo_ObjectMover();
		ImpactMover.SetUpdateFunc(2);
		ImpactType = template.ImpactMoverType;
		switch (ImpactType)
		{
		case "Rotational":
			ImpactMover.AnglePull = template.Pull;
			ImpactMover.AngleDrag = template.Drag;
			break;
		case "Positional":
			ImpactMover.Pull = template.Pull;
			ImpactMover.Drag = template.Drag;
			break;
		case "Scale":
			ImpactMover.ScalePull = template.Pull;
			ImpactMover.ScaleDrag = template.Drag;
			break;
		}
	}

	public void FixedUpdate()
	{
		if (ImpactMover != null)
		{
			if (_impactMoverTweaker)
			{
				ImpactMover.Drag = _impactMoverDragTweaker;
				ImpactMover.AnglePull = _impactMoverAnglePullTweaker;
				ImpactMover.AngleDrag = _impactMoverAngleDragTweaker;
				ImpactMover.ScalePull = _impactMoverScalePullTweaker;
				ImpactMover.ScaleDrag = _impactMoverScaleDragTweaker;
			}
			ImpactMover.Update();
		}
	}

	public abstract void AddLocalImpulse(float impact);

	public void Stunned()
	{
		IsStunned = true;
		PlayAnimation("Stun", 0.1f, WrapMode.Loop);
		_lastStunTime = Time.time;
		MilMo_EventSystem.At(0.2f, TryUnStun);
	}

	protected void HandleDeathCollectNodeInterpolation()
	{
		base.LocalPosition = Vector3.Lerp(base.LocalPosition, Vector3.zero, 10f * Time.deltaTime);
		base.LocalRotation = Quaternion.Lerp(base.LocalRotation, Quaternion.identity, 10f * Time.deltaTime);
	}

	private void TryUnStun()
	{
		float num = Time.time - _lastStunTime;
		float num2 = 0.2f - num;
		if ((double)num2 <= 0.01)
		{
			UnStun();
		}
		else
		{
			MilMo_EventSystem.At(num2, TryUnStun);
		}
	}

	private void UnStun()
	{
		if (!Dead)
		{
			IsStunned = false;
			if (_playAggroAnimOnUnStun)
			{
				Aggro();
			}
			else if (AnimationHandler != null)
			{
				PlayAnimation(AnimationHandler.CurrentMainAnimation, 0.3f, WrapMode.Loop);
			}
		}
	}

	public override void Aggro()
	{
		if (!MilMo_World.Instance.enabled)
		{
			return;
		}
		if (IsStunned)
		{
			_playAggroAnimOnUnStun = true;
			return;
		}
		PlayAnimation("Aggro", 0.1f, WrapMode.Once);
		AudioSourceWrapper audioSourceWrapper = ((Template.AggroSound != null && VisualRep != null && VisualRep.GameObject != null) ? VisualRep.GameObject.GetComponent<AudioSourceWrapper>() : null);
		if (audioSourceWrapper != null)
		{
			audioSourceWrapper.Clip = Template.AggroSound;
			audioSourceWrapper.Loop = false;
			audioSourceWrapper.Play();
		}
		_playAggroAnimOnUnStun = false;
	}

	public override void NoAggro()
	{
		_playAggroAnimOnUnStun = false;
	}

	public override void Kill()
	{
		Dead = true;
		if (!HasImpulse)
		{
			Target = base.Position;
		}
	}

	public override void ReadUpdate(CreatureUpdate creatureUpdate)
	{
		base.ReadUpdate(creatureUpdate);
		HasImpulse = false;
	}

	public override void StartMoving(Vector3 target, float speed, float timeSinceCreationMsg)
	{
		base.StartMoving(target, speed, timeSinceCreationMsg);
		HasImpulse = false;
	}

	public virtual void HandleRealImpulse(ServerMoveableImpulse msg)
	{
		if (msg != null)
		{
			vector3 position = msg.getPosition();
			vector3 target = msg.getTarget();
			float impulse = msg.getImpulse();
			sbyte deathImpulse = msg.getDeathImpulse();
			base.Position = new Vector3(position.GetX(), position.GetY(), position.GetZ());
			Velocity = impulse;
			Target = new Vector3(target.GetX(), target.GetY(), target.GetZ());
			HasImpulse = true;
			HasDeathImpulse = deathImpulse != 0;
			DeathImpulseStartTime = (HasDeathImpulse ? Time.time : 0f);
			LastServerUpdateTime = Time.time;
		}
	}

	public void SetupDeathImpulseFall()
	{
		_deathImpulseFallStartTime = Time.time;
		_deathImpulseFallTarget = base.Position;
		_deathImpulseFallTarget.y = MilMo_Level.GetWalkableHeight(_deathImpulseFallTarget);
		_deathImpulseFallVelocity = 0f;
		float magnitude = (_deathImpulseFallTarget - base.Position).magnitude;
		float num = Mathf.Sqrt(2f * magnitude / 7.64f);
		_deathImpulseFadeOutSpeed = 1f / num;
		Renderer renderer = VisualRep.Renderer;
		Material mat = new Material(renderer.material);
		Material material2 = (renderer.material = new Material(MilMo_ResourceManager.LoadShaderLocal("Shaders/Junebug/DiffuseTrans")));
		material2.CopyPropertiesFromMaterial(mat);
	}

	public bool UpdateDeathImpulseFall()
	{
		_deathImpulseFallVelocity += -7.64f * Time.deltaTime;
		Vector3 position = base.Position;
		position.y += Time.deltaTime * _deathImpulseFallVelocity;
		base.Position = position;
		Material material = VisualRep.Renderer.material;
		Color color = material.color;
		color.a -= Time.deltaTime * _deathImpulseFadeOutSpeed;
		material.color = color;
		if ((base.Position - _deathImpulseFallTarget).sqrMagnitude > 0.01f)
		{
			return Time.time - _deathImpulseFallStartTime < 3f;
		}
		return false;
	}

	protected override bool IsMainAnimation(string anim)
	{
		switch (anim)
		{
		default:
			return anim == "Swim";
		case "Idle":
		case "Walk":
		case "Fly":
			return true;
		}
	}
}
