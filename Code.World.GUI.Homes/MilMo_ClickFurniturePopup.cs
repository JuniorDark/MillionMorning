using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Items.Home.HomeEquipment.ColoredHomeEquipment.Furniture;
using Code.Core.Sound;
using Code.World.Home;
using UnityEngine;

namespace Code.World.GUI.Homes;

public class MilMo_ClickFurniturePopup : MilMo_Widget
{
	private readonly MilMo_Button m_RotateClockwiseWidget;

	private readonly MilMo_Button m_RotateAntiClockwiseWidget;

	private readonly MilMo_HomeFurniture m_FurnitureToSurround;

	public MilMo_HomeFurniture CurrentFurniture => m_FurnitureToSurround;

	public MilMo_ClickFurniturePopup(MilMo_UserInterface ui, MilMo_EventSystem.MilMo_Callback rotateClockwiseFunction, MilMo_EventSystem.MilMo_Callback rotateAntiClockwiseFunction, MilMo_HomeFurniture item)
		: base(ui)
	{
		SetScale(1f, 1f);
		SetAlignment(MilMo_GUI.Align.CenterCenter);
		AllowPointerFocus = false;
		m_RotateClockwiseWidget = CreateButton("Batch01/Textures/Homes/RotateRightGreen", MilMo_GUI.Align.CenterRight, delegate
		{
			rotateClockwiseFunction();
		});
		m_RotateAntiClockwiseWidget = CreateButton("Batch01/Textures/Homes/RotateLeftGreen", MilMo_GUI.Align.CenterLeft, delegate
		{
			rotateAntiClockwiseFunction();
		});
		if (!(item.Item is MilMo_WallFurniture))
		{
			UI.AddChild(m_RotateClockwiseWidget);
			UI.AddChild(m_RotateAntiClockwiseWidget);
		}
		m_FurnitureToSurround = item;
		UI.AddChild(this);
		Enabled = true;
		m_RotateClockwiseWidget.Enabled = true;
		m_RotateAntiClockwiseWidget.Enabled = true;
	}

	private MilMo_Button CreateButton(string texture, MilMo_GUI.Align align, MilMo_Button.ButtonFunc function)
	{
		MilMo_Button milMo_Button = new MilMo_Button(UI);
		milMo_Button.SetAllTextures(texture);
		milMo_Button.SetScale(32f, 32f);
		milMo_Button.SetAlignment(align);
		milMo_Button.AllowPointerFocus = true;
		milMo_Button.SetScalePull(0.05f, 0.05f);
		milMo_Button.SetScaleDrag(0.6f, 0.7f);
		milMo_Button.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		milMo_Button.SetExtraScaleOnHover(5f, 5f);
		milMo_Button.Function = function;
		milMo_Button.SetHoverSound(new MilMo_AudioClip("Content/Sounds/Batch01/CharBuilder/CharBuilderTick"));
		return milMo_Button;
	}

	public override void Step()
	{
		if (m_FurnitureToSurround != null)
		{
			base.Step();
			if (m_FurnitureToSurround.Item is MilMo_FloorFurniture)
			{
				MilMo_FloorFurniture milMo_FloorFurniture = m_FurnitureToSurround.Item as MilMo_FloorFurniture;
				Vector3 position = m_FurnitureToSurround.Position;
				Vector2 vector = new Vector2(position.x - milMo_FloorFurniture.Grid.PivotCol * 1f + (float)milMo_FloorFurniture.Grid.Columns * 0.5f * 1f, position.z + milMo_FloorFurniture.Grid.PivotRow * 1f - (float)milMo_FloorFurniture.Grid.Rows * 0.5f * 1f);
				position.x = vector.x;
				position.z = vector.y;
				Vector3 vector2 = MilMo_Global.MainCamera.WorldToScreenPoint(position);
				SetPosition(vector2.x, (float)Screen.height - vector2.y);
				m_RotateAntiClockwiseWidget.SetPosition(Pos.x + 20f, Pos.y);
				m_RotateClockwiseWidget.SetPosition(Pos.x - 20f, Pos.y);
			}
			else if (m_FurnitureToSurround.Item is MilMo_AttachableFurniture)
			{
				Vector3 position2 = m_FurnitureToSurround.Position;
				Vector2 vector3 = new Vector2(position2.x - 0.5f + 0.5f, position2.z + 0.5f - 0.5f);
				position2.x = vector3.x;
				position2.z = vector3.y;
				Vector3 vector4 = MilMo_Global.MainCamera.WorldToScreenPoint(position2);
				SetPosition(vector4.x, (float)Screen.height - vector4.y);
				m_RotateAntiClockwiseWidget.SetPosition(Pos.x + 20f, Pos.y);
				m_RotateClockwiseWidget.SetPosition(Pos.x - 20f, Pos.y);
			}
			else if (m_FurnitureToSurround.Item is MilMo_WallFurniture)
			{
				Vector3 vector5 = MilMo_Global.MainCamera.WorldToScreenPoint(m_FurnitureToSurround.Position);
				SetPosition(vector5.x, (float)Screen.height - vector5.y);
			}
		}
	}

	public bool MouseOverAll()
	{
		if (m_RotateClockwiseWidget.Hover() || m_RotateAntiClockwiseWidget.Hover())
		{
			return true;
		}
		return false;
	}

	public override void Draw()
	{
		if (m_FurnitureToSurround != null)
		{
			base.Draw();
		}
	}

	public void Close()
	{
		UI.RemoveChild(this);
		if (!(m_FurnitureToSurround.Item is MilMo_WallFurniture))
		{
			UI.RemoveChild(m_RotateClockwiseWidget);
			UI.RemoveChild(m_RotateAntiClockwiseWidget);
		}
	}
}
