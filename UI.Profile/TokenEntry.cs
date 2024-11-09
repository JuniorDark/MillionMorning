using System.Collections.Generic;
using Code.World.Achievements;
using Code.World.Level.LevelObject;
using Core.GameEvent;
using UI.Elements.Slot;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Profile;

public class TokenEntry : MonoBehaviour
{
	[Header("Assets")]
	[SerializeField]
	protected Image rewardIcon;

	[SerializeField]
	protected List<Toggle> tokens;

	private MilMo_ExplorationTokenEntry _entry;

	private int _loadingTween;

	private void Awake()
	{
		if (rewardIcon == null)
		{
			Debug.LogWarning("Missing rewardIcon!");
			base.enabled = false;
		}
		List<Toggle> list = tokens;
		if (list == null || list.Count != 8)
		{
			Debug.LogWarning("Tokens are missing or is not 8!");
			base.enabled = false;
		}
		if (base.enabled)
		{
			rewardIcon.enabled = false;
		}
	}

	public void ClearEntry()
	{
		if (_entry != null)
		{
			SetIcon(null);
			_entry = null;
		}
	}

	public void SetEntry(MilMo_ExplorationTokenEntry entry)
	{
		ClearEntry();
		if (entry != null)
		{
			_entry = entry;
			RefreshFoundTokens();
			if (_entry.GetReward() != null)
			{
				LoadIcon();
			}
		}
	}

	private void RefreshFoundTokens()
	{
		List<bool> foundTokens = _entry.GetFoundTokens();
		List<Toggle> list = tokens;
		if (list != null && list.Count == 8 && foundTokens != null && foundTokens.Count == 8)
		{
			for (int i = 0; i < tokens.Count; i++)
			{
				tokens[i].isOn = foundTokens[i];
			}
		}
	}

	private async void LoadIcon()
	{
		if (!rewardIcon)
		{
			return;
		}
		if (_loadingTween == 0 && (float)LeanTween.tweensRunning < (float)LeanTween.maxSimulataneousTweens * 0.5f)
		{
			_loadingTween = LeanTween.rotateAroundLocal(rewardIcon.gameObject, Vector3.forward, -360f, 2f).setRepeat(999).id;
		}
		rewardIcon.color = new Color(1f, 1f, 1f, 0.4f);
		IEntryItem reward = _entry.GetReward();
		if (reward != null)
		{
			Texture2D texture2D = reward.GetItemIcon();
			if (!texture2D)
			{
				texture2D = await reward.AsyncGetIcon();
			}
			SetIcon(texture2D);
		}
	}

	private void SetIcon(Texture2D newTexture)
	{
		if (rewardIcon == null)
		{
			return;
		}
		if (_loadingTween != 0)
		{
			LeanTween.cancel(_loadingTween);
			_loadingTween = 0;
		}
		rewardIcon.transform.localRotation = Quaternion.identity;
		if (!newTexture)
		{
			rewardIcon.enabled = false;
			return;
		}
		if (!rewardIcon.sprite || rewardIcon.sprite.texture != newTexture)
		{
			Vector2 pivot = rewardIcon.rectTransform.pivot;
			Rect rect = new Rect(0f, 0f, newTexture.width, newTexture.height);
			Sprite sprite = Sprite.Create(newTexture, rect, pivot);
			rewardIcon.sprite = sprite;
		}
		rewardIcon.color = ((!_entry.IsFinished()) ? new Color(0f, 0f, 0f, 0.5f) : Color.white);
		rewardIcon.enabled = true;
	}

	private Texture2D GetIcon()
	{
		if (!rewardIcon || !rewardIcon.sprite || !rewardIcon.sprite.texture)
		{
			return null;
		}
		return rewardIcon.sprite.texture;
	}

	public async void ShowTooltip()
	{
		if (!_entry.IsFinished())
		{
			MilMo_Medal medal = _entry.GetMedal();
			TooltipData args = new TooltipData((medal == null) ? "" : medal.Template?.NotCompleteDescription?.String);
			GameEvent.ShowTooltipEvent?.RaiseEvent(args);
			return;
		}
		IEntryItem reward = _entry.GetReward();
		Texture2D icon = GetIcon();
		if (reward != null)
		{
			TooltipData args2 = await new CreateTooltipDataHandler(reward, icon).GetTooltipData();
			GameEvent.ShowTooltipEvent?.RaiseEvent(args2);
		}
	}

	public void HideTooltip()
	{
		GameEvent.HideTooltipEvent?.RaiseEvent();
	}

	public bool IsValid()
	{
		return _entry?.GetReward() != null;
	}
}
