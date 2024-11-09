namespace Code.Core.Network;

public interface IHandler
{
	void Handle(IMessage message, IZenListener listener);
}
