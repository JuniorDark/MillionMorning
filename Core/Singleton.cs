using UnityEngine;

namespace Core;

public class Singleton<T> : MonoBehaviour where T : Component
{
	private static object _lock = new object();

	private static bool _applicationIsQuitting;

	private static T _instance;

	public static T Instance
	{
		get
		{
			if (_applicationIsQuitting)
			{
				return null;
			}
			lock (_lock)
			{
				if (_instance != null)
				{
					return _instance;
				}
				_instance = Object.FindObjectOfType<T>(includeInactive: true);
				if (_instance != null)
				{
					return _instance;
				}
				_instance = new GameObject
				{
					name = typeof(T).Name
				}.AddComponent<T>();
				return _instance;
			}
		}
	}

	public virtual void OnApplicationQuit()
	{
		_applicationIsQuitting = true;
	}
}
