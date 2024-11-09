using System.Collections.Generic;
using UnityEngine;

namespace Code.World.GUI.ShopPopups;

public sealed class WeaponStatBrackets
{
	private readonly float[] _attackValues = new float[11]
	{
		0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f,
		10f
	};

	private readonly float[] _magicValues = new float[11]
	{
		0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f,
		10f
	};

	private readonly float[] _speedValues = new float[11]
	{
		3f, 1f, 0.7f, 0.55f, 0.47f, 0.4f, 0.35f, 0.3f, 0.23f, 0.1f,
		0f
	};

	private readonly float[] _rangeValues = new float[11]
	{
		0f, 1.25f, 1.5f, 1.85f, 2f, 10f, 15f, 20f, 25f, 30f,
		40f
	};

	public int GetAttackPoints(float attack)
	{
		return GetClosestIndex(attack, _attackValues);
	}

	public int GetMagicPoints(float magic)
	{
		return GetClosestIndex(magic, _magicValues);
	}

	public int GetSpeedPoints(float speed)
	{
		return Mathf.Min(GetClosestIndex(speed, _speedValues) + 1, 10);
	}

	public int GetRangePoints(float range)
	{
		return GetClosestIndex(range, _rangeValues);
	}

	private int GetClosestIndex(float value, IReadOnlyList<float> array)
	{
		float num = 1000000f;
		int result = 0;
		for (int i = 0; i < array.Count; i++)
		{
			float num2 = Mathf.Abs(value - array[i]);
			if (!(num2 > num))
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}
}
