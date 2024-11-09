using System.Collections;
using System.Collections.Generic;
using Core;
using Core.Utilities;
using UnityEngine;

namespace UI;

public class WorldSpaceManager : Singleton<WorldSpaceManager>, IList<IWorldSpaceObject>, ICollection<IWorldSpaceObject>, IEnumerable<IWorldSpaceObject>, IEnumerable
{
	[Header("Setup")]
	[SerializeField]
	private GameObject container;

	private readonly IList<IWorldSpaceObject> _objects = new List<IWorldSpaceObject>();

	public int Count => _objects.Count;

	public bool IsReadOnly => _objects.IsReadOnly;

	public IWorldSpaceObject this[int index]
	{
		get
		{
			return _objects[index];
		}
		set
		{
			_objects[index] = value;
		}
	}

	protected void Awake()
	{
		if (container == null)
		{
			Debug.LogError(base.gameObject.name + ": container is null");
		}
	}

	private void DestroyObject(IWorldSpaceObject worldSpaceObject)
	{
		if (worldSpaceObject != null)
		{
			Object.Destroy(worldSpaceObject.GetGameObject());
		}
	}

	public static T GetWorldSpaceObject<T>(string address, Transform parent = null) where T : WorldSpaceObject
	{
		return Instantiator.Instantiate<T>(address, parent);
	}

	public void Add(IWorldSpaceObject worldSpaceObject)
	{
		if (worldSpaceObject != null)
		{
			GameObject gameObject = worldSpaceObject.GetGameObject();
			if (gameObject != null && container != null)
			{
				gameObject.transform.SetParent(container.transform);
			}
			_objects?.Add(worldSpaceObject);
		}
	}

	public void Clear()
	{
		for (int num = Count - 1; num >= 0; num--)
		{
			DestroyObject(_objects[num]);
			_objects.RemoveAt(num);
		}
	}

	public void CopyTo(IWorldSpaceObject[] array, int arrayIndex)
	{
		_objects.CopyTo(array, arrayIndex);
	}

	bool ICollection<IWorldSpaceObject>.Remove(IWorldSpaceObject item)
	{
		return _objects.Remove(item);
	}

	public void Remove(IWorldSpaceObject worldSpaceObject)
	{
		_objects.Remove(worldSpaceObject);
	}

	public bool Contains(IWorldSpaceObject worldSpaceObject)
	{
		return _objects.Contains(worldSpaceObject);
	}

	public IEnumerator<IWorldSpaceObject> GetEnumerator()
	{
		return _objects.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public int IndexOf(IWorldSpaceObject item)
	{
		return _objects.IndexOf(item);
	}

	public void Insert(int index, IWorldSpaceObject item)
	{
		_objects.Insert(index, item);
	}

	public void RemoveAt(int index)
	{
		_objects.RemoveAt(index);
	}

	public static WorldSpaceManager Get()
	{
		return Singleton<WorldSpaceManager>.Instance;
	}

	public static GameObject GetContainer()
	{
		return Singleton<WorldSpaceManager>.Instance.container;
	}
}
