using System.Threading.Tasks;
using Code.Core.ResourceSystem;

namespace Core.Dependencies;

public class BadWordFilterDependency : Dependency
{
	public override async Task<bool> Check()
	{
		await MilMo_BadWordFilter.Init();
		return true;
	}
}
