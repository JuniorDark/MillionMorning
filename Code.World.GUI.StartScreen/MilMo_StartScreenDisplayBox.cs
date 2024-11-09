using System.Collections.Generic;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow.Window;
using Code.Core.Items;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.World.CharacterShop;
using Code.World.CharacterShop.RemoteShop;
using Code.World.GUI.Hub;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.World.GUI.StartScreen;

public class MilMo_StartScreenDisplayBox : MilMo_Widget
{
	public delegate void CloseCallback();

	public sealed class HomeOfTheDayBox : MilMo_StartScreenDisplayBox
	{
		public HomeOfTheDayBox(MilMo_UserInterface ui, CloseCallback closeCallback)
			: base(ui, closeCallback)
		{
			base.Caption = MilMo_Localization.GetLocString("StartScreen_13505");
		}

		public void SetHomeOfTheDay(MilMo_HomeOfTheDay home)
		{
			if (home == null)
			{
				return;
			}
			int id = home.OwnerId;
			string name = home.OwnerName;
			MilMo_LocString locString = MilMo_Localization.GetLocString("StartScreen_13504");
			locString.SetFormatArgs(home.HomeName, home.OwnerName);
			_mTextWidget.SetText(locString);
			MilMo_LocString locString2 = MilMo_Localization.GetLocString("Homes_6808");
			locString2.SetFormatArgs(home.OwnerName);
			_mIcon.Tooltip = new MilMo_Tooltip(locString2);
			base.Texture = "Batch01/Textures/Homes/IconMyHome";
			_mIcon.Function = delegate
			{
				if (MilMo_Player.Instance.OkToLeaveHub())
				{
					MilMo_World.Instance.GoToHome(id.ToString(), name);
				}
				if (_mCloseCallback != null)
				{
					_mCloseCallback();
				}
			};
		}
	}

	public sealed class HotItemsBox : MilMo_StartScreenDisplayBox
	{
		private class ItemAndTexture
		{
			public readonly Texture2D Tex;

			public readonly string Identifier;

			public ItemAndTexture(string identifier, Texture2D tex)
			{
				Tex = tex;
				Identifier = identifier;
			}
		}

		private readonly List<ItemAndTexture> _mTextures = new List<ItemAndTexture>();

		private void SetTextures()
		{
			for (int i = 0; i < _mTextures.Count; i++)
			{
				MilMo_Button t = new MilMo_Button(UI);
				t.SetScale(60f, 60f);
				t.SetAlignment(MilMo_GUI.Align.CenterLeft);
				t.SetPosition(i * 65, Scale.y * 0.5f + 21f);
				t.SetAllTextures(_mTextures[i].Tex);
				t.PointerHoverFunction = delegate
				{
					t.ScaleImpulse(1f, 1f);
				};
				t.SetScalePull(0.1f, 0.1f);
				t.SetScaleDrag(0.6f, 0.6f);
				t.Tooltip = new MilMo_Tooltip(MilMo_Localization.GetLocString("Homes_6803"));
				int index = i;
				t.Function = delegate
				{
					if (MilMo_Player.Instance.OkToLeaveHub())
					{
						MilMo_Player.Instance.RequestEnterShop();
						MilMo_CharacterShop.SelectItem(_mTextures[index].Identifier);
					}
					if (_mCloseCallback != null)
					{
						_mCloseCallback();
					}
				};
				AddChild(t);
			}
		}

		public HotItemsBox(MilMo_UserInterface ui, CloseCallback closeCallback)
			: base(ui, closeCallback)
		{
			base.Caption = MilMo_Localization.GetLocString("StartScreen_13503");
			base.Texture = "Batch01/Textures/Core/Invisible";
			MilMo_HotItems.AsyncLoadHotItemsFile(delegate(bool success)
			{
				if (!success)
				{
					return;
				}
				_mTextures.Clear();
				int num = 0;
				foreach (string itemIdentifier in MilMo_HotItems.ItemIdentifiers)
				{
					if (num == 4)
					{
						break;
					}
					Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(itemIdentifier, delegate(MilMo_Template template, bool timeOut)
					{
						if (timeOut)
						{
							LoadAndSetInvisibleIconsAsync(template);
						}
						else
						{
							MilMo_ItemTemplate item = template as MilMo_ItemTemplate;
							if (item == null)
							{
								Debug.Log("Item template is null.");
							}
							else
							{
								item.Instantiate(new Dictionary<string, string>()).AsyncGetIcon(delegate(Texture2D tex)
								{
									if (tex == null)
									{
										Debug.LogError("Unable to load texture for " + item.Identifier);
									}
									else
									{
										_mTextures.Add(new ItemAndTexture(template.Identifier, tex));
										if (_mTextures.Count == 4)
										{
											SetTextures();
										}
									}
								});
							}
						}
					});
					num++;
				}
			});
		}

