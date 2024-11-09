using System;
using System.Threading.Tasks;
using Code.Core.Avatar;
using Code.World.CharBuilder.WearableHandlers;
using Core.Analytics;
using Core.Dependencies;
using Core.Utilities;
using UI.LoadingScreen;
using UnityEngine;

namespace Code.World.CharBuilder;

public class AvatarEditor : MonoBehaviour
{
	[SerializeField]
	private DependencyLoader dependencyLoader;

	[SerializeField]
	private LoadingScreen loadingScreen;

	private static bool _isInitialized;

	private AvatarHandler _male;

	private AvatarHandler _female;

	public HairStyleHandler HairStyleHandler { get; private set; }

	public ShirtHandler ShirtHandler { get; private set; }

	public PantsHandler PantsHandler { get; private set; }

	public ShoesHandler ShoesHandler { get; private set; }

	public AvatarHandler CurrentAvatarHandler { get; private set; }

	public event Action OnInitialized;

	public event Action OnSelectionReset;

	public event Action OnGenderChanged;

	public event Action<AvatarSelection.Shapes> OnColorChangedForShape;

	public event Action OnAvatarApply;

	public bool IsInitialized()
	{
		return _isInitialized;
	}

	private void Awake()
	{
		ShoesHandler = new ShoesHandler(this);
		HairStyleHandler = new HairStyleHandler(this);
		ShirtHandler = new ShirtHandler(this);
		PantsHandler = new PantsHandler(this);
		_female?.Destroy();
		_male?.Destroy();
	}

	private async void Start()
	{
		MilMoAnalyticsHandler.CharbuilderStart();
		UpdateLoadingScreen("Initializing", 0f);
		dependencyLoader.OnProgress += OnDependencyLoaderProgress;
		if (!(await dependencyLoader.LoadDependencies()))
		{
			Debug.LogError("Failed to meet dependencies!");
			return;
		}
		await InitializeAvatarHandlers();
		await Task.Delay(500);
		_isInitialized = true;
		this.OnInitialized?.Invoke();
		UpdateLoadingScreen("Ready!", 1f);
	}

	public void AvatarUpdated()
	{
		AsyncApplyAll();
	}

	public void ColorUpdatedForShape(AvatarSelection.Shapes shape)
	{
		this.OnColorChangedForShape?.Invoke(shape);
	}

	private void OnDependencyLoaderProgress(string text, float progress)
	{
		UpdateLoadingScreen(text, progress * 0.9f);
	}

	private void UpdateLoadingScreen(string text, float progress)
	{
		loadingScreen.UpdateProgress(progress);
		loadingScreen.UpdateText(text);
	}

	private async Task InitializeAvatarHandlers()
	{
		_male = new AvatarHandler(AvatarGender.Male);
		await _male.InitAvatar();
		EquipAvatar(_male);
		_female = new AvatarHandler(AvatarGender.Female);
		await _female.InitAvatar();
		EquipAvatar(_female);
		SetAvatarName(ValidNameGenerator.GetValidName());
		SetupCurrentAvatar();
		this.OnSelectionReset?.Invoke();
	}

	private void EquipAvatar(AvatarHandler handler)
	{
		CurrentAvatarHandler = handler;
		AvatarSelection selection = handler.GetSelection();
		HairStyleHandler.SetHairStyle(selection.HairStyle);
		HairStyleHandler.SetHairColor(selection.HairColor);
		ShirtHandler.SetShirt(selection.Shirt);
		PantsHandler.SetPants(selection.Pants);
		ShoesHandler.SetShoes(selection.Shoes);
		SetSkinColor(selection.SkinColor);
		SetEyeColor(selection.EyeColor);
		SetMouth(selection.Mouth);
		SetEyes(selection.Eyes);
		SetEyeBrows(selection.EyeBrows);
		SetHeight(selection.Height);
		CurrentAvatarHandler = null;
	}

	private void OnDestroy()
	{
		_male?.Destroy();
		_female?.Destroy();
	}

	public AvatarHandler[] GetAvatarHandlers()
	{
		return new AvatarHandler[2] { _male, _female };
	}

	public AvatarSelection GetCurrentSelection()
	{
		if (CurrentAvatarHandler == null)
		{
			Debug.LogError("CurrentAvatarHandler not set yet!");
			return null;
		}
		return CurrentAvatarHandler.GetSelection();
	}

