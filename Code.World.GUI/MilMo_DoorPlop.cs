using Code.Core.Global;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using UnityEngine;

namespace Code.World.GUI;

public class MilMo_DoorPlop : MilMo_Widget
{
	private const float SQR_LOD_DISTANCE = 1600f;

	private static readonly Color FriendInsideColor = new Color(0f, 1f, 0f, 1f);

	private static readonly Vector2 NumberOffset = new Vector2(0f, 5f);

	private readonly MilMo_Widget _number;

	private readonly Vector3 _worldPosition;

	private bool _friendInside;

	private int _playersInsideCount;

	private readonly string _room;

	public bool FriendInside
	{
		set
		{
			_friendInside = value;
			if (_friendInside)
			{
				TextColorTo(FriendInsideColor);
				_number.TextColorTo(FriendInsideColor);
			}
			else
			{
				TextColorTo(DefaultColor);
				_number.TextColorTo(DefaultColor);
			}
		}
	}

	public int PlayersInsideCount
	{
		set
		{
			_playersInsideCount = value;
			_number.SetTextNoLocalization(_playersInsideCount.ToString());
		}
	}

	public MilMo_DoorPlop(MilMo_UserInterface ui, Vector3 worldPosition, string room)
		: base(ui)
	{
		Identifier = "DoorPlop";
		_worldPosition = worldPosition;
		_room = room;
		SetAlignment(MilMo_GUI.Align.BottomCenter);
		FadeToDefaultColor = false;
		FadeToDefaultTextColor = false;
		SetFadeSpeed(0.05f);
		SetDefaultTextColor(DefaultColor);
		ScaleNow(30f, 20f);
		SetTextureInvisible();
		SetFont(MilMo_GUI.Font.EborgLarge);
		SetTextDropShadowPos(2f, 2f);
		SetTextAlignment(MilMo_GUI.Align.BottomCenter);
		SetTextNoLocalization(".");
		ui.AddChild(this);
		_number = new MilMo_Widget(ui);
		_number.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_number.FadeToDefaultColor = false;
		_number.FadeToDefaultTextColor = false;
		_number.SetFadeSpeed(0.05f);
		_number.SetDefaultTextColor(DefaultColor);
		_number.ScaleNow(30f, 30f);
		_number.SetTextureInvisible();
		_number.SetFont(MilMo_GUI.Font.EborgSmall);
		_number.SetTextDropShadowPos(2f, 2f);
		_number.SetTextAlignment(MilMo_GUI.Align.BottomCenter);
		_number.SetPosition(NumberOffset);
		_number.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		AddChild(_number);
	}

	public override void Draw()
	{
		if (!Enabled || _playersInsideCount == 0 || _room != MilMo_UserInterface.CurrentRoom)
		{
			return;
		}
		Vector3 vector = MilMo_Global.Camera.WorldToScreenPoint(_worldPosition);
		if (vector.z > 0f)
		{
			GoToNow(vector.x / base.Res.x, ((float)MilMo_Global.Camera.pixelHeight - vector.y) / base.Res.y);
			if ((MilMo_Global.Camera.transform.position - _worldPosition).sqrMagnitude > 1600f || _playersInsideCount <= 1)
			{
				_number.SetEnabled(e: false);
			}
			else
			{
				_number.SetEnabled(e: true);
			}
			base.Draw();
		}
	}

	public void Remove()
	{
		RemoveChild(_number);
		UI.RemoveChild(this);
	}
}
