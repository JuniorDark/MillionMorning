using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual;

public class MilMo_VisualRepContainer
{
	public delegate void VisualRepDone(MilMo_VisualRep visualRep);

	private static readonly List<MilMo_VisualRep> VisualReps = new List<MilMo_VisualRep>();

	private static readonly List<MilMo_VisualRep> UpdateList = new List<MilMo_VisualRep>();

	private static readonly List<MilMo_VisualRep> RemoveFromUpdateList = new List<MilMo_VisualRep>();

	private static readonly List<MilMo_VisualRep> AddToUpdate = new List<MilMo_VisualRep>();

	private static bool _isUpdatingVisualReps;

	private static bool _initialized;

	public static void Initialize()
	{
		if (!_initialized)
		{
			MilMo_EventSystem.RegisterUpdate(UpdateVisualReps);
			_initialized = true;
		}
	}

	public static void AddForUpdate(MilMo_VisualRep visualRep)
	{
		if (_isUpdatingVisualReps)
		{
			AddToUpdate.Add(visualRep);
		}
		else if (!UpdateList.Contains(visualRep))
		{
			UpdateList.Add(visualRep);
		}
	}

	public static void RemoveFromUpdate(MilMo_VisualRep visualRep)
	{
		if (_isUpdatingVisualReps)
		{
			RemoveFromUpdateList.Add(visualRep);
		}
		else
		{
			UpdateList.Remove(visualRep);
		}
	}

	private static void UpdateVisualReps(object o)
	{
		_isUpdatingVisualReps = true;
		foreach (MilMo_VisualRep update in UpdateList)
		{
			update.Update();
		}
		foreach (MilMo_VisualRep item in AddToUpdate.Where((MilMo_VisualRep visualRepToAdd) => !UpdateList.Contains(visualRepToAdd)))
		{
			UpdateList.Add(item);
		}
		AddToUpdate.Clear();
		foreach (MilMo_VisualRep removeFromUpdate in RemoveFromUpdateList)
		{
			UpdateList.Remove(removeFromUpdate);
		}
		RemoveFromUpdateList.Clear();
		_isUpdatingVisualReps = false;
	}

	public static MilMo_VisualRep GetVisualRep(GameObject gameObject)
	{
		if (gameObject == null)
		{
			return null;
		}
		MilMo_VisualRepComponent componentInChildren = gameObject.GetComponentInChildren<MilMo_VisualRepComponent>();
		if (!(componentInChildren == null))
		{
			return componentInChildren.GetVisualRep();
		}
		return null;
	}

	public static MilMo_VisualRepData GetVisualRepData(GameObject gameObject)
	{
		if (gameObject == null)
		{
			return null;
		}
		MilMo_VisualRepComponent componentInChildren = gameObject.GetComponentInChildren<MilMo_VisualRepComponent>();
		if (!(componentInChildren == null))
		{
			return componentInChildren.GetData();
		}
		return null;
	}

	public static void AsyncCreateVisualRep(string fullPath, Vector3 position, Quaternion rotation, VisualRepDone callback)
	{
		AsyncCreateVisualRep(fullPath, null, position, rotation, new Vector3(1f, 1f, 1f), callback);
	}

	public static void AsyncCreateVisualRep(string fullPath, Vector3 position, Quaternion rotation, string tag, VisualRepDone callback)
	{
		AsyncCreateVisualRep(fullPath, null, position, rotation, new Vector3(1f, 1f, 1f), tag, callback);
	}

	public static void AsyncCreateVisualRep(string fullPath, Vector3 position, Quaternion rotation, bool setDefaultMaterialTexture, bool waitForMaterial, VisualRepDone callback)
	{
		AsyncCreateVisualRep(fullPath, null, position, rotation, new Vector3(1f, 1f, 1f), "Generic", setDefaultMaterialTexture, waitForMaterial, callback);
	}

	public static void AsyncCreateVisualRep(string fullPath, MilMo_SFFile overrideFile, Vector3 position, Quaternion rotation, Vector3 scale, VisualRepDone callback)
	{
		AsyncCreateVisualRep(fullPath, overrideFile, position, rotation, scale, "Generic", setDefaultMaterialTexture: true, waitForMaterial: false, callback);
	}

	public static void AsyncCreateVisualRep(string fullPath, MilMo_SFFile overrideFile, Vector3 position, Quaternion rotation, Vector3 scale, string tag, VisualRepDone callback)
	{
		AsyncCreateVisualRep(fullPath, overrideFile, position, rotation, scale, tag, setDefaultMaterialTexture: true, waitForMaterial: false, callback);
	}

	public static void AsyncCreateVisualRep(string fullPath, MilMo_SFFile overrideFile, Vector3 position, Quaternion rotation, Vector3 scale, string tag, MilMo_ResourceManager.Priority priority, VisualRepDone callback)
	{
		AsyncCreateVisualRep(fullPath, overrideFile, position, rotation, scale, tag, setDefaultMaterialTexture: true, waitForMaterial: false, pauseModeOnMaterial: false, priority, callback);
	}

