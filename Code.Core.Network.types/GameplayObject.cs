using System.Collections.Generic;

namespace Code.Core.Network.types;

public class GameplayObject : LevelObject
{
	public new class Factory : LevelObject.Factory
	{
		public override LevelObject Create(MessageReader reader)
		{
			return new GameplayObject(reader);
		}
	}

	private readonly TemplateReference _template;

	private readonly vector3 _scale;

	private readonly TemplateReference _moverSpline;

	private readonly float _moverTime;

	private readonly string _room;

	private readonly IList<float> _timesSinceLastTriggerActivations;

	private readonly sbyte _isCaptured;

	private readonly string _capturerPlayerId;

	private readonly sbyte _isZoneCaptured;

	private readonly int _zoneCapturerTeamId;

	private readonly vector3 _targetScale;

	private readonly long _shrinkEndTimeStamp;

	private const int TYPE_ID = 4;

	public override int GetTypeId()
	{
		return 4;
	}

	public GameplayObject(MessageReader reader)
		: base(reader)
	{
		_template = new TemplateReference(reader);
		_scale = new vector3(reader);
		if (reader.ReadInt8() == 1)
		{
			_moverSpline = new TemplateReference(reader);
		}
		_moverTime = reader.ReadFloat();
		_room = reader.ReadString();
		_timesSinceLastTriggerActivations = new List<float>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_timesSinceLastTriggerActivations.Add(reader.ReadFloat());
		}
		_isCaptured = reader.ReadInt8();
		_capturerPlayerId = reader.ReadString();
		_isZoneCaptured = reader.ReadInt8();
		_zoneCapturerTeamId = reader.ReadInt32();
		_targetScale = new vector3(reader);
		_shrinkEndTimeStamp = reader.ReadInt64();
	}

	public GameplayObject(TemplateReference template, vector3 scale, TemplateReference moverSpline, float moverTime, string room, IList<float> timesSinceLastTriggerActivations, bool isCaptured, string capturerPlayerId, bool isZoneCaptured, int zoneCapturerTeamId, vector3 targetScale, long shrinkEndTimeStamp, string type, int id, string fullLevelName, string visualRep, vector3 position, vector3 rotation, IList<string> spawnTypes, IList<string> removalEffects, float lifespan)
		: base(type, id, fullLevelName, visualRep, position, rotation, spawnTypes, removalEffects, lifespan)
	{
		_template = template;
		_scale = scale;
		_moverSpline = moverSpline;
		_moverTime = moverTime;
		_room = room;
		_timesSinceLastTriggerActivations = timesSinceLastTriggerActivations;
		_isCaptured = (sbyte)(isCaptured ? 1 : 0);
		_capturerPlayerId = capturerPlayerId;
		_isZoneCaptured = (sbyte)(isZoneCaptured ? 1 : 0);
		_zoneCapturerTeamId = zoneCapturerTeamId;
		_targetScale = targetScale;
		_shrinkEndTimeStamp = shrinkEndTimeStamp;
		_targetScale = targetScale;
		_shrinkEndTimeStamp = shrinkEndTimeStamp;
	}

	public TemplateReference GetTemplate()
	{
		return _template;
	}

	public vector3 GetScale()
	{
		return _scale;
	}

	public TemplateReference GetMoverSpline()
	{
		return _moverSpline;
	}

	public float GetMoverTime()
	{
		return _moverTime;
	}

	public string GetRoom()
	{
		return _room;
	}

	public IList<float> GetTimesSinceLastTriggerActivations()
	{
		return _timesSinceLastTriggerActivations;
	}

	public bool IsCaptured()
	{
		return _isCaptured == 1;
	}

	public string GetCapturerPlayerId()
	{
		return _capturerPlayerId;
	}

	public bool IsZoneCaptured()
	{
		return _isZoneCaptured == 1;
	}

	public int GetZoneCapturerTeamId()
	{
		return _zoneCapturerTeamId;
	}

	public vector3 GetTargetScale()
	{
		return _targetScale;
	}

	public long GetShrinkEndTimeStamp()
	{
		return _shrinkEndTimeStamp;
	}

	public override int Size()
	{
		int num = 37 + base.Size();
		num += _template.Size();
		if (_moverSpline != null)
		{
			num += _moverSpline.Size();
		}
		num += MessageWriter.GetSize(_room);
		num += (short)(_timesSinceLastTriggerActivations.Count * 4);
		num += MessageWriter.GetSize(_capturerPlayerId);
		return num + _targetScale.Size();
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_template.Write(writer);
		_scale.Write(writer);
		if (_moverSpline == null)
		{
			writer.WriteInt8(0);
		}
		else
		{
			writer.WriteInt8(1);
			_moverSpline.Write(writer);
		}
		writer.WriteFloat(_moverTime);
		writer.WriteString(_room);
		writer.WriteInt16((short)_timesSinceLastTriggerActivations.Count);
		foreach (float timesSinceLastTriggerActivation in _timesSinceLastTriggerActivations)
		{
			writer.WriteFloat(timesSinceLastTriggerActivation);
		}
		writer.WriteInt8(_isCaptured);
		writer.WriteString(_capturerPlayerId);
		writer.WriteInt8(_isZoneCaptured);
		writer.WriteInt32(_zoneCapturerTeamId);
		_targetScale.Write(writer);
		writer.WriteInt64(_shrinkEndTimeStamp);
	}
}
