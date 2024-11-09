using System;
using System.Collections.Generic;
using System.Linq;
using Code.Apps.Fade;
using Code.Core.Avatar;
using Code.Core.BodyPack;
using Code.Core.Camera;
using Code.Core.Config;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.Input;
using Code.Core.Items;
using Code.Core.Items.Home;
using Code.Core.Items.Home.FurnitureGrid;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.HomeSurface;
using Code.Core.Monetization;
using Code.Core.Network;
using Code.Core.Network.nexus;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.Core.Visual;
using Code.World.CharacterShop.RemoteShop;
using Code.World.CharBuilder;
using Code.World.GUI;
using Code.World.Home;
using Code.World.Player;
using Core;
using Core.Analytics;
using Core.State;
using UnityEngine;

namespace Code.World.CharacterShop;

public sealed class MilMo_CharacterShop : MonoBehaviour
{
	private static class BuyFailCode
	{
		public const sbyte NOT_ENOUGH_JUNE_COINS = 1;

		public const sbyte ALREADY_HAS_ITEM = 2;

		public const sbyte GENDER_MISS_MATCH = 10;

		public const sbyte HAS_MAX_AMOUNT_ITEM = 11;
	}

	private enum ViewMode
	{
		Character,
		Furniture,
		Wallpaper,
		Floor
	}

	public delegate void ShopReady(bool success);

	private readonly List<MilMo_CharacterShopCategory> _categoryList = new List<MilMo_CharacterShopCategory>();

	private int _numMainCats;

	private int _curBottomCat;

	private float _hotItemScrollVal;

	private MilMo_ScrollView _mainCatScroller;

	private MilMo_ScrollView _subCatScroller;

	private MilMo_ScrollView _bottomScroller;

	private float _mainCatIconHeight;

	public const string UI_IDENTIFIER = "CharacterShop";

	public const byte SHOP_REQUEST_SUCCESS = 0;

	public const byte SHOP_REQUEST_FAIL = 1;

	private static MilMo_VisualRep _thePlatform;

	private static MilMo_VisualRep _theBackdrop;

	private static MilMo_VisualRep _theShopClouds;

	private static MilMo_VisualRep _theWallpaperObject;

	private static MilMo_VisualRep _theFloorObject;

	private static MilMo_VisualRep _theWallObject;

	private static readonly Vector3 WallObjectScaleWallItems = new Vector3(100f, 100f, 1f);

	private static readonly Vector3 WallObjectScaleRoom = new Vector3(0.7f, 6f, 1f);

	private static readonly Vector3 WallObjectPositionRoom = new Vector3(0f, -320f, 20f);

	private static readonly Quaternion WallObjectRotationRoom = Quaternion.Euler(0f, 180f, 0f);

	private static MilMo_CharacterShop _theCharacterShop;

	private MilMo_RemoteShop _remoteShop;

	private MilMo_UserInterface _ui;

	private GameObject _character;

	private readonly List<MilMo_ObjectMover> _allMovers = new List<MilMo_ObjectMover>();

	private readonly MilMo_ObjectMover _characterMover = new MilMo_ObjectMover();

	private readonly MilMo_ObjectMover _wallpaperMover = new MilMo_ObjectMover();

	private readonly MilMo_ObjectMover _floorMover = new MilMo_ObjectMover();

	private MilMo_ObjectMover _mover;

	private float _objectAngle;

	private bool _platformHooked;

	private bool _currentObjectMountedInWall;

	private static MilMo_ShopEmoteHandler _shopEmoteHandler;

	private MilMo_GenericReaction _memberStatusReaction;

	private GameObject _shopLight;

	private GameObject _shopLightHomeObjects;

	private readonly Color _defaultAmbientColor = new Color(0.2f, 0.2f, 0.2f);

	private int _curCaptionColor;

	private bool _allowBrowsingItemCards = true;

	private bool _goingBackToGame;

	private byte _currentGender;

	private ViewMode _viewMode;

	private MilMo_Button _tabLeftArrow;

	private MilMo_Button _tabRightArrow;

	private MilMo_Button _iconLeftArrow;

	private MilMo_Button _iconRightArrow;

	private MilMo_Button _mChargeButton;

	private MilMo_Button _backButton;

	private MilMo_Widget _mainCaption;

	private MilMo_Widget _ingameCoinsIcon;

	private MilMo_Widget _currencyIcon;

	private MilMo_Widget _juneCoinSign;

	private MilMo_Widget _accountBalance;

	private MilMo_Widget _ingameCoinsAccountBalance;

	private MilMo_Widget _backArrow;

	private MilMo_Widget _middleIcon;

	private MilMo_Widget _charSpinner;

	private MilMo_Widget _topBack;

	private MilMo_Widget _coverFlowGradient;

	private MilMo_Widget _back1;

	private MilMo_Widget _back2;

	private MilMo_Widget _back3;

	private MilMo_Widget _back4;

	private MilMo_Widget _back5;

	private MilMo_Widget _back6;

	private MilMo_Widget _back7;

	private MilMo_Widget _purpleBack;

	private MilMo_Widget _purpleBackRight;

	private MilMo_Widget _helpReflect;

	private MilMo_Widget _captionBar3;

	private MilMo_Widget _inventoryBack;

	private MilMo_Widget _inventoryShade;

	private MilMo_Widget _invLeftShade;

	private MilMo_Widget _invRightShade;

	private MilMo_Widget _backToGameBack;

	public MilMo_Widget helpCaption;

	public MilMo_Widget helpText;

	public MilMo_Widget descPriceTag;

	public MilMo_Widget mainCaptionBack;

	public MilMo_Widget captionShine;

	public MilMo_Widget topDiv;

	private static MilMo_Widget _returnToLevelIcon;

	private static MilMo_Button _returnToLevelName;

	private MilMo_SoftFrame _hotItemsFrame;

	private MilMo_Widget _fadeout;

	private static Color[] _captionColor;

	private MilMo_Button _colorsButton;

	private MilMo_Button _hotItemsButton;

	private MilMo_PulseCursor _pulseCursor;

	private MilMo_Widget _fadeTop;

	private MilMo_Widget _fadeBottom;

	private MilMo_Widget _captionCharge;

	private MilMo_Widget _chargeBack;

	private MilMo_Widget _leftFade;

	private MilMo_Widget _rightFade;

	private MilMo_Widget _bottomFade;

	private MilMo_Widget _topFade;

	private MilMo_LoadingPane _loadingPane;

	public MilMoGiftMenu giftMenu;

	private MilMo_Widget _selectMenu;

	private MilMo_Button _selectedBuyButton;

	private MilMo_Button _selectedGiftButton;

	private MilMo_Button _selectedRemoveButton;

	private readonly List<MilMo_Item> _currentlyHideFromSelection = new List<MilMo_Item>();

	private static ShopReady _readyCallback;

	private bool _isBuying;

	private string _lastGiftFriendId;

	private string _lastGiftFriendAvatarName;

	private readonly MilMo_TimerEvent _junecoinsUpdater;

	private int _currentJuneCashValue;

	private int _wantedJuneCashValue;

	private float _juneCashUpdateTime;

	private int _currentIngameCoinsValue;

	private int _wantedIngameCoinsValue;

	private MilMo_EventSystem.MilMo_Callback _pushcolors;

	private MilMo_EventSystem.MilMo_Callback _pushhotitems;

	private MilMo_EventSystem.MilMo_Callback _allowBrowsingItemCardsCallback;

	private MilMo_EventSystem.MilMo_Callback _allowSelectMenuInput;

	private MilMo_TimerEvent _allowSelectMenuInputTimer;

	private MilMo_TimerEvent _autoEmote;

	private readonly Vector2 _arrowSize = new Vector2(16f, 32f);

	public MilMo_ItemInfoBox infoBox;

	private static string _itemToSelectOnEnter = "";

	private MilMo_ShopItem _selectedItem;

	public List<MilMo_Button> tryButtonList = new List<MilMo_Button>();

	public MilMo_AudioClip tickSound = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/Generic/Tick");

	public MilMo_AudioClip pickSound = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/Generic/Pick");

	public MilMo_AudioClip softTickSound = new MilMo_AudioClip("Content/Sounds/Batch01/CharacterShop/SoftTick");

	public MilMo_AudioClip juneCoinTickSound = new MilMo_AudioClip("Content/Sounds/Batch01/CharacterShop/JuneCoinTick");

	private float _juneCoinTickSoundDelay;

	private static MilMo_LocString _levelName;

	private static Texture2D _levelIcon;

	public int screenWidth = 1024;

	public int screenHeight = 720;

	public MilMo_CharacterShopCategory CurCategory { get; private set; }

	public static bool PreloadedAssetsDone { get; private set; }

	public static void AsyncLoadVisualReps()
	{
		MilMo_VisualRepContainer.AsyncCreateVisualRep("Content/CharacterShop/EarthChunk", new Vector3(0f, 1f, 0f) + MilMo_ShopCameraController.ShopPosition, Quaternion.Euler(new Vector3(0f, 180f, 0f)), "Shop", delegate(MilMo_VisualRep visualRep)
		{
			if (visualRep != null)
			{
				_thePlatform = visualRep;
				_thePlatform.GameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
				if (_theCharacterShop == null || !_theCharacterShop.enabled)
				{
					_thePlatform.Disable();
				}
			}
		});
		MilMo_VisualRepContainer.AsyncCreateVisualRep("Content/CharacterShop/Wallpaper", null, Vector3.zero, Quaternion.identity, Vector3.one, "Shop", setDefaultMaterialTexture: false, waitForMaterial: false, delegate(MilMo_VisualRep visualRep)
		{
			if (visualRep != null)
			{
				_theWallpaperObject = visualRep;
				_theWallpaperObject.GameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				if (_theCharacterShop == null || !_theCharacterShop.enabled)
				{
					_theWallpaperObject.Disable();
				}
			}
		});
		MilMo_VisualRepContainer.AsyncCreateVisualRep("Content/CharacterShop/Floor", null, Vector3.zero, Quaternion.identity, Vector3.one, "Shop", setDefaultMaterialTexture: false, waitForMaterial: false, delegate(MilMo_VisualRep visualRep)
		{
			if (visualRep != null)
			{
				_theFloorObject = visualRep;
				_theFloorObject.GameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
				if (_theCharacterShop == null || !_theCharacterShop.enabled)
				{
					_theFloorObject.Disable();
				}
			}
		});
		MilMo_VisualRepContainer.AsyncCreateVisualRep("Content/CharacterShop/BlackWall", null, Vector3.zero, Quaternion.identity, Vector3.one, "Shop", setDefaultMaterialTexture: false, waitForMaterial: false, delegate(MilMo_VisualRep visualRep)
		{
			if (visualRep != null)
			{
				_theWallObject = visualRep;
				_theWallObject.GameObject.transform.localScale = new Vector3(100f, 100f, 1f);
				if (_theCharacterShop == null || !_theCharacterShop.enabled)
				{
					_theWallObject.Disable();
				}
			}
		});
		MilMo_VisualRepContainer.AsyncCreateVisualRep("Content/CharacterShop/Backdrop", new Vector3(-215.1027f, -282.4604f, -128.5411f), Quaternion.Euler(new Vector3(-18.989f, 2.688f, -2.013f)), "Shop", delegate(MilMo_VisualRep visualRep)
		{
			if (visualRep != null)
			{
				_theBackdrop = visualRep;
				_theBackdrop.GameObject.transform.localScale = new Vector3(2f, 6f, 2f);
				_theBackdrop.GameObject.transform.parent = MilMo_Global.Camera.transform;
				if (_theCharacterShop == null || !_theCharacterShop.enabled)
				{
					_theBackdrop.Disable();
				}
			}
		});
		MilMo_VisualRepContainer.AsyncCreateVisualRep("Apps/CharBuilder/Content/Objects/CloudCylinder", new Vector3(12.15f, -120.56f, 56.77f) + MilMo_ShopCameraController.ShopPosition, Quaternion.Euler(new Vector3(349.6f, 54.5f, 343.9f)), "Shop", delegate(MilMo_VisualRep visualRep)
		{
			if (visualRep != null)
			{
				_theShopClouds = visualRep;
				_theShopClouds.GameObject.transform.localScale = new Vector3(-0.75f, 0.75f, 0.75f);
				_theShopClouds.GameObject.AddComponent<MilMo_Spin>().spin = new Vector3(0f, -0.02f, 0f);
				if (_theCharacterShop == null || !_theCharacterShop.enabled)
				{
					_theShopClouds.Disable();
				}
			}
		});
	}

	private static void UpdateVisualReps()
	{
		if (_thePlatform != null)
		{
			_thePlatform.Update();
		}
		if (_theBackdrop != null)
		{
			_theBackdrop.Update();
		}
		if (_theShopClouds != null)
		{
			_theShopClouds.Update();
		}
		if (_theWallpaperObject != null)
		{
			_theWallpaperObject.Update();
		}
		if (_theFloorObject != null)
		{
			_theFloorObject.Update();
		}
		if (_theWallObject != null)
		{
			_theWallObject.Update();
		}
	}

	private void InitLight()
	{
		_shopLight = new GameObject("ShopLight");
		Light light = _shopLight.AddComponent<Light>();
		light.type = LightType.Directional;
		light.shadows = LightShadows.None;
		light.renderMode = LightRenderMode.ForcePixel;
		light.color = Color.white;
		light.intensity = 1f;
		_shopLight.transform.eulerAngles = new Vector3(20f, 35f, 0f);
		RenderSettings.ambientLight = _defaultAmbientColor;
		_shopLightHomeObjects = new GameObject("ShopLightHomeObjects");
		Light light2 = _shopLightHomeObjects.AddComponent<Light>();
		light2.type = LightType.Directional;
		light2.shadows = LightShadows.None;
		light2.renderMode = LightRenderMode.ForcePixel;
		light2.color = MilMo_Home.DirectionalLightColor;
		light2.intensity = MilMo_Home.DirectionalLightIntensity;
		_shopLightHomeObjects.transform.eulerAngles = MilMo_Home.LightDirection;
		_shopLightHomeObjects.SetActive(value: false);
	}

	private static void InitCamera()
	{
	}

	public void InitCharacterMover()
	{
		_mover = _characterMover;
		_allMovers.Add(_characterMover);
		_allMovers.Add(_wallpaperMover);
		_allMovers.Add(_floorMover);
		_mover.Pull = 0.01f;
		_mover.Drag = 0.75f;
		_mover.GoToNow(new Vector3(0f, -69f, 1f) + MilMo_ShopCameraController.ShopPosition);
		_mover.GoToNow(new Vector3(0f, -50f, 1f) + MilMo_ShopCameraController.ShopPosition);
		_mover.SetUpdateFunc(2);
		_mover.AnglePull = 0.035f;
		_mover.AngleDrag = 0.6f;
		_mover.AngleNow(0f, 208f, 0f);
		_objectAngle = _mover.Angle.y;
		_mover.AttachObject(_character);
		_mover.UnPause();
	}

	private void InitObjects()
	{
		_character = MilMo_Player.Instance.Avatar.GameObject;
		if (_character == null)
		{
			Debug.LogWarning("Failed to init objects in character shop. Character doesn't exist.");
			return;
		}
		if (_thePlatform != null)
		{
			_thePlatform.Enable();
		}
		if (_theBackdrop != null)
		{
			_theBackdrop.Enable();
		}
		if (_theShopClouds != null)
		{
			_theShopClouds.Enable();
		}
		if (_theWallpaperObject != null)
		{
			_theWallpaperObject.Enable();
			if (!_wallpaperMover.IsAttached(_theWallpaperObject.GameObject))
			{
				_wallpaperMover.AttachObject(_theWallpaperObject.GameObject);
			}
			_wallpaperMover.GoToNow(new Vector3(3f, -50f, 1f) + MilMo_ShopCameraController.ShopPosition);
		}
		if (_theFloorObject != null)
		{
			_theFloorObject.Enable();
			if (!_floorMover.IsAttached(_theFloorObject.GameObject))
			{
				_floorMover.AttachObject(_theFloorObject.GameObject);
			}
			_floorMover.GoToNow(new Vector3(3f, -50f, 1f) + MilMo_ShopCameraController.ShopPosition);
		}
	}

	private void RefreshWallObject()
	{
		if (_theWallObject == null || !_theWallObject.GameObject)
		{
			return;
		}
		GameObject gameObject = null;
		if (_selectedItem != null && _selectedItem.Item is MilMo_Furniture)
		{
			gameObject = ((MilMo_Furniture)_selectedItem.Item).GameObject;
			if (!gameObject || gameObject.GetComponentsInChildren<Transform>().Length <= 1)
			{
				return;
			}
		}
		else if (_selectedItem != null && _selectedItem.Item is MilMo_ShopRoom)
		{
			gameObject = ((MilMo_ShopRoom)_selectedItem.Item).GameObject;
			if (!gameObject || gameObject.GetComponentsInChildren<Transform>().Length <= 1)
			{
				return;
			}
		}
		_theWallObject.GameObject.transform.parent = null;
		_theWallObject.GameObject.transform.localScale = WallObjectScaleWallItems;
		_currentObjectMountedInWall = false;
		_theWallObject.Disable();
		if (gameObject == null || (!(_selectedItem.Item is MilMo_WallFurniture) && !(_selectedItem.Item is MilMo_ShopRoom)))
		{
			return;
		}
		if (_selectedItem.Item is MilMo_ShopRoom)
		{
			_theWallObject.Enable();
			_theWallObject.GameObject.transform.position = WallObjectPositionRoom;
			_theWallObject.GameObject.transform.localScale = WallObjectScaleRoom;
			_theWallObject.GameObject.transform.rotation = WallObjectRotationRoom;
			return;
		}
		MilMo_VisualRepComponent[] componentsInChildren = gameObject.GetComponentsInChildren<MilMo_VisualRepComponent>();
		MilMo_VisualRep milMo_VisualRep = null;
		MilMo_VisualRepComponent[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			MilMo_VisualRep visualRep = array[i].GetVisualRep();
			if (visualRep != null && visualRep.MeshHeld != null)
			{
				milMo_VisualRep = visualRep;
			}
		}
		if (milMo_VisualRep != null)
		{
			_theWallObject.Enable();
			_theWallObject.GameObject.transform.parent = gameObject.transform;
			_theWallObject.GameObject.transform.localPosition = new Vector3(1f, -50f, 0f);
			_theWallObject.GameObject.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
			_currentObjectMountedInWall = true;
		}
	}

	private static void PlayIntroEmote(MilMo_Avatar avatar, string unused2)
	{
		_shopEmoteHandler.PlayIntroEmote(avatar);
	}

	public void PlayIdleAnimation(MilMo_Avatar avatar)
	{
		_shopEmoteHandler.PlayIdleAnimation(avatar, "");
	}

	private static void PlayFirstEmote(MilMo_Avatar avatar, string category)
	{
		_shopEmoteHandler.PlayFirstEmote(avatar, category);
	}

	private static void PlayAutoEmote(MilMo_Avatar avatar, string category)
	{
		_shopEmoteHandler.PlayAutoEmote(avatar, category);
	}

