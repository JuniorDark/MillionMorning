using System.Threading.Tasks;
using Code.Apps.Fade;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget.QuickInfoDialogs;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.Player;
using UnityEngine;

namespace Code.World.GUI.LoadingScreen;

public sealed class MilMo_LoadingScreen
{
	public enum State
	{
		Disabled,
		LoadLevel,
		IntraTeleport,
		LoadCharBuilder,
		LoadChaBuilderNew,
		LoadShop,
		LoadRoom,
		Respawn,
		LoadHome,
		LoadTown,
		LoadNavigator
	}

	private static MilMo_LoadingScreen _theLoadingScreen;

	private readonly MilMo_UserInterface _ui;

	private MilMo_Widget _icon;

	private MilMo_Widget _text;

	private MilMo_LoadingBar _progressBar;

	private AudioClip _fadeOutSound;

	private bool _loadingFadeOutSound;

	private MilMo_TimerEvent _fadeTimer;

	private float _propFraction;

	private float _homeEquipmentFraction;

	private float _townPropFraction;

	private float _charbuilderInitWaitProgress;

	private int _shopProgress;

	private MilMo_GenericReaction _createPremiumTokenReaction;

	private readonly MilMo_GenericReaction _renderUpdate;

	private MilMo_PremiumIcon _premiumIcon;

	private MilMo_TokenIcons _tokenIcons;

	private ServerExplorationTokensCreate _explorationTokens;

	private ServerCoinTokensCreate _coinTokens;

	public static MilMo_LoadingScreen Instance
	{
		get
		{
			if (_theLoadingScreen == null)
			{
				Create();
			}
			return _theLoadingScreen;
		}
	}

	public State LoadingState { get; private set; }

	public bool IsLoading => LoadingState != State.Disabled;

	private static void Create()
	{
		_theLoadingScreen = new MilMo_LoadingScreen(MilMo_Fade.Instance.UserInterface);
	}

	private void RefreshUI()
	{
		_icon?.SetPosition((float)Screen.width * 0.5f + MilMo_LoadingScreenConf.IconCenterOffset.x, (float)Screen.height * 0.5f + MilMo_LoadingScreenConf.IconCenterOffset.y);
		_text?.SetPosition((float)Screen.width * 0.5f + MilMo_LoadingScreenConf.TextCenterOffset.x, (float)Screen.height * 0.5f + MilMo_LoadingScreenConf.TextCenterOffset.y);
		_ui.ScreenSizeDirty = false;
	}

	private void OnPreRender(object obj)
	{
		if (_ui.ScreenSizeDirty)
		{
			RefreshUI();
		}
	}

	private MilMo_LoadingScreen(MilMo_UserInterface ui)
	{
		LoadingState = State.Disabled;
		_ui = ui;
		MilMo_EventSystem.Listen("level_explorationtoken_create", CreateExplorationIcons).Repeating = true;
		MilMo_EventSystem.Listen("level_cointoken_create", CreateCoinIcons).Repeating = true;
		MilMo_EventSystem.UnregisterPreRender(_renderUpdate);
		_renderUpdate = MilMo_EventSystem.RegisterPreRender(OnPreRender);
	}

	public void CustomizeNewAvatarFade()
	{
		CharBuilderFade(0f, isMakeOverStudio: false);
	}

	public async void MakeOverStudioFade()
	{
		CharBuilderFade(0f, isMakeOverStudio: true);
		Texture2D icon = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/WorldMap/IconMakeOver");
		await Task.Delay(250);
		SetMakeOverStudioIcon(icon);
	}

	public void CreateAvatarFade()
	{
		if (LoadingState != State.LoadChaBuilderNew)
		{
			SetArriveSound("");
			if (LoadingState != 0)
			{
				Hide();
			}
			MilMo_QuickInfoDialogHandler.GetInstance().PauseQueue();
			LoadingState = State.LoadChaBuilderNew;
			SetLoadingText(MilMo_Localization.GetLocString("CharBuilder_12730"));
			MilMo_Fade.Instance.FadeInBackground(delayFadeWidgets: true, resetAlpha: false);
			_progressBar = new MilMo_LoadingBar(_ui);
			_ui.AddChild(_progressBar);
		}
	}

