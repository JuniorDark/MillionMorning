using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.World.Home;
using UnityEngine;

namespace Code.World.GUI;

public class MilMoObjectMarker : MilMo_Button
{
	public const int NPC = 0;

	public const int PICKUP = 1;

	public const int CHATROOM = 2;

	public const int SITPOINT = 3;

	public const int LOOKAT_PRIO_DEBUG = 4;

	public const int GAMEPLAYOBJECT = 5;

	public const int NPCARROW = 6;

	public const int UNLOOTABLEPICKUP = 7;

	public const int HIDEALL = 0;

	public const int HIDETEXTFIELD = 1;

	private readonly Vector3 _offset;

	public Vector2 DistanceScale = Vector2.zero;

	public float Distance;

	public float MaxDistance;

	public float MinDistance;

	private MilMo_LocString _pickupTextBackup;

	private readonly GameObject _target;

	private readonly MilMo_TimerEvent _cancelShowText;

	private readonly MilMo_TimerEvent _cancelShowTextDropShadow;

	private MilMo_TimerEvent _scheduleShowPickupText1;

	private MilMo_TimerEvent _scheduleShowPickupText2;

	private MilMo_TimerEvent _scheduleShowPickupText3;

	private MilMo_TimerEvent _scheduleShowPickupText4;

	private readonly AudioSourceWrapper _audioSource;

	private readonly MilMo_AudioClip _showClip;

	private readonly MilMo_AudioClip _hideClip;

	private bool _didPlaySound;

	public readonly MilMo_Widget TextField;

	private readonly float _showTextDelay = 0.1f;

	private readonly float _birthTime;

	public MilMoObjectMarker(MilMo_UserInterface ui, GameObject obj, Vector3 offset, int type, int id, MilMo_LocString message)
		: base(ui)
	{
		Identifier = "ObjectMarker";
		_target = obj;
		_offset = offset;
		Info2 = type;
		Info = id;
		if (type == 4)
		{
			_showTextDelay = 0f;
		}
		SetAllTextures("Batch01/Textures/World/NPCMarkerSilver");
		SetScale(64f, 0f);
		ScaleTo(32f, 64f);
		SetScalePull(0.2f, 0.2f);
		SetScaleDrag(0.5f, 0.8f);
		SetDefaultColor(1f, 1f, 1f, 1f);
		SetAlignment(MilMo_GUI.Align.BottomCenter);
		FadeToDefaultColor = false;
		SetFadeOutSpeed(0.2f);
		TextField = new MilMo_Widget(UI);
		TextField.GoToNow(100f, 100f);
		TextField.ScaleNow(200f, 100f);
		TextField.SetAlignment(MilMo_GUI.Align.BottomCenter);
		TextField.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		TextField.SetWordWrap(w: false);
		TextField.SetTexture("Batch01/Textures/Core/Invisible");
		TextField.SetFont(MilMo_GUI.Font.EborgSmall);
		TextField.SetDefaultTextColor(Color.white);
		TextField.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		TextOutline = new Vector2(0f, 0f);
		TextField.SetTextDropShadowPos(0f, 0f);
		TextField.SetDefaultColor(1f, 1f, 1f, 0f);
		TextField.FadeToDefaultColor = false;
		TextField.SetFadeSpeed(0.08f);
		TextField.SetText(message);
		TextField.AllowPointerFocus = false;
		UI.AddChild(TextField);
		if (type == 6)
		{
			SetTexture("Batch01/Textures/Generic/DownArrow");
			SetScale(50f, 50f);
			AllowPointerFocus = false;
			SetPosPull(0.09f, 0.09f);
			SetPosDrag(0.45f, 0.45f);
			SetFadeOutSpeed(0.05f);
			SetMoveType(MilMo_Mover.UpdateFunc.Sinus);
			PosMover.SinRate = new Vector2(0f, 4f);
			PosMover.SinAmp = new Vector2(0f, -20f);
			SetScaleType(MilMo_Mover.UpdateFunc.Sinus);
			ScaleMover.SinRate = new Vector2(4f, 4f);
			ScaleMover.SinAmp = new Vector2(3f, -4f);
			SetDefaultColor(1f, 1f, 1f, 1f);
			TextField.SetEnabled(e: false);
		}
		if (type == 1)
		{
			ScheduleShowPickupText();
		}
		CustomFunction = Hide;
		int num = 0;
		Args = num;
		Vector3 vector = MilMo_Global.Camera.WorldToScreenPoint(_target.transform.position + _offset);
		GoToNow(vector.x / base.Res.x, ((float)MilMo_Global.Camera.pixelHeight - vector.y) / base.Res.y);
		_cancelShowText = MilMo_EventSystem.At(_showTextDelay, OnShowText);
		_cancelShowTextDropShadow = MilMo_EventSystem.At(_showTextDelay + 0.25f, OnShowTextDropShadow);
		if (type != 6 && MilMo_Home.CurrentHome == null)
		{
			_audioSource = MilMo_Global.AudioListener.AddComponent<AudioSourceWrapper>();
			_showClip = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/ObjectMarkerShow");
			_hideClip = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/ObjectMarkerHide");
		}
		_birthTime = Time.time;
	}

