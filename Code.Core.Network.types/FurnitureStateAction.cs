namespace Code.Core.Network.types;

public class FurnitureStateAction
{
	public class Factory
	{
		public virtual FurnitureStateAction Create(MessageReader reader)
		{
			return new FurnitureStateAction(reader);
		}
	}

	private static readonly Factory[] ChildFactories;

	private readonly string _type;

	private const int TYPE_ID = 0;

	static FurnitureStateAction()
	{
		ChildFactories = new Factory[2];
		ChildFactories[0] = new Factory();
		ChildFactories[1] = new ActionObjectEffect.Factory();
	}

	public static FurnitureStateAction Create(int id, MessageReader reader)
	{
		return ChildFactories[id].Create(reader);
	}

	public virtual int GetTypeId()
	{
		return 0;
	}

	public FurnitureStateAction(MessageReader reader)
	{
		_type = reader.ReadString();
	}

	public FurnitureStateAction(string type)
	{
		_type = type;
	}

	public string GetTemplateType()
	{
		return _type;
	}

	public virtual int Size()
	{
		return 2 + MessageWriter.GetSize(_type);
	}

	public virtual void Write(MessageWriter writer)
	{
		writer.WriteString(_type);
	}
}
