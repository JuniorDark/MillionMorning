using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.World.Player;
using Code.World.Voting;
using Core;
using Localization;
using UI.HUD.Dialogues;
using UnityEngine;

namespace Code.World.GUI.Ladder;

public class MilMo_VotingVisualization : MilMo_Widget
{
	public const int STARSCALE = 25;

	private MilMo_Widget _votingText;

	private int _score;

	private int _newScore;

	private readonly int _voteObjectIdentifier;

	private readonly MilMo_VoteManager.VoteTypes _voteType;

	private List<MilMoVotingPoint> _pointButtons;

	private List<MilMo_Widget> _buttonBackGrounds;

	private MilMo_Button _confirmVoteButton;

	private MilMo_Button _cancelVoteButton;

	private bool _lockedPointFade;

	public MilMo_VotingVisualization(MilMo_UserInterface ui, MilMo_VoteManager.VoteTypes voteType, int voteObjectIdentifier, string outLinePath, string filledPath, string emptyPath, Vector2 position, int score, int max, bool canChangeVote, float rating)
		: base(ui)
	{
		MilMo_VotingVisualization milMo_VotingVisualization = this;
		_voteType = voteType;
		_score = score;
		_voteObjectIdentifier = voteObjectIdentifier;
		SetPosition(position);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		Vector2 pointPosition = new Vector2(70f, 25f);
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetScale(64f, 64f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetPosition(1f, 1f);
		milMo_Widget.SetTexture(filledPath);
		milMo_Widget.FadeToDefaultColor = false;
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Widget.SetFont(MilMo_GUI.Font.EborgMedium);
		milMo_Widget.SetDefaultTextColor(0f, 0f, 0f, 1f);
		if (rating > 0f)
		{
			milMo_Widget.SetText(MilMo_Localization.GetNotLocalizedLocString(Math.Round(rating, 2).ToString()));
		}
		else
		{
			milMo_Widget.SetAlpha(0.5f);
			milMo_Widget.SetText(MilMo_Localization.GetNotLocalizedLocString("0"));
			milMo_Widget.SetDefaultTextColor(0f, 0f, 0f, 0.5f);
		}
		milMo_Widget.AllowPointerFocus = false;
		MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
		milMo_Widget2.SetScale(64f, 64f);
		milMo_Widget2.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget2.SetPosition(0f, 0f);
		milMo_Widget2.SetTexture(outLinePath);
		milMo_Widget2.SetDefaultColor(0f, 0f, 0f, 1f);
		milMo_Widget.AddChild(milMo_Widget2);
		AddChild(milMo_Widget);
		MilMo_EventSystem.At(1.5f, delegate
		{
			for (int i = 0; i < max; i++)
			{
				if (!canChangeVote)
				{
					if (milMo_VotingVisualization._buttonBackGrounds == null)
					{
						milMo_VotingVisualization._buttonBackGrounds = new List<MilMo_Widget>();
					}
					MilMo_Button milMo_Button = new MilMo_Button(milMo_VotingVisualization.UI);
					milMo_Button.SetPosition(pointPosition);
					if (i + 1 <= score)
					{
						milMo_Button.SetAllTextures(filledPath);
					}
					else
					{
						milMo_Button.SetAllTextures(emptyPath);
						milMo_Button.SetDefaultColor(1f, 1f, 1f, 0.65f);
					}
					milMo_Button.SetScale(0f, 0f);
					milMo_Button.ScaleTo(25f, 25f);
					milMo_Button.SetScalePull(0.05f, 0.05f);
					milMo_Button.SetScaleDrag(0.7f, 0.7f);
					milMo_Button.SetPosPull(0.05f, 0.05f);
					milMo_Button.SetPosDrag(0.8f, 0.8f);
					float angle = 0f - 25f * MilMo_Utility.Random() - 12.5f;
					milMo_Button.SetAngle(angle);
					milMo_Button.Angle(0f);
					milMo_Button.Impulse(10f, 0f);
					milMo_Button.SetAlpha(0f);
					milMo_Button.FadeSpeed = 0.1f;
					milMo_Button.SetAlignment(MilMo_GUI.Align.TopLeft);
					MilMo_Widget milMo_Widget3 = new MilMo_Widget(milMo_VotingVisualization.UI);
					milMo_Widget3.SetScale(0f, 0f);
					milMo_Widget3.ScaleTo(25f, 25f);
					milMo_Widget3.SetPosition(0f, 0f);
					milMo_Widget3.SetScalePull(0.05f, 0.05f);
					milMo_Widget3.SetScaleDrag(0.7f, 0.7f);
					milMo_Widget3.SetPosPull(0.05f, 0.05f);
					milMo_Widget3.SetPosDrag(0.8f, 0.8f);
					milMo_Widget3.SetAngle(angle);
					milMo_Widget3.Angle(0f);
					milMo_Widget3.Impulse(10f, 0f);
					milMo_Widget3.SetAlpha(0f);
					milMo_Widget3.FadeSpeed = 0.1f;
					milMo_Widget3.SetTexture(outLinePath);
					milMo_Widget3.SetDefaultColor(0f, 0f, 0f, 1f);
					milMo_Widget3.SetAlignment(MilMo_GUI.Align.TopLeft);
					milMo_Widget3.AllowPointerFocus = false;
					milMo_Button.AddChild(milMo_Widget3);
					milMo_Button.Function = delegate
					{
						DialogueSpawner.SpawnWarningModalDialogue(new LocalizedStringWithArgument("CharacterShop_253"), new LocalizedStringWithArgument("Homes_13304"));
						MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
					};
					milMo_VotingVisualization.AddChild(milMo_Button);
				}
				else
				{
					if (milMo_VotingVisualization._pointButtons == null)
					{
						milMo_VotingVisualization._pointButtons = new List<MilMoVotingPoint>();
					}
					if (milMo_VotingVisualization._confirmVoteButton == null)
					{
						milMo_VotingVisualization._confirmVoteButton = new MilMo_Button(milMo_VotingVisualization.UI);
						milMo_VotingVisualization._confirmVoteButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
						milMo_VotingVisualization._confirmVoteButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
						milMo_VotingVisualization._confirmVoteButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
						milMo_VotingVisualization._confirmVoteButton.SetPosition(75f, 75f);
						milMo_VotingVisualization._confirmVoteButton.SetScale(100f, 25f);
						milMo_VotingVisualization._confirmVoteButton.SetAlpha(0f);
						milMo_VotingVisualization._confirmVoteButton.SetFont(MilMo_GUI.Font.EborgSmall);
						milMo_VotingVisualization._confirmVoteButton.SetFontScale(0.8f);
						milMo_VotingVisualization._confirmVoteButton.FadeToDefaultColor = false;
						milMo_VotingVisualization._confirmVoteButton.SetText(MilMo_Localization.GetLocString("Homes_13296"));
						milMo_VotingVisualization._confirmVoteButton.SetExtraDrawTextSize(50f, 0f);
						milMo_VotingVisualization._confirmVoteButton.Function = milMo_VotingVisualization.ConfirmVoteButtonClicked;
						milMo_VotingVisualization._confirmVoteButton.SetFadeSpeed(0.01f);
						milMo_VotingVisualization.AddChild(milMo_VotingVisualization._confirmVoteButton);
						milMo_VotingVisualization._cancelVoteButton = new MilMo_Button(milMo_VotingVisualization.UI);
						milMo_VotingVisualization._cancelVoteButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
						milMo_VotingVisualization._cancelVoteButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
						milMo_VotingVisualization._cancelVoteButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
						milMo_VotingVisualization._cancelVoteButton.SetPosition(175f, 75f);
						milMo_VotingVisualization._cancelVoteButton.SetScale(100f, 25f);
						milMo_VotingVisualization._cancelVoteButton.SetAlpha(0f);
						milMo_VotingVisualization._cancelVoteButton.SetFont(MilMo_GUI.Font.EborgSmall);
						milMo_VotingVisualization._cancelVoteButton.SetFontScale(0.8f);
						milMo_VotingVisualization._cancelVoteButton.FadeToDefaultColor = false;
						milMo_VotingVisualization._cancelVoteButton.SetText(MilMo_Localization.GetLocString("Homes_13295"));
						milMo_VotingVisualization._cancelVoteButton.SetExtraDrawTextSize(50f, 0f);
						milMo_VotingVisualization._cancelVoteButton.Function = milMo_VotingVisualization.CancelVoteButtonClicked;
						milMo_VotingVisualization._cancelVoteButton.SetFadeSpeed(0.01f);
						milMo_VotingVisualization.AddChild(milMo_VotingVisualization._cancelVoteButton);
					}
					MilMoVotingPoint milMoVotingPoint = new MilMoVotingPoint(milMo_VotingVisualization.UI, pointPosition, outLinePath, filledPath, emptyPath);
					milMoVotingPoint.Info = i + 1;
					milMoVotingPoint.Args = milMoVotingPoint;
					if (i > score - 1)
					{
						milMoVotingPoint.SetEmpty();
					}
					else
					{
						milMoVotingPoint.SetFilled();
					}
					milMoVotingPoint.PointerHoverFunction = milMo_VotingVisualization.MouseOverVoteButton;
					milMoVotingPoint.PointerLeaveFunction = milMo_VotingVisualization.MouseLeaveVoteButton;
					milMoVotingPoint.Function = milMo_VotingVisualization.MouseClickVoteButton;
					milMo_VotingVisualization._pointButtons.Add(milMoVotingPoint);
					milMo_VotingVisualization.AddChild(milMoVotingPoint);
				}
				pointPosition.x += 25f;
			}
			milMo_VotingVisualization.SetText();
			milMo_VotingVisualization.SetScale(250f, 100f);
		});
	}

	public override void Step()
	{
		if (Enabled && MilMo_Player.InHome && !MilMo_Player.InMyHome)
		{
			base.Step();
		}
	}

	public override void Draw()
	{
		if (Enabled && MilMo_Player.InHome && !MilMo_Player.InMyHome)
		{
			base.Draw();
		}
	}

	private void ConfirmVoteButtonClicked(object o)
	{
		foreach (MilMoVotingPoint pointButton in _pointButtons)
		{
			pointButton.DectivateOutline();
			pointButton.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.None);
			pointButton.PointerHoverFunction = null;
			pointButton.Function = null;
			pointButton.PointerLeaveFunction = null;
		}
		_confirmVoteButton.SetAlpha(0f);
		_cancelVoteButton.SetAlpha(0f);
		Singleton<GameNetwork>.Instance.SendToGameServer(new ClientCastVote(new VoteData((sbyte)_newScore, (sbyte)_voteType, _voteObjectIdentifier)));
		DialogueSpawner.SpawnQuickInfoDialogue(new LocalizedStringWithArgument("Homes_13309"), new LocalizedStringWithArgument("Homes_13311"), "ConfirmIcon");
	}

