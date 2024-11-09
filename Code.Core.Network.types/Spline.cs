using System.Collections.Generic;

namespace Code.Core.Network.types;

public class Spline : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new Spline(reader);
		}
	}

	private readonly IList<Waypoint> _waypoints;

	private const int TYPE_ID = 8;

	public override int GetTypeId()
	{
		return 8;
	}

	public Spline(MessageReader reader)
		: base(reader)
	{
		_waypoints = new List<Waypoint>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_waypoints.Add(new Waypoint(reader));
		}
	}

	public Spline(IList<Waypoint> waypoints, string type, TemplateReference reference)
		: base(type, reference)
	{
		_waypoints = waypoints;
	}

	public IList<Waypoint> GetWaypoints()
	{
		return _waypoints;
	}

	public override int Size()
	{
		return 2 + base.Size() + (short)(_waypoints.Count * 28);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt16((short)_waypoints.Count);
		foreach (Waypoint waypoint in _waypoints)
		{
			waypoint.Write(writer);
		}
	}
}
