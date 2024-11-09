using System.Collections.Generic;
using System.IO;
using Core.BodyShapes;
using Core.Colors;
using UnityEngine;

namespace Core.Avatar;

public class BodyPackComponent : MonoBehaviour
{
	[SerializeField]
	private bool autoApply;

	[SerializeField]
	[Range(2f, 2048f)]
	private int atlasWidth = 1024;

	[SerializeField]
	[Range(2f, 2048f)]
	private int atlasHeight = 1024;

	[SerializeField]
	private Texture2D writeToTexture;

	[SerializeField]
	private Renderer targetRenderer;

	[SerializeField]
	private ScriptableShape skin;

	[SerializeField]
	private ScriptableShape mouth;

	[SerializeField]
	private ScriptableShape eyes;

	[SerializeField]
	private ScriptableShape eyeBrows;

	[SerializeField]
	private ScriptableShape eyeBalls;

	[SerializeField]
	private ScriptableShape teeth;

	[SerializeField]
	private ScriptableShape eyeSpecular1;

	[SerializeField]
	private ScriptableShape eyeSpecular2;

	[SerializeField]
	private ScriptableColor skinColor;

	[SerializeField]
	private ScriptableColor hairColor;

	[SerializeField]
	private ScriptableColor eyeColor;

	private readonly Rect _regionMain = new Rect(0f, 0f, 0.5f, 0.5f);

	private readonly Rect _regionMouth = new Rect(0f, 0.5f, 0.5f, 0.5f);

	private readonly Rect _regionEyes = new Rect(0.5f, 0f, 45f / 128f, 0.5f);

	private readonly Rect _regionEyeBrows = new Rect(0.39453125f, 0.5f, 0.10546875f, 0.5f);

	private readonly Rect _regionTeeth = new Rect(0.5f, 57f / 128f, 45f / 128f, 7f / 128f);

	private readonly Rect _regionEyeBalls = new Rect(109f / 128f, 0f, 19f / 128f, 0.5f);

	private readonly Rect _regionSkinColor1 = new Rect(0f, 0f, 0.5f, 0.5f);

	private readonly Rect _regionSkinColor2 = new Rect(0f, 0.5f, 0.3935547f, 0.5f);

	private readonly Rect _regionSkinColor3 = new Rect(0.5f, 0f, 45f / 128f, 57f / 128f);

	private readonly Rect _regionHairColor = new Rect(0.39453125f, 0.5f, 0.10546875f, 0.5f);

	private readonly Rect _regionEyeColor = new Rect(109f / 128f, 0f, 19f / 128f, 0.5f);

	private readonly List<Rect> _regionsEyeSpecular1 = new List<Rect>
	{
		new Rect(0.5253906f, 0.029296875f, 0.0625f, 0.0625f),
		new Rect(0.6425781f, 0.029296875f, 0.0625f, 0.0625f),
		new Rect(0.7597656f, 0.029296875f, 0.0625f, 0.0625f),
		new Rect(0.5253906f, 9f / 64f, 0.0625f, 0.0625f),
		new Rect(0.6425781f, 9f / 64f, 0.0625f, 0.0625f),
		new Rect(0.7597656f, 9f / 64f, 0.0625f, 0.0625f),
		new Rect(0.5253906f, 0.6230469f, 0.0625f, 0.0625f),
		new Rect(0.6425781f, 0.6230469f, 0.0625f, 0.0625f),
		new Rect(0.7597656f, 0.6230469f, 0.0625f, 0.0625f),
		new Rect(0.5253906f, 0.51171875f, 0.0625f, 0.0625f),
		new Rect(0.6425781f, 0.51171875f, 0.0625f, 0.0625f),
		new Rect(0.7597656f, 0.51171875f, 0.0625f, 0.0625f)
	};

	private readonly Rect _regionEyeSpecular2 = new Rect(0.5f, 0.25f, 0.125f, 0.125f);

	private readonly ShapeRenderer _shapeRenderer = new ShapeRenderer();

	private readonly ColorRenderer _colorRenderer = new ColorRenderer();

	private Material _material;

	private Texture2D _result;

	private void OnValidate()
	{
		if (autoApply)
		{
			Apply();
		}
	}

	public void Apply()
	{
		Debug.Log("Apply");
		if (!targetRenderer)
		{
			Debug.LogWarning("Missing target");
			return;
		}
		if (_shapeRenderer == null || _colorRenderer == null)
		{
			Debug.LogWarning("Missing renderers");
			return;
		}
		ResetResult();
		Render();
		HandleResult();
	}

