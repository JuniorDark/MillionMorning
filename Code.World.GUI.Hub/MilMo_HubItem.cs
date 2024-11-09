using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Input;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.Core.Visual;
using Code.World.Level.LevelInfo;
using Core;
using Core.Input;
using UnityEngine;

namespace Code.World.GUI.Hub;

public sealed class MilMo_HubItem
{
	private readonly MilMo_HubItemFunctionality _function;

	private Vector2 _resolution = new Vector2(1f, 1f);

	private MilMo_VisualRep _visualRep;

	private readonly GameObject _mesh;

	private bool _isScalingToOrig;

	private bool _isScalingToMo;

	private readonly MilMo_PlayerCountTag _chatRoomsTag;

	private readonly MilMo_ObjectMover _scaleMover;

	private readonly MilMo_UserInterface _ui;

	private readonly Vector3 _position;

	private readonly MilMo_HubInfoData.MilMo_HubItemInfoData _data;

	private readonly MilMo_Widget _text;

	private string _stateType = "";

	private bool _isPaused = true;

	private MilMo_HubBubble _bubble;

	private bool _isWobbling;

	private float _lastTimeRequestedPlayerCounts;

	private static AudioSourceWrapper _soundFx;

	private AudioClip _mouseOverSound;

	private static MilMo_AudioClip _openClip;

	private bool _havePlayedOpenSound;

	private MilMo_HubInfoWindow _infoWindow;

	public string Identifier { get; }

	private bool IsEnabled { get; set; }

	public bool StateIsActive { get; private set; }

	public Vector2 ScreenPosition
	{
		get
		{
			Vector2 vector = MilMo_Global.MainCamera.WorldToScreenPoint(_position);
			return new Vector2(vector.x, (float)Screen.height - vector.y);
		}
	}

	public MilMo_HubItem(MilMo_VisualRep rep, MilMo_HubInfoData.MilMo_HubItemInfoData data, Vector3 position)
	{
		_ui = MilMo_Hub.Instance.UI;
		_data = data;
		_visualRep = rep;
		Identifier = _data.Identifier;
		_soundFx = MilMo_Global.AudioListener.AddComponent<AudioSourceWrapper>();
		LoadMouseOverSound();
		if (_openClip == null)
		{
			_openClip = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/Generic/Info");
		}
		GameObject childGameObject = MilMo_Utility.GetChildGameObject(_visualRep.GameObject, "Col", includeInactive: true);
		if (childGameObject != null)
		{
			childGameObject.transform.parent = _visualRep.GameObject.transform;
		}
		_mesh = MilMo_Utility.GetChildGameObject(_visualRep.GameObject, "Mesh", includeInactive: true);
		_position = position;
		Vector2 vector = MilMo_Global.MainCamera.WorldToScreenPoint(_position);
		if (Identifier.ToUpper() == "CHATROOMS" || Identifier.ToUpper() == "COLOSSEUM")
		{
			MilMo_EventSystem.Listen("level_player_counts_info", GotLevelPlayerCounts).Repeating = true;
			_chatRoomsTag = new MilMo_PlayerCountTag(_ui)
			{
				FixedRes = true
			};
			_chatRoomsTag.SetAlignment(MilMo_GUI.Align.BottomCenter);
			_chatRoomsTag.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
			_chatRoomsTag.SetTexture("Batch01/Textures/WorldMap/SplineDot");
			_chatRoomsTag.SetFont(MilMo_GUI.Font.EborgSmall);
			_chatRoomsTag.SetDefaultColor(0f, 0f, 0f, 0.8f);
			_chatRoomsTag.SetFontScale(0.8f);
			_chatRoomsTag.Enabled = false;
			_chatRoomsTag.SetDefaultTextColor(1f, 1f, 1f, 1f);
			_chatRoomsTag.SetScale(28f, 28f);
			_chatRoomsTag.SetPosition(new Vector2(vector.x, (float)Screen.height - vector.y));
			_ui.AddChild(_chatRoomsTag);
			List<string> list = new List<string>();
			foreach (MilMo_LevelInfoData chatRoom in MilMo_LevelInfo.GetChatRooms())
			{
				if (chatRoom.IsPvp)
				{
					if (!(Identifier.ToUpper() == "COLOSSEUM"))
					{
						continue;
					}
				}
				else if (!(Identifier.ToUpper() == "CHATROOMS"))
				{
					continue;
				}
				list.Add(chatRoom.World + ":" + chatRoom.Level);
			}
			Singleton<GameNetwork>.Instance.RequestLevelPlayerCounts(list);
		}
		_scaleMover = new MilMo_ObjectMover();
		_scaleMover.SetUpdateFunc(8);
		_scaleMover.SinRate = new Vector3(0f, 2.5f, 0f);
		_scaleMover.SinAmp = new Vector3(0f, 0.1055f, 0f);
		_scaleMover.Scale = Vector3.one;
		_scaleMover.Pos = Vector3.zero;
		_scaleMover.ScalePull = 0.05f;
		_scaleMover.ScaleDrag = 0.75f;
		_scaleMover.Pull = 0f;
		_scaleMover.Drag = 0f;
		_scaleMover.Vel = Vector3.zero;
		_visualRep.GameObject.transform.localScale = _data.Scale;
		_function = new MilMo_HubItemFunctionality(_ui);
		SetUpInfoWindow(_data.IconTexture, new Vector2(60f, 60f), _data.Description, _data.Text);
		_text = new MilMo_Widget(_ui);
		_text.SetScale(300f, 50f);
		_text.AllowPointerFocus = false;
		_text.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_text.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_text.SetText(_data.NoTitleInTown ? MilMo_LocString.Empty : data.Text);
		_text.SetFont(MilMo_GUI.Font.EborgLarge);
		_text.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		_text.SetTextOutline(1f, 1f);
		_text.TextOutlineColor = Color.black;
		_text.SetPosition(new Vector2(vector.x, (float)Screen.height - vector.y) + _data.TextOffset);
		_ui.AddChild(_text);
	}

