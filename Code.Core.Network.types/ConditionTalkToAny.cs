using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ConditionTalkToAny : Condition
{
	public new class Factory : Condition.Factory
	{
		public override Condition Create(MessageReader reader)
		{
			return new ConditionTalkToAny(reader);
		}
	}

	private readonly short _numberToTalkTo;

	private readonly IList<TalkToInfo> _npcs;

	private const int TYPE_ID = 10;

	public override int GetTypeId()
	{
		return 10;
	}

	public ConditionTalkToAny(MessageReader reader)
		: base(reader)
	{
		_numberToTalkTo = reader.ReadInt16();
		_npcs = new List<TalkToInfo>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_npcs.Add(new TalkToInfo(reader));
		}
	}

	public ConditionTalkToAny(short numberToTalkTo, IList<TalkToInfo> npcs, sbyte completed, sbyte active)
		: base(completed, active)
	{
		_numberToTalkTo = numberToTalkTo;
		_npcs = npcs;
	}

	public short GetNumberToTalkTo()
	{
		return _numberToTalkTo;
	}

	public IList<TalkToInfo> GetNPCS()
	{
		return _npcs;
	}

	public override int Size()
	{
		int num = 6;
		foreach (TalkToInfo npc in _npcs)
		{
			num += npc.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt16(_numberToTalkTo);
		writer.WriteInt16((short)_npcs.Count);
		foreach (TalkToInfo npc in _npcs)
		{
			npc.Write(writer);
		}
	}
}
