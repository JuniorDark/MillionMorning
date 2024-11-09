using System;
using System.Collections.Generic;
using Core.State;
using Core.State.Basic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.HUD.HealthBar;

public class HealthBar : HudElement
{
	[SerializeField]
	private AssetReference heartPrefab;

	private readonly List<Heart> _hearts = new List<Heart>();

	private GameObject _heart;

	[SerializeField]
	private IntState health;

	[SerializeField]
	private IntState maxHealth;

	private int _displayedHealth;

	private int _displayedMaxHealth;

	private int _disableScheduleID;

	private const float REFRESH_RATE = 0.1f;

	private float _lastRefresh;

	private float _heartBeatRate = 1f;

	private float _lastHeartBeat;

	private void Awake()
	{
		if (!heartPrefab.RuntimeKeyIsValid())
		{
			Debug.LogError("Missing heart prefab!");
			return;
		}
		_lastRefresh = Time.time;
		_lastHeartBeat = Time.time;
		if (_heart == null)
		{
			_heart = heartPrefab.LoadAssetAsync<GameObject>().WaitForCompletion();
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if ((bool)child)
			{
				UnityEngine.Object.Destroy(child.gameObject);
			}
		}
	}

	public void Update()
	{
		UpdateDisplayedHealth();
		if (Time.time > _lastHeartBeat + _heartBeatRate)
		{
			DoHeartBeat();
		}
		if (Time.time > _lastRefresh + 0.1f)
		{
			Refresh();
		}
	}

	private void RebuildSlots()
	{
		int count = _hearts.Count;
		foreach (Heart heart2 in _hearts)
		{
			heart2.Remove();
		}
		_hearts.Clear();
		for (int i = 1; i <= maxHealth.Get(); i += 2)
		{
			Heart heart = CreateHeart();
			if ((bool)heart)
			{
				heart.heartState.SetSize((maxHealth.Get() - i > 0) ? HeartState.HeartSize.Whole : HeartState.HeartSize.Half);
				if ((double)i * 0.5 <= (double)count)
				{
					heart.ShowFast();
				}
				else
				{
					heart.Show();
				}
			}
		}
		FastRefresh();
	}

	private void DoHeartBeat()
	{
		_lastHeartBeat = Time.time;
		if (_displayedHealth > 3)
		{
			return;
		}
		foreach (Heart heart in _hearts)
		{
			heart.Beat();
		}
	}

	private void Refresh()
	{
		_lastRefresh = Time.time;
		if (!_heart)
		{
			return;
		}
		if (_displayedMaxHealth != maxHealth.Get())
		{
			_displayedMaxHealth = maxHealth.Get();
			RebuildSlots();
			return;
		}
		int num = Math.Min(health.Get(), maxHealth.Get());
		int num2 = num - _displayedHealth;
		if (num2 != 0)
		{
			if (num < 4)
			{
				_lastHeartBeat = Time.time - 1f;
				_heartBeatRate = num switch
				{
					3 => 3, 
					2 => 2, 
					_ => 1, 
				};
			}
			if (num2 > 0)
			{
				AddOneHealth();
			}
			else
			{
				DecOneHealth();
			}
		}
	}

	private void FastRefresh()
	{
		_lastRefresh = Time.time;
		int num = Math.Min(health.Get(), maxHealth.Get());
		for (int i = 0; i < _hearts.Count; i++)
		{
			int num2 = (i + 1) * 2 - 1;
			if (num < num2)
			{
				_hearts[i].heartState.SetValue(HeartState.HeartValue.Empty);
			}
			else if (num == num2)
			{
				_hearts[i].heartState.SetValue(HeartState.HeartValue.Half);
			}
			else if (num > num2)
			{
				_hearts[i].heartState.SetValue(HeartState.HeartValue.Whole);
			}
		}
	}

	private void UpdateDisplayedHealth()
	{
		_displayedHealth = 0;
		foreach (Heart heart in _hearts)
		{
			_displayedHealth += (int)heart.heartState.GetValue();
		}
	}

	private void AddOneHealth()
	{
		for (int i = 0; i < _hearts.Count; i++)
		{
			switch (_hearts[i].heartState.GetValue())
			{
			case HeartState.HeartValue.Empty:
				_hearts[i].heartState.SetValue(HeartState.HeartValue.Half);
				_hearts[i].GainFX();
				if (i - 1 >= 0)
				{
					_hearts[i - 1].GainFX();
				}
				return;
			case HeartState.HeartValue.Half:
				_hearts[i].heartState.SetValue(HeartState.HeartValue.Whole);
				_hearts[i].GainFX();
				return;
			default:
				throw new ArgumentOutOfRangeException();
			case HeartState.HeartValue.Whole:
				break;
			}
		}
	}

	private void DecOneHealth()
	{
		for (int num = _hearts.Count - 1; num >= 0; num--)
		{
			switch (_hearts[num].heartState.GetValue())
			{
			case HeartState.HeartValue.Half:
				_hearts[num].heartState.SetValue(HeartState.HeartValue.Empty);
				_hearts[num].DamageFX();
				if (num - 1 >= 0)
				{
					_hearts[num].DamageFX();
				}
				return;
			case HeartState.HeartValue.Whole:
				_hearts[num].heartState.SetValue(HeartState.HeartValue.Half);
				_hearts[num].DamageFX();
				return;
			default:
				throw new ArgumentOutOfRangeException();
			case HeartState.HeartValue.Empty:
				break;
			}
		}
	}

	private Heart CreateHeart()
	{
		if (!_heart)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(_heart, base.transform, worldPositionStays: false);
		Heart component = gameObject.GetComponent<Heart>();
		if (!component)
		{
			UnityEngine.Object.Destroy(gameObject);
			return null;
		}
		_hearts.Add(component);
		return component;
	}

	public override void SetHudVisibility(bool shouldShow)
	{
		if (shouldShow)
		{
			Open();
		}
		else
		{
			Close();
		}
	}

	private void Open()
	{
		if (_disableScheduleID != 0)
		{
			LeanTween.cancel(_disableScheduleID);
			_disableScheduleID = 0;
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
			RebuildSlots();
		}
		float num = 0.4f;
		foreach (Heart heart in _hearts)
		{
			num += 0.06f;
			LeanTween.delayedCall(heart.gameObject, num, (Action)delegate
			{
				heart.Show();
			});
		}
	}

	private void Close()
	{
		if (_disableScheduleID != 0)
		{
			return;
		}
		_disableScheduleID = LeanTween.delayedCall(base.gameObject, 1.5f, (Action)delegate
		{
			foreach (Heart heart2 in _hearts)
			{
				heart2.Remove();
			}
			_hearts.Clear();
			base.gameObject.SetActive(value: false);
			_disableScheduleID = 0;
		}).id;
		float num = 0f;
		foreach (Heart heart in _hearts)
		{
			num += 0.06f;
			LeanTween.delayedCall(heart.gameObject, num, (Action<object>)delegate
			{
				heart.Hide();
			});
		}
	}
}
