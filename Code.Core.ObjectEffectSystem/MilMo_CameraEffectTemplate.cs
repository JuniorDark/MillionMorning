using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_CameraEffectTemplate : MilMo_ObjectEffectTemplate
{
	public readonly string Event;

	public MilMo_CameraEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Event"))
			{
				Event = file.GetString();
			}
			else
			{
				file.NextToken();
			}
		}
	}

	public override MilMo_ObjectEffect CreateObjectEffect(GameObject gameObject)
	{
		return new MilMo_CameraEffect(gameObject, this);
	}
}