	private void CharBuilderFade(float timeout, bool isMakeOverStudio)
	{
		if (LoadingState == State.LoadCharBuilder)
		{
			return;
		}
		SetArriveSound("");
		bool resetAlpha = true;
		if (LoadingState != 0)
		{
			Hide();
			resetAlpha = false;
		}
		MilMo_QuickInfoDialogHandler.GetInstance().PauseQueue();
		SetLoadingText(isMakeOverStudio ? MilMo_Localization.GetLocString("CharBuilder_4787") : MilMo_Localization.GetLocString("CharBuilder_57"));
		MilMo_Fade.Instance.FadeInBackground(delayFadeWidgets: true, resetAlpha);
		_progressBar = new MilMo_LoadingBar(_ui);
		_ui.AddChild(_progressBar);
		MilMo_EventSystem.At(0.1f, CharbuilderInitWait);
		if (timeout > 0f)
		{
			_fadeTimer = MilMo_EventSystem.At(2f, delegate
			{
				Hide(isMakeOverStudio);
			});
		}
		LoadingState = State.LoadCharBuilder;
	}

	public void LevelLoadFade(float timeout = 0f, string arriveSound = "", bool forceNoResetAlpha = false)
	{
		if (LoadingState == State.LoadLevel)
		{
			SetArriveSound(arriveSound);
			return;
		}
		_explorationTokens = null;
		_coinTokens = null;
		SetArriveSound(arriveSound);
		bool resetAlpha = !forceNoResetAlpha;
		if (LoadingState != 0)
		{
			Hide();
			resetAlpha = false;
		}
		MilMo_QuickInfoDialogHandler.GetInstance().PauseQueue();
		SetLoadingText(MilMo_LocString.Empty);
		MilMo_EventSystem.At(1f, delegate
		{
			if (MilMo_World.Instance != null)
			{
				if (MilMo_World.Instance.PlayerController != null)
				{
					MilMo_World.Instance.PlayerController.Lock(0f, playMoveAnimationOnUnlock: true);
				}
				if (MilMo_World.Instance.UI != null)
				{
					MilMo_World.Instance.UI.Enabled = false;
				}
			}
			else
			{
				Debug.LogWarning("MilMo_World is null when disabling GUI for level fade");
			}
		});
		MilMo_Fade.Instance.FadeInBackground(delayFadeWidgets: true, resetAlpha);
		_progressBar = new MilMo_LoadingBar(_ui);
		_ui.AddChild(_progressBar);
		if (timeout > 0f)
		{
			_fadeTimer = MilMo_EventSystem.At(timeout, delegate
			{
				Hide();
			});
		}
		LoadingState = State.LoadLevel;
	}

