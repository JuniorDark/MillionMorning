using Code.Core.Global;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using UnityEngine;

namespace Code.World.GUI.Homes;

public class MilMoAttachNodeMarker : MilMo_Button
{
	private Transform _nodeTransform;

	public MilMoAttachNodeMarker(MilMo_UserInterface ui, Transform nodeTransform, ButtonFunc clickCallback)
		: base(ui)
	{
		Identifier = "AttachNodeMarker";
		_nodeTransform = nodeTransform;
		SetAlignment(MilMo_GUI.Align.BottomCenter);
		ScaleNow(24f, 24f);
		SetAllTextures("Batch01/Textures/Homes/GreenNode1");
		SetHoverColor(1f, 1f, 1f, 1f);
		SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		SetFadeInSpeed(0.05f);
		SetFadeOutSpeed(1f);
		SetDefaultColor(1f, 1f, 1f, 0.4f);
		Function = clickCallback;
		ui.AddChild(this);
	}

	public override void Draw()
	{
		if (Enabled && !(_nodeTransform == null))
		{
			Vector3 vector = MilMo_Global.Camera.WorldToScreenPoint(_nodeTransform.position);
			if (vector.z > 0f)
			{
				GoToNow(vector.x / base.Res.x, ((float)MilMo_Global.Camera.pixelHeight - vector.y) / base.Res.y);
				base.Draw();
			}
		}
	}

	public void Destroy()
	{
		_nodeTransform = null;
		UI.RemoveChild(this);
	}
}
