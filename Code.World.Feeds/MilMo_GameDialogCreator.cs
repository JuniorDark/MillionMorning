using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core.Command;
using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.Items;
using Code.Core.Items.Home;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Utility;
using Code.World.GUI;
using Code.World.GUI.FriendInvites;
using Code.World.GUI.GameDialog;
using Code.World.Level;
using Code.World.Level.LevelInfo;
using Code.World.Player;
using Core;
using UI.HUD.Dialogues;
using UnityEngine;

namespace Code.World.Feeds;

public abstract class MilMo_GameDialogCreator
{
	protected class OpenDialog
	{
		public readonly MilMo_GameDialog Dialog;

		public readonly MilMo_GameDialogCreator DialogCreator;

		public bool IsClosed;

		public OpenDialog(MilMo_GameDialog dialog, MilMo_GameDialogCreator dialogCreator)
		{
			Dialog = dialog;
			DialogCreator = dialogCreator;
		}
	}

	private static readonly Dictionary<MilMo_UserInterface, List<MilMo_GameDialogCreator>> DialogQueues = new Dictionary<MilMo_UserInterface, List<MilMo_GameDialogCreator>>();

	private static readonly Dictionary<MilMo_UserInterface, List<MilMo_EventSystem.MilMo_Callback>> LastDialogClosedCallbacks = new Dictionary<MilMo_UserInterface, List<MilMo_EventSystem.MilMo_Callback>>();

	protected static readonly Dictionary<MilMo_UserInterface, OpenDialog> TheDialogs = new Dictionary<MilMo_UserInterface, OpenDialog>();

	protected MilMo_LocString FeedEventIngame;

	protected MilMo_LocString FeedDescriptionIngame;

	protected MilMo_LocString ObjectName;

	protected string IconPathIngame;

	protected string CustomJinglePath = "";

	protected readonly MilMo_UserInterface UI;

	protected MilMo_GameDialogCreator(MilMo_UserInterface ui)
	{
		UI = ui ?? throw new NullReferenceException("User interface is null when creating game dialog");
	}

	public static bool IsShowingStuff()
	{
		return TheDialogs.Any(delegate(KeyValuePair<MilMo_UserInterface, OpenDialog> dialog)
		{
			OpenDialog value = dialog.Value;
			return value != null && !value.IsClosed;
		});
	}

	private static async void AddDialogue(MilMo_UserInterface instanceUI, MilMo_GameDialogCreator dialogCreator)
	{
		await Task.Delay(200);
		if (MilMo_Player.Instance.InDialogue)
		{
			MilMo_Player.Instance.OnStopTalkingWithNPC += delegate
			{
				AddDialogue(instanceUI, dialogCreator);
			};
			return;
		}
		if (!TheDialogs.ContainsKey(instanceUI) || TheDialogs[instanceUI] == null)
		{
			dialogCreator.CreateDialog();
			return;
		}
		if (!DialogQueues.ContainsKey(instanceUI))
		{
			DialogQueues.Add(instanceUI, new List<MilMo_GameDialogCreator>());
		}
		DialogQueues[instanceUI].Add(dialogCreator);
	}

	protected abstract void CreateDialog();

	public static void CloseAll()
	{
		if (TheDialogs == null || TheDialogs.Count < 1)
		{
			return;
		}
		foreach (OpenDialog value in TheDialogs.Values)
		{
			value?.DialogCreator?.CloseAndRemove();
		}
	}

	public void CloseAndRemove()
	{
		CloseDialog(UI);
	}

	protected virtual void CloseDialog(object o)
	{
		if (UI == null || !TheDialogs.ContainsKey(UI) || TheDialogs[UI] == null || TheDialogs[UI].IsClosed)
		{
			return;
		}
		TheDialogs[UI].Dialog.Hide();
		TheDialogs[UI].IsClosed = true;
		MilMo_EventSystem.At(0.375f, delegate
		{
			TheDialogs[UI] = null;
			if (DialogQueues.ContainsKey(UI) && DialogQueues[UI].Count > 0)
			{
				MilMo_GameDialogCreator milMo_GameDialogCreator = DialogQueues[UI][0];
				DialogQueues[UI].RemoveAt(0);
				milMo_GameDialogCreator.CreateDialog();
			}
			else if (LastDialogClosedCallbacks.ContainsKey(UI))
			{
				foreach (MilMo_EventSystem.MilMo_Callback item in LastDialogClosedCallbacks[UI])
				{
					item();
				}
				LastDialogClosedCallbacks[UI].Clear();
			}
		});
	}