	public async void LoadHomeFade(float timeout, string homeOwnerName)
	{
		if (LoadingState == State.LoadHome)
		{
			return;
		}
		SetArriveSound("");
		Texture2D icon = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/Homes/IconMyHome");
		await Task.Delay(250);
		SetHomesIcon(icon);
		bool resetAlpha = true;
		if (LoadingState != 0)
		{
			Hide();
			resetAlpha = false;
		}
		MilMo_QuickInfoDialogHandler.GetInstance().PauseQueue();
		MilMo_LocString milMo_LocString;
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.Avatar == null || homeOwnerName == MilMo_Player.Instance.Avatar.Name)
		{
			milMo_LocString = MilMo_Localization.GetLocString("Homes_6820");
		}
		else
		{
			milMo_LocString = MilMo_Localization.GetLocString("Homes_6821").GetCopy();
			milMo_LocString.SetFormatArgs(homeOwnerName);
		}
		SetLoadingText(milMo_LocString);
		MilMo_EventSystem.At(1f, delegate
		{
			if (MilMo_World.Instance != null)
			{
				if (MilMo_World.Instance.PlayerController != null)
				{
					MilMo_World.Instance.PlayerController.Lock(0f, playMoveAnimationOnUnlock: true);
				}
				if (MilMo_World.Instance.UI != null)
				{
					MilMo_World.Instance.UI.Enabled = false;
				}
			}
			else
			{
				Debug.LogWarning("MilMo_World is null when disabling GUI for level fade");
			}
		});
		MilMo_Fade.Instance.FadeInBackground(delayFadeWidgets: true, resetAlpha);
		_progressBar = new MilMo_LoadingBar(_ui);
		_ui.AddChild(_progressBar);
		if (timeout > 0f)
		{
			_fadeTimer = MilMo_EventSystem.At(timeout, delegate
			{
				Hide();
			});
		}
		_fadeOutSound = null;
		_loadingFadeOutSound = false;
		LoadingState = State.LoadHome;
	}

	public void IntraTeleportFade(float timeout, string arriveSound, bool fadeToWhite)
	{
		if (LoadingState == State.IntraTeleport)
		{
			SetArriveSound(arriveSound);
			return;
		}
		bool resetAlpha = true;
		if (LoadingState != 0)
		{
			Hide();
			resetAlpha = false;
		}
		MilMo_QuickInfoDialogHandler.GetInstance().PauseQueue();
		SetArriveSound(arriveSound);
		SetLoadingText(MilMo_LocString.Empty);
		if (MilMo_World.Instance != null && MilMo_World.Instance.PlayerController != null)
		{
			MilMo_World.Instance.PlayerController.Lock(0f, playMoveAnimationOnUnlock: true);
		}
		MilMo_EventSystem.At(1f, delegate
		{
			if (MilMo_World.Instance != null && MilMo_World.Instance.UI != null)
			{
				MilMo_World.Instance.UI.Enabled = false;
			}
		});
		MilMo_Fade.Instance.FadeInBackground(fadeToWhite ? Color.white : Color.black, delayFadeWidgets: true, resetAlpha);
		if (timeout > 0f)
		{
			_fadeTimer = MilMo_EventSystem.At(timeout, delegate
			{
				Hide();
			});
		}
		LoadingState = State.IntraTeleport;
	}

	public void LoadRoomFade(float timeout, string arriveSound, bool fadeToWhite = false)
	{
		if (LoadingState == State.LoadRoom)
		{
			SetArriveSound(arriveSound);
			return;
		}
		bool resetAlpha = true;
		if (LoadingState != 0)
		{
			Hide();
			resetAlpha = false;
		}
		MilMo_QuickInfoDialogHandler.GetInstance().PauseQueue();
		SetArriveSound(arriveSound);
		SetLoadingText(MilMo_LocString.Empty);
		if (MilMo_World.Instance != null && MilMo_World.Instance.PlayerController != null)
		{
			MilMo_World.Instance.PlayerController.Lock(0f, playMoveAnimationOnUnlock: true);
		}
		MilMo_EventSystem.At(1f, delegate
		{
			if (MilMo_World.Instance != null && MilMo_World.Instance.UI != null)
			{
				MilMo_World.Instance.UI.Enabled = false;
			}
		});
		MilMo_Fade.Instance.FadeInBackground(fadeToWhite ? Color.white : Color.black, delayFadeWidgets: true, resetAlpha);
		if (timeout > 0f)
		{
			_fadeTimer = MilMo_EventSystem.At(timeout, delegate
			{
				Hide();
			});
		}
		LoadingState = State.LoadRoom;
	}

	public void RespawnFade(float timeout)
	{
		if (LoadingState == State.Respawn)
		{
			return;
		}
		SetArriveSound("");
		bool resetAlpha = true;
		if (LoadingState != 0)
		{
			Hide();
			resetAlpha = false;
		}
		SetLoadingText(MilMo_LocString.Empty);
		MilMo_EventSystem.At(1f, delegate
		{
			if (MilMo_World.Instance != null && MilMo_World.Instance.UI != null)
			{
				MilMo_World.Instance.UI.Enabled = false;
			}
		});
		MilMo_Fade.Instance.FadeInBackground(delayFadeWidgets: true, resetAlpha);
		if (timeout > 0f)
		{
			_fadeTimer = MilMo_EventSystem.At(timeout, delegate
			{
				Hide();
			});
		}
		LoadingState = State.Respawn;
	}

	public async void LoadShopFade()
	{
		if (LoadingState != State.LoadShop)
		{
			SetArriveSound("");
			MilMo_QuickInfoDialogHandler.GetInstance().PauseQueue();
			bool resetAlpha = true;
			if (LoadingState != 0)
			{
				Hide();
				resetAlpha = false;
			}
			LoadingState = State.LoadShop;
			_shopProgress = 0;
			SetLoadingText(MilMo_Localization.GetLocString("CharacterShop_270"));
			Texture2D shopIcon = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/HUD/IconShop");
			await Task.Delay(250);
			SetShopIcon(shopIcon);
			if (MilMo_World.Instance != null && MilMo_World.Instance.PlayerController != null)
			{
				MilMo_World.Instance.PlayerController.Lock(0f, playMoveAnimationOnUnlock: true);
			}
			MilMo_Fade.Instance.FadeInBackground(delayFadeWidgets: true, resetAlpha);
			_progressBar = new MilMo_LoadingBar(_ui);
			_ui.AddChild(_progressBar);
			ShopInitWait();
		}
	}

	public async void LoadTownFade()
	{
		if (LoadingState != State.LoadTown)
		{
			SetArriveSound("");
			MilMo_QuickInfoDialogHandler.GetInstance().PauseQueue();
			if (LoadingState != 0)
			{
				Hide();
			}
			SetLoadingText(MilMo_Localization.GetLocString("World_8823"));
			Texture2D icon = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/Homes/IconMyHome");
			await Task.Delay(250);
			SetTownIcon(icon);
			_progressBar = new MilMo_LoadingBar(_ui);
			_ui.AddChild(_progressBar);
			if (MilMo_World.Instance != null && MilMo_World.Instance.PlayerController != null)
			{
				MilMo_World.Instance.PlayerController.Lock(0f, playMoveAnimationOnUnlock: true);
			}
			MilMo_Fade.Instance.FadeInAll();
			LoadingState = State.LoadTown;
		}
	}

	public async void LoadNavigatorFade()
	{
		if (LoadingState != State.LoadNavigator)
		{
			SetArriveSound("");
			MilMo_QuickInfoDialogHandler.GetInstance().PauseQueue();
			if (LoadingState != 0)
			{
				Hide();
			}
			SetLoadingText(MilMo_Localization.GetLocString("WorldMap_4748"));
			Texture2D icon = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/HUD/IconWorlds");
			await Task.Delay(250);
			SetNavigatorIcon(icon);
			_progressBar = new MilMo_LoadingBar(_ui);
			_ui.AddChild(_progressBar);
			if (MilMo_World.Instance != null && MilMo_World.Instance.PlayerController != null)
			{
				MilMo_World.Instance.PlayerController.Lock(0f, playMoveAnimationOnUnlock: true);
			}
			MilMo_Fade.Instance.FadeInAll();
			LoadingState = State.LoadNavigator;
		}
	}

	public void Hide(bool enableWorldUI = true)
	{
		Hide(isTimeoutEvent: false, enableWorldUI);
	}

	private void Hide(bool isTimeoutEvent, bool enableWorldUI)
	{
		if (LoadingState == State.Disabled)
		{
			return;
		}
		MilMo_Fade.Instance.FadeOutAll();
		if (_fadeOutSound != null)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(_fadeOutSound);
			_fadeOutSound = null;
		}
		if (_progressBar != null)
		{
			_ui.RemoveChild(_progressBar);
			_progressBar = null;
		}
		if (_tokenIcons != null)
		{
			_ui.RemoveChild(_tokenIcons);
			_tokenIcons = null;
		}
		if (_text != null)
		{
			_ui.RemoveChild(_text);
			_text = null;
		}
		if (_icon != null)
		{
			_ui.RemoveChild(_icon);
			_icon = null;
		}
		if (_premiumIcon != null)
		{
			_premiumIcon.SetInactive();
			_ui.RemoveChild(_premiumIcon);
			_premiumIcon = null;
		}
		MilMo_EventSystem.At(0.4f, delegate
		{
			if (!(MilMo_World.Instance == null))
			{
				if (enableWorldUI && MilMo_World.Instance.UI != null)
				{
					MilMo_World.Instance.UI.Enabled = true;
				}
				if (MilMo_World.Instance.PlayerController != null)
				{
					MilMo_World.Instance.PlayerController.Unlock();
				}
			}
		});
		if (!isTimeoutEvent)
		{
			CancelTimeout();
		}
		else
		{
			_fadeTimer = null;
		}
		LoadingState = State.Disabled;
		MilMo_QuickInfoDialogHandler.GetInstance().ResumeQueue();
	}

	private void SetArriveSound(string arriveSound)
	{
		if (!string.IsNullOrEmpty(arriveSound))
		{
			_loadingFadeOutSound = true;
			_fadeOutSound = null;
			LoadArriveSoundAsync(arriveSound);
		}
		else
		{
			_loadingFadeOutSound = false;
			_fadeOutSound = null;
		}
	}

	private async void LoadArriveSoundAsync(string arriveSound)
	{
		AudioClip fadeOutSound = await MilMo_ResourceManager.Instance.LoadAudioAsync(arriveSound);
		if (_loadingFadeOutSound)
		{
			_fadeOutSound = fadeOutSound;
		}
	}

	public void CancelTimeout()
	{
		if (_fadeTimer != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_fadeTimer);
			_fadeTimer = null;
		}
	}

	public void LevelIconLoaded(Texture2D icon)
	{
		if (LoadingState == State.LoadLevel)
		{
			SetLevelIcon(icon);
			if (_progressBar != null)
			{
				_progressBar.CurrentProgress += 0.02f;
			}
		}
	}

	public void PropsFileLoaded(MilMo_SFFile file)
	{
		if (LoadingState == State.LoadLevel)
		{
			CalculatePropFraction(file);
			if (_progressBar != null)
			{
				_progressBar.CurrentProgress += 0.02f;
			}
		}
	}

	public void TerrainDataLoaded()
	{
		if (LoadingState == State.LoadLevel && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.08f;
		}
	}

	public void GroundMaterials01Loaded()
	{
		if (LoadingState == State.LoadLevel && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.04f;
		}
	}

	public void GroundMaterials02Loaded()
	{
		if (LoadingState == State.LoadLevel && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.026f;
		}
	}

	public void EnvironmentLoaded()
	{
		if (LoadingState == State.LoadLevel && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.002f;
		}
	}

	public void PreStreamListLoaded()
	{
		if (LoadingState == State.LoadLevel && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.002f;
		}
	}

	public void GroundMaterialsFileLoaded()
	{
		if (LoadingState == State.LoadLevel && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.002f;
		}
	}

	public void MusicLoaded()
	{
		if (LoadingState == State.LoadLevel && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.002f;
		}
	}

	public void MusicAreasLoaded()
	{
		if (LoadingState == State.LoadLevel && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.002f;
		}
	}

	public void SoundAreasLoaded()
	{
		if (LoadingState == State.LoadLevel && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.002f;
		}
	}

	public void ClimbingSurfacesLoaded()
	{
		if (LoadingState == State.LoadLevel && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.002f;
		}
	}

	public void PropLoaded()
	{
		if (LoadingState == State.LoadLevel && _progressBar != null)
		{
			_progressBar.CurrentProgress += _propFraction;
		}
	}

	public void GotHomeEquipmentCount(int count)
	{
		if (LoadingState == State.LoadHome)
		{
			_homeEquipmentFraction = 1f / (float)count;
		}
	}

	public void HomeEquipmentLoaded()
	{
		if (LoadingState == State.LoadHome && _progressBar != null)
		{
			_progressBar.CurrentProgress += _homeEquipmentFraction;
		}
	}

	public void GotTownPropsCount(int count)
	{
		if (LoadingState == State.LoadTown)
		{
			_townPropFraction = 0.75f / (float)count;
		}
	}

	public void TownPropLoaded()
	{
		if (LoadingState == State.LoadTown && _progressBar != null)
		{
			_progressBar.CurrentProgress += _townPropFraction;
		}
	}

	public void PlayerLoaded()
	{
		if (LoadingState == State.LoadTown && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.25f;
		}
	}

	public void CharbuilderDataLoaded()
	{
		if (LoadingState == State.LoadCharBuilder && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.137f;
		}
	}

	public void CharbuilderSyncLoadedObjectsDone()
	{
		if (LoadingState == State.LoadCharBuilder && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.023f;
		}
	}

	public void CharbuilderFirstBodyDone()
	{
		if (LoadingState == State.LoadCharBuilder && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.202f;
		}
	}

	public void CharbuilderSecondBodyDone()
	{
		if (LoadingState == State.LoadCharBuilder && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.024f;
		}
	}

	public void CharbuilderFirstGroundImpact()
	{
		if (LoadingState == State.LoadCharBuilder && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.121f;
			MilMo_EventSystem.At(0.1f, CharbuilderFinalWait);
		}
	}

	private void CharbuilderInitWait()
	{
		if (LoadingState == State.LoadCharBuilder && _progressBar != null)
		{
			float num = 0.00365f;
			if (_charbuilderInitWaitProgress + num >= 0.219f)
			{
				num = 0.219f - _charbuilderInitWaitProgress;
			}
			else
			{
				MilMo_EventSystem.At(0.1f, CharbuilderInitWait);
			}
			_progressBar.CurrentProgress += num;
			_charbuilderInitWaitProgress += num;
		}
	}

	private void CharbuilderFinalWait()
	{
		if (LoadingState == State.LoadCharBuilder && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.0137f;
			MilMo_EventSystem.At(0.1f, CharbuilderFinalWait);
		}
	}

	public void NewCharbuilderBoyTemplatesLoaded()
	{
		if (LoadingState == State.LoadChaBuilderNew && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.1f;
		}
	}

	public void NewCharbuilderGirlTemplatesLoaded()
	{
		if (LoadingState == State.LoadChaBuilderNew && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.1f;
		}
	}

	public void NewCharbuilderSyncLoadedObjectsDone()
	{
		if (LoadingState == State.LoadChaBuilderNew && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.1f;
		}
	}

	public void NewCharbuilderBoyAvatarInitialized()
	{
		if (LoadingState == State.LoadChaBuilderNew && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.15f;
		}
	}

	public void NewCharbuilderGirlAvatarInitialized()
	{
		if (LoadingState == State.LoadChaBuilderNew && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.15f;
		}
	}

	public void NewCharbuilderBoyAvatarDressed()
	{
		if (LoadingState == State.LoadChaBuilderNew && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.2f;
		}
	}

	public void NewCharbuilderGirlAvatarDressed()
	{
		if (LoadingState == State.LoadChaBuilderNew && _progressBar != null)
		{
			_progressBar.CurrentProgress += 0.2f;
		}
	}

	private void ShopInitWait()
	{
		if (LoadingState == State.LoadShop && _progressBar != null && _shopProgress <= 0 && !(_progressBar.CurrentProgress > 0.375f))
		{
			_progressBar.CurrentProgress += 0.025f;
			MilMo_EventSystem.At(0.1f, ShopInitWait);
		}
	}

	public void ShopActivationWait()
	{
		if (LoadingState != State.LoadShop || _progressBar == null || _shopProgress > 1)
		{
			return;
		}
		_shopProgress = 1;
		if (!(_progressBar.CurrentProgress > 0.5f))
		{
			if (_progressBar.CurrentProgress < 0.375f)
			{
				_progressBar.CurrentProgress = 0.375f;
			}
			_progressBar.CurrentProgress += 0.025f;
			MilMo_EventSystem.At(0.1f, ShopActivationWait);
		}
	}

	public void ShopFinalWait()
	{
		if (LoadingState != State.LoadShop || _progressBar == null)
		{
			return;
		}
		_shopProgress = 2;
		if (!(_progressBar.CurrentProgress > 1f))
		{
			if (_progressBar.CurrentProgress < 0.5f)
			{
				_progressBar.CurrentProgress = 0.5f;
			}
			_progressBar.CurrentProgress = 0.5f + 0.5f * ((Time.realtimeSinceStartup - (float)_shopProgress) / 2f);
			MilMo_EventSystem.At(0.05f, ShopFinalWait);
		}
	}

	private void CalculatePropFraction(MilMo_SFFile propsFile)
	{
		if (propsFile == null)
		{
			if (_progressBar != null)
			{
				_progressBar.CurrentProgress += 0.8f;
			}
			return;
		}
		float num = 0f;
		while (propsFile.NextRow())
		{
			if (propsFile.IsNext("<PROP>"))
			{
				num += 1f;
				while (propsFile.NextRow() && !propsFile.IsNext("</PROP>"))
				{
				}
			}
		}
		propsFile.Reset();
		_propFraction = 0.8f / num;
	}

	public void SetLoadingText(MilMo_LocString locString)
	{
		if (_text == null)
		{
			_text = new MilMo_Widget(_ui);
			_text.FadeToDefaultColor = false;
			_text.SetFont(MilMo_GUI.Font.EborgMedium);
			_text.SetTexture("Batch01/Textures/Core/Invisible");
			_text.SetPosition((float)Screen.width * 0.5f + MilMo_LoadingScreenConf.TextCenterOffset.x, (float)Screen.height * 0.5f + MilMo_LoadingScreenConf.TextCenterOffset.y);
			_text.SetScale(Screen.width, 30f);
			_text.SetAlignment(MilMo_GUI.Align.CenterCenter);
			_text.SetDefaultTextColor(1f, 1f, 1f, 1f);
			_text.SetTextColor(1f, 1f, 1f, 1f);
			_text.Enabled = true;
			_text.IgnoreGlobalFade = true;
			_text.AllowPointerFocus = false;
			_text.SetAlpha(0f);
			_text.AlphaTo(1f);
			_text.SetFadeSpeed(0.05f);
			_ui.AddChild(_text);
		}
		_text.SetText(locString);
	}

	private void SetLevelIcon(Texture icon)
	{
		if (LoadingState == State.LoadLevel)
		{
			SetIcon(icon);
		}
	}

	private void SetTownIcon(Texture icon)
	{
		if (LoadingState == State.LoadTown)
		{
			SetIcon(icon);
		}
	}

	private void SetNavigatorIcon(Texture icon)
	{
		if (LoadingState == State.LoadNavigator)
		{
			SetIcon(icon);
		}
	}

	private void SetShopIcon(Texture icon)
	{
		if (LoadingState == State.LoadShop)
		{
			SetIcon(icon);
		}
	}

	private void SetMakeOverStudioIcon(Texture icon)
	{
		if (LoadingState == State.LoadCharBuilder)
		{
			SetIcon(icon);
		}
	}

	private void SetHomesIcon(Texture icon)
	{
		if (LoadingState == State.LoadHome)
		{
			SetIcon(icon);
		}
	}

	private void SetIcon(Texture icon)
	{
		if (_icon == null)
		{
			_icon = new MilMo_Widget(_ui);
			_icon.SetTexture(new MilMo_Texture(icon));
			_icon.SetPosition((float)Screen.width * 0.5f + MilMo_LoadingScreenConf.IconCenterOffset.x, (float)Screen.height * 0.5f + MilMo_LoadingScreenConf.IconCenterOffset.y);
			_icon.SetScale(64f, 64f);
			_icon.SetAlignment(MilMo_GUI.Align.CenterCenter);
			_icon.Enabled = true;
			_icon.IgnoreGlobalFade = true;
			_icon.AllowPointerFocus = false;
			_icon.SetAlpha(0f);
			_icon.AlphaTo(1f);
			_icon.SetFadeSpeed(0.05f);
			_ui.AddChild(_icon);
		}
		_icon.SetTexture(new MilMo_Texture(icon));
	}

	public void SetPremiumIcon(PremiumToken token)
	{
		if (LoadingState == State.LoadLevel && MilMo_Player.Instance.IsMember && token != null)
		{
			if (_premiumIcon == null)
			{
				_premiumIcon = new MilMo_PremiumIcon(_ui);
				_ui.AddChild(_premiumIcon);
			}
			if (token.GetProgress() >= 1f)
			{
				_premiumIcon.SetActive(isActive: true, token.GetValue(), 0f);
			}
			else
			{
				_premiumIcon.SetActive(isActive: false, 0, token.GetProgress());
			}
		}
	}

	private void CreateExplorationIcons(object msgAsObj)
	{
		if (LoadingState == State.LoadLevel)
		{
			_explorationTokens = msgAsObj as ServerExplorationTokensCreate;
			ShowExplorationTokenOrCoinIcons();
		}
	}

	private void CreateCoinIcons(object msgAsObj)
	{
		if (LoadingState == State.LoadLevel)
		{
			_coinTokens = msgAsObj as ServerCoinTokensCreate;
			ShowExplorationTokenOrCoinIcons();
		}
	}

	private void ShowExplorationTokenOrCoinIcons()
	{
		if (_explorationTokens != null && _coinTokens != null)
		{
			_tokenIcons = new MilMo_TokenIcons(_ui, _explorationTokens.getExplorationTokens(), _coinTokens.getCoinTokens());
			_ui.AddChild(_tokenIcons);
		}
	}
}