	private async void LoadMouseOverSound()
	{
		_mouseOverSound = await MilMo_ResourceManager.Instance.LoadAudioAsync("Content/Sounds/Batch01/CharacterShop/SoftTick");
	}

	private void GotLevelPlayerCounts(object o)
	{
		if (!IsEnabled || !(o is ServerLevelPlayerCounts serverLevelPlayerCounts))
		{
			return;
		}
		int num = 0;
		foreach (LevelPlayerCount playerCount in serverLevelPlayerCounts.getPlayerCounts())
		{
			string fullLevelName = playerCount.GetWorld() + ":" + playerCount.GetLevel();
			if (MilMo_LevelInfo.IsPvp(fullLevelName) && Identifier.ToUpper() == "COLOSSEUM")
			{
				num += playerCount.GetPlayers();
			}
			else if (MilMo_LevelInfo.IsChatroom(fullLevelName) && Identifier.ToUpper() == "CHATROOMS")
			{
				num += playerCount.GetPlayers();
			}
		}
		_chatRoomsTag.Enabled = num > 1;
		_chatRoomsTag.SetText((num <= 1) ? MilMo_LocString.Empty : MilMo_Localization.GetNotLocalizedLocString(num.ToString()));
	}

	public void Enable()
	{
		if (IsEnabled)
		{
			return;
		}
		IsEnabled = true;
		_visualRep.Enable();
		if (_isWobbling)
		{
			UnpauseWobble();
		}
		MilMo_VisualRepContainer.AddForUpdate(_visualRep);
		if ((Identifier.ToUpper() != "CHATROOMS" && Identifier.ToUpper() != "COLOSSEUM") || (_lastTimeRequestedPlayerCounts != 0f && !(Time.time - _lastTimeRequestedPlayerCounts > 5f)))
		{
			return;
		}
		_lastTimeRequestedPlayerCounts = Time.time;
		List<string> list = new List<string>();
		foreach (MilMo_LevelInfoData chatRoom in MilMo_LevelInfo.GetChatRooms())
		{
			string text = Identifier.ToUpper();
			if (!(text == "CHATROOMS"))
			{
				if (text == "COLOSSEUM" && chatRoom.IsPvp)
				{
					list.Add(chatRoom.World + ":" + chatRoom.Level);
				}
			}
			else if (!chatRoom.IsPvp)
			{
				list.Add(chatRoom.World + ":" + chatRoom.Level);
			}
		}
		Singleton<GameNetwork>.Instance.RequestLevelPlayerCounts(list);
	}

	public void Disable()
	{
		if (IsEnabled)
		{
			IsEnabled = false;
			_visualRep.Disable();
			if (_isWobbling)
			{
				PauseWobble();
			}
			MilMo_VisualRepContainer.RemoveFromUpdate(_visualRep);
		}
	}

