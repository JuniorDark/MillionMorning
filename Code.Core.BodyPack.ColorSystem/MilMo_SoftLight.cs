using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.BodyPack.ColorSystem;

public class MilMo_SoftLight : MilMo_ColorAction
{
	private Color _color;

	public MilMo_SoftLight()
	{
		base.Name = "Softlight";
	}

	public override bool Read(MilMo_SFFile file)
	{
		if (!file.HasMoreTokens())
		{
			Debug.LogWarning("Got soft light without color in '" + file.Path + "' at line " + file.GetLineNumber());
			return false;
		}
		_color = file.GetColorFromInt();
		return true;
	}

	public override void Apply(Material m, int layer)
	{
		m.SetColor("_Color" + layer, _color);
	}
}
