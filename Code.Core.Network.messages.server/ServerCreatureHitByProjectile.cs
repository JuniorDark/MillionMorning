namespace Code.Core.Network.messages.server;

public class ServerCreatureHitByProjectile : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 327;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerCreatureHitByProjectile(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 12;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 327;

	private int creatureId;

	private int projectileId;

	private float newHealth;

	private ServerCreatureHitByProjectile(MessageReader reader)
	{
		creatureId = reader.ReadInt32();
		projectileId = reader.ReadInt32();
		newHealth = reader.ReadFloat();
	}

	public ServerCreatureHitByProjectile(int creatureId, int projectileId, float newHealth)
	{
		this.creatureId = creatureId;
		this.projectileId = projectileId;
		this.newHealth = newHealth;
	}

	public int getCreatureId()
	{
		return creatureId;
	}

	public int getProjectileId()
	{
		return projectileId;
	}

	public float getNewHealth()
	{
		return newHealth;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(14);
		messageWriter.WriteOpCode(327);
		messageWriter.WriteInt32(creatureId);
		messageWriter.WriteInt32(projectileId);
		messageWriter.WriteFloat(newHealth);
		return messageWriter.GetData();
	}
}
