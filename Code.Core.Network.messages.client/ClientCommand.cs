namespace Code.Core.Network.messages.client;

public class ClientCommand : IMessage
{
	private const int OPCODE = 1;

	private readonly string command;

	public ClientCommand(string command)
	{
		this.command = command;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(command);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(1);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(command);
		return messageWriter.GetData();
	}
}
