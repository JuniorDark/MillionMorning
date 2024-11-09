using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Settings;

[CreateAssetMenu(fileName = "ResolutionSettings", menuName = "Settings/Resolutions")]
public class ResolutionsSO : ScriptableObject
{
	[Serializable]
	public struct Resolution
	{
		public int width;

		public int height;
	}

	public List<Resolution> supportedResolutions;

	public Resolution GetWindowResolution(int index)
	{
		Resolution resolution = supportedResolutions[index];
		float num = (float)resolution.width / (float)resolution.height;
		for (int num2 = index; num2 >= 0; num2--)
		{
			Resolution result = supportedResolutions[num2];
			float num3 = (float)result.width / (float)result.height;
			if ((double)Math.Abs(num - num3) < 0.01 && ((float)Screen.currentResolution.width * 0.9f > (float)result.width || (float)Screen.currentResolution.height * 0.9f > (float)result.height))
			{
				return result;
			}
		}
		return supportedResolutions[0];
	}
}
