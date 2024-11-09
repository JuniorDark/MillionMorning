namespace Code.Core.GUI;

public sealed class MilMo_CoinGUIUpdateInfo
{
	private const int PREMIUM_TOKEN_PANE = 1;

	private const int QUICK_INFO_DIALOG = 4;

	private readonly int _mGUIEffects;

	public int Change { get; private set; }

	public bool PremiumTokenPane => (_mGUIEffects & 1) != 0;

	public bool QuickInfoDialog => (_mGUIEffects & 4) != 0;

	public MilMo_CoinGUIUpdateInfo(int guiEffects, int change)
	{
		_mGUIEffects = guiEffects;
		Change = change;
	}
}
