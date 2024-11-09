using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Visual;
using UnityEngine;

namespace Code.Core.BodyPack.HairSystem;

internal class MilMo_HairAddon : MilMo_Addon
{
	private class PlayerData
	{
		public GameObject Hair;

		public GameObject LOD;

		public GameObject HatHair;

		public GameObject HatHairLod;

		public bool HaveHat;

		public bool HideHair;

		public bool Enabled = true;
	}

	private const float HAIR_LOD_DISTANCE_SQUARED = 64f;

	private readonly Dictionary<int, PlayerData> _playerData = new Dictionary<int, PlayerData>();

	private AddonDone _loadingContentCallback;

	private SkinnedMeshRenderer _loadingContentRenderer;

	public MilMo_HairAddon(string node, string visualRepPath, bool scale, string bodyPackPath, IList<MilMo_TextureColorGroupPair> addonLayers, Vector3 boyOffset, Vector3 girlOffset)
		: base(node, visualRepPath, scale, bodyPackPath, addonLayers, boyOffset, girlOffset)
	{
	}

	public void SetHaveHat(SkinnedMeshRenderer meshRenderer, bool hat)
	{
		if (_playerData.TryGetValue(meshRenderer.GetInstanceID(), out var value))
		{
			value.HaveHat = hat;
			Update(meshRenderer);
		}
	}

	public void SetHideHair(SkinnedMeshRenderer meshRenderer, bool hideHair)
	{
		if (_playerData.TryGetValue(meshRenderer.GetInstanceID(), out var value))
		{
			value.HideHair = hideHair;
			Update(meshRenderer);
		}
	}

	public override void Disable(SkinnedMeshRenderer renderer)
	{
		if (_playerData.TryGetValue(renderer.GetInstanceID(), out var value))
		{
			DeactivateAll(value);
			value.Enabled = false;
		}
	}

	public override void Enable(SkinnedMeshRenderer renderer)
	{
		if (_playerData.TryGetValue(renderer.GetInstanceID(), out var value))
		{
			value.Enabled = true;
			Update(renderer);
		}
	}

	public override void SetLayer(SkinnedMeshRenderer renderer, int layer)
	{
		if (_playerData.TryGetValue(renderer.GetInstanceID(), out var value) && value != null)
		{
			if (value.Hair != null)
			{
				value.Hair.layer = layer;
			}
			if (value.HatHair != null)
			{
				value.HatHair.layer = layer;
			}
			if (value.LOD != null)
			{
				value.LOD.layer = layer;
			}
			if (value.HatHairLod != null)
			{
				value.HatHairLod.layer = layer;
			}
		}
	}

	protected override void OnVisualRepLoaded(SkinnedMeshRenderer renderer, MilMo_VisualRep visualRep, bool success)
	{
		if (success)
		{
			visualRep.UpdateLods = false;
			SetupHatsAndLods(renderer);
		}
	}

	public override void Equip(SkinnedMeshRenderer meshRenderer, bool isBoy, float height = 1f)
	{
		base.Equip(meshRenderer, isBoy, height);
		if (_playerData.TryGetValue(meshRenderer.GetInstanceID(), out var value))
		{
			if (value.LOD != null)
			{
				value.LOD.SetActive(value: false);
			}
			if (value.HatHair != null)
			{
				value.HatHair.SetActive(value: false);
			}
			if (value.HatHairLod != null)
			{
				value.HatHairLod.SetActive(value: false);
			}
		}
		else
		{
			Debug.LogWarning("Trying to equip hair addon " + VisualRepPath + " without valid player data");
		}
	}

	public override void UnEquip(SkinnedMeshRenderer meshRenderer)
	{
		base.UnEquip(meshRenderer);
		if (_playerData.TryGetValue(meshRenderer.GetInstanceID(), out var value))
		{
			DeactivateAll(value);
		}
		else
		{
			Debug.LogWarning("Trying to unEquip hair '" + VisualRepPath + "' that hasn't been equipped");
		}
	}

	protected override void Unload(SkinnedMeshRenderer meshRenderer)
	{
		base.Unload(meshRenderer);
		if (_playerData.TryGetValue(meshRenderer.GetInstanceID(), out var _))
		{
			_playerData.Remove(meshRenderer.GetInstanceID());
		}
	}

