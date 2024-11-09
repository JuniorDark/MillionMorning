namespace Code.World.GUI.Homes;

public abstract class MilMo_SettingsHandler<T> where T : struct
{
	protected int OriginalHash;

	protected T CurrentSetting { get; set; }

	protected abstract void LoadSetting();

	public T GetSettings()
	{
		return CurrentSetting;
	}

	public void UpdateSetting(T setting)
	{
		CurrentSetting = setting;
	}

	public abstract void Persist();
}
