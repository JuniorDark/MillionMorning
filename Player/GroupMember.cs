using System;
using Code.Core.Network.nexus;
using Localization;
using UI.HUD.Dialogues;
using UI.Sprites;

namespace Player;

public class GroupMember : IIdentity
{
	public int UserIdentifier { get; }

	public string Id => UserIdentifier.ToString();

	public string Name { get; }

	public bool IsLeader { get; private set; }

	private bool IsRequest { get; set; }

	public event Action<bool> OnLeaderChanged;

	public GroupMember(string id, string name, bool isLeader, bool isInvitation = false)
	{
		UserIdentifier = int.Parse(id);
		Name = name;
		IsLeader = isLeader;
		IsRequest = isInvitation;
	}

	public void ChangeLeader(GroupMember member)
	{
		IsLeader = member.UserIdentifier == UserIdentifier;
		this.OnLeaderChanged?.Invoke(IsLeader);
	}

	public void OpenGroupRequestDialog()
	{
		if (IsRequest)
		{
			GroupManager groupManager = GroupManager.Get();
			LocalizedStringWithArgument localizedStringWithArgument = new LocalizedStringWithArgument("Messenger_FriendList_10259", Name);
			DialogueSpawner.SpawnAcceptIgnoreLaterModalDialogue("Messenger_FriendList_10260", localizedStringWithArgument.GetMessage(), delegate
			{
				groupManager.AcceptInvitation(Id);
			}, null, delegate
			{
				groupManager.DeclineInvitation(Id);
			}, new PortraitSpriteLoader(Id));
		}
	}
}
