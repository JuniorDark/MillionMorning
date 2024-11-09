using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ConditionTalkTo : Condition
{
	public new class Factory : Condition.Factory
	{
		public override Condition Create(MessageReader reader)
		{
			return new ConditionTalkTo(reader);
		}
	}

	private readonly string _npcTemplateIdentifier;

	private readonly string _npcVisualRep;

	private readonly string _npcDisplayName;

	private readonly IList<string> _fullLevelNames;

	private const int TYPE_ID = 9;

	public override int GetTypeId()
	{
		return 9;
	}

	public ConditionTalkTo(MessageReader reader)
		: base(reader)
	{
		_npcTemplateIdentifier = reader.ReadString();
		_npcVisualRep = reader.ReadString();
		_npcDisplayName = reader.ReadString();
		_fullLevelNames = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_fullLevelNames.Add(reader.ReadString());
		}
	}

	public ConditionTalkTo(string npcTemplateIdentifier, string npcVisualRep, string npcDisplayName, IList<string> fullLevelNames, sbyte completed, sbyte active)
		: base(completed, active)
	{
		_npcTemplateIdentifier = npcTemplateIdentifier;
		_npcVisualRep = npcVisualRep;
		_npcDisplayName = npcDisplayName;
		_fullLevelNames = fullLevelNames;
	}

	public string GetNpcTemplateIdentifier()
	{
		return _npcTemplateIdentifier;
	}

	public string GetNpcVisualRep()
	{
		return _npcVisualRep;
	}

	public string GetNpcDisplayName()
	{
		return _npcDisplayName;
	}

	public IList<string> GetFullLevelNames()
	{
		return _fullLevelNames;
	}

	public override int Size()
	{
		int num = 10;
		num += MessageWriter.GetSize(_npcTemplateIdentifier);
		num += MessageWriter.GetSize(_npcVisualRep);
		num += MessageWriter.GetSize(_npcDisplayName);
		num += (short)(2 * _fullLevelNames.Count);
		foreach (string fullLevelName in _fullLevelNames)
		{
			num += MessageWriter.GetSize(fullLevelName);
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_npcTemplateIdentifier);
		writer.WriteString(_npcVisualRep);
		writer.WriteString(_npcDisplayName);
		writer.WriteInt16((short)_fullLevelNames.Count);
		foreach (string fullLevelName in _fullLevelNames)
		{
			writer.WriteString(fullLevelName);
		}
	}
}
