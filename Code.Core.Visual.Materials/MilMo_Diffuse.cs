using System;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Materials;

public sealed class MilMo_Diffuse : MilMo_Material
{
	private const string SHADER_FILENAME = "Junebug/Diffuse";

	private string _velvetRamp = "";

	private Vector4 _velvetChannel = new Vector4(0f, 1f, 0f, 0f);

	private Color _velvetColor = new Color(1f, 1f, 1f, 1f);

	private Vector2 _uvScroll = new Vector2(0f, 0f);

	private static readonly int Color = Shader.PropertyToID("_Color");

	private static readonly int VelvetColor = Shader.PropertyToID("_VelvetColor");

	private static readonly int VelvetChannel = Shader.PropertyToID("_VelvetChannel");

	private static readonly int USpeed = Shader.PropertyToID("_USpeed");

	private static readonly int VSpeed = Shader.PropertyToID("_VSpeed");

	private static readonly int Ramp = Shader.PropertyToID("_Ramp");

	protected override string Name => "Junebug/Diffuse";

	public MilMo_Diffuse()
		: base("Junebug/Diffuse")
	{
	}

	public override void Load(MilMo_SFFile file)
	{
		while (file.NextRow() && !file.IsNext("</MAT>"))
		{
			if (file.IsNext("Diffuse"))
			{
				ReadTexture(file, "_MainTex");
			}
			else if (file.IsNext("VelvetRamp"))
			{
				_velvetRamp = file.GetString();
			}
			else if (file.IsNext("VelvetChannel"))
			{
				_velvetChannel = file.GetVector3();
			}
			else if (file.IsNext("VelvetColor"))
			{
				_velvetColor = file.GetColor();
			}
			else if (file.IsNext("MainColor"))
			{
				base.MainColor = file.GetColor();
			}
			else if (file.IsNext("UVScroll"))
			{
				_uvScroll = file.GetVector2();
			}
		}
	}

	protected override bool CreateInternal()
	{
		LoadAllTextures(_velvetRamp);
		base.Material.SetColor(Color, base.MainColor);
		base.Material.SetColor(VelvetColor, _velvetColor);
		base.Material.SetVector(VelvetChannel, _velvetChannel);
		base.Material.SetFloat(USpeed, _uvScroll.x);
		base.Material.SetFloat(VSpeed, _uvScroll.y);
		return true;
	}

	public static void Write(Material material, MilMo_SFFile file, int index, MilMo_Material template)
	{
		try
		{
			MilMo_Diffuse milMo_Diffuse = template as MilMo_Diffuse;
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
			if (material.mainTexture != null)
			{
				MilMo_Material.WriteTexture(material, file, milMo_Diffuse, "_MainTex", "Diffuse");
			}
			Texture texture = material.GetTexture(Ramp);
			if ((bool)texture && texture.name != "VelvetRamp")
			{
				string text = MilMo_ResourceManager.Instance.ResolveTexturePath(texture.name);
				if (milMo_Diffuse == null || text != milMo_Diffuse._velvetRamp)
				{
					file.AddRow();
					file.Write("VelvetRamp");
					file.Write(text);
				}
			}
			Color color = material.GetColor(Color);
			if (milMo_Diffuse == null || !MilMo_Utility.Equals(color, milMo_Diffuse.MainColor))
			{
				file.AddRow();
				file.Write("MainColor");
				file.Write(color);
			}
			Color color2 = material.GetColor(VelvetColor);
			if (milMo_Diffuse == null || !MilMo_Utility.Equals(color2, milMo_Diffuse._velvetColor))
			{
				file.AddRow();
				file.Write("VelvetColor");
				file.Write(color2);
			}
			Vector4 vector = material.GetVector(VelvetChannel);
			if (milMo_Diffuse == null || !MilMo_Utility.Equals(vector, milMo_Diffuse._velvetChannel))
			{
				file.AddRow();
				file.Write("VelvetChannel");
				file.Write(vector);
			}
			Vector2 vector2 = new Vector2(material.GetFloat(USpeed), material.GetFloat(VSpeed));
			if (!MilMo_Utility.Equals(vector2, new Vector2(0f, 0f)) && (milMo_Diffuse == null || !MilMo_Utility.Equals(vector2, milMo_Diffuse._uvScroll)))
			{
				file.AddRow();
				file.Write("UVScroll");
				file.Write(vector2);
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
