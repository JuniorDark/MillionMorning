using System;
using UnityEngine;

namespace Core.State.Basic;

[CreateAssetMenu(fileName = "newIntState", menuName = "State/Int State")]
public class IntState : ScriptableObject, ISerializationCallbackReceiver
{
	[SerializeField]
	private int initialValue;

	[NonSerialized]
	private int _runtimeValue;

	public event Action<int> OnChange;

	public void OnAfterDeserialize()
	{
		_runtimeValue = initialValue;
	}

	public void OnBeforeSerialize()
	{
	}

	public void Set(int newValue)
	{
		_runtimeValue = newValue;
		this.OnChange?.Invoke(_runtimeValue);
	}

	public void SetSilently(int newValue)
	{
		_runtimeValue = newValue;
	}

	public int Get()
	{
		return _runtimeValue;
	}

	private void OnValidate()
	{
		this.OnChange?.Invoke(_runtimeValue);
	}
}
