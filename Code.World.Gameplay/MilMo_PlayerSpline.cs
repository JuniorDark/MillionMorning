using Code.Core.Spline;

namespace Code.World.Gameplay;

public class MilMo_PlayerSpline : MilMo_GameObjectSpline
{
	public new MilMo_PlayerSplineTemplate Template => ((MilMo_Spline)this).Template as MilMo_PlayerSplineTemplate;

	public MilMo_PlayerSpline()
	{
	}

	public MilMo_PlayerSpline(MilMo_PlayerSplineTemplate template)
		: base(template)
	{
	}
}
