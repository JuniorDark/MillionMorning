using System.Threading.Tasks;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Core.Dependencies;

public class LocalizationDependency : Dependency
{
	public override async Task<bool> Check()
	{
		if (MilMo_Localization.CurrentLanguage != null)
		{
			return true;
		}
		await MilMo_Localization.AsyncInitializeSystemLanguage();
		if (MilMo_Localization.CurrentLanguage != null)
		{
			return true;
		}
		Debug.LogError("Localization could not be initialized!");
		return false;
	}
}
