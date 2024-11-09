using System.Collections.Generic;

namespace Code.Core.Network.types;

public class LevelExplorationTokens
{
	private readonly string _level;

	private readonly IList<ExplorationToken> _tokens;

	public LevelExplorationTokens(MessageReader reader)
	{
		_level = reader.ReadString();
		_tokens = new List<ExplorationToken>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_tokens.Add(new ExplorationToken(reader));
		}
	}

	public LevelExplorationTokens(string level, IList<ExplorationToken> tokens)
	{
		_level = level;
		_tokens = tokens;
	}

	public string GetLevel()
	{
		return _level;
	}

	public IList<ExplorationToken> GetTokens()
	{
		return _tokens;
	}

	public int Size()
	{
		return 4 + MessageWriter.GetSize(_level) + (short)(_tokens.Count * 14);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_level);
		writer.WriteInt16((short)_tokens.Count);
		foreach (ExplorationToken token in _tokens)
		{
			token.Write(writer);
		}
	}
}
