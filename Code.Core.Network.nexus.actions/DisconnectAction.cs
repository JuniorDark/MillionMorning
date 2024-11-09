namespace Code.Core.Network.nexus.actions;

public class DisconnectAction : IAction
{
	public void Accept(INexusListener listener)
	{
		listener.Disconnected();
	}
}
