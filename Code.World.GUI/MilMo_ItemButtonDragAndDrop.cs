using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.Input;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.World.Inventory;
using Code.World.Player;
using Code.World.Player.Skills;
using UnityEngine;

namespace Code.World.GUI;

public sealed class MilMo_ItemButtonDragAndDrop : MilMo_Window
{
	private readonly MilMo_Item _mItem;

	private readonly MilMo_Skill _mSkill;

	private readonly long _mItemId;

	public MilMo_ItemButtonDragAndDrop(MilMo_ItemButton buttonToMove)
		: base(MilMo_GlobalUI.GetSystemUI)
	{
		if (buttonToMove.Skill == null)
		{
			_mItem = buttonToMove.Item;
			MilMo_InventoryEntry entry = MilMo_Player.Instance.Inventory.GetEntry(_mItem.Template.Identifier);
			if (entry == null)
			{
				Debug.LogWarning("Error. Item was null.");
			}
			if (entry != null)
			{
				_mItemId = entry.Id;
			}
		}
		else
		{
			_mSkill = buttonToMove.Skill;
		}
		UseParentAlpha = false;
		FadeToDefaultColor = false;
		base.Text = MilMo_LocString.Empty;
		AllowPointerFocus = true;
		SpawnScale = new Vector2(64f, 64f);
		TargetScale = SpawnScale;
		MilMo_Button milMo_Button = new MilMo_Button(UI);
		milMo_Button.SetScale(64f, 64f);
		milMo_Button.SetAlpha(0f);
		milMo_Button.SetPosition(0f, 0f);
		milMo_Button.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Button.SetAllTextures(buttonToMove.Icon.Texture);
		milMo_Button.FadeToDefaultColor = false;
		milMo_Button.FadeSpeed = 0.1f;
		milMo_Button.UseParentAlpha = false;
		milMo_Button.AllowPointerFocus = true;
		milMo_Button.Function = Click;
		AddChild(milMo_Button);
		GoToNow(MilMo_Pointer.Position);
		TargetPos = Pos;
		UI.AddChild(this);
		Open();
		milMo_Button.AlphaTo(0.9f);
	}

	public override void Step()
	{
		SetAlpha(0f);
		SetPosition(MilMo_Pointer.Position.x - 32f, MilMo_Pointer.Position.y - 32f);
		TargetPos = Pos;
		base.Step();
		BringToFront();
	}

	private void Click(object o)
	{
		if (MilMo_World.HudHandler != null)
		{
			_ = MilMo_World.HudHandler.gameObject.activeSelf;
		}
		Close(null);
	}
}
