using Code.Core.Network.nexus;

namespace Core.GameEvent.Types.Identity;

public class IdentityEventReference : EventReference<IIdentity, IdentityEvent>
{
	public IdentityEventReference(string addressableKey)
		: base(addressableKey)
	{
	}
}
