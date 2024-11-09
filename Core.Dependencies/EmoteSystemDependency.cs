using System.Threading.Tasks;
using Code.Core.Emote;

namespace Core.Dependencies;

public class EmoteSystemDependency : Dependency
{
	public override Task<bool> Check()
	{
		return Task.FromResult(MilMo_EmoteSystem.Create());
	}
}
