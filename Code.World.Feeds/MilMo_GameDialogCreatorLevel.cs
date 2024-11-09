using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.World.GUI.GameDialog;
using Code.World.Level.LevelInfo;

namespace Code.World.Feeds;

public class MilMo_GameDialogCreatorLevel : MilMo_GameDialogCreator
{
	private readonly MilMo_EventSystem.MilMo_Callback _closeCallback;

	public MilMo_GameDialogCreatorLevel(MilMo_LevelInfoData levelInfo, string iconPathIngame, MilMo_UserInterface ui, MilMo_EventSystem.MilMo_Callback closeCallback)
		: base(ui)
	{
		MilMo_FeedDefaultTemplate defaults = MilMo_FeedDefaultTemplate.GetDefaults("Level");
		if (defaults != null)
		{
			FeedEventIngame = defaults.FeedEventIngame;
			CustomJinglePath = defaults.DialogSound;
		}
		IconPathIngame = iconPathIngame;
		FeedDescriptionIngame = levelInfo.FeedDescriptionInGame;
		ObjectName = levelInfo.DisplayName;
		_closeCallback = closeCallback;
		if (levelInfo.FeedEventInGame != null && !string.IsNullOrEmpty(levelInfo.FeedEventInGame.String))
		{
			FeedEventIngame = levelInfo.FeedEventInGame;
		}
	}

	protected override void CreateDialog()
	{
		if (UI != null && (bool)MilMo_World.Instance && MilMo_World.Instance.UI != null && (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI) || MilMo_GameDialogCreator.TheDialogs[UI] == null))
		{
			if (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI))
			{
				MilMo_GameDialogCreator.TheDialogs.Add(UI, null);
			}
			MilMo_GameDialogLevel milMo_GameDialogLevel = new MilMo_GameDialogLevel(UI, CloseDialog, MilMo_FeedExclamations.GetExclamation(), FeedEventIngame, ObjectName, FeedDescriptionIngame);
			if (!string.IsNullOrEmpty(CustomJinglePath))
			{
				milMo_GameDialogLevel.CustomJinglePath = CustomJinglePath;
			}
			UI.AddChild(milMo_GameDialogLevel);
			milMo_GameDialogLevel.Show(IconPathIngame);
			MilMo_GameDialogCreator.TheDialogs[UI] = new OpenDialog(milMo_GameDialogLevel, this);
		}
	}

	protected override void CloseDialog(object o)
	{
		base.CloseDialog(null);
		if (_closeCallback != null)
		{
			_closeCallback();
		}
	}
}
