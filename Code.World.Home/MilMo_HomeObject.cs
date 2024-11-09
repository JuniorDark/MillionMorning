using Code.Core.Items;
using Code.Core.Items.Home;
using Code.Core.Network.types;
using Code.Core.Template;
using Core;
using UnityEngine;

namespace Code.World.Home;

public abstract class MilMo_HomeObject
{
	public delegate void OnReadDone(bool success);

	protected delegate void GameObjectDone(bool success);

	private OnReadDone _readDoneCallback;

	protected MilMo_HomeEquipment _item;

	protected Vector3 _spawnPosition;

	protected float _targetRotation;

	protected GameObject _gameObject;

	private bool _isUnloaded;

	public MilMo_HomeEquipment Item => _item;

	public Vector3 Position
	{
		get
		{
			if (!(_gameObject == null))
			{
				return _gameObject.transform.position;
			}
			return _spawnPosition;
		}
		set
		{
			if (_gameObject == null)
			{
				_spawnPosition = value;
			}
			else
			{
				_gameObject.transform.position = value;
			}
		}
	}

	public float Rotation
	{
		get
		{
			return _item?.Rotation ?? _targetRotation;
		}
		set
		{
			if (_item != null)
			{
				_item.Rotation = value;
			}
			_targetRotation = value;
		}
	}

	public GameObject GameObject => _gameObject;

	public virtual void Update()
	{
		UpdateRotation();
	}

	public virtual void UpdateRotation()
	{
		if (_gameObject != null)
		{
			_gameObject.transform.rotation = Quaternion.Euler(0f, _targetRotation, 0f);
		}
	}

	public virtual void Unload()
	{
		_isUnloaded = true;
	}

	public void Load(MilMo_HomeEquipment item, OnReadDone callback)
	{
		_item = item;
		_targetRotation = item.Rotation;
		_readDoneCallback = callback;
		AsyncLoad(delegate(bool success)
		{
			if (success)
			{
				_readDoneCallback(success: true);
			}
			else
			{
				Debug.LogWarning("Failed to load home equipment " + _item.Template.VisualRep);
				_readDoneCallback(success: false);
			}
		});
	}

	public void Read(HomeEquipment equipmentData, OnReadDone callback)
	{
		_targetRotation = equipmentData.GetRotation();
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(equipmentData.GetTemplate(), delegate(MilMo_Template template, bool timeOut)
		{
			MilMo_HomeEquipmentTemplate milMo_HomeEquipmentTemplate = template as MilMo_HomeEquipmentTemplate;
			if (timeOut || milMo_HomeEquipmentTemplate == null)
			{
				if (callback != null)
				{
					callback(success: false);
				}
			}
			else
			{
				_item = milMo_HomeEquipmentTemplate.Instantiate(MilMo_Item.ReadModifiers(equipmentData.GetModifiers())) as MilMo_HomeEquipment;
				if (_item != null)
				{
					_item.Read(equipmentData);
					_item.Rotation = _targetRotation;
					_readDoneCallback = callback;
					AsyncLoad(delegate(bool success)
					{
						if (success)
						{
							_readDoneCallback(success: true);
						}
						else
						{
							Debug.LogWarning("Failed to load home equipment " + _item.Template.VisualRep);
							_readDoneCallback(success: false);
						}
					});
				}
			}
		});
	}

	protected abstract void AsyncLoad(GameObjectDone callback);

	protected virtual bool FinishLoad()
	{
		if (_isUnloaded)
		{
			Unload();
			return false;
		}
		if (_gameObject == null)
		{
			return _gameObject != null;
		}
		_gameObject.transform.position = _spawnPosition;
		_gameObject.transform.rotation = Quaternion.Euler(0f, _targetRotation, 0f);
		return _gameObject != null;
	}
}
