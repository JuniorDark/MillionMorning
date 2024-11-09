using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.World.Tutorial;
using Core.GameEvent;
using Localization;
using UnityEngine;

namespace UI.HUD.Dialogues.Feed.Tutorial;

[CreateAssetMenu(menuName = "Dialogues/TutorialFeedDialogueSO", fileName = "TutorialFeedDialogueSO")]
public class TutorialFeedDialogueSO : FeedDialogueSO
{
	[SerializeField]
	private string faceIconPath;

	[SerializeField]
	private string targetIconPath;

	[SerializeField]
	private string keybindIconPath;

	[SerializeField]
	protected string contentHeadline;

	[SerializeField]
	protected string contentDescription;

	private ITutorialData _template;

	private IMilMo_Tutorial _tutorial;

	public override string GetAddressableKey()
	{
		return "TutorialFeedDialogueWindow";
	}

	public override int GetPriority()
	{
		return 2;
	}

	public async Task<Texture2D> GetFaceIcon()
	{
		return await GetIconAsync(faceIconPath);
	}

	public async Task<Texture2D> GetSlot1Icon()
	{
		return await GetIconAsync(targetIconPath);
	}

	public async Task<Texture2D> GetSlot2Icon()
	{
		return await GetIconAsync(keybindIconPath);
	}

	public string GetContentHeadline()
	{
		return contentHeadline;
	}

	public string GetContentDescription()
	{
		return contentDescription;
	}

	public void Init(IMilMo_Tutorial tutorial)
	{
		_tutorial = tutorial;
		_template = tutorial.GetTemplate();
		contentHeadline = _template.Headline?.String;
		contentDescription = _template.Text?.String;
		faceIconPath = "IconTutorial01";
		targetIconPath = _template.TargetImagePath;
		keybindIconPath = _template.KeyBindImagePath;
	}

	public void Triggered()
	{
		IMilMo_Tutorial tutorial = _tutorial;
		tutorial.OnCloseTriggered = (Action)Delegate.Combine(tutorial.OnCloseTriggered, new Action(DialogueWindow.Close));
		if (!string.IsNullOrEmpty(_template.ArrowTarget))
		{
			GameEvent.OnTutorialArrowEvent.RaiseEvent(_template.ArrowTarget);
		}
	}

	public void Finished()
	{
		IMilMo_Tutorial tutorial = _tutorial;
		tutorial.OnCloseTriggered = (Action)Delegate.Remove(tutorial.OnCloseTriggered, new Action(DialogueWindow.Close));
		_tutorial.RemoveCloseTriggers();
		GameEvent.OnCloseTutorialArrowEvent.RaiseEvent();
	}

	private void Confirm()
	{
		DialogueWindow.Close();
	}

	public override List<DialogueButtonInfo> GetActions()
	{
		return new List<DialogueButtonInfo>
		{
			new DialogueButtonInfo(Confirm, new LocalizedStringWithArgument("Generic_OK"), isDefault: true)
		};
	}

	public override bool CanBeShown()
	{
		return !DialogueUser.IsTooHappy;
	}

	public string GetTutorialIdentifier()
	{
		return _tutorial.GetIdentifier();
	}
}
