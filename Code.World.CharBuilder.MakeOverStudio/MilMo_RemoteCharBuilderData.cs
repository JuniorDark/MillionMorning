using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core.EventSystem;
using Code.Core.Items;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.Template;
using Core;
using UnityEngine;

namespace Code.World.CharBuilder.MakeOverStudio;

public class MilMo_RemoteCharBuilderData : MilMo_CharBuilderData
{
	private MilMo_GenericReaction _requestReaction;

	private ServerCharbuilderData _data;

	private bool _waitingForData;

	public override async Task<bool> LoadDataAsync()
	{
		_waitingForData = true;
		_requestReaction = MilMo_EventSystem.Listen("charbuilder_data", ReceivedCharBuilderData);
		Singleton<GameNetwork>.Instance.RequestCharBuilderData();
		ServerCharbuilderData serverCharbuilderData = await WaitForData();
		MilMo_EventSystem.RemoveReaction(_requestReaction);
		_requestReaction = null;
		if (serverCharbuilderData == null)
		{
			return false;
		}
		LoadCharbuilderData(serverCharbuilderData);
		return true;
	}

	private async Task<ServerCharbuilderData> WaitForData()
	{
		for (int time = 0; time < 40000; time += 100)
		{
			if (!_waitingForData)
			{
				if (_data == null)
				{
					Debug.LogError("ServerCharbuilderData is null!");
				}
				return _data;
			}
			await Task.Delay(100);
		}
		Debug.LogError("ServerCharbuilderData timeout!");
		return null;
	}

	private void ReceivedCharBuilderData(object msgAsObject)
	{
		_data = msgAsObject as ServerCharbuilderData;
		_waitingForData = false;
	}

	private void LoadCharbuilderData(ServerCharbuilderData msg)
	{
		SkinColors.Clear();
		SkinColors.AddRange(msg.getSkinColors());
		HairColors.Clear();
		HairColors.AddRange(msg.getHairColors());
		EyeColors.Clear();
		EyeColors.AddRange(msg.getEyeColors());
		BoyMouths.Clear();
		BoyMouths.AddRange(msg.getBoyMouths());
		BoyEyes.Clear();
		BoyEyes.AddRange(msg.getBoyEyes());
		BoyEyeBrows.Clear();
		BoyEyeBrows.AddRange(msg.getBoyEyeBrows());
		GirlMouths.Clear();
		GirlMouths.AddRange(msg.getGirlMouths());
		GirlEyes.Clear();
		GirlEyes.AddRange(msg.getGirlEyes());
		GirlEyeBrows.Clear();
		GirlEyeBrows.AddRange(msg.getGirlEyeBrows());
		foreach (Template boyShirt in msg.getBoyShirts())
		{
			MilMo_Wearable milMo_Wearable = LoadWearable(boyShirt);
			if (milMo_Wearable != null)
			{
				BoyShirtItems.Add(milMo_Wearable);
			}
		}
		foreach (Template boyPant in msg.getBoyPants())
		{
			MilMo_Wearable milMo_Wearable2 = LoadWearable(boyPant);
			if (milMo_Wearable2 != null)
			{
				BoyPantsItems.Add(milMo_Wearable2);
			}
		}
		foreach (Template boyShoe in msg.getBoyShoes())
		{
			MilMo_Wearable milMo_Wearable3 = LoadWearable(boyShoe);
			if (milMo_Wearable3 != null)
			{
				BoyShoesItems.Add(milMo_Wearable3);
			}
		}
		foreach (Template boyHair in msg.getBoyHairs())
		{
			MilMo_Wearable milMo_Wearable4 = LoadWearable(boyHair);
			if (milMo_Wearable4 != null)
			{
				BoyHairStyleItems.Add(milMo_Wearable4);
			}
		}
		foreach (Template girlShirt in msg.getGirlShirts())
		{
			MilMo_Wearable milMo_Wearable5 = LoadWearable(girlShirt);
			if (milMo_Wearable5 != null)
			{
				GirlShirtItems.Add(milMo_Wearable5);
			}
		}
		foreach (Template girlPant in msg.getGirlPants())
		{
			MilMo_Wearable milMo_Wearable6 = LoadWearable(girlPant);
			if (milMo_Wearable6 != null)
			{
				GirlPantsItems.Add(milMo_Wearable6);
			}
		}
		foreach (Template girlShoe in msg.getGirlShoes())
		{
			MilMo_Wearable milMo_Wearable7 = LoadWearable(girlShoe);
			if (milMo_Wearable7 != null)
			{
				GirlShoesItems.Add(milMo_Wearable7);
			}
		}
		foreach (Template girlHair in msg.getGirlHairs())
		{
			MilMo_Wearable milMo_Wearable8 = LoadWearable(girlHair);
			if (milMo_Wearable8 != null)
			{
				GirlHairStyleItems.Add(milMo_Wearable8);
			}
		}
	}

	private MilMo_Wearable LoadWearable(Template template)
	{
		if (!(Singleton<MilMo_TemplateContainer>.Instance.LoadTemplateFromNetworkMessage(template) is MilMo_WearableTemplate milMo_WearableTemplate))
		{
			return null;
		}
		return milMo_WearableTemplate.Instantiate(new Dictionary<string, string>()) as MilMo_Wearable;
	}
}
