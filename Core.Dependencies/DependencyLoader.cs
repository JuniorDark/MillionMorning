using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Dependencies;

public class DependencyLoader : MonoBehaviour
{
	[SerializeField]
	private List<Dependency> dependencies;

	public event Action<string, float> OnProgress;

	public async Task<bool> LoadDependencies()
	{
		bool allGood = true;
		int total = dependencies.Count;
		int current = 1;
		foreach (Dependency dependency in dependencies)
		{
			string arg = "Loading " + dependency.GetType().Name;
			this.OnProgress?.Invoke(arg, 1f * (float)current / (float)total);
			bool ok = await dependency.Check();
			await Task.Delay(10);
			if (!ok)
			{
				Debug.LogError("Failed to load dependency: dependency!");
				allGood = false;
			}
			current++;
		}
		return allGood;
	}
}