	public static void AsyncCreateVisualRep(string fullPath, MilMo_SFFile overrideFile, Vector3 position, Quaternion rotation, Vector3 scale, string tag, bool materialPauseMode, MilMo_ResourceManager.Priority priority, VisualRepDone callback)
	{
		AsyncCreateVisualRep(fullPath, overrideFile, position, rotation, scale, tag, setDefaultMaterialTexture: true, waitForMaterial: false, materialPauseMode, priority, callback);
	}

	public static void AsyncCreateVisualRep(string fullPath, MilMo_SFFile overrideFile, Vector3 position, Quaternion rotation, Vector3 scale, string tag, bool setDefaultMaterialTexture, bool waitForMaterial, VisualRepDone callback)
	{
		AsyncCreateVisualRep(fullPath, overrideFile, position, rotation, scale, tag, setDefaultMaterialTexture, waitForMaterial, pauseModeOnMaterial: false, MilMo_ResourceManager.Priority.High, callback);
	}

	public static async void AsyncCreateVisualRep(string fullPath, MilMo_SFFile overrideFile, Vector3 position, Quaternion rotation, Vector3 scale, string tag, bool setDefaultMaterialTexture, bool waitForMaterial, bool pauseModeOnMaterial, MilMo_ResourceManager.Priority priority, VisualRepDone callback)
	{
		if (overrideFile != null)
		{
			overrideFile = new MilMo_SFFile(overrideFile);
		}
		MilMo_VisualRep newRep = CreateVisualRep(fullPath, tag, setDefaultMaterialTexture, pauseModeOnMaterial, priority);
		await newRep.LoadAsync();
		if (overrideFile != null)
		{
			newRep.ParseFile(overrideFile);
		}
		MilMo_VisualRep finishedVisualRep = await newRep.InstantiateAsync(position, rotation, scale);
		if (finishedVisualRep == null)
		{
			Debug.LogWarning("Async Instantiate failed for " + fullPath);
			callback(null);
			return;
		}
		if (finishedVisualRep.Renderer == null)
		{
			Debug.LogWarning("Renderer is null for " + fullPath);
		}
		if (waitForMaterial)
		{
			AddForUpdate(finishedVisualRep);
			if (!finishedVisualRep.MaterialsFinished)
			{
				finishedVisualRep.RegisterMaterialsDoneCallback(delegate
				{
					VisualReps.Add(finishedVisualRep);
					if (finishedVisualRep._noPlayerCollisionRadius > 0f)
					{
						MilMo_EventSystem.Instance.PostEvent("no_playercollision_area_created", finishedVisualRep);
					}
					callback(finishedVisualRep);
				});
			}
			else
			{
				VisualReps.Add(finishedVisualRep);
				if (finishedVisualRep._noPlayerCollisionRadius > 0f)
				{
					MilMo_EventSystem.Instance.PostEvent("no_playercollision_area_created", finishedVisualRep);
				}
				callback(finishedVisualRep);
			}
		}
		else
		{
			VisualReps.Add(finishedVisualRep);
			if (finishedVisualRep._noPlayerCollisionRadius > 0f)
			{
				MilMo_EventSystem.Instance.PostEvent("no_playercollision_area_created", finishedVisualRep);
			}
			callback(finishedVisualRep);
		}
	}

	public static MilMo_VisualRep CreateVisualRep(string fullPath, Vector3 position, Quaternion rotation, MilMo_SFFile overrideFile = null, bool setTextureInDefaultMaterial = true)
	{
		MilMo_VisualRep milMo_VisualRep = CreateVisualRep(fullPath, "Generic", setTextureInDefaultMaterial);
		milMo_VisualRep.Load();
		if (overrideFile != null)
		{
			milMo_VisualRep.ParseFile(overrideFile);
		}
		if (!milMo_VisualRep.Instantiate(position, rotation))
		{
			Debug.LogWarning("Failed to instantiate visual rep " + fullPath);
			return null;
		}
		VisualReps.Add(milMo_VisualRep);
		if (milMo_VisualRep._noPlayerCollisionRadius > 0f)
		{
			MilMo_EventSystem.Instance.PostEvent("no_playercollision_area_created", milMo_VisualRep);
		}
		return milMo_VisualRep;
	}

	public static MilMo_VisualRep CreateVisualRep(string fullPath, string assetTag = "Generic", bool setTextureInDefaultMaterial = true, bool pauseModeOnMaterial = false, MilMo_ResourceManager.Priority priority = MilMo_ResourceManager.Priority.High)
	{
		MilMo_VisualRep milMo_VisualRep = new MilMo_VisualRep();
		milMo_VisualRep.Init(fullPath, assetTag, setTextureInDefaultMaterial, pauseModeOnMaterial, priority);
		return milMo_VisualRep;
	}

	public static void RemoveFromList(MilMo_VisualRep visualRep)
	{
		VisualReps.Remove(visualRep);
	}

	public static void DestroyVisualRep(MilMo_VisualRep visualRep)
	{
		visualRep?.Destroy();
	}

	public static string Debug_Count(string[] args)
	{
		return VisualReps.Count + " visual reps loaded";
	}
}
