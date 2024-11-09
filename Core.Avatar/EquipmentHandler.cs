using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Avatar;

public class EquipmentHandler : MonoBehaviour
{
	[Serializable]
	public struct EquipmentPost
	{
		public Equipment equipment;

		public bool equipped;
	}

	[SerializeField]
	private SkinnedMeshRenderer skinnedMeshRenderer;

	[SerializeField]
	private List<EquipmentPost> equipment = new List<EquipmentPost>();

	[SerializeField]
	private PantsEquipment pants;

	private readonly List<EquipmentPost> _equipped = new List<EquipmentPost>();

	private void OnValidate()
	{
		RefreshEquipment();
	}

	public void RefreshEquipment()
	{
		Debug.Log("RefreshEquipment");
		foreach (EquipmentPost item in equipment)
		{
			Debug.Log(item.equipped + ":" + item.equipment?.ToString() + ":" + _equipped.ToList().Contains(item));
			if (item.equipped)
			{
				Equip(item);
			}
			if (!item.equipped)
			{
				UnEquip(item);
			}
		}
	}

	private void UnEquip(EquipmentPost equipmentPost)
	{
		Debug.Log("UnEquip");
		_equipped.Remove(equipmentPost);
		equipmentPost.equipment.Detach();
	}

	private void Equip(EquipmentPost equipmentPost)
	{
		Debug.Log("Equip");
		_equipped.Add(equipmentPost);
		equipmentPost.equipment.Attach(this);
	}

	public Transform[] GetBones()
	{
		if (!(skinnedMeshRenderer != null))
		{
			return new Transform[0];
		}
		return skinnedMeshRenderer.bones;
	}

	public Transform GetRootBone()
	{
		if (!(skinnedMeshRenderer != null))
		{
			return null;
		}
		return skinnedMeshRenderer.rootBone;
	}
}
