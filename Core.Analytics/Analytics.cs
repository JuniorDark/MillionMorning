using System;
using System.Collections;
using System.Collections.Generic;
using Code.Core.Network;
using Code.World.Player;
using UnityEngine;
using UnityEngine.Analytics;

namespace Core.Analytics;

public class Analytics : Singleton<Analytics>
{
	private FPS _fps;

	private Ping _ping;

	private Visualizer _visualizer;

	private bool _visualize;

	private void Start()
	{
		_fps = new FPS();
		_ping = new Ping();
		_visualizer = new Visualizer();
		_visualize = false;
	}

	private void Update()
	{
		try
		{
			_fps?.Update();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			base.enabled = false;
		}
	}

	private void FixedUpdate()
	{
		try
		{
			_fps?.FixedUpdate();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			base.enabled = false;
		}
	}

	private void OnGUI()
	{
		if (!_visualize)
		{
			return;
		}
		try
		{
			Draw();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			_visualize = false;
		}
	}

	public override void OnApplicationQuit()
	{
		base.OnApplicationQuit();
		GameOver();
	}

	private void Draw()
	{
		float fPS = _fps.GetFPS();
		float fixedFPS = _fps.GetFixedFPS();
		IEnumerable pingTimes = _ping.GetPingTimes();
		_visualizer?.Draw(fPS, fixedFPS, pingTimes);
	}

	public bool ToggleVisualization()
	{
		return _visualize = !_visualize;
	}

	private void GameOver()
	{
		float num = _fps?.MedianFps ?? 0f;
		float num2 = _ping?.MedianPing ?? 0f;
		MilMoAnalyticsHandler.GameOver(num, num2);
		if (!(Singleton<GameNetwork>.Instance == null) && num > 0f && num2 > 0f)
		{
			Singleton<GameNetwork>.Instance.SendSessionFPSAndPingReport(num, num2);
		}
	}

	public void LevelStart(string verboseName, float loadTimeDelta)
	{
		_fps?.StartSampling();
		if (!string.IsNullOrEmpty(verboseName))
		{
			MilMoAnalyticsHandler.LevelStart(verboseName, loadTimeDelta);
			AnalyticsEvent.LevelStart(verboseName);
		}
		SendPlayerEvent("Enter" + verboseName, "");
		if (Singleton<GameNetwork>.Instance != null)
		{
			Singleton<GameNetwork>.Instance.SendLevelLoadTimeReport(verboseName, loadTimeDelta);
		}
	}

	public void LevelQuit(string verboseName)
	{
		if (verboseName == null)
		{
			verboseName = "";
		}
		SendPlayerEvent("Exit" + verboseName, "");
		FPS fps = _fps;
		if (fps != null && fps.IsSampling)
		{
			_fps.StopSampling();
			if (Singleton<GameNetwork>.Instance != null)
			{
				Singleton<GameNetwork>.Instance.SendLevelFPSReport(verboseName, _fps.MedianLevelFps);
			}
		}
	}

	public static void ScreenVisit(string screenName)
	{
		MilMoAnalyticsHandler.SceneVisit(screenName);
		SendPlayerEvent(screenName, "");
	}

	public static void CutsceneStart(string scene)
	{
		MilMoAnalyticsHandler.StartCutscene(scene);
		SendPlayerEvent("Start" + scene, "");
	}

	public static void CutsceneSkip(string scene)
	{
		MilMoAnalyticsHandler.SkipCutscene(scene);
		SendPlayerEvent("Skip" + scene, "");
	}

	public static void CutsceneEnd(string scene)
	{
		MilMoAnalyticsHandler.EndCutscene(scene);
		SendPlayerEvent("End" + scene, "");
	}

	public static void ChangeWorld(string world)
	{
		SendPlayerEvent("ChangeWorldMap", world);
	}

	public static void OpenCompass(string level)
	{
		MilMoAnalyticsHandler.OpenCompass(level);
		SendPlayerEvent("OpenCompass", "");
	}

	private static void SendPlayerEvent(string eventTag, string data)
	{
		if (Application.isPlaying && !(Singleton<GameNetwork>.Instance == null) && MilMo_Player.Instance != null && MilMo_Player.Instance.IsNewPlayer())
		{
			Singleton<GameNetwork>.Instance.SendPlayerEvent(eventTag, data);
		}
	}

	public static void CustomEventWithData(string eventName, Dictionary<string, object> eventData)
	{
		UnityEngine.Analytics.Analytics.CustomEvent(eventName, eventData);
	}

	public static void CustomEvent(string eventName)
	{
		UnityEngine.Analytics.Analytics.CustomEvent(eventName);
	}

	public void PositionUpdateSent()
	{
		_ping?.PositionUpdateSent();
	}

	public void PositionUpdateReceived()
	{
		_ping?.PositionUpdateReceived();
	}

	public static void PlayedMinutes(int minutes)
	{
		CustomEventWithData("PlayedMinutes", new Dictionary<string, object> { { "played_minutes", minutes } });
	}

	public static void Exhausted(string levelId, Vector3 position)
	{
		Vector3 vector = position;
		string text = "xyz: " + vector.ToString();
		MilMoAnalyticsHandler.PlayerExhausted(levelId, text);
		SendPlayerEvent("Exhausted", text);
	}

	public static void Teleport(Vector3 position)
	{
		Vector3 vector = position;
		SendPlayerEvent("Teleport", "xyz: " + vector.ToString());
	}

	public static void PremiumStoreOpened()
	{
		AnalyticsEvent.StoreOpened(StoreType.Premium);
		SendPlayerEvent("EnterShop", "");
	}

	public static void NPCStoreOpened()
	{
		AnalyticsEvent.StoreOpened(StoreType.Soft);
	}

	public static void Transaction(string product, int price)
	{
		UnityEngine.Analytics.Analytics.Transaction(product, price, "USD", null, null);
	}

	public static void StoreClick(string item)
	{
		AnalyticsEvent.StoreItemClick(StoreType.Soft, item);
	}

	public static void ItemAcquired(short amount, string item, string category)
	{
		AnalyticsEvent.ItemAcquired(AcquisitionType.Soft, "SHOP", amount, item, category);
	}
}
