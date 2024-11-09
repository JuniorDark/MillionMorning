using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Visual.Audio;
using UnityEngine;

namespace Code.Core.Visual.Effect;

public class MilMo_RandomSoundAction : MilMo_SoundAction
{
	private MilMo_RandomSoundTemplate _template;

	public override AudioClip Sound
	{
		get
		{
			if (_template == null)
			{
				return null;
			}
			return _template.GetClip();
		}
	}

	private MilMo_RandomSoundAction()
	{
	}

	public new static MilMo_RandomSoundAction Load(MilMo_SFFile file)
	{
		MilMo_RandomSoundAction milMo_RandomSoundAction = new MilMo_RandomSoundAction
		{
			Looping = false,
			Duration = 0f
		};
		string text = (milMo_RandomSoundAction.Name = file.GetString());
		milMo_RandomSoundAction._template = MilMo_TemplateContainer.Get().GetTemplate("RandomSound", "RandomSound/" + text) as MilMo_RandomSoundTemplate;
		if (milMo_RandomSoundAction._template == null)
		{
			Debug.LogWarning("Failed to load RandomSound template " + text + " for random sound particle effect action in " + file.Path);
			return null;
		}
		while (file.HasMoreTokens())
		{
			milMo_RandomSoundAction.ReadToken(file);
		}
		return milMo_RandomSoundAction;
	}
}
