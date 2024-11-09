using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Command;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Input;
using Code.Core.Items;
using Code.Core.Music;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.Core.Network.messages.server;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Visual;
using Code.World.CharacterShop.RemoteShop;
using Code.World.GUI.LoadingScreen;
using Code.World.Level;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Code.World.WorldMap;
using Core;
using Core.GameEvent;
using UnityEngine;

namespace Code.World.GUI.Hub;

public sealed class MilMo_Hub : MonoBehaviour
{
	private static MilMo_Hub _theHub;

	private MilMo_HubInfoData _data;

	private float _lastInfoMessageTime = float.MinValue;

	private bool _needsNewInfo;

	private static bool _firstTimeEntering;

	private MilMo_LoadingPane _loadingPane;

	public static bool WasTravelClosed;

	public static string TravelClosedFullLevelName;

	public static Vector3 HubWorldPosition;

	private List<MilMo_HubItem> _hubItems;

	private List<MilMo_VisualRep> _models;

	private GameObject _hubLight;

	private Light[] _worldLights;

	internal static bool IsSafeToClick { get; private set; }

	public MilMo_HubMenu Menu { get; private set; }

	public MilMo_HomeOfTheDay HomeOfTheDay { get; private set; }

	public static MilMo_Hub Instance
	{
		get
		{
			if (_theHub == null)
			{
				Init();
			}
			return _theHub;
		}
	}

	public MilMo_UserInterface UI { get; private set; }

	public ServerStartScreenInfo StartScreenInfo { get; private set; }

	static MilMo_Hub()
	{
		_firstTimeEntering = true;
		TravelClosedFullLevelName = "";
		HubWorldPosition = new Vector3(-10000f, -10000f, -10000f);
		IsSafeToClick = true;
	}

	public static void ClickMade()
	{
		IsSafeToClick = false;
	}

