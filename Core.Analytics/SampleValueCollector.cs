using System;
using System.Collections.Generic;

namespace Core.Analytics;

public class SampleValueCollector
{
	private readonly List<float> _values = new List<float>();

	private float _sum;

	public float Median
	{
		get
		{
			if (_values.Count == 0)
			{
				return 0f;
			}
			if (_values.Count % 2 == 1)
			{
				return _values[_values.Count / 2];
			}
			return (_values[_values.Count / 2 - 1] + _values[_values.Count / 2]) / 2f;
		}
	}

	public float Mean
	{
		get
		{
			if (_values.Count == 0)
			{
				return 0f;
			}
			return _sum / (float)_values.Count;
		}
	}

	public void AddSample(float value)
	{
		_sum += value;
		if (_values.Count == 0 || value >= _values[_values.Count - 1])
		{
			_values.Add(value);
			return;
		}
		if (value < _values[0])
		{
			_values.Insert(0, value);
			return;
		}
		int num = 0;
		int num2 = _values.Count - 1;
		do
		{
			int num3 = (int)((double)(num + num2) * 0.5);
			if ((double)Math.Abs(value - _values[num3]) <= 0.0)
			{
				_values.Insert(num3, value);
				return;
			}
			if (value < _values[num3])
			{
				num2 = num3;
			}
			else
			{
				num = num3;
			}
		}
		while (num2 - num != 1 || !(value < _values[num2]) || !(value > _values[num]));
		_values.Insert(num2, value);
	}

	public void Clear()
	{
		_values.Clear();
		_sum = 0f;
	}
}
