using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.BodyPack.ColorSystem;

public class MilMo_Overlay : MilMo_ColorAction
{
	private Color _color;

	public MilMo_Overlay()
	{
		base.Name = "Overlay";
	}

	public override bool Read(MilMo_SFFile file)
	{
		if (!file.HasMoreTokens())
		{
			Debug.LogWarning("Got overlay without color in '" + file.Path + "' at line " + file.GetLineNumber());
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
