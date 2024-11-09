using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerActivatePlayerState : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 110;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerActivatePlayerState(reader);
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

	private const int OPCODE = 110;

	private string playerId;

	private TemplateReference stateTemplate;

	private ExposedVariableUpdate variableUpdate;

	private ServerActivatePlayerState(MessageReader reader)
	{
		playerId = reader.ReadString();
		stateTemplate = new TemplateReference(reader);
		if (reader.ReadInt8() == 1)
		{
			variableUpdate = new ExposedVariableUpdate(reader);
		}
	}

	public ServerActivatePlayerState(string playerId, TemplateReference stateTemplate, ExposedVariableUpdate variableUpdate)
	{
		this.playerId = playerId;
		this.stateTemplate = stateTemplate;
		this.variableUpdate = variableUpdate;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public TemplateReference getStateTemplate()
	{
		return stateTemplate;
	}

	public ExposedVariableUpdate getVariableUpdate()
	{
		return variableUpdate;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += MessageWriter.GetSize(playerId);
		num += stateTemplate.Size();
		if (variableUpdate != null)
		{
			num += variableUpdate.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(110);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		stateTemplate.Write(messageWriter);
		if (variableUpdate == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			variableUpdate.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
