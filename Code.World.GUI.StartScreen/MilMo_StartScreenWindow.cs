using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.Network.messages.server;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.World.GUI.Hub;
using Code.World.Player;
using UnityEngine;

namespace Code.World.GUI.StartScreen;

public sealed class MilMo_StartScreenWindow : MilMo_HubInfoWindow
{
	private MilMo_StartScreenDisplayBox.HomeOfTheDayBox _homeOfTheDayBox;

	private MilMo_StartScreenDisplayBox.HotItemsBox _hotItemsBox;

	private MilMo_StartScreenDisplayBox.MiscBox _featuredNewsBox;

	private MilMo_Button _newsImage;

	private string _newsImageLink;

	private MilMo_MotdWidget _messageOfTheDay;

	public MilMo_StartScreenWindow(MilMo_UserInterface ui)
		: base(ui, new Vector2(590f, 420f))
	{
		MilMo_EventSystem.Listen("message_of_the_day", UpdateMotd).Repeating = true;
		Init();
	}

	private void Init()
	{
		_messageOfTheDay = new MilMo_MotdWidget(UI, new Vector2(310f, 310f));
		AddChild(_messageOfTheDay);
		_homeOfTheDayBox = new MilMo_StartScreenDisplayBox.HomeOfTheDayBox(UI, Close);
		_homeOfTheDayBox.SetPosition(310f, 22f);
		AddChild(_homeOfTheDayBox);
		_hotItemsBox = new MilMo_StartScreenDisplayBox.HotItemsBox(UI, Close);
		_hotItemsBox.SetPosition(310f, 299f);
		AddChild(_hotItemsBox);
		UpdateNewsContent();
		AddCloseButton();
	}

	public override void Open(float x, float y)
	{
		MilMo_NewsManager.Instance.SetNewsRead();
		_homeOfTheDayBox.SetHomeOfTheDay(MilMo_Hub.Instance.HomeOfTheDay);
		base.Open(x, y);
	}

	public override void Draw()
	{
		UI.BringToFront(this);
		base.Draw();
	}

	private void UpdateNewsContent()
	{
		_messageOfTheDay.RefreshMotd();
		if (MilMo_NewsManager.Instance.FeaturedNewsItem != null)
		{
			if (_featuredNewsBox == null)
			{
				_featuredNewsBox = new MilMo_StartScreenDisplayBox.MiscBox(UI, Close, MilMo_NewsManager.Instance.FeaturedNewsItem);
				AddChild(_featuredNewsBox);
			}
			else
			{
				_featuredNewsBox.SetContent(MilMo_NewsManager.Instance.FeaturedNewsItem);
			}
			_featuredNewsBox.SetPosition(310f, 161f);
			_hotItemsBox.SetPosition(310f, 299f);
		}
		else
		{
			if (_featuredNewsBox != null)
			{
				RemoveChild(_featuredNewsBox);
				_featuredNewsBox = null;
			}
			_hotItemsBox.SetPosition(310f, 161f);
		}
		if (MilMo_NewsManager.Instance.NewsImage != "")
		{
			if (_newsImage == null)
			{
				_newsImage = new MilMo_Button(UI);
				_newsImage.SetAlignment(MilMo_GUI.Align.TopLeft);
				_newsImage.SetPosition(10f, 335f);
				_newsImage.SetScale(270f, 75f);
				_newsImage.SetFadeInSpeed(0.03f);
				AddChild(_newsImage);
			}
			_newsImage.SetAllTextures(MilMo_Localization.GetLocTexturePath(MilMo_NewsManager.Instance.NewsImage), prefixStandardGuiPath: false);
			_newsImageLink = MilMo_NewsManager.Instance.NewsImageLink;
			Debug.Log(_newsImageLink);
			if (!string.IsNullOrEmpty(_newsImageLink))
			{
				_newsImage.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetNotLocalizedLocString(_newsImageLink));
				_newsImage.Function = delegate
				{
					Application.OpenURL(_newsImageLink);
					MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
				};
			}
		}
		else if (_newsImage != null)
		{
			RemoveChild(_newsImage);
			_newsImage = null;
		}
	}

	private void UpdateMotd(object msgAsObj)
	{
		if (msgAsObj is ServerMOTD serverMOTD)
		{
			MilMo_NewsManager.Instance.ReadNews(serverMOTD.getMOTD(), serverMOTD.getNewsImage(), serverMOTD.getNewsImageLink(), serverMOTD.getFeaturedNews());
			UpdateNewsContent();
		}
	}
}
