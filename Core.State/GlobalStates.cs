using System;
using Core.State.Basic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.State;

public class GlobalStates : MonoBehaviour
{
	[Header("Player State")]
	public PlayerState playerState;

	public IntState playerUnusedClassLevels;

	[Header("Level State")]
	public LevelState levelState;

	private static GlobalStates _instance;

	public static GlobalStates Instance
	{
		get
		{
			if ((bool)_instance)
			{
				return _instance;
			}
			GameObject obj = Addressables.LoadAssetAsync<GameObject>("GlobalState").WaitForCompletion();
			if (!obj)
			{
				throw new Exception("GlobalStates prefab could not be loaded!");
			}
			GlobalStates component = obj.GetComponent<GlobalStates>();
			if (!component)
			{
				throw new Exception("GlobalStates component could not be loaded!");
			}
			_instance = component;
			return _instance;
		}
	}
}
