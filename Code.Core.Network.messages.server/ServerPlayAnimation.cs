using System;

namespace Code.Core.Network.messages.server;

public class ServerPlayAnimation : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 366;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayAnimation(reader);
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

	private const int OPCODE = 366;

	private string playerId;

	private string animation;

	private string particles;

	private sbyte isStaticParticleEffect;

	private ServerPlayAnimation(MessageReader reader)
	{
		playerId = reader.ReadString();
		animation = reader.ReadString();
		particles = reader.ReadString();
		isStaticParticleEffect = reader.ReadInt8();
	}

	public ServerPlayAnimation(string playerId, string animation, string particles, sbyte isStaticParticleEffect)
	{
		this.playerId = playerId;
		this.animation = animation;
		this.particles = particles;
		this.isStaticParticleEffect = isStaticParticleEffect;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public string getAnimation()
	{
		return animation;
	}

	public string getParticles()
	{
		return particles;
	}

	public sbyte getIsStaticParticleEffect()
	{
		return isStaticParticleEffect;
	}

	public byte[] GetData()
	{
		int num = 11;
		num += MessageWriter.GetSize(playerId);
		num += MessageWriter.GetSize(animation);
		num += MessageWriter.GetSize(particles);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(366);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteString(animation);
		messageWriter.WriteString(particles);
		messageWriter.WriteInt8(isStaticParticleEffect);
		return messageWriter.GetData();
	}
}