	public override void Update(SkinnedMeshRenderer meshRenderer)
	{
		if (!_playerData.TryGetValue(meshRenderer.GetInstanceID(), out var value) || !value.Enabled || UnityEngine.Camera.main == null)
		{
			return;
		}
		bool flag = (meshRenderer.gameObject.transform.position - UnityEngine.Camera.main.transform.position).sqrMagnitude * MilMo_Lod.GlobalLodFactor > 64f;
		DeactivateAll(value);
		if (value.HideHair)
		{
			return;
		}
		if (value.HaveHat && value.HatHair != null)
		{
			if (flag && value.HatHairLod != null)
			{
				value.HatHairLod.SetActive(value: true);
			}
			else
			{
				value.HatHair.SetActive(value: true);
			}
		}
		else if (value.Hair != null)
		{
			if (flag && value.LOD != null)
			{
				value.LOD.SetActive(value: true);
			}
			else
			{
				value.Hair.SetActive(value: true);
			}
		}
	}

	private static void DeactivateAll(PlayerData data)
	{
		if (data.Hair != null)
		{
			data.Hair.SetActive(value: false);
		}
		if (data.LOD != null)
		{
			data.LOD.SetActive(value: false);
		}
		if (data.HatHair != null)
		{
			data.HatHair.SetActive(value: false);
		}
		if (data.HatHairLod != null)
		{
			data.HatHairLod.SetActive(value: false);
		}
	}

	public override void CreateAddonTexture(SkinnedMeshRenderer renderer, IDictionary<string, int> colorIndices)
	{
		base.CreateAddonTexture(renderer, colorIndices);
		if (_playerData.TryGetValue(renderer.GetInstanceID(), out var value))
		{
			UpdateHatsAndLods(value);
		}
	}

	private static void UpdateHatsAndLods(PlayerData data)
	{
		if (data == null || data.Hair == null)
		{
			return;
		}
		Renderer[] componentsInChildren = data.Hair.gameObject.transform.parent.GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (Renderer renderer in componentsInChildren)
		{
			if (renderer.gameObject.name.Equals(data.Hair.gameObject.name + "_lod", StringComparison.InvariantCultureIgnoreCase))
			{
				data.LOD = renderer.gameObject;
				data.LOD.GetComponent<Renderer>().material = data.Hair.gameObject.transform.GetComponent<Renderer>().material;
				data.LOD.SetActive(value: false);
			}
			else if (renderer.gameObject.name.Equals(data.Hair.gameObject.name + "_hat", StringComparison.InvariantCultureIgnoreCase))
			{
				data.HatHair = renderer.gameObject;
				data.HatHair.GetComponent<Renderer>().material = data.Hair.gameObject.transform.GetComponent<Renderer>().material;
				data.HatHair.SetActive(value: false);
				data.HatHair.transform.SetParent(data.Hair.gameObject.transform.parent);
			}
			else if (renderer.gameObject.name.Equals(data.Hair.gameObject.name + "_hatLod", StringComparison.InvariantCultureIgnoreCase))
			{
				data.HatHairLod = renderer.gameObject;
				data.HatHairLod.GetComponent<Renderer>().material = data.Hair.gameObject.transform.GetComponent<Renderer>().material;
				data.HatHairLod.SetActive(value: false);
				data.HatHairLod.transform.SetParent(data.Hair.gameObject.transform.parent);
			}
		}
	}

	private void SetupHatsAndLods(UnityEngine.Object renderer)
	{
		MilMo_VisualRep value;
		PlayerData value2;
		if (renderer == null)
		{
			Debug.LogWarning("Calling SetupHatsAndLods on addon in " + BodyPackPath + " with null renderer");
		}
		else if (!PlayerAddons.TryGetValue(renderer.GetInstanceID(), out value))
		{
			Debug.LogWarning("Trying to load content for hair addon without valid visual rep [" + VisualRepPath + "]");
		}
		else if (!_playerData.TryGetValue(renderer.GetInstanceID(), out value2))
		{
			value2 = new PlayerData
			{
				Hair = value.Renderer.gameObject
			};
			Renderer[] componentsInChildren = value2.Hair.gameObject.transform.parent.GetComponentsInChildren<Renderer>(includeInactive: true);
			Renderer renderer2 = componentsInChildren.FirstOrDefault((Renderer x) => x.name == "Hat");
			if (renderer2 != null)
			{
				renderer2.gameObject.name = value2.Hair.gameObject.name + "_hat";
			}
			Renderer renderer3 = componentsInChildren.FirstOrDefault((Renderer x) => x.name == "HLod1");
			if (renderer3 != null)
			{
				renderer3.gameObject.name = value2.Hair.gameObject.name + "_hatLod";
			}
			Renderer renderer4 = componentsInChildren.FirstOrDefault((Renderer x) => x.name == "Lod1");
			if (renderer4 != null)
			{
				renderer4.gameObject.name = value2.Hair.gameObject.name + "_lod";
			}
			UpdateHatsAndLods(value2);
			_playerData.Add(renderer.GetInstanceID(), value2);
		}
	}
}
