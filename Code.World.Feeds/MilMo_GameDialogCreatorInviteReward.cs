using Code.Core.Items;
using Code.World.GUI;
using Code.World.GUI.GameDialog;

namespace Code.World.Feeds;

public class MilMo_GameDialogCreatorInviteReward : MilMo_GameDialogCreator
{
	private readonly int _invitedCount;

	private readonly MilMo_Item _reward;

	private readonly int _rewardAmount;

	private readonly int _nextRewardInviteCount;

	private readonly MilMo_Item _nextReward;

	private readonly int _nextRewardAmount;

	private readonly bool _isAcceptedRewards;

	public MilMo_GameDialogCreatorInviteReward(int invitedCount, MilMo_Item reward, int rewardAmount, int nextRewardInviteCount, MilMo_Item nextReward, int nextRewardAmount, bool isAcceptedRewards)
		: base(MilMo_GlobalUI.GetSystemUI)
	{
		_invitedCount = invitedCount;
		_reward = reward;
		_rewardAmount = rewardAmount;
		_nextRewardInviteCount = nextRewardInviteCount;
		_nextReward = nextReward;
		_nextRewardAmount = nextRewardAmount;
		_isAcceptedRewards = isAcceptedRewards;
	}

	protected override void CreateDialog()
	{
		if (UI != null && (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI) || MilMo_GameDialogCreator.TheDialogs[UI] == null))
		{
			if (!MilMo_GameDialogCreator.TheDialogs.ContainsKey(UI))
			{
				MilMo_GameDialogCreator.TheDialogs.Add(UI, null);
			}
			MilMo_GameDialogInviteReward milMo_GameDialogInviteReward = new MilMo_GameDialogInviteReward(UI, CloseDialog, _invitedCount, _reward, _rewardAmount, _nextRewardInviteCount, _nextReward, _nextRewardAmount, _isAcceptedRewards);
			UI.AddChild(milMo_GameDialogInviteReward);
			milMo_GameDialogInviteReward.Show(_reward);
			MilMo_GameDialogCreator.TheDialogs[UI] = new OpenDialog(milMo_GameDialogInviteReward, this);
		}
	}
}
