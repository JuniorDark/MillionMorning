using Code.Core.EventSystem;
using Code.Core.Template;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.World.Gameplay;

public class MilMo_GameplayTriggerReactionSplineRide : MilMo_GameplayTriggerReaction
{
	private MilMo_PlayerSpline _spline;

	public MilMo_GameplayTriggerReactionSplineRide(string templatePath)
	{
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("Spline", templatePath, TemplateLoaded);
	}

	public override bool MayActivate()
	{
		if (_spline != null)
		{
			return base.MayActivate();
		}
		return false;
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_Player player)
	{
		if (_spline != null)
		{
			MilMo_PlayerControllerSplineRide.Spline.ClearVisualization();
			MilMo_PlayerControllerSplineRide.Spline = _spline;
			MilMo_EventSystem.Instance.PostEvent("spline_ride_start", _spline);
			MilMo_World.Instance.ChangePlayerController(MilMo_PlayerControllerBase.ControllerType.SplineRide);
		}
	}

	public override void Activate(MilMo_GameplayObject obj, MilMo_RemotePlayer player)
	{
		if (_spline != null)
		{
			player.StartSplineRide(_spline);
		}
	}

	private void TemplateLoaded(MilMo_Template t, bool timeout)
	{
		if (timeout)
		{
			Debug.LogWarning("Failed to load spline template: timeout");
		}
		else if (!(t is MilMo_PlayerSplineTemplate template))
		{
			Debug.LogWarning("Failed to load spline template: template is null");
		}
		else
		{
			_spline = new MilMo_PlayerSpline(template);
		}
	}
}