	public void Destroy()
	{
		if (_visualRep != null)
		{
			MilMo_VisualRepContainer.RemoveFromUpdate(_visualRep);
			MilMo_VisualRepContainer.DestroyVisualRep(_visualRep);
			_visualRep = null;
		}
	}

	private void MouseOver()
	{
		if (!_isScalingToMo)
		{
			if ((bool)_mouseOverSound)
			{
				_soundFx.Clip = _mouseOverSound;
				_soundFx.Loop = false;
				_soundFx.Play();
			}
			if (_isWobbling)
			{
				PauseWobble();
			}
			_isScalingToMo = true;
			_scaleMover.ScaleTo(_data.MouseOverScale);
			ShowInfoWindow();
		}
	}

	private void MouseLeave()
	{
		if (!_isScalingToOrig)
		{
			_isScalingToOrig = true;
			if (!_isWobbling)
			{
				_scaleMover.ScaleTo(Vector3.one);
			}
			HideInfoWindow();
			if (_isWobbling)
			{
				UnpauseWobble();
			}
		}
	}

	public void Update()
	{
		if (!IsEnabled)
		{
			return;
		}
		if (TestMouseOver())
		{
			_isScalingToOrig = false;
			MouseOver();
			if (MilMo_Hub.IsSafeToClick && MilMo_Pointer.LeftClick && MilMo_UserInterface.FinalMouseFocus == null)
			{
				if (_isWobbling)
				{
					MilMo_EventSystem.Instance.PostEvent("hubitem_state_off", Identifier);
				}
				_function.Run(Identifier);
			}
		}
		else
		{
			_isScalingToMo = false;
			MouseLeave();
		}
		_scaleMover.Update();
		if (!_isPaused)
		{
			_mesh.transform.localScale = Vector3.one + _scaleMover.Pos;
		}
		else
		{
			Vector3 localScale = new Vector3(1f + _scaleMover.Pos.x, _scaleMover.Scale.y, 1f + _scaleMover.Pos.z);
			_mesh.transform.localScale = localScale;
		}
		Vector2 vector = MilMo_Global.MainCamera.WorldToScreenPoint(_position);
		_text.SetPosition(new Vector2(vector.x, (float)Screen.height - vector.y) + new Vector2(_data.TextOffset.x * _resolution.x, _data.TextOffset.y * _resolution.y));
		MilMo_HubBubble bubble = _bubble;
		if (bubble != null && bubble.Enabled)
		{
			_bubble.SetPosition(new Vector2(vector.x + _data.StateIconOffset.x * _resolution.x, (float)Screen.height - vector.y + _data.StateIconOffset.y * _resolution.y));
		}
		_chatRoomsTag?.SetPosition(new Vector2(vector.x, (float)Screen.height - vector.y));
	}

	public void RefreshUI()
	{
		_resolution = new Vector2((float)Screen.width / 1024f, (float)Screen.height / 720f);
		float fontScale = 1f;
		if (_resolution.x < 1f || _resolution.y < 1f)
		{
			fontScale = ((_resolution.x <= _resolution.y) ? _resolution.x : _resolution.y);
		}
		_text.SetFontScale(fontScale);
	}

	private bool TestMouseOver()
	{
		if (MilMo_UserInterface.FinalMouseFocus != null)
		{
			return false;
		}
		if (!Physics.Raycast(MilMo_Global.MainCamera.ScreenPointToRay(InputSwitch.MousePosition), out var hitInfo, 1400f))
		{
			return false;
		}
		GameObject gameObject = hitInfo.collider.gameObject;
		if (!gameObject)
		{
			return false;
		}
		if (_visualRep.GameObject == MilMo_Utility.GetAncestor(gameObject))
		{
			return MilMo_UserInterfaceManager.MouseFocus == null;
		}
		return false;
	}

	public void SetWobbleOn()
	{
		if (_stateType == "HOMEDELIVERYBOXACTIVE")
		{
			MilMo_EventSystem.At(2f, delegate
			{
				_isWobbling = true;
				UnpauseWobble();
			});
		}
		else
		{
			_isWobbling = true;
			UnpauseWobble();
		}
	}

	public void SetWobbleOff()
	{
		_isWobbling = false;
		MilMo_HubBubble bubble = _bubble;
		if (bubble != null && bubble.Enabled)
		{
			_bubble.Hide();
		}
		PauseWobble();
	}

	private void PauseWobble()
	{
		_isPaused = true;
	}

