using System.Collections.Generic;
using Code.Core.Utility;
using Code.World.Level;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_MoverEffect : MilMo_ObjectEffect
{
	private Vector3 _target;

	private float _timeAtTarget;

	private readonly MilMo_ObjectMover _mover;

	private const int MIN_BOUNCES = 5;

	private bool _inBounce;

	private int _bounces;

	private readonly List<MilMo_ObjectEffect> _bounceEffects = new List<MilMo_ObjectEffect>();

	private MilMo_MoverEffectTemplate Template => EffectTemplate as MilMo_MoverEffectTemplate;

	public override float Duration => 0f;

	public MilMo_MoverEffect(GameObject gameObject, MilMo_MoverEffectTemplate template)
		: base(gameObject, template)
	{
		_mover = new MilMo_ObjectMover();
		_mover.SetUpdateFunc(4);
		_mover.Pull = template.Pull;
		_mover.Drag = template.Drag;
		_mover.Impulse(0f, template.StartVelocityY, 0f);
		SetTargetPosition(GameObject.transform.position);
	}

	public void SetTargetPosition(Vector3 newTarget)
	{
		_target = newTarget;
		_mover.Pos.y = (_target.y = GetHeight(_target));
	}

	public override void FixedUpdate()
	{
		_mover?.Update();
		foreach (MilMo_ObjectEffect bounceEffect in _bounceEffects)
		{
			bounceEffect.FixedUpdate();
		}
	}

	public override bool Update()
	{
		if (GameObject == null)
		{
			Destroy();
			return false;
		}
		Vector3 position = GameObject.transform.position;
		Vector3 vector = position;
		float num = Template.XzSpeed * Time.deltaTime;
		Vector3 vector2 = new Vector3(_target.x, 0f, _target.z) - new Vector3(position.x, 0f, position.z);
		if (vector2.magnitude <= num)
		{
			vector = _target;
		}
		else
		{
			vector2.Normalize();
			vector += vector2 * num;
		}
		vector.y = GetHeight(vector) + Mathf.Max(_mover.Pos.y, 0f);
		position = (GameObject.transform.position = vector);
		bool flag = (double)(position - _target).magnitude < 0.01;
		bool flag2 = Mathf.Abs(_mover.Vel.y) > 0.05f;
		if (!flag2)
		{
			_timeAtTarget += Time.deltaTime;
		}
		if (_mover.Pos.y <= 0f)
		{
			if (!_inBounce)
			{
				_inBounce = true;
				_bounces++;
				if (flag2 && !string.IsNullOrEmpty(Template.BounceEffect))
				{
					MilMo_ObjectEffect objectEffect = MilMo_ObjectEffectSystem.GetObjectEffect(GameObject, Template.BounceEffect);
					if (objectEffect != null)
					{
						_bounceEffects.Add(objectEffect);
					}
				}
			}
		}
		else if (_inBounce)
		{
			_inBounce = false;
		}
		for (int num2 = _bounceEffects.Count - 1; num2 >= 0; num2--)
		{
			if (!_bounceEffects[num2].Update())
			{
				_bounceEffects.RemoveAt(num2);
			}
		}
		if (flag && (_bounces >= 5 || _mover.Paused || _timeAtTarget > 1.5f))
		{
			GameObject.transform.position = _target;
			Destroy();
			return false;
		}
		return true;
	}

	public override void Destroy()
	{
		base.Destroy();
		foreach (MilMo_ObjectEffect bounceEffect in _bounceEffects)
		{
			bounceEffect.Destroy();
		}
	}

	private float GetHeight(Vector3 position)
	{
		if (!Template.DontUseGroundHeight)
		{
			return MilMo_Level.GetWalkableHeight(position);
		}
		return position.y;
	}
}
