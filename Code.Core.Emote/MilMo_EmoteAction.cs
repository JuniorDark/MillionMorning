using Code.Core.Config;
using Code.Core.ResourceSystem;

namespace Code.Core.Emote;

public abstract class MilMo_EmoteAction
{
	protected static readonly bool DevMode = MilMo_Config.Instance.IsTrue("Debug.EmoteSystem", defaultValue: false);

	public void Execute(object manager)
	{
		ExecuteInternal((MilMo_EmoteManager)manager);
	}

	public abstract bool Read(MilMo_SFFile file);

	public abstract MilMo_EmoteMover GetMover(MilMo_EmoteManager manager);

	public abstract void UpdateDefaultValues(MilMo_EmoteManager manager);

	protected abstract void ExecuteInternal(MilMo_EmoteManager manager);

	public abstract MilMo_EmoteAction Instantiate();
}