	private MilMo_Avatar SetupCurrentAvatar(MilMo_Avatar avatar = null)
	{
		if (avatar != null)
		{
			CurrentAvatarHandler = ((avatar.Gender == 0) ? _male : _female);
			SetSkinColor(avatar.SkinColor);
			SetEyeColor(avatar.EyeColor);
			HairStyleHandler.SetHairColor(avatar.HairColor);
			SetMouth(avatar.Mouth);
			SetEyes(avatar.Eyes);
			SetEyeBrows(avatar.EyeBrows);
			SetHeight(avatar.Height);
		}
		else
		{
			SetRandomGender();
		}
		return CurrentAvatarHandler.GetAvatar();
	}

	private void SetRandomGender()
	{
		if ((double)UnityEngine.Random.value >= 0.5)
		{
			ChangeToFemale();
		}
		else
		{
			ChangeToMale();
		}
	}

	public void GenerateRandomSelection()
	{
		string avatarName = CurrentAvatarHandler.GetSelection().AvatarName;
		AvatarGender randomGender = GetRandomGender();
		AvatarSelection selection = new AvatarHandler(randomGender).GetSelection();
		selection.SetAvatarName(avatarName);
		if (randomGender == AvatarGender.Female)
		{
			_female.SetSelection(selection);
			EquipAvatar(_female);
			SetGender(1);
		}
		else
		{
			_male.SetSelection(selection);
			EquipAvatar(_male);
			SetGender(0);
		}
		this.OnSelectionReset?.Invoke();
	}

	private AvatarGender GetRandomGender()
	{
		if (!((double)UnityEngine.Random.value >= 0.5))
		{
			return AvatarGender.Male;
		}
		return AvatarGender.Female;
	}

	private void SwapGender()
	{
		if (CurrentAvatarHandler.Equals(_male))
		{
			ChangeToFemale();
		}
		else
		{
			ChangeToMale();
		}
	}

	private void ChangeToMale()
	{
		if (CurrentAvatarHandler == null || !CurrentAvatarHandler.Equals(_male))
		{
			_female.Hide();
			_male.Show();
			CurrentAvatarHandler = _male;
			this.OnGenderChanged?.Invoke();
		}
	}

	private void ChangeToFemale()
	{
		if (CurrentAvatarHandler == null || !CurrentAvatarHandler.Equals(_female))
		{
			_male.Hide();
			_female.Show();
			CurrentAvatarHandler = _female;
			this.OnGenderChanged?.Invoke();
		}
	}

	public float GetHeight(AvatarGender gender)
	{
		if (gender != 0)
		{
			return _female.GetSelection().Height;
		}
		return _male.GetSelection().Height;
	}

	public void SetGender(int gender)
	{
		if (gender == 0)
		{
			ChangeToFemale();
		}
		else
		{
			ChangeToMale();
		}
		SwapGender();
	}

	public void SetSkinColor(string skinColor)
	{
		CurrentAvatarHandler.ChangeSkinColor(skinColor);
		AsyncApplyAll();
	}

	public void SetEyeColor(string eyeColor)
	{
		CurrentAvatarHandler.ChangeEyeColor(eyeColor);
		AsyncApplyAll();
	}

	public void SetMouth(string mouth)
	{
		CurrentAvatarHandler.ChangeMouth(mouth);
		AsyncApplyAll();
	}

	public void SetEyes(string eyes)
	{
		CurrentAvatarHandler.ChangeEyes(eyes);
		AsyncApplyAll();
	}

	public void SetEyeBrows(string eyeBrows)
	{
		CurrentAvatarHandler.ChangeEyeBrows(eyeBrows);
		AsyncApplyAll();
	}

	public void SetHeight(float height)
	{
		CurrentAvatarHandler?.ChangeHeight(height);
		AsyncApplyAll();
	}

	public void SetMood(string mood)
	{
		_female.ChangeMood(mood);
		_male.ChangeMood(mood);
	}

	public void SetAvatarName(string avatarName)
	{
		_female.ChangeName(avatarName);
		_male.ChangeName(avatarName);
	}

	public void AsyncApplyAll()
	{
		CurrentAvatarHandler.GetAvatar().AsyncApply(Callback, "AvatarEditor");
		void Callback(MilMo_Avatar avatar, string userTag)
		{
			this.OnAvatarApply?.Invoke();
		}
	}
}
