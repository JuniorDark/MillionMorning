using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.BodyPack.ColorSystem;

public class MilMo_Color
{
	private const string THE_SHADER_PATH = "Shaders/BodyBuilder/MilMoColor";

	private readonly List<MilMo_ColorAction> _colorActions = new List<MilMo_ColorAction>();

	private static readonly int Stitch = Shader.PropertyToID("_Stitch");

	private static readonly int BlendTex1 = Shader.PropertyToID("_BlendTex1");

	public Color IconColor { get; private set; }

	public string Name { get; private set; }

	public bool Read(MilMo_SFFile file)
	{
		if (!file.NextRow())
		{
			return false;
		}
		Name = file.Name;
		if (file.IsNext("<color>"))
		{
			while (file.NextRow())
			{
				if (file.IsNext("IconColor"))
				{
					IconColor = file.GetColorFromInt();
				}
				else if (file.IsNext("ColorIndex"))
				{
					file.GetInt();
				}
				else if (file.IsNext("ColorName"))
				{
					Name = file.GetString();
				}
				else if (file.IsNext("Overlay"))
				{
					MilMo_ColorAction milMo_ColorAction = new MilMo_Overlay();
					if (milMo_ColorAction.Read(file))
					{
						_colorActions.Add(milMo_ColorAction);
					}
					else
					{
						Debug.LogWarning("Failed to create overlay color action for color " + file.Path);
					}
				}
				else if (file.IsNext("Softlight"))
				{
					MilMo_ColorAction milMo_ColorAction2 = new MilMo_SoftLight();
					if (milMo_ColorAction2.Read(file))
					{
						_colorActions.Add(milMo_ColorAction2);
					}
					else
					{
						Debug.LogWarning("Failed to create soft light color action for color " + file.Path);
					}
				}
				else if (file.IsNext("Saturation"))
				{
					MilMo_ColorAction milMo_ColorAction3 = new MilMo_Saturation();
					if (milMo_ColorAction3.Read(file))
					{
						_colorActions.Add(milMo_ColorAction3);
					}
					else
					{
						Debug.LogWarning("Failed to create saturation color action for color " + file.Path);
					}
				}
				else
				{
					if (file.IsNext("</color>"))
					{
						break;
					}
					Debug.LogWarning("Got unknown color action in '" + file.Path + "' at line " + file.GetLineNumber());
				}
			}
			_colorActions.Reverse();
			return true;
		}
		Debug.LogWarning("No <color> start tag");
		return false;
	}

	public void Apply(Rect r, Texture stitchTexture, Texture blendTexture = null, string shaderKeyword = "")
	{
		Material material = new Material(Resources.Load<Shader>("Shaders/BodyBuilder/MilMoColor"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		if (!string.IsNullOrEmpty(shaderKeyword))
		{
			material.EnableKeyword(shaderKeyword);
		}
		int num = 1;
		foreach (MilMo_ColorAction colorAction in _colorActions)
		{
			if (colorAction.Name == "Overlay")
			{
				material.EnableKeyword("OVERLAY" + num);
			}
			if (colorAction.Name == "Softlight")
			{
				material.EnableKeyword("SOFTLIGHT" + num);
			}
			if (colorAction.Name == "Saturation")
			{
				material.EnableKeyword("SATURATION");
			}
			colorAction.Apply(material, num++);
		}
		material.SetTexture(Stitch, stitchTexture);
		if ((bool)blendTexture)
		{
			material.SetTexture(BlendTex1, blendTexture);
		}
		material.SetPass(0);
		MilMo_ColorShaderUtil.DrawQuad(r);
		if (!Application.isPlaying)
		{
			Object.DestroyImmediate(material);
		}
		else
		{
			Object.Destroy(material);
		}
	}
}
