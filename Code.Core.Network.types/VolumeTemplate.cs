namespace Code.Core.Network.types;

public class VolumeTemplate
{
	public class Factory
	{
		public virtual VolumeTemplate Create(MessageReader reader)
		{
			return new VolumeTemplate(reader);
		}
	}

	private static readonly Factory[] ChildFactories;

	private const int TYPE_ID = 0;

	static VolumeTemplate()
	{
		ChildFactories = new Factory[5];
		ChildFactories[0] = new Factory();
		ChildFactories[1] = new SphereTemplate.Factory();
		ChildFactories[2] = new BoxTemplate.Factory();
		ChildFactories[3] = new InvertedCircleTemplate.Factory();
		ChildFactories[4] = new InvertedCylinderTemplate.Factory();
	}

	public static VolumeTemplate Create(int id, MessageReader reader)
	{
		return ChildFactories[id].Create(reader);
	}

	public virtual int GetTypeId()
	{
		return 0;
	}

	public VolumeTemplate(MessageReader reader)
	{
	}

	public VolumeTemplate()
	{
	}

	public virtual int Size()
	{
		return 0;
	}

	public virtual void Write(MessageWriter writer)
	{
	}
}
