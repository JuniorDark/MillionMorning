using System;
using Code.Core.Network;
using Code.Core.Visual;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.World.Quest;

public sealed class MilMo_QuestArea
{
	private const float UPDATE_TIME_LIMIT = 8f;

	private readonly string _world;

	private readonly string _level;

	private readonly Vector3 _center;

	private readonly float _radiusSquared;

	private readonly float _maxHeight;

	private readonly float _minHeight;

	private float _lastUpdateTime = -8f;

	private readonly AreaEffect _areaFX;

	public string Name { get; }

	public string FullLevelName => _world + ":" + _level;

	public MilMo_QuestArea(string fullAreaName, Vector3 center, float radiusSquared, float height)
	{
		string[] array = fullAreaName.Split(':');
		try
		{
			_world = array[0];
			_level = array[1];
			Name = array[2];
		}
		catch (IndexOutOfRangeException ex)
		{
			Debug.LogWarning("Got invalid area name when creating quest area. Area name is " + fullAreaName + "\n" + ex.Message);
			_world = "";
			_level = "";
			Name = "";
		}
		_center = center;
		_radiusSquared = radiusSquared;
		_maxHeight = center.y + height;
		_minHeight = center.y - height;
		_areaFX = new AreaEffect(fullAreaName, center, (float)Math.Sqrt(radiusSquared), height);
	}

	public void Update()
	{
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.Avatar == null)
		{
			return;
		}
		_areaFX.Update();
		if (!(Time.time - _lastUpdateTime < 8f))
		{
			Vector3 position = MilMo_Player.Instance.Avatar.Position;
			float y = position.y;
			if ((position - _center).sqrMagnitude < _radiusSquared && y < _maxHeight && y > _minHeight)
			{
				Debug.Log("Inside area " + Name);
				Singleton<GameNetwork>.Instance.SendInsideQuestAreaRequest(Name, position);
				_lastUpdateTime = Time.time;
			}
		}
	}

	public void Remove()
	{
		_areaFX.Destroy();
	}
}
