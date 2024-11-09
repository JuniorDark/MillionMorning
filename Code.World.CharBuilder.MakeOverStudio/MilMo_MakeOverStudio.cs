using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Avatar;
using Code.Core.BodyPack;
using Code.Core.BodyPack.ColorSystem;
using Code.Core.Camera;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.Input;
using Code.Core.Items;
using Code.Core.Music;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.Core.Visual;
using Code.World.GUI.LoadingScreen;
using Code.World.Inventory;
using Code.World.Player;
using Core;
using Core.Avatar.Presets;
using Core.State;
using UnityEngine;

namespace Code.World.CharBuilder.MakeOverStudio;

public sealed class MilMo_MakeOverStudio : MonoBehaviour
{
	private delegate void InitializeDone(bool success);

	public MilMo_CameraMover cameraMover = new MilMo_CameraMover();

	private readonly Vector3 _defaultAngle = new Vector3(0f, 205f, 0f);

	private readonly Vector3 _frontAngle = new Vector3(0f, 180f, 0f);

	private readonly Vector3 _startCameraPos = new Vector3(45.01f, 17f, 0f);

	private readonly Vector3 _startCameraAngle = new Vector3(0f, 0f, 0f);

	private const float START_CAMERA_ZOOM = 20f;

	private readonly Vector3 _bodyCameraPos = new Vector3(-0.4f, 0f, 0f);

	private readonly Vector3 _bodyCameraAngle = new Vector3(0f, 0f, 0f);

	private const float BODY_CAMERA_ZOOM = 20f;

	private readonly Vector3 _clothesCameraPos = new Vector3(-0.4f, 0.3f, 0.84f);

	private readonly Vector3 _clothesCameraAngle = new Vector3(2.9f, -0.14f, 0f);

	private const float CLOTHES_CAMERA_ZOOM = 20f;

	private readonly Vector3 _shoesCameraPos = new Vector3(-0.34f, 0.98f, 2.17f);

	private readonly Vector3 _shoesCameraAngle = new Vector3(20f, -0.14f, 0f);

	private const float SHOES_CAMERA_ZOOM = 20f;

	private readonly Vector3 _faceCameraPos = new Vector3(-0.122f, -0.155f, 2.748f);

	private readonly Vector3 _faceCameraAngle = new Vector3(-9.391f, -2.878f, 0f);

	private const float FACE_CAMERA_ZOOM = 20f;

	private readonly Vector3 _hairCameraPos = new Vector3(-0.122f, 0.55f, 2.748f);

	private readonly Vector3 _hairCameraAngle = new Vector3(3.06f, -2.878f, 0f);

	private const float HAIR_CAMERA_ZOOM = 20f;

	private MilMo_UserInterface _ui;

	private MilMo_AvatarHandler _avatarHandler;

	private const string UI_IDENTIFIER = "MakeOverStudio";

	public const byte CHAR_BUILDER_REQUEST_SUCCESS = 0;

	private bool _redoFall = true;

	private const float MIN_BOY_HEIGHT = 0.9f;

	private const float MAX_BOY_HEIGHT = 1.1f;

	private const float MIN_GIRL_HEIGHT = 0.85f;

	private const float MAX_GIRL_HEIGHT = 1.05f;

	public int screenWidth = 1024;

	public int screenHeight = 720;

	private int _charState = 5;

	private MilMo_LocString[] _boyEyeBrowName;

	private MilMo_LocString[] _girlEyeBrowName;

	private const int FALLING = 0;

	private const int IMPACT = 1;

	private const int STOPPING = 2;

	private const int USER_CONTROL = 3;

	private const int SWAPPING = 4;

	private const int PAUSED = 5;

	private MilMo_MakeOverStudioWindow _window;

	private MilMo_Widget _charSpinner;

	private MilMo_Widget _cameraWidget;

	private MilMo_Avatar _boy;

	private MilMo_Avatar _girl;

	private int _charBodyCount;

	public readonly MilMo_AvatarIcon[] AvatarIcons = new MilMo_AvatarIcon[6];

	public readonly MilMo_AudioClip TickSound = new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/CharBuilderTick");

	public MilMo_AudioClip SelectCatSound = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/SelectSubCategory");

	private readonly MilMo_AudioClip _plopSound = new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/Pick");

	private readonly MilMo_AudioClip _selectSound = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/SelectAndClose");

	private readonly MilMo_AudioClip _swishSound = new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/Swish");

	private readonly MilMo_AudioClip _thudSound = new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/ThudSoft");

	private readonly MilMo_AudioClip _inflateSound = new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/Inflate");

	private readonly MilMo_AudioClip _deflateSound = new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/Deflate");

	private InitializeDone _initializeCallback;

	private bool _waitingForRemoteCharBuilder = true;

	private bool _waitingForObjectsInitialization = true;

	private bool _shouldInitializeObject = true;

	private static float _startTime;

	private static bool _timeOutDialog;

	private static bool _timeOutActive = true;

	private GameObject _character;

	private MilMo_VisualRep _platform;

	private MilMo_VisualRep _ocean;

	private MilMo_VisualRep _foreground;

	private MilMo_VisualRep _cloudBlend;

	private MilMo_VisualRep _backdrop;

	private MilMo_VisualRep _windowBack;

	private MilMo_VisualRep _diffuseCloud;

	private MilMo_VisualRep _cloudCylinder;

	private float _objectAngle;

	private GameObject _makeOverStudioLight;

	private GameObject _viewPortReset;

	private readonly MilMo_ObjectMover _charMover = new MilMo_ObjectMover();

	private readonly MilMo_ObjectMover _swapInMover = new MilMo_ObjectMover();

	private readonly MilMo_ObjectMover _swapOutMover = new MilMo_ObjectMover();

	private readonly MilMo_ObjectMover _swapFarMover = new MilMo_ObjectMover();

	private readonly MilMo_ObjectMover _platMover = new MilMo_ObjectMover();

	private readonly MilMo_Mover _heightMover = new MilMo_Mover();

	private MilMo_ShopEmoteHandler _shopEmoteHandler;

	private bool _isApplying;

	private bool _isBuying;

	private readonly Vector3 _swapStartPos = new Vector3(0.08f, -0.58f, 7f);

	private readonly Vector3 _boySwapMidPos = new Vector3(-0.4f, -0.6f, 6.5f);

	private readonly Vector3 _girlSwapMidPos = new Vector3(0.4f, -0.6f, 6.5f);

	private readonly Vector3 _platformPos = new Vector3(0f, -0.6f, 6f);

	private readonly Vector3 _startFallingPos = new Vector3(0f, 4f, 6f);

	private readonly Vector3 _hiddenPos = new Vector3(0f, -100f, 0f);

	private MilMo_TimerEvent _autoEmoteTimer;

	private int _modifyPrice;

	private static int _mainTex;

	private static int _mainColor;

	private float previousLOD = 1f;

	public const int PHYSIQUE = 0;

	public const int EYES = 1;

	public const int MOUTH = 2;

	public const int HAIR = 3;

	public const int SHIRT = 4;

	public const int PANTS = 5;

	public const int SHOES = 6;

	public const int NAME = 7;

	public MilMo_CharBuilderData RemoteCharBuilder { get; private set; }

	public void FocusOnFace()
	{
		cameraMover.GoTo(OffsetPos(_faceCameraPos));
		cameraMover.ZoomTo(20f);
		cameraMover.SetAngle(_faceCameraAngle);
		cameraMover.IsShakyCam(s: true);
		FacePlatformForward();
	}

	public void FocusOnHair()
	{
		cameraMover.GoTo(OffsetPos(_hairCameraPos));
		cameraMover.ZoomTo(20f);
		cameraMover.SetAngle(_hairCameraAngle);
		cameraMover.IsShakyCam(s: true);
		ResetPlatformAngle();
	}

	public void FocusOnClothes()
	{
		cameraMover.GoTo(OffsetPos(_clothesCameraPos));
		cameraMover.ZoomTo(20f);
		cameraMover.SetAngle(_clothesCameraAngle);
		cameraMover.IsShakyCam(s: false);
		ResetPlatformAngle();
	}

	public void FocusOnShoes()
	{
		cameraMover.GoTo(OffsetPos(_shoesCameraPos));
		cameraMover.ZoomTo(20f);
		cameraMover.SetAngle(_shoesCameraAngle);
		cameraMover.IsShakyCam(s: true);
		ResetPlatformAngle();
	}

	public void FocusOnBody()
	{
		cameraMover.GoTo(OffsetPos(_bodyCameraPos));
		cameraMover.ZoomTo(20f);
		cameraMover.SetAngle(_bodyCameraAngle);
		cameraMover.IsShakyCam(s: false);
		ResetPlatformAngle();
	}

