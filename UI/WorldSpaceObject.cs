using UnityEngine;

namespace UI;

public abstract class WorldSpaceObject : MonoBehaviour, IWorldSpaceObject
{
	private WorldSpaceManager _worldSpaceManager;

	protected virtual void Awake()
	{
		_worldSpaceManager = WorldSpaceManager.Get();
		if (_worldSpaceManager == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find WorldSpaceManager");
		}
	}

	public virtual void Initialize()
	{
		if (!(_worldSpaceManager == null))
		{
			_worldSpaceManager.Add(this);
		}
	}

	protected virtual void OnDestroy()
	{
		if (!(_worldSpaceManager == null))
		{
			_worldSpaceManager.Remove(this);
		}
	}

	public void Remove()
	{
		Object.Destroy(base.gameObject);
	}

	public GameObject GetGameObject()
	{
		return base.gameObject;
	}
}
