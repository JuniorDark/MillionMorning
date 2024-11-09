using Code.Core.EventSystem;
using Code.Core.Network.messages.server;
using Code.World.GUI;
using Code.World.GUI.Ladder;
using Code.World.Home;
using Code.World.Player;

namespace Code.World.Voting;

public static class MilMo_VoteManager
{
	public enum VoteTypes
	{
		HOMES,
		CLOTHING
	}

	public const int MINIMUM_VOTES = 15;

	public static void Initialize()
	{
		MilMo_EventSystem.Listen("vote_info_received", GotVoteInfo).Repeating = true;
		MilMo_EventSystem.Listen("vote_score_received", GotVoteScore).Repeating = true;
		MilMo_EventSystem.Listen("vote_numvotes_received", GotNumVotes).Repeating = true;
		MilMo_EventSystem.Listen("ladder_page_received", LadderPageReceived).Repeating = true;
	}

	private static void GotVoteInfo(object msgAsObj)
	{
		if (msgAsObj is ServerVoteStatus serverVoteStatus && serverVoteStatus.getVoteData().GetVoteType() == 0 && MilMo_Player.InHome && !MilMo_Player.InMyHome)
		{
			MilMo_Home.CurrentHome.ShowVoteData(serverVoteStatus.getCanVote() == 1, (byte)serverVoteStatus.getVoteData().GetVoteValue(), serverVoteStatus.getCurrentScore());
		}
	}

	private static void GotVoteScore(object msgAsObj)
	{
		if (msgAsObj is ServerVoteScore serverVoteScore && serverVoteScore.getVoteType() == 0 && MilMo_Player.InHome && MilMo_Player.InMyHome)
		{
			MilMo_Home.CurrentHome.SetVoteScore(serverVoteScore.getScore(), serverVoteScore.getNumVotes());
		}
	}

	private static void GotNumVotes(object msgAsObj)
	{
		if (msgAsObj is ServerNumVotes serverNumVotes && serverNumVotes.getVoteType() == 0 && MilMo_Player.InHome && MilMo_Player.InMyHome)
		{
			MilMo_Home.CurrentHome.SetVoteScore(0f, serverNumVotes.getNumVotes());
		}
	}

	private static void LadderPageReceived(object msgAsObj)
	{
		ServerLadderPage serverLadderPage = (ServerLadderPage)msgAsObj;
		if (serverLadderPage.getVoteType() == 0)
		{
			((MilMoLadderWindow)MilMo_GlobalUI.Instance.GetItem("HomeLadderWindow")).PageReceived(serverLadderPage);
		}
	}
}
