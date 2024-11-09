using System.Collections.Generic;
using System.Linq;
using Code.Core.GUI.Widget.Button;

namespace Code.Core.GUI;

public sealed class MilMo_RadioGroup
{
	private readonly List<MilMo_RadioButton> _buttons;

	public MilMo_RadioGroup()
	{
		_buttons = new List<MilMo_RadioButton>();
	}

	public void AddRadiobutton(MilMo_RadioButton button)
	{
		button.RegisterCallback(ButtonChecked);
		button.Info = _buttons.Count;
		_buttons.Add(button);
	}

	private void ButtonChecked(object o)
	{
		foreach (MilMo_RadioButton item in _buttons.Where((MilMo_RadioButton b) => b.Info != ((MilMo_RadioButton)o).Info))
		{
			item.Checked = false;
		}
	}
}
