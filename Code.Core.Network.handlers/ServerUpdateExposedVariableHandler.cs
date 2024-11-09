using Code.Core.EventSystem;
using Code.Core.Network.messages.server;

namespace Code.Core.Network.handlers;

public class ServerUpdateExposedVariableHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		ServerUpdateExposedVariable serverUpdateExposedVariable = (ServerUpdateExposedVariable)message;
		MilMo_EventSystem.Instance.AsyncPostEvent("update_exposed_variable", serverUpdateExposedVariable.getVariableUpdate());
	}
}
