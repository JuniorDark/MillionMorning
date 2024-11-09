using System.Collections.Generic;
using Code.Core.Visual;
using UnityEngine;

namespace Code.Core.Avatar.BlobShadow;

public class BlobShadowHandler
{
	private MilMo_Avatar _avatar;

	private readonly List<MilMo_BlobShadow> _blobShadows = new List<MilMo_BlobShadow>();

	private readonly MilMo_BlobShadowData[] _blobData = new MilMo_BlobShadowData[6]
	{
		new MilMo_BlobShadowData("ShadowMaterial", "SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase", 60f, 0f, 1.5f, orthographic: false, 0f),
		new MilMo_BlobShadowData("ShadowMaterial", "LHip", 80f, 0f, 1.5f, orthographic: false, 0f),
		new MilMo_BlobShadowData("ShadowMaterial", "RHip", 80f, 0f, 1.5f, orthographic: false, 0f),
		new MilMo_BlobShadowData("ShadowMaterial", "LHip/LKnee", 90f, 0f, 1.5f, orthographic: false, 0f),
		new MilMo_BlobShadowData("ShadowMaterial", "RHip/RKnee", 90f, 0f, 1.5f, orthographic: false, 0f),
		new MilMo_BlobShadowData("ShadowMaterial02", "SpineLumbarBase", 5f, 2.5f, 20f, orthographic: true, 0.45f)
	};

	private readonly MilMo_BlobShadowData[] _remoteBlobData = new MilMo_BlobShadowData[1]
	{
		new MilMo_BlobShadowData("ShadowMaterial", "SpineLumbarBase/SpineLumbarLo/SpineThoracicLo/SpineThoracicBase/NeckBase", 60f, 0f, 1.5f, orthographic: false, 0f)
	};

	public void Init(MilMo_Avatar avatar)
	{
		_avatar = avatar;
		foreach (MilMo_BlobShadow blobShadow in _blobShadows)
		{
			blobShadow.Destroy();
		}
		_blobShadows.Clear();
		MilMo_BlobShadowData[] array = (_avatar.IsTheLocalPlayer ? _blobData : _remoteBlobData);
		foreach (MilMo_BlobShadowData milMo_BlobShadowData in array)
		{
			Transform transform = _avatar.GameObject.transform.Find(milMo_BlobShadowData.Bone);
			if ((bool)transform)
			{
				MilMo_BlobShadow milMo_BlobShadow = transform.gameObject.AddComponent<MilMo_BlobShadow>();
				if ((bool)milMo_BlobShadow)
				{
					milMo_BlobShadow.AsyncLoad(milMo_BlobShadowData);
					milMo_BlobShadow.IgnoreMask(61440);
					_blobShadows.Add(milMo_BlobShadow);
				}
			}
		}
	}

	public void Enable()
	{
		foreach (MilMo_BlobShadow blobShadow in _blobShadows)
		{
			blobShadow.Enabled = true;
		}
	}

	public void Disable()
	{
		foreach (MilMo_BlobShadow blobShadow in _blobShadows)
		{
			blobShadow.Enabled = false;
		}
	}
}
