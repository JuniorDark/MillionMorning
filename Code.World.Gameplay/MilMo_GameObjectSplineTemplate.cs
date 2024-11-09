using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.Spline;

namespace Code.World.Gameplay;

public class MilMo_GameObjectSplineTemplate : MilMo_SplineTemplate
{
	public List<AnimationPoint> AnimationPoints { get; private set; }

	public List<SoundPoint> SoundPoints { get; private set; }

	public string BackgroundSound { get; private set; }

	protected MilMo_GameObjectSplineTemplate(string category, string path, string filePath, string type)
		: base(category, path, filePath, type)
	{
	}

	public override bool LoadFromNetwork(Template t)
	{
		base.LoadFromNetwork(t);
		GameObjectSpline gameObjectSpline = (GameObjectSpline)t;
		AnimationPoints = (List<AnimationPoint>)gameObjectSpline.GetAnimationPoints();
		SoundPoints = (List<SoundPoint>)gameObjectSpline.GetSoundPoints();
		BackgroundSound = gameObjectSpline.GetBackgroundSound();
		return true;
	}

	public new static MilMo_GameObjectSplineTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_GameObjectSplineTemplate(category, path, filePath, "GameObjectSpline");
	}
}
