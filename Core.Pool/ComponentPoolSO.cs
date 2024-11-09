using UnityEngine;

namespace Core.Pool;

public abstract class ComponentPoolSO<T> : PoolSO<T> where T : Component
{
	private Transform _poolRoot;

	private Transform _parent;

	private Transform PoolRoot
	{
		get
		{
			if (_poolRoot == null)
			{
				_poolRoot = new GameObject(base.name).transform;
				_poolRoot.SetParent(_parent);
			}
			return _poolRoot;
		}
	}

	public void SetParent(Transform t)
	{
		_parent = t;
		PoolRoot.SetParent(_parent);
	}

	public override T Request()
	{
		T val = base.Request();
		if (val != null)
		{
			val.gameObject.SetActive(value: true);
		}
		return val;
	}

	public override void Return(T member)
	{
		member.transform.SetParent(PoolRoot.transform);
		member.gameObject.SetActive(value: false);
		base.Return(member);
	}

	protected override T Create()
	{
		T val = base.Create();
		val.transform.SetParent(PoolRoot.transform);
		val.gameObject.SetActive(value: false);
		return val;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		if (_poolRoot != null)
		{
			Object.Destroy(_poolRoot.gameObject);
		}
	}
}
