using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.BodyPack.ColorSystem;

public class MilMo_Saturation : MilMo_ColorAction
{
	private float _saturation;

	private static readonly int Saturation = Shader.PropertyToID("_Saturation");

	public MilMo_Saturation()
	{
		base.Name = "Saturation";
	}

	public override bool Read(MilMo_SFFile file)
	{
		if (!file.HasMoreTokens())
		{
			Debug.LogWarning("Got hue saturation without enough arguments in '" + file.Path + "' at line " + file.GetLineNumber());
			return false;
		}
		_saturation = file.GetFloat() / 100f;
		if (file.HasMoreTokens())
		{
			file.GetFloat();
		}
		return true;
	}

	public override void Apply(Material m, int layer)
	{
		m.SetFloat(Saturation, _saturation);
	}
}
