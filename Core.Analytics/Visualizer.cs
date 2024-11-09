using System.Collections;
using Code.Core.Global;
using UnityEngine;

namespace Core.Analytics;

public class Visualizer
{
	private readonly Texture2D _texture;

	private float _frameFps;

	private float _fixedFrameFps;

	public Visualizer()
	{
		_texture = MakeTex(4, 4);
	}

	private Texture2D MakeTex(int width, int height)
	{
		Color[] array = new Color[width * height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Color.white;
		}
		Texture2D texture2D = new Texture2D(width, height);
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	public void Draw(float fps, float fixedFPS, IEnumerable pingTimes)
	{
		float num = (float)Screen.height - 100f;
		_frameFps = Mathf.Lerp(_frameFps, fps, Time.deltaTime);
		float num2 = _frameFps / 60f;
		float num3 = num * num2;
		GUI.color = new Color(1f * (1f - num2), 1f * num2, 0.1f, 0.9f);
		GUI.DrawTexture(new Rect(10f, 20f + (num - num3), 16f, num3), _texture, ScaleMode.StretchToFill);
		GUI.color = new Color(0f, 0f, 0f, 1f);
		GUI.Label(new Rect(10f, 20f + num, 32f, 20f), fps.ToString("f0"));
		_fixedFrameFps = Mathf.Lerp(_fixedFrameFps, fixedFPS, Time.deltaTime);
		float num4 = _fixedFrameFps / 70f;
		float num5 = num * num4;
		GUI.color = new Color(1f * (1f - num4), 1f * num4, 0.1f, 0.9f);
		GUI.DrawTexture(new Rect(40f, 20f + (num - num5), 16f, num5), _texture, ScaleMode.StretchToFill);
		GUI.color = new Color(0f, 0f, 0f, 1f);
		GUI.Label(new Rect(40f, 20f + num, 32f, 20f), fixedFPS.ToString("f0"));
		float num6 = 0f;
		int num7 = 0;
		foreach (float pingTime in pingTimes)
		{
			float num9 = num * pingTime;
			GUI.color = new Color(pingTime, 1f - pingTime, 0.1f, 0.9f);
			GUI.DrawTexture(new Rect(70f + (float)(3 * num7), 20f + (num - num9), 3f, num9), _texture, ScaleMode.StretchToFill);
			num6 += pingTime;
			num7++;
		}
		float num10 = num6 / (float)num7;
		GUI.color = new Color(0f, 0f, 0f, 1f);
		GUI.Label(new Rect(70f, 20f + num, 70f, 20f), num10.ToString("f2") + "s");
		GUI.Label(new Rect(150f, 20f + num, 150f, 20f), "Extrapolation " + (MilMo_Global.PositionExtrapolation ? "ON" : "OFF"));
		GUI.Label(new Rect(350f, 20f + num, 200f, 20f), "Cubic Interpolation " + (MilMo_Global.CubicInterpolation ? "ON" : "OFF"));
	}
}
