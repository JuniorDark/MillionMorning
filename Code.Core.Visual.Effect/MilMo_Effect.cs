using System.Collections.Generic;
using UnityEngine;

namespace Code.Core.Visual.Effect;

public class MilMo_Effect
{
	private float _time;

	private float _lastTime = -1f;

	private readonly GameObject _parent;

	private readonly Vector3 _position = Vector3.zero;

	private readonly List<MilMo_SubEffect> _subEffects = new List<MilMo_SubEffect>();

	private readonly MilMo_EffectTemplate _template;

	private bool _yLocked;

	private float _yPos;

	private Vector3 _offset = new Vector3(0f, 0f, 0f);

	private int _nextAction;

	public string Name => _template.Name;

	public float Duration => _template.Duration;

	public MilMo_Effect(MilMo_EffectTemplate template, Vector3 position)
	{
		_template = template;
		_position = position;
	}

	public MilMo_Effect(MilMo_EffectTemplate template, GameObject parent)
	{
		_template = template;
		_parent = parent;
	}

	public void SetOffset(Vector3 offset)
	{
		_offset = offset;
	}

	public bool TriggerNextAction()
	{
		if (_template.Actions.Count <= _nextAction)
		{
			_nextAction = 0;
			return false;
		}
		AddSubEffect(_template.Actions[_nextAction], _offset);
		_nextAction++;
		return true;
	}

	public GameObject GetCurrentGameObject()
	{
		if (_subEffects.Count != 0)
		{
			return _subEffects[_subEffects.Count - 1].EmittingObject;
		}
		return null;
	}

	public bool Update()
	{
		_time += Time.deltaTime;
		bool flag = false;
		foreach (MilMo_EffectAction action in _template.Actions)
		{
			if (_time >= action.StartTime && _lastTime < action.StartTime)
			{
				AddSubEffect(action, _offset);
			}
			else if (_time < action.StartTime)
			{
				flag = true;
			}
		}
		for (int num = _subEffects.Count - 1; num >= 0; num--)
		{
			if (_subEffects[num] == null || !_subEffects[num].Update())
			{
				_subEffects.RemoveAt(num);
			}
		}
		_lastTime = _time;
		return _subEffects.Count != 0 || flag;
	}

	public void Update(float time)
	{
		_time = time;
		foreach (MilMo_EffectAction action in _template.Actions)
		{
			if (_time >= action.StartTime && (_lastTime < action.StartTime || _time < _lastTime))
			{
				AddSubEffect(action, _offset);
			}
		}
		for (int num = _subEffects.Count - 1; num >= 0; num--)
		{
			if (_subEffects[num] == null || !_subEffects[num].Update())
			{
				_subEffects.RemoveAt(num);
			}
		}
		_lastTime = _time;
	}

	public void Restart()
	{
		_time = 0f;
		_lastTime = -1f;
		_yLocked = false;
	}

	public void Restart(float staticYHeight)
	{
		_time = 0f;
		_lastTime = -1f;
		LockY(staticYHeight);
	}

	public void Destroy()
	{
		foreach (MilMo_SubEffect subEffect in _subEffects)
		{
			subEffect.Destroy();
		}
		_subEffects.Clear();
		_nextAction = 0;
	}

	public void DestroyWhenDone()
	{
		foreach (MilMo_SubEffect subEffect in _subEffects)
		{
			subEffect.DestroyWhenDone();
		}
		_subEffects.Clear();
		_nextAction = 0;
	}

	public void Stop()
	{
		foreach (MilMo_SubEffect subEffect in _subEffects)
		{
			subEffect.Stop();
		}
	}

	private void LockY(float height)
	{
		_yLocked = true;
		_yPos = height;
	}

	private void AddSubEffect(MilMo_EffectAction action, Vector3 offset)
	{
		MilMo_SubEffect milMo_SubEffect = ((!_parent) ? action.CreateSubEffect(_position + offset) : (_yLocked ? action.CreateSubEffect(_parent, _yPos) : action.CreateSubEffect(_parent, offset)));
		if (milMo_SubEffect != null && (bool)milMo_SubEffect.EmittingObject)
		{
			_subEffects.Add(milMo_SubEffect);
		}
	}
}
