using System.Collections.Generic;

namespace Code.Core.Network.types;

public class NullableExplorationTokenList
{
	private readonly IList<ExplorationToken> _tokens;

	public NullableExplorationTokenList(MessageReader reader)
	{
		_tokens = new List<ExplorationToken>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_tokens.Add(new ExplorationToken(reader));
		}
	}

	public NullableExplorationTokenList(IList<ExplorationToken> tokens)
	{
		_tokens = tokens;
	}

	public IList<ExplorationToken> GetTokens()
	{
		return _tokens;
	}

	public int Size()
	{
		return 2 + (short)(_tokens.Count * 14);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt16((short)_tokens.Count);
		foreach (ExplorationToken token in _tokens)
		{
			token.Write(writer);
		}
	}
}
