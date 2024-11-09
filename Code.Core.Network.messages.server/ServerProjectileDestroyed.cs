namespace Code.Core.Network.messages.server;

public class ServerProjectileDestroyed : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 330;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerProjectileDestroyed(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 330;

	private int ProjectileId;

	private ServerProjectileDestroyed(MessageReader reader)
	{
		ProjectileId = reader.ReadInt32();
	}

	public ServerProjectileDestroyed(int ProjectileId)
	{
		this.ProjectileId = ProjectileId;
	}

	public int getProjectileId()
	{
		return ProjectileId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(330);
		messageWriter.WriteInt32(ProjectileId);
		return messageWriter.GetData();
	}
}
