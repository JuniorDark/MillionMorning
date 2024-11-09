using System;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Materials;

public class MilMo_SkyLayer : MilMo_Material
{
	private const string SHADER_FILENAME = "Junebug/SkyLayer";

	private string _outerTexture;

	private string _middleTexture;

	private string _innerTexture;

	private Vector2 _outerOffset = new Vector2(0f, 0f);

	private Vector2 _middleOffset = new Vector2(0f, 0f);

	private Vector2 _innerOffset = new Vector2(0f, 0f);

	private Color _uvAnimation = new Color(0f, 0f, 0f, 0f);

	private float _alphaFade = 0.01f;

	private float _alphaFadeOffset;

	private Color _tilt = new Color(0.5f, 0.5f, 0.5f, 0f);

	private float _fogColorImpact = 1f;

	private static readonly int UVAnimation = Shader.PropertyToID("_UVAnimation");

	private static readonly int AlphaFade = Shader.PropertyToID("_AlphaFade");

	private static readonly int AlphaFadeOffset = Shader.PropertyToID("_AlphaFadeOffset");

	private static readonly int Tilt = Shader.PropertyToID("_Tilt");

	private static readonly int FogColorImpact = Shader.PropertyToID("_FogColorImpact");

	private static readonly int Outer = Shader.PropertyToID("_Outer");

	private static readonly int Middle = Shader.PropertyToID("_Middle");

	private static readonly int Inner = Shader.PropertyToID("_Inner");

	protected override string Name => "Junebug/SkyLayer";

	public MilMo_SkyLayer()
		: base("Junebug/SkyLayer")
	{
	}

	public override void Load(MilMo_SFFile file)
	{
		while (file.NextRow() && !file.IsNext("</MAT>"))
		{
			if (file.IsNext("Outer"))
			{
				_outerTexture = file.GetString();
			}
			else if (file.IsNext("Middle"))
			{
				_middleTexture = file.GetString();
			}
			else if (file.IsNext("Inner"))
			{
				_innerTexture = file.GetString();
			}
			else if (file.IsNext("OuterOffset"))
			{
				float @float = file.GetFloat();
				float y = (file.HasMoreTokens() ? file.GetFloat() : 0f);
				_outerOffset = new Vector2(@float, y);
			}
			else if (file.IsNext("MiddleOffset"))
			{
				float float2 = file.GetFloat();
				float y2 = (file.HasMoreTokens() ? file.GetFloat() : 0f);
				_middleOffset = new Vector2(float2, y2);
			}
			else if (file.IsNext("InnerOffset"))
			{
				float float3 = file.GetFloat();
				float y3 = (file.HasMoreTokens() ? file.GetFloat() : 0f);
				_innerOffset = new Vector2(float3, y3);
			}
			else if (file.IsNext("UVAnimation"))
			{
				_uvAnimation = file.GetColor();
			}
			else if (file.IsNext("AlphaFade"))
			{
				_alphaFade = file.GetFloat();
			}
			else if (file.IsNext("AlphaFadeOffset"))
			{
				_alphaFadeOffset = file.GetFloat();
			}
			else if (file.IsNext("Tilt"))
			{
				_tilt = file.GetColor();
			}
			else if (file.IsNext("FogColorImpact"))
			{
				_fogColorImpact = file.GetFloat();
			}
		}
	}

	protected override bool CreateInternal()
	{
		TextureCountToLoad = 3;
		LoadTexture(_outerTexture, "_Outer", _outerOffset, new Vector2(1f, 1f), delegate
		{
			TextureLoaded();
		});
		LoadTexture(_middleTexture, "_Middle", _middleOffset, new Vector2(1f, 1f), delegate
		{
			TextureLoaded();
		});
		LoadTexture(_innerTexture, "_Inner", _innerOffset, new Vector2(1f, 1f), delegate
		{
			TextureLoaded();
		});
		base.Material.SetColor(UVAnimation, _uvAnimation);
		base.Material.SetFloat(AlphaFade, _alphaFade);
		base.Material.SetFloat(AlphaFadeOffset, _alphaFadeOffset);
		base.Material.SetColor(Tilt, _tilt);
		base.Material.SetFloat(FogColorImpact, _fogColorImpact);
		return true;
	}

