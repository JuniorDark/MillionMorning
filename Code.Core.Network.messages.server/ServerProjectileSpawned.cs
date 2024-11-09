using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerProjectileSpawned : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 89;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerProjectileSpawned(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 2;
			if (buffer.Remaining() < lengthSize + 2)
			{
				length = 0;
				return false;
			}
			byte[] array = new byte[lengthSize];
			Array.Copy(buffer.Bytes, buffer.Pos + 2, array, 0, lengthSize);
			MessageReader messageReader = new MessageReader(array);
			length = messageReader.ReadInt16();
			return buffer.Remaining() >= length + lengthSize + 2;
		}
	}

	private const int OPCODE = 89;

	private string FullLevelName;

	private int ProjectileId;

	private vector3 Position;

	private vector3 Direction;

	private TemplateReference ProjectileTemplate;

	private ServerProjectileSpawned(MessageReader reader)
	{
		FullLevelName = reader.ReadString();
		ProjectileId = reader.ReadInt32();
		Position = new vector3(reader);
		Direction = new vector3(reader);
		ProjectileTemplate = new TemplateReference(reader);
	}

	public ServerProjectileSpawned(string FullLevelName, int ProjectileId, vector3 Position, vector3 Direction, TemplateReference ProjectileTemplate)
	{
		this.FullLevelName = FullLevelName;
		this.ProjectileId = ProjectileId;
		this.Position = Position;
		this.Direction = Direction;
		this.ProjectileTemplate = ProjectileTemplate;
	}

	public string getFullLevelName()
	{
		return FullLevelName;
	}

	public int getProjectileId()
	{
		return ProjectileId;
	}

	public vector3 getPosition()
	{
		return Position;
	}

	public vector3 getDirection()
	{
		return Direction;
	}

	public TemplateReference getProjectileTemplate()
	{
		return ProjectileTemplate;
	}

	public byte[] GetData()
	{
		int num = 34;
		num += MessageWriter.GetSize(FullLevelName);
		num += ProjectileTemplate.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(89);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(FullLevelName);
		messageWriter.WriteInt32(ProjectileId);
		Position.Write(messageWriter);
		Direction.Write(messageWriter);
		ProjectileTemplate.Write(messageWriter);
		return messageWriter.GetData();
	}
}