	public void RandomizeAvatar(object o)
	{
		if (!_isApplying && !_isBuying)
		{
			ChangeGender(MilMo_Utility.RandomInt(0, 1));
			float newHeight = (_avatarHandler.IsMale ? MilMo_Utility.RandomFloat(0.9f, 1.1f) : MilMo_Utility.RandomFloat(0.85f, 1.05f));
			ChangeHeight(newHeight);
			ChangeSkinColor(RemoteCharBuilder.SkinColors[MilMo_Utility.RandomInt(0, RemoteCharBuilder.SkinColors.Count - 1)]);
			ChangeEyeColor(RemoteCharBuilder.EyeColors[MilMo_Utility.RandomInt(0, RemoteCharBuilder.EyeColors.Count - 1)]);
			string newEyes = (_avatarHandler.IsMale ? RemoteCharBuilder.BoyEyes[MilMo_Utility.RandomInt(0, RemoteCharBuilder.BoyEyes.Count - 1)] : RemoteCharBuilder.GirlEyes[MilMo_Utility.RandomInt(0, RemoteCharBuilder.GirlEyes.Count - 1)]);
			ChangeEyes(newEyes);
			string newMouth = (_avatarHandler.IsMale ? RemoteCharBuilder.BoyMouths[MilMo_Utility.RandomInt(0, RemoteCharBuilder.BoyMouths.Count - 1)] : RemoteCharBuilder.GirlMouths[MilMo_Utility.RandomInt(0, RemoteCharBuilder.GirlMouths.Count - 1)]);
			ChangeMouth(newMouth);
			string newEyeBrows = (_avatarHandler.IsMale ? RemoteCharBuilder.BoyEyeBrows[MilMo_Utility.RandomInt(0, RemoteCharBuilder.BoyEyeBrows.Count - 1)] : RemoteCharBuilder.GirlEyeBrows[MilMo_Utility.RandomInt(0, RemoteCharBuilder.GirlEyeBrows.Count - 1)]);
			ChangeEyeBrows(newEyeBrows);
			MilMo_Wearable newHair = (_avatarHandler.IsMale ? RemoteCharBuilder.BoyHairStyleItems[MilMo_Utility.RandomInt(0, RemoteCharBuilder.BoyHairStyleItems.Count - 1)] : RemoteCharBuilder.GirlHairStyleItems[MilMo_Utility.RandomInt(0, RemoteCharBuilder.GirlHairStyleItems.Count - 1)]);
			ChangeHair(newHair);
			ChangeHairColor(RemoteCharBuilder.HairColors[MilMo_Utility.RandomInt(0, RemoteCharBuilder.HairColors.Count - 1)]);
			MilMo_Wearable milMo_Wearable = (_avatarHandler.IsMale ? RemoteCharBuilder.BoyShirtItems[MilMo_Utility.RandomInt(0, RemoteCharBuilder.BoyShirtItems.Count - 1)] : RemoteCharBuilder.GirlShirtItems[MilMo_Utility.RandomInt(0, RemoteCharBuilder.GirlShirtItems.Count - 1)]);
			ChangeShirt(milMo_Wearable);
			ColorGroup colorGroup = milMo_Wearable.BodyPack.ColorGroups[0];
			int num = colorGroup.ColorIndices[MilMo_Utility.RandomInt(0, colorGroup.ColorIndices.Count - 1)];
			ChangeShirtColor("ColorGroup:" + colorGroup?.ToString() + "#" + num);
			MilMo_Wearable milMo_Wearable2 = (_avatarHandler.IsMale ? RemoteCharBuilder.BoyPantsItems[MilMo_Utility.RandomInt(0, RemoteCharBuilder.BoyPantsItems.Count - 1)] : RemoteCharBuilder.GirlPantsItems[MilMo_Utility.RandomInt(0, RemoteCharBuilder.GirlPantsItems.Count - 1)]);
			ChangePants(milMo_Wearable2);
			ColorGroup colorGroup2 = milMo_Wearable2.BodyPack.ColorGroups[0];
			int num2 = colorGroup2.ColorIndices[MilMo_Utility.RandomInt(0, colorGroup2.ColorIndices.Count - 1)];
			ChangePantsColor("ColorGroup:" + colorGroup2?.ToString() + "#" + num2);
			MilMo_Wearable milMo_Wearable3 = (_avatarHandler.IsMale ? RemoteCharBuilder.BoyShoesItems[MilMo_Utility.RandomInt(0, RemoteCharBuilder.BoyShoesItems.Count - 1)] : RemoteCharBuilder.GirlShoesItems[MilMo_Utility.RandomInt(0, RemoteCharBuilder.GirlShoesItems.Count - 1)]);
			ChangeShoes(milMo_Wearable3);
			IList<ColorGroup> colorGroupsSorted = milMo_Wearable3.BodyPack.ColorGroupsSorted;
			ColorGroup colorGroup3 = colorGroupsSorted[0];
			int num3 = colorGroup3.ColorIndices[MilMo_Utility.RandomInt(0, colorGroup3.ColorIndices.Count - 1)];
			ColorGroup colorGroup4 = colorGroupsSorted[1];
			int num4 = colorGroup3.ColorIndices[MilMo_Utility.RandomInt(0, colorGroup4.ColorIndices.Count - 1)];
			ChangeShoesColor("ColorGroup:" + colorGroup3.GroupName + "#" + num3, "ColorGroup:" + colorGroup4.GroupName + "#" + num4);
			AsyncApplyAll();
		}
	}

	public void ResetAvatar(object o)
	{
		if (!_isApplying && !_isBuying && _modifyPrice > 0)
		{
			MilMo_Avatar avatar = MilMo_Player.Instance.Avatar;
			ChangeGender(avatar.Gender);
			ChangeHeight(avatar.Height);
			ChangeSkinColor(avatar.SkinColor);
			ChangeEyeColor(avatar.EyeColor);
			ChangeEyes(avatar.Eyes);
			ChangeEyeBrows(avatar.EyeBrows);
			ChangeMouth(avatar.Mouth);
			ChangeHair(null);
			ChangeHairColor(avatar.HairColor);
			ChangeShirt(null);
			ChangePants(null);
			ChangeShoes(null);
			_avatarHandler.CurrentAvatar.UnequipAll();
			MilMo_Player.Instance.Inventory.EquipAllOnAvatar(_avatarHandler.CurrentAvatar);
			AsyncApplyAll();
		}
	}

	public void PrevEyeBrows(object notUsed)
	{
		if (!_isApplying && !_isBuying)
		{
			int num = Mathf.Min(RemoteCharBuilder.BoyEyeBrows.Count, RemoteCharBuilder.GirlEyeBrows.Count) - 1;
			int eyeBrowsIndex = RemoteCharBuilder.GetEyeBrowsIndex(_avatarHandler.CurrentAvatar.EyeBrows, _avatarHandler.CurrentGender);
			eyeBrowsIndex--;
			if (eyeBrowsIndex < 0)
			{
				eyeBrowsIndex = num;
			}
			ChangeEyeBrows(_avatarHandler.IsMale ? RemoteCharBuilder.BoyEyeBrows[eyeBrowsIndex] : RemoteCharBuilder.GirlEyeBrows[eyeBrowsIndex]);
			AsyncApplyAll();
		}
	}

	public void NextEyeBrows(object notUsed)
	{
		if (!_isApplying && !_isBuying)
		{
			int num = Mathf.Min(RemoteCharBuilder.BoyEyeBrows.Count, RemoteCharBuilder.GirlEyeBrows.Count) - 1;
			int eyeBrowsIndex = RemoteCharBuilder.GetEyeBrowsIndex(_avatarHandler.CurrentAvatar.EyeBrows, _avatarHandler.CurrentGender);
			eyeBrowsIndex++;
			if (eyeBrowsIndex > num)
			{
				eyeBrowsIndex = 0;
			}
			ChangeEyeBrows(_avatarHandler.IsMale ? RemoteCharBuilder.BoyEyeBrows[eyeBrowsIndex] : RemoteCharBuilder.GirlEyeBrows[eyeBrowsIndex]);
			AsyncApplyAll();
		}
	}

	public void SwapToMale(object o)
	{
		if (!_isApplying && !_isBuying)
		{
			ChangeGender(0);
			AsyncApplyAll();
		}
	}

	public void SwapToFemale(object o)
	{
		if (!_isApplying && !_isBuying)
		{
			ChangeGender(1);
			AsyncApplyAll();
		}
	}

	public void SetEyeTexture(object o)
	{
		if (!_isApplying && !_isBuying)
		{
			int index = (int)o;
			ChangeEyes(_avatarHandler.IsMale ? RemoteCharBuilder.BoyEyes[index] : RemoteCharBuilder.GirlEyes[index]);
			AsyncApplyAll();
		}
	}

	public void SetMouthTexture(object o)
	{
		if (!_isApplying && !_isBuying)
		{
			int index = (int)o;
			ChangeMouth(_avatarHandler.IsMale ? RemoteCharBuilder.BoyMouths[index] : RemoteCharBuilder.GirlMouths[index]);
			AsyncApplyAll();
		}
	}

	public void SetHairStyle(object hairStylesAsObject)
	{
		if (!_isApplying && !_isBuying)
		{
			ChangeHair(((MilMo_Wearable[])hairStylesAsObject)[(!_avatarHandler.IsMale) ? 1u : 0u]);
			AsyncApplyAll();
		}
	}

	public void SetShirt(object shirtIndexAsObject)
	{
		if (!_isApplying && !_isBuying)
		{
			int index = (int)shirtIndexAsObject;
			ChangeShirt(_avatarHandler.IsMale ? RemoteCharBuilder.BoyShirtItems[index] : RemoteCharBuilder.GirlShirtItems[index]);
			AsyncApplyAll();
		}
	}

	public void SetPants(object pantsIndexAsObject)
	{
		if (!_isApplying && !_isBuying)
		{
			int index = (int)pantsIndexAsObject;
			ChangePants(_avatarHandler.IsMale ? RemoteCharBuilder.BoyPantsItems[index] : RemoteCharBuilder.GirlPantsItems[index]);
			AsyncApplyAll();
		}
	}

	public void SetShoes(object shoesIndexAsObject)
	{
		if (!_isApplying && !_isBuying)
		{
			int index = (int)shoesIndexAsObject;
			ChangeShoes(_avatarHandler.IsMale ? RemoteCharBuilder.BoyShoesItems[index] : RemoteCharBuilder.GirlShoesItems[index]);
			AsyncApplyAll();
		}
	}

	private void ChangeGender(int newGender)
	{
		_avatarHandler.SetCurrentGender(newGender);
		ChangeHeight(_window.HeightSlider.Val);
	}

	private void ChangeHeight(float newHeight)
	{
		switch (_avatarHandler.CurrentGender)
		{
		case 0:
			_window.HeightSlider.Min = 0.9f;
			_window.HeightSlider.Max = 1.1f;
			_window.HeightSlider.Val = Mathf.Min(Mathf.Max(newHeight, 0.9f), 1.1f);
			break;
		case 1:
			_window.HeightSlider.Min = 0.85f;
			_window.HeightSlider.Max = 1.05f;
			_window.HeightSlider.Val = Mathf.Min(Mathf.Max(newHeight, 0.85f), 1.05f);
			break;
		}
		_heightMover.Target.x = _window.HeightSlider.Val;
		_heightMover.Target.y = _window.HeightSlider.Val;
		_heightMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
		_avatarHandler.CurrentAvatar.GameObject.transform.localScale = new Vector3(_heightMover.Val.x, _heightMover.Val.y, _heightMover.Val.x);
		_avatarHandler.CurrentAvatar.Height = _window.HeightSlider.Val;
	}

	private void ChangeSkinColor(string newSkinColor)
	{
		_avatarHandler.ChangeSkinColor(newSkinColor);
		MilMo_AvatarIcon[] avatarIcons = AvatarIcons;
		for (int i = 0; i < avatarIcons.Length; i++)
		{
			avatarIcons[i].ChangeSkinColor(newSkinColor);
		}
	}