	public static void CreateListeners()
	{
		MilMo_EventSystem.Listen("notification", Notification).Repeating = true;
		MilMo_EventSystem.Listen("box_opened", BoxOpened).Repeating = true;
		MilMo_EventSystem.Listen("converter_used", ConverterUsed).Repeating = true;
		MilMo_EventSystem.Listen("request_notifications", delegate
		{
			RequestNotifications();
		}).Repeating = true;
		MilMo_EventSystem.Listen("received_invite_rewards", ReceivedInviteReward).Repeating = true;
		MilMo_Command.Instance.RegisterCommand("Notifications.Request", Debug_RequestNotifications);
		MilMo_Command.Instance.RegisterCommand("GameDialog.SpawnBecameMember", Debug_SpawnBecameMemberDialog);
	}

	public static bool RequestNotifications()
	{
		return Singleton<GameNetwork>.Instance.RequestNotifications();
	}

	public static void AddLastDialogClosedCallback(MilMo_UserInterface ui, bool callImmediatelyIfNoDialog, MilMo_EventSystem.MilMo_Callback callback)
	{
		if (callback == null)
		{
			return;
		}
		if (callImmediatelyIfNoDialog && (!TheDialogs.ContainsKey(ui) || TheDialogs[ui] == null) && (!DialogQueues.ContainsKey(ui) || DialogQueues[ui].Count == 0))
		{
			callback();
			return;
		}
		if (!LastDialogClosedCallbacks.ContainsKey(ui))
		{
			LastDialogClosedCallbacks.Add(ui, new List<MilMo_EventSystem.MilMo_Callback>());
		}
		LastDialogClosedCallbacks[ui].Add(callback);
	}

