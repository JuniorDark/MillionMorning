using System.Collections.Generic;
using System.Linq;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Network.types;
using UnityEngine;

namespace Code.World.GUI.LoadingScreen;

public sealed class MilMo_TokenIcons : MilMo_Widget
{
	private readonly int _tokens;

	public MilMo_TokenIcons(MilMo_UserInterface ui, ICollection<ExplorationToken> explorationTokens, IEnumerable<CoinToken> coinTokens)
		: base(ui)
	{
		_tokens = explorationTokens.Count;
		Vector2 explorationIconCenterOffset = MilMo_LoadingScreenConf.ExplorationIconCenterOffset;
		Vector2 position = new Vector2((float)Screen.width / 2f + explorationIconCenterOffset.x, (float)Screen.height / 2f + explorationIconCenterOffset.y);
		position.x -= ((float)explorationTokens.Count * 32f + (float)(explorationTokens.Count - 1) * 5f) / 2f;
		SetAlignment(MilMo_GUI.Align.CenterLeft);
		SetPosition(position);
		SetScale(0f, 0f);
		position = new Vector2(0f, 0f);
		if (explorationTokens.All((ExplorationToken token) => token.GetIsFound() != 0))
		{
			foreach (CoinToken coinToken in coinTokens)
			{
				MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
				string texture = ((coinToken.GetIsFound() == 1) ? "Batch01/Textures/LoadingScreen/IconSilverTokenFound" : "Batch01/Textures/LoadingScreen/IconSilverTokenUnfound");
				milMo_Widget.SetTexture(texture);
				milMo_Widget.SetAlignment(MilMo_GUI.Align.CenterLeft);
				milMo_Widget.SetPosition(position);
				milMo_Widget.SetScale(32f, 32f);
				milMo_Widget.SetAlpha(0f);
				milMo_Widget.AlphaTo(1f);
				milMo_Widget.SetFadeSpeed(0.05f);
				AddChild(milMo_Widget);
				position.x += 37f;
			}
		}
		else
		{
			foreach (ExplorationToken explorationToken in explorationTokens)
			{
				MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
				string texture2 = ((explorationToken.GetIsFound() == 1) ? "Batch01/Textures/LoadingScreen/IconExplorationTokenFound" : "Batch01/Textures/LoadingScreen/IconExplorationTokenUnfound");
				milMo_Widget2.SetTexture(texture2);
				milMo_Widget2.SetAlignment(MilMo_GUI.Align.CenterLeft);
				milMo_Widget2.SetPosition(position);
				milMo_Widget2.SetScale(32f, 32f);
				milMo_Widget2.SetAlpha(0f);
				milMo_Widget2.AlphaTo(1f);
				milMo_Widget2.SetFadeSpeed(0.05f);
				AddChild(milMo_Widget2);
				position.x += 37f;
			}
		}
		IgnoreGlobalFade = true;
	}

	private void RefreshUI()
	{
		Vector2 explorationIconCenterOffset = MilMo_LoadingScreenConf.ExplorationIconCenterOffset;
		Vector2 position = new Vector2((float)Screen.width / 2f + explorationIconCenterOffset.x, (float)Screen.height / 2f + explorationIconCenterOffset.y);
		position.x -= ((float)_tokens * 32f + (float)(_tokens - 1) * 5f) / 2f;
		SetPosition(position);
	}

	public override void Draw()
	{
		if (UI.ScreenSizeDirty)
		{
			RefreshUI();
		}
		base.Draw();
	}
}
