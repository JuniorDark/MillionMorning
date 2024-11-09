using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual;

public class MilMo_Gib : MilMo_Lod
{
	public Collider Collider;

	public Rigidbody Rigidbody;

	public MilMo_Gib(int index, MilMo_VisualRep visualRep)
		: base(index, visualRep)
	{
	}

	public MilMo_Gib(int index, MilMo_VisualRep visualRep, GameObject gameObject, bool useParentMaterial)
		: base(index, visualRep, gameObject, 0f)
	{
		if (useParentMaterial && (bool)base.Renderer && visualRep != null && (bool)visualRep.Renderer)
		{
			base.Renderer.sharedMaterials = visualRep.Renderer.sharedMaterials;
		}
	}

	public override void Write(MilMo_SFFile file, MilMo_Lod template)
	{
		if (!(template is MilMo_Gib milMo_Gib))
		{
			return;
		}
		file.AddAndWrite("<GIB>");
		file.AddAndWrite("Index");
		file.Write(base.Index);
		Renderer component = GameObject.GetComponent<Renderer>();
		if (component != null)
		{
			int num = 0;
			Material[] sharedMaterials = component.sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (num < milMo_Gib.Materials.Count)
				{
					MilMo_Material.WriteMaterial(material, file, num, milMo_Gib.Materials[num]);
				}
				else
				{
					MilMo_Material.WriteMaterial(material, file);
				}
				num++;
			}
		}
		file.AddAndWrite("</GIB>");
	}
}
