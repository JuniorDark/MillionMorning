using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual;

public class MilMo_MeshHeld : MilMo_Lod
{
	public MilMo_MeshHeld(MilMo_VisualRep visualRep)
		: base(0, visualRep)
	{
	}

	public MilMo_MeshHeld(MilMo_VisualRep visualRep, GameObject gameObject)
		: base(0, visualRep, gameObject, 0f)
	{
	}

	public override void InitializeMaterials(MilMo_ResourceManager.Priority priority, bool pauseMode, bool async)
	{
		if (GameObject == null || Culled)
		{
			return;
		}
		base.Renderer = GameObject.GetComponent<Renderer>();
		if (base.Renderer != null)
		{
			if (base.Materials.Count == 0)
			{
				return;
			}
			Material[] array = new Material[base.Materials.Count];
			int num = 0;
			foreach (MilMo_Material material in base.Materials)
			{
				material.Create(base.ParentVisualRep.Path, base.ParentVisualRep.Name, base.ParentVisualRep.AssetTag, priority, pauseMode, async);
				material.SetColor(base.ParentVisualRep.DefaultColor);
				array[num] = material.Material;
				num++;
			}
			base.Renderer.materials = array;
		}
		else
		{
			Debug.Log("Trying to load MeshHeld without a renderer. " + base.ParentVisualRep.FullPath);
		}
	}

	public override void Write(MilMo_SFFile file, MilMo_Lod template)
	{
		file.AddAndWrite("<MESHHELD>");
		if ((bool)base.Renderer)
		{
			int num = 0;
			Material[] sharedMaterials = base.Renderer.sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (template != null && num < template.Materials.Count)
				{
					MilMo_Material.WriteMaterial(material, file, num, template.Materials[num]);
				}
				else
				{
					MilMo_Material.WriteMaterial(material, file);
				}
				num++;
			}
		}
		file.AddAndWrite("</MESHHELD>");
	}
}