	private void UnpauseWobble()
	{
		_isPaused = false;
		MilMo_EventSystem.At(0.3f, delegate
		{
			if (_bubble != null && !_bubble.Enabled && _isWobbling && StateIsActive && IsEnabled)
			{
				_bubble.Open();
				if (!_havePlayedOpenSound)
				{
					MilMo_GuiSoundManager.Instance.PlaySoundFx(_openClip.AudioClip);
					_havePlayedOpenSound = true;
				}
			}
		});
	}

	public void SetState(bool active, string stateType)
	{
		_stateType = stateType;
		StateIsActive = active;
		if (!active)
		{
			if (_isWobbling)
			{
				SetWobbleOff();
			}
		}
		else
		{
			_bubble?.Destroy();
			CreateBubble();
		}
	}

	public void ShowFakeBubble()
	{
		Vector2 vector = MilMo_Global.MainCamera.WorldToScreenPoint(_position);
		_bubble = _function.GetBubble("FAKESTATE", new Vector2(vector.x + _data.StateIconOffset.x * _resolution.x, (float)Screen.height - vector.y + _data.StateIconOffset.y * _resolution.y), delegate
		{
			_ui.RemoveChild(_bubble);
			_bubble = null;
		});
		_bubble.Open();
	}

	private void CreateBubble()
	{
		Vector2 vector = MilMo_Global.MainCamera.WorldToScreenPoint(_position);
		_bubble = _function.GetBubble(_stateType, new Vector2(vector.x + _data.StateIconOffset.x * _resolution.x, (float)Screen.height - vector.y + _data.StateIconOffset.y * _resolution.y), delegate(MilMo_HubBubble bubble)
		{
			_ui.RemoveChild(bubble);
			_bubble = null;
		});
	}

	private void SetUpInfoWindow(string iconPath, Vector2 iconStartSize, MilMo_LocString textToShow, MilMo_LocString title)
	{
		_infoWindow = new MilMo_HubInfoWindow(_ui, new Vector2(220f, 100f));
		_ui.AddChild(_infoWindow);
		_infoWindow.SetAllowMouseFocus(state: false);
		MilMo_Widget milMo_Widget = new MilMo_Widget(_ui);
		if (iconStartSize.x > iconStartSize.y)
		{
			float num = 60f / iconStartSize.x;
			float y = iconStartSize.y * num;
			milMo_Widget.SetScale(60f, y);
		}
		else
		{
			float num2 = 60f / iconStartSize.y;
			float x = iconStartSize.x * num2;
			milMo_Widget.SetScale(x, 60f);
		}
		milMo_Widget.SetAlignment(MilMo_GUI.Align.BottomLeft);
		milMo_Widget.SetTexture(iconPath);
		milMo_Widget.SetPosition(0f, 92f);
		milMo_Widget.UseParentAlpha = false;
		_infoWindow.AddChild(milMo_Widget);
		MilMo_Widget milMo_Widget2 = new MilMo_Widget(_ui);
		milMo_Widget2.SetScale(215f, 40f);
		milMo_Widget2.SetPosition(0f, -3f);
		milMo_Widget2.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget2.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		milMo_Widget2.UseParentAlpha = false;
		milMo_Widget2.SetFont(MilMo_GUI.Font.EborgSmall);
		milMo_Widget2.SetText(title);
		milMo_Widget2.SetDefaultTextColor(1f, 1f, 1f, 1f);
		_infoWindow.AddChild(milMo_Widget2);
		MilMo_Widget milMo_Widget3 = new MilMo_Widget(_ui);
		milMo_Widget3.SetScale(135f, 80f);
		milMo_Widget3.SetPosition(70f, 25f);
		milMo_Widget3.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget3.SetWordWrap(w: true);
		milMo_Widget3.SetFont(MilMo_GUI.Font.ArialRounded);
		milMo_Widget3.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget3.UseParentAlpha = false;
		milMo_Widget3.SetDefaultTextColor(1f, 1f, 1f, 1f);
		milMo_Widget3.SetText(textToShow);
		_infoWindow.AddChild(milMo_Widget3);
	}

	private void ShowInfoWindow()
	{
		_infoWindow.Open((float)Screen.width * 0.5f - _infoWindow.FullScale.x * 0.5f, (float)Screen.height - (_infoWindow.FullScale.y + 50f));
	}

	private void HideInfoWindow()
	{
		_infoWindow.Close();
	}
}
