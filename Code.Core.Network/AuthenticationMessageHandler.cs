using UnityEngine;

namespace Code.Core.Network;

public class AuthenticationMessageHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		Debug.LogWarning("Handling authentication message in dispatcher. ");
	}
}
