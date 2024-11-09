using System.Threading.Tasks;
using Code.Core.Template;

namespace Core.Dependencies;

public class TemplateContainerDependency : Dependency
{
	public override Task<bool> Check()
	{
		return Task.FromResult(Singleton<MilMo_TemplateContainer>.Instance != null);
	}
}
