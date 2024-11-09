using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ConditionKilledAny : Condition
{
	public new class Factory : Condition.Factory
	{
		public override Condition Create(MessageReader reader)
		{
			return new ConditionKilledAny(reader);
		}
	}

	private readonly int _numberToKill;

	private readonly IList<KilledInfo> _kills;

	private const int TYPE_ID = 5;

	public override int GetTypeId()
	{
		return 5;
	}

	public ConditionKilledAny(MessageReader reader)
		: base(reader)
	{
		_numberToKill = reader.ReadInt32();
		_kills = new List<KilledInfo>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_kills.Add(new KilledInfo(reader));
		}
	}

	public ConditionKilledAny(int numberToKill, IList<KilledInfo> kills, sbyte completed, sbyte active)
		: base(completed, active)
	{
		_numberToKill = numberToKill;
		_kills = kills;
	}

	public int GetNumberToKill()
	{
		return _numberToKill;
	}

	public IList<KilledInfo> GetKills()
	{
		return _kills;
	}

	public override int Size()
	{
		int num = 8;
		foreach (KilledInfo kill in _kills)
		{
			num += kill.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt32(_numberToKill);
		writer.WriteInt16((short)_kills.Count);
		foreach (KilledInfo kill in _kills)
		{
			kill.Write(writer);
		}
	}
}
