using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.World.GUI.GameDialog;

namespace Code.World.Feeds;

public class MilMo_GameDialogCreatorConverter : MilMo_GameDialogCreator
{
	private readonly MilMo_GameDialogConverter.Converter _converter;

	public MilMo_GameDialogCreatorConverter(MilMo_GameDialogConverter.Converter converter, MilMo_UserInterface ui)
		: base(ui)
	{
		_converter = converter;
	}

	protected override async void CreateDialog()
	{
		if (UI != null && (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI) || MilMo_GameDialogCreator.TheDialogs[UI] == null))
		{
			if (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI))
			{
				MilMo_GameDialogCreator.TheDialogs.Add(UI, null);
			}
			MilMo_GameDialogConverter dialog = new MilMo_GameDialogConverter(UI, CloseDialog, _converter);
			UI.AddChild(dialog);
			await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/" + _converter.MOpenTexture);
			dialog.Show();
			MilMo_GameDialogCreator.TheDialogs[UI] = new OpenDialog(dialog, this);
		}
	}
}
