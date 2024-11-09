using Core.Settings;
using UnityEngine;

namespace Code.Core.Avatar;

public static class MilMo_AvatarGlobalLODSettings
{
	public const int MAX_RAGDOLLS = 2;

	private static int _maxNumberOfHighQualityAvatars = 8;

	private static int _maxNumberOfMediumQualityAvatars = 16;

	private static int _maxNumberOfLowQualityAvatars = 76;

	private static int _maxNumberOfHighQualityAvatarsPvp = 2;

	private static int _maxNumberOfMediumQualityAvatarsPvp = 4;

	private static int _maxNumberOfLowQualityAvatarsPvp = 84;

	public static float MediumQualitySquaredDistance = 256f;

	public static float LowQualitySquaredDistance = 1024f;

	public static float NoAvatarSquaredDistance = 5184f;

	public static int LocalAvatarTextureSize = 1024;

	public static int RemoteAvatarTextureSize = 512;

	public const SkinQuality HIGH_AVATAR_SKIN_QUALITY = SkinQuality.Auto;

	public const SkinQuality MEDIUM_AVATAR_SKIN_QUALITY = SkinQuality.Bone2;

	public const SkinQuality LOW_AVATAR_SKIN_QUALITY = SkinQuality.Bone1;

	public static bool IsPvpMode = false;

	public static int MaxNumberOfHighQualityAvatars
	{
		get
		{
			if (!IsPvpMode)
			{
				return _maxNumberOfHighQualityAvatars;
			}
			return _maxNumberOfHighQualityAvatarsPvp;
		}
		set
		{
			if (IsPvpMode)
			{
				_maxNumberOfHighQualityAvatarsPvp = value;
			}
			else
			{
				_maxNumberOfHighQualityAvatars = value;
			}
		}
	}

	public static int MaxNumberOfMediumQualityAvatars
	{
		get
		{
			if (!IsPvpMode)
			{
				return _maxNumberOfMediumQualityAvatars;
			}
			return _maxNumberOfMediumQualityAvatarsPvp;
		}
		set
		{
			if (IsPvpMode)
			{
				_maxNumberOfMediumQualityAvatarsPvp = value;
			}
			else
			{
				_maxNumberOfMediumQualityAvatars = value;
			}
		}
	}

	public static int MaxNumberOfLowQualityAvatars
	{
		get
		{
			if (!IsPvpMode)
			{
				return _maxNumberOfLowQualityAvatars;
			}
			return _maxNumberOfLowQualityAvatarsPvp;
		}
		set
		{
			if (IsPvpMode)
			{
				_maxNumberOfLowQualityAvatarsPvp = value;
			}
			else
			{
				_maxNumberOfLowQualityAvatars = value;
			}
		}
	}

	public static void LoadGlobalLODSettings(Settings.QualityTierSetting settings)
	{
		switch (settings)
		{
		case Settings.QualityTierSetting.Low:
			_maxNumberOfHighQualityAvatars = 0;
			_maxNumberOfMediumQualityAvatars = 16;
			_maxNumberOfLowQualityAvatars = 84;
			MediumQualitySquaredDistance = 144f;
			LowQualitySquaredDistance = 576f;
			NoAvatarSquaredDistance = 2304f;
			LocalAvatarTextureSize = 512;
			RemoteAvatarTextureSize = 256;
			break;
		case Settings.QualityTierSetting.Medium:
			_maxNumberOfHighQualityAvatars = 4;
			_maxNumberOfMediumQualityAvatars = 16;
			_maxNumberOfLowQualityAvatars = 80;
			MediumQualitySquaredDistance = 144f;
			LowQualitySquaredDistance = 576f;
			NoAvatarSquaredDistance = 4096f;
			LocalAvatarTextureSize = 512;
			RemoteAvatarTextureSize = 512;
			break;
		}
		_maxNumberOfHighQualityAvatarsPvp = Mathf.Min(_maxNumberOfHighQualityAvatarsPvp, _maxNumberOfHighQualityAvatars);
		_maxNumberOfMediumQualityAvatarsPvp = Mathf.Min(_maxNumberOfMediumQualityAvatarsPvp, _maxNumberOfMediumQualityAvatars);
		_maxNumberOfLowQualityAvatarsPvp = Mathf.Min(_maxNumberOfLowQualityAvatarsPvp, _maxNumberOfLowQualityAvatars);
	}
}