	private void HandleResult()
	{
		if (Application.isPlaying)
		{
			UpdateRenderer();
		}
		else
		{
			WriteResultsToDisc();
		}
	}

	private void UpdateRenderer()
	{
		targetRenderer.sharedMaterial.mainTexture = _result;
	}

	private void SaveTexture2D(in Texture2D tex, string path)
	{
		byte[] bytes = tex.EncodeToPNG();
		File.WriteAllBytes(path, bytes);
	}

	private void WriteResultsToDisc()
	{
	}

	private void Render()
	{
		RenderTexture temporary = RenderTexture.GetTemporary(atlasWidth, atlasHeight);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = temporary;
		GL.Clear(clearDepth: true, clearColor: true, Color.clear);
		GL.PushMatrix();
		GL.LoadOrtho();
		RenderSkinParts();
		RenderColors();
		RenderCopyOperations();
		GL.PopMatrix();
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(temporary);
	}

	private void RenderSkinParts()
	{
		List<ShapePart> list = PackSkinParts();
		if (list.Count >= 1)
		{
			_shapeRenderer.Render(list);
			_result.ReadPixels(new Rect(0f, 0f, atlasWidth, atlasHeight), 0, 0, recalculateMipMaps: true);
			_result.Apply();
		}
	}

	private void RenderColors()
	{
		List<ColorPart> list = PackColors();
		if (list.Count >= 1)
		{
			_colorRenderer.Render(list);
			_result.ReadPixels(new Rect(0f, 0f, atlasWidth, atlasHeight), 0, 0, recalculateMipMaps: true);
			_result.Apply();
		}
	}

	private void RenderCopyOperations()
	{
		List<ColorPart> list = PackCopyOperations();
		if (list.Count >= 1)
		{
			_colorRenderer.Render(list);
			_result.ReadPixels(new Rect(0f, 0f, atlasWidth, atlasHeight), 0, 0, recalculateMipMaps: true);
			_result.Apply();
		}
	}

	private List<ShapePart> PackSkinParts()
	{
		List<ShapePart> list = new List<ShapePart>();
		if ((bool)skin)
		{
			list.Add(new ShapePart(_regionMain, skin.GetTexture()));
		}
		if ((bool)mouth)
		{
			list.Add(new ShapePart(_regionMouth, mouth.GetTexture()));
		}
		if ((bool)eyes)
		{
			list.Add(new ShapePart(_regionEyes, eyes.GetTexture()));
		}
		if ((bool)eyeBrows)
		{
			list.Add(new ShapePart(_regionEyeBrows, eyeBrows.GetTexture()));
		}
		if ((bool)eyeBalls)
		{
			list.Add(new ShapePart(_regionEyeBalls, eyeBalls.GetTexture()));
		}
		if ((bool)teeth)
		{
			list.Add(new ShapePart(_regionTeeth, teeth.GetTexture()));
		}
		return list;
	}

	private List<ColorPart> PackColors()
	{
		List<ColorPart> list = new List<ColorPart>();
		if ((bool)skinColor)
		{
			list.AddRange(new List<ColorPart>
			{
				new ColorPart(_regionSkinColor1, skinColor, _result),
				new ColorPart(_regionSkinColor2, skinColor, _result),
				new ColorPart(_regionSkinColor3, skinColor, _result)
			});
		}
		if ((bool)hairColor)
		{
			list.Add(new ColorPart(_regionHairColor, hairColor, _result));
		}
		if ((bool)eyeColor)
		{
			list.Add(new ColorPart(_regionEyeColor, eyeColor, _result, null, "EYE"));
		}
		return list;
	}

	private List<ColorPart> PackCopyOperations()
	{
		List<ColorPart> list = new List<ColorPart>();
		if ((bool)eyeSpecular1)
		{
			Texture2D texture = eyeSpecular1.GetTexture();
			foreach (Rect item in _regionsEyeSpecular1)
			{
				list.Add(new ColorPart(item, null, _result, texture, "COPY"));
			}
		}
		if ((bool)eyeSpecular2)
		{
			Texture2D texture2 = eyeSpecular2.GetTexture();
			list.Add(new ColorPart(_regionEyeSpecular2, null, _result, texture2, "COPY"));
		}
		return list;
	}

	private void ResetResult()
	{
		if ((bool)_result)
		{
			if (!Application.isPlaying)
			{
				Object.DestroyImmediate(_result);
			}
			else
			{
				Object.Destroy(_result);
			}
		}
		_result = new Texture2D(atlasWidth, atlasHeight);
	}

	public Texture GetResult()
	{
		return _result;
	}
}