		private async void LoadAndSetInvisibleIconsAsync(MilMo_Template template)
		{
			Texture2D tex = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/Core/Invisible");
			_mTextures.Add(new ItemAndTexture(template.Identifier, tex));
			if (_mTextures.Count == 4)
			{
				SetTextures();
			}
		}
	}

	public sealed class MiscBox : MilMo_StartScreenDisplayBox
	{
		public MiscBox(MilMo_UserInterface ui, CloseCallback closeCallback, FeaturedNewsItem newsItem)
			: base(ui, closeCallback)
		{
			SetContent(newsItem);
			_mIcon.PointerHoverFunction = delegate
			{
			};
		}

		public void SetContent(FeaturedNewsItem newsItem)
		{
			base.Caption = MilMo_Localization.GetNotLocalizedLocString(newsItem.GetHeadline());
			_mTextWidget.SetText(MilMo_Localization.GetNotLocalizedLocString(newsItem.GetMessage()));
			_mIcon.SetAllTextures(newsItem.GetIconPath(), prefixStandardGuiPath: false);
		}
	}

	private readonly Vector2 _mScale = new Vector2(270f, 80f);

	private readonly MilMo_Widget _mTextWidget;

	private readonly MilMo_Button _mIcon;

	private readonly CloseCallback _mCloseCallback;

	private new string Texture
	{
		set
		{
			_mIcon.SetAllTextures(value);
		}
	}

	private MilMo_LocString Caption
	{
		set
		{
			SetText(value);
		}
	}

	private MilMo_StartScreenDisplayBox(MilMo_UserInterface ui, CloseCallback closeCallback)
		: base(ui)
	{
		_mCloseCallback = closeCallback;
		SetAlignment(MilMo_GUI.Align.TopLeft);
		ScaleNow(_mScale);
		SetTextAlignment(MilMo_GUI.Align.TopCenter);
		SetFont(MilMo_GUI.Font.EborgMedium);
		base.Text = MilMo_Localization.GetLocString("Generic_TEMP");
		SetTextOffset(0f, -3f);
		SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		_mTextWidget = new MilMo_Widget(UI);
		_mTextWidget.SetScale(_mScale);
		_mTextWidget.SetPosition(10f, 30f);
		_mTextWidget.SetFont(MilMo_GUI.Font.EborgSmall);
		_mTextWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mTextWidget.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(_mTextWidget);
		_mIcon = new MilMo_Button(UI);
		_mIcon.SetAlignment(MilMo_GUI.Align.CenterRight);
		_mIcon.SetPosition(265f, Scale.y * 0.5f + 17f);
		_mIcon.SetScale(60f, 60f);
		_mIcon.SetTextureWhite();
		_mIcon.PointerHoverFunction = delegate
		{
			_mIcon.ScaleImpulse(1f, 1f);
		};
		_mIcon.SetScalePull(0.1f, 0.1f);
		_mIcon.SetScaleDrag(0.6f, 0.6f);
		AddChild(_mIcon);
	}

	public override void Draw()
	{
		GUISkin skin = UnityEngine.GUI.skin;
		UnityEngine.GUI.skin = Skin;
		Color currentColor = CurrentColor;
		Rect screenPosition = GetScreenPosition();
		screenPosition.width = _mScale.x;
		screenPosition.height = _mScale.y + 30f;
		UnityEngine.GUI.color = currentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
		UnityEngine.GUI.Box(screenPosition, "");
		UnityEngine.GUI.skin = skin;
		base.Draw();
	}
}
