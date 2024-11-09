using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ConditionArrivesAtAny : Condition
{
	public new class Factory : Condition.Factory
	{
		public override Condition Create(MessageReader reader)
		{
			return new ConditionArrivesAtAny(reader);
		}
	}

	private readonly int _numberToVisit;

	private readonly IList<LevelAreaInfo> _aeras;

	private const int TYPE_ID = 2;

	public override int GetTypeId()
	{
		return 2;
	}

	public ConditionArrivesAtAny(MessageReader reader)
		: base(reader)
	{
		_numberToVisit = reader.ReadInt32();
		_aeras = new List<LevelAreaInfo>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_aeras.Add(new LevelAreaInfo(reader));
		}
	}

	public ConditionArrivesAtAny(int numberToVisit, IList<LevelAreaInfo> aeras, sbyte completed, sbyte active)
		: base(completed, active)
	{
		_numberToVisit = numberToVisit;
		_aeras = aeras;
	}

	public int GetNumberToVisit()
	{
		return _numberToVisit;
	}

	public IList<LevelAreaInfo> GetAeras()
	{
		return _aeras;
	}

	public override int Size()
	{
		int num = 8;
		foreach (LevelAreaInfo aera in _aeras)
		{
			num += aera.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt32(_numberToVisit);
		writer.WriteInt16((short)_aeras.Count);
		foreach (LevelAreaInfo aera in _aeras)
		{
			aera.Write(writer);
		}
	}
}
