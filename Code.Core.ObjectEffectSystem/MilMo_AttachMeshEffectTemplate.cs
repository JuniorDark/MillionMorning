using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_AttachMeshEffectTemplate : MilMo_ObjectEffectTemplate
{
	public string VisualRepPath { get; private set; }

	public Vector3 Offset { get; private set; }

	public float Duration { get; private set; }

	public MilMo_AttachMeshEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		while (file.HasMoreTokens())
		{
			if (file.IsNext("VisualRep"))
			{
				VisualRepPath = file.GetString();
			}
			else if (file.IsNext("Offset"))
			{
				Offset = file.GetVector3();
			}
			else if (file.IsNext("Duration"))
			{
				Duration = file.GetFloat();
			}
			else
			{
				file.NextToken();
			}
		}
	}

	public override MilMo_ObjectEffect CreateObjectEffect(GameObject gameObject)
	{
		return new MilMo_AttachMeshEffect(gameObject, this);
	}
}
