using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.Global;
using Code.World.Player;
using UnityEngine;

namespace Code.World;

public static class MilMo_AvatarLODManager
{
	private class MilMo_AvatarLODData
	{
		public MilMo_RemotePlayer RemotePlayer;

		public MilMo_Avatar Avatar;

		public float SquaredDistance;
	}

	private static readonly List<MilMo_AvatarLODData> TheAvatars = new List<MilMo_AvatarLODData>();

	public static void StartLODUpdate()
	{
		TheAvatars.Clear();
	}

	public static void AddAvatar(MilMo_RemotePlayer remotePlayer)
	{
		if (remotePlayer?.Avatar?.BodyPackManager?.SoftMeshManager == null)
		{
			Debug.LogWarning("Trying to add null avatar (or null body pack manager) to avatar LOD manager");
			return;
		}
		MilMo_AvatarLODData milMo_AvatarLODData = new MilMo_AvatarLODData
		{
			RemotePlayer = remotePlayer,
			Avatar = remotePlayer.Avatar
		};
		milMo_AvatarLODData.SquaredDistance = Vector3.SqrMagnitude(MilMo_Global.MainCamera.transform.position - milMo_AvatarLODData.Avatar.Position);
		for (int i = 0; i < TheAvatars.Count; i++)
		{
			if (TheAvatars[i].SquaredDistance > milMo_AvatarLODData.SquaredDistance)
			{
				TheAvatars.Insert(i, milMo_AvatarLODData);
				return;
			}
		}
		TheAvatars.Add(milMo_AvatarLODData);
	}

	public static void UpdateLOD()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < TheAvatars.Count; i++)
		{
			MilMo_AvatarLODData milMo_AvatarLODData = TheAvatars[i];
			if (!milMo_AvatarLODData.RemotePlayer.InShop && !milMo_AvatarLODData.RemotePlayer.InWorldMap)
			{
				if (milMo_AvatarLODData.SquaredDistance > MilMo_AvatarGlobalLODSettings.NoAvatarSquaredDistance || MilMo_Player.Instance.InSinglePlayerArea || MilMo_Player.Instance.Avatar.Room != milMo_AvatarLODData.Avatar.Room)
				{
					UpdateQuality(milMo_AvatarLODData.Avatar, MilMo_AvatarQuality.Disabled);
				}
				else if (milMo_AvatarLODData.SquaredDistance > MilMo_AvatarGlobalLODSettings.LowQualitySquaredDistance)
				{
					UpdateQuality(milMo_AvatarLODData.Avatar, MilMo_AvatarQuality.Low);
					num3++;
				}
				else if (milMo_AvatarLODData.SquaredDistance > MilMo_AvatarGlobalLODSettings.MediumQualitySquaredDistance)
				{
					UpdateQuality(milMo_AvatarLODData.Avatar, MilMo_AvatarQuality.Medium);
					num2++;
				}
				else if (num < MilMo_AvatarGlobalLODSettings.MaxNumberOfHighQualityAvatars)
				{
					UpdateQuality(milMo_AvatarLODData.Avatar, MilMo_AvatarQuality.High);
					num++;
				}
				else if (num2 < MilMo_AvatarGlobalLODSettings.MaxNumberOfMediumQualityAvatars)
				{
					UpdateQuality(milMo_AvatarLODData.Avatar, MilMo_AvatarQuality.Medium);
					num2++;
				}
				else if (num3 < MilMo_AvatarGlobalLODSettings.MaxNumberOfLowQualityAvatars)
				{
					UpdateQuality(milMo_AvatarLODData.Avatar, MilMo_AvatarQuality.Low);
					num3++;
				}
			}
		}
	}

	private static void UpdateQuality(MilMo_Avatar avatar, MilMo_AvatarQuality quality)
	{
		if (quality == MilMo_AvatarQuality.High && avatar.Quality != 0)
		{
			avatar.Enable();
			avatar.Renderer.quality = SkinQuality.Auto;
			avatar.EnableAnimations();
			avatar.EmoteManager.Enable = true;
			avatar.BodyPackManager.HighQualityShader = true;
			avatar.EnableBlobShadows();
			avatar.EnableRefraction();
			avatar.EnableSuperAliveness();
		}
		else if (quality == MilMo_AvatarQuality.Medium && avatar.Quality != MilMo_AvatarQuality.Medium)
		{
			avatar.Enable();
			avatar.Renderer.quality = SkinQuality.Bone2;
			avatar.EnableAnimations();
			avatar.EmoteManager.Enable = true;
			avatar.BodyPackManager.HighQualityShader = true;
			avatar.DisableBlobShadows();
			avatar.DisableRefraction();
			avatar.DisableSuperAliveness();
		}
		else if (quality == MilMo_AvatarQuality.Low && avatar.Quality != MilMo_AvatarQuality.Low)
		{
			avatar.Enable();
			avatar.Renderer.quality = SkinQuality.Bone1;
			avatar.EnableAnimations();
			avatar.EmoteManager.Enable = false;
			avatar.BodyPackManager.HighQualityShader = false;
			avatar.DisableBlobShadows();
			avatar.DisableRefraction();
			avatar.DisableSuperAliveness();
		}
		else if (quality == MilMo_AvatarQuality.Disabled && avatar.Quality != MilMo_AvatarQuality.Disabled)
		{
			avatar.Disable();
			avatar.DisableAnimations();
			avatar.EmoteManager.Enable = false;
			avatar.DisableBlobShadows();
			avatar.DisableSuperAliveness();
		}
		avatar.Quality = quality;
	}
}
