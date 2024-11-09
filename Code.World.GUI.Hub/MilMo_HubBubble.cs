using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.GUI.Hub;

public sealed class MilMo_HubBubble : MilMo_Widget
{
	public delegate void OnClose(MilMo_HubBubble bubble);

	private readonly MilMo_Widget _mIconWidget;

	private Rect _mShadowRect;

	private Rect _mFrameRect;

	private readonly Vector2 _mRectScale = Vector2.zero;

	private readonly Color _mBubbleColor = new Color(0f, 0.5f, 1f, 1f);

	private readonly OnClose _mOnCloseCallback;

	private bool _mIsOpening;

	private MilMo_TimerEvent _mUpdateTextureTimerEvent;

	private List<Texture2D> _mStoredTextures;

	public MilMo_HubBubble(MilMo_UserInterface ui, Vector2 scale, OnClose onCloseCallback)
		: base(ui)
	{
		_mOnCloseCallback = onCloseCallback;
		Identifier = "Bubble" + MilMo_UserInterface.GetRandomID();
		SetAlignment(MilMo_GUI.Align.BottomCenter);
		_mRectScale = scale;
		SetScale(0f, 0f);
		_mIconWidget = new MilMo_Widget(UI);
		_mIconWidget.UseParentAlpha = false;
		_mIconWidget.FadeSpeed = 0.02f;
		_mIconWidget.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_mIconWidget.SetScale(0f, 0f);
		ScaleType = MilMo_Mover.UpdateFunc.Spring;
		SetScalePull(0.12f, 0.12f);
		SetScaleDrag(0.7f, 0.7f);
		ScaleMover.MinVel.x = 0.01f;
		ScaleMover.MinVel.y = 0.01f;
		AddChild(_mIconWidget);
		SetSkin(1);
		_mIconWidget.AllowPointerFocus = false;
		AllowPointerFocus = false;
	}

	public void Open()
	{
		if (!_mIsOpening)
		{
			_mIsOpening = true;
			SetScale(0f, 0f);
			_mIconWidget.SetScale(0f, 0f);
			Enabled = true;
			Vector2 vector = new Vector2((float)Screen.width / 1024f, (float)Screen.height / 720f);
			float num = 1f;
			if (vector.x < 1f || vector.y < 1f)
			{
				num = ((vector.x <= vector.y) ? vector.x : vector.y);
			}
			_mIconWidget.ScaleToAbsolute(60f * num, 60f * num);
			_mIconWidget.SetPosition(0f, 0f);
			ScaleToAbsolute(_mRectScale.x * num, _mRectScale.y * num);
			_mIconWidget.SetPosition(_mRectScale.x * num * 0.25f, _mRectScale.y * num * 0.25f);
			if (_mStoredTextures != null)
			{
				NextTexture(0, _mStoredTextures);
			}
		}
	}

	public void Close()
	{
		ScaleTo(0f, 0f);
		_mIconWidget.SetScale(0f, 0f);
		_mIsOpening = false;
		if (_mUpdateTextureTimerEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mUpdateTextureTimerEvent);
			_mUpdateTextureTimerEvent = null;
		}
		MilMo_EventSystem.At(0.15f, delegate
		{
			if (!_mIsOpening)
			{
				Enabled = false;
				if (_mOnCloseCallback != null)
				{
					_mOnCloseCallback(this);
				}
			}
		});
	}

	public void Destroy()
	{
		Enabled = false;
		if (_mOnCloseCallback != null)
		{
			_mOnCloseCallback(this);
		}
	}

	public void Hide()
	{
		ScaleTo(0f, 0f);
		_mIconWidget.SetScale(0f, 0f);
		_mIsOpening = false;
		if (_mUpdateTextureTimerEvent != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mUpdateTextureTimerEvent);
			_mUpdateTextureTimerEvent = null;
		}
		MilMo_EventSystem.At(0.15f, delegate
		{
			if (!_mIsOpening)
			{
				Enabled = false;
			}
		});
	}

	public void SetIconTexture(string texture)
	{
		_mIconWidget.SetTexture(texture);
	}

	public void SetIconTexture(Texture2D texture)
	{
		_mIconWidget.SetTexture(texture);
	}

	public void SetIconTextureList(List<Texture2D> textures)
	{
		_mStoredTextures = textures;
	}

	private void NextTexture(int index, List<Texture2D> textures)
	{
		if (!Enabled)
		{
			return;
		}
		if (index >= textures.Count)
		{
			index = 0;
		}
		_mIconWidget.FadeAlpha(1f, 0f);
		_mUpdateTextureTimerEvent = MilMo_EventSystem.At(0.8f, delegate
		{
			_mIconWidget.SetTexture(textures[index]);
			_mIconWidget.FadeAlpha(0f, 1f);
			_mUpdateTextureTimerEvent = MilMo_EventSystem.At(2.7f, delegate
			{
				index++;
				NextTexture(index, textures);
			});
		});
	}

	public override void Draw()
	{
		if (Enabled && !(Scale.x < 20f) && !(Scale.y < 20f))
		{
			_mFrameRect = GetScreenPosition();
			_mFrameRect.x -= Scale.x * 0.5f;
			_mFrameRect.y -= Scale.y * 0.5f;
			_mFrameRect.width += Scale.x * 0.5f;
			_mFrameRect.height += Scale.y * 0.5f;
			_mShadowRect = _mFrameRect;
			_mShadowRect.x -= 6f;
			_mShadowRect.y -= 6f;
			_mShadowRect.width += 12f;
			_mShadowRect.height += 12f;
			GUISkin skin = UnityEngine.GUI.skin;
			UnityEngine.GUI.skin = Skin;
			Color mBubbleColor = _mBubbleColor;
			mBubbleColor.a = CurrentColor.a * 0.2f;
			UnityEngine.GUI.color = mBubbleColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			UnityEngine.GUI.Box(_mShadowRect, "");
			mBubbleColor = CurrentColor;
			UnityEngine.GUI.color = mBubbleColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			UnityEngine.GUI.Box(_mFrameRect, "");
			UnityEngine.GUI.skin = skin;
			base.Draw();
		}
	}
}