	private static void Notification(object notificationAsObject)
	{
		MilMo_UserInterface ui = MilMo_GlobalUI.GetSystemUI;
		if (ui == null)
		{
			Debug.LogWarning("Failed to find system UI when creating notification game dialog.");
			return;
		}
		ServerNewNotification notification = notificationAsObject as ServerNewNotification;
		if (notification == null)
		{
			return;
		}
		string notificationType = notification.getNotificationType();
		Debug.Log("Got Notification: " + notificationType);
		Debug.Log("Custom data: " + notification.getNotificationCustomData());
		switch (notificationType)
		{
		case "memberitem":
		case "subscribeitem":
		case "monthlymemberitem":
		case "monthlysubscribeitem":
		{
			string[] customData = notification.getNotificationCustomData().Split(';');
			if (customData.Length != 4)
			{
				Debug.LogWarning("Got invalid custom data for notification " + notification.getNotificationId());
				break;
			}
			Debug.Log("Item check: " + MilMo_Utility.StringToInt(customData[1]));
			Singleton<GameNetwork>.Instance.SendItemCheckRequest(MilMo_Utility.StringToInt(customData[1]));
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(customData[0], delegate(MilMo_Template template, bool timeout)
			{
				MilMo_ItemTemplate milMo_ItemTemplate = template as MilMo_ItemTemplate;
				if (timeout || milMo_ItemTemplate == null)
				{
					Debug.LogWarning("Failed to show notification gamedialog for item: " + notification.getNotificationCustomData());
				}
				else
				{
					MilMo_GameDialogCreatorItemNotification dialogCreator4 = new MilMo_GameDialogCreatorItemNotification(notificationType, milMo_ItemTemplate, MilMo_Utility.StringToInt(customData[2]), customData[3], ui);
					AddDialogue(ui, dialogCreator4);
					Singleton<GameNetwork>.Instance.SendNotificationRead(notification.getNotificationId());
				}
			});
			break;
		}
		case "becamemember":
		case "membershipextended":
		{
			MilMo_GameDialogCreatorBecameMember dialogCreator2 = new MilMo_GameDialogCreatorBecameMember(MilMo_Utility.StringToInt(notification.getNotificationCustomData()), notificationType == "membershipextended", ui);
			AddDialogue(ui, dialogCreator2);
			Singleton<GameNetwork>.Instance.SendNotificationRead(notification.getNotificationId());
			break;
		}
		case "giftitem":
		{
			string[] customData = notification.getNotificationCustomData().Split(';');
			if (customData.Length != 6)
			{
				Debug.LogWarning("Got invalid custom data for gift item notification " + notification.getNotificationId());
				break;
			}
			if (!int.TryParse(customData[5], out var amount))
			{
				Debug.LogWarning("Failed to parse Amount in gift notification.");
				amount = 1;
			}
			MilMo_Player.Instance.Inventory.GiftReceived(customData[3]);
			if (customData[0].Equals("ammo", StringComparison.InvariantCultureIgnoreCase))
			{
				Singleton<GameNetwork>.Instance.SendAmmoCheckRequest();
			}
			else
			{
				Singleton<GameNetwork>.Instance.SendItemCheckRequest(MilMo_Utility.StringToInt(customData[0]));
			}
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(customData[3], delegate(MilMo_Template template, bool timeout)
			{
				MilMo_ItemTemplate milMo_ItemTemplate2 = template as MilMo_ItemTemplate;
				if (timeout || milMo_ItemTemplate2 == null)
				{
					Debug.LogWarning("Failed to show gift notification gamedialog for item: " + notification.getNotificationCustomData());
				}
				else
				{
					if (milMo_ItemTemplate2 is MilMo_HomeEquipmentTemplate && MilMo_Player.InMyHome)
					{
						Singleton<GameNetwork>.Instance.RequestCheckForNewHomeItems();
					}
					MilMo_GameDialogCreatorGiftItem dialogCreator5 = new MilMo_GameDialogCreatorGiftItem(milMo_ItemTemplate2, customData[4], customData[2], amount, ui);
					AddDialogue(ui, dialogCreator5);
					Singleton<GameNetwork>.Instance.SendNotificationRead(notification.getNotificationId());
				}
			});
			break;
		}
		case "gifttelepods":
		{
			string[] array2 = notification.getNotificationCustomData().Split(';');
			if (array2.Length != 3)
			{
				Debug.LogWarning("Got invalid custom data for gift telepods notification " + notification.getNotificationId());
				break;
			}
			Singleton<GameNetwork>.Instance.RequestTelepods();
			MilMo_GameDialogCreatorGiftTelepods dialogCreator3 = new MilMo_GameDialogCreatorGiftTelepods(int.Parse(array2[2]), array2[1], ui);
			AddDialogue(ui, dialogCreator3);
			Singleton<GameNetwork>.Instance.SendNotificationRead(notification.getNotificationId());
			break;
		}
		case "giftmembership":
		case "giftmembershipextended":
		{
			string[] array = notification.getNotificationCustomData().Split(';');
			if (array.Length != 3)
			{
				Debug.LogWarning("Got invalid custom data for gift membership notification " + notification.getNotificationId());
				break;
			}
			Singleton<GameNetwork>.Instance.RequestBecameMemberVerification();
			MilMo_GameDialogCreatorGiftMembership dialogCreator = new MilMo_GameDialogCreatorGiftMembership(array[1], int.Parse(array[2]), notificationType == "giftmembershipextended", ui);
			AddDialogue(ui, dialogCreator);
			Singleton<GameNetwork>.Instance.SendNotificationRead(notification.getNotificationId());
			break;
		}
		}
	}

	private static void ConverterUsed(object msgAsObj)
	{
		ServerConverterUsed message = msgAsObj as ServerConverterUsed;
		if (message == null)
		{
			return;
		}
		MilMo_Item.AsyncGetItem(message.getConverterIdentifier(), delegate(MilMo_Item cItem)
		{
			MilMo_Converter converter = cItem as MilMo_Converter;
			if (converter != null)
			{
				MilMo_Item.AsyncGetItem(message.getItemIdentifier(), delegate(MilMo_Item item)
				{
					MilMo_UserInterface ui = (MilMo_Player.Instance.InShop ? MilMo_GlobalUI.GetSystemUI : MilMo_World.Instance.UI);
					if (ui == null)
					{
						Debug.LogWarning("Failed to find correct UI when creating BoxOpened game dialog. (InShop: " + MilMo_Player.Instance.InShop + ")");
					}
					else
					{
						converter.AsyncGetIcon(delegate(Texture2D texture)
						{
							int itemCount = 0;
							CreateImmediately(new MilMo_GameDialogCreatorConverter(new MilMo_GameDialogConverter.Converter(new MilMo_Texture(texture), converter.Template.OpenTexture, item, itemCount), ui), ui);
						});
					}
				});
			}
		});
	}