	private void CancelVoteButtonClicked(object o)
	{
		foreach (MilMoVotingPoint pointButton in _pointButtons)
		{
			pointButton.AllowPointerFocus = true;
		}
		_lockedPointFade = false;
		_confirmVoteButton.SetAlpha(0f);
		_cancelVoteButton.SetAlpha(0f);
		MouseLeaveVoteButton();
	}

	private void MouseClickVoteButton(object o)
	{
		foreach (MilMoVotingPoint pointButton in _pointButtons)
		{
			pointButton.AllowPointerFocus = false;
		}
		_confirmVoteButton.AlphaTo(1f);
		_cancelVoteButton.AlphaTo(1f);
		MilMo_Button milMo_Button = (MilMo_Button)o;
		_newScore = milMo_Button.Info;
		_lockedPointFade = true;
	}

	private void MouseLeaveVoteButton()
	{
		if (_lockedPointFade || _pointButtons.Any((MilMoVotingPoint point) => point.Hover()))
		{
			return;
		}
		for (int i = 0; i < _pointButtons.Count; i++)
		{
			if (i <= _score - 1)
			{
				_pointButtons[i].SetFilled();
			}
			else
			{
				_pointButtons[i].SetEmpty();
			}
		}
	}

	private void MouseOverVoteButton()
	{
		int num = 0;
		foreach (MilMoVotingPoint item in _pointButtons.Where((MilMoVotingPoint point) => point.Hover()))
		{
			num = item.Info;
		}
		for (int i = 0; i < _pointButtons.Count; i++)
		{
			if (i < num)
			{
				_pointButtons[i].SetFilled();
			}
			else
			{
				_pointButtons[i].SetEmpty();
			}
		}
	}