	public static void Write(Material material, MilMo_SFFile file, int index, MilMo_Material template)
	{
		try
		{
			MilMo_SkyLayer milMo_SkyLayer = template as MilMo_SkyLayer;
			file.AddRow();
			file.Write("<MAT>");
			if (index != -1)
			{
				file.AddRow();
				file.Write("Index");
				file.Write(index);
			}
			file.AddRow();
			file.Write(material.shader.name);
			Texture texture = material.GetTexture(Outer);
			if ((bool)texture)
			{
				string text = MilMo_ResourceManager.Instance.ResolveTexturePath(texture.name);
				if (milMo_SkyLayer == null || text != milMo_SkyLayer._outerTexture)
				{
					file.AddRow();
					file.Write("Outer");
					file.Write(text);
				}
				Vector2 textureOffset = material.GetTextureOffset(Outer);
				if (milMo_SkyLayer == null || !MilMo_Utility.Equals(textureOffset, milMo_SkyLayer._outerOffset))
				{
					file.AddRow();
					file.Write("OuterOffset");
					file.Write(textureOffset);
				}
			}
			Texture texture2 = material.GetTexture(Middle);
			if ((bool)texture2)
			{
				string text2 = MilMo_ResourceManager.Instance.ResolveTexturePath(texture2.name);
				if (milMo_SkyLayer == null || text2 != milMo_SkyLayer._middleTexture)
				{
					file.AddRow();
					file.Write("Middle");
					file.Write(text2);
				}
				Vector2 textureOffset2 = material.GetTextureOffset(Middle);
				if (milMo_SkyLayer == null || !MilMo_Utility.Equals(textureOffset2, milMo_SkyLayer._middleOffset))
				{
					file.AddRow();
					file.Write("MiddleOffset");
					file.Write(textureOffset2);
				}
			}
			Texture texture3 = material.GetTexture(Inner);
			if ((bool)texture3)
			{
				string text3 = MilMo_ResourceManager.Instance.ResolveTexturePath(texture3.name);
				if (milMo_SkyLayer == null || text3 != milMo_SkyLayer._innerTexture)
				{
					file.AddRow();
					file.Write("Inner");
					file.Write(text3);
				}
				Vector2 textureOffset3 = material.GetTextureOffset(Inner);
				if (milMo_SkyLayer == null || !MilMo_Utility.Equals(textureOffset3, milMo_SkyLayer._innerOffset))
				{
					file.AddRow();
					file.Write("InnerOffset");
					file.Write(textureOffset3);
				}
			}
			Color color = material.GetColor(UVAnimation);
			if (milMo_SkyLayer == null || !MilMo_Utility.Equals(color, milMo_SkyLayer._uvAnimation))
			{
				file.AddRow();
				file.Write("UVAnimation");
				file.Write(color);
			}
			float @float = material.GetFloat(AlphaFade);
			if (milMo_SkyLayer == null || (double)Math.Abs(@float - milMo_SkyLayer._alphaFade) > 0.001)
			{
				file.AddRow();
				file.Write("AlphaFade");
				file.Write(@float);
			}
			float float2 = material.GetFloat(AlphaFadeOffset);
			if (milMo_SkyLayer == null || (double)Math.Abs(float2 - milMo_SkyLayer._alphaFadeOffset) > 0.001)
			{
				file.AddRow();
				file.Write("AlphaFadeOffset");
				file.Write(float2);
			}
			Color color2 = material.GetColor(Tilt);
			if (milMo_SkyLayer == null || !MilMo_Utility.Equals(color2, milMo_SkyLayer._tilt))
			{
				file.AddRow();
				file.Write("Tilt");
				file.Write(color2);
			}
			float float3 = material.GetFloat(FogColorImpact);
			if (milMo_SkyLayer == null || (double)Math.Abs(float3 - milMo_SkyLayer._fogColorImpact) > 0.001)
			{
				file.AddRow();
				file.Write("FogColorImpact");
				file.Write(float3);
			}
			file.AddRow();
			file.Write("</MAT>");
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Failed to write " + material.shader.name + " material");
			Debug.LogWarning(ex.ToString());
		}
	}
}
