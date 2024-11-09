namespace Code.Core.Network.types;

public class GameplayTriggerReaction
{
	private readonly string _type;

	private readonly string _name;

	public GameplayTriggerReaction(MessageReader reader)
	{
		_type = reader.ReadString();
		_name = reader.ReadString();
	}

	public GameplayTriggerReaction(string type, string name)
	{
		_type = type;
		_name = name;
	}

	public string GetTemplateType()
	{
		return _type;
	}

	public string GetName()
	{
		return _name;
	}

	public int Size()
	{
		return 4 + MessageWriter.GetSize(_type) + MessageWriter.GetSize(_name);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteString(_type);
		writer.WriteString(_name);
	}
}
