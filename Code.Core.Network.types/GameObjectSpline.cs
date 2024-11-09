using System.Collections.Generic;

namespace Code.Core.Network.types;

public class GameObjectSpline : Spline
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new GameObjectSpline(reader);
		}
	}

	private readonly IList<SoundPoint> _soundPoints;

	private readonly IList<AnimationPoint> _animationPoints;

	private readonly string _backgroundSound;

	private const int TYPE_ID = 78;

	public override int GetTypeId()
	{
		return 78;
	}

	public GameObjectSpline(MessageReader reader)
		: base(reader)
	{
		_soundPoints = new List<SoundPoint>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_soundPoints.Add(new SoundPoint(reader));
		}
		_animationPoints = new List<AnimationPoint>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_animationPoints.Add(new AnimationPoint(reader));
		}
		_backgroundSound = reader.ReadString();
	}

	public GameObjectSpline(IList<SoundPoint> soundPoints, IList<AnimationPoint> animationPoints, string backgroundSound, IList<Waypoint> waypoints, string type, TemplateReference reference)
		: base(waypoints, type, reference)
	{
		_soundPoints = soundPoints;
		_animationPoints = animationPoints;
		_backgroundSound = backgroundSound;
	}

	public IList<SoundPoint> GetSoundPoints()
	{
		return _soundPoints;
	}

	public IList<AnimationPoint> GetAnimationPoints()
	{
		return _animationPoints;
	}

	public string GetBackgroundSound()
	{
		return _backgroundSound;
	}

	public override int Size()
	{
		int num = 6 + base.Size();
		foreach (SoundPoint soundPoint in _soundPoints)
		{
			num += soundPoint.Size();
		}
		foreach (AnimationPoint animationPoint in _animationPoints)
		{
			num += animationPoint.Size();
		}
		return num + MessageWriter.GetSize(_backgroundSound);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt16((short)_soundPoints.Count);
		foreach (SoundPoint soundPoint in _soundPoints)
		{
			soundPoint.Write(writer);
		}
		writer.WriteInt16((short)_animationPoints.Count);
		foreach (AnimationPoint animationPoint in _animationPoints)
		{
			animationPoint.Write(writer);
		}
		writer.WriteString(_backgroundSound);
	}
}
