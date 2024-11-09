using System.Collections.Generic;

namespace Code.Core.Network.types;

public class TalkToInfo
{
	private readonly string _npcTemplateIdentifier;

	private readonly string _npcVisualRep;

	private readonly string _npcDisplayName;

	private readonly IList<string> _fullLevelNames;

	private readonly sbyte _talkedTo;

	public TalkToInfo(MessageReader reader)
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
		_talkedTo = reader.ReadInt8();
	}

	public TalkToInfo(string npcTemplateIdentifier, string npcVisualRep, string npcDisplayName, IList<string> fullLevelNames, sbyte talkedTo)
	{
		_npcTemplateIdentifier = npcTemplateIdentifier;
		_npcVisualRep = npcVisualRep;
		_npcDisplayName = npcDisplayName;
		_fullLevelNames = fullLevelNames;
		_talkedTo = talkedTo;
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

	public sbyte GetTalkedTo()
	{
		return _talkedTo;
	}

	public int Size()
	{
		int num = 9;
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

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_npcTemplateIdentifier);
		writer.WriteString(_npcVisualRep);
		writer.WriteString(_npcDisplayName);
		writer.WriteInt16((short)_fullLevelNames.Count);
		foreach (string fullLevelName in _fullLevelNames)
		{
			writer.WriteString(fullLevelName);
		}
		writer.WriteInt8(_talkedTo);
	}
}
