using System.Threading.Tasks;
using Code.Core.BodyPack;

namespace Core.Dependencies;

public class BodyPackSystemDependency : Dependency
{
	public override async Task<bool> Check()
	{
		if (MilMo_BodyPackSystem.AllDone)
		{
			return true;
		}
		MilMo_BodyPackSystem.CreateGeneric();
		await Task.Delay(10);
		MilMo_BodyPackSystem.CreateForBoy();
		await Task.Delay(10);
		MilMo_BodyPackSystem.CreateForGirl();
		await Task.Delay(10);
		return true;
	}
}