	private static async void BoxOpened(object msgAsObj)
	{
		if (!(msgAsObj is ServerBoxOpened msg))
		{
			return;
		}
		if (!(await Singleton<MilMo_TemplateContainer>.Instance.GetTemplateAsync(msg.getBoxTemplate().GetIdentifier()) is IMilMo_OpenableBox boxTemplate))
		{
			Debug.LogWarning("boxTemplate is not an openable box");
			return;
		}
		int templatesLoaded = 0;
		List<MilMo_BoxLoot> receivedItems = new List<MilMo_BoxLoot>();
		for (int i = 0; i < msg.getLoot().Count; i++)
		{
			BoxLoot receivedItemStruct = msg.getLoot()[i];
			if (await Singleton<MilMo_TemplateContainer>.Instance.GetTemplateAsync(receivedItemStruct.GetTemplate().GetIdentifier()) is MilMo_ItemTemplate milMo_ItemTemplate)
			{
				MilMo_BoxLoot item = new MilMo_BoxLoot
				{
					ItemTemplate = milMo_ItemTemplate,
					Amount = receivedItemStruct.GetAmount(),
					Item = milMo_ItemTemplate.Instantiate(new Dictionary<string, string>())
				};
				receivedItems.Add(item);
				templatesLoaded++;
			}
		}
		if (templatesLoaded != msg.getLoot().Count || receivedItems.Count <= 0)
		{
			Debug.LogWarning("Loot count miss match");
		}
		else
		{
			DialogueSpawner.SpawnOpenableBoxDialogue(receivedItems, boxTemplate);
		}
	}

	private static void ReceivedInviteReward(object msgAsObj)
	{
		ServerInviteItemReceived msg = msgAsObj as ServerInviteItemReceived;
		if (msg == null)
		{
			return;
		}
		MilMoFriendInviteDialog.GetInstance().UpdateValues(msg.getTotalInvites(), msg.getTotalAccepts());
		if (MilMo_World.Instance == null || MilMo_World.Instance.UI == null)
		{
			MilMo_EventSystem.Listen("world_created", delegate
			{
				ReceivedInviteReward(msgAsObj);
			});
			return;
		}
		if (msg.getNextAcceptItems() == null)
		{
			ReceivedInviteReward(msg.getTotalAccepts(), msg.getAcceptItems(), null, 0, 0, isAcceptedRewards: true);
		}
		else
		{
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(msg.getNextAcceptItems().GetItem().GetTemplate(), delegate(MilMo_Template template, bool timeOut)
			{
				if (!(template is MilMo_ItemTemplate milMo_ItemTemplate2))
				{
					Debug.LogWarning("Failed to load next intvite reward template " + msg.getNextAcceptItems().GetItem().GetTemplate()
						.GetPath());
					ReceivedInviteReward(msg.getTotalAccepts(), msg.getAcceptItems(), null, 0, 0, isAcceptedRewards: true);
				}
				else
				{
					MilMo_Item nextInviteReward2 = milMo_ItemTemplate2.Instantiate(MilMo_Item.ReadModifiers(msg.getNextAcceptItems().GetItem().GetModifiers()));
					ReceivedInviteReward(msg.getTotalAccepts(), msg.getAcceptItems(), nextInviteReward2, msg.getNextAcceptItems().GetAmount(), msg.getNextAcceptItems().GetInvitesNeeded(), isAcceptedRewards: true);
				}
			});
		}
		if (msg.getNextInviteItems() == null)
		{
			ReceivedInviteReward(msg.getTotalInvites(), msg.getInviteItems(), null, 0, 0, isAcceptedRewards: false);
			return;
		}
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(msg.getNextInviteItems().GetItem().GetTemplate(), delegate(MilMo_Template template, bool timeOut)
		{
			if (!(template is MilMo_ItemTemplate milMo_ItemTemplate))
			{
				Debug.LogWarning("Failed to load next invite reward template " + msg.getNextInviteItems().GetItem().GetTemplate()
					.GetPath());
				ReceivedInviteReward(msg.getTotalInvites(), msg.getInviteItems(), null, 0, 0, isAcceptedRewards: false);
			}
			else
			{
				MilMo_Item nextInviteReward = milMo_ItemTemplate.Instantiate(MilMo_Item.ReadModifiers(msg.getNextInviteItems().GetItem().GetModifiers()));
				ReceivedInviteReward(msg.getTotalInvites(), msg.getInviteItems(), nextInviteReward, msg.getNextInviteItems().GetAmount(), msg.getNextInviteItems().GetInvitesNeeded(), isAcceptedRewards: false);
			}
		});
	}

