using System.Collections.Generic;

namespace Code.Core.Network.types;

public class PlayerSpline : GameObjectSpline
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new PlayerSpline(reader);
		}
	}

	private readonly float _fov;

	private readonly float _cameraDistance;

	private const int TYPE_ID = 121;

	public override int GetTypeId()
	{
		return 121;
	}

	public PlayerSpline(MessageReader reader)
		: base(reader)
	{
		_fov = reader.ReadFloat();
		_cameraDistance = reader.ReadFloat();
	}

	public PlayerSpline(float fov, float cameraDistance, IList<SoundPoint> soundPoints, IList<AnimationPoint> animationPoints, string backgroundSound, IList<Waypoint> waypoints, string type, TemplateReference reference)
		: base(soundPoints, animationPoints, backgroundSound, waypoints, type, reference)
	{
		_fov = fov;
		_cameraDistance = cameraDistance;
	}

	public float GetFov()
	{
		return _fov;
	}

	public float GetCameraDistance()
	{
		return _cameraDistance;
	}

	public override int Size()
	{
		return 8 + base.Size();
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteFloat(_fov);
		writer.WriteFloat(_cameraDistance);
	}
}
