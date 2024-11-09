using Core.GameEvent.Types.Base;
using UnityEngine;

namespace Core.GameEvent.Types.Void;

[CreateAssetMenu(menuName = "GameEvents/Void Event")]
public class VoidEvent : BaseGameEvent<VoidType>
{
	public void Raise()
	{
		Raise(default(VoidType));
	}
}
