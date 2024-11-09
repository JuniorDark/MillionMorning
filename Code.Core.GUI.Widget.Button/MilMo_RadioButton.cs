using System.Collections.Generic;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.Core.Sound;

namespace Code.Core.GUI.Widget.Button;

public sealed class MilMo_RadioButton : MilMo_Button
{
	private readonly List<ButtonFunc> _mCallbacks;

	private bool _mIsChecked;

	private MilMo_Texture _mCheckedTexture;

	private MilMo_Texture _mUncheckedTexture;

	private MilMo_Texture _mCheckedMoTexture;

	private MilMo_Texture _mUncheckedMoTexture;

	public bool Checked
	{
		get
		{
			return _mIsChecked;
		}
		set
		{
			InternalChecker(value);
		}
	}

	public MilMo_RadioButton(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "RadioButton " + MilMo_UserInterface.GetRandomID();
		_mCallbacks = new List<ButtonFunc>();
		Function = delegate
		{
			InternalChecker(val: true);
		};
		PointerHoverFunction = delegate
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Tick);
			SetAllTextures(Checked ? _mCheckedMoTexture : _mUncheckedMoTexture);
		};
		PointerLeaveFunction = delegate
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Tick);
			SetAllTextures(Checked ? _mCheckedTexture : _mUncheckedTexture);
		};
	}

	public void RegisterCallback(ButtonFunc callback)
	{
		_mCallbacks.Add(callback);
	}

	private void InternalChecker(bool val)
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Pick);
		_mIsChecked = val;
		if (val)
		{
			SetChecked();
		}
		else
		{
			SetUnchecked();
		}
	}

	private void SetChecked()
	{
		SetAllTextures(_mCheckedTexture);
		foreach (ButtonFunc mCallback in _mCallbacks)
		{
			mCallback(this);
		}
	}

	private void SetUnchecked()
	{
		SetAllTextures(_mUncheckedTexture);
	}

	public void SetCheckedTexture(string texture)
	{
		_mCheckedTexture = new MilMo_Texture("Content/GUI/" + texture);
		_mCheckedTexture.AsyncLoad();
	}

	public void SetUncheckedTexture(string texture)
	{
		_mUncheckedTexture = new MilMo_Texture("Content/GUI/" + texture);
		_mUncheckedTexture.AsyncLoad();
	}

	public void SetCheckedMouseOverTexture(string texture)
	{
		_mCheckedMoTexture = new MilMo_Texture("Content/GUI/" + texture);
		_mCheckedMoTexture.AsyncLoad();
	}

	public void SetUncheckedMouseOverTexture(string texture)
	{
		_mUncheckedMoTexture = new MilMo_Texture("Content/GUI/" + texture);
		_mUncheckedMoTexture.AsyncLoad();
	}
}
