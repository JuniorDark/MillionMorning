using System;
using UnityEngine;

namespace Core.State.Basic;

[CreateAssetMenu(fileName = "newStringState", menuName = "State/String State")]
public class StringState : ScriptableObject, ISerializationCallbackReceiver
{
	[SerializeField]
	private string initialValue;

	[NonSerialized]
	private string _runtimeValue;

	public event Action<string> OnChange;

	public void OnAfterDeserialize()
	{
		_runtimeValue = initialValue;
	}

	public void OnBeforeSerialize()
	{
	}

	public void Set(string newValue)
	{
		_runtimeValue = newValue;
		this.OnChange?.Invoke(_runtimeValue);
	}

	public string Get()
	{
		return _runtimeValue;
	}

	private void OnValidate()
	{
		this.OnChange?.Invoke(_runtimeValue);
	}
}