	protected void Hide(object o)
	{
		int num = 0;
		if (o != null)
		{
			num = (int)o;
		}
		if (Info2 == 6)
		{
			AlphaTo(0f);
		}
		else
		{
			switch (num)
			{
			case 0:
				AlphaTo(0f);
				SetScaleDrag(0.5f, 0.5f);
				ScaleTo(0f, 0f);
				ScaleMover.Arrive = Remove;
				OnHideText();
				break;
			case 1:
				OnHideText();
				break;
			}
		}
		if (_scheduleShowPickupText1 != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_scheduleShowPickupText1);
		}
		if (_scheduleShowPickupText2 != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_scheduleShowPickupText2);
		}
		if (_scheduleShowPickupText3 != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_scheduleShowPickupText3);
		}
		if (_scheduleShowPickupText4 != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_scheduleShowPickupText4);
		}
		int info = Info2;
		if ((info == 1 || info == 7) && !(_audioSource == null) && !(_hideClip.AudioClip == null))
		{
			_audioSource.Clip = _hideClip.AudioClip;
			_audioSource.Play();
		}
	}

	public virtual void Show()
	{
		if (Info2 == 6)
		{
			AlphaTo(1f);
		}
	}

	public override void Step()
	{
		int info = Info2;
		if ((info == 1 || info == 7) && Time.time < _birthTime + 1f)
		{
			return;
		}
		base.Step();
		if (_audioSource != null && _showClip.AudioClip != null && !_didPlaySound)
		{
			_audioSource.Clip = _showClip.AudioClip;
			_audioSource.Loop = false;
			_audioSource.Play();
			_didPlaySound = true;
		}
		if (Info2 == 6)
		{
			float a = (MaxDistance - Distance) / (MaxDistance / 50f);
			a = Mathf.Min(a, 50f);
			a = Mathf.Max(a, 25f);
			DistanceScale = new Vector2(a * 0.96f, a);
			if (Distance < MaxDistance && Distance > MinDistance)
			{
				Show();
			}
			else
			{
				Hide(null);
			}
		}
	}

	private void OnShowTextDropShadow()
	{
		TextOutline = new Vector2(1f, 1f);
		ScaleMover.ClearArriveFunc();
		if (Info2 != 7)
		{
			TextField.SetTextDropShadowPos(2f, 2f);
		}
	}

	private void OnShowText()
	{
		if (Info2 != 7)
		{
			TextOutline = new Vector2(1f, 1f);
			TextField.SetTextDropShadowPos(2f, 2f);
		}
		TextField.AlphaTo(TextField.DefaultTextColor.a);
	}

	private void OnHideText()
	{
		if (_cancelShowText != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_cancelShowText);
		}
		if (_cancelShowTextDropShadow != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_cancelShowTextDropShadow);
		}
		TextField.AlphaTo(0f);
		TextField.SetTextDropShadowPos(0f, 0f);
		TextOutline = new Vector2(0f, 0f);
	}

	private void ScheduleShowPickupText()
	{
		if (_scheduleShowPickupText1 != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_scheduleShowPickupText1);
		}
		_scheduleShowPickupText1 = MilMo_EventSystem.At(1.5f, delegate
		{
			OnHideText();
			if (_scheduleShowPickupText2 != null)
			{
				MilMo_EventSystem.RemoveTimerEvent(_scheduleShowPickupText2);
			}
			_scheduleShowPickupText2 = MilMo_EventSystem.At(0.4f, delegate
			{
				OnShowText();
				_pickupTextBackup = TextField.Text;
				TextField.SetText(MilMo_Localization.GetLocString("Interact_PickUp"));
				if (_scheduleShowPickupText3 != null)
				{
					MilMo_EventSystem.RemoveTimerEvent(_scheduleShowPickupText3);
				}
				_scheduleShowPickupText3 = MilMo_EventSystem.At(2.5f, delegate
				{
					OnHideText();
					if (_scheduleShowPickupText4 != null)
					{
						MilMo_EventSystem.RemoveTimerEvent(_scheduleShowPickupText4);
					}
					_scheduleShowPickupText4 = MilMo_EventSystem.At(0.4f, delegate
					{
						OnShowText();
						TextField.SetText(_pickupTextBackup);
						ScheduleShowPickupText();
					});
				});
			});
		});
	}

	public override void Draw()
	{
		if (!Enabled)
		{
			return;
		}
		if (_target == null)
		{
			Remove();
			return;
		}
		Vector3 vector = MilMo_Global.Camera.WorldToScreenPoint(_target.transform.position + _offset);
		if (!(vector.z > 0f))
		{
			return;
		}
		if (Info2 != 0)
		{
			float x = vector.x / base.Res.x;
			float num = ((float)MilMo_Global.Camera.pixelHeight - vector.y) / base.Res.y;
			if (Info2 == 6)
			{
				float num2 = Mathf.Max(0.25f, (MaxDistance - Distance) / MaxDistance);
				GoToNow(x, num - (20f + PosMover.SinVel.y) * num2);
				ScaleNow(DistanceScale.x, DistanceScale.y);
				PosMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Sinus);
				ScaleMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Sinus);
			}
			else
			{
				GoToNow(x, num);
			}
			TextField.GoToNow(x, num - 32f);
		}
		base.Draw();
	}

	public void Remove()
	{
		UI.RemoveChild(this);
		UI.RemoveChild(TextField);
		if ((bool)_audioSource)
		{
			Object.Destroy(_audioSource);
		}
		_showClip?.Destroy();
	}
}
