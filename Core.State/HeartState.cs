using System;
using UnityEngine;

namespace Core.State;

public class HeartState : ScriptableObject
{
	public enum HeartValue
	{
		Empty,
		Half,
		Whole
	}

	public enum HeartSize
	{
		Half,
		Whole
	}

	[SerializeField]
	private HeartValue value;

	[SerializeField]
	private HeartSize size = HeartSize.Whole;

	public event Action<HeartValue> OnValueChange;

	public event Action<HeartSize> OnSizeChange;

	public void SetValue(HeartValue newValue)
	{
		value = newValue;
		this.OnValueChange?.Invoke(value);
	}

	public void SetSize(HeartSize newSize)
	{
		size = newSize;
		this.OnSizeChange?.Invoke(size);
	}

	public HeartValue GetValue()
	{
		return value;
	}

	public HeartSize GetSize()
	{
		return size;
	}

	private void OnValidate()
	{
		this.OnValueChange?.Invoke(value);
		this.OnSizeChange?.Invoke(size);
	}
}
