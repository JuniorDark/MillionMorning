using System.Threading.Tasks;
using Code.Core.Visual;

namespace Core.Dependencies;

public class VisualRepDependency : Dependency
{
	public override Task<bool> Check()
	{
		MilMo_VisualRepContainer.Initialize();
		return Task.FromResult(result: true);
	}
}