	private static void ReceivedInviteReward(int totalInvites, IList<InviteItem> receivedItems, MilMo_Item nextInviteReward, int nextRewardAmount, int nextRewardInviteCount, bool isAcceptedRewards)
	{
		MilMo_GameDialogInvite.SetNextRewardInfo(totalInvites, nextRewardInviteCount, nextInviteReward, nextRewardAmount);
		foreach (InviteItem receivedItem in receivedItems)
		{
			InviteItem item = receivedItem;
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(item.GetItem().GetTemplate(), delegate(MilMo_Template template, bool timeOut)
			{
				if (!(template is MilMo_ItemTemplate milMo_ItemTemplate))
				{
					Debug.LogWarning("Failed to load template for invite reward " + item.GetItem().GetTemplate().GetPath());
				}
				else
				{
					MilMo_Item reward = milMo_ItemTemplate.Instantiate(MilMo_Item.ReadModifiers(item.GetItem().GetModifiers()));
					MilMo_GameDialogCreatorInviteReward dialogCreator = new MilMo_GameDialogCreatorInviteReward(totalInvites, reward, item.GetAmount(), nextRewardInviteCount, nextInviteReward, nextRewardAmount, isAcceptedRewards);
					AddDialogue(MilMo_World.Instance.UI, dialogCreator);
				}
			});
		}
	}

	public static void LevelUnlocked(MilMo_Level level, MilMo_UserInterface ui, MilMo_EventSystem.MilMo_Callback closeCallback)
	{
		if (level == null)
		{
			Debug.LogWarning("Failed to create game dialog for \"level unlocked\"-event: got null level or wrong object type");
			return;
		}
		MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(level.VerboseName);
		if (levelInfoData == null)
		{
			Debug.LogWarning("Failed to create game dialog for \"level unlocked\"-event: no level info found for " + level.VerboseName);
			return;
		}
		string text = "LevelIcon" + level.WorldContentName + level.LevelContentName;
		string iconPathIngame = "Content/Worlds/" + level.WorldContentName + "/LevelIcons/" + text;
		MilMo_GameDialogCreatorLevel dialogCreator = new MilMo_GameDialogCreatorLevel(levelInfoData, iconPathIngame, ui, closeCallback);
		AddDialogue(MilMo_World.Instance.UI, dialogCreator);
	}

	private static void CreateImmediately(MilMo_GameDialogCreator dialogCreator, MilMo_UserInterface ui)
	{
		if (!TheDialogs.ContainsKey(ui) || TheDialogs[ui] == null)
		{
			dialogCreator.CreateDialog();
			return;
		}
		OpenDialog openDialog = TheDialogs[ui];
		if (!DialogQueues.ContainsKey(ui))
		{
			DialogQueues.Add(ui, new List<MilMo_GameDialogCreator>());
		}
		DialogQueues[ui].Insert(0, openDialog.DialogCreator);
		DialogQueues[ui].Insert(0, dialogCreator);
		openDialog.DialogCreator.CloseDialog(null);
	}

	private static string Debug_RequestNotifications(string[] args)
	{
		if (!RequestNotifications())
		{
			return "Failed to request notifications";
		}
		return "Notifications requested";
	}

	private static string Debug_SpawnBecameMemberDialog(string[] args)
	{
		if (args.Length < 2)
		{
			return "usage GameDialog.SpawnBecameMember <numberOfDays> <[optional]wasExtended:1|0>";
		}
		bool wasExtended = false;
		if (args.Length > 2)
		{
			wasExtended = int.Parse(args[2]) == 1;
		}
		MilMo_GameDialogCreatorBecameMember dialogCreator = new MilMo_GameDialogCreatorBecameMember(MilMo_Utility.StringToInt(args[1]), wasExtended, MilMo_World.Instance.UI);
		AddDialogue(MilMo_World.Instance.UI, dialogCreator);
		return "";
	}
}