	private static void Init()
	{
		try
		{
			MilMo_Hub milMo_Hub = MilMo_Global.MainGameObject.AddComponent<MilMo_Hub>();
			if (milMo_Hub == null)
			{
				Debug.LogWarning("Failed to create hub. Failed to add component MilMo_Hub.");
			}
			_theHub = milMo_Hub;
			if (!(_theHub == null))
			{
				_theHub.enabled = false;
				_theHub.UI = MilMo_UserInterfaceManager.CreateUserInterface("hubUI");
				MilMo_UserInterfaceManager.SetUserInterfaceDepth(_theHub.UI, -500);
				_theHub.InitListeners();
				_theHub.InitVisual();
				_theHub.InitUI();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	public static void TravelWorld(string world)
	{
		if (!_theHub)
		{
			Debug.LogWarning("Trying to travel to world " + world + " when theHub is null");
			return;
		}
		MilMo_World.CurrentWorld = world;
		TravelClosedFullLevelName = MilMo_LevelInfo.GetLastLevelInfoForWorld(world).FullLevelName;
		WasTravelClosed = true;
		MilMo_Player.Instance.RequestLeaveHub();
	}

	public void SetNeedsNewInfo()
	{
		_needsNewInfo = true;
		if (base.enabled)
		{
			Singleton<GameNetwork>.Instance.RequestStartScreenInfo();
		}
	}

	public void ReadInfoMessage(ServerStartScreenInfo info)
	{
		StartScreenInfo = info;
		_lastInfoMessageTime = Time.time;
		_needsNewInfo = false;
		if (base.enabled)
		{
			MilMo_EventSystem.At(1f, SetHomeState);
		}
	}

	private void InitUI()
	{
		MilMo_Command.Instance.RegisterCommand("Hub.Activate", delegate
		{
			foreach (MilMo_HubItem hubItem in _hubItems)
			{
				hubItem.ShowFakeBubble();
			}
			return "";
		});
		_data = new MilMo_HubInfoData(MilMo_SimpleFormat.LoadLocal("StartScreen/StartScreen"));
		Menu = new MilMo_HubMenu(UI);
		if (_loadingPane == null)
		{
			_loadingPane = new MilMo_LoadingPane(UI);
			UI.AddChild(_loadingPane);
		}
		UI.AddChild(Menu);
		UI.ResetLayout();
		UI.ScreenSizeDirty = true;
		UI.Enabled = false;
	}

	private void InitListeners()
	{
		MilMo_EventSystem.Listen("hubitem_state_off", delegate(object o)
		{
			string identifier = o as string;
			if (string.IsNullOrEmpty(identifier))
			{
				Debug.LogWarning("Trying to deactivate a hud item. But identifier is null.");
				return;
			}
			if (identifier.ToUpper() == "NEWS")
			{
				MilMo_NewsManager.Instance.SetNewsRead();
			}
			using IEnumerator<MilMo_HubItem> enumerator2 = _hubItems.Where((MilMo_HubItem hubItem) => string.Equals(hubItem.Identifier, identifier, StringComparison.CurrentCultureIgnoreCase)).GetEnumerator();
			if (enumerator2.MoveNext())
			{
				enumerator2.Current.SetState(active: false, "");
				StateUpdate();
			}
		}).Repeating = true;
		MilMo_EventSystem.Listen("hubitem_state_on", delegate(object o)
		{
			string[] arguments = o as string[];
			if (arguments != null)
			{
				if (!string.IsNullOrEmpty(arguments[0]))
				{
					using (IEnumerator<MilMo_HubItem> enumerator = _hubItems.Where((MilMo_HubItem hubItem) => string.Equals(hubItem.Identifier, arguments[0], StringComparison.CurrentCultureIgnoreCase)).GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							MilMo_HubItem current = enumerator.Current;
							if (current.Identifier.ToUpper() == "SHOP" && arguments[1] == "SHOPHOTITEMS" && current.StateIsActive)
							{
								current.SetState(active: false, "");
							}
							current.SetState(active: true, arguments[1]);
							StateUpdate();
						}
						return;
					}
				}
				Debug.LogWarning("Trying to deactivate a hud item. But identifier is null.");
			}
		}).Repeating = true;
		MilMo_EventSystem.Listen("home_of_the_day_response", delegate(object msg)
		{
			if (msg is ServerHomeOfTheDayResponse serverHomeOfTheDayResponse)
			{
				HomeOfTheDay = new MilMo_HomeOfTheDay(serverHomeOfTheDayResponse.getHome().GetAvatarName(), serverHomeOfTheDayResponse.getHome().GetHomeName(), serverHomeOfTheDayResponse.getHome().GetUserId());
			}
			if (MilMo_Player.Instance.InHub)
			{
				MilMo_HubItemFunctionality.OpenStartScreen();
			}
		}).Repeating = true;
	}

	public Vector2 GetScreenPosition(string identifier)
	{
		using (IEnumerator<MilMo_HubItem> enumerator = _hubItems.Where((MilMo_HubItem item) => string.Equals(item.Identifier, identifier, StringComparison.CurrentCultureIgnoreCase)).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current.ScreenPosition;
			}
		}
		return new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
	}

	private void InitVisual()
	{
		_hubItems = new List<MilMo_HubItem>();
		_models = new List<MilMo_VisualRep>();
	}

	private void FixedUpdate()
	{
		try
		{
			if (UI != null && UI.ScreenSizeDirty)
			{
				RefreshUI();
				UI.ScreenSizeDirty = false;
			}
			foreach (MilMo_HubItem hubItem in _hubItems)
			{
				hubItem.Update();
			}
			IsSafeToClick = true;
			if (MilMo_Pointer.LeftClick)
			{
				ClickMade();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private MilMo_HubItem GetItem(string identifier)
	{
		return _hubItems.FirstOrDefault((MilMo_HubItem t) => string.Equals(t.Identifier, identifier, StringComparison.CurrentCultureIgnoreCase));
	}

	private void StateUpdate()
	{
		List<string> list = (from item in _hubItems
			where item.StateIsActive
			select item.Identifier.ToUpper()).ToList();
		foreach (MilMo_HubItem hubItem in _hubItems)
		{
			hubItem.SetWobbleOff();
		}
		if (list.Contains("HOME"))
		{
			GetItem("HOME").SetWobbleOn();
		}
		else if (list.Contains("SHOP"))
		{
			GetItem("SHOP").SetWobbleOn();
		}
		else if (list.Contains("NEWS"))
		{
			GetItem("NEWS").SetWobbleOn();
		}
		else if (list.Contains("COLOSSEUM"))
		{
			GetItem("COLOSSEUM").SetWobbleOn();
		}
		else if (list.Contains("MAPROOM"))
		{
			GetItem("MAPROOM").SetWobbleOn();
		}
		else if (list.Contains("CHATROOMS"))
		{
			GetItem("CHATROOMS").SetWobbleOn();
		}
		else if (list.Contains("MAKEOVERSTUDIO"))
		{
			GetItem("MAKEOVERSTUDIO").SetWobbleOn();
		}
	}

	private void RefreshUI()
	{
		_loadingPane?.SetPosition(Screen.width - 50, 100f);
		Menu.RefreshUI();
		foreach (MilMo_HubItem hubItem in _hubItems)
		{
			hubItem.RefreshUI();
		}
	}

	public void Activate(MilMo_WorldMap.WorldMapReady callback)
	{
		try
		{
			_worldLights = UnityEngine.Object.FindObjectsOfType(typeof(Light)) as Light[];
			if (_worldLights != null)
			{
				Light[] worldLights = _worldLights;
				foreach (Light light in worldLights)
				{
					if (light != null)
					{
						light.gameObject.SetActive(value: false);
					}
				}
			}
			if (MilMo_Player.Instance != null)
			{
				MilMo_Player.Instance.InHub = true;
			}
			if (MilMo_World.PvpScoreBoard != null)
			{
				MilMo_World.PvpScoreBoard.Close(null);
			}
			if (Time.time - _lastInfoMessageTime > 60f || _needsNewInfo)
			{
				Singleton<GameNetwork>.Instance.RequestStartScreenInfo();
			}
			if (MilMo_Button.StaticFunction == null)
			{
				MilMo_Button.StaticFunction = delegate
				{
					if (Instance != null && Instance.enabled)
					{
						ClickMade();
					}
				};
			}
			SetUpHouses(delegate(bool success)
			{
				if (success)
				{
					MilMo_World.Instance.UI.Enabled = false;
					MilMo_World.HudHandler.enabled = false;
					GameEvent.ShowHUDEvent.RaiseEvent(args: false);
					if (_hubLight == null)
					{
						_hubLight = new GameObject("HubLight");
					}
					Light light2 = _hubLight.GetComponent<Light>();
					if (!light2)
					{
						light2 = _hubLight.AddComponent<Light>();
					}
					light2.type = LightType.Directional;
					light2.shadows = LightShadows.None;
					light2.renderMode = LightRenderMode.ForcePixel;
					light2.color = _data.Light.LightColor;
					light2.intensity = _data.Light.Intensity;
					light2.range = _data.Light.Range;
					light2.transform.position = new Vector3(-10000f, -9900f, -10000f);
					_hubLight.transform.eulerAngles = _data.Light.Rotation;
					RenderSettings.ambientLight = _data.Light.AmbientLightColor;
					RenderSettings.fog = _data.Light.Fog;
					RenderSettings.fogColor = _data.Light.FogColor;
					RenderSettings.fogDensity = _data.Light.FogDensity;
					Menu.Open();
					foreach (MilMo_HubItem hubItem in _hubItems)
					{
						hubItem.Enable();
					}
					foreach (MilMo_VisualRep model in _models)
					{
						model.Enable();
						MilMo_VisualRepContainer.AddForUpdate(model);
					}
					base.enabled = true;
					UI.Enabled = true;
					MilMo_World.Instance.Camera.HookupHubCam();
					MilMo_EventSystem.At(1f, delegate
					{
						SetNewsState();
						SetShopState();
						SetHomeState();
					});
					if (!string.IsNullOrEmpty(_data.Music))
					{
						MilMo_Music.Instance.FadeIn(_data.Music);
					}
				}
				else
				{
					Debug.Log("Error loading HUB.");
				}
				if (callback != null)
				{
					callback(success);
				}
				if (_firstTimeEntering)
				{
					Singleton<GameNetwork>.Instance.SendToGameServer(new ClientRequestHomeOfTheDay());
					_firstTimeEntering = false;
				}
			});
			Debug.LogWarning("Joined Town");
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	public void Deactivate()
	{
		try
		{
			Light[] worldLights = _worldLights;
			foreach (Light light in worldLights)
			{
				if (light != null)
				{
					light.gameObject.SetActive(value: true);
				}
			}
			MilMo_Global.Destroy(_hubLight);
			MilMo_World.HudHandler.enabled = true;
			GameEvent.ShowHUDEvent.RaiseEvent(args: true);
			MilMo_World.Instance.UI.Enabled = true;
			foreach (MilMo_HubItem hubItem in _hubItems)
			{
				hubItem.Disable();
			}
			foreach (MilMo_VisualRep model in _models)
			{
				model.Disable();
				MilMo_VisualRepContainer.RemoveFromUpdate(model);
			}
			if (MilMo_Instance.CurrentInstance != null)
			{
				MilMo_Instance.CurrentInstance.Environment.RestoreRenderSettings();
			}
			Menu.Close();
			UI.Enabled = false;
			base.enabled = false;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private void SetHomeState()
	{
		if (StartScreenInfo == null)
		{
			return;
		}
		if (StartScreenInfo.getHomeDeliveryBox() != null)
		{
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(StartScreenInfo.getHomeDeliveryBox(), delegate(MilMo_Template template, bool success)
			{
				if (template is MilMo_ItemTemplate milMo_ItemTemplate)
				{
					milMo_ItemTemplate.Instantiate(new Dictionary<string, string>()).AsyncGetIcon(delegate(Texture2D tex)
					{
						MilMo_HubItemFunctionality.CurrentHomeDeliveryBoxTexture = tex;
						MilMo_EventSystem.Instance.PostEvent("hubitem_state_on", new string[2] { "HOME", "HOMEDELIVERYBOXACTIVE" });
						StateUpdate();
					});
				}
			});
			return;
		}
		IList<string> guests = StartScreenInfo.getGuestsInHome();
		if (guests == null || guests.Count <= 0)
		{
			return;
		}
		foreach (string item in guests)
		{
			MilMo_ProfileManager.GetPlayerPortrait(item, delegate(bool success, string playerId, Texture2D texture)
			{
				if (!success)
				{
					GetInvisibleTextureAsync(guests);
				}
				else
				{
					MilMo_HubItemFunctionality.GuestInHomeTextures.Add(texture);
					if (MilMo_HubItemFunctionality.GuestInHomeTextures.Count == guests.Count)
					{
						MilMo_EventSystem.Instance.PostEvent("hubitem_state_on", new string[2] { "HOME", "HOMEGUEST" });
						StateUpdate();
					}
				}
			});
		}
	}

	private static async void GetInvisibleTextureAsync(IList<string> guests)
	{
		Texture2D item = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/Core/Invisible");
		MilMo_HubItemFunctionality.GuestInHomeTextures.Add(item);
		if (MilMo_HubItemFunctionality.GuestInHomeTextures.Count == guests.Count)
		{
			MilMo_EventSystem.Instance.PostEvent("hubitem_state_on", new string[2] { "HOME", "HOMEGUEST" });
		}
	}

	private void SetShopState()
	{
		MilMo_HotItems.AsyncLoadHotItemsFile(HotItemsDone);
	}

	private async void HotItemsDone(bool success)
	{
		if (!success || !MilMo_HotItems.HotItemsAreNew)
		{
			return;
		}
		MilMo_HubItemFunctionality.HotItemsTextures.Clear();
		foreach (string itemIdentifier in MilMo_HotItems.ItemIdentifiers)
		{
			if (!(await Singleton<MilMo_TemplateContainer>.Instance.GetTemplateAsync(itemIdentifier) is MilMo_ItemTemplate milMo_ItemTemplate))
			{
				Debug.Log("Item template is null.");
				Texture2D item = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/Core/Invisible");
				MilMo_HubItemFunctionality.HotItemsTextures.Add(item);
				if (MilMo_HubItemFunctionality.HotItemsTextures.Count != MilMo_HotItems.ItemIdentifiers.Count)
				{
					return;
				}
				MilMo_EventSystem.Instance.PostEvent("hubitem_state_on", new string[2] { "SHOP", "SHOPHOTITEMS" });
				StateUpdate();
			}
			else
			{
				Texture2D item2 = await milMo_ItemTemplate.Instantiate(new Dictionary<string, string>()).AsyncGetIcon();
				MilMo_HubItemFunctionality.HotItemsTextures.Add(item2);
				if (MilMo_HubItemFunctionality.HotItemsTextures.Count != MilMo_HotItems.ItemIdentifiers.Count)
				{
					return;
				}
				MilMo_EventSystem.Instance.PostEvent("hubitem_state_on", new string[2] { "SHOP", "SHOPHOTITEMS" });
				StateUpdate();
			}
		}
	}

	private void SetNewsState()
	{
		if (MilMo_NewsManager.Instance.NewsAreNew)
		{
			MilMo_EventSystem.Instance.PostEvent("hubitem_state_on", new string[2] { "NEWS", "NEWNEWS" });
		}
	}

	private void SetUpHouses(MilMo_WorldMap.WorldMapReady callback)
	{
		if (_hubItems.Count == _data.Items.Count && _models.Count == _data.Models.Count)
		{
			callback(success: true);
			return;
		}
		UnloadAllVisualReps();
		int amountNeededToBeLoaded = _data.Items.Count;
		int modelsNeededToBeLoaded = _data.Models.Count;
		bool modelsLoaded = false;
		bool itemsLoaded = false;
		MilMo_LoadingScreen.Instance.GotTownPropsCount(amountNeededToBeLoaded + modelsNeededToBeLoaded);
		foreach (MilMo_HubInfoData.MilMo_HubItemInfoData item in _data.Items)
		{
			MilMo_HubInfoData.MilMo_HubItemInfoData d = item;
			Vector3 position = HubWorldPosition - d.Position;
			MilMo_VisualRepContainer.AsyncCreateVisualRep(item.Model, position, Quaternion.Euler(item.Rotation), setDefaultMaterialTexture: true, waitForMaterial: true, delegate(MilMo_VisualRep rep)
			{
				if (rep != null)
				{
					_hubItems.Add(new MilMo_HubItem(rep, d, position));
				}
				else
				{
					Debug.Log("Error creating visual rep in MilMo_Hub.");
				}
				MilMo_LoadingScreen.Instance.TownPropLoaded();
				int num2 = amountNeededToBeLoaded;
				amountNeededToBeLoaded = num2 - 1;
				if (amountNeededToBeLoaded == 0)
				{
					itemsLoaded = true;
					if (itemsLoaded && modelsLoaded)
					{
						callback(success: true);
					}
				}
			});
		}
		foreach (MilMo_HubInfoData.MilMo_HubModelData model in _data.Models)
		{
			MilMo_HubInfoData.MilMo_HubModelData m = model;
			Vector3 position2 = HubWorldPosition - m.Position;
			MilMo_VisualRepContainer.AsyncCreateVisualRep(m.Model, position2, Quaternion.Euler(m.Rotation), setDefaultMaterialTexture: true, waitForMaterial: true, delegate(MilMo_VisualRep rep)
			{
				if (rep != null)
				{
					rep.GameObject.transform.localScale = m.Scale;
					_models.Add(rep);
				}
				else
				{
					Debug.Log("Error creating visual rep in MilMo_Hub.");
				}
				MilMo_LoadingScreen.Instance.TownPropLoaded();
				int num = modelsNeededToBeLoaded;
				modelsNeededToBeLoaded = num - 1;
				if (modelsNeededToBeLoaded == 0)
				{
					modelsLoaded = true;
					if (modelsLoaded && itemsLoaded)
					{
						callback(success: true);
					}
				}
			});
		}
	}

	private void UnloadAllVisualReps()
	{
		foreach (MilMo_VisualRep model in _models)
		{
			MilMo_VisualRepContainer.RemoveFromUpdate(model);
			MilMo_VisualRepContainer.DestroyVisualRep(model);
		}
		_models.Clear();
		foreach (MilMo_HubItem hubItem in _hubItems)
		{
			hubItem.Destroy();
		}
		_hubItems.Clear();
	}
}
