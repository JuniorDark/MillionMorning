namespace Code.Core.Network.types;

public class ActionObjectEffect : FurnitureStateAction
{
	public new class Factory : FurnitureStateAction.Factory
	{
		public override FurnitureStateAction Create(MessageReader reader)
		{
			return new ActionObjectEffect(reader);
		}
	}

	private readonly string _objectEffectName;

	private const int TYPE_ID = 1;

	public override int GetTypeId()
	{
		return 1;
	}

	public ActionObjectEffect(MessageReader reader)
		: base(reader)
	{
		_objectEffectName = reader.ReadString();
	}

	public ActionObjectEffect(string objectEffectName, string type)
		: base(type)
	{
		_objectEffectName = objectEffectName;
	}

	public string GetObjectEffectName()
	{
		return _objectEffectName;
	}

	public override int Size()
	{
		return 2 + base.Size() + MessageWriter.GetSize(_objectEffectName);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_objectEffectName);
	}
}
