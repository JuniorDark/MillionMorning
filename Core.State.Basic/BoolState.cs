using System;
using UnityEngine;

namespace Core.State.Basic;

[CreateAssetMenu(fileName = "newBoolState", menuName = "State/Bool State")]
public class BoolState : ScriptableObject, ISerializationCallbackReceiver
{
	[SerializeField]
	private bool initialValue;

	[NonSerialized]
	private bool _runtimeValue;

	public event Action<bool> OnChange;

	public void OnAfterDeserialize()
	{
		_runtimeValue = initialValue;
	}

	public void OnBeforeSerialize()
	{
	}

	public void Set(bool newValue)
	{
		_runtimeValue = newValue;
		this.OnChange?.Invoke(_runtimeValue);
	}

	public bool Get()
	{
		return _runtimeValue;
	}

	private void OnValidate()
	{
		this.OnChange?.Invoke(_runtimeValue);
	}
}
