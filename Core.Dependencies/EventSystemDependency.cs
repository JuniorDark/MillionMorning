using System.Threading.Tasks;
using Code.Core.EventSystem;
using UnityEngine;

namespace Core.Dependencies;

public class EventSystemDependency : Dependency
{
	public override Task<bool> Check()
	{
		if (Object.FindObjectOfType<MilMo_EventSystemRunner>() != null)
		{
			return Task.FromResult(result: true);
		}
		Debug.LogError("No EventSystemRunner found in scene!");
		return Task.FromResult(result: false);
	}
}
