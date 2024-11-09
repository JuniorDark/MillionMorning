using System;
using Code.Core.Network.nexus;
using UnityEngine.Events;

namespace Core.GameEvent.Types.Identity;

[Serializable]
public class UnityIdentityEvent : UnityEvent<IIdentity>
{
}
