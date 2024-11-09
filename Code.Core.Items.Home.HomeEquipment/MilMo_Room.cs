using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment;
using UI.Elements.Slot;
using UnityEngine;

namespace Code.Core.Items.Home.HomeEquipment;

public sealed class MilMo_Room : MilMo_HomeEquipment, IEntryItem
{
	public const string ROOM_ENTRANCE_MODIFIER_KEY = "RoomEntrance";

	private List<MilMo_HomeEquipment> _mEquipment;

	public MilMo_Furniture Entrance { get; private set; }

	public long WantedEntranceId
	{
		get
		{
			if (!base.Modifiers.ContainsKey("RoomEntrance"))
			{
				return -1L;
			}
			return long.Parse(base.Modifiers["RoomEntrance"]);
		}
	}

	public bool NeedsEntrance
	{
		get
		{
			if (WantedEntranceId != -1)
			{
				return Entrance == null;
			}
			return false;
		}
	}

	public MilMo_Room(MilMo_ItemTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public new Texture2D GetItemIcon()
	{
		return Entrance?.ItemIcon;
	}

	public new async Task<Texture2D> AsyncGetIcon()
	{
		TaskCompletionSource<Texture2D> tcs = new TaskCompletionSource<Texture2D>();
		if (Entrance == null)
		{
			return null;
		}
		Entrance.AsyncGetIcon(delegate(Texture2D texture)
		{
			if (!texture)
			{
				Debug.LogWarning("could not load icon for: " + base.Identifier);
			}
			tcs.TrySetResult(texture);
		});
		return await tcs.Task;
	}

	public void SetEntrance(MilMo_Furniture entrance)
	{
		if (!entrance.Template.IsDoor)
		{
			throw new ArgumentException("Can only set room entrance to a door");
		}
		if (!base.Modifiers.ContainsKey("RoomEntrance"))
		{
			throw new InvalidOperationException("Trying to set entrance for a room that shouldn't have one.");
		}
		if (entrance.Id != long.Parse(base.Modifiers["RoomEntrance"]))
		{
			throw new ArgumentException("Trying to set room entrance to furniture with id " + entrance.Id + " when id " + base.Modifiers["RoomEntrance"] + " was expected.");
		}
		Entrance = entrance;
	}
}
