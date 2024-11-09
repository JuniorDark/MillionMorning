using System;
using System.Collections.Generic;
using Code.Core.Global;
using Code.Core.Items;
using Code.Core.Utility;
using Code.Core.Visual;
using Code.World.Home;
using UnityEngine;

namespace Code.World.CharacterShop;

public class MilMo_ShopRoom : MilMo_Item
{
	public delegate void ShopRoomLoaded(GameObject gameObject);

	private MilMo_ActiveRoom m_LoadedRoom;

	private readonly List<MilMo_VisualRep> m_WallExtensions = new List<MilMo_VisualRep>();

	public new MilMo_ShopRoomTemplate Template => (MilMo_ShopRoomTemplate)base.Template;

	public GameObject GameObject { get; private set; }

	public override bool IsWearable()
	{
		return false;
	}

	public override bool IsWieldable()
	{
		return false;
	}

	public MilMo_ShopRoom(MilMo_ShopRoomTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public void AsyncLoadContent(ShopRoomLoaded callback)
	{
		MilMo_ShopRoomLoader.AsyncLoad(Template, delegate(bool success, MilMo_ActiveRoom room)
		{
			if (!success || room == null)
			{
				callback(null);
			}
			else
			{
				m_LoadedRoom = room;
				FinishLoading();
				callback(GameObject);
			}
		});
	}

	private void FinishLoading()
	{
		GameObject = new GameObject(Template.Identifier);
		GameObject.transform.position = MilMo_Utility.GetAncestor(m_LoadedRoom.GameObject).transform.position + new Vector3(m_LoadedRoom.Grid.Columns, 0f, -m_LoadedRoom.Grid.Rows) * 0.5f * 1f;
		MilMo_Utility.GetAncestor(m_LoadedRoom.GameObject).transform.parent = GameObject.transform;
		foreach (MilMo_HomeFurniture value in m_LoadedRoom.Furniture.Values)
		{
			value.UpdateRotation();
			MilMo_Utility.GetAncestor(value.GameObject).transform.parent = GameObject.transform;
		}
		foreach (MilMo_VisualRep wallExtension in m_WallExtensions)
		{
			wallExtension.GameObject.transform.parent = GameObject.transform;
		}
		int num = Mathf.Max(m_LoadedRoom.Grid.Rows, m_LoadedRoom.Grid.Columns);
		float num2 = (float)(0.7178239263 * Math.Pow(num, -0.6893788896));
		GameObject.transform.localScale = new Vector3(num2, num2, num2);
	}

	public void Unload()
	{
		if (m_LoadedRoom != null)
		{
			m_LoadedRoom.Unload();
		}
		if (GameObject != null)
		{
			MilMo_Global.Destroy(GameObject);
		}
		m_LoadedRoom = null;
		GameObject = null;
		foreach (MilMo_VisualRep wallExtension in m_WallExtensions)
		{
			MilMo_VisualRepContainer.RemoveFromUpdate(wallExtension);
			MilMo_VisualRepContainer.DestroyVisualRep(wallExtension);
		}
		m_WallExtensions.Clear();
	}

	public void Update()
	{
		if (m_LoadedRoom != null)
		{
			if (m_LoadedRoom.VisualRep != null)
			{
				m_LoadedRoom.VisualRep.Update();
			}
			foreach (MilMo_HomeFurniture value in m_LoadedRoom.Furniture.Values)
			{
				value.UpdateFade();
			}
		}
		foreach (MilMo_VisualRep wallExtension in m_WallExtensions)
		{
			wallExtension.Update();
		}
	}
}
