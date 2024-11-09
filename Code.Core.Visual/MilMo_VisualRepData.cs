using System;
using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual;

[Serializable]
public class MilMo_VisualRepData
{
	public enum LoadPriority
	{
		High,
		Medium,
		Low,
		Auto
	}

	public Color defaultColor;

	public string groundMaterial;

	public string[] particleEffects;

	public Vector3[] particleOffsets;

	public float[] lodDistances;

	public string[] animations;

	public float[] animationSpeeds;

	public string[] animationSounds;

	public float[] animationSoundTilings;

	public Vector3 spin;

	public bool billboard;

	public bool onlyLodsUseBillboard;

	public bool blocker;

	public bool silhouette;

	public LoadPriority loadPriority = LoadPriority.Auto;

	public Vector2 uvAnimation = Vector2.zero;

	public bool walkable;

	public string walkableName;

	public int walkableResolution = 32;

	public bool critterBlocker;

	public bool treatAsTerrainForJump;

	public bool noCameraCollision;

	public Texture2D debugTex;

	public float noPlayerCollisionRadius;

	public bool receiveShadows;

	public bool castShadows;

	public string eventTag = "";

	public MilMo_ResourceManager.Priority GetPriority()
	{
		return loadPriority switch
		{
			LoadPriority.High => MilMo_ResourceManager.Priority.High, 
			LoadPriority.Medium => MilMo_ResourceManager.Priority.Medium, 
			LoadPriority.Low => MilMo_ResourceManager.Priority.Low, 
			_ => MilMo_ResourceManager.Priority.High, 
		};
	}

	public void SetPriority(MilMo_ResourceManager.Priority priority)
	{
		loadPriority = priority switch
		{
			MilMo_ResourceManager.Priority.High => LoadPriority.High, 
			MilMo_ResourceManager.Priority.Medium => LoadPriority.Medium, 
			MilMo_ResourceManager.Priority.Low => LoadPriority.Low, 
			_ => loadPriority, 
		};
	}

	public void SetAutoPriority()
	{
		loadPriority = LoadPriority.Auto;
	}

	public void SetupAnimations(Dictionary<string, MilMo_VisualRep.AnimationConfig> animationConfigs)
	{
		string[] array = new string[animationConfigs.Count];
		float[] array2 = new float[animationConfigs.Count];
		string[] array3 = new string[animationConfigs.Count];
		float[] array4 = new float[animationConfigs.Count];
		Dictionary<string, MilMo_VisualRep.AnimationConfig>.Enumerator enumerator = animationConfigs.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext() && num < animationConfigs.Count)
		{
			array[num] = enumerator.Current.Key;
			array2[num] = enumerator.Current.Value.Speed;
			array3[num] = ((enumerator.Current.Value.Sound != null) ? enumerator.Current.Value.Sound.Path : "");
			array4[num] = enumerator.Current.Value.Sound?.Tiling ?? 1f;
			num++;
		}
		enumerator.Dispose();
		animations = array;
		animationSpeeds = array2;
		animationSounds = array3;
		animationSoundTilings = array4;
	}
}
