namespace Code.Core.Network.types;

public class InterTeleport : Teleport
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new InterTeleport(reader);
		}
	}

	private const int TYPE_ID = 80;

	public override int GetTypeId()
	{
		return 80;
	}

	public InterTeleport(MessageReader reader)
		: base(reader)
	{
	}

	public InterTeleport(string arriveSound, string type, TemplateReference reference)
		: base(arriveSound, type, reference)
	{
	}

	public override int Size()
	{
		return base.Size();
	}
}
