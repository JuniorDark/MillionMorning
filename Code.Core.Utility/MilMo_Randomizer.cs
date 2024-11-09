using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Core.Utility;

public class MilMo_Randomizer
{
	public interface IRandomElement
	{
		float Probability { get; }

		float NormalizedProbability { get; set; }
	}

	private readonly List<IRandomElement> _elements;

	public MilMo_Randomizer(List<IRandomElement> elements)
	{
		_elements = elements;
		NormalizeProbabilities(-1);
	}

	public IRandomElement Next()
	{
		if (_elements.Count == 1)
		{
			return _elements[0];
		}
		float value = Random.value;
		float num = 0f;
		for (int i = 0; i < _elements.Count; i++)
		{
			if (!(_elements[i].NormalizedProbability <= 0f))
			{
				num += _elements[i].NormalizedProbability;
				if (!(value > num))
				{
					NormalizeProbabilities(i);
					return _elements[i];
				}
			}
		}
		NormalizeProbabilities(-1);
		return null;
	}

	private void NormalizeProbabilities(int lastReturnedIndex)
	{
		float num = _elements.Where((IRandomElement t, int i) => i != lastReturnedIndex).Sum((IRandomElement t) => t.Probability);
		if (num <= 0f)
		{
			return;
		}
		for (int j = 0; j < _elements.Count; j++)
		{
			if (j != lastReturnedIndex)
			{
				_elements[j].NormalizedProbability = _elements[j].Probability / num;
			}
			else
			{
				_elements[j].NormalizedProbability = 0f;
			}
		}
	}
}