	private void ChangeEyeColor(string newEyeColor)
	{
		_avatarHandler.ChangeEyeColor(newEyeColor);
		MilMo_AvatarIcon[] avatarIcons = AvatarIcons;
		for (int i = 0; i < avatarIcons.Length; i++)
		{
			avatarIcons[i].ChangeEyeColor(newEyeColor);
		}
	}

	private void ChangeHairColor(int newHairColor)
	{
		_avatarHandler.ChangeHairColor(newHairColor);
	}

	private void ChangeMouth(string newMouth)
	{
		_avatarHandler.ChangeMouth(newMouth);
	}

	private void ChangeEyes(string newEyes)
	{
		_avatarHandler.ChangeEyes(newEyes);
	}

	private void ChangeEyeBrows(string newEyeBrows)
	{
		_avatarHandler.ChangeEyeBrows(newEyeBrows);
	}

	private void ChangeHair(MilMo_Wearable newHair)
	{
		_avatarHandler.ChangeHair(newHair);
	}

	private void ChangeShirt(MilMo_Wearable newShirt)
	{
		_avatarHandler.ChangeShirt(newShirt);
	}

	private void ChangeShirtColor(string newShirtColorModifier)
	{
		MilMo_Wearable milMo_Wearable = _avatarHandler.CurrentShirt ?? (_avatarHandler.IsMale ? RemoteCharBuilder.BoyShirtItems[0] : RemoteCharBuilder.GirlShirtItems[0]);
		_avatarHandler.CurrentAvatar.UnequipLocal(_avatarHandler.CurrentShirt);
		string colorIndexFromModifier = MilMo_CharBuilderPresets.GetColorIndexFromModifier(newShirtColorModifier);
		int shirtColorIndex = RemoteCharBuilder.GetShirtColorIndex(milMo_Wearable.Template.Identifier, colorIndexFromModifier, _avatarHandler.CurrentGender);
		string colorGroup = milMo_Wearable.BodyPack.Path + ":" + milMo_Wearable.BodyPack.ColorGroups[0].GroupName;
		milMo_Wearable.UpdateColorIndex(colorGroup, milMo_Wearable.BodyPack.ColorGroups[0].ColorIndices[shirtColorIndex]);
		_avatarHandler.CurrentAvatar.EquipLocal(_avatarHandler.CurrentShirt);
	}

	private void ChangePants(MilMo_Wearable newPants)
	{
		_avatarHandler.ChangePants(newPants);
	}

	private void ChangePantsColor(string newPantsColorModifier)
	{
		MilMo_Wearable milMo_Wearable = _avatarHandler.CurrentPants ?? (_avatarHandler.IsMale ? RemoteCharBuilder.BoyPantsItems[0] : RemoteCharBuilder.GirlPantsItems[0]);
		_avatarHandler.CurrentAvatar.UnequipLocal(_avatarHandler.CurrentPants);
		string colorIndexFromModifier = MilMo_CharBuilderPresets.GetColorIndexFromModifier(newPantsColorModifier);
		int pantsColorIndex = RemoteCharBuilder.GetPantsColorIndex(milMo_Wearable.Template.Identifier, colorIndexFromModifier, _avatarHandler.CurrentGender);
		string colorGroup = milMo_Wearable.BodyPack.Path + ":" + milMo_Wearable.BodyPack.ColorGroups[0].GroupName;
		milMo_Wearable.UpdateColorIndex(colorGroup, milMo_Wearable.BodyPack.ColorGroups[0].ColorIndices[pantsColorIndex]);
		_avatarHandler.CurrentAvatar.EquipLocal(_avatarHandler.CurrentPants);
	}

	private void ChangeShoes(MilMo_Wearable newShoesItem)
	{
		_avatarHandler.ChangeShoes(newShoesItem);
	}

	private void ChangeShoesColor(string newShoesColor1Modifier, string newShoesColor2Modifier)
	{
		if (string.Compare(newShoesColor1Modifier, newShoesColor2Modifier, StringComparison.Ordinal) > 0)
		{
			throw new ArgumentException("First modifier must be alphabetically lesser or equal than second modifier when changing shoes color (" + newShoesColor1Modifier + ">" + newShoesColor2Modifier + ").");
		}
		MilMo_Wearable milMo_Wearable = _avatarHandler.CurrentShoes ?? (_avatarHandler.IsMale ? RemoteCharBuilder.BoyShoesItems[0] : RemoteCharBuilder.GirlShoesItems[0]);
		_avatarHandler.CurrentAvatar.UnequipLocal(_avatarHandler.CurrentShoes);
		string colorIndexFromModifier = MilMo_CharBuilderPresets.GetColorIndexFromModifier(newShoesColor1Modifier);
		string colorGroupNameFromModifier = MilMo_CharBuilderPresets.GetColorGroupNameFromModifier(newShoesColor1Modifier);
		string colorIndexFromModifier2 = MilMo_CharBuilderPresets.GetColorIndexFromModifier(newShoesColor2Modifier);
		string colorGroupNameFromModifier2 = MilMo_CharBuilderPresets.GetColorGroupNameFromModifier(newShoesColor2Modifier);
		int shoesFirstColorIndex = RemoteCharBuilder.GetShoesFirstColorIndex(milMo_Wearable.Template.Identifier, colorIndexFromModifier, colorGroupNameFromModifier, _avatarHandler.CurrentGender);
		int shoesSecondColorIndex = RemoteCharBuilder.GetShoesSecondColorIndex(milMo_Wearable.Template.Identifier, colorIndexFromModifier2, colorGroupNameFromModifier2, _avatarHandler.CurrentGender);
		IList<ColorGroup> colorGroupsSorted = milMo_Wearable.BodyPack.ColorGroupsSorted;
		string colorGroup = milMo_Wearable.BodyPack.Path + ":" + colorGroupsSorted[0].GroupName;
		string colorGroup2 = milMo_Wearable.BodyPack.Path + ":" + colorGroupsSorted[1].GroupName;
		milMo_Wearable.UpdateColorIndex(colorGroup, colorGroupsSorted[0].ColorIndices[shoesFirstColorIndex]);
		milMo_Wearable.UpdateColorIndex(colorGroup2, colorGroupsSorted[1].ColorIndices[shoesSecondColorIndex]);
		_avatarHandler.CurrentAvatar.EquipLocal(_avatarHandler.CurrentShoes);
	}

	private void AsyncApplyAll()
	{
		_isApplying = true;
		_avatarHandler.CurrentAvatar.AsyncApply(Callback, "char_builder");
		MilMo_AvatarIcon[] avatarIcons = AvatarIcons;
		for (int i = 0; i < avatarIcons.Length; i++)
		{
			avatarIcons[i].ApplyGender(_avatarHandler.CurrentGender);
		}
		void Callback(MilMo_Avatar avatar, string userTag)
		{
			if (_avatarHandler.CurrentGender != _avatarHandler.OldGender)
			{
				_ui.SoundFx.Play(_swishSound);
				ClearMovers();
				_boy.Enable();
				_girl.Enable();
				GameObject o = _avatarHandler.CurrentAvatar.GameObject;
				GameObject o2 = _avatarHandler.OldAvatar.GameObject;
				Vector3 pos = ((_avatarHandler.CurrentGender == 0) ? _boySwapMidPos : _girlSwapMidPos);
				Vector3 pos2 = ((_avatarHandler.CurrentGender == 0) ? _girlSwapMidPos : _boySwapMidPos);
				_swapInMover.AttachObject(o);
				_swapInMover.GoToNow(OffsetPos(_swapStartPos));
				_swapInMover.GoTo(OffsetPos(pos));
				_swapOutMover.AttachObject(o2);
				_swapOutMover.GoToNow(OffsetPos(_platformPos));
				_swapOutMover.GoTo(OffsetPos(pos2));
				_charState = 4;
				ResetPlatformAngle();
				switch (_avatarHandler.CurrentGender)
				{
				case 0:
					UseBoyMesh();
					break;
				case 1:
					UseGirlMesh();
					break;
				}
				_avatarHandler.SetOldGender(_avatarHandler.CurrentGender);
				_charState = 4;
			}
			UpdateUI();
			_isApplying = false;
		}
	}

	private static Vector3 OffsetPos(Vector3 pos)
	{
		return pos + new Vector3(4000f, -2000f, 4000f);
	}

	private static float OffsetY(float y)
	{
		return y + -2000f;
	}

	private void Awake()
	{
		RemoteCharBuilder = new MilMo_RemoteCharBuilderData();
		_avatarHandler = new MilMo_AvatarHandler();
	}

