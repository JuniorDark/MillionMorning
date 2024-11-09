using Code.Core.Sound;
using UnityEngine;

namespace Code.Core.Utility;

public static class MilMo_AudioUtils
{
	private const float ROLL_OFF_DISTANCE = 500f;

	public static float RollOffToDistance(float rollOffFactor)
	{
		return 500f * rollOffFactor;
	}

	public static void SetRollOffFactor(AudioSourceWrapper audioSource, float rollOffFactor, float minDistanceAdd = 0f)
	{
		if ((bool)audioSource)
		{
			audioSource.RolloffMode = AudioRolloffMode.Logarithmic;
			audioSource.MinDistance = 2.75f + minDistanceAdd;
			audioSource.MaxDistance = 2.75f + 500f * rollOffFactor;
			audioSource.SpatialBlend = 1f;
		}
	}

	public static float GetRollOffFactorFromDistance(float distance)
	{
		return distance / 500f;
	}
}
