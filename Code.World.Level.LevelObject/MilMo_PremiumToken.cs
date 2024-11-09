using Code.Core.Network;
using Code.Core.Network.types;
using Core;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public class MilMo_PremiumToken : MilMo_LevelToken
{
	public const int DEFAULT_VALUE = 15;

	public const int VALUE_INCREMENT = 5;

	public MilMo_PremiumToken(PremiumToken token)
		: base(token)
	{
		TokenFoundEffectName = "ExplorationTokenFind";
		switch (Mathf.Clamp(token.GetValue() - 15, 0, int.MaxValue) / 5)
		{
		case 0:
			UnFoundMeshVisualRepPath = "Content/Items/Batch01/SpecialItems/PremiumToken";
			break;
		case 1:
			UnFoundMeshVisualRepPath = "Content/Items/Batch01/SpecialItems/PremiumTokenGreen";
			break;
		case 2:
			UnFoundMeshVisualRepPath = "Content/Items/Batch01/SpecialItems/PremiumTokenYellow";
			break;
		default:
			UnFoundMeshVisualRepPath = "Content/Items/Batch01/SpecialItems/PremiumTokenRed";
			break;
		}
		if (!base.IsFound)
		{
			Load();
		}
	}

	protected override void SendFindRequest(Vector3 position)
	{
		Singleton<GameNetwork>.Instance.RequestFindPremiumToken(position);
	}
}