	private async void Start()
	{
		try
		{
			MilMo_Player.Instance.InCharBuilder = true;
			_startTime = Time.realtimeSinceStartup;
			_mainTex = Shader.PropertyToID("_MainTex");
			_mainColor = Shader.PropertyToID("_Color");
			previousLOD = MilMo_Lod.GlobalLodFactor;
			MilMo_Lod.GlobalLodFactor = 0.01f;
			_avatarHandler.Reset();
			Initialize(await RemoteCharBuilder.LoadDataAsync());
			_shopEmoteHandler = new MilMo_ShopEmoteHandler();
			_viewPortReset = new GameObject("ViewPortReset", typeof(Camera));
			_viewPortReset.GetComponent<Camera>().depth = 100f;
			_viewPortReset.GetComponent<Camera>().cullingMask = 0;
			_viewPortReset.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
			_boyEyeBrowName = new MilMo_LocString[4];
			_boyEyeBrowName[0] = MilMo_Localization.GetLocString("CharBuilder_32");
			_boyEyeBrowName[1] = MilMo_Localization.GetLocString("CharBuilder_33");
			_boyEyeBrowName[2] = MilMo_Localization.GetLocString("CharBuilder_34");
			_boyEyeBrowName[3] = MilMo_Localization.GetLocString("CharBuilder_35");
			_girlEyeBrowName = new MilMo_LocString[4];
			_girlEyeBrowName[0] = MilMo_Localization.GetLocString("CharBuilder_36");
			_girlEyeBrowName[1] = MilMo_Localization.GetLocString("CharBuilder_33");
			_girlEyeBrowName[2] = MilMo_Localization.GetLocString("CharBuilder_34");
			_girlEyeBrowName[3] = MilMo_Localization.GetLocString("CharBuilder_39");
			RenderSettings.fog = false;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
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
		_cameraWidget.SetPosition(0f, 0f);
		_cameraWidget.SetScale(screenWidth, screenHeight);
		_charSpinner.SetPosition((float)screenWidth / 2f, 0f);
		_charSpinner.SetScale((float)screenWidth / 2f, screenHeight);
		_window.RefreshUI();
		_ui.ScreenSizeDirty = false;
	}

	private void Update()
	{
		try
		{
			if (_ui != null && _ui.ScreenSizeDirty)
			{
				RefreshUI();
			}
			if (_window != null && _window.Window.Scale.x < 10f)
			{
				_window.Window.CurrentColor.a = 0f;
				foreach (MilMo_Widget child in _window.Window.Children)
				{
					child.CurrentColor.a = 0f;
				}
			}
			_cloudCylinder?.Update();
			_platform?.Update();
			_ocean?.Update();
			_foreground?.Update();
			_cloudBlend?.Update();
			_backdrop?.Update();
			_windowBack?.Update();
			_diffuseCloud?.Update();
			if (_window != null && (bool)_window.BgCam)
			{
				Rect screenPosition = _window.Window.GetScreenPosition();
				screenPosition.x += 5f;
				screenPosition.y += 5f;
				screenPosition.width -= 10f;
				screenPosition.height -= 10f;
				screenPosition.y = (float)Screen.height - screenPosition.height - screenPosition.y;
				screenPosition.x /= Screen.width;
				screenPosition.y /= Screen.height;
				screenPosition.width /= Screen.width;
				screenPosition.height /= Screen.height;
				_window.BgCam.rect = screenPosition;
				_window.BgCam.enabled = false;
				_viewPortReset.GetComponent<Camera>().enabled = false;
			}
			UpdatePrice();
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
			if (_timeOutActive && Time.realtimeSinceStartup - _startTime > 60f && !_timeOutDialog)
			{
				if (_waitingForRemoteCharBuilder)
				{
					MilMo_Player.WarningDialog(MilMo_Localization.GetLocString("CharBuilder_30"), MilMo_Localization.GetLocString("CharBuilder_44"));
				}
				else if (!MilMo_BodyPackSystem.AllDone)
				{
					MilMo_Player.WarningDialog(MilMo_Localization.GetLocString("CharBuilder_30"), MilMo_Localization.GetLocString("CharBuilder_45"));
				}
				_timeOutDialog = true;
			}
			if (!MilMo_BodyPackSystem.AllDone || _waitingForRemoteCharBuilder)
			{
				return;
			}
			_timeOutActive = false;
			if (_shouldInitializeObject)
			{
				MilMo_Timer.StopUnique("CharBuilder.Start");
				InitCamera();
				InitLight();
				InitMovers();
				AsyncInitObjects(delegate(bool success)
				{
					if (!success)
					{
						Debug.LogWarning("Failed to init objects for char builder");
					}
					else
					{
						Debug.Log("Char builder objects was initialized successfully");
					}
				});
				_shouldInitializeObject = false;
			}
			else if (!_waitingForObjectsInitialization)
			{
				DoMouseRotate();
				DoMovers();
				if (_avatarHandler.CurrentAvatar != null)
				{
					_avatarHandler.CurrentAvatar.Update();
				}
			}
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
			if (_avatarHandler.CurrentAvatar != null)
			{
				_avatarHandler.CurrentAvatar.LateUpdate();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private void DoMouseRotate()
	{
		if (_charState == 3 && MilMo_Pointer.LeftButton && MilMo_UserInterface.PointerFocus == _charSpinner)
		{
			float num = MilMo_Pointer.Move.x * _ui.Res.x;
			_objectAngle -= num * MilMo_Pointer.PointerSens.x;
			_platMover.SetAngle(0f, _objectAngle, 0f);
		}
	}

	private void DoMovers()
	{
		if (_charMover != null && _charMover.GameObjects.Count > 0)
		{
			_charMover.Update();
		}
		if (_platMover != null && _platMover.GameObjects.Count > 0)
		{
			_platMover.Update();
		}
		if (_swapInMover != null && _swapInMover.GameObjects.Count > 0)
		{
			_swapInMover.Update();
		}
		if (_swapOutMover != null && _swapOutMover.GameObjects.Count > 0)
		{
			_swapOutMover.Update();
		}
		if (_swapFarMover != null && _swapFarMover.GameObjects.Count > 0)
		{
			_swapFarMover.Update();
		}
		_heightMover?.Update();
		if (cameraMover != null && (bool)cameraMover.CameraObject)
		{
			cameraMover.FixedUpdate();
		}
		UpdateHeight();
		if (_charState == 3)
		{
			GetAngleFromPlatform();
			return;
		}
		if (_charMover != null && _charState == 0 && _charMover.Vel.y >= 0f)
		{
			if (!_redoFall)
			{
				_ui.SoundFx.Play(_thudSound);
			}
			_charState = 1;
			if (_avatarHandler.CurrentAvatar != null)
			{
				_avatarHandler.CurrentAvatar.PlayAnimation("SuperHover01");
				_avatarHandler.CurrentAvatar.EmitPuff("HardImpact", "Pavement");
				_avatarHandler.CurrentAvatar.EmitPuff("SoftImpact", "Grass");
			}
			if (_platMover != null)
			{
				_platMover.Impulse(0f, 0f, -0.2f, -0.2f, 0f, 0f);
				_platMover.AngleImpulse(-10f, -10f, 0f, 0f, 10f, 10f);
			}
		}
		else if (_charMover != null && _charState == 1 && _charMover.Pos.y <= _charMover.Target.y)
		{
			_charState = 2;
			if (_avatarHandler.CurrentAvatar != null)
			{
				PlayIdleAnimation();
				_avatarHandler.CurrentAvatar.AsyncApply(PlayIntroEmote);
				if (_redoFall)
				{
					MilMo_LoadingScreen.Instance.CharbuilderFirstGroundImpact();
					_redoFall = false;
					_charMover.GoToNow(OffsetPos(_hiddenPos));
					MilMo_EventSystem.At(2.5f, OnFadeIn);
				}
			}
		}
		else if (_platMover != null && _charMover != null && _charState == 2 && Mathf.Abs(_charMover.Pos.y - _platMover.Pos.y) < 0.01f)
		{
			_charMover.Pause();
			_platMover.Pos.y = _charMover.Pos.y;
			_charMover.DetachObject(_character);
			_platMover.AttachObject(_character);
			_charState = 3;
		}
		else if (_charState == 4 && _swapInMover != null && _swapInMover.Pos.z <= OffsetPos(_boySwapMidPos).z + 0.1f)
		{
			_swapInMover.GoTo(OffsetPos(_platformPos));
			_swapOutMover?.GoTo(OffsetPos(_swapStartPos));
		}
		if (_swapInMover == null || _platMover == null || (_charState == 4 && Mathf.Abs(_swapInMover.Pos.x - _platMover.Pos.x) < 0.02f))
		{
			ClearMovers();
			_swapFarMover?.GoToNow(OffsetPos(_swapStartPos));
			_swapFarMover?.SetAcceleration(0.005f, 0f, 0.1f);
			switch (_avatarHandler.CurrentGender)
			{
			case 0:
				_swapFarMover?.AttachObject(_girl.GameObject);
				_girl.Disable();
				break;
			case 1:
				_swapFarMover?.AttachObject(_boy.GameObject);
				_boy.Disable();
				break;
			}
			_charState = 3;
			_platMover?.AttachObject(_character);
		}
	}

	private void OnFadeIn()
	{
		MilMo_LoadingScreen.Instance.Hide(enableWorldUI: false);
		MilMo_Music.Instance.FadeIn("Batch01/Music/ItemShop");
		MilMo_EventSystem.At(0f, OnIntroAnim);
		_window.Window.SetScale(0f, 0f);
		cameraMover.GoToNow(OffsetPos(_bodyCameraPos));
		cameraMover.ZoomTo(20f);
		cameraMover.SetAngle(_bodyCameraAngle);
	}

	private void OnIntroAnim()
	{
		_charMover.GoToNow(OffsetPos(_startFallingPos));
		StartFalling();
		_window.StartTimerEvent = MilMo_EventSystem.At(2f, _window.StartCallback);
	}

	private void StartFalling()
	{
		ClearMovers();
		_charMover.AttachObject(_character);
		_charState = 0;
		_charMover.GoToNow(OffsetPos(_startFallingPos));
		_charMover.FallTo(OffsetY(-0.6f));
		MilMo_EventSystem.At(0.6f, delegate
		{
			_avatarHandler.CurrentAvatar.PlayAnimation("SuperFall01");
		});
		_waitingForObjectsInitialization = false;
	}

	public void AvatarIconShowMouths()
	{
		MilMo_AvatarIcon[] avatarIcons = AvatarIcons;
		foreach (MilMo_AvatarIcon milMo_AvatarIcon in avatarIcons)
		{
			if (milMo_AvatarIcon != null)
			{
				milMo_AvatarIcon.Enabled = true;
				milMo_AvatarIcon.ShowMouths();
			}
		}
	}

	public void AvatarIconShowEyes()
	{
		MilMo_AvatarIcon[] avatarIcons = AvatarIcons;
		foreach (MilMo_AvatarIcon milMo_AvatarIcon in avatarIcons)
		{
			if (milMo_AvatarIcon != null)
			{
				milMo_AvatarIcon.Enabled = true;
				milMo_AvatarIcon.ShowEyes();
			}
		}
	}

	public void AvatarIconDisable()
	{
		MilMo_AvatarIcon[] avatarIcons = AvatarIcons;
		foreach (MilMo_AvatarIcon milMo_AvatarIcon in avatarIcons)
		{
			if (milMo_AvatarIcon != null)
			{
				milMo_AvatarIcon.Enabled = false;
			}
		}
	}

	private void UpdateHeight()
	{
		if (_window != null && _window.HeightSlider != null && _boy != null && _girl != null)
		{
			float val = _window.HeightSlider.Val;
			UpdateHeightMover(val);
		}
	}

	private void UpdateHeightMover(float sliderVal)
	{
		if (_window != null && _window.HeightSlider != null && _boy != null && _girl != null)
		{
			_heightMover.Target.x = sliderVal;
			_heightMover.Target.y = sliderVal;
			_heightMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
			_boy.GameObject.transform.localScale = new Vector3(_heightMover.Val.x, _heightMover.Val.y, _heightMover.Val.x);
			_girl.GameObject.transform.localScale = new Vector3(_heightMover.Val.x, _heightMover.Val.y, _heightMover.Val.x);
			_boy.Height = sliderVal;
			_girl.Height = sliderVal;
		}
	}

	private void GetAngleFromPlatform()
	{
		_charMover.AngleNow(_platMover.Angle);
		_swapInMover.AngleNow(_platMover.Angle);
		_swapOutMover.AngleNow(_platMover.Angle);
		_swapFarMover.AngleNow(_platMover.Angle);
	}

	private void ResetPlatformAngle()
	{
		_platMover.AngleNow(0f, _platMover.Angle.y % 360f, 0f);
		_platMover.SetAngle(_defaultAngle);
		_charMover.AngleNow(0f, _charMover.Angle.y % 360f, 0f);
		_charMover.SetAngle(_defaultAngle);
		_swapInMover.AngleNow(0f, _swapInMover.Angle.y % 360f, 0f);
		_swapInMover.SetAngle(_defaultAngle);
		_swapOutMover.AngleNow(0f, _swapOutMover.Angle.y % 360f, 0f);
		_swapOutMover.SetAngle(_defaultAngle);
		_swapFarMover.AngleNow(0f, _swapFarMover.Angle.y % 360f, 0f);
		_swapFarMover.SetAngle(_defaultAngle);
		_objectAngle = _charMover.TargetAngle.y;
	}

	private void FacePlatformForward()
	{
		_platMover.AngleNow(0f, _platMover.Angle.y % 360f, 0f);
		_platMover.SetAngle(_frontAngle);
		_charMover.AngleNow(0f, _charMover.Angle.y % 360f, 0f);
		_charMover.SetAngle(_frontAngle);
		_swapInMover.AngleNow(0f, _swapInMover.Angle.y % 360f, 0f);
		_swapInMover.SetAngle(_frontAngle);
		_swapOutMover.AngleNow(0f, _swapOutMover.Angle.y % 360f, 0f);
		_swapOutMover.SetAngle(_frontAngle);
		_swapFarMover.AngleNow(0f, _swapFarMover.Angle.y % 360f, 0f);
		_swapFarMover.SetAngle(_frontAngle);
		_objectAngle = _charMover.TargetAngle.y;
	}

	private void AsyncInitObjects(InitializeDone callback)
	{
		MilMo_Timer.StartUnique("CharBuilder.AsyncInitObjects");
		MilMo_Avatar avatar = MilMo_Player.Instance.Avatar;
		_boy = new MilMo_Avatar(thumbnailMode: false);
		_boy.BodyPackManager.SetMainTextureSize(MilMo_AvatarGlobalLODSettings.LocalAvatarTextureSize);
		_boy.SetInitializedCallback(OnCharBodyArrive, "");
		_boy.InitLocal(avatar.Name, 0, avatar.SkinColor, avatar.EyeColor, avatar.HairColor, avatar.IsBoy ? avatar.Mouth : RemoteCharBuilder.BoyMouths[0], avatar.IsBoy ? avatar.Eyes : RemoteCharBuilder.BoyEyes[0], avatar.IsBoy ? avatar.EyeBrows : RemoteCharBuilder.BoyEyeBrows[0], avatar.IsBoy ? avatar.Hair : null, avatar.Height);
		_boy.GameObject.transform.position = OffsetPos(_hiddenPos);
		_girl = new MilMo_Avatar(thumbnailMode: false);
		_girl.BodyPackManager.SetMainTextureSize(MilMo_AvatarGlobalLODSettings.LocalAvatarTextureSize);
		_girl.SetInitializedCallback(OnCharBodyArrive, "");
		_girl.InitLocal(avatar.Name, 1, avatar.SkinColor, avatar.EyeColor, avatar.HairColor, avatar.IsGirl ? avatar.Mouth : RemoteCharBuilder.GirlMouths[0], avatar.IsGirl ? avatar.Eyes : RemoteCharBuilder.GirlEyes[0], avatar.IsGirl ? avatar.EyeBrows : RemoteCharBuilder.GirlEyeBrows[0], avatar.IsGirl ? avatar.Hair : null, avatar.Height);
		_girl.GameObject.transform.position = OffsetPos(OffsetPos(_hiddenPos));
		_platform = MilMo_VisualRepContainer.CreateVisualRep("Models/EarthChunk", OffsetPos(Vector3.zero), Quaternion.Euler(new Vector3(0f, 180f, 0f)));
		_platform.GameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		_platMover.AttachObject(_platform.GameObject);
		_ocean = MilMo_VisualRepContainer.CreateVisualRep("Objects/Ocean", OffsetPos(new Vector3(56.84f, -190.85f, 460.15f)), Quaternion.Euler(new Vector3(0f, 181.203f, 0f)));
		_ocean.GameObject.transform.localScale = new Vector3(0.93f, 0.85f, 1f);
		_foreground = MilMo_VisualRepContainer.CreateVisualRep("Objects/DiffuseCloudForeground", OffsetPos(new Vector3(-0.26f, -0.28f, 6.89f)), Quaternion.Euler(new Vector3(339.3f, 74.53f, 9.91f)));
		_foreground.GameObject.transform.localScale = new Vector3(0.42f, 0.99f, 1.34f);
		_cloudBlend = MilMo_VisualRepContainer.CreateVisualRep("Objects/CloudBlend", OffsetPos(new Vector3(-46.76f, -68.82f, 196.11f)), Quaternion.Euler(new Vector3(340.37f, 69.01f, 5.39f)));
		_cloudBlend.GameObject.transform.localScale = new Vector3(2.97f, 6.36f, 46.83f);
		_backdrop = MilMo_VisualRepContainer.CreateVisualRep("Objects/Backdrop", OffsetPos(new Vector3(12.15f, -84f, 56.77f)), Quaternion.Euler(new Vector3(-2f, 4f, 20f)));
		_backdrop.GameObject.transform.localScale = new Vector3(1.5f, 4f, 1.5f);
		_windowBack = MilMo_VisualRepContainer.CreateVisualRep("Objects/WindowBack", OffsetPos(new Vector3(1000f, 1000f, 1005f)), Quaternion.Euler(new Vector3(0f, 180f, 0f)));
		_windowBack.GameObject.transform.localScale = new Vector3(0.0048f, 0.0048f, 0.0048f);
		_windowBack.Renderer.material.SetTextureScale(_mainTex, new Vector2(6f, 6f));
		_windowBack.Renderer.material.SetColor(_mainColor, new Color(1f, 1f, 1f, 0.3f));
		_diffuseCloud = MilMo_VisualRepContainer.CreateVisualRep("Objects/DiffuseCloud", OffsetPos(new Vector3(7f, 1f, 12f)), Quaternion.Euler(new Vector3(0f, 180f, 0f)));
		_diffuseCloud.GameObject.transform.localScale = new Vector3(0.01f, 0.004f, 0.01f);
		for (int i = 0; i < AvatarIcons.Length; i++)
		{
			string text = (i + 1).ToString();
			AvatarIcons[i] = new MilMo_AvatarIcon();
			AvatarIcons[i].CreateBoy(_boy.SkinColor, _boy.EyeColor, _boy.HairColor, "BoyMouth0" + text, "BoyEyes0" + text, _boy.EyeBrows, null);
			AvatarIcons[i].CreateGirl(_girl.SkinColor, _girl.EyeColor, _girl.HairColor, "GirlMouth0" + text, "GirlEyes0" + text, _girl.EyeBrows, null);
			AvatarIcons[i].Highlight(new Color(0.1f, 0.1f, 0.1f, 0.1f));
			AvatarIcons[i].SetScreenRect(new Rect(0f, 0f, 0f, 0f));
			_window.EyesButton[i].AvatarIcon = AvatarIcons[i];
			_window.MouthButton[i].AvatarIcon = AvatarIcons[i];
		}
		_initializeCallback = callback;
		MilMo_VisualRepContainer.AsyncCreateVisualRep("Apps/CharBuilder/Content/Objects/CloudCylinder", null, OffsetPos(new Vector3(12.15f, -60f, 56.77f)), Quaternion.Euler(new Vector3(-2f, 4f, 20f)), new Vector3(-1f, 1f, 1f), delegate(MilMo_VisualRep visualRep)
		{
			_cloudCylinder = visualRep;
			MilMo_Timer.StopUnique("CharBuilder.AsyncInitObjects");
			_initializeCallback(success: true);
		});
		MilMo_LoadingScreen.Instance.CharbuilderSyncLoadedObjectsDone();
	}

	private void OnCharBodyArrive(MilMo_Avatar o, string s)
	{
		_charBodyCount++;
		if (_charBodyCount == 2)
		{
			MilMo_Avatar avatar = MilMo_Player.Instance.Avatar;
			_avatarHandler.SetBoyAndGirl(_boy, _girl);
			_avatarHandler.SetOldGender(avatar.Gender);
			ChangeGender(avatar.Gender);
			ChangeSkinColor(avatar.SkinColor);
			ChangeEyeColor(avatar.EyeColor);
			ChangeEyeBrows(avatar.EyeBrows);
			ChangeMouth(avatar.Mouth);
			ChangeEyes(avatar.Eyes);
			ChangeHeight(avatar.Height);
			MilMo_Player.Instance.Inventory.EquipAllOnAvatar(_avatarHandler.CurrentAvatar);
			switch (_avatarHandler.CurrentGender)
			{
			case 0:
				UseBoyMesh();
				break;
			case 1:
				UseGirlMesh();
				break;
			default:
				UseBoyMesh();
				break;
			}
			UpdateUI();
			_avatarHandler.CurrentAvatar.AsyncApply(OnBegin);
			MilMo_LoadingScreen.Instance.CharbuilderSecondBodyDone();
		}
		else
		{
			MilMo_LoadingScreen.Instance.CharbuilderFirstBodyDone();
		}
	}

	private void OnBegin(MilMo_Avatar avatar, string userTag)
	{
		StartFalling();
		if (_window.StartTimerEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_window.StartTimerEvent);
		}
	}

	private void InitCamera()
	{
		Debug.Log("Initialize camera for char builder");
		MilMo_Global.Camera.backgroundColor = new Color(0.7f, 0.7f, 1f);
		MilMo_Global.Camera.farClipPlane = 1000f;
		MilMo_Global.Camera.nearClipPlane = 0.1f;
		MilMo_Global.Camera.fieldOfView = 20f;
		MilMo_Global.Camera.depth = 0f;
		MilMo_Global.Camera.transform.position = OffsetPos(_bodyCameraPos);
		MilMo_Global.Camera.transform.rotation = Quaternion.Euler(_bodyCameraAngle);
	}

	private void InitLight()
	{
		_makeOverStudioLight = new GameObject("MakeOverStudioLight");
		Light light = _makeOverStudioLight.AddComponent<Light>();
		light.type = LightType.Directional;
		light.shadows = LightShadows.None;
		light.renderMode = LightRenderMode.ForcePixel;
		light.color = Color.white;
		light.intensity = 1f;
		_makeOverStudioLight.transform.eulerAngles = new Vector3(20f, 35f, 0f);
		RenderSettings.ambientLight = new Color(0.2f, 0.2f, 0.2f);
	}

	private void InitMovers()
	{
		Debug.Log("Initialize movers for char builder");
		_charMover.Pull = 0.05f;
		_charMover.Drag = 0.75f;
		_charMover.SetUpdateFunc(4);
		_charMover.AnglePull = 0.035f;
		_charMover.AngleDrag = 0.6f;
		_charMover.Pause();
		_heightMover.Pull = new Vector2(0.02f, 0.05f);
		_heightMover.Drag = new Vector2(0.85f, 0.85f);
		_heightMover.Val.x = _window.HeightSlider.Val;
		_heightMover.Target.x = _window.HeightSlider.Val;
		_heightMover.Val.y = _window.HeightSlider.Val;
		_heightMover.Target.y = _window.HeightSlider.Val;
		_swapInMover.SetUpdateFunc(2);
		_swapInMover.Pull = 0.4f;
		_swapInMover.Drag = 0.55f;
		_swapInMover.AnglePull = 0.035f;
		_swapInMover.AngleDrag = 0.6f;
		_swapOutMover.SetUpdateFunc(2);
		_swapOutMover.Pull = 0.4f;
		_swapOutMover.Drag = 0.55f;
		_swapOutMover.AnglePull = 0.035f;
		_swapOutMover.AngleDrag = 0.6f;
		_swapFarMover.SetUpdateFunc(6);
		_swapFarMover.AnglePull = 0.035f;
		_swapFarMover.AngleDrag = 0.6f;
		_platMover.SetUpdateFunc(2);
		_platMover.Pull = 0.045f;
		_platMover.Drag = 0.9f;
		_platMover.GoToNow(OffsetPos(_platformPos));
		_platMover.AnglePull = 0.035f;
		_platMover.AngleDrag = 0.6f;
		_platMover.AngleNow(_defaultAngle);
		_objectAngle = _platMover.Angle.y;
		GetAngleFromPlatform();
		cameraMover.Pull = 0.1f;
		cameraMover.Drag = 0.2f;
		cameraMover.AnglePull = 0.045f;
		cameraMover.AngleDrag = 0.2f;
		cameraMover.ZoomPull = 0.01f;
		cameraMover.ZoomDrag = 0.3f;
		cameraMover.GoToNow(OffsetPos(_startCameraPos));
		cameraMover.ZoomToNow(20f);
		cameraMover.AngleNow(_startCameraAngle);
		cameraMover.MinVel = new Vector3(1E-05f, 1E-05f, 1E-05f);
		cameraMover.IsShakyCam(s: false);
		cameraMover.MinShakeTime1 = 0.4f;
		cameraMover.MaxShakeTime1 = 0.6f;
		cameraMover.MinShakeAmp1.x = -0.0005f;
		cameraMover.MinShakeAmp1.y = -0.0005f;
		cameraMover.MinShakeAmp1.z = -0.0005f;
		cameraMover.MaxShakeAmp1.x = 0.0005f;
		cameraMover.MaxShakeAmp1.y = 0.0005f;
		cameraMover.MaxShakeAmp1.z = 0.0005f;
		cameraMover.MinShakeTime2 = 0.2f;
		cameraMover.MaxShakeTime2 = 0.3f;
		cameraMover.MinShakeAmp2.x = -0.0005f;
		cameraMover.MinShakeAmp2.y = -0.0005f;
		cameraMover.MinShakeAmp2.z = -0.0005f;
		cameraMover.MaxShakeAmp2.x = 0.0005f;
		cameraMover.MaxShakeAmp2.y = 0.0005f;
		cameraMover.MaxShakeAmp2.z = 0.0005f;
		cameraMover.AttachObject(MilMo_Global.Camera.gameObject);
	}

	private void Cleanup()
	{
		_ui.Cleanup();
		UnityEngine.Object.Destroy(_makeOverStudioLight);
		if (_platform != null)
		{
			_platform.Destroy();
			_platform = null;
		}
		if (_ocean != null)
		{
			_ocean.Destroy();
			_ocean = null;
		}
		if (_foreground != null)
		{
			_foreground.Destroy();
			_foreground = null;
		}
		if (_cloudBlend != null)
		{
			_cloudBlend.Destroy();
			_cloudBlend = null;
		}
		if (_backdrop != null)
		{
			_backdrop.Destroy();
			_backdrop = null;
		}
		if (_windowBack != null)
		{
			_windowBack.Destroy();
			_windowBack = null;
		}
		if (_diffuseCloud != null)
		{
			_diffuseCloud.Destroy();
			_diffuseCloud = null;
		}
		if (_cloudCylinder != null)
		{
			_cloudCylinder.Destroy();
			_cloudCylinder = null;
		}
		_boy?.Destroy();
		_girl?.Destroy();
		if (_avatarHandler.CurrentAvatar != null)
		{
			_avatarHandler.CurrentAvatar.Destroy();
		}
		UnityEngine.Object.Destroy(_window.MBgCamGameObject);
		_window.MBgCamGameObject = null;
		UnityEngine.Object.Destroy(_window.BgCam);
		_window.BgCam = null;
		_window.Destroy();
		_avatarHandler.Reset();
		UnityEngine.Object.Destroy(this);
		MilMo_AvatarIcon[] avatarIcons = AvatarIcons;
		for (int i = 0; i < avatarIcons.Length; i++)
		{
			avatarIcons[i]?.Destroy();
		}
		UnityEngine.Object.Destroy(_viewPortReset);
	}

	private void PlayIntroEmote(MilMo_Avatar avatar, string unused2)
	{
		_shopEmoteHandler.PlayIntroEmote(avatar);
	}

	public void PlayIdleAnimation()
	{
		string category = _window.ActivePage switch
		{
			1 => "EYES", 
			2 => "MOUTH", 
			3 => "HAIR", 
			4 => "SHIRT", 
			5 => "PANTS", 
			6 => "SHOES", 
			_ => "", 
		};
		if (_avatarHandler.CurrentAvatar != null)
		{
			_shopEmoteHandler.PlayIdleAnimation(_avatarHandler.CurrentAvatar, category);
		}
	}

	private void PlayAutoEmote(MilMo_Avatar avatar, string category)
	{
		_shopEmoteHandler.PlayAutoEmote(avatar, category);
	}

	public void ScheduleAutoEmote()
	{
		if (_autoEmoteTimer != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_autoEmoteTimer);
		}
		string category = _window.ActivePage switch
		{
			1 => "EYES", 
			2 => "MOUTH", 
			3 => "HAIR", 
			4 => "SHIRT", 
			5 => "PANTS", 
			6 => "SHOES", 
			_ => "", 
		};
		if (string.IsNullOrEmpty(category))
		{
			return;
		}
		_autoEmoteTimer = MilMo_EventSystem.At(MilMo_Utility.RandomInt(8, 16), delegate
		{
			MilMo_Avatar milMo_Avatar = (_avatarHandler.IsMale ? _boy : _girl);
			if (milMo_Avatar != null)
			{
				milMo_Avatar.AsyncApply(PlayAutoEmote, category);
				ScheduleAutoEmote();
			}
		});
	}

	public static void Exit(object obj)
	{
		if (MilMo_Player.Instance.OkToLeaveCharBuilder())
		{
			MilMo_Player.Instance.RequestLeaveCharBuilder();
		}
	}

	public void Leave()
	{
		MilMo_Player.Instance.InCharBuilder = false;
		MilMo_Lod.GlobalLodFactor = previousLOD;
		Cleanup();
		_ui.Enabled = false;
	}

	public void Buy(object o)
	{
		if (_isApplying || _isBuying || _modifyPrice <= 0)
		{
			return;
		}
		if (_modifyPrice > GlobalStates.Instance.playerState.gems.Get())
		{
			_window.GemCounter.ShakeInRed();
			_window.AvatarGemCounter.ShakeInRed();
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
			return;
		}
		_isBuying = true;
		bool updating = false;
		MilMo_LocString locString = MilMo_Localization.GetLocString("CharBuilder_4788");
		locString.SetFormatArgs(_modifyPrice);
		MilMo_Dialog buyDialog = new MilMo_Dialog(_ui);
		_ui.AddChild(buyDialog);
		buyDialog.BringToFront();
		buyDialog.DoYesNo("Batch01/Textures/Dialog/Info", MilMo_Localization.GetLocString("CharBuilder_4789"), locString, delegate
		{
			if (!updating)
			{
				updating = true;
				MilMo_EventSystem.Listen("player_avatar_updated", delegate(object msgAsObject)
				{
					_isBuying = false;
					if (msgAsObject is ServerUpdateAvatarResult serverUpdateAvatarResult)
					{
						if (serverUpdateAvatarResult.GetEnterResult() == 0)
						{
							MilMo_Player.WarningDialog(MilMo_Localization.GetLocString("CharBuilder_4789"), MilMo_Localization.GetLocString("CharBuilder_4790"));
							buyDialog.CloseAndRemove(null);
						}
						else
						{
							MilMo_Player.Instance.RecreateAvatar(serverUpdateAvatarResult.GetAvatar(), delegate
							{
								buyDialog.CloseAndRemove(null);
							});
						}
					}
				});
				List<Item> list = new List<Item>();
				if (_avatarHandler.CurrentShirt != null)
				{
					list.Add(_avatarHandler.CurrentShirt.ItemStruct);
				}
				if (_avatarHandler.CurrentPants != null)
				{
					list.Add(_avatarHandler.CurrentPants.ItemStruct);
				}
				if (_avatarHandler.CurrentShoes != null)
				{
					list.Add(_avatarHandler.CurrentShoes.ItemStruct);
				}
				if (_avatarHandler.CurrentHair != null)
				{
					list.Add(_avatarHandler.CurrentHair.ItemStruct);
				}
				Singleton<GameNetwork>.Instance.RequestUpdateAvatar((byte)_avatarHandler.CurrentGender, _avatarHandler.CurrentAvatar.SkinColor, _avatarHandler.CurrentAvatar.HairColor, _avatarHandler.CurrentAvatar.EyeColor, _avatarHandler.CurrentAvatar.Mouth, _avatarHandler.CurrentAvatar.Eyes, _avatarHandler.CurrentAvatar.EyeBrows, _avatarHandler.CurrentAvatar.Height, list);
			}
		}, delegate
		{
			if (!updating)
			{
				_isBuying = false;
				buyDialog.CloseAndRemove(null);
			}
		});
	}

	private void UpdatePrice()
	{
		if (!_isApplying && !_isBuying && _window != null && _avatarHandler.CurrentAvatar != null)
		{
			MilMo_Avatar avatar = MilMo_Player.Instance.Avatar;
			MilMo_Inventory inventory = MilMo_Player.Instance.Inventory;
			bool flag = false;
			int num = 0;
			if (_avatarHandler.CurrentGender != avatar.Gender)
			{
				num++;
				flag = true;
			}
			if (!flag && _avatarHandler.CurrentAvatar.SkinColor != avatar.SkinColor)
			{
				num++;
			}
			if (!flag && _avatarHandler.CurrentAvatar.EyeColor != avatar.EyeColor)
			{
				num++;
			}
			if (!flag && !MilMo_Utility.Equals(_window.HeightSlider.Val, avatar.Height, 0.01f))
			{
				num++;
			}
			if (!flag && _avatarHandler.CurrentAvatar.Mouth != avatar.Mouth)
			{
				num++;
			}
			if (!flag && _avatarHandler.CurrentAvatar.Eyes != avatar.Eyes)
			{
				num++;
			}
			if (!flag && _avatarHandler.CurrentAvatar.EyeBrows != avatar.EyeBrows)
			{
				num++;
			}
			if (_avatarHandler.CurrentHair != null && !inventory.HaveItem(_avatarHandler.CurrentHair))
			{
				num++;
			}
			if (_avatarHandler.CurrentShirt != null && !inventory.HaveItem(_avatarHandler.CurrentShirt))
			{
				num++;
			}
			if (_avatarHandler.CurrentPants != null && !inventory.HaveItem(_avatarHandler.CurrentPants))
			{
				num++;
			}
			if (_avatarHandler.CurrentShoes != null && !inventory.HaveItem(_avatarHandler.CurrentShoes))
			{
				num++;
			}
			_modifyPrice = num * 500;
			_window.UpdatePrice(_modifyPrice);
			_window.EnableResetButton(_modifyPrice > 0);
		}
	}

	private void UseBoyMesh()
	{
		_avatarHandler.CurrentAvatar.LoadPuffsForGameObject();
		_character = _avatarHandler.CurrentAvatar.GameObject;
		_window.MaleButton.SetTexture("Batch01/Textures/Dialog/ButtonPressed");
		_window.MaleButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonPressed");
		_window.MaleButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		_window.FemaleButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		_window.FemaleButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		_window.FemaleButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		MilMo_AvatarIcon[] avatarIcons = AvatarIcons;
		for (int i = 0; i < avatarIcons.Length; i++)
		{
			avatarIcons[i]?.ShowBoy();
		}
	}

	private void UseGirlMesh()
	{
		_avatarHandler.CurrentAvatar.LoadPuffsForGameObject();
		_character = _avatarHandler.CurrentAvatar.GameObject;
		_window.FemaleButton.SetTexture("Batch01/Textures/Dialog/ButtonPressed");
		_window.FemaleButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonPressed");
		_window.FemaleButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		_window.MaleButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		_window.MaleButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		_window.MaleButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		MilMo_AvatarIcon[] avatarIcons = AvatarIcons;
		for (int i = 0; i < avatarIcons.Length; i++)
		{
			avatarIcons[i]?.ShowGirl();
		}
	}

	private void ClearMovers()
	{
		_charMover.DetachAll();
		_swapInMover.DetachAll();
		_swapOutMover.DetachAll();
		_swapFarMover.DetachAll();
		_platMover.DetachAll();
		_platMover.AttachObject(_platform.GameObject);
	}

	private void Initialize(bool dataOK)
	{
		if (!dataOK)
		{
			Debug.LogWarning("Failed to get char builder data from server.");
			MilMo_Player.WarningDialog(MilMo_Localization.GetLocString("CharBuilder_30"), MilMo_Localization.GetLocString("CharBuilder_31"));
			return;
		}
		InitUI();
		InitSound();
		_waitingForRemoteCharBuilder = false;
		MilMo_LoadingScreen.Instance.CharbuilderDataLoaded();
	}

	private void InitSound()
	{
		_inflateSound.SetVolume(0.1f);
		_deflateSound.SetVolume(0.1f);
		_selectSound.SetVolume(0.01f);
		TickSound.SetPitch(1f);
		_plopSound.SetPitch(1f);
	}

	private void InitUI()
	{
		_ui = MilMo_UserInterfaceManager.CreateUserInterface("MakeOverStudio");
		UpdateRes();
		_cameraWidget = new MilMo_Widget(_ui);
		_cameraWidget.IsInvisible = true;
		_cameraWidget.SetPosition(0f, 0f);
		_cameraWidget.SetScale(screenWidth, screenHeight);
		_cameraWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		_ui.AddChild(_cameraWidget);
		_ui.ForceCameraWidget = _cameraWidget;
		_charSpinner = new MilMo_Widget(_ui);
		_charSpinner.IsInvisible = true;
		_charSpinner.SetPosition((float)screenWidth / 2f, 0f);
		_charSpinner.SetScale((float)screenWidth / 2f, screenHeight);
		_charSpinner.SetAlignment(MilMo_GUI.Align.TopLeft);
		_ui.AddChild(_charSpinner);
		_window = new MilMo_MakeOverStudioWindow(_ui, this);
		_window.InitUI();
		if (RemoteCharBuilder.SkinColors.Count >= 8)
		{
			for (int i = 0; i < 10; i++)
			{
				MilMo_Color skinColor = MilMo_ColorSystem.GetSkinColor(RemoteCharBuilder.SkinColors[i]);
				if (skinColor == null)
				{
					Debug.LogWarning("Got unknown skin color '" + RemoteCharBuilder.SkinColors[i] + "' from remote char builder");
				}
				else
				{
					_window.SkinColorBar.SetButtonColor255(i, skinColor.IconColor, skinColor.Name);
				}
			}
		}
		else
		{
			Debug.LogWarning("Didn't get enough skin colors for char builder");
		}
		_window.SkinColorBar.CustomFunction = delegate
		{
			if (!_isApplying && !_isBuying)
			{
				ChangeSkinColor(_window.SkinColorBar.SelectedColorIndex);
				AsyncApplyAll();
			}
		};
		_window.SkinColorBar.SetMouseOverSound(TickSound);
		_window.SkinColorBar.SetClickSound(_plopSound);
		if (RemoteCharBuilder.EyeColors.Count >= 10)
		{
			for (int j = 0; j < 10; j++)
			{
				MilMo_Color eyeColor = MilMo_ColorSystem.GetEyeColor(RemoteCharBuilder.EyeColors[j]);
				if (eyeColor == null)
				{
					Debug.LogWarning("Got unknown eye color '" + RemoteCharBuilder.EyeColors[j] + "' from remote char builder");
				}
				else
				{
					_window.EyeColorBar.SetButtonColor255(j, eyeColor.IconColor, eyeColor.Name);
				}
			}
		}
		else
		{
			Debug.LogWarning("Didn't get enough eye colors for char builder");
		}
		_window.EyeColorBar.CustomFunction = delegate
		{
			if (!_isApplying && !_isBuying)
			{
				ChangeEyeColor(_window.EyeColorBar.SelectedColorIndex);
				AsyncApplyAll();
			}
		};
		_window.EyeColorBar.SetMouseOverSound(TickSound);
		_window.EyeColorBar.SetClickSound(_plopSound);
		if (RemoteCharBuilder.HairColors.Count >= 10)
		{
			for (int k = 0; k < 10; k++)
			{
				MilMo_Color colorFromIndex = MilMo_BodyPackSystem.GetColorFromIndex(RemoteCharBuilder.HairColors[k]);
				if (colorFromIndex == null)
				{
					Debug.LogWarning("Got unknown hair color '" + RemoteCharBuilder.HairColors[k] + "' from remote char builder");
				}
				else
				{
					_window.HairColorBar.SetButtonColor255(k, colorFromIndex.IconColor, RemoteCharBuilder.HairColors[k].ToString());
				}
			}
		}
		else
		{
			Debug.LogWarning("Didn't get enough hair colors for char builder");
		}
		_window.HairColorBar.CustomFunction = delegate
		{
			if (!_isApplying && !_isBuying)
			{
				ChangeHairColor(int.Parse(_window.HairColorBar.SelectedColorIndex));
				AsyncApplyAll();
			}
		};
		_window.HairColorBar.SetMouseOverSound(TickSound);
		_window.HairColorBar.SetClickSound(_plopSound);
		_window.ShirtColorBar.CustomFunction = delegate
		{
			if (!_isApplying && !_isBuying)
			{
				ChangeShirtColor(_window.ShirtColorBar.SelectedColorIndex);
				AsyncApplyAll();
			}
		};
		_window.PantsColorBar.CustomFunction = delegate
		{
			if (!_isApplying && !_isBuying)
			{
				ChangePantsColor(_window.PantsColorBar.SelectedColorIndex);
				AsyncApplyAll();
			}
		};
		_window.ShoesColorBar.CustomFunction = delegate
		{
			if (!_isApplying && !_isBuying)
			{
				ChangeShoesColor(_window.ShoesColorBar.SelectedColorIndex, _window.ShoesColorBar2.SelectedColorIndex);
				AsyncApplyAll();
			}
		};
		_window.ShoesColorBar2.CustomFunction = delegate
		{
			if (!_isApplying && !_isBuying)
			{
				ChangeShoesColor(_window.ShoesColorBar.SelectedColorIndex, _window.ShoesColorBar2.SelectedColorIndex);
				AsyncApplyAll();
			}
		};
	}

	private void OnApplicationQuit()
	{
		_boy?.Destroy();
		_girl?.Destroy();
		if (_avatarHandler.CurrentAvatar != null)
		{
			_avatarHandler.CurrentAvatar.Destroy();
		}
	}

	private void UpdateUI()
	{
		int skinColorIndex = RemoteCharBuilder.GetSkinColorIndex(_avatarHandler.CurrentAvatar.SkinColor);
		_window.SkinColorBar.Select(skinColorIndex);
		int eyeColorIndex = RemoteCharBuilder.GetEyeColorIndex(_avatarHandler.CurrentAvatar.EyeColor);
		_window.EyeColorBar.Select(eyeColorIndex);
		int eyeBrowsIndex = RemoteCharBuilder.GetEyeBrowsIndex(_avatarHandler.CurrentAvatar.EyeBrows, _avatarHandler.CurrentGender);
		MilMo_LocString text = (_avatarHandler.IsMale ? _boyEyeBrowName[eyeBrowsIndex] : _girlEyeBrowName[eyeBrowsIndex]);
		_window.EyeBrowBar.TextLabel.SetText(text);
		int num = 0;
		foreach (MilMo_WearableButton button in _window.HairPage.OfType<MilMo_WearableButton>())
		{
			if (_avatarHandler.IsMale)
			{
				if (num >= RemoteCharBuilder.BoyHairStyleItems.Count)
				{
					break;
				}
				RemoteCharBuilder.BoyHairStyleItems[num].AsyncGetIcon(delegate(Texture2D icon)
				{
					button.SetAllTextures(icon);
				});
			}
			else
			{
				if (num >= RemoteCharBuilder.GirlHairStyleItems.Count)
				{
					break;
				}
				RemoteCharBuilder.GirlHairStyleItems[num].AsyncGetIcon(delegate(Texture2D icon)
				{
					button.SetAllTextures(icon);
				});
			}
			num++;
		}
		int num2 = 0;
		foreach (int hairColor in RemoteCharBuilder.HairColors)
		{
			MilMo_Color colorFromIndex = MilMo_BodyPackSystem.GetColorFromIndex(hairColor);
			_window.HairColorBar.SetButtonColor255(num2, colorFromIndex.IconColor, hairColor.ToString());
			num2++;
		}
		string colorIndexFromModifier = MilMo_CharBuilderPresets.GetColorIndexFromModifier((_avatarHandler.CurrentHair ?? ((_avatarHandler.CurrentGender == 0) ? RemoteCharBuilder.BoyHairStyleItems[0] : RemoteCharBuilder.GirlHairStyleItems[0])).ModifiersAsList[0]);
		int hairColorIndex = RemoteCharBuilder.GetHairColorIndex(colorIndexFromModifier);
		_window.HairColorBar.Select(hairColorIndex);
		int num3 = 0;
		foreach (MilMo_WearableButton button in _window.ShirtPage.OfType<MilMo_WearableButton>())
		{
			if (_avatarHandler.IsMale)
			{
				RemoteCharBuilder.BoyShirtItems[num3].AsyncGetIcon(delegate(Texture2D icon)
				{
					button.SetAllTextures(icon);
				});
			}
			else
			{
				RemoteCharBuilder.GirlShirtItems[num3].AsyncGetIcon(delegate(Texture2D icon)
				{
					button.SetAllTextures(icon);
				});
			}
			num3++;
		}
		MilMo_Wearable milMo_Wearable = _avatarHandler.CurrentShirt ?? ((_avatarHandler.CurrentGender == 0) ? RemoteCharBuilder.BoyShirtItems[0] : RemoteCharBuilder.GirlShirtItems[0]);
		ColorGroup colorGroup = milMo_Wearable.BodyPack.ColorGroups[0];
		_window.ShirtColorBar.SetText(colorGroup.DisplayName);
		_window.ShirtColorBar.ColorGroupName = colorGroup.GroupName;
		for (int i = 0; i < colorGroup.ColorIndices.Count; i++)
		{
			int index = colorGroup.ColorIndices[i];
			string id = "ColorGroup:" + colorGroup.GroupName + "#" + index;
			_window.ShirtColorBar.SetButtonColor255(i, MilMo_BodyPackSystem.GetColorFromIndex(index).IconColor, id);
		}
		string colorIndexFromModifier2 = MilMo_CharBuilderPresets.GetColorIndexFromModifier(milMo_Wearable.ModifiersAsList[0]);
		int shirtColorIndex = RemoteCharBuilder.GetShirtColorIndex(milMo_Wearable.Template.Identifier, colorIndexFromModifier2, _avatarHandler.CurrentGender);
		_window.ShirtColorBar.Select(shirtColorIndex);
		int num4 = 0;
		foreach (MilMo_WearableButton button in _window.PantsPage.OfType<MilMo_WearableButton>())
		{
			if (_avatarHandler.IsMale)
			{
				RemoteCharBuilder.BoyPantsItems[num4].AsyncGetIcon(delegate(Texture2D icon)
				{
					button.SetAllTextures(icon);
				});
			}
			else
			{
				RemoteCharBuilder.GirlPantsItems[num4].AsyncGetIcon(delegate(Texture2D icon)
				{
					button.SetAllTextures(icon);
				});
			}
			num4++;
		}
		MilMo_Wearable milMo_Wearable2 = _avatarHandler.CurrentPants ?? ((_avatarHandler.CurrentGender == 0) ? RemoteCharBuilder.BoyPantsItems[0] : RemoteCharBuilder.GirlPantsItems[0]);
		colorGroup = milMo_Wearable2.BodyPack.ColorGroups[0];
		_window.PantsColorBar.SetText(colorGroup.DisplayName);
		_window.PantsColorBar.ColorGroupName = colorGroup.GroupName;
		for (int j = 0; j < colorGroup.ColorIndices.Count; j++)
		{
			int index2 = colorGroup.ColorIndices[j];
			string id2 = "ColorGroup:" + colorGroup.GroupName + "#" + index2;
			_window.PantsColorBar.SetButtonColor255(j, MilMo_BodyPackSystem.GetColorFromIndex(index2).IconColor, id2);
		}
		string colorIndexFromModifier3 = MilMo_CharBuilderPresets.GetColorIndexFromModifier(milMo_Wearable2.ModifiersAsList[0]);
		int pantsColorIndex = RemoteCharBuilder.GetPantsColorIndex(milMo_Wearable2.Template.Identifier, colorIndexFromModifier3, _avatarHandler.CurrentGender);
		_window.PantsColorBar.Select(pantsColorIndex);
		int num5 = 0;
		foreach (MilMo_WearableButton button in _window.ShoesPage.OfType<MilMo_WearableButton>())
		{
			if (_avatarHandler.IsMale)
			{
				RemoteCharBuilder.BoyShoesItems[num5].AsyncGetIcon(delegate(Texture2D icon)
				{
					button.SetAllTextures(icon);
				});
			}
			else
			{
				RemoteCharBuilder.GirlShoesItems[num5].AsyncGetIcon(delegate(Texture2D icon)
				{
					button.SetAllTextures(icon);
				});
			}
			num5++;
		}
		MilMo_Wearable milMo_Wearable3 = _avatarHandler.CurrentShoes ?? ((_avatarHandler.CurrentGender == 0) ? RemoteCharBuilder.BoyShoesItems[0] : RemoteCharBuilder.GirlShoesItems[0]);
		IList<ColorGroup> colorGroupsSorted = milMo_Wearable3.BodyPack.ColorGroupsSorted;
		ColorGroup colorGroup2 = colorGroupsSorted[0];
		ColorGroup colorGroup3 = colorGroupsSorted[1];
		_window.ShoesColorBar.SetText(colorGroup2.DisplayName);
		_window.ShoesColorBar.ColorGroupName = colorGroup2.GroupName;
		for (int k = 0; k < colorGroup2.ColorIndices.Count; k++)
		{
			int index3 = colorGroup2.ColorIndices[k];
			string id3 = "ColorGroup:" + colorGroup2.GroupName + "#" + index3;
			_window.ShoesColorBar.SetButtonColor255(k, MilMo_BodyPackSystem.GetColorFromIndex(index3).IconColor, id3);
		}
		_window.ShoesColorBar2.SetText(colorGroup3.DisplayName);
		_window.ShoesColorBar2.ColorGroupName = colorGroup3.GroupName;
		for (int l = 0; l < colorGroup3.ColorIndices.Count; l++)
		{
			int index4 = colorGroup3.ColorIndices[l];
			string id4 = "ColorGroup:" + colorGroup3.GroupName + "#" + index4;
			_window.ShoesColorBar2.SetButtonColor255(l, MilMo_BodyPackSystem.GetColorFromIndex(index4).IconColor, id4);
		}
		IList<string> modifiersAsList = milMo_Wearable3.ModifiersAsList;
		string colorIndexFromModifier4 = MilMo_CharBuilderPresets.GetColorIndexFromModifier(modifiersAsList[0]);
		string colorGroupNameFromModifier = MilMo_CharBuilderPresets.GetColorGroupNameFromModifier(modifiersAsList[0]);
		string colorIndexFromModifier5 = MilMo_CharBuilderPresets.GetColorIndexFromModifier(modifiersAsList[1]);
		string colorGroupNameFromModifier2 = MilMo_CharBuilderPresets.GetColorGroupNameFromModifier(modifiersAsList[1]);
		int shoesFirstColorIndex = RemoteCharBuilder.GetShoesFirstColorIndex(milMo_Wearable3.Template.Identifier, colorIndexFromModifier4, colorGroupNameFromModifier, _avatarHandler.CurrentGender);
		int shoesSecondColorIndex = RemoteCharBuilder.GetShoesSecondColorIndex(milMo_Wearable3.Template.Identifier, colorIndexFromModifier5, colorGroupNameFromModifier2, _avatarHandler.CurrentGender);
		_window.ShoesColorBar.Select(shoesFirstColorIndex);
		_window.ShoesColorBar2.Select(shoesSecondColorIndex);
	}
}
