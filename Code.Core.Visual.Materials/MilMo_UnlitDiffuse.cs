using System;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual.Materials;

public class MilMo_UnlitDiffuse : MilMo_Material
{
	private const string SHADER_FILENAME = "Junebug/UnlitDiffuse";

	private Vector2 _uvScroll = new Vector2(0f, 0f);

	private static readonly int Color = Shader.PropertyToID("_Color");

	private static readonly int USpeed = Shader.PropertyToID("_USpeed");

	private static readonly int VSpeed = Shader.PropertyToID("_VSpeed");

	protected override string Name => "Junebug/UnlitDiffuse";

	public MilMo_UnlitDiffuse()
		: base("Junebug/UnlitDiffuse")
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
		LoadAllTextures();
		base.Material.SetColor(Color, base.MainColor);
		base.Material.SetFloat(USpeed, _uvScroll.x);
		base.Material.SetFloat(VSpeed, _uvScroll.y);
		return true;
	}

	public static void Write(Material material, MilMo_SFFile file, int index, MilMo_Material template)
	{
		try
		{
			MilMo_UnlitDiffuse milMo_UnlitDiffuse = template as MilMo_UnlitDiffuse;
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
				MilMo_Material.WriteTexture(material, file, milMo_UnlitDiffuse, "_MainTex", "Diffuse");
			}
			Color color = material.GetColor(Color);
			if (milMo_UnlitDiffuse == null || !MilMo_Utility.Equals(color, milMo_UnlitDiffuse.MainColor))
			{
				file.AddRow();
				file.Write("MainColor");
				file.Write(color);
			}
			Vector2 vector = new Vector2(material.GetFloat(USpeed), material.GetFloat(VSpeed));
			if (!MilMo_Utility.Equals(vector, new Vector2(0f, 0f)) && (milMo_UnlitDiffuse == null || !MilMo_Utility.Equals(vector, milMo_UnlitDiffuse._uvScroll)))
			{
				file.AddRow();
				file.Write("UVScroll");
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
