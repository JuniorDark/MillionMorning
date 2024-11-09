using Code.Core.EventSystem;
using Code.Core.Network.types;
using Code.Core.Utility;
using Code.Core.Visual;
using UnityEngine;

namespace Code.World.CreatureMover;

public abstract class MilMo_MovableObjectMover
{
	private Transform _transform;

	protected Vector3 OriginalScale = Vector3.one;

	protected Vector3 Target;

	protected Quaternion TargetRotation;

	protected float Velocity;

	protected float LastServerUpdateTime;

	protected bool Dead;

	protected float RotationSpeed = 0.5f;

	protected MilMo_VisualRep VisualRep;

	protected MilMo_MovableAnimationHandler AnimationHandler;

	private MilMo_GenericReaction _activateWorldReaction;

	protected Vector3 Position
	{
		get
		{
			if (!_transform)
			{
				return Vector3.zero;
			}
			return _transform.position;
		}
		set
		{
			if ((bool)_transform && !float.IsNaN(value.x) && !float.IsNaN(value.y) && !float.IsNaN(value.z))
			{
				_transform.position = value;
			}
		}
	}

	protected Vector3 LocalPosition
	{
		get
		{
			if (!_transform)
			{
				return Vector3.zero;
			}
			return _transform.localPosition;
		}
		set
		{
			if ((bool)_transform && !float.IsNaN(value.x) && !float.IsNaN(value.y) && !float.IsNaN(value.z))
			{
				_transform.localPosition = value;
			}
		}
	}

	protected Quaternion Rotation
	{
		get
		{
			if (!_transform)
			{
				return Quaternion.identity;
			}
			return _transform.rotation;
		}
		set
		{
			if ((bool)_transform)
			{
				_transform.rotation = value;
			}
		}
	}

	protected Quaternion LocalRotation
	{
		get
		{
			if (!_transform)
			{
				return Quaternion.identity;
			}
			return _transform.localRotation;
		}
		set
		{
			if ((bool)_transform)
			{
				_transform.localRotation = value;
			}
		}
	}

	protected Vector3 LocalScale
	{
		set
		{
			if ((bool)_transform)
			{
				_transform.localScale = value;
			}
		}
	}

	public float DistanceToTarget => (Position - Target).magnitude;

	protected bool IsReady { get; private set; }

	public Vector3 SetRealPosition
	{
		set
		{
			Position = value;
		}
	}

	protected void SetParent(Transform parentTransform)
	{
		_transform.parent = parentTransform;
	}

	public void Initialize(MilMo_VisualRep visualRep)
	{
		if (visualRep != null && !(visualRep.GameObject == null))
		{
			AnimationHandler = visualRep.GameObject.GetComponent<MilMo_MovableAnimationHandler>();
			if (AnimationHandler != null)
			{
				AnimationHandler.IsMainAnimationCallback = IsMainAnimation;
			}
			_transform = visualRep.GameObject.transform;
			if ((bool)_transform)
			{
				OriginalScale = _transform.localScale;
			}
			SetTargetRotation();
			if (visualRep.GameObject.GetComponent<Animation>() == null)
			{
				visualRep.GameObject.AddComponent<Animation>();
			}
			VisualRep = visualRep;
			IsReady = true;
		}
	}

	public abstract void Update();

	protected abstract void Update(float time);

	protected abstract void SetTargetRotation();

	public abstract void TurnTo(Vector3 position);

	public abstract void Aggro();

	public abstract void NoAggro();

	public abstract void Kill();

	protected abstract bool IsMainAnimation(string anim);

	public virtual void ReadUpdate(CreatureUpdate creatureUpdate)
	{
		if (!_transform || creatureUpdate == null)
		{
			return;
		}
		float speed = creatureUpdate.GetSpeed();
		vector3 position = creatureUpdate.GetPosition();
		vector3 target = creatureUpdate.GetTarget();
		Position = new Vector3(position.GetX(), position.GetY(), position.GetZ());
		Target = new Vector3(target.GetX(), target.GetY(), target.GetZ());
		Velocity = 0f;
		LastServerUpdateTime = Time.time;
		Vector3 vector = new Vector3(Position.x, 0f, Position.z);
		Vector3 vector2 = new Vector3(Target.x, 0f, Target.z);
		float magnitude = (vector2 - vector).magnitude;
		if (!MilMo_Utility.Equals(vector, vector2))
		{
			float num = 1000f;
			if (!MilMo_Utility.IsClose(speed, 0f, 0.0001f) && magnitude != 0f)
			{
				num = magnitude / speed;
			}
			Velocity = magnitude / num;
			SetTargetRotation();
		}
	}

	public virtual void StartMoving(Vector3 target, float speed, float timeSinceCreationMsg)
	{
		if (_transform == null)
		{
			return;
		}
		Target = target;
		Vector3 vector = new Vector3(Position.x, 0f, Position.z);
		Vector3 vector2 = new Vector3(Target.x, 0f, Target.z);
		float magnitude = (vector2 - vector).magnitude;
		if (speed == 0f)
		{
			Target = Position;
			Velocity = 0f;
		}
		else
		{
			float num = magnitude / speed - timeSinceCreationMsg;
			if (num <= 0f)
			{
				Position = Target;
			}
			else
			{
				Velocity = speed;
				Update(timeSinceCreationMsg);
				float magnitude2 = (vector2 - vector).magnitude;
				Velocity = magnitude2 / num;
			}
		}
		SetTargetRotation();
		_activateWorldReaction = MilMo_EventSystem.Listen("world_activated", Unpause);
		_activateWorldReaction.Repeating = true;
	}

	public void PlayAnimation(string anim, float crossFade, WrapMode wrapMode, bool startAtRandomTime = false)
	{
		if (AnimationHandler != null)
		{
			AnimationHandler.PlayAnimation(anim, crossFade, wrapMode, startAtRandomTime);
		}
	}

	public void Destroy()
	{
		MilMo_EventSystem.RemoveReaction(_activateWorldReaction);
	}

	private void Unpause(object o)
	{
		float time = Mathf.Min((float)o, Time.time - LastServerUpdateTime);
		Update(time);
	}
}
