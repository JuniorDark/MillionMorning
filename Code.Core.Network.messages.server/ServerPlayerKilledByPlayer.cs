using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerPlayerKilledByPlayer : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 329;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerKilledByPlayer(reader);
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

	private const int OPCODE = 329;

	private string killedIdentifier;

	private string killerIdentifier;

	private TemplateReference weapon;

	private ServerPlayerKilledByPlayer(MessageReader reader)
	{
		killedIdentifier = reader.ReadString();
		killerIdentifier = reader.ReadString();
		weapon = new TemplateReference(reader);
	}

	public ServerPlayerKilledByPlayer(string killedIdentifier, string killerIdentifier, TemplateReference weapon)
	{
		this.killedIdentifier = killedIdentifier;
		this.killerIdentifier = killerIdentifier;
		this.weapon = weapon;
	}

	public string getKilledIdentifier()
	{
		return killedIdentifier;
	}

	public string getKillerIdentifier()
	{
		return killerIdentifier;
	}

	public TemplateReference getWeapon()
	{
		return weapon;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(killedIdentifier);
		num += MessageWriter.GetSize(killerIdentifier);
		num += weapon.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(329);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(killedIdentifier);
		messageWriter.WriteString(killerIdentifier);
		weapon.Write(messageWriter);
		return messageWriter.GetData();
	}
}
