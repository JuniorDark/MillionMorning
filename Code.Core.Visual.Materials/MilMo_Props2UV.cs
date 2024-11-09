using System;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Materials;

public class MilMo_Props2UV : MilMo_Material
{
	private const string SHADER_FILENAME = "Junebug/Props2UV";

	private string _velvetRamp = "";

	private Color _ambientColor = new Color(0f, 0f, 0f, 0f);

	private Vector4 _velvetChannel = new Vector4(0f, 1f, 0f, 0f);

	private Color _velvetColor = new Color(1f, 1f, 1f, 1f);

	private static readonly int Color = Shader.PropertyToID("_Color");

	private static readonly int AmbientColor = Shader.PropertyToID("_AmbientColor");

	private static readonly int VelvetColor = Shader.PropertyToID("_VelvetColor");

	private static readonly int VelvetChannel = Shader.PropertyToID("_VelvetChannel");

	private static readonly int AmbientTex = Shader.PropertyToID("_AmbientTex");

	private static readonly int Ramp1 = Shader.PropertyToID("_Ramp");

	protected override string Name => "Junebug/Props2UV";

	public MilMo_Props2UV()
		: base("Junebug/Props2UV")
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
			else if (file.IsNext("Ambient"))
			{
				ReadTexture(file, "_AmbientTex");
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
			else if (file.IsNext("AmbientColor"))
			{
				_ambientColor = file.GetColor();
			}
			else if (file.IsNext("MainColor"))
			{
				base.MainColor = file.GetColor();
			}
		}
	}

	protected override bool CreateInternal()
	{
		LoadAllTextures(_velvetRamp);
		base.Material.SetColor(Color, base.MainColor);
		base.Material.SetColor(AmbientColor, _ambientColor);
		base.Material.SetColor(VelvetColor, _velvetColor);
		base.Material.SetVector(VelvetChannel, _velvetChannel);
		return true;
	}

	public static void Write(Material material, MilMo_SFFile file, int index, MilMo_Material template)
	{
		try
		{
			MilMo_Props2UV milMo_Props2UV = template as MilMo_Props2UV;
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
				MilMo_Material.WriteTexture(material, file, milMo_Props2UV, "_MainTex", "Diffuse");
			}
			if ((bool)material.GetTexture(AmbientTex))
			{
				MilMo_Material.WriteTexture(material, file, milMo_Props2UV, "_AmbientTex", "Ambient");
			}
			Texture texture = material.GetTexture(Ramp1);
			if ((bool)texture && texture.name != "VelvetRamp")
			{
				string text = MilMo_ResourceManager.Instance.ResolveTexturePath(texture.name);
				if (milMo_Props2UV == null || text != milMo_Props2UV._velvetRamp)
				{
					file.AddRow();
					file.Write("VelvetRamp");
					file.Write(text);
				}
			}
			Color color = material.GetColor(Color);
			if (milMo_Props2UV == null || !MilMo_Utility.Equals(color, milMo_Props2UV.MainColor))
			{
				file.AddRow();
				file.Write("MainColor");
				file.Write(color);
			}
			Color color2 = material.GetColor(AmbientColor);
			if (milMo_Props2UV == null || !MilMo_Utility.Equals(color2, milMo_Props2UV._ambientColor))
			{
				file.AddRow();
				file.Write("AmbientColor");
				file.Write(color2);
			}
			Color color3 = material.GetColor(VelvetColor);
			if (milMo_Props2UV == null || !MilMo_Utility.Equals(color3, milMo_Props2UV._velvetColor))
			{
				file.AddRow();
				file.Write("VelvetColor");
				file.Write(color3);
			}
			Vector4 vector = material.GetVector(VelvetChannel);
			if (milMo_Props2UV == null || !MilMo_Utility.Equals(vector, milMo_Props2UV._velvetChannel))
			{
				file.AddRow();
				file.Write("VelvetChannel");
				file.Write(vector);
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
