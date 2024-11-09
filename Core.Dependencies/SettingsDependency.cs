using System.Threading.Tasks;
using Core.Settings;

namespace Core.Dependencies;

public class SettingsDependency : Dependency
{
	public override Task<bool> Check()
	{
		Core.Settings.Settings.Init();
		return Task.FromResult(result: true);
	}
}
