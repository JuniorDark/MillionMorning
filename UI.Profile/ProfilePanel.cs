using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core.EventSystem;
using Code.World.Player;
using Core;
using UI.Elements;
using UI.LockState;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Profile;

public class ProfilePanel : Panel
{
	public MilMo_Profile profile;

	public UnityEvent onProfileError;

	[SerializeField]
	private List<Toggle> onlyLocalPlayerTabs;

	public override void Open()
	{
		Open(0);
		MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_ToggleProfile");
	}

	public void Open(int playerId)
	{
		OpenProfileAsync(playerId);
		if (profile != null)
		{
			profile.AddTitleChangeListener();
		}
	}

	public override void Close()
	{
		if (profile != null)
		{
			profile.RemoveTitleChangeListener();
		}
		base.Close();
	}

	private async Task OpenProfileAsync(int playerId)
	{
		profile = await MilMo_ProfileManager.GetProfileAsync((playerId == 0) ? MilMo_Player.Instance.Id : playerId.ToString());
		if (profile == null)
		{
			onProfileError?.Invoke();
		}
		else
		{
			if (!Singleton<LockStateManager>.Instance.HasUnlockedProfile)
			{
				return;
			}
			if (base.isActiveAndEnabled)
			{
				Close();
			}
			if (onlyLocalPlayerTabs != null)
			{
				foreach (Toggle onlyLocalPlayerTab in onlyLocalPlayerTabs)
				{
					onlyLocalPlayerTab.gameObject.SetActive(profile.isMe);
				}
			}
			base.Open();
		}
	}
}
