using System;
using UnityEngine;

namespace Core.State.Basic;

[CreateAssetMenu(fileName = "newFloatState", menuName = "State/Float State")]
public class FloatState : ScriptableObject, ISerializationCallbackReceiver
{
	[SerializeField]
	private float initialValue;

	[NonSerialized]
	private float _runtimeValue;

	public event Action<float> OnChange;

	public void OnAfterDeserialize()
	{
		_runtimeValue = initialValue;
	}

	public void OnBeforeSerialize()
	{
	}

	public void Set(float newValue)
	{
		_runtimeValue = newValue;
		this.OnChange?.Invoke(_runtimeValue);
	}

	public float Get()
	{
		return _runtimeValue;
	}

	private void OnValidate()
	{
		this.OnChange?.Invoke(_runtimeValue);
	}
}
