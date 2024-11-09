namespace Code.Core.Network.types;

public class Teleport : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new Teleport(reader);
		}
	}

	private readonly string _arriveSound;

	private const int TYPE_ID = 11;

	public override int GetTypeId()
	{
		return 11;
	}

	public Teleport(MessageReader reader)
		: base(reader)
	{
		_arriveSound = reader.ReadString();
	}

	public Teleport(string arriveSound, string type, TemplateReference reference)
		: base(type, reference)
	{
		_arriveSound = arriveSound;
	}

	public string GetArriveSound()
	{
		return _arriveSound;
	}

	public override int Size()
	{
		return 2 + base.Size() + MessageWriter.GetSize(_arriveSound);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_arriveSound);
	}
}
