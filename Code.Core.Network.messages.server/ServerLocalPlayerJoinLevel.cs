using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerLocalPlayerJoinLevel : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 44;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLocalPlayerJoinLevel(reader);
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

	private const int OPCODE = 44;

	private vector3 playerPosition;

	private float playerRotation;

	private string room;

	private int gemBonusTimeLeftInSeconds;

	private int membershipTimeLeftInDays;

	private ServerLocalPlayerJoinLevel(MessageReader reader)
	{
		playerPosition = new vector3(reader);
		playerRotation = reader.ReadFloat();
		room = reader.ReadString();
		gemBonusTimeLeftInSeconds = reader.ReadInt32();
		membershipTimeLeftInDays = reader.ReadInt32();
	}

	public ServerLocalPlayerJoinLevel(vector3 playerPosition, float playerRotation, string room, int gemBonusTimeLeftInSeconds, int membershipTimeLeftInDays)
	{
		this.playerPosition = playerPosition;
		this.playerRotation = playerRotation;
		this.room = room;
		this.gemBonusTimeLeftInSeconds = gemBonusTimeLeftInSeconds;
		this.membershipTimeLeftInDays = membershipTimeLeftInDays;
	}

	public vector3 getPlayerPosition()
	{
		return playerPosition;
	}

	public float getPlayerRotation()
	{
		return playerRotation;
	}

	public string getRoom()
	{
		return room;
	}

	public int getGemBonusTimeLeftInSeconds()
	{
		return gemBonusTimeLeftInSeconds;
	}

	public int getMembershipTimeLeftInDays()
	{
		return membershipTimeLeftInDays;
	}

	public byte[] GetData()
	{
		int num = 30;
		num += MessageWriter.GetSize(room);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(44);
		messageWriter.WriteInt16((short)(num - 4));
		playerPosition.Write(messageWriter);
		messageWriter.WriteFloat(playerRotation);
		messageWriter.WriteString(room);
		messageWriter.WriteInt32(gemBonusTimeLeftInSeconds);
		messageWriter.WriteInt32(membershipTimeLeftInDays);
		return messageWriter.GetData();
	}
}