	public void SetScore(int score)
	{
		_score = score;
		MouseLeaveVoteButton();
	}

	private void SetText()
	{
		_votingText = new MilMo_Widget(UI);
		_votingText.SetFont(MilMo_GUI.Font.GothamSmall);
		_votingText.SetExtraDrawTextSize(250f, 0f);
		_votingText.SetAlignment(MilMo_GUI.Align.TopLeft);
		_votingText.AllowPointerFocus = false;
		_votingText.SetEnabled(e: false);
		_votingText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		if (_score == 0)
		{
			_votingText.SetPosition(new Vector2(70f, -10f));
			MilMo_LocString locString = MilMo_Localization.GetLocString("Homes_13308");
			_votingText.SetText(locString);
		}
		else
		{
			_votingText.SetPosition(new Vector2(80f, -10f));
			MilMo_LocString locString2 = MilMo_Localization.GetLocString("Homes_13307");
			_votingText.SetText(locString2);
		}
		AddChild(_votingText);
		MilMo_EventSystem.At(0.5f, delegate
		{
			_votingText.SetEnabled(e: true);
			_votingText.SetScale(0f, 0f);
			_votingText.ScaleTo(50f, 50f);
			_votingText.SetScalePull(0.05f, 0.05f);
			_votingText.SetScaleDrag(0.3f, 0.3f);
			_votingText.SetPosPull(0.05f, 0.05f);
			_votingText.SetPosDrag(0.8f, 0.8f);
			_votingText.SetAngle(0f - 25f * MilMo_Utility.Random() - 12.5f);
			_votingText.Angle(0f);
			_votingText.FadeSpeed = 0.05f;
			_votingText.Impulse(10f, 0f);
		});
	}
}
