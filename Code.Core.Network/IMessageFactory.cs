namespace Code.Core.Network;

public interface IMessageFactory
{
	int GetOpCode();

	IMessage CreateMessage(MessageReader reader);

	bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize);
}
