using System.Collections.Generic;

namespace Code.Core.Network.types;

public class PlayerStateEffectCosmetic
{
	private readonly string _type;

	private readonly IList<string> _parameters;

	public PlayerStateEffectCosmetic(MessageReader reader)
	{
		_type = reader.ReadString();
		_parameters = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_parameters.Add(reader.ReadString());
		}
	}

	public PlayerStateEffectCosmetic(string type, IList<string> parameters)
	{
		_type = type;
		_parameters = parameters;
	}

	public string GetTemplateType()
	{
		return _type;
	}

	public IList<string> GetParameters()
	{
		return _parameters;
	}

	public int Size()
	{
		int num = 4;
		num += MessageWriter.GetSize(_type);
		num += (short)(2 * _parameters.Count);
		foreach (string parameter in _parameters)
		{
			num += MessageWriter.GetSize(parameter);
		}
		return num;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_type);
		writer.WriteInt16((short)_parameters.Count);
		foreach (string parameter in _parameters)
		{
			writer.WriteString(parameter);
		}
	}
}