	public void ScheduleAutoEmote()
	{
		if (MilMo_Config.Instance.IsTrue("Launcher.OfflineShop", defaultValue: false))
		{
			return;
		}
		if (_autoEmote != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_autoEmote);
		}
		MilMo_Avatar avatar = MilMo_Player.Instance.Avatar;
		string category = "";
		switch (CurCategory.IdentifierName)
		{
		case "Upper_Body":
			category = "SHIRT";
			break;
		case "Lower_Body":
			category = "PANTS";
			break;
		case "Shoes":
			category = "SHOES";
			break;
		case "Accessories":
			switch (MilMo_Utility.RandomInt(1, 3))
			{
			case 1:
				category = "EYES";
				break;
			case 2:
				category = "MOUTH";
				break;
			case 3:
				category = "HAIR";
				break;
			}
			break;
		}
		if (!string.IsNullOrEmpty(category))
		{
			_autoEmote = MilMo_EventSystem.At(MilMo_Utility.RandomInt(8, 16), delegate
			{
				avatar.AsyncApply(PlayAutoEmote, category);
				ScheduleAutoEmote();
			});
		}
	}

	private void SpawnHotItem(MilMo_ShopItem shopItem)
	{
		MilMo_Button item = new MilMo_Button(_ui);
		item.SetPosition(90f * (float)_bottomScroller.Children.Count + 8f, 50f);
		_hotItemsFrame.SetScale(90f * (float)_bottomScroller.Children.Count, 64f);
		item.TextColorTo(0f, 1f, 1f, 1f);
		shopItem.AsyncGetIcon(delegate(Texture2D icon)
		{
			item.SetTexture(new MilMo_Texture(icon));
			item.SetHoverTexture(new MilMo_Texture(icon));
			item.SetPressedTexture(new MilMo_Texture(icon));
		});
		if (_curBottomCat != 1)
		{
			item.Angle(0f);
			item.SetAngle(-90 * _bottomScroller.Children.Count);
			item.SetScale(0f, 0f);
			item.ScaleToAbsolute(_ui.ScaleToLowestUIRes(80f, 80f));
		}
		else
		{
			item.SetAngle(0f);
			item.ScaleToAbsolute(_ui.ScaleToLowestUIRes(80f, 80f));
		}
		item.SetDefaultScale(_ui.ScaleToLowestUIRes(80f / _ui.Res.x, 80f / _ui.Res.y));
		item.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		item.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		item.SetExtraScaleOnHover(7f, 3f);
		item.SetDefaultColor(1f, 1f, 1f, 1f);
		item.SetHoverColor(1f, 1f, 1f, 1f);
		item.SetScalePull(0.07f, 0.07f);
		item.SetScaleDrag(0.8f, 0.8f);
		item.SetAnglePull(0.05f);
		item.SetAngleDrag(0.6f);
		item.SetAlignment(MilMo_GUI.Align.CenterCenter);
		item.SetPosPull(0.05f, 0.05f);
		item.SetPosDrag(0.6f, 0.6f);
		item.SetFadeSpeed(0.0005f);
		item.SetFadeOutSpeed(0.008f);
		item.SetFadeInSpeed(0.008f);
		item.Function = BrowseToItemCard;
		item.SetHoverSound(tickSound);
		item.Args = shopItem.Id;
		item.Info2 = shopItem.Id;
		_bottomScroller.AddChild(item);
	}

	private void RefreshHotItems()
	{
		_bottomScroller.RemoveAllChildren();
		_bottomScroller.ScrollToNow(_hotItemScrollVal, 0f);
		_bottomScroller.AddChild(_hotItemsFrame);
		foreach (MilMo_ShopItem item in MilMo_HotItems.Items.Where((MilMo_ShopItem item) => item.IsAvailable))
		{
			SpawnHotItem(item);
		}
		_bottomScroller.RefreshViewSize(50f, 0f);
		_curBottomCat = 1;
	}

	private void RefreshColors()
	{
		_bottomScroller.RemoveAllChildren();
		_bottomScroller.ScrollToNow(0f, 0f);
		if (_selectedItem != null && _selectedItem.Item != null)
		{
			MilMo_Wearable milMo_Wearable = _selectedItem.Item as MilMo_Wearable;
			MilMo_Furniture milMo_Furniture = _selectedItem.Item as MilMo_Furniture;
			MilMo_HomeSurface milMo_HomeSurface = _selectedItem.Item as MilMo_HomeSurface;
			List<ColorGroup> list = null;
			if (milMo_Wearable != null)
			{
				list = milMo_Wearable.BodyPack.ColorGroups;
			}
			else if (milMo_Furniture != null)
			{
				list = milMo_Furniture.HomePack.ColorGroups;
			}
			else if (milMo_HomeSurface != null)
			{
				list = milMo_HomeSurface.HomePackBase.ColorGroups;
			}
			if (list != null && list.Count > 0)
			{
				foreach (ColorGroup item in list.Where((ColorGroup g) => g != null))
				{
					if (milMo_Furniture != null)
					{
						AddColorBar(milMo_Furniture, item, milMo_Furniture.HomePack.Path);
					}
					else if (milMo_Wearable != null)
					{
						AddColorBar(milMo_Wearable, item, milMo_Wearable.BodyPack.Path);
					}
					else
					{
						AddColorBar(milMo_HomeSurface, item, milMo_HomeSurface.HomePackBase.Path);
					}
				}
			}
		}
		_curBottomCat = 0;
		ScaleColorBarsToEdgeIfNoScrollbar();
	}

	private void ScaleColorBarsToEdgeIfNoScrollbar()
	{
		int num = _bottomScroller.Children.Cast<MilMo_ColorBar>().Count();
		if (_bottomScroller.MViewSize.width + (float)(num * 80) < _bottomScroller.Scale.x)
		{
			foreach (MilMo_ColorBar item in _bottomScroller.Children.Cast<MilMo_ColorBar>())
			{
				item.SetYScale(101f);
			}
		}
		RefreshColorBarPositions();
	}

	private void AddColorBar(MilMo_Item item, ColorGroup colorGroup, string bodyPackPath)
	{
		MilMo_Wearable milMo_Wearable = item as MilMo_Wearable;
		MilMo_Furniture milMo_Furniture = item as MilMo_Furniture;
		MilMo_HomeSurface milMo_HomeSurface = item as MilMo_HomeSurface;
		if (milMo_Wearable == null && milMo_Furniture == null && milMo_HomeSurface == null)
		{
			return;
		}
		MilMo_ColorBar milMo_ColorBar = new MilMo_ColorBar(_ui, colorGroup.ColorIndices.Count, GetColorsPerRow(colorGroup.ColorIndices.Count));
		float num = 16f / _ui.Res.y;
		milMo_ColorBar.SetYScale(101f - num);
		milMo_ColorBar.ColorGroupName = colorGroup.GroupName;
		milMo_ColorBar.SetText(colorGroup.DisplayName);
		milMo_ColorBar.SetFont(MilMo_GUI.Font.EborgSmall);
		int num2 = 0;
		foreach (int colorIndex in colorGroup.ColorIndices)
		{
			milMo_ColorBar.SetButtonColor255(num2, MilMo_BodyPackSystem.GetColorFromIndex(colorIndex).IconColor, colorIndex.ToString());
			milMo_ColorBar.SetMouseOverSound(tickSound);
			milMo_ColorBar.SetClickSound(pickSound);
			num2++;
		}
		string key = bodyPackPath + ":" + colorGroup.GroupName;
		if (milMo_Wearable != null)
		{
			milMo_ColorBar.CustomFunction = UpdateColors;
			if (MilMo_Player.Instance.Avatar.BodyPackManager.ContainsBodyPackColorIndex(key))
			{
				milMo_ColorBar.ClkSelect(colorGroup.ColorIndices.IndexOf(MilMo_Player.Instance.Avatar.BodyPackManager.GetBodyPackColorIndex(key)), exec: false);
			}
			else
			{
				milMo_ColorBar.ClkSelect(0, exec: true);
			}
		}
		else if (milMo_Furniture != null)
		{
			milMo_ColorBar.CustomFunction = UpdateColors;
			if (milMo_Furniture.ContainsColorIndex(key))
			{
				milMo_ColorBar.ClkSelect(colorGroup.ColorIndices.IndexOf(milMo_Furniture.GetColorIndex(key)), exec: false);
			}
			else
			{
				milMo_ColorBar.ClkSelect(0, exec: true);
			}
		}
		else
		{
			milMo_ColorBar.CustomFunction = UpdateColors;
			if (milMo_HomeSurface.ContainsColorIndex(key))
			{
				milMo_ColorBar.ClkSelect(colorGroup.ColorIndices.IndexOf(milMo_HomeSurface.GetColorIndex(key)), exec: false);
			}
			else
			{
				milMo_ColorBar.ClkSelect(0, exec: true);
			}
		}
		_bottomScroller.AddChild(milMo_ColorBar);
		RefreshColorBarPositions();
		_bottomScroller.RefreshViewSize(50f, 0f);
	}

	private void RefreshColorBarPositions()
	{
		float num = 0f;
		foreach (MilMo_Widget child in _bottomScroller.Children)
		{
			child.SetPosition(num + 10f, 10f);
			float num2 = child.Pos.x + child.Scale.x;
			if (num2 > num)
			{
				num = num2;
			}
			num /= _ui.Res.x;
		}
	}

	private void UpdateColors(object o)
	{
		MilMo_Wearable milMo_Wearable = _selectedItem.Item as MilMo_Wearable;
		MilMo_Furniture milMo_Furniture = _selectedItem.Item as MilMo_Furniture;
		MilMo_HomeSurface milMo_HomeSurface = _selectedItem.Item as MilMo_HomeSurface;
		if (milMo_Wearable == null && milMo_Furniture == null && milMo_HomeSurface == null)
		{
			return;
		}
		string text = ((milMo_Wearable != null) ? milMo_Wearable.BodyPack.Path : ((milMo_Furniture == null) ? milMo_HomeSurface.HomePackBase.Path : milMo_Furniture.HomePack.Path));
		bool flag = false;
		foreach (MilMo_Widget child in _bottomScroller.Children)
		{
			if (child is MilMo_ColorBar milMo_ColorBar)
			{
				string text2 = text + ":" + milMo_ColorBar.ColorGroupName;
				if (milMo_Wearable != null)
				{
					flag = MilMo_Player.Instance.Avatar.BodyPackManager.SetBodyPackColorIndex(text2, int.Parse(milMo_ColorBar.GetIdentifier())) || flag;
					milMo_Wearable.UpdateColorIndex(text2, int.Parse(milMo_ColorBar.GetIdentifier()));
				}
				else
				{
					flag = ((milMo_Furniture == null) ? (milMo_HomeSurface.UpdateColorIndex(text2, int.Parse(milMo_ColorBar.GetIdentifier())) || flag) : (milMo_Furniture.UpdateColorIndex(text2, int.Parse(milMo_ColorBar.GetIdentifier())) || flag));
				}
			}
		}
		if (!flag)
		{
			return;
		}
		if (milMo_Wearable != null)
		{
			MilMo_Player.Instance.Avatar.AsyncApply();
		}
		else if (milMo_Furniture != null)
		{
			milMo_Furniture.AsyncApply();
		}
		else if (milMo_HomeSurface is MilMo_Wallpaper)
		{
			milMo_HomeSurface.Apply(_theWallpaperObject.GameObject);
			Texture2D texture = milMo_HomeSurface.GetTexture(_theWallpaperObject.GameObject);
			if (_theWallpaperObject.Renderer.material.mainTexture != texture)
			{
				_theWallpaperObject.Renderer.material.mainTexture = texture;
			}
		}
		else if (milMo_HomeSurface is MilMo_Floor)
		{
			milMo_HomeSurface.Apply(_theFloorObject.GameObject);
			Texture2D texture2 = milMo_HomeSurface.GetTexture(_theFloorObject.GameObject);
			if (_theFloorObject.Renderer.material.mainTexture != texture2)
			{
				_theFloorObject.Renderer.material.mainTexture = texture2;
			}
		}
	}

	private static int GetColorsPerRow(int colors)
	{
		if (colors == 0)
		{
			return 5;
		}
		int num = 1;
		if (colors > 6)
		{
			num = 2;
		}
		if (colors > 25)
		{
			num = 3;
		}
		int num2 = colors / num;
		int num3 = colors % num2;
		return num2 + num3;
	}

	private void SpawnMainCategory(string categoryName, MilMo_LocString displayName, string icon, Color col)
	{
		MilMo_Texture milMo_Texture = new MilMo_Texture(icon);
		milMo_Texture.AsyncLoad();
		SpawnMainCategory(categoryName, displayName, milMo_Texture, col);
	}

	private void SpawnMainCategory(string identifierName, MilMo_LocString displayName, MilMo_Texture icon, Color col)
	{
		MilMo_CharacterShopCategory milMo_CharacterShopCategory = new MilMo_CharacterShopCategory(_numMainCats, identifierName, displayName);
		_numMainCats++;
		milMo_CharacterShopCategory.Button = new MilMo_Button(_ui);
		milMo_CharacterShopCategory.Button.SetFontScale(0.5f);
		milMo_CharacterShopCategory.Button.TextColorNow(0f, 0f, 0f, 0f);
		milMo_CharacterShopCategory.Button.SetHoverTextColor(1f, 1f, 1f, 0.3f);
		milMo_CharacterShopCategory.Button.SetTexture(icon);
		milMo_CharacterShopCategory.Button.SetHoverTexture(icon);
		milMo_CharacterShopCategory.Button.SetPressedTexture(icon);
		milMo_CharacterShopCategory.Button.SetDefaultColor(1f, 1f, 1f, 1f);
		milMo_CharacterShopCategory.Button.SetHoverColor(1f, 1f, 1f, 1f);
		milMo_CharacterShopCategory.Button.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		milMo_CharacterShopCategory.Button.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Fade);
		milMo_CharacterShopCategory.Button.SetScaleDrag(0.5f, 0.7f);
		milMo_CharacterShopCategory.Button.SetScalePull(0.05f, 0.07f);
		milMo_CharacterShopCategory.Button.SetExtraScaleOnHover(4f, 4f);
		milMo_CharacterShopCategory.Button.SetPosition((float)_categoryList.Count * 80f + 65f + 160f, 0f);
		milMo_CharacterShopCategory.Button.SetAlignment(MilMo_GUI.Align.TopCenter);
		milMo_CharacterShopCategory.Button.SetScale(60f, 60f);
		milMo_CharacterShopCategory.Button.SetMinScale(60f, 60f);
		milMo_CharacterShopCategory.Button.SetDefaultScale(60f, 60f);
		milMo_CharacterShopCategory.Button.SetFixedPointerZoneSize(50f, 50f);
		_mainCatScroller.AddChild(milMo_CharacterShopCategory.Button);
		milMo_CharacterShopCategory.Number = _categoryList.Count;
		milMo_CharacterShopCategory.Button.SetArguments(milMo_CharacterShopCategory.Number, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
		milMo_CharacterShopCategory.Button.Function = ClkSelectMainCategory;
		milMo_CharacterShopCategory.Button.SetPosPull(0.09f, 0.09f);
		milMo_CharacterShopCategory.Button.SetPosDrag(0.7f, 0.7f);
		_categoryList.Add(milMo_CharacterShopCategory);
		CurCategory = milMo_CharacterShopCategory;
		milMo_CharacterShopCategory.ButtonReflect = new MilMo_Widget(_ui);
		milMo_CharacterShopCategory.ButtonReflect.SetTexture(icon);
		milMo_CharacterShopCategory.ButtonReflect.SetDefaultColor(0.8f, 0.8f, 0.8f, 0.5f);
		milMo_CharacterShopCategory.ButtonReflect.SetAlignment(MilMo_GUI.Align.TopCenter);
		milMo_CharacterShopCategory.ButtonReflect.SetPosition((float)(_categoryList.Count - 1) * 80f + 65f + 160f, 120f);
		milMo_CharacterShopCategory.ButtonReflect.SetPosPull(0.09f, 0.09f);
		milMo_CharacterShopCategory.ButtonReflect.SetPosDrag(0.7f, 0.7f);
		milMo_CharacterShopCategory.ButtonReflect.SetScaleDrag(0.5f, 0.7f);
		milMo_CharacterShopCategory.ButtonReflect.SetScalePull(0.05f, 0.07f);
		milMo_CharacterShopCategory.ButtonReflect.SetScale(60f, -60f);
		milMo_CharacterShopCategory.ButtonReflect.SetMinScale(60f, -60f);
		_mainCatScroller.AddChild(milMo_CharacterShopCategory.ButtonReflect);
		milMo_CharacterShopCategory.CaptionColor = col;
	}

	private void SpawnSubCategory(string identifier, MilMo_LocString displayName)
	{
		MilMo_Button milMo_Button = new MilMo_Button(_ui);
		milMo_Button.SetDefaultColor(0.75f, 0.75f, 0.75f, 1f);
		milMo_Button.Identifier = identifier;
		milMo_Button.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		milMo_Button.SetFadeInSpeed(1f);
		milMo_Button.SetFadeOutSpeed(0.02f);
		milMo_Button.SetAlignment(MilMo_GUI.Align.BottomLeft);
		milMo_Button.SetTextDropShadowPos(5f, 5f);
		milMo_Button.SetFontScale(0.708f / _ui.GetLowestUIRes());
		milMo_Button.SetTextOffset(0f, 0f);
		milMo_Button.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Button.SetPosPull(0.05f, 0.05f);
		milMo_Button.SetPosDrag(0.6f, 0.6f);
		milMo_Button.SetScalePull(0.05f, 0.05f);
		milMo_Button.SetScaleDrag(0.6f, 0.6f);
		milMo_Button.Info = CurCategory.TabList.Count;
		milMo_Button.SetArguments(milMo_Button.Info, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
		milMo_Button.Function = ClkSelectSubCategory;
		milMo_Button.GoToNow((float)milMo_Button.Info * 96f + 32f, 64f);
		milMo_Button.SetScale(96f, 27f);
		_subCatScroller.AddChild(milMo_Button);
		CurCategory.TabList.Add(milMo_Button);
		milMo_Button.SetFont(MilMo_GUI.Font.GothamLarge);
		milMo_Button.SetText(displayName);
		MilMo_ScrollView milMo_ScrollView = new MilMo_ScrollView(_ui);
		milMo_ScrollView.SetText(MilMo_LocString.Empty);
		milMo_ScrollView.SetPosition(0f, 155.76276f);
		milMo_ScrollView.SetScale(450f, screenHeight - 312);
		milMo_ScrollView.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_ScrollView.SetTexture("Batch01/Textures/Core/BlackTransparent");
		milMo_ScrollView.ColorNow(1f, 1f, 1f, 1f);
		milMo_ScrollView.SetViewSize(100f, 150f);
		milMo_ScrollView.HasBackground(b: false);
		milMo_ScrollView.SetEnabled(e: false);
		milMo_ScrollView.Info = milMo_Button.Info;
		milMo_ScrollView.ShowHorizontalBar(h: false);
		_ui.AddChild(milMo_ScrollView);
		CurCategory.ScrollViewList.Add(milMo_ScrollView);
		CurCategory.CurrentSubCategory = milMo_Button.Info;
		RefreshSubCategories(0);
	}

	private void RefreshSubCategories(int recurseCount)
	{
		Vector2 vector = default(Vector2);
		do
		{
			bool flag = false;
			foreach (MilMo_CharacterShopCategory item in _categoryList.Where((MilMo_CharacterShopCategory category) => category.Number != CurCategory.Number))
			{
				foreach (MilMo_Button tab in item.TabList)
				{
					tab.SetPosition((float)tab.Info * 96f + 32f, 64f);
					tab.SetScale(96f, 27f);
				}
				foreach (MilMo_ScrollView scrollView in item.ScrollViewList)
				{
					scrollView.SetEnabled(e: false);
				}
			}
			if (CurCategory.TabList.Count < 5)
			{
				_tabRightArrow.ScaleTo(_arrowSize.x, 0f);
				_tabLeftArrow.ScaleTo(_arrowSize.x, 0f);
			}
			else
			{
				_tabRightArrow.ScaleTo(_arrowSize);
				_tabLeftArrow.ScaleTo(_arrowSize);
			}
			foreach (MilMo_Button tab2 in CurCategory.TabList)
			{
				if (tab2.Info == CurCategory.CurrentSubCategory)
				{
					if (CurCategory.TabList.Count == 1)
					{
						tab2.SetAllTextures("Batch01/Textures/Core/Invisible");
						tab2.AllowPointerFocus = false;
						tab2.SetFontScale(0f);
					}
					else
					{
						tab2.AllowPointerFocus = true;
						tab2.SetAllTextures("Batch01/Textures/Shop/ActiveTabNormal");
						tab2.SetHoverColor(1f, 1f, 1f, 1f);
						tab2.GoTo((float)tab2.Info * 96f + 32f, 32f);
						tab2.ScaleTo(96f, 32f);
						tab2.SetTextOffset(0f, -5f);
						tab2.ColorNow(tab2.HoverColor);
						tab2.TextColorTo(1f, 1f, 1f, 1f);
						tab2.SetHoverTextColor(1f, 1f, 1f, 1f);
						tab2.SetFontScale(0.708f);
					}
				}
				else if (CurCategory.TabList.Count == 1)
				{
					tab2.SetAllTextures("Batch01/Textures/Core/Invisible");
					tab2.AllowPointerFocus = false;
					tab2.SetFontScale(0f);
				}
				else
				{
					tab2.AllowPointerFocus = true;
					tab2.SetAllTextures("Batch01/Textures/Shop/InactiveTabNormal");
					tab2.SetHoverColor(1f, 1f, 0.7f, 1f);
					tab2.GoTo((float)tab2.Info * 96f + 32f, 32f);
					tab2.SetScale(96f, 27f);
					tab2.TextColorNow(0.6f, 0.6f, 0.6f, 1f);
					tab2.SetHoverTextColor(0.9f, 0.9f, 0.9f, 1f);
					tab2.SetFontScale(0.708f);
				}
				tab2.SetEnabled(e: true);
				tab2.GoTo((float)tab2.Info * 96f + 32f, 32f);
				if (tab2.Pos.x > _subCatScroller.SoftScroll.Target.x + 354f * tab2.UI.Res.x)
				{
					if (tab2.Info == CurCategory.CurrentSubCategory)
					{
						Vector2 scrollTarget = _subCatScroller.GetScrollTarget();
						_subCatScroller.ScrollTo((float)CurCategory.CurrentSubCategory * 96f - 287f, scrollTarget.y);
						flag = true;
					}
					else
					{
						tab2.SetEnabled(e: false);
					}
					if (!_tabRightArrow.IsEnabled())
					{
						_tabRightArrow.ColorNow(1f, 1f, 1f, 1f);
					}
				}
				else if (tab2.Pos.x - _subCatScroller.SoftScroll.Target.x < 0f)
				{
					if (tab2.Info == CurCategory.CurrentSubCategory)
					{
						Vector2 scrollTarget2 = _subCatScroller.GetScrollTarget();
						_subCatScroller.ScrollTo((float)CurCategory.CurrentSubCategory * 96f, scrollTarget2.y);
						flag = true;
					}
					else
					{
						tab2.SetEnabled(e: false);
					}
					if (!_tabLeftArrow.IsEnabled())
					{
						_tabLeftArrow.ColorNow(1f, 1f, 1f, 1f);
					}
				}
			}
			foreach (MilMo_ScrollView scrollView2 in CurCategory.ScrollViewList)
			{
				scrollView2.SetEnabled(e: false);
				if (scrollView2.Info == CurCategory.CurrentSubCategory)
				{
					vector.x = 10f;
					vector.y = 20f;
					int num = 0;
					foreach (MilMo_Widget child in scrollView2.Children)
					{
						if (child.GetType() == typeof(MilMo_ItemCard))
						{
							num++;
							child.ColorNow(1f, 1f, 1f, 1f);
							child.GoToNow(-500f, vector.y);
							child.GoTo(vector.x, vector.y);
							if (vector.x >= 220f)
							{
								vector.x = -200f;
								vector.y += 140f;
							}
							vector.x += 210f;
							child.PosMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Nothing);
							child.CustomFunction((float)num * 0.35f);
							((MilMo_ItemCard)child).RefreshUI();
						}
						else if (child.GetType() == typeof(MilMo_ComingSoonCard))
						{
							MilMo_ComingSoonCard obj = (MilMo_ComingSoonCard)child;
							obj.Show();
							obj.RefreshUI();
						}
					}
					scrollView2.IsUserControlled(b: true);
					scrollView2.SetPosition(0f, 155.76276f);
					scrollView2.SetScale(450f, screenHeight - 312);
					scrollView2.SetEnabled(e: true);
					scrollView2.RefreshViewSize(0f, 50f);
					continue;
				}
				foreach (MilMo_Widget item2 in scrollView2.Children.Where((MilMo_Widget w) => w.GetType() == typeof(MilMo_ItemCard)))
				{
					item2.ColorTo(0f, 0f, 0f, 0f);
					item2.GoTo(-500f, item2.PosMover.Target.y / _ui.Res.y);
				}
				scrollView2.IsUserControlled(b: false);
				scrollView2.SetPosition(0f, 155.76276f);
				scrollView2.SetScale(450f, screenHeight - 312);
				scrollView2.Scale.x -= 16f;
				scrollView2.SetEnabled(e: false);
			}
			if (!flag)
			{
				break;
			}
			recurseCount++;
		}
		while (recurseCount <= 1);
	}

	private void ClkSelectBottomCategory(object arg)
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.SelectSubCat);
		SelectBottomCategory(arg);
	}

	private void SelectBottomCategory(object arg)
	{
		int num = (int)arg;
		_colorsButton.SetTexture("Batch01/Textures/Shop/BottomScrollerTab");
		_colorsButton.SetHoverTexture("Batch01/Textures/Shop/BottomScrollerTabMO");
		_colorsButton.SetPressedTexture("Batch01/Textures/Shop/BottomScrollerTab");
		_colorsButton.SetDefaultTextColor(1f, 1f, 1f, 0.2f);
		_colorsButton.SetHoverTextColor(1f, 1f, 1f, 1f);
		_hotItemsButton.SetTexture("Batch01/Textures/Shop/BottomScrollerTab");
		_hotItemsButton.SetHoverTexture("Batch01/Textures/Shop/BottomScrollerTabMO");
		_hotItemsButton.SetPressedTexture("Batch01/Textures/Shop/BottomScrollerTab");
		_hotItemsButton.SetDefaultTextColor(1f, 1f, 1f, 0.2f);
		_hotItemsButton.SetHoverTextColor(1f, 1f, 1f, 1f);
		if (_curBottomCat == 1)
		{
			_hotItemScrollVal = _bottomScroller.SoftScroll.Val.x;
		}
		switch (num)
		{
		case 0:
			_colorsButton.SetTexture("Batch01/Textures/Shop/BottomScrollerTabMO");
			_colorsButton.SetHoverTexture("Batch01/Textures/Shop/BottomScrollerTabMO");
			_colorsButton.SetPressedTexture("Batch01/Textures/Shop/BottomScrollerTab");
			_colorsButton.SetDefaultTextColor(1f, 1f, 1f, 1f);
			_colorsButton.SetDefaultColor(1f, 1f, 1f, 1f);
			_colorsButton.GoTo(450f, screenHeight - 113);
			_hotItemsButton.GoTo(581f, screenHeight - 113);
			_ui.BringToFront(_colorsButton);
			RefreshColors();
			break;
		case 1:
			_hotItemsButton.SetTexture("Batch01/Textures/Shop/BottomScrollerTabMO");
			_hotItemsButton.SetHoverTexture("Batch01/Textures/Shop/BottomScrollerTabMO");
			_hotItemsButton.SetPressedTexture("Batch01/Textures/Shop/BottomScrollerTab");
			_hotItemsButton.SetDefaultTextColor(1f, 1f, 1f, 1f);
			_hotItemsButton.SetDefaultColor(1f, 1f, 1f, 1f);
			_hotItemsButton.GoTo(450f, screenHeight - 113);
			_colorsButton.GoTo(627f, screenHeight - 113);
			_ui.BringToFront(_hotItemsButton);
			RefreshHotItems();
			break;
		}
	}

	private void ClkSelectMainCategory(object arg)
	{
		if (CurCategory != null)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.SelectMainCat);
			SelectMainCategory(arg);
		}
	}

	private void SelectMainCategory(object arg, bool loadItems = true)
	{
		if (CurCategory == null)
		{
			return;
		}
		int num = (int)((float[])arg)[0];
		_iconRightArrow.ScaleTo(_arrowSize);
		_iconLeftArrow.ScaleTo(_arrowSize);
		CurCategory.ScrollTarget = _subCatScroller.SoftScroll.Target.x;
		foreach (MilMo_CharacterShopCategory category in _categoryList)
		{
			if (category.Number != num)
			{
				category.Button.GoTo((float)category.Number * 80f + 65f + 160f, 0f);
				category.Button.SetDefaultScale(_ui.ScaleToLowestUIRes(60f / _ui.Res.x, 60f / _ui.Res.y));
				category.Button.ScaleToAbsolute(_ui.ScaleToLowestUIRes(60f, 60f));
				category.Button.SetExtraScaleOnHover(4f, 4f);
				category.Button.SetDefaultColor(1f, 1f, 1f, 1f);
				category.ButtonReflect.SetDefaultColor(0.8f, 0.8f, 0.8f, 0.5f);
				category.Button.SetMinScale(_ui.ScaleToLowestUIRes(60f / _ui.Res.x, 60f / _ui.Res.y));
				category.ButtonReflect.GoTo((float)category.Number * 80f + 65f + 160f, 120f);
				category.ButtonReflect.SetScaleAbsolute(_ui.ScaleToLowestUIRes(60f, -60f));
				category.ButtonReflect.SetMinScale(_ui.ScaleToLowestUIRes(60f / _ui.Res.x, -60f / _ui.Res.y));
			}
			else
			{
				_middleIcon.GoToNow(225 - (CurCategory.Number - category.Number) * 80, 0f);
				_middleIcon.GoTo(225f, 0f);
				_middleIcon.SetTexture(category.Button.Texture);
				_middleIcon.SetScaleAbsolute(_ui.ScaleToLowestUIRes(80f, 80f));
				_middleIcon.ScaleToAbsolute(_ui.ScaleToLowestUIRes(100f, 100f));
				CurCategory = category;
				_subCatScroller.ScrollTo(CurCategory.ScrollTarget, 0f);
				CurCategory.Button.SetDefaultScale(0f, 0f);
				CurCategory.Button.ScaleNow(0f, 0f);
				CurCategory.Button.SetMinScale(0f, 0f);
				CurCategory.ButtonReflect.ScaleNow(0f, 0f);
				CurCategory.ButtonReflect.SetMinScale(0f, 0f);
				CurCategory.Button.SetExtraScaleOnHover(0f, 0f);
				CurCategory.Button.SetDefaultColor(1f, 1f, 1f, 0f);
				CurCategory.Button.ColorNow(1f, 1f, 1f, 0f);
				CurCategory.ButtonReflect.SetDefaultColor(0.8f, 0.8f, 0.8f, 0f);
			}
		}
		_mainCatScroller.ScrollTo((float)CurCategory.Number * 80f + 65f + 160f - 225f, 0f);
		DisableOffScreenIcons();
		_mainCaption.SetText(CurCategory.DisplayName);
		_mainCaption.ColorNow(1f, 1f, 1f, 0f);
		_mainCaption.ColorTo(1f, 1f, 1f, 1f);
		_mainCaption.SetXPos(32f);
		_mainCaption.GoToY((CurCategory.TabList.Count > 1) ? 87f : 100f);
		mainCaptionBack.ColorTo(CurCategory.CaptionColor);
		captionShine.SetPosition(450f, 82f);
		captionShine.GoTo(250f, 82f);
		RefreshSubCategories(0);
		ScheduleAutoEmote();
		PlayIdleAnimation(MilMo_Player.Instance.Avatar);
		if (loadItems && !MilMo_Config.Instance.IsTrue("Launcher.OfflineShop", defaultValue: false))
		{
			MilMo_ShopAssetManager.CategoryChanged(_categoryList, CurCategory);
		}
		BringPanesToFront();
		_iconRightArrow.SetScale(_arrowSize);
		_iconLeftArrow.SetScale(_arrowSize);
	}

	private void MainCatSoftScrollArrive()
	{
		_mainCatIconHeight = -90f;
		DisableOffScreenIcons();
	}

	private void ClkPrevMainCategory(object arg)
	{
		float[] array = new float[8] { CurCategory.Number, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
		if (array[0] < 1f)
		{
			array[0] = _numMainCats - 1;
			_categoryList.ForEach(delegate(MilMo_CharacterShopCategory category)
			{
				category.Button.GoTo((float)category.Number * 80f + 65f + 160f, 0f);
				category.Button.SetEnabled(e: true);
				_mainCatIconHeight = 0f;
			});
		}
		else
		{
			array[0] -= 1f;
		}
		ClkSelectMainCategory(array);
		_mainCatIconHeight = -90f;
	}

	private void ClkNextMainCategory(object arg)
	{
		float[] array = new float[8] { CurCategory.Number, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
		if ((double)Math.Abs(array[0] - (float)(_numMainCats - 1)) <= 0.0)
		{
			array[0] = 0f;
			_categoryList.ForEach(delegate(MilMo_CharacterShopCategory category)
			{
				category.Button.GoTo((float)category.Number * 80f + 65f + 160f, 0f);
				category.Button.SetEnabled(e: true);
				_mainCatIconHeight = 0f;
			});
		}
		else
		{
			array[0] += 1f;
		}
		ClkSelectMainCategory(array);
		_mainCatIconHeight = -90f;
	}

	private void ClkSelectSubCategory(object arg)
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.SelectSubCat);
		SelectSubCategory(arg);
	}

	private void SelectSubCategory(object arg)
	{
		CurCategory.ScrollViewList.ForEach(delegate(MilMo_ScrollView scroller)
		{
			if (scroller.Info == CurCategory.CurrentSubCategory)
			{
				scroller.Info2++;
			}
		});
		CurCategory.CurrentSubCategory = (int)((float[])arg)[0];
		RefreshSubCategories(0);
		MilMo_ShopAssetManager.CategoryChanged(_categoryList, CurCategory);
	}

	private void ClkPrevSubCategory(object arg)
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.SelectSubCat);
		if (CurCategory.CurrentSubCategory > 0)
		{
			CurCategory.CurrentSubCategory--;
		}
		else
		{
			CurCategory.CurrentSubCategory = CurCategory.TabList.Count - 1;
		}
		RefreshSubCategories(0);
		MilMo_ShopAssetManager.CategoryChanged(_categoryList, CurCategory);
	}

	private void ClkNextSubCategory(object arg)
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.SelectSubCat);
		if (CurCategory.CurrentSubCategory < CurCategory.TabList.Count - 1)
		{
			CurCategory.CurrentSubCategory++;
		}
		else
		{
			CurCategory.CurrentSubCategory = 0;
		}
		RefreshSubCategories(0);
		MilMo_ShopAssetManager.CategoryChanged(_categoryList, CurCategory);
	}

	public static void ClearShopData()
	{
		if (_theCharacterShop != null)
		{
			MilMo_EventSystem.RemoveReaction(_theCharacterShop._memberStatusReaction);
			MilMo_UserInterfaceManager.DestroyUserInterface(_theCharacterShop._ui);
			_theCharacterShop._remoteShop.Destroy();
			UnityEngine.Object.Destroy(_theCharacterShop);
		}
		_theCharacterShop = null;
		PreloadedAssetsDone = false;
		_shopEmoteHandler = null;
		_captionColor = null;
		_readyCallback = null;
		_returnToLevelName = null;
		_returnToLevelIcon = null;
		_levelName = null;
		_levelIcon = null;
	}

	private static void Create()
	{
		if (!(_theCharacterShop != null))
		{
			Component component = MilMo_Global.MainGameObject.AddComponent<MilMo_CharacterShop>();
			if (component == null)
			{
				Debug.LogWarning("Failed to create character shop. Failed to add component MilMo_CharacterShop.");
				return;
			}
			_theCharacterShop = (MilMo_CharacterShop)component;
			_theCharacterShop.enabled = false;
			_theCharacterShop._ui = MilMo_UserInterfaceManager.CreateUserInterface("CharacterShop");
			_theCharacterShop.InitUI();
			_theCharacterShop._ui.Enabled = false;
			_theCharacterShop._remoteShop = new MilMo_RemoteShop(_theCharacterShop.BuySuccess, _theCharacterShop.BuyFailed);
			_shopEmoteHandler = new MilMo_ShopEmoteHandler();
			_theCharacterShop._remoteShop.RequestRemoteData(_theCharacterShop.GotShopItems);
			_theCharacterShop._memberStatusReaction = MilMo_EventSystem.Listen("member_status_activated", _theCharacterShop.MemberStatusActivated);
			_theCharacterShop._memberStatusReaction.Repeating = true;
		}
	}

	public static void SetReturnIcon(Texture2D returnIcon)
	{
		_levelIcon = returnIcon;
		_returnToLevelIcon.SetTexture(new MilMo_Texture(_levelIcon));
	}

	public static void SetReturnName(MilMo_LocString returnName)
	{
		_levelName = returnName;
		_returnToLevelName.SetText(_levelName);
	}

	public static void Activate(ShopReady callback)
	{
		_readyCallback = callback;
		if (_theCharacterShop == null)
		{
			Create();
		}
		MilMo_ResourceManager.Instance.PreloadAssets(MilMo_ShopPreload.assets, "Shop", delegate
		{
			PreloadedAssetsDone = true;
		});
		if (_theCharacterShop._remoteShop.HaveData)
		{
			if (MilMo_Player.Instance.Avatar.Gender != _theCharacterShop._currentGender)
			{
				foreach (MilMo_ScrollView item in _theCharacterShop._categoryList.SelectMany((MilMo_CharacterShopCategory category) => category.ScrollViewList))
				{
					_theCharacterShop._ui.RemoveChild(item);
				}
				_theCharacterShop._ui.RemoveAllChildrenByType(typeof(MilMo_ItemCard));
				_theCharacterShop.CurCategory = null;
				_theCharacterShop._mainCatScroller.RemoveAllChildren();
				_theCharacterShop._numMainCats = 0;
				_theCharacterShop._subCatScroller.RemoveAllChildren();
				_theCharacterShop.RefreshHotItems();
				_theCharacterShop._categoryList.Clear();
				_theCharacterShop.GotShopItems(gotData: true);
			}
			else
			{
				_theCharacterShop.ActivateInternal();
			}
		}
		if (MilMo_Monetization.Instance.Currency != null)
		{
			MilMo_Monetization.Instance.Currency.RefreshAccountBalance();
		}
	}

	public static void Deactivate()
	{
		_theCharacterShop.DeactivateInternal();
	}

	private void ActivateInternal()
	{
		_theCharacterShop.InitObjects();
		_viewMode = ViewMode.Character;
		base.enabled = true;
		InitCamera();
		InitLight();
		InitCharacterMover();
		_ui.Enabled = true;
		_currentObjectMountedInWall = false;
		float[] arg = new float[1] { 2f };
		if (MilMo_Home.CurrentHome != null)
		{
			float[] arg2 = new float[1];
			SelectMainCategory(arg2, loadItems: false);
		}
		else
		{
			SelectMainCategory(arg, loadItems: false);
		}
		SelectBottomCategory(1);
		foreach (MilMo_CharacterShopCategory category in _categoryList)
		{
			category.CurrentSubCategory = 0;
			RefreshSubCategories(0);
		}
		if (!MilMo_Config.Instance.IsTrue("Launcher.OfflineShop", defaultValue: false))
		{
			MilMo_ShopAssetManager.CategoryChanged(_categoryList, CurCategory);
		}
		_middleIcon.GoToNow(225f, 0f);
		_backButton.SetHoverTexture("Batch01/Textures/Shop/BackButtonMO");
		_backButton.SetFontScale(0.8f);
		MilMo_Player.Instance.Inventory.Store();
		_currentIngameCoinsValue = (_wantedIngameCoinsValue = GlobalStates.Instance.playerState.coins.Get());
		_ingameCoinsAccountBalance.SetTextNoLocalization(Convert.ToString(_currentIngameCoinsValue));
		_ingameCoinsIcon.SetTexture("Batch01/Textures/Shop/Coins");
		_ingameCoinsAccountBalance.SetEnabled(e: true);
		_ingameCoinsIcon.SetEnabled(e: true);
		if (MilMo_Monetization.Instance.Currency.ShowAccountBalance)
		{
			_currentJuneCashValue = (_wantedJuneCashValue = MilMo_Player.Instance.JuneCash);
			_accountBalance.SetTextNoLocalization(Convert.ToString(_currentJuneCashValue));
			_currencyIcon.SetTexture(MilMo_Monetization.Instance.Currency.IconPath);
			_accountBalance.SetEnabled(e: true);
			_currencyIcon.SetEnabled(e: true);
		}
		else
		{
			_currentJuneCashValue = (_wantedJuneCashValue = 0);
			_accountBalance.SetTextNoLocalization("");
			_accountBalance.SetEnabled(e: false);
			_currencyIcon.SetEnabled(e: false);
		}
		RefreshAccountBalanceScale();
		_mChargeButton.SetScale(MilMo_Monetization.Instance.Currency.ChargeButtonSize);
		_mChargeButton.SetTexture(MilMo_Monetization.Instance.Currency.ChargeButtonTexture);
		_mChargeButton.SetHoverTexture(MilMo_Monetization.Instance.Currency.ChargeButtonTextureMO);
		_mChargeButton.SetPressedTexture(MilMo_Monetization.Instance.Currency.ChargeButtonTexturePressed);
		MilMo_LocString locString = MilMo_Localization.GetLocString("CharacterShop_243");
		locString.SetFormatArgs(MilMo_Monetization.Instance.Currency.Name, MilMo_Monetization.Instance.Currency.ChargeButtonText, MilMo_Monetization.Instance.Currency.Name);
		helpText.SetText(locString);
		MilMo_HotItems.AsyncFetchHotItems(_remoteShop.Root, delegate(bool success)
		{
			RefreshHotItems();
			if (success)
			{
				MilMo_HotItems.SetHotItemsSeen();
			}
		});
		if (!MilMo_Config.Instance.IsTrue("Launcher.OfflineShop", defaultValue: false) && MilMo_Player.Instance.Avatar != null)
		{
			MilMo_Player.Instance.Avatar.AsyncApply(PlayIntroEmote);
		}
		_goingBackToGame = false;
		MilMo_EventSystem.At(0.5f, delegate
		{
			if (_readyCallback != null)
			{
				_readyCallback(success: true);
			}
		});
		MilMo_EventSystem.At(1f, delegate
		{
			if (!string.IsNullOrEmpty(_itemToSelectOnEnter))
			{
				SelectItem(_itemToSelectOnEnter);
			}
			_itemToSelectOnEnter = "";
		});
	}

	private void DeactivateInternal()
	{
		MilMo_Global.Destroy(_shopLight);
		MilMo_Global.Destroy(_shopLightHomeObjects);
		if (_thePlatform != null)
		{
			_thePlatform.Disable();
		}
		if (_theBackdrop != null)
		{
			_theBackdrop.Disable();
		}
		if (_theShopClouds != null)
		{
			_theShopClouds.Disable();
		}
		if (_theWallpaperObject != null)
		{
			_theWallpaperObject.Disable();
		}
		if (_theFloorObject != null)
		{
			_theFloorObject.Disable();
		}
		if (_theWallObject != null)
		{
			_theWallObject.Disable();
			_theWallObject.GameObject.transform.parent = null;
		}
		foreach (MilMo_ShopCategory mainCategory in _remoteShop.MainCategories)
		{
			foreach (MilMo_ShopCategory subCategory in mainCategory.SubCategories)
			{
				foreach (MilMo_ShopItem item in subCategory.Items)
				{
					if (item.Item is MilMo_Furniture milMo_Furniture)
					{
						if (milMo_Furniture.GameObject != null)
						{
							milMo_Furniture.UnloadContent();
						}
					}
					else if (item.Item is MilMo_ShopRoom)
					{
						((MilMo_ShopRoom)item.Item).Unload();
					}
				}
			}
		}
		foreach (MilMo_Button tryButton in tryButtonList)
		{
			_ui.RemoveChild(tryButton);
		}
		_pulseCursor.SetPosition(0f, -100f);
		_selectMenu.SetPosition(0f, -100f);
		_pulseCursor.SetEnabled(e: false);
		_selectMenu.SetEnabled(e: false);
		tryButtonList.Clear();
		_ui.Enabled = false;
		foreach (MilMo_ObjectMover allMover in _allMovers)
		{
			allMover.DetachAll();
		}
		base.enabled = false;
		_platformHooked = false;
		MilMo_EventSystem.RemoveTimerEvent(_autoEmote);
		MilMo_EventSystem.RemoveTimerEvent(_junecoinsUpdater);
	}

	private void GotShopItems(bool gotData)
	{
		if (!gotData)
		{
			Debug.LogWarning("Failed to fetch shop data from server");
			return;
		}
		_currentGender = MilMo_Player.Instance.Avatar.Gender;
		foreach (MilMo_ShopCategory mainCategory in _remoteShop.MainCategories)
		{
			SpawnMainCategory(mainCategory.IdentifierName, mainCategory.DisplayName, mainCategory.IconPath, GetNextCaptionColor());
			foreach (MilMo_ShopCategory item in mainCategory.SubCategories.Where((MilMo_ShopCategory sub) => sub.IsGender(MilMo_Player.Instance.Avatar)))
			{
				SpawnSubCategory(item.IdentifierName, item.DisplayName);
				if (item.Items.Count == 0)
				{
					SpawnComingSoonMessage();
					continue;
				}
				foreach (MilMo_ShopItem item2 in item.Items)
				{
					if (item2.IsAvailable && item2.IsUseableByGender(MilMo_Player.Instance.Avatar.IsBoy))
					{
						MilMo_ItemCard milMo_ItemCard = SpawnShopItem(item2);
						milMo_ItemCard.Args = item2;
						milMo_ItemCard.Function = Try;
						milMo_ItemCard.BuyButton.Args = item2;
						milMo_ItemCard.BuyButton.Function = Buy;
						milMo_ItemCard.GiftButton.Args = item2;
						milMo_ItemCard.GiftButton.Function = BuyAsGift;
					}
				}
				foreach (MilMo_ScrollView item3 in from scroller in CurCategory.ScrollViewList
					where scroller.Info == CurCategory.CurrentSubCategory
					where scroller.Children.Count < 1
					select scroller)
				{
					_ = item3;
					SpawnComingSoonMessage();
				}
			}
		}
		BringPanesToFront();
		ActivateInternal();
		RefreshUI();
	}

	private void Update()
	{
		try
		{
			UpdateVisualReps();
			if (_selectedItem != null && _selectedItem.Item is MilMo_ShopRoom)
			{
				((MilMo_ShopRoom)_selectedItem.Item).Update();
			}
			if (_thePlatform != null && (bool)_thePlatform.GameObject && !_platformHooked)
			{
				_mover.AttachObject(_thePlatform.GameObject);
				_platformHooked = true;
			}
			if (_ui != null && _ui.ScreenSizeDirty)
			{
				RefreshUI();
			}
			if (MilMo_Player.Instance.Avatar != null)
			{
				MilMo_Player.Instance.Avatar.Update();
			}
			if (MilMo_Monetization.Instance.Currency.ShowAccountBalance)
			{
				_wantedJuneCashValue = MilMo_Monetization.Instance.Currency.UserAccountBalance;
				if (_currentJuneCashValue != _wantedJuneCashValue)
				{
					int num = (int)Mathf.Lerp(_currentJuneCashValue, _wantedJuneCashValue, Time.deltaTime * 4f);
					int num2 = ((_currentJuneCashValue < _wantedJuneCashValue) ? 1 : (-1));
					_currentJuneCashValue = ((num - _currentJuneCashValue != 0) ? num : (_currentJuneCashValue + num2));
					_accountBalance.SetTextNoLocalization(Convert.ToString(_currentJuneCashValue));
					RefreshAccountBalanceScale();
					_juneCoinTickSoundDelay += Time.deltaTime * 60f;
					if (_juneCoinTickSoundDelay >= 4f)
					{
						_juneCoinTickSoundDelay = 0f;
						_ui.SoundFx.Play(juneCoinTickSound);
					}
				}
			}
			else
			{
				_accountBalance.SetTextNoLocalization("");
			}
			_wantedIngameCoinsValue = GlobalStates.Instance.playerState.coins.Get();
			if (_currentIngameCoinsValue != _wantedIngameCoinsValue)
			{
				int num3 = (int)Mathf.Lerp(_currentIngameCoinsValue, _wantedIngameCoinsValue, Time.deltaTime * 4f);
				int num4 = ((_currentIngameCoinsValue < _wantedIngameCoinsValue) ? 1 : (-1));
				_currentIngameCoinsValue = ((num3 - _currentIngameCoinsValue != 0) ? num3 : (_currentIngameCoinsValue + num4));
				_ingameCoinsAccountBalance.SetTextNoLocalization(Convert.ToString(_currentIngameCoinsValue));
				RefreshInGameAccountBalanceScale();
				_juneCoinTickSoundDelay += Time.deltaTime * 60f;
				if (_juneCoinTickSoundDelay >= 4f)
				{
					_juneCoinTickSoundDelay = 0f;
					_ui.SoundFx.Play(juneCoinTickSound);
				}
			}
			if (_middleIcon != null)
			{
				_middleIcon.SetAlpha((_ui != null && _middleIcon.Pos.x > 350f * _ui.Res.x) ? 0f : 1f);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private void FixedUpdate()
	{
		try
		{
			if (Time.time - _juneCashUpdateTime > 5f && MilMo_Player.Instance.InShop)
			{
				Singleton<GameNetwork>.Instance.RequestJuneCash();
				_juneCashUpdateTime = Time.time;
			}
			if (MilMo_Player.Instance != null && MilMo_Player.Instance.Avatar != null)
			{
				MilMo_Player.Instance.Avatar.FixedUpdate();
			}
			if (_ui != null && MilMo_Pointer.LeftButton && MilMo_UserInterface.PointerFocus == _charSpinner)
			{
				float x = MilMo_Pointer.Move.x;
				_objectAngle -= x * MilMo_Pointer.PointerSens.x;
				if (_currentObjectMountedInWall)
				{
					_objectAngle = Mathf.Repeat(_objectAngle, 360f);
					_objectAngle = Mathf.Clamp(_objectAngle, 195f, 345f);
				}
				_mover.SetAngle(0f, _objectAngle, 0f);
			}
			if (_currentObjectMountedInWall)
			{
				float num = Mathf.Repeat(_mover.Angle.y, 360f);
				float b = Mathf.Clamp(num, 195f, 345f);
				if (!MilMo_Utility.Equals(num, b, 0.001f))
				{
					_objectAngle = Mathf.Repeat(_objectAngle, 360f);
					_objectAngle = Mathf.Clamp(_objectAngle, 195f, 345f);
					_mover.AngleNow(0f, _objectAngle, 0f);
				}
			}
			if (_mover != null)
			{
				_mover.Update();
			}
			foreach (MilMo_ObjectMover item in _allMovers.Where((MilMo_ObjectMover mover) => mover != _mover))
			{
				item.Update();
			}
			if (CurCategory == null || CurCategory.TabList == null)
			{
				return;
			}
			CurCategory.TabList.ForEach(delegate(MilMo_Button tab)
			{
				if (tab.Pos.x > _subCatScroller.SoftScroll.Target.x + 354f * tab.UI.Res.x)
				{
					tab.ColorNow(0f, 0f, 0f, 0f);
				}
				else
				{
					tab.ColorNow(1f, 1f, 1f, 1f);
				}
			});
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private void LateUpdate()
	{
		try
		{
			if (MilMo_Player.Instance.Avatar != null)
			{
				MilMo_Player.Instance.Avatar.LateUpdate();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private Color GetNextCaptionColor()
	{
		Color result = _captionColor[_curCaptionColor];
		_curCaptionColor++;
		if (_curCaptionColor >= _captionColor.Length)
		{
			_curCaptionColor = 0;
		}
		return result;
	}

	private MilMo_ItemCard SpawnShopItem(MilMo_ShopItem item)
	{
		MilMo_ItemCard itemCard = new MilMo_ItemCard(_ui, this, item);
		_ui.AddChild(itemCard);
		itemCard.SetText(item.DisplayName);
		itemCard.SetDescription(item.Item.Template.ShopDescription);
		itemCard.PriceTag.SetTextNoLocalization(item.GetPrice(MilMo_Player.Instance.IsMember, isGift: false).ToString());
		itemCard.CoinsPriceTag.SetTextNoLocalization(item.GetCoinPrice().ToString());
		itemCard.SetIcon(null);
		itemCard.Thumbnail.Function = TryLocal;
		CurCategory.ScrollViewList.ForEach(delegate(MilMo_ScrollView scroller)
		{
			if (scroller.Info == CurCategory.CurrentSubCategory)
			{
				scroller.AddChild(itemCard);
			}
		});
		return itemCard;
	}

	private void MemberStatusActivated(object dummy)
	{
		Debug.Log("MemberStatusActivated=" + MilMo_Player.Instance.IsMember);
		UpdateAllPriceTags();
	}

	private void UpdateAllPriceTags()
	{
		bool isMember = MilMo_Player.Instance.IsMember;
		foreach (MilMo_CharacterShopCategory category in _categoryList)
		{
			foreach (MilMo_ScrollView scrollView in category.ScrollViewList)
			{
				foreach (MilMo_ItemCard item in scrollView.Children.OfType<MilMo_ItemCard>())
				{
					item.PriceTag.SetTextNoLocalization(item.ShopItem.GetPrice(isMember, isGift: false).ToString());
				}
			}
		}
	}

	private void SpawnComingSoonMessage()
	{
		foreach (MilMo_ScrollView scrollView in CurCategory.ScrollViewList)
		{
			if (scrollView.Info == CurCategory.CurrentSubCategory)
			{
				MilMo_ComingSoonCard w = new MilMo_ComingSoonCard(_ui);
				scrollView.AddChild(w);
			}
		}
	}

	private bool SpawnTryButton(MilMo_ShopItem item)
	{
		if (tryButtonList.Any((MilMo_Button but) => item == (MilMo_ShopItem)but.Args))
		{
			ClkSelectItem(item);
			return false;
		}
		if (tryButtonList.Count == 9)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
			int num = 0;
			foreach (MilMo_Button tryButton in tryButtonList)
			{
				tryButton.Impulse(0f, num * 2);
				if (num < 4)
				{
					tryButton.SetColor(1f, 0.2f * (float)num, 0.2f * (float)num, 1f);
					tryButton.ColorTo(1f, 1f, 1f, 1f);
				}
				num++;
			}
			return false;
		}
		MilMo_Button tryingButton = new MilMo_Button(_ui);
		item.AsyncGetIcon(delegate(Texture2D icon)
		{
			tryingButton.SetAllTextures(new MilMo_Texture(icon));
		});
		tryingButton.SetPosition(974f, 50f);
		tryingButton.SetAngle(-90f);
		tryingButton.Angle(0f);
		tryingButton.SetScale(0f, 0f);
		tryingButton.ScaleToAbsolute(_ui.ScaleToLowestUIRes(60f, 60f));
		tryingButton.SetScalePull(0.07f, 0.07f);
		tryingButton.SetScaleDrag(0.6f, 0.6f);
		tryingButton.SetPosPull(0.07f, 0.07f);
		tryingButton.SetPosDrag(0.8f, 0.8f);
		tryingButton.FadeToDefaultColor = true;
		tryingButton.SetFadeSpeed(0.04f);
		tryingButton.SetFadeInSpeed(0.04f);
		tryingButton.SetFadeOutSpeed(0.04f);
		tryingButton.Args = item;
		tryingButton.Function = ClkSelectItem;
		tryingButton.SetDefaultColor(1f, 1f, 1f, 0.7f);
		tryingButton.SetHoverColor(1f, 1f, 1f, 1f);
		tryingButton.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		_ui.AddChild(tryingButton);
		tryButtonList.Add(tryingButton);
		tryingButton.SetDefaultScale(60f, 60f);
		tryingButton.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		tryingButton.SetExtraScaleOnHover(7f, 3f);
		tryingButton.SetHoverSound(tickSound);
		return true;
	}

	private void ClkSelectItem(object o)
	{
		_selectedBuyButton.AllowPointerFocus = false;
		_selectedGiftButton.AllowPointerFocus = false;
		_selectedRemoveButton.AllowPointerFocus = false;
		bool num = _selectedItem != null && _selectedItem.Item is MilMo_Furniture && (((MilMo_ShopItem)o).Item is MilMo_Furniture || ((MilMo_ShopItem)o).Item is MilMo_ShopRoom) && _selectedItem != o;
		_selectedItem = o as MilMo_ShopItem;
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
		RefreshSelection();
		if (num)
		{
			SwitchFurniture();
		}
	}

	private void RefreshTryButtons()
	{
		for (int num = tryButtonList.Count - 1; num >= 0; num--)
		{
			MilMo_Button milMo_Button = tryButtonList[num];
			MilMo_ShopItem milMo_ShopItem = milMo_Button.Args as MilMo_ShopItem;
			bool flag = false;
			if (milMo_ShopItem != null)
			{
				MilMo_Wearable milMo_Wearable = milMo_ShopItem.Item as MilMo_Wearable;
				MilMo_Furniture milMo_Furniture = milMo_ShopItem.Item as MilMo_Furniture;
				MilMo_HomeSurface milMo_HomeSurface = milMo_ShopItem.Item as MilMo_HomeSurface;
				MilMo_ShopRoom milMo_ShopRoom = milMo_ShopItem.Item as MilMo_ShopRoom;
				if (milMo_Wearable != null)
				{
					foreach (MilMo_BodyPack item in MilMo_Player.Instance.Avatar.BodyPackManager.Equipped)
					{
						if (milMo_Wearable.BodyPack != null && item.Identifier == milMo_Wearable.BodyPack.Identifier)
						{
							flag = true;
						}
					}
					if (_currentlyHideFromSelection.Contains(milMo_ShopItem.Item))
					{
						flag = false;
					}
				}
				else if ((milMo_Furniture != null || milMo_HomeSurface != null || milMo_ShopRoom != null) && !_currentlyHideFromSelection.Contains(milMo_ShopItem.Item))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				tryButtonList.Remove(milMo_Button);
				_ui.RemoveChild(milMo_Button);
			}
		}
		int num2 = 0;
		foreach (MilMo_Button tryButton in tryButtonList)
		{
			num2++;
			tryButton.GoTo(screenWidth - 50, 50 + (tryButtonList.Count - num2) * 60);
			tryButton.ScaleToAbsolute(_ui.ScaleToLowestUIRes(60f, 60f));
		}
	}

	private void RefreshSelection()
	{
		HideSelectMenu();
		RefreshTryButtons();
		if (tryButtonList.Count < 1)
		{
			_selectedItem = null;
			_pulseCursor.SetEnabled(e: false);
			SelectBottomCategory(1);
			UpdateViewMode();
			return;
		}
		if (_selectedItem == null || _currentlyHideFromSelection.Contains(_selectedItem.Item))
		{
			_selectedItem = tryButtonList[tryButtonList.Count - 1].Args as MilMo_ShopItem;
		}
		UpdateViewMode();
		MilMo_Wearable milMo_Wearable = _selectedItem.Item as MilMo_Wearable;
		MilMo_Furniture milMo_Furniture = _selectedItem.Item as MilMo_Furniture;
		MilMo_HomeSurface milMo_HomeSurface = _selectedItem.Item as MilMo_HomeSurface;
		MilMo_ShopRoom milMo_ShopRoom = _selectedItem.Item as MilMo_ShopRoom;
		if (milMo_Wearable == null && milMo_Furniture == null && milMo_HomeSurface == null && milMo_ShopRoom == null)
		{
			_selectedItem = null;
			_pulseCursor.SetEnabled(e: false);
			SelectBottomCategory(1);
			return;
		}
		if (milMo_Wearable != null && milMo_Wearable.BodyPack.ColorGroups.Count > 0)
		{
			SelectBottomCategory(0);
		}
		else if (milMo_Furniture != null && milMo_Furniture.HomePack.ColorGroups.Count > 0)
		{
			SelectBottomCategory(0);
		}
		else if (milMo_HomeSurface != null && milMo_HomeSurface.HomePackBase.ColorGroups.Count > 0)
		{
			SelectBottomCategory(0);
		}
		else
		{
			SelectBottomCategory(1);
		}
		ShowSelectMenu();
		foreach (MilMo_Button tryButton in tryButtonList)
		{
			if (tryButton.Args as MilMo_ShopItem == _selectedItem)
			{
				tryButton.SetDefaultColor(1f, 1f, 1f, 1f);
				_pulseCursor.SetEnabled(e: true);
				_pulseCursor.ScaleImpulse(10f, 10f);
				_pulseCursor.GetPosFrom = tryButton;
				_pulseCursor.ColorNow(1f, 1f, 1f, 0.75f);
				_pulseCursor.ColorTo(1f, 1f, 1f, 0.35f);
				_ui.BringToFront(_pulseCursor);
			}
			else
			{
				tryButton.SetDefaultColor(1f, 1f, 1f, 0.7f);
				tryButton.SetHoverColor(1f, 1f, 1f, 1f);
			}
		}
	}

	private void UpdateSelectMenuPos()
	{
		foreach (MilMo_Button item in tryButtonList.Where((MilMo_Button but) => _selectedItem == but.Args))
		{
			_selectMenu.GetPosFrom = item;
		}
	}

	private void HideSelectMenu()
	{
		if (!MilMo_Config.Instance.IsTrue("Launcher.OfflineShop", defaultValue: false))
		{
			_selectedBuyButton.AllowPointerFocus = false;
			_selectedGiftButton.AllowPointerFocus = false;
			_selectedRemoveButton.AllowPointerFocus = false;
			_selectMenu.FadeSpeed = 0.05f;
			_selectMenu.AlphaTo(0f);
			_selectMenu.ScaleTo(0f, 68f);
		}
	}

	private void InstantHideSelectMenu()
	{
		_selectedBuyButton.AllowPointerFocus = false;
		_selectedGiftButton.AllowPointerFocus = false;
		_selectedRemoveButton.AllowPointerFocus = false;
		_selectMenu.SetAlpha(0f);
		_selectedBuyButton.SetAlpha(0f);
		_selectedGiftButton.SetAlpha(0f);
		_selectedRemoveButton.SetAlpha(0f);
	}

	private void ShowSelectMenu()
	{
		UpdateSelectMenuPos();
		_selectMenu.SetEnabled(e: true);
		if (_allowSelectMenuInputTimer != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_allowSelectMenuInputTimer);
		}
		_allowSelectMenuInputTimer = MilMo_EventSystem.At(0f, _allowSelectMenuInput);
		_selectMenu.FadeSpeed = 0.025f;
		_selectMenu.AlphaTo(1f);
		_selectMenu.ScaleMover.Arrive = Nothing;
		_selectMenu.ScaleTo(144f, 68f);
	}

	private void OnAllowSelectMenuInput()
	{
		_selectedBuyButton.AllowPointerFocus = true;
		_selectedGiftButton.AllowPointerFocus = true;
		_selectedRemoveButton.AllowPointerFocus = true;
	}

	private void OnRefreshApplied(MilMo_Avatar unused1, string unused2)
	{
		OnRefreshApplied();
	}

	private void OnRefreshApplied()
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Swap);
		RefreshSelection();
	}

	private void BrowseToItemCard(object o)
	{
		BrowseToItemCard(o, mayOpenMembershipScreen: true);
	}

	private void BrowseToItemCard(object o, bool mayOpenMembershipScreen)
	{
		if (!_allowBrowsingItemCards)
		{
			return;
		}
		_allowBrowsingItemCards = false;
		MilMo_EventSystem.At(1f, _allowBrowsingItemCardsCallback);
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
		int id = (int)o;
		int num = 0;
		int num2 = 0;
		MilMo_ItemCard milMo_ItemCard = null;
		foreach (MilMo_CharacterShopCategory category in _categoryList)
		{
			num2 = 0;
			foreach (MilMo_ScrollView scrollView in category.ScrollViewList)
			{
				foreach (MilMo_Widget child in scrollView.Children)
				{
					if (child is MilMo_ItemCard milMo_ItemCard2 && ((MilMo_ShopItem)milMo_ItemCard2.BuyButton.Args).Id == id)
					{
						milMo_ItemCard = milMo_ItemCard2;
						Try(milMo_ItemCard2.BuyButton.Args, mayOpenMembershipScreen);
						break;
					}
				}
				if (milMo_ItemCard != null)
				{
					break;
				}
				num2++;
			}
			if (milMo_ItemCard != null)
			{
				break;
			}
			num++;
		}
		if (milMo_ItemCard == null)
		{
			MilMo_Dialog failDialog = new MilMo_Dialog(_ui);
			_ui.AddChild(failDialog);
			failDialog.DoWarning(MilMo_Localization.GetLocString("CharacterShop_253"), MilMo_Localization.GetLocString("CharacterShop_254"), delegate
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Info);
				failDialog.CloseAndRemove(null);
			});
			return;
		}
		float[] arg = new float[1] { num };
		float[] arg2 = new float[1] { num2 };
		if (num != CurCategory.Number)
		{
			SelectMainCategory(arg);
		}
		if (num2 != CurCategory.CurrentSubCategory)
		{
			SelectSubCategory(arg2);
		}
		if (milMo_ItemCard.Pos.y < CurCategory.ScrollViewList[num2].SoftScroll.Val.y)
		{
			CurCategory.ScrollViewList[num2].ScrollTo(0f, milMo_ItemCard.Pos.y - 20f);
		}
		if (milMo_ItemCard.Pos.y > CurCategory.ScrollViewList[num2].SoftScroll.Val.y + 356f)
		{
			CurCategory.ScrollViewList[num2].ScrollTo(0f, milMo_ItemCard.Pos.y - 336f);
		}
		if (_curBottomCat == 1)
		{
			foreach (MilMo_Widget item in _bottomScroller.Children.Where((MilMo_Widget hotItem) => hotItem.Info2 == id))
			{
				if (item.Pos.x - item.ScaleMover.Target.x / 2f < _bottomScroller.SoftScroll.Val.x)
				{
					_bottomScroller.ScrollTo(item.Pos.x - 20f, 0f);
				}
				if (item.Pos.x + item.ScaleMover.Target.x / 2f > _bottomScroller.SoftScroll.Val.x + 386f)
				{
					_bottomScroller.ScrollTo(item.Pos.x - 346f, 0f);
				}
				item.ScaleImpulse(8f, 8f);
			}
		}
		milMo_ItemCard.Flash();
	}

	private void BringPanesToFront()
	{
		_ui.BringToFront(_fadeTop);
		_ui.BringToFront(_fadeBottom);
		_ui.BringToFront(helpCaption);
		_ui.BringToFront(_captionCharge);
		_ui.BringToFront(topDiv);
		_ui.BringToFront(_chargeBack);
		_ui.BringToFront(_currencyIcon);
		_ui.BringToFront(_ingameCoinsIcon);
		_ui.BringToFront(_accountBalance);
		_ui.BringToFront(_ingameCoinsAccountBalance);
		_ui.BringToFront(_mChargeButton);
	}

	private void AllowBrowsingItemCards()
	{
		_allowBrowsingItemCards = true;
	}

	private void UpdateViewMode()
	{
		ViewMode viewMode = ViewMode.Character;
		if (_selectedItem != null && _selectedItem.Item != null)
		{
			MilMo_Wearable milMo_Wearable = _selectedItem.Item as MilMo_Wearable;
			MilMo_Furniture milMo_Furniture = _selectedItem.Item as MilMo_Furniture;
			MilMo_HomeSurface milMo_HomeSurface = _selectedItem.Item as MilMo_HomeSurface;
			MilMo_ShopRoom milMo_ShopRoom = _selectedItem.Item as MilMo_ShopRoom;
			if (milMo_Wearable == null && milMo_Furniture == null && milMo_HomeSurface == null && milMo_ShopRoom == null)
			{
				return;
			}
			if (milMo_Furniture != null)
			{
				viewMode = ViewMode.Furniture;
			}
			else if (milMo_Wearable != null)
			{
				_selectedItem.Mover = _characterMover;
				viewMode = ViewMode.Character;
			}
			else if (milMo_HomeSurface != null)
			{
				if (milMo_HomeSurface is MilMo_Wallpaper)
				{
					viewMode = ViewMode.Wallpaper;
				}
				else if (milMo_HomeSurface is MilMo_Floor)
				{
					viewMode = ViewMode.Floor;
				}
			}
			else
			{
				viewMode = ViewMode.Furniture;
			}
		}
		if (viewMode != _viewMode)
		{
			_viewMode = viewMode;
			ViewModeChanged();
		}
		else if (_viewMode == ViewMode.Furniture)
		{
			MilMo_ObjectMover mover = _mover;
			float z = ((_selectedItem != null && _selectedItem.Item is MilMo_ShopRoom) ? 0.7f : 1f);
			Vector3 pos = new Vector3(-3f, -50f, z) + MilMo_ShopCameraController.ShopPosition;
			Vector3 pos2 = new Vector3(3f, -50f, mover.Pos.z) + MilMo_ShopCameraController.ShopPosition;
			if (_selectedItem != null)
			{
				if (_selectedItem.Mover == null)
				{
					_selectedItem.Mover = new MilMo_ObjectMover();
				}
				_mover = _selectedItem.Mover;
				Vector3 pos3 = new Vector3(0f, -50.4f, z) + MilMo_ShopCameraController.ShopPosition;
				if (!_allMovers.Contains(_mover))
				{
					_allMovers.Add(_mover);
				}
				if (_selectedItem.Item is MilMo_WallFurniture)
				{
					MilMo_World.Instance.Camera.shopCameraController.SetViewModeWallFurniture();
				}
				else if (_selectedItem.Item is MilMo_ShopRoom)
				{
					MilMo_World.Instance.Camera.shopCameraController.SetViewModeRoom();
				}
				else
				{
					MilMo_World.Instance.Camera.shopCameraController.SetViewModeFloorFurniture();
				}
				mover.GoTo(pos2);
				_mover.GoToNow(pos);
				_mover.GoTo(pos3);
				_mover.SetUpdateFunc(2);
				_mover.Pull = 0.5f;
				_mover.Drag = 0.2f;
				_mover.AnglePull = 0.035f;
				_mover.AngleDrag = 0.6f;
				_objectAngle = ((_selectedItem.Item is MilMo_WallFurniture) ? 270f : 208f);
			}
			_mover.SetAngle(0f, _objectAngle, 0f);
		}
		RefreshWallObject();
	}

	private void ViewModeChanged()
	{
		float z = ((_selectedItem != null && _selectedItem.Item is MilMo_ShopRoom) ? 0.7f : 1f);
		MilMo_ObjectMover mover = _mover;
		Vector3 pos = new Vector3(-3f, -50f, z) + MilMo_ShopCameraController.ShopPosition;
		Vector3 pos2 = Vector3.zero;
		Vector3 pos3 = new Vector3(3f, -50f, mover.Pos.z) + MilMo_ShopCameraController.ShopPosition;
		switch (_viewMode)
		{
		case ViewMode.Character:
			_mover = _characterMover;
			pos2 = new Vector3(0f, -50f, z) + MilMo_ShopCameraController.ShopPosition;
			MilMo_World.Instance.Camera.shopCameraController.SetViewModeCharacter();
			break;
		case ViewMode.Floor:
			_mover = _floorMover;
			pos2 = new Vector3(0f, -50.2f, z) + MilMo_ShopCameraController.ShopPosition;
			MilMo_World.Instance.Camera.shopCameraController.SetViewModeFloor();
			break;
		case ViewMode.Furniture:
			if (_selectedItem != null)
			{
				if (_selectedItem.Mover == null)
				{
					_selectedItem.Mover = new MilMo_ObjectMover();
				}
				_mover = _selectedItem.Mover;
				pos2 = new Vector3(0f, -50.4f, z) + MilMo_ShopCameraController.ShopPosition;
				if (!_allMovers.Contains(_mover))
				{
					_allMovers.Add(_mover);
				}
				if (_selectedItem.Item is MilMo_WallFurniture)
				{
					MilMo_World.Instance.Camera.shopCameraController.SetViewModeWallFurniture();
				}
				else if (_selectedItem.Item is MilMo_ShopRoom)
				{
					MilMo_World.Instance.Camera.shopCameraController.SetViewModeRoom();
				}
				else
				{
					MilMo_World.Instance.Camera.shopCameraController.SetViewModeFloorFurniture();
				}
			}
			break;
		case ViewMode.Wallpaper:
			_mover = _wallpaperMover;
			pos2 = new Vector3(0f, -50.2f, z) + MilMo_ShopCameraController.ShopPosition;
			MilMo_World.Instance.Camera.shopCameraController.SetViewModeWallpaper();
			break;
		}
		mover.GoTo(pos3);
		_mover.GoToNow(pos);
		_mover.GoTo(pos2);
		_mover.SetUpdateFunc(2);
		_mover.Pull = 0.5f;
		_mover.Drag = 0.2f;
		_mover.AnglePull = 0.035f;
		_mover.AngleDrag = 0.6f;
		if (_selectedItem != null && _selectedItem.Item is MilMo_WallFurniture)
		{
			_objectAngle = 270f;
		}
		else
		{
			_objectAngle = 208f;
		}
		_mover.SetAngle(0f, _objectAngle, 0f);
		if (_viewMode == ViewMode.Character)
		{
			_shopLight.SetActive(value: true);
			_shopLightHomeObjects.SetActive(value: false);
			RenderSettings.ambientLight = _defaultAmbientColor;
		}
		else
		{
			_shopLight.SetActive(value: false);
			_shopLightHomeObjects.SetActive(value: true);
			RenderSettings.ambientLight = MilMo_Home.AmbientLightColor;
		}
		RefreshWallObject();
	}

	private void SwitchFurniture()
	{
		ViewModeChanged();
	}

	private void DisableOffScreenIcons()
	{
		foreach (MilMo_CharacterShopCategory category in _categoryList)
		{
			if (category.Button.PosMover.Target.x > CurCategory.Button.PosMover.Target.x + 200f * _ui.Res.x)
			{
				category.Button.GoTo((float)category.Number * 80f + 65f + 160f, _mainCatIconHeight);
				category.ButtonReflect.GoTo((float)category.Number * 80f + 65f + 160f, 120f - _mainCatIconHeight);
			}
			if (category.Button.PosMover.Target.x < CurCategory.Button.PosMover.Target.x - 200f * _ui.Res.x)
			{
				category.Button.GoTo((float)category.Number * 80f + 65f + 160f, _mainCatIconHeight);
				category.ButtonReflect.GoTo((float)category.Number * 80f + 65f + 160f, 120f - _mainCatIconHeight);
			}
		}
	}

	private void GrowJuneCoinIcon()
	{
		_mChargeButton.SetFontScale(1f);
		_currencyIcon.SetScalePull(0.1f, 0.1f);
		_currencyIcon.SetScaleDrag(0.85f, 0.85f);
		_currencyIcon.ScaleImpulse(0f, 7f);
		_currencyIcon.SetScalePull(0.03f, 0.03f);
		_currencyIcon.SetScaleDrag(0.85f, 0.85f);
		_accountBalance.ImpulseRandom(2f, 2f, 0f, 0f);
		_accountBalance.SetPosPull(0.1f, 0.1f);
		_accountBalance.SetPosDrag(0.75f, 0.75f);
	}

	private void ShrinkJuneCoinIcon()
	{
		_mChargeButton.SetFontScale(0.8f);
		_currencyIcon.SetScalePull(0.1f, 0.1f);
		_currencyIcon.SetScaleDrag(0.85f, 0.85f);
	}

	private void RefreshAccountBalanceScale()
	{
		if (_accountBalance.Text.Length < 5)
		{
			_accountBalance.SetFontScale(0.9f, 1f);
			_accountBalance.SetTextOffset(-10f, -7f);
		}
		else if (_accountBalance.Text.Length < 6)
		{
			_accountBalance.SetFontScale(0.85f, 1f);
			_accountBalance.SetTextOffset(-10f, -7f);
		}
		else if (_accountBalance.Text.Length < 7)
		{
			_accountBalance.SetFontScale(0.75f, 1f);
			_accountBalance.SetTextOffset(-10f, -7f);
		}
		else if (_accountBalance.Text.Length < 8)
		{
			_accountBalance.SetFontScale(0.65f, 1f);
			_accountBalance.SetTextOffset(-10f, -7f);
		}
		else
		{
			_accountBalance.SetFontScale(0.58f, 1f);
			_accountBalance.SetTextOffset(-10f, -7f);
		}
	}

	private void RefreshInGameAccountBalanceScale()
	{
		if (_ingameCoinsAccountBalance.Text.Length < 5)
		{
			_ingameCoinsAccountBalance.SetFontScale(0.99f, 1.1f);
			_ingameCoinsAccountBalance.SetTextOffset(-5f, 0f);
		}
		else if (_ingameCoinsAccountBalance.Text.Length < 6)
		{
			_ingameCoinsAccountBalance.SetFontScale(0.93500006f, 1.1f);
			_ingameCoinsAccountBalance.SetTextOffset(-5f, 0f);
		}
		else if (_ingameCoinsAccountBalance.Text.Length < 7)
		{
			_ingameCoinsAccountBalance.SetFontScale(0.82500005f, 1.1f);
			_ingameCoinsAccountBalance.SetTextOffset(-5f, 0f);
		}
		else if (_ingameCoinsAccountBalance.Text.Length < 8)
		{
			_ingameCoinsAccountBalance.SetFontScale(0.715f, 1.1f);
			_ingameCoinsAccountBalance.SetTextOffset(-5f, 0f);
		}
		else
		{
			_ingameCoinsAccountBalance.SetFontScale(0.638f, 1.1f);
			_ingameCoinsAccountBalance.SetTextOffset(-5f, 0f);
		}
	}

	private void ShowBackArrow()
	{
		if (!_goingBackToGame)
		{
			_backButton.SetFontScale(1f);
			_returnToLevelName.SetDefaultTextColor(1f, 1f, 0f, 1f);
			_backArrow.ColorNow(1f, 1f, 0f, 0f);
			_backArrow.ColorTo(1f, 1f, 0f, 1f);
			_backArrow.GoToNow(screenWidth - 74, screenHeight - 85);
			_backArrow.GoTo(screenWidth - 14, screenHeight - 85);
			_returnToLevelIcon.ColorNow(1f, 1f, 1f, 0f);
			_returnToLevelIcon.ColorTo(1f, 1f, 1f, 1f);
			_returnToLevelIcon.GoToNow(screenWidth - 104, screenHeight - 85);
			_returnToLevelIcon.GoTo(screenWidth - 124, screenHeight - 85);
			_returnToLevelIcon.ScaleNow(0f, 0f);
			_returnToLevelIcon.ScaleToAbsolute(_ui.ScaleToLowestUIRes(50f, 50f));
			_returnToLevelIcon.SetScalePull(0.03f, 0.03f);
			_returnToLevelIcon.SetScaleDrag(0.85f, 0.85f);
		}
	}

	private void HideBackArrow()
	{
		_backButton.SetFontScale(0.8f);
		_returnToLevelName.SetDefaultTextColor(1f, 1f, 1f, 0.5f);
		if (_backArrow != null)
		{
			_backArrow.ColorTo(1f, 1f, 0f, 0f);
			_backArrow.GoTo(screenWidth - 74, screenHeight - 85);
			_returnToLevelIcon.ColorTo(1f, 1f, 0f, 0f);
			_returnToLevelIcon.GoTo(screenWidth - 104, screenHeight - 85);
			_returnToLevelIcon.ScaleTo(0f, 0f);
			_returnToLevelIcon.SetScalePull(0.03f, 0.03f);
			_returnToLevelIcon.SetScaleDrag(0.85f, 0.85f);
		}
	}

	private void GoBackToGame(object arg)
	{
		if (MilMo_Player.Instance.OkToLeaveShop())
		{
			_goingBackToGame = true;
			_backButton.SetHoverTexture("Batch01/Textures/Shop/BackButton");
			_backButton.SetFontScale(0.8f);
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
			if (_backArrow != null)
			{
				_backArrow.ColorTo(1f, 1f, 1f, 1f);
				_backArrow.GoTo(screenWidth - 26, screenHeight - 85);
				_returnToLevelIcon.ColorTo(1f, 1f, 0f, 0f);
				_returnToLevelIcon.ScaleTo(1024f, 1024f);
				_returnToLevelIcon.SetScalePull(0.03f, 0.03f);
				_returnToLevelIcon.SetScaleDrag(0.65f, 0.65f);
			}
			MilMo_Fade.Instance.FadeInBackground();
			MilMo_EventSystem.At(1f, MilMo_Player.Instance.RequestLeaveShop);
		}
	}

	private void DoNoJuneCoinAlert()
	{
		MilMo_EventSystem.At(1f, delegate
		{
			FlashJuneCoinPanel();
			GrowJuneCoinIcon();
			_mChargeButton.SetFontScale(0.8f);
		});
		MilMo_EventSystem.At(1.3f, FlashJuneCoinPanel);
		MilMo_EventSystem.At(1.6f, delegate
		{
			FlashJuneCoinPanel();
			ShrinkJuneCoinIcon();
		});
	}

	private void FlashJuneCoinPanel()
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Attention);
		_captionCharge.SetColor(1f, 0f, 0f, 1f);
		_captionCharge.ColorTo(1f, 1f, 1f, 1f);
		_chargeBack.SetColor(1f, 0f, 0f, 1f);
		_chargeBack.ColorTo(1f, 1f, 1f, 1f);
		_mChargeButton.SetDefaultColor(1f, 1f, 1f, 1f);
		_mChargeButton.SetColor(1f, 0f, 0f, 1f);
	}

	private void RefreshAccountBalance(object o)
	{
		if (base.enabled && MilMo_Monetization.Instance.Currency != null)
		{
			MilMo_Monetization.Instance.Currency.RefreshAccountBalance();
		}
	}

	private static void Nothing()
	{
	}

	public static string Debug_ClearRemoteData(string[] args)
	{
		ClearShopData();
		return "Remote shop data cleared.";
	}

	private void InitUI()
	{
		UpdateRes();
		_mainCatIconHeight = -90f;
		_leftFade = new MilMo_Widget(_ui);
		_leftFade.SetTexture("Batch01/Textures/Generic/HorizontalGradient");
		_leftFade.SetDefaultColor(0f, 0f, 0f, 1f);
		_leftFade.SetAlignment(MilMo_GUI.Align.TopLeft);
		_leftFade.AllowPointerFocus = false;
		_ui.AddChild(_leftFade);
		_rightFade = new MilMo_Widget(_ui);
		_rightFade.SetTexture("Batch01/Textures/Generic/HorizontalGradient");
		_rightFade.SetDefaultColor(0f, 0f, 0f, 1f);
		_rightFade.SetAlignment(MilMo_GUI.Align.TopLeft);
		_rightFade.AllowPointerFocus = false;
		_ui.AddChild(_rightFade);
		_topFade = new MilMo_Widget(_ui);
		_topFade.SetTexture("Batch01/Textures/Shop/FadeBottom");
		_topFade.SetDefaultColor(1f, 1f, 1f, 1f);
		_topFade.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_topFade.AllowPointerFocus = false;
		_ui.AddChild(_topFade);
		_bottomFade = new MilMo_Widget(_ui);
		_bottomFade.SetTexture("Batch01/Textures/Shop/FadeBottom");
		_bottomFade.SetDefaultColor(1f, 1f, 1f, 1f);
		_bottomFade.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_bottomFade.AllowPointerFocus = false;
		_ui.AddChild(_bottomFade);
		_loadingPane = new MilMo_LoadingPane(_ui);
		_ui.AddChild(_loadingPane);
		_topBack = new MilMo_Widget(_ui);
		_topBack.SetTexture("Batch01/Textures/Shop/TopBackground");
		_topBack.SetPosition(0f, 0f);
		_topBack.SetAlignment(MilMo_GUI.Align.TopLeft);
		_topBack.SetScale(450f, 82f);
		_ui.AddChild(_topBack);
		_captionColor = new Color[7];
		_captionColor[0] = new Color(0.5f, 0.9f, 0.5f, 0.9f);
		_captionColor[1] = new Color(1f, 0.5f, 0.5f, 0.9f);
		_captionColor[2] = new Color(0.7f, 0.7f, 1f, 0.9f);
		_captionColor[3] = new Color(1f, 0.8f, 0.3f, 0.9f);
		_captionColor[4] = new Color(1f, 0.6f, 0.6f, 0.9f);
		_captionColor[5] = new Color(0.9f, 0.7f, 0.5f, 0.9f);
		_captionColor[6] = new Color(1f, 1f, 1f, 0.9f);
		_charSpinner = new MilMo_Widget(_ui)
		{
			Identifier = "CharSpinner"
		};
		_charSpinner.SetTexture("Batch01/Textures/Core/Invisible");
		_charSpinner.SetAlignment(MilMo_GUI.Align.TopLeft);
		_ui.AddChild(_charSpinner);
		_ui.ForceCameraWidget = _charSpinner;
		_mainCatScroller = new MilMo_ScrollView(_ui);
		_mainCatScroller.SetText(MilMo_LocString.Empty);
		_mainCatScroller.SetPosition(0f, 0f);
		_mainCatScroller.SetScale(450f, Screen.height + 50);
		_mainCatScroller.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mainCatScroller.SetTexture("Batch01/Textures/Core/Invisible");
		_mainCatScroller.ColorNow(1f, 1f, 1f, 0f);
		_mainCatScroller.SetViewSize(10000f, 200f);
		_mainCatScroller.HasBackground(b: false);
		_mainCatScroller.IsUserControlled(b: false);
		_mainCatScroller.ScrollToNow(0f, 100f);
		_mainCatScroller.ScrollTo(0f, 0f);
		_mainCatScroller.SoftScroll.Arrive = MainCatSoftScrollArrive;
		_mainCatScroller.MouseWheelScrollable = false;
		_ui.AddChild(_mainCatScroller);
		_iconLeftArrow = new MilMo_Button(_ui);
		_iconLeftArrow.SetTexture("Batch01/Textures/Shop/TabArrowLeft");
		_iconLeftArrow.SetHoverTexture("Batch01/Textures/Shop/TabArrowLeftMO");
		_iconLeftArrow.SetPressedTexture("Batch01/Textures/Shop/TabArrowLeftMO");
		_iconLeftArrow.SetHoverColor(1f, 1f, 1f, 1f);
		_iconLeftArrow.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		_iconLeftArrow.SetFadeInSpeed(0.05f);
		_iconLeftArrow.SetFadeOutSpeed(1f);
		_iconLeftArrow.SetDefaultColor(0.5f, 0.5f, 0.5f, 1f);
		_iconLeftArrow.SetPosition(0f, 20f);
		_iconLeftArrow.SetDefaultScale(_arrowSize);
		_iconLeftArrow.SetScale(_arrowSize.x, 0f);
		_iconLeftArrow.ScaleMover.Vel.x = 0f;
		_iconLeftArrow.ScaleMover.Vel.y = 0f;
		_iconLeftArrow.SetAlignment(MilMo_GUI.Align.TopLeft);
		_iconLeftArrow.Function = ClkPrevMainCategory;
		_iconLeftArrow.SetScalePull(0.1f, 0.1f);
		_iconLeftArrow.SetScaleDrag(0.55f, 0.55f);
		_ui.AddChild(_iconLeftArrow);
		_iconRightArrow = new MilMo_Button(_ui);
		_iconRightArrow.SetTexture("Batch01/Textures/Shop/TabArrowRight");
		_iconRightArrow.SetHoverTexture("Batch01/Textures/Shop/TabArrowRightMO");
		_iconRightArrow.SetPressedTexture("Batch01/Textures/Shop/TabArrowRightMO");
		_iconRightArrow.SetHoverColor(1f, 1f, 1f, 1f);
		_iconRightArrow.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		_iconRightArrow.SetFadeInSpeed(0.05f);
		_iconRightArrow.SetFadeOutSpeed(1f);
		_iconRightArrow.SetDefaultColor(0.5f, 0.5f, 0.5f, 1f);
		_iconRightArrow.SetPosition(433f, 20f);
		_iconRightArrow.SetAlignment(MilMo_GUI.Align.TopLeft);
		_iconRightArrow.SetDefaultScale(_arrowSize);
		_iconRightArrow.SetScale(_arrowSize.x, 0f);
		_iconRightArrow.Function = ClkNextMainCategory;
		_iconRightArrow.SetScalePull(0.1f, 0.1f);
		_iconRightArrow.SetScaleDrag(0.55f, 0.55f);
		_ui.AddChild(_iconRightArrow);
		_coverFlowGradient = new MilMo_Widget(_ui);
		_coverFlowGradient.SetTexture("Batch01/Textures/Shop/CoverFlowGradient");
		_coverFlowGradient.SetPosition(0f, 52f);
		_coverFlowGradient.SetAlignment(MilMo_GUI.Align.TopLeft);
		_coverFlowGradient.SetScale(450f, 32f);
		_ui.AddChild(_coverFlowGradient);
		mainCaptionBack = new MilMo_Widget(_ui);
		mainCaptionBack.SetTexture("Batch01/Textures/Shop/MainCaptionBackground");
		mainCaptionBack.SetPosition(0f, 82f);
		mainCaptionBack.SetAlignment(MilMo_GUI.Align.TopLeft);
		mainCaptionBack.SetScale(450f, 64f);
		mainCaptionBack.FadeToDefaultColor = false;
		mainCaptionBack.SetFadeSpeed(0.02f);
		_ui.AddChild(mainCaptionBack);
		_mainCaption = new MilMo_Widget(_ui);
		_mainCaption.SetTexture("Batch01/Textures/Core/Invisible");
		_mainCaption.SetText(MilMo_Localization.GetLocString("CharacterShop_241"));
		_mainCaption.SetPosition(0f, 82f);
		_mainCaption.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_mainCaption.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mainCaption.SetScale(250f, 32f);
		_mainCaption.SetTextOffset(0f, 0f);
		_mainCaption.ColorNow(1f, 1f, 1f, 1f);
		_mainCaption.SetPosPull(0.06f, 0.06f);
		_mainCaption.SetPosDrag(0.6f, 0.6f);
		_mainCaption.SetFadeSpeed(0.03f);
		_mainCaption.SetFont(MilMo_GUI.Font.GothamLarge);
		_ui.AddChild(_mainCaption);
		captionShine = new MilMo_Widget(_ui);
		captionShine.SetTexture("Batch01/Textures/Shop/CaptionShine");
		captionShine.SetPosition(450f, 82f);
		captionShine.GoTo(250f, 82f);
		captionShine.SetScale(250f, 8f);
		captionShine.SetAlignment(MilMo_GUI.Align.TopRight);
		captionShine.SetPosPull(0.1f, 0.1f);
		captionShine.SetPosDrag(0.35f, 0.35f);
		_ui.AddChild(captionShine);
		_middleIcon = new MilMo_Widget(_ui);
		_middleIcon.SetDefaultColor(1f, 1f, 1f, 1f);
		_middleIcon.SetAlignment(MilMo_GUI.Align.TopCenter);
		_middleIcon.SetPosition(225f, 0f);
		_middleIcon.PosMover.Pull.x = _mainCatScroller.SoftScroll.Pull.x;
		_middleIcon.PosMover.Drag.x = _mainCatScroller.SoftScroll.Drag.x;
		_middleIcon.PosMover.Pull.y = _mainCatScroller.SoftScroll.Pull.y;
		_middleIcon.PosMover.Drag.y = _mainCatScroller.SoftScroll.Drag.y;
		_middleIcon.SetPosPull(_mainCatScroller.SoftScroll.Pull.x / _ui.Res.x * 1f, _mainCatScroller.SoftScroll.Pull.y / _ui.Res.y);
		_middleIcon.SetPosDrag(_mainCatScroller.SoftScroll.Drag.x / _ui.Res.x * 1f, _mainCatScroller.SoftScroll.Drag.y / _ui.Res.y);
		_middleIcon.SetScaleDrag(0.5f, 0.7f);
		_middleIcon.SetScalePull(0.05f, 0.07f);
		_middleIcon.ScaleTo(100f, 100f);
		_middleIcon.SetMinScale(60f, 60f);
		_ui.AddChild(_middleIcon);
		_subCatScroller = new MilMo_ScrollView(_ui);
		_subCatScroller.SetText(MilMo_LocString.Empty);
		_subCatScroller.SetPosition(0f, 114f);
		_subCatScroller.SetScale(435f, Screen.height + 50);
		_subCatScroller.SetAlignment(MilMo_GUI.Align.TopLeft);
		_subCatScroller.SetTexture("Batch01/Textures/Core/BlackTransparent");
		_subCatScroller.ColorNow(1f, 1f, 1f, 1f);
		_subCatScroller.SetViewSize(1000f, 10f);
		_subCatScroller.HasBackground(b: false);
		_subCatScroller.IsUserControlled(b: false);
		_subCatScroller.SetScrollPull(0.1f, 0.1f);
		_subCatScroller.SetScrollDrag(0.55f, 0.55f);
		_subCatScroller.MouseWheelScrollable = false;
		_ui.AddChild(_subCatScroller);
		_tabLeftArrow = new MilMo_Button(_ui);
		_tabLeftArrow.SetTexture("Batch01/Textures/Shop/TabArrowLeft");
		_tabLeftArrow.SetHoverTexture("Batch01/Textures/Shop/TabArrowLeftMO");
		_tabLeftArrow.SetPressedTexture("Batch01/Textures/Shop/TabArrowLeftMO");
		_tabLeftArrow.SetHoverColor(1f, 1f, 1f, 1f);
		_tabLeftArrow.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		_tabLeftArrow.SetFadeInSpeed(0.05f);
		_tabLeftArrow.SetFadeOutSpeed(1f);
		_tabLeftArrow.SetDefaultColor(0.5f, 0.5f, 0.5f, 1f);
		_tabLeftArrow.SetPosition(8f, 110f);
		_tabLeftArrow.SetDefaultScale(_arrowSize);
		_tabLeftArrow.SetScale(_arrowSize.x, 0f);
		_tabLeftArrow.ScaleMover.Vel.x = 0f;
		_tabLeftArrow.ScaleMover.Vel.y = 0f;
		_tabLeftArrow.SetAlignment(MilMo_GUI.Align.TopLeft);
		_tabLeftArrow.Function = ClkPrevSubCategory;
		_tabLeftArrow.SetScalePull(0.1f, 0.1f);
		_tabLeftArrow.SetScaleDrag(0.55f, 0.55f);
		_ui.AddChild(_tabLeftArrow);
		_tabRightArrow = new MilMo_Button(_ui);
		_tabRightArrow.SetTexture("Batch01/Textures/Shop/TabArrowRight");
		_tabRightArrow.SetHoverTexture("Batch01/Textures/Shop/TabArrowRightMO");
		_tabRightArrow.SetPressedTexture("Batch01/Textures/Shop/TabArrowRightMO");
		_tabRightArrow.SetHoverColor(1f, 1f, 1f, 1f);
		_tabRightArrow.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		_tabRightArrow.SetFadeInSpeed(0.05f);
		_tabRightArrow.SetFadeOutSpeed(1f);
		_tabRightArrow.SetDefaultColor(0.5f, 0.5f, 0.5f, 1f);
		_tabRightArrow.SetPosition(425f, 110f);
		_tabRightArrow.SetAlignment(MilMo_GUI.Align.TopLeft);
		_tabRightArrow.SetDefaultScale(_arrowSize);
		_tabRightArrow.SetScale(_arrowSize.x, 0f);
		_tabRightArrow.Function = ClkNextSubCategory;
		_tabRightArrow.SetScalePull(0.1f, 0.1f);
		_tabRightArrow.SetScaleDrag(0.55f, 0.55f);
		_ui.AddChild(_tabRightArrow);
		_back1 = new MilMo_Widget(_ui);
		_back1.SetTexture("Batch01/Textures/Shop/Background");
		_back1.SetPosition(0f, 154.76276f);
		_back1.SetAlignment(MilMo_GUI.Align.TopLeft);
		_back1.SetScale(450f, 128f);
		_back1.SetCropMode(MilMo_GUI.CropMode.Crop);
		_ui.AddChild(_back1);
		_back2 = new MilMo_Widget(_ui);
		_back2.SetTexture("Batch01/Textures/Shop/Background");
		_back2.SetPosition(0f, 282.76276f);
		_back2.SetAlignment(MilMo_GUI.Align.TopLeft);
		_back2.SetScale(450f, 128f);
		_back2.SetCropMode(MilMo_GUI.CropMode.Crop);
		_ui.AddChild(_back2);
		_back3 = new MilMo_Widget(_ui);
		_back3.SetTexture("Batch01/Textures/Shop/Background");
		_back3.SetPosition(0f, 410.76276f);
		_back3.SetAlignment(MilMo_GUI.Align.TopLeft);
		_back3.SetScale(450f, 128f);
		_back3.SetCropMode(MilMo_GUI.CropMode.Crop);
		_ui.AddChild(_back3);
		_back4 = new MilMo_Widget(_ui);
		_back4.SetTexture("Batch01/Textures/Shop/Background");
		_back4.SetPosition(0f, 538.76276f);
		_back4.SetAlignment(MilMo_GUI.Align.TopLeft);
		_back4.SetScale(450f, 128f);
		_back4.SetCropMode(MilMo_GUI.CropMode.Crop);
		_ui.AddChild(_back4);
		_back5 = new MilMo_Widget(_ui);
		_back5.SetTexture("Batch01/Textures/Shop/Background");
		_back5.SetPosition(0f, 538.76276f);
		_back5.SetAlignment(MilMo_GUI.Align.TopLeft);
		_back5.SetScale(450f, 128f);
		_back5.SetCropMode(MilMo_GUI.CropMode.Crop);
		_ui.AddChild(_back5);
		_back6 = new MilMo_Widget(_ui);
		_back6.SetTexture("Batch01/Textures/Shop/Background");
		_back6.SetPosition(0f, 538.76276f);
		_back6.SetAlignment(MilMo_GUI.Align.TopLeft);
		_back6.SetScale(450f, 128f);
		_back6.SetCropMode(MilMo_GUI.CropMode.Crop);
		_ui.AddChild(_back6);
		_back7 = new MilMo_Widget(_ui);
		_back7.SetTexture("Batch01/Textures/Shop/Background");
		_back7.SetPosition(0f, 538.76276f);
		_back7.SetAlignment(MilMo_GUI.Align.TopLeft);
		_back7.SetScale(450f, 128f);
		_back7.SetCropMode(MilMo_GUI.CropMode.Crop);
		_ui.AddChild(_back7);
		topDiv = new MilMo_Widget(_ui);
		topDiv.SetTexture("Batch01/Textures/Shop/TopDivider");
		topDiv.SetPosition(0f, 146f);
		topDiv.SetAlignment(MilMo_GUI.Align.TopLeft);
		topDiv.SetScale(450f, 9.3748f);
		topDiv.SetCropMode(MilMo_GUI.CropMode.Crop);
		_ui.AddChild(topDiv);
		_fadeTop = new MilMo_Widget(_ui);
		_fadeTop.SetTexture("Batch01/Textures/Shop/FadeTop");
		_fadeTop.SetPosition(0f, 153.5909f);
		_fadeTop.SetAlignment(MilMo_GUI.Align.TopLeft);
		_fadeTop.SetScale(450f, 37.4992f);
		_fadeTop.AllowPointerFocus = false;
		_ui.AddChild(_fadeTop);
		_fadeBottom = new MilMo_Widget(_ui);
		_fadeBottom.SetTexture("Batch01/Textures/Shop/FadeBottom");
		_fadeBottom.SetDefaultColor(0f, 0f, 0f, 1f);
		_fadeBottom.SetPosition(0f, 539f);
		_fadeBottom.SetAlignment(MilMo_GUI.Align.TopLeft);
		_fadeBottom.SetScale(450f, 75f);
		_fadeBottom.AllowPointerFocus = false;
		_ui.AddChild(_fadeBottom);
		_purpleBack = new MilMo_Widget(_ui);
		_purpleBack.SetTexture("Batch01/Textures/Shop/PurpleBack");
		_purpleBack.SetPosition(0f, 654f);
		_purpleBack.SetAlignment(MilMo_GUI.Align.TopLeft);
		_purpleBack.SetScale(323f, 114f);
		_ui.AddChild(_purpleBack);
		_purpleBackRight = new MilMo_Widget(_ui);
		_purpleBackRight.SetTexture("Batch01/Textures/Shop/PurpleBackRight");
		_purpleBackRight.SetPosition(0f, 655f);
		_purpleBackRight.SetAlignment(MilMo_GUI.Align.TopLeft);
		_purpleBackRight.SetScale(8f, 112f);
		_ui.AddChild(_purpleBackRight);
		descPriceTag = new MilMo_Widget(_ui);
		descPriceTag.SetTextureBlack();
		descPriceTag.SetPosition(255f, 613f);
		descPriceTag.SetAlignment(MilMo_GUI.Align.TopCenter);
		descPriceTag.SetScale(128f, 50f);
		descPriceTag.SetText(MilMo_LocString.Empty);
		descPriceTag.FadeToDefaultTextColor = false;
		descPriceTag.FadeToDefaultColor = false;
		descPriceTag.TextColorNow(0.47058824f, 27f / 85f, 36f / 85f, 1f);
		descPriceTag.ColorNow(1f, 1f, 1f, 0f);
		descPriceTag.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		descPriceTag.SetTextOffset(120f, -2f);
		descPriceTag.SetFontScale(1f);
		descPriceTag.SetFont(MilMo_GUI.Font.EborgSmall);
		descPriceTag.SetFadeSpeed(0.01f);
		descPriceTag.SetPosPull(0.08f, 0.08f);
		descPriceTag.SetPosDrag(0.5f, 0.5f);
		descPriceTag.SetScalePull(0.08f, 0.08f);
		descPriceTag.SetScaleDrag(0.4f, 0.4f);
		_ui.AddChild(descPriceTag);
		helpCaption = new MilMo_Widget(_ui);
		helpCaption.SetTexture("Batch01/Textures/Shop/CaptionBarPurple");
		helpCaption.SetPosition(0f, 613f);
		helpCaption.SetAlignment(MilMo_GUI.Align.TopLeft);
		helpCaption.SetScale(323f, 41f);
		helpCaption.SetText(MilMo_Localization.GetLocString("CharacterShop_242"));
		helpCaption.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		helpCaption.SetDefaultTextColor(1f, 1f, 1f, 1f);
		helpCaption.SetTextOffset(20f, 10f);
		helpCaption.SetTextDropShadowPos(2f, 2f);
		helpCaption.SetFont(MilMo_GUI.Font.GothamMedium);
		helpCaption.FadeToDefaultTextColor = false;
		helpCaption.SetFadeSpeed(0.01f);
		_ui.AddChild(helpCaption);
		helpText = new MilMo_Widget(_ui);
		helpText.SetPosition(0f, 654f);
		helpText.SetTexture("Batch01/Textures/Core/Invisible");
		helpText.SetAlignment(MilMo_GUI.Align.TopLeft);
		helpText.SetScale(300f, 112f);
		MilMo_LocString locString = MilMo_Localization.GetLocString("CharacterShop_243");
		locString.SetFormatArgs(MilMo_Monetization.Instance.Currency.Name, MilMo_Monetization.Instance.Currency.ChargeButtonText, MilMo_Monetization.Instance.Currency.Name);
		helpText.SetText(locString);
		helpText.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		helpText.SetWordWrap(w: true);
		helpText.SetTextOffset(20f, 10f);
		helpText.TextColorNow(1f, 1f, 1f, 0.85f);
		helpText.FadeToDefaultTextColor = false;
		helpText.SetFont(MilMo_GUI.Font.ArialRounded);
		helpText.SetFontScale(1f);
		helpText.FadeToDefaultTextColor = false;
		helpText.SetFadeSpeed(0.025f);
		_ui.AddChild(helpText);
		_helpReflect = new MilMo_Widget(_ui);
		_helpReflect.SetTexture("Batch01/Textures/Shop/Reflection");
		_helpReflect.SetPosition(8f, 663f);
		_helpReflect.SetAlignment(MilMo_GUI.Align.TopLeft);
		_helpReflect.SetColor(1f, 1f, 1f, 0.3f);
		_helpReflect.SetScale(220f, 128f);
		_ui.AddChild(_helpReflect);
		_captionCharge = new MilMo_Widget(_ui);
		_captionCharge.SetTexture("Batch01/Textures/Shop/CaptionCharge");
		_captionCharge.SetPosition(322f, 613f);
		_captionCharge.SetAlignment(MilMo_GUI.Align.TopLeft);
		_captionCharge.SetScale(128f, 41f);
		_captionCharge.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_captionCharge.SetTextOffset(0f, -5f);
		_captionCharge.SetTextDropShadowPos(2f, 2f);
		_captionCharge.SetFontScale(0.9f);
		_captionCharge.SetFont(MilMo_GUI.Font.GothamLarge);
		_captionCharge.SetFadeSpeed(0.05f);
		_captionCharge.AllowPointerFocus = false;
		_ui.AddChild(_captionCharge);
		_chargeBack = new MilMo_Widget(_ui);
		_chargeBack.SetTexture("Batch01/Textures/Shop/ChargeBack");
		_chargeBack.SetPosition(322f, 654f);
		_chargeBack.SetAlignment(MilMo_GUI.Align.TopLeft);
		_chargeBack.SetScale(128f, 128f);
		_chargeBack.SetFadeSpeed(0.05f);
		_chargeBack.AllowPointerFocus = false;
		_ui.AddChild(_chargeBack);
		_currencyIcon = new MilMo_Widget(_ui);
		_currencyIcon.SetTexture(MilMo_Monetization.Instance.Currency.IconPath);
		_currencyIcon.SetPosition(348f, 696f);
		_currencyIcon.SetAlignment(MilMo_GUI.Align.BottomCenter);
		_currencyIcon.SetScale(32f, 32f);
		_ui.AddChild(_currencyIcon);
		_ingameCoinsIcon = new MilMo_Widget(_ui);
		_ingameCoinsIcon.SetTexture("Batch01/Textures/Shop/Coins");
		_ingameCoinsIcon.SetPosition(348f, 660f);
		_ingameCoinsIcon.SetAlignment(MilMo_GUI.Align.BottomCenter);
		_ingameCoinsIcon.SetScale(32f, 32f);
		_ui.AddChild(_ingameCoinsIcon);
		_ingameCoinsAccountBalance = new MilMo_Widget(_ui);
		_ingameCoinsAccountBalance.SetTexture("Batch01/Textures/Core/Invisible");
		_ingameCoinsAccountBalance.SetAlignment(MilMo_GUI.Align.CenterLeft);
		_ingameCoinsAccountBalance.SetTextAlignment(MilMo_GUI.Align.BottomLeft);
		_ingameCoinsAccountBalance.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		_ingameCoinsAccountBalance.SetTextDropShadowPos(2f, 3f);
		_ingameCoinsAccountBalance.TextOutlineColor = new Color(0f, 0f, 0f, 1f);
		_ingameCoinsAccountBalance.SetTextNoLocalization("0");
		_ingameCoinsAccountBalance.SetFont(MilMo_GUI.Font.GothamLarge);
		RefreshInGameAccountBalanceScale();
		_ui.AddChild(_ingameCoinsAccountBalance);
		_juneCoinSign = new MilMo_Widget(_ui);
		_juneCoinSign.SetTexture("Batch01/Textures/Shop/AccountBalance");
		_juneCoinSign.SetAlignment(MilMo_GUI.Align.CenterLeft);
		_juneCoinSign.SetScale(64f, 32f);
		_ui.AddChild(_juneCoinSign);
		_accountBalance = new MilMo_Widget(_ui);
		_accountBalance.SetTexture("Batch01/Textures/Core/Invisible");
		_accountBalance.SetAlignment(MilMo_GUI.Align.CenterLeft);
		_accountBalance.SetScale(64f, 32f);
		_accountBalance.SetTextAlignment(MilMo_GUI.Align.BottomLeft);
		_accountBalance.SetTextOffset(0f, -5f);
		_accountBalance.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		_accountBalance.SetTextDropShadowPos(2f, 3f);
		_accountBalance.TextOutlineColor = new Color(0f, 0f, 0f, 1f);
		_accountBalance.SetFontScale(0.9f);
		_accountBalance.SetTextNoLocalization("0");
		_accountBalance.SetFont(MilMo_GUI.Font.GothamLarge);
		RefreshAccountBalanceScale();
		_ui.AddChild(_accountBalance);
		_mChargeButton = new MilMo_Button(_ui);
		_mChargeButton.SetTexture(MilMo_Monetization.Instance.Currency.ChargeButtonTexture);
		_mChargeButton.SetPressedTexture(MilMo_Monetization.Instance.Currency.ChargeButtonTexturePressed);
		_mChargeButton.SetHoverTexture(MilMo_Monetization.Instance.Currency.ChargeButtonTextureMO);
		_mChargeButton.SetPosition(386f, 730f);
		_mChargeButton.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_mChargeButton.SetScale(MilMo_Monetization.Instance.Currency.ChargeButtonSize);
		_mChargeButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_mChargeButton.SetFont(MilMo_GUI.Font.GothamLarge);
		_mChargeButton.SetFontScale(0.8f);
		_mChargeButton.Function = Charge;
		_mChargeButton.PointerHoverFunction = GrowJuneCoinIcon;
		_mChargeButton.PointerLeaveFunction = ShrinkJuneCoinIcon;
		_mChargeButton.SetHoverTextColor(1f, 1f, 0f, 1f);
		_mChargeButton.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		_mChargeButton.SetTextOutline(1f, 1f);
		_mChargeButton.TextOutlineColor = new Color(0f, 0f, 0f, 0.5f);
		_mChargeButton.SetTextDropShadowPos(2f, 2f);
		_mChargeButton.TextDropShadowColor.a = 0.5f;
		_mChargeButton.SetHoverSound(softTickSound);
		_mChargeButton.FadeToDefaultColor = true;
		_mChargeButton.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		_mChargeButton.SetExtraScaleOnHover(-3f, 3f);
		_mChargeButton.SetScalePull(0.05f, 0.05f);
		_mChargeButton.SetScaleDrag(0.6f, 0.7f);
		_mChargeButton.SetFadeSpeed(0.05f);
		_mChargeButton.SetFadeInSpeed(0.05f);
		_mChargeButton.SetFadeOutSpeed(0.05f);
		_mChargeButton.SetTextOffset(-2f, -5f);
		_ui.AddChild(_mChargeButton);
		_captionBar3 = new MilMo_Widget(_ui);
		_captionBar3.SetTexture("Batch01/Textures/Shop/CaptionInventory");
		_captionBar3.SetPosition(450f, 613f);
		_captionBar3.SetAlignment(MilMo_GUI.Align.TopLeft);
		_captionBar3.SetScale(574f, 41f);
		_captionBar3.SetText(MilMo_LocString.Empty);
		_captionBar3.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_captionBar3.SetTextOffset(10f, 6f);
		_captionBar3.SetTextDropShadowPos(2f, 2f);
		_captionBar3.SetFont(MilMo_GUI.Font.GothamLarge);
		_ui.AddChild(_captionBar3);
		_inventoryBack = new MilMo_Widget(_ui);
		_inventoryBack.SetTexture("Batch01/Textures/Shop/Background");
		_inventoryBack.SetPosition(450f, 654f);
		_inventoryBack.SetAlignment(MilMo_GUI.Align.TopLeft);
		_inventoryBack.SetScale(450f, 128f);
		_inventoryBack.SetCropMode(MilMo_GUI.CropMode.Crop);
		_ui.AddChild(_inventoryBack);
		_bottomScroller = new MilMo_ScrollView(_ui);
		_bottomScroller.SetText(MilMo_LocString.Empty);
		_bottomScroller.SetPosition(450f, 654f);
		_bottomScroller.SetScale(446f, 115f);
		_bottomScroller.SetAlignment(MilMo_GUI.Align.TopLeft);
		_bottomScroller.SetTexture("Batch01/Textures/Core/Invisible");
		_bottomScroller.ColorNow(1f, 1f, 1f, 1f);
		_bottomScroller.SetViewSize(1000f, 85f);
		_bottomScroller.HasBackground(b: false);
		_bottomScroller.IsUserControlled(b: true);
		_bottomScroller.ScrollToNow(0f, 100f);
		_bottomScroller.ScrollTo(0f, 0f);
		_bottomScroller.ShowVerticalBar(v: false);
		_ui.AddChild(_bottomScroller);
		_hotItemsButton = new MilMo_Button(_ui);
		_hotItemsButton.SetTexture("Batch01/Textures/Shop/BottomScrollerTab");
		_hotItemsButton.SetHoverTexture("Batch01/Textures/Shop/BottomScrollerTabMO");
		_hotItemsButton.SetPressedTexture("Batch01/Textures/Shop/BottomScrollerTab");
		_hotItemsButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_hotItemsButton.SetScale(194f, 42f);
		_hotItemsButton.SetPosition(581f, 655f);
		_hotItemsButton.SetPosPull(0.08f, 0.08f);
		_hotItemsButton.SetPosDrag(0.6f, 0.6f);
		_hotItemsButton.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		_hotItemsButton.SetExtraScaleOnHover(0f, 5f);
		_hotItemsButton.SetScalePull(0.1f, 0.1f);
		_hotItemsButton.SetScaleDrag(0.5f, 0.5f);
		_hotItemsButton.ScaleMover.MinVel.y = 0.0095f;
		_hotItemsButton.SetHoverSound(tickSound);
		_hotItemsButton.SetFont(MilMo_GUI.Font.GothamLarge);
		_hotItemsButton.SetText(MilMo_Localization.GetLocString("CharacterShop_246"));
		_hotItemsButton.SetDefaultTextColor(1f, 1f, 1f, 0.5f);
		_hotItemsButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_hotItemsButton.SetTextOffset(-8f, 0f);
		_hotItemsButton.Args = 1;
		_hotItemsButton.Function = ClkSelectBottomCategory;
		_hotItemsButton.PosMover.MinVel.x = 0.1f;
		_ui.AddChild(_hotItemsButton);
		_colorsButton = new MilMo_Button(_ui);
		_colorsButton.SetTexture("Batch01/Textures/Shop/BottomScrollerTab");
		_colorsButton.SetHoverTexture("Batch01/Textures/Shop/BottomScrollerTabMO");
		_colorsButton.SetPressedTexture("Batch01/Textures/Shop/BottomScrollerTab");
		_colorsButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_colorsButton.SetScale(144f, 42f);
		_colorsButton.SetPosition(450f, 655f);
		_colorsButton.SetPosPull(0.08f, 0.08f);
		_colorsButton.SetPosDrag(0.6f, 0.6f);
		_colorsButton.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		_colorsButton.SetExtraScaleOnHover(0f, 5f);
		_colorsButton.SetScalePull(0.1f, 0.1f);
		_colorsButton.SetScaleDrag(0.5f, 0.5f);
		_colorsButton.ScaleMover.MinVel.y = 0.0095f;
		_colorsButton.SetHoverSound(tickSound);
		_colorsButton.SetFont(MilMo_GUI.Font.GothamLarge);
		_colorsButton.SetText(MilMo_Localization.GetLocString("CharacterShop_247"));
		_colorsButton.SetDefaultTextColor(1f, 1f, 1f, 0.5f);
		_colorsButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_colorsButton.SetTextOffset(-8f, 0f);
		_colorsButton.Args = 0;
		_colorsButton.Function = ClkSelectBottomCategory;
		_colorsButton.PosMover.MinVel.x = 0.1f;
		_ui.AddChild(_colorsButton);
		_hotItemsFrame = new MilMo_SoftFrame(_ui);
		_hotItemsFrame.SetPosition(50f, 53f);
		_hotItemsFrame.SetAlignment(MilMo_GUI.Align.CenterLeft);
		_hotItemsFrame.SetScale(350f, 64f);
		_hotItemsFrame.SetScalePull(0.05f, 0.05f);
		_hotItemsFrame.SetScaleDrag(0.6f, 0.6f);
		_hotItemsFrame.SetTexture("Batch01/Textures/Shop/SoftFrameMiddle");
		_hotItemsFrame.SetFrameWidth(16f);
		_hotItemsFrame.SetLeftTexture("Batch01/Textures/Shop/SoftFrameLeft");
		_hotItemsFrame.SetRightTexture("Batch01/Textures/Shop/SoftFrameRight");
		_bottomScroller.AddChild(_hotItemsFrame);
		_selectMenu = new MilMo_Widget(_ui);
		_selectMenu.SetPosition(800f, 200f);
		_selectMenu.SetPosPull(0.07f, 0.07f);
		_selectMenu.SetPosDrag(0.8f, 0.8f);
		_selectMenu.SetScalePull(0.07f, 0.07f);
		_selectMenu.SetScaleDrag(0.6f, 0.6f);
		_selectMenu.ScaleMover.MinVel.x = 0.1f;
		_selectMenu.SetScale(144f, 68f);
		_selectMenu.SetAlignment(MilMo_GUI.Align.CenterRight);
		_selectMenu.SetTexture("Batch01/Textures/Shop/SelectedItemMenu");
		_selectMenu.FadeToDefaultColor = false;
		_selectMenu.AllowPointerFocus = false;
		_selectMenu.SetEnabled(e: true);
		_selectMenu.SetDefaultColor(0f, 0f, 0f, 1f);
		if (!MilMo_Config.Instance.IsTrue("Launcher.OfflineShop", defaultValue: false))
		{
			_selectMenu.SetPosition(0f, -100f);
		}
		_ui.AddChild(_selectMenu);
		_selectedBuyButton = new MilMo_Button(_ui);
		_selectedBuyButton.SetPosition(7f, 8f);
		_selectedBuyButton.SetScale(80f, 25f);
		_selectedBuyButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		_selectedBuyButton.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		_selectedBuyButton.SetTexture("Batch01/Textures/Core/Invisible");
		_selectedBuyButton.SetText(MilMo_Localization.GetLocString("CharacterShop_248"));
		_selectedBuyButton.SetFont(MilMo_GUI.Font.GothamLarge);
		_selectedBuyButton.SetTextOffset(0f, 0f);
		_selectedBuyButton.SetTextAngle(-8f);
		_selectedBuyButton.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Impulse);
		_selectedBuyButton.SetDefaultTextColor(1f, 1f, 1f, 0.75f);
		_selectedBuyButton.SetHoverTextColor(1f, 1f, 1f, 1f);
		_selectedBuyButton.SetTextOutline(1f, 1f);
		_selectedBuyButton.TextOutlineColor = new Color(0f, 0f, 0f, 0.5f);
		_selectedBuyButton.SetHoverImpulseColor(1f, 1f, 1f, 1f);
		_selectedBuyButton.SetFadeInSpeed(0.009f);
		_selectedBuyButton.SetFadeOutSpeed(0.6f);
		_selectedBuyButton.SetHoverSound(tickSound);
		_selectedBuyButton.UseParentAlpha = true;
		_selectedBuyButton.Function = BuySelected;
		_selectMenu.AddChild(_selectedBuyButton);
		_selectedGiftButton = new MilMo_Button(_ui);
		_selectedGiftButton.SetPosition(12f, 26f);
		_selectedGiftButton.SetScale(80f, 20f);
		_selectedGiftButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		_selectedGiftButton.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		_selectedGiftButton.SetTexture("Batch01/Textures/Core/Invisible");
		_selectedGiftButton.SetText(MilMo_Localization.GetLocString("CharacterShop_249"));
		_selectedGiftButton.SetFont(MilMo_GUI.Font.GothamMedium);
		_selectedGiftButton.SetTextOffset(0f, -3f);
		_selectedGiftButton.SetTextAngle(-8f);
		_selectedGiftButton.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Impulse);
		_selectedGiftButton.SetDefaultTextColor(1f, 1f, 1f, 0.4f);
		_selectedGiftButton.SetHoverTextColor(1f, 1f, 1f, 0.8f);
		_selectedGiftButton.SetTextOutline(1f, 1f);
		_selectedGiftButton.TextOutlineColor = new Color(0f, 0f, 0f, 0.5f);
		_selectedGiftButton.SetHoverImpulseColor(1f, 1f, 1f, 1f);
		_selectedGiftButton.SetFadeInSpeed(0.009f);
		_selectedGiftButton.SetFadeOutSpeed(0.6f);
		_selectedGiftButton.SetHoverSound(tickSound);
		_selectedGiftButton.UseParentAlpha = true;
		_selectedGiftButton.Function = BuySelectedAsGift;
		_selectMenu.AddChild(_selectedGiftButton);
		_selectedRemoveButton = new MilMo_Button(_ui);
		_selectedRemoveButton.SetPosition(19f, 46f);
		_selectedRemoveButton.SetScale(80f, 20f);
		_selectedRemoveButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		_selectedRemoveButton.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		_selectedRemoveButton.SetTexture("Batch01/Textures/Core/Invisible");
		_selectedRemoveButton.SetText(MilMo_Localization.GetLocString("CharacterShop_251"));
		_selectedRemoveButton.SetFont(MilMo_GUI.Font.GothamMedium);
		_selectedRemoveButton.SetTextOffset(0f, -3f);
		_selectedRemoveButton.SetTextAngle(-8f);
		_selectedRemoveButton.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Impulse);
		_selectedRemoveButton.SetDefaultTextColor(1f, 1f, 1f, 0.3f);
		_selectedRemoveButton.SetHoverTextColor(1f, 1f, 1f, 0.8f);
		_selectedRemoveButton.SetTextOutline(1f, 1f);
		_selectedRemoveButton.TextOutlineColor = new Color(0f, 0f, 0f, 0.5f);
		_selectedRemoveButton.SetHoverImpulseColor(1f, 1f, 1f, 1f);
		_selectedRemoveButton.SetFadeInSpeed(0.009f);
		_selectedRemoveButton.SetFadeOutSpeed(0.6f);
		_selectedRemoveButton.SetHoverSound(tickSound);
		_selectedRemoveButton.Function = RemoveSelected;
		_selectedRemoveButton.UseParentAlpha = true;
		_selectMenu.AddChild(_selectedRemoveButton);
		_inventoryShade = new MilMo_Widget(_ui);
		_inventoryShade.SetTexture("Batch01/Textures/Shop/PanelShade");
		_inventoryShade.SetPosition(450f, 654f);
		_inventoryShade.SetAlignment(MilMo_GUI.Align.TopLeft);
		_inventoryShade.SetScale(450f, 117f);
		_inventoryShade.AllowPointerFocus = false;
		_ui.AddChild(_inventoryShade);
		_invLeftShade = new MilMo_Widget(_ui);
		_invLeftShade.SetTexture("Batch01/Textures/Shop/FadeLeft");
		_invLeftShade.SetDefaultColor(0f, 0f, 0f, 1f);
		_invLeftShade.SetPosition(450f, 654f);
		_invLeftShade.SetAlignment(MilMo_GUI.Align.TopLeft);
		_invLeftShade.SetScale(64f, 128f);
		_invLeftShade.AllowPointerFocus = false;
		_ui.AddChild(_invLeftShade);
		_invRightShade = new MilMo_Widget(_ui);
		_invRightShade.SetTexture("Batch01/Textures/Shop/FadeRight");
		_invRightShade.SetDefaultColor(0f, 0f, 0f, 1f);
		_invRightShade.SetPosition(832f, 654f);
		_invRightShade.SetAlignment(MilMo_GUI.Align.TopLeft);
		_invRightShade.SetScale(64f, 128f);
		_invRightShade.AllowPointerFocus = false;
		_ui.AddChild(_invRightShade);
		_backToGameBack = new MilMo_Widget(_ui);
		_backToGameBack.SetTexture("Batch01/Textures/Shop/BlueSquare");
		_backToGameBack.SetPosition(896f, 654f);
		_backToGameBack.SetAlignment(MilMo_GUI.Align.TopLeft);
		_backToGameBack.SetScale(128f, 128f);
		_ui.AddChild(_backToGameBack);
		_backButton = new MilMo_Button(_ui);
		_backButton.SetTexture("Batch01/Textures/Shop/BackButton");
		_backButton.SetPressedTexture("Batch01/Textures/Shop/BackButton");
		_backButton.SetHoverTexture("Batch01/Textures/Shop/BackButtonMO");
		_backButton.SetPosition(960f, 730f);
		_backButton.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_backButton.SetScale(128f, 64f);
		_backButton.SetText(MilMo_Localization.GetLocString("CharacterShop_252"));
		_backButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_backButton.SetHoverTextColor(1f, 1f, 1f, 1f);
		_backButton.SetHoverColor(1f, 1f, 0f, 1f);
		_backButton.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		_backButton.SetDefaultTextColor(1f, 1f, 1f, 1f);
		_backButton.SetHoverTextColor(1f, 1f, 0f, 1f);
		_backButton.FadeToDefaultTextColor = false;
		_backButton.SetFont(MilMo_GUI.Font.GothamLarge);
		_backButton.SetFontScale(0.8f);
		_backButton.PointerHoverFunction = ShowBackArrow;
		_backButton.PointerLeaveFunction = HideBackArrow;
		_backButton.Function = GoBackToGame;
		_backButton.SetTextOutline(1f, 1f);
		_backButton.TextOutlineColor = new Color(0f, 0f, 0f, 0.5f);
		_backButton.SetTextDropShadowPos(2f, 2f);
		_backButton.TextDropShadowColor.a = 0.5f;
		_backButton.SetHoverSound(softTickSound);
		_backButton.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		_backButton.SetExtraScaleOnHover(-3f, 3f);
		_backButton.SetScalePull(0.05f, 0.05f);
		_backButton.SetScaleDrag(0.6f, 0.7f);
		_backButton.SetFadeSpeed(0.05f);
		_backButton.SetFadeInSpeed(0.05f);
		_backButton.SetFadeOutSpeed(0.05f);
		_backButton.SetTextOffset(0f, -5f);
		_ui.AddChild(_backButton);
		_returnToLevelName = new MilMo_Button(_ui);
		_returnToLevelName.SetAllTextures("Batch01/Textures/Core/Invisible");
		_returnToLevelName.SetPosition(960f, 680f);
		_returnToLevelName.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_returnToLevelName.SetScale(110f, 55f);
		_returnToLevelName.SetText(_levelName);
		_returnToLevelName.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_returnToLevelName.SetTextOffset(0f, 0f);
		_returnToLevelName.SetTextDropShadowPos(2f, 2f);
		_returnToLevelName.SetFontScale(0.8f);
		_returnToLevelName.SetDefaultTextColor(1f, 1f, 1f, 0.5f);
		_returnToLevelName.SetFadeOutSpeed(0.05f);
		_returnToLevelName.FadeToDefaultTextColor = false;
		_returnToLevelName.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		_returnToLevelName.SetFont(MilMo_GUI.Font.GothamLarge);
		_returnToLevelName.AllowPointerFocus = false;
		_ui.AddChild(_returnToLevelName);
		_backArrow = new MilMo_Widget(_ui);
		_backArrow.SetTexture("Batch01/Textures/Shop/FatArrow");
		_backArrow.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_backArrow.SetPosition(960f, 660f);
		_backArrow.ColorNow(1f, 1f, 0f, 0f);
		_backArrow.FadeToDefaultColor = false;
		_backArrow.SetScale(32f, 32f);
		_backArrow.SetFadeSpeed(0.1f);
		_backArrow.SetPosPull(0.03f, 0.03f);
		_backArrow.SetPosDrag(0.85f, 0.85f);
		_backArrow.AllowPointerFocus = false;
		_ui.AddChild(_backArrow);
		_returnToLevelIcon = new MilMo_Widget(_ui);
		_returnToLevelIcon.SetTexture(new MilMo_Texture(_levelIcon));
		_returnToLevelIcon.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_returnToLevelIcon.SetPosition(860f, 660f);
		_returnToLevelIcon.ColorNow(1f, 1f, 1f, 0f);
		_returnToLevelIcon.FadeToDefaultColor = false;
		_returnToLevelIcon.SetScale(0f, 0f);
		_returnToLevelIcon.SetFadeSpeed(0.1f);
		_returnToLevelIcon.SetPosPull(0.03f, 0.03f);
		_returnToLevelIcon.SetPosDrag(0.85f, 0.85f);
		_returnToLevelIcon.SetScalePull(0.03f, 0.03f);
		_returnToLevelIcon.SetScaleDrag(0.85f, 0.85f);
		_returnToLevelIcon.SetFadeSpeed(0.08f);
		_returnToLevelIcon.AllowPointerFocus = false;
		_ui.AddChild(_returnToLevelIcon);
		_middleIcon.GoToNow(225f, 0f);
		infoBox = new MilMo_ItemInfoBox(_ui);
		infoBox.SetEnabled(e: false);
		_ui.AddChild(infoBox);
		_allowBrowsingItemCardsCallback = AllowBrowsingItemCards;
		_allowSelectMenuInput = OnAllowSelectMenuInput;
		_pulseCursor = new MilMo_PulseCursor(_ui);
		_ui.AddChild(_pulseCursor);
		_pulseCursor.SetTexture("Batch01/Textures/Shop/ShopCursor");
		_pulseCursor.SetScalePull(0.02f, 0.02f);
		_pulseCursor.SetScaleDrag(0.92f, 0.92f);
		_pulseCursor.PulseDownDelay = 0f;
		_pulseCursor.PulseUpDelay = 0f;
		_pulseCursor.ScaleMover.MinVel.x = 0.0001f;
		_pulseCursor.ScaleMover.MinVel.y = 0.0001f;
		_pulseCursor.MinSize = new Vector2(44f, 44f);
		_pulseCursor.MaxSize = new Vector2(64f, 64f);
		_pulseCursor.MinColor = new Color(0f, 0f, 0f, 0.3f);
		_pulseCursor.MaxColor = new Color(0f, 0f, 0f, 1f);
		_pulseCursor.SetAlpha(0.4f);
		_pulseCursor.SetFadeSpeed(0.02f);
		_pulseCursor.AngleMover.Vel.x = 0.25f;
		_pulseCursor.SetEnabled(e: false);
		_pulseCursor.SetPosition(0f, -100f);
		_pulseCursor.Start();
	}

	private void UpdateRes()
	{
		screenWidth = ((Screen.width <= 1024 || Screen.height <= 720) ? 1024 : Screen.width);
		screenHeight = ((Screen.width <= 1024 || Screen.height <= 720) ? 720 : Screen.height);
		if (screenWidth <= 1024 || screenHeight <= 720)
		{
			screenWidth = 1024;
			screenHeight = 720;
			_ui.OffsetMode = true;
			_ui.UpdateGlobalOffset();
			_ui.ResetLayout(10f, 10f);
		}
		else
		{
			_ui.OffsetMode = false;
			_ui.SetGlobalOffset(new Vector2(0f, 0f));
			_ui.ResetLayout(10f, 10f);
		}
	}

	private void RefreshUI()
	{
		UpdateRes();
		_leftFade.SetPosition(447f, 0f);
		_leftFade.SetScale(12f, screenHeight - 156);
		_rightFade.SetPosition(screenWidth, 0f);
		_rightFade.SetScale(-12f, screenHeight - 156);
		_topFade.SetPosition(447f, 0f);
		_topFade.SetScale(screenWidth - 450, -12f);
		_bottomFade.SetPosition(447f, screenHeight - 156);
		_bottomFade.SetScale(screenWidth - 450, 12f);
		_topBack.SetPosition(0f, 0f);
		_topBack.SetAlignment(MilMo_GUI.Align.TopLeft);
		_topBack.SetScale(450f, 82f);
		_charSpinner.SetPosition(450f, 0f);
		_charSpinner.SetScale(screenWidth - 450, screenHeight);
		_mainCatScroller.SetPosition(0f, 0f);
		_mainCatScroller.SetScale(450f, Screen.height + 50);
		float[] arg = new float[1] { CurCategory.Number };
		SelectMainCategory(arg);
		foreach (MilMo_CharacterShopCategory category in _categoryList)
		{
			category.Button.SetEnabled(e: true);
			category.Button.SetPosition((float)category.Number * 80f + 65f + 160f, 0f);
			category.Button.SetScaleAbsolute(_ui.ScaleToLowestUIRes(60f, 60f));
			category.ButtonReflect.SetPosition((float)category.Number * 80f + 65f + 160f, 120f);
			category.ButtonReflect.GoTo((float)category.Number * 80f + 65f + 160f, 120f);
			category.ButtonReflect.SetMinScale(_ui.ScaleToLowestUIRes(60f / _ui.Res.x, -60f / _ui.Res.y));
			category.ButtonReflect.SetScaleAbsolute(_ui.ScaleToLowestUIRes(60f, -60f));
			_mainCatIconHeight = 0f;
		}
		_mainCatScroller.ScrollToNow((float)CurCategory.Number * 80f + 65f + 160f - 225f, 0f);
		_mainCatIconHeight = -90f;
		DisableOffScreenIcons();
		_iconLeftArrow.SetPosition(0f, 20f);
		_iconLeftArrow.SetDefaultScale(_arrowSize);
		_iconLeftArrow.SetScale(_arrowSize.x, _arrowSize.y);
		_iconRightArrow.SetPosition(433f, 20f);
		_iconRightArrow.SetDefaultScale(_arrowSize);
		_iconRightArrow.SetScale(_arrowSize.x, _arrowSize.y);
		_coverFlowGradient.SetPosition(0f, 52f);
		_coverFlowGradient.SetAlignment(MilMo_GUI.Align.TopLeft);
		_coverFlowGradient.SetScale(450f, 32f);
		mainCaptionBack.SetPosition(0f, 82f);
		mainCaptionBack.SetScale(450f, 64f);
		_mainCaption.SetXPos(32f);
		_mainCaption.SetYPos((CurCategory.TabList.Count > 1) ? 87f : 100f);
		_mainCaption.SetScale(250f, 32f);
		_mainCaption.SetFontScale(1f);
		captionShine.SetPosition(250f, 82f);
		captionShine.SetScale(250f, 8f);
		_middleIcon.SetPosition(225f, 0f);
		_middleIcon.SetScaleAbsolute(_ui.ScaleToLowestUIRes(100f, 100f));
		_middleIcon.SetMinScale(_ui.ScaleToLowestUIRes(60f / _ui.Res.x, 60f / _ui.Res.y));
		_subCatScroller.SetPosition(0f, 114f);
		_subCatScroller.SetScale(435f, Screen.height + 50);
		_tabLeftArrow.SetPosition(8f, 110f);
		_tabLeftArrow.SetDefaultScale(_arrowSize);
		_tabLeftArrow.SetScale(_arrowSize.x, _arrowSize.y);
		_tabRightArrow.SetPosition(425f, 110f);
		_tabRightArrow.SetDefaultScale(_arrowSize);
		_tabRightArrow.SetScale(_arrowSize.x, _arrowSize.y);
		_back1.SetPosition(0f, 154.76276f);
		_back1.SetScale(450f, 128f);
		_back2.SetPosition(0f, 282.76276f);
		_back2.SetScale(450f, 128f);
		_back3.SetPosition(0f, 410.76276f);
		_back3.SetScale(450f, 128f);
		_back4.SetPosition(0f, 538.76276f);
		_back4.SetScale(450f, 128f);
		_back5.SetPosition(0f, 666.76276f);
		_back5.SetScale(450f, 128f);
		_back6.SetPosition(0f, 794.76276f);
		_back6.SetScale(450f, 128f);
		_back7.SetPosition(0f, 922.76276f);
		_back7.SetScale(450f, 128f);
		foreach (MilMo_Button tab in CurCategory.TabList)
		{
			tab.SetEnabled(e: true);
			tab.SetPosition((float)tab.Info * 96f + 32f, 32f);
			tab.SetScale(96f, 32f);
		}
		CurCategory.CurrentSubCategory = 0;
		RefreshSubCategories(0);
		topDiv.SetPosition(0f, 146f);
		topDiv.SetScale(450f, 9.3748f);
		_fadeTop.SetPosition(0f, 153.5909f);
		_fadeTop.SetScale(450f, 37.4992f);
		_fadeBottom.SetPosition(0f, screenHeight - 229);
		_fadeBottom.SetScale(450f, 75f);
		_purpleBack.SetPosition(0f, screenHeight - 114);
		_purpleBack.SetScale(323f, 114f);
		_purpleBackRight.SetPosition(0f, screenHeight - 113);
		_purpleBackRight.SetScale(8f, 112f);
		descPriceTag.SetPosition(255f, screenHeight - 155);
		descPriceTag.SetScale(128f, 50f);
		helpCaption.SetPosition(0f, screenHeight - 155);
		helpCaption.SetFontScale(1f);
		helpCaption.SetScale(323f, 41f);
		helpText.SetPosition(0f, screenHeight - 114);
		helpText.SetScale(300f, 112f);
		helpText.SetFontScale(1f);
		_helpReflect.SetPosition(8f, screenHeight - 105);
		_helpReflect.SetScale(220f, 128f);
		_captionCharge.SetPosition(322f, screenHeight - 155);
		_captionCharge.SetScale(128f, 41f);
		_captionCharge.SetFontScale(0.8f);
		_chargeBack.SetPosition(322f, screenHeight - 114);
		_chargeBack.SetScale(128f, 128f);
		_ingameCoinsIcon.SetPosition(348f, screenHeight - 118);
		_ingameCoinsIcon.SetScale(32f, 32f);
		_ingameCoinsAccountBalance.SetPosition(380f, screenHeight - 140);
		_ingameCoinsAccountBalance.SetFontScale(0.9f);
		_ingameCoinsAccountBalance.SetScale(64f, 32f);
		RefreshInGameAccountBalanceScale();
		_currencyIcon.SetPosition(348f, screenHeight - 72);
		_currencyIcon.SetScale(32f, 32f);
		_juneCoinSign.SetPosition(333f, screenHeight - 88);
		_juneCoinSign.SetScale(64f, 32f);
		_accountBalance.SetPosition(383f, screenHeight - 88);
		_accountBalance.SetScale(64f, 32f);
		RefreshAccountBalanceScale();
		_mChargeButton.SetPosition(386f, MilMo_Monetization.Instance.Currency.ShowAccountBalance ? (screenHeight - 38) : (screenHeight - 52));
		_mChargeButton.SetScale(MilMo_Monetization.Instance.Currency.ChargeButtonSize);
		_mChargeButton.SetFontScale(0.8f);
		_captionBar3.SetPosition(450f, screenHeight - 156);
		_captionBar3.SetScale(screenWidth - 450, 41f);
		_inventoryBack.SetPosition(450f, screenHeight - 115);
		_inventoryBack.SetScale(screenWidth - 450 - 128, 128f);
		_bottomScroller.SetPosition(450f, screenHeight - 116);
		_bottomScroller.SetScale(screenWidth - 450 - 128, 115f);
		_hotItemsButton.SetYPos(screenHeight - 113);
		_hotItemsButton.SetScale(194f, 42f);
		_hotItemsButton.SetFontScale(1f);
		_colorsButton.SetYPos(screenHeight - 113);
		_colorsButton.SetScale(144f, 42f);
		_colorsButton.SetFontScale(1f);
		_hotItemsFrame.SetPosition(50f, 53f);
		SelectBottomCategory(_curBottomCat);
		_colorsButton.SetXPos(_colorsButton.PosMover.Target.x / _ui.Res.x);
		_hotItemsButton.SetXPos(_hotItemsButton.PosMover.Target.x / _ui.Res.x);
		_inventoryShade.SetPosition(450f, screenHeight - 115);
		_inventoryShade.SetScale(screenWidth - 450 - 128, 117f);
		_invLeftShade.SetPosition(450f, screenHeight - 115);
		_invLeftShade.SetScale(64f, 128f);
		_invRightShade.SetPosition(screenWidth - 128, screenHeight - 115);
		_invRightShade.SetScale(64f, 128f);
		_backToGameBack.SetPosition(screenWidth - 128, screenHeight - 115);
		_backToGameBack.SetScale(128f, 128f);
		_backButton.SetPosition(screenWidth - 64, screenHeight - 38);
		_backButton.SetScale(128f, 64f);
		_backButton.SetFontScale(0.8f);
		_returnToLevelName.SetPosition(screenWidth - 64, screenHeight - 88);
		_returnToLevelName.SetScale(110f, 55f);
		_returnToLevelName.SetFontScale(0.8f);
		_backArrow.SetPosition(screenWidth - 64, screenHeight - 108);
		_backArrow.SetScale(32f, 32f);
		_returnToLevelIcon.SetPosition(screenWidth - 64, screenHeight - 108);
		_returnToLevelIcon.SetScale(0f, 0f);
		_middleIcon.GoToNow(225f, 0f);
		_pulseCursor.SetPosition(0f, -100f);
		if (MilMo_Config.Instance.IsTrue("Launcher.OfflineShop", defaultValue: false))
		{
			_selectMenu.SetPosition(800f, 200f);
		}
		else
		{
			_selectMenu.SetPosition(0f, -100f);
		}
		_selectMenu.SetScale(144f, 68f);
		RefreshSelection();
		_selectedBuyButton.SetPosition(7f, 8f);
		_selectedBuyButton.SetScale(80f, 25f);
		_selectedBuyButton.SetFontScale(1f);
		if (MilMo_Localization.CurrentLanguage.IsPortugueseBrazilian)
		{
			_selectedBuyButton.SetFontScale(0.72f);
		}
		float num = 2f;
		float num2 = 6f;
		_selectedGiftButton.SetPosition(10f + num, 23f + num2);
		_selectedGiftButton.SetScale(80f, 20f);
		_selectedGiftButton.SetFontScale(1f);
		if (MilMo_Localization.CurrentLanguage.IsPortugueseBrazilian)
		{
			_selectedGiftButton.SetFontScale(0.85f);
		}
		_selectedRemoveButton.SetPosition(15f + num, 36f + num2);
		_selectedRemoveButton.SetScale(80f, 20f);
		_selectedRemoveButton.SetFontScale(1f);
		if (MilMo_Localization.CurrentLanguage.IsPortugueseBrazilian)
		{
			_selectedRemoveButton.SetFontScale(0.85f);
		}
		if (_loadingPane != null)
		{
			_loadingPane.SetPosition(1024f * _ui.Res.x - 67f, 173f);
		}
		_ui.ScreenSizeDirty = false;
	}

	private void RemoveSelected(object o)
	{
		if (_selectedItem == null)
		{
			return;
		}
		if (_selectedItem.Item is MilMo_Wearable milMo_Wearable)
		{
			for (int num = MilMo_Player.Instance.Avatar.BodyPackManager.Equipped.Count - 1; num >= 0; num--)
			{
				if (!(milMo_Wearable.BodyPack.Identifier != MilMo_Player.Instance.Avatar.BodyPackManager.Equipped[num].Identifier))
				{
					MilMo_Player.Instance.Avatar.BodyPackManager.Unequip(MilMo_Player.Instance.Avatar.BodyPackManager.Equipped[num]);
					MilMo_Player.Instance.Avatar.AsyncApply(OnRefreshApplied);
					InstantHideSelectMenu();
				}
			}
			_selectedItem = null;
			return;
		}
		MilMo_Item item = ((_selectedItem.Item is MilMo_Furniture || _selectedItem.Item is MilMo_HomeSurface || _selectedItem.Item is MilMo_ShopRoom) ? _selectedItem.Item : null);
		if (item == null)
		{
			return;
		}
		using (IEnumerator<MilMo_Button> enumerator = (from button in tryButtonList
			let shopItem = button.Args as MilMo_ShopItem
			where shopItem != null && item == shopItem.Item
			select button).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				MilMo_Button current = enumerator.Current;
				tryButtonList.Remove(current);
				_ui.RemoveChild(current);
			}
		}
		InstantHideSelectMenu();
		_selectedItem = null;
		OnRefreshApplied();
	}

	public static void BuySelectedItem()
	{
		if (_theCharacterShop == null)
		{
			throw new InvalidOperationException("Trying to buy selected item in shop when character shop is null");
		}
		if (!_theCharacterShop.enabled)
		{
			throw new InvalidOperationException("Trying to buy selected item in shop when character shop is not active");
		}
		_theCharacterShop.BuySelected(null);
	}

	private void BuySelected(object o)
	{
		if (_selectedItem != null)
		{
			Buy(_selectedItem);
		}
	}

	private void BuySelectedAsGift(object o)
	{
		if (_selectedItem != null)
		{
			BuyAsGift(_selectedItem);
		}
	}

	private static void Charge(object arg)
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
		if (MilMo_Monetization.Instance.Currency.ChargeButtonCallback != null)
		{
			MilMo_Monetization.Instance.Currency.ChargeButtonCallback();
		}
	}

	private static void TryLocal(object o)
	{
	}

	private void Try(object itemAsObject)
	{
		Try(itemAsObject, mayOpenMembershipScreen: true);
	}

	private void Try(object itemAsObject, bool mayOpenMembershipScreen)
	{
		for (int num = tryButtonList.Count - 1; num >= 0; num--)
		{
			if (!(tryButtonList[num].Args is MilMo_ShopItem milMo_ShopItem))
			{
				Debug.LogWarning("Button contains a shop item that is null. - CharacterShop.");
			}
			else if (milMo_ShopItem.Item is MilMo_HomeEquipment)
			{
				_ui.RemoveChild(tryButtonList[num]);
				tryButtonList.Remove(tryButtonList[num]);
			}
		}
		if (!(itemAsObject is MilMo_ShopItem milMo_ShopItem2))
		{
			return;
		}
		if (_currentlyHideFromSelection.Contains(milMo_ShopItem2.Item))
		{
			_currentlyHideFromSelection.Remove(milMo_ShopItem2.Item);
		}
		if (SpawnTryButton(milMo_ShopItem2))
		{
			Analytics.StoreClick(milMo_ShopItem2.Item.Identifier);
			bool isNewFurniture = milMo_ShopItem2.Item is MilMo_Furniture && _selectedItem != null && (_selectedItem.Item is MilMo_ShopRoom || _selectedItem.Item is MilMo_Furniture) && milMo_ShopItem2 != _selectedItem;
			bool isNewRoom = milMo_ShopItem2.Item is MilMo_ShopRoom && _selectedItem != null && (_selectedItem.Item is MilMo_ShopRoom || _selectedItem.Item is MilMo_Furniture) && milMo_ShopItem2 != _selectedItem;
			_selectedItem = milMo_ShopItem2;
			if (milMo_ShopItem2.Item is MilMo_Wearable)
			{
				TryWearable((MilMo_Wearable)milMo_ShopItem2.Item);
			}
			else if (milMo_ShopItem2.Item is MilMo_Furniture)
			{
				TryFurniture((MilMo_Furniture)milMo_ShopItem2.Item, isNewFurniture);
			}
			else if (milMo_ShopItem2.Item is MilMo_Wallpaper)
			{
				TryWallpaper((MilMo_Wallpaper)milMo_ShopItem2.Item);
			}
			else if (milMo_ShopItem2.Item is MilMo_Floor)
			{
				TryFloor((MilMo_Floor)milMo_ShopItem2.Item);
			}
			else if (milMo_ShopItem2.Item is MilMo_MemberSubscription && mayOpenMembershipScreen)
			{
				TrySubscription();
			}
			else if (milMo_ShopItem2.Item is MilMo_ShopRoom)
			{
				TryRoom((MilMo_ShopRoom)milMo_ShopItem2.Item, isNewRoom);
			}
			RefreshTryButtons();
			RefreshWallObject();
		}
	}

	private void TryWearable(MilMo_Wearable wearable)
	{
		MilMo_Avatar avatar = MilMo_Player.Instance.Avatar;
		avatar.EquipLocal(wearable);
		avatar.AsyncApply(OnRefreshApplied);
		switch (CurCategory.IdentifierName)
		{
		case "Upper_Body":
			avatar.AsyncApply(PlayFirstEmote, "SHIRT");
			break;
		case "Lower_Body":
			avatar.AsyncApply(PlayFirstEmote, "PANTS");
			break;
		case "Shoes":
			avatar.AsyncApply(PlayFirstEmote, "SHOES");
			break;
		case "Accessories":
			switch (MilMo_Utility.RandomInt(1, 3))
			{
			case 1:
				avatar.AsyncApply(PlayFirstEmote, "EYES");
				break;
			case 2:
				avatar.AsyncApply(PlayFirstEmote, "MOUTH");
				break;
			case 3:
				avatar.AsyncApply(PlayFirstEmote, "HAIR");
				break;
			}
			break;
		}
	}

	private void TryRoom(MilMo_ShopRoom room, bool isNewRoom)
	{
		if (room.GameObject != null)
		{
			if (isNewRoom)
			{
				SwitchFurniture();
			}
			else
			{
				UpdateViewMode();
			}
			OnRefreshApplied();
			return;
		}
		MilMo_ShopItem item = _selectedItem;
		room.AsyncLoadContent(delegate(GameObject obj)
		{
			if (obj == null)
			{
				Debug.LogWarning("Failed to load room " + room.Template.Path + " for shop preview");
			}
			else
			{
				obj.transform.position = new Vector3(-3f, -50f, 0.7f) + MilMo_ShopCameraController.ShopPosition;
				if (isNewRoom)
				{
					SwitchFurniture();
				}
				else
				{
					UpdateViewMode();
				}
				if (item.Mover == null)
				{
					item.Mover = new MilMo_ObjectMover();
				}
				item.Mover.AttachObject(obj);
				OnRefreshApplied();
				RefreshWallObject();
			}
		});
	}

	private void TryFurniture(MilMo_Furniture furniture, bool isNewFurniture)
	{
		if (furniture.GameObject != null)
		{
			if (isNewFurniture)
			{
				SwitchFurniture();
			}
			else
			{
				UpdateViewMode();
			}
			OnRefreshApplied();
			return;
		}
		MilMo_ShopItem item = _selectedItem;
		furniture.AsyncLoadContent(delegate(GameObject furnitureObject)
		{
			if (furnitureObject == null)
			{
				Debug.LogWarning("Failed to load furniture " + furniture.Template.Path + " for shop preview");
			}
			else
			{
				furnitureObject.transform.position = new Vector3(-3f, -50f, 1f) + MilMo_ShopCameraController.ShopPosition;
				furniture.AsyncApply();
				if (isNewFurniture)
				{
					SwitchFurniture();
				}
				else
				{
					UpdateViewMode();
				}
				float x = 0f;
				float z = 0f;
				float y = 0f;
				if (furniture is MilMo_FloorFurniture milMo_FloorFurniture)
				{
					MilMo_FurnitureFloorGrid gridForRotation = milMo_FloorFurniture.Template.GetGridForRotation(0f);
					x = gridForRotation.PivotCol - (float)gridForRotation.Columns * 0.5f;
					z = 0f - (gridForRotation.PivotRow - (float)gridForRotation.Rows * 0.5f);
				}
				else if (furniture is MilMo_WallFurniture)
				{
					MilMo_FurnitureWallGrid grid = ((MilMo_WallFurniture)furniture).Template.Grid;
					z = grid.Pivot - (float)(int)grid.Width * 0.5f;
					y = 1.35f;
					furnitureObject.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
				}
				if (furniture.Template.IsDoor)
				{
					y = 2f;
				}
				furnitureObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
				for (int i = 0; i < furnitureObject.transform.childCount; i++)
				{
					furnitureObject.transform.GetChild(i).transform.localPosition = new Vector3(x, y, z);
				}
				if (item.Mover == null)
				{
					item.Mover = new MilMo_ObjectMover();
				}
				item.Mover.AttachObject(furnitureObject);
				OnRefreshApplied();
				RefreshWallObject();
			}
		});
	}

	private void TryWallpaper(MilMo_HomeSurface wallpaper)
	{
		wallpaper.AsyncLoadContent(delegate
		{
			UpdateViewMode();
			wallpaper.Apply(_theWallpaperObject.GameObject);
			Texture2D texture = wallpaper.GetTexture(_theWallpaperObject.GameObject);
			if (_theWallpaperObject.Renderer.material.mainTexture != texture)
			{
				_theWallpaperObject.Renderer.material.mainTexture = texture;
			}
			OnRefreshApplied();
		});
	}

	private void TryFloor(MilMo_HomeSurface floor)
	{
		floor.AsyncLoadContent(delegate
		{
			UpdateViewMode();
			floor.Apply(_theFloorObject.GameObject);
			OnRefreshApplied();
			Texture2D texture = floor.GetTexture(_theFloorObject.GameObject);
			if (_theFloorObject.Renderer.material.mainTexture != texture)
			{
				_theFloorObject.Renderer.material.mainTexture = texture;
			}
		});
	}

	private static void TrySubscription()
	{
		MilMo_GlobalUI.Instance.GetWindow("AboutMembership").Open();
	}

	private void Buy(object itemAsObject)
	{
		Buy(itemAsObject, null, null);
	}

	private void Buy(object itemAsObject, string avatarName, string giftFriendId)
	{
		if (_isBuying)
		{
			return;
		}
		_isBuying = true;
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
		MilMo_ShopItem itemToBuy = itemAsObject as MilMo_ShopItem;
		bool flag = avatarName != null;
		if (itemToBuy == null)
		{
			_isBuying = false;
			return;
		}
		if (flag)
		{
			_lastGiftFriendAvatarName = avatarName;
			_lastGiftFriendId = giftFriendId;
		}
		if (!flag && itemToBuy.Item.Template.IsUnique && MilMo_Player.Instance.Inventory.HaveItem(itemToBuy.Item))
		{
			MilMo_Dialog haveItemDialog = new MilMo_Dialog(_ui);
			_ui.AddChild(haveItemDialog);
			MilMo_LocString locString = MilMo_Localization.GetLocString("CharacterShop_260");
			locString.SetFormatArgs(itemToBuy.Item.Template.DisplayName);
			haveItemDialog.DoWarning(MilMo_Localization.GetLocString("CharacterShop_258"), locString, delegate
			{
				haveItemDialog.CloseAndRemove(null);
				_isBuying = false;
			});
		}
		else if (!flag && MilMo_Player.Instance.Inventory.HaveItem(itemToBuy.Item) && MilMo_Player.Instance.Inventory.GetEntry(itemToBuy.Item.Template.Identifier).Amount + itemToBuy.GetAmount() > 32767)
		{
			MilMo_Dialog maxItemAmountDialog = new MilMo_Dialog(_ui);
			_ui.AddChild(maxItemAmountDialog);
			MilMo_LocString locString2 = MilMo_Localization.GetLocString("CharacterShop_271");
			locString2.SetFormatArgs(itemToBuy.DisplayName);
			maxItemAmountDialog.DoWarning(MilMo_Localization.GetLocString("CharacterShop_258"), locString2, delegate
			{
				maxItemAmountDialog.CloseAndRemove(null);
				_isBuying = false;
			});
		}
		else if (!flag && itemToBuy.CanBuyWithCoins && itemToBuy.GetCoinPrice() <= GlobalStates.Instance.playerState.coins.Get())
		{
			MilMo_LocString locString3 = MilMo_Localization.GetLocString("CharacterShop_5581");
			locString3.SetFormatArgs(itemToBuy.DisplayName, MilMo_Monetization.Instance.Currency.Name, null);
			MilMo_Dialog buyDialog = new MilMo_Dialog(_ui);
			_ui.AddChild(buyDialog);
			buyDialog.DoCustomCustomCancel("Batch01/Textures/Core/Invisible", MilMo_Localization.GetLocString("CharacterShop_258"), locString3, MilMo_Localization.GetLocString("World_5965"), MilMo_Monetization.Instance.Currency.NameForButtons, delegate
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
				_remoteShop.Buy(itemToBuy, useCoins: true);
				buyDialog.CloseAndRemove(null);
			}, delegate
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
				_remoteShop.Buy(itemToBuy, useCoins: false);
				buyDialog.CloseAndRemove(null);
			}, delegate
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Cancel);
				buyDialog.CloseAndRemove(null);
				_isBuying = false;
			});
			buyDialog.Icon.SetTexture(new MilMo_Texture((Texture)null));
			itemToBuy.AsyncGetIcon(delegate(Texture2D icon)
			{
				buyDialog.Icon.SetTexture(new MilMo_Texture(icon));
			});
		}
		else if (!flag && MilMo_Monetization.Instance.Currency.ShowConfirmBuyDialog)
		{
			MilMo_Dialog acceptBuyDialog = new MilMo_Dialog(_ui);
			_ui.AddChild(acceptBuyDialog);
			MilMo_LocString locString4 = MilMo_Localization.GetLocString("CharacterShop_261");
			locString4.SetFormatArgs(itemToBuy.DisplayName);
			acceptBuyDialog.DoOKCancel("Batch01/Textures/Core/Invisible", MilMo_Localization.GetLocString("CharacterShop_258"), locString4, delegate
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
				_remoteShop.Buy(itemToBuy, useCoins: false);
				acceptBuyDialog.CloseAndRemove(null);
			}, delegate
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Cancel);
				acceptBuyDialog.CloseAndRemove(null);
				_isBuying = false;
			});
			acceptBuyDialog.Icon.SetTexture(new MilMo_Texture((Texture)null));
			itemToBuy.AsyncGetIcon(delegate(Texture2D icon)
			{
				acceptBuyDialog.Icon.SetTexture(new MilMo_Texture(icon));
			});
		}
		else if (flag)
		{
			_remoteShop.BuyAsGift(itemToBuy, useCoins: false, avatarName);
		}
		else
		{
			_remoteShop.Buy(itemToBuy, useCoins: false);
		}
	}

	private void BuySuccess(MilMo_ShopItem itemBought)
	{
		Debug.Log("Successfully bought item: " + itemBought.Item.Identifier);
		if (!_remoteShop.IsGift)
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Swap);
			_currentlyHideFromSelection.Add(itemBought.Item);
			RefreshSelection();
			if (itemBought.Item.IsWearable())
			{
				MilMo_Player.Instance.Inventory.AddToStoreAndRemoveByCategory(itemBought.Item);
				MilMo_Player.Instance.Avatar.AsyncApply();
			}
		}
		else
		{
			Singleton<GameNetwork>.Instance.RequestNotificationUpdate(_lastGiftFriendId);
		}
		_isBuying = false;
	}

	private void BuyFailed(MilMo_ShopItem itemNotBought, sbyte failCode)
	{
		MilMo_LocString displayName = itemNotBought.DisplayName;
		bool juneCoinAlert = false;
		MilMo_LocString locString;
		if (failCode != 1)
		{
			if (failCode != 2)
			{
				if (failCode == 11)
				{
					if (_remoteShop.IsGift)
					{
						locString = MilMo_Localization.GetLocString("CharacterShop_5651");
						locString.SetFormatArgs(_lastGiftFriendAvatarName, displayName);
					}
					else
					{
						locString = MilMo_Localization.GetLocString("CharacterShop_271");
						locString.SetFormatArgs(displayName);
					}
				}
				else if (failCode == 10 && _remoteShop.IsGift)
				{
					locString = MilMo_Localization.GetLocString("CharacterShop_6088");
					locString.SetFormatArgs(displayName, _lastGiftFriendAvatarName);
				}
				else
				{
					locString = MilMo_Localization.GetLocString("CharacterShop_264");
					locString.SetFormatArgs(displayName, failCode);
				}
			}
			else if (_remoteShop.IsGift)
			{
				locString = MilMo_Localization.GetLocString("CharacterShop_5650");
				locString.SetFormatArgs(_lastGiftFriendAvatarName, displayName);
			}
			else
			{
				locString = MilMo_Localization.GetLocString("CharacterShop_263");
				locString.SetFormatArgs(displayName);
			}
		}
		else
		{
			locString = MilMo_Localization.GetLocString("CharacterShop_262");
			locString.SetFormatArgs(MilMo_Monetization.Instance.Currency.Name, displayName);
			juneCoinAlert = true;
		}
		MilMo_Dialog failedToBuyDialog = new MilMo_Dialog(_ui);
		_ui.AddChild(failedToBuyDialog);
		failedToBuyDialog.DoWarning(MilMo_Localization.GetLocString("CharacterShop_240"), locString, delegate
		{
			failedToBuyDialog.CloseAndRemove(null);
			_isBuying = false;
			if (juneCoinAlert)
			{
				DoNoJuneCoinAlert();
			}
		});
	}

	private void BuyAsGift(object itemAsObject)
	{
		if (giftMenu == null)
		{
			giftMenu = new MilMoGiftMenu(MilMo_GlobalUI.GetSystemUI);
			_ui.AddChild(giftMenu);
		}
		MilMo_ShopItem itemToBuy = itemAsObject as MilMo_ShopItem;
		giftMenu.Open();
		giftMenu.SendGiftButton.Function = delegate
		{
			Friend bud = giftMenu.GetFriend();
			if (bud == null)
			{
				MilMo_Dialog failDialog = new MilMo_Dialog(_ui);
				_ui.AddChild(failDialog);
				failDialog.DoWarning(MilMo_Localization.GetLocString("World_5964"), MilMo_Localization.GetLocString("World_5963"), delegate
				{
					failDialog.CloseAndRemove(null);
				});
				failDialog.BringToFront();
			}
			else
			{
				MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
				MilMo_Dialog acceptBuyDialog = new MilMo_Dialog(_ui);
				_ui.AddChild(acceptBuyDialog);
				MilMo_LocString locString = MilMo_Localization.GetLocString("World_5962");
				if (itemToBuy != null)
				{
					locString.SetFormatArgs(itemToBuy.DisplayName, bud.Name);
					acceptBuyDialog.DoOKCancel("Batch01/Textures/Core/Invisible", MilMo_Localization.GetLocString("World_5961"), locString, delegate
					{
						MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Confirm);
						Buy(itemAsObject, bud.Name, bud.UserIdentifier.ToString());
						acceptBuyDialog.CloseAndRemove(null);
						giftMenu.Close(null);
					}, delegate
					{
						MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Cancel);
						acceptBuyDialog.CloseAndRemove(null);
					});
					acceptBuyDialog.BringToFront();
					acceptBuyDialog.Icon.SetTexture(new MilMo_Texture((Texture)null));
					itemToBuy.AsyncGetIcon(delegate(Texture2D icon)
					{
						acceptBuyDialog.Icon.SetTexture(new MilMo_Texture(icon));
					});
				}
			}
		};
	}

	public static void SelectItem(string itemTemplateIdentifier)
	{
		if (_theCharacterShop != null && _theCharacterShop.enabled)
		{
			foreach (MilMo_CharacterShopCategory category in _theCharacterShop._categoryList)
			{
				foreach (MilMo_ScrollView scrollView in category.ScrollViewList)
				{
					foreach (MilMo_Widget child in scrollView.Children)
					{
						if (child is MilMo_ItemCard milMo_ItemCard)
						{
							MilMo_ShopItem milMo_ShopItem = (MilMo_ShopItem)milMo_ItemCard.BuyButton.Args;
							if (milMo_ShopItem.Item.Template.Identifier.Equals(itemTemplateIdentifier))
							{
								_theCharacterShop.BrowseToItemCard(milMo_ShopItem.Id, mayOpenMembershipScreen: false);
								return;
							}
						}
					}
				}
			}
			return;
		}
		_itemToSelectOnEnter = itemTemplateIdentifier;
	}
}
