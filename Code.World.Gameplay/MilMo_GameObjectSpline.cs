using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.Spline;
using UnityEngine;

namespace Code.World.Gameplay;

public class MilMo_GameObjectSpline : MilMo_Spline
{
	private class MilMo_AnimationPoint
	{
		public readonly string Animation;

		public readonly byte AlignAxis;

		public readonly float Time;

		public MilMo_AnimationPoint(string animation, byte alignAxis, float time)
		{
			Animation = animation;
			AlignAxis = alignAxis;
			Time = time;
		}
	}

	private class MilMo_SoundPoint
	{
		public readonly string Sound;

		public readonly float Time;

		public MilMo_SoundPoint(string sound, float time)
		{
			Sound = sound;
			Time = time;
		}
	}

	public new class SplinePoint : MilMo_Spline.SplinePoint
	{
		public const byte NO_AXIS = 0;

		public const byte X_AXIS = 1;

		public const byte Y_AXIS = 2;

		public const byte Z_AXIS = 3;

		public string Sound = "";

		public string Animation = "";

		public byte AlignAxis;

		public SplinePoint(MilMo_Spline.SplinePoint p)
		{
			Position = p.Position;
			Normal = p.Normal;
			Tangent = p.Tangent;
			Binormal = p.Binormal;
		}
	}

	private readonly List<MilMo_AnimationPoint> _animationPoints = new List<MilMo_AnimationPoint>();

	private readonly List<MilMo_SoundPoint> _soundPoints = new List<MilMo_SoundPoint>();

	public new MilMo_GameObjectSplineTemplate Template => base.Template as MilMo_GameObjectSplineTemplate;

	protected MilMo_GameObjectSpline()
	{
	}

	private MilMo_GameObjectSpline(MilMo_Spline s)
	{
		base.Waypoints = s.Waypoints;
	}

	public MilMo_GameObjectSpline(MilMo_GameObjectSplineTemplate template)
		: base(template)
	{
		foreach (SoundPoint soundPoint in template.SoundPoints)
		{
			_soundPoints.Add(new MilMo_SoundPoint(soundPoint.GetPath(), soundPoint.GetTime()));
		}
		foreach (AnimationPoint animationPoint in template.AnimationPoints)
		{
			_animationPoints.Add(new MilMo_AnimationPoint(animationPoint.GetAnimation(), (byte)animationPoint.GetAlignAxis(), animationPoint.GetTime()));
		}
	}

	public new SplinePoint GetPointAtTime(float time)
	{
		SplinePoint splinePoint = new SplinePoint(base.GetPointAtTime(time));
		time = Mathf.Min(Mathf.Max(base.Waypoints[0].Time, time), base.Waypoints[base.Waypoints.Count - 1].Time);
		for (int num = _animationPoints.Count - 1; num >= 0; num--)
		{
			if (time >= _animationPoints[num].Time)
			{
				splinePoint.Animation = _animationPoints[num].Animation;
				splinePoint.AlignAxis = _animationPoints[num].AlignAxis;
				break;
			}
		}
		for (int num2 = _soundPoints.Count - 1; num2 >= 0; num2--)
		{
			if (time >= _soundPoints[num2].Time)
			{
				splinePoint.Sound = _soundPoints[num2].Sound;
				break;
			}
		}
		return splinePoint;
	}

	public new MilMo_GameObjectSpline GetInvertedSpline()
	{
		MilMo_GameObjectSpline milMo_GameObjectSpline = new MilMo_GameObjectSpline(base.GetInvertedSpline());
		float time = base.Waypoints[base.Waypoints.Count - 1].Time;
		for (int num = _animationPoints.Count - 1; num >= 0; num--)
		{
			milMo_GameObjectSpline._animationPoints.Add(new MilMo_AnimationPoint(_animationPoints[num].Animation, _animationPoints[num].AlignAxis, time - _animationPoints[num].Time));
		}
		for (int num2 = _soundPoints.Count - 1; num2 >= 0; num2--)
		{
			milMo_GameObjectSpline._soundPoints.Add(new MilMo_SoundPoint(_soundPoints[num2].Sound, time - _soundPoints[num2].Time));
		}
		return milMo_GameObjectSpline;
	}
}
