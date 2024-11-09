using Code.Core.Network;
using Code.Core.Network.types;
using Core;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public class MilMo_CoinToken : MilMo_LevelToken
{
	private readonly sbyte _index;

	public MilMo_CoinToken(CoinToken token, sbyte index)
		: base(token)
	{
		_index = index;
		TokenFoundEffectName = "CoinTokenFind";
		TokenFoundRemotePlayerEffectName = "";
		UnFoundMeshVisualRepPath = "Content/Items/Batch01/SpecialItems/SilverToken";
		FoundMeshVisualRepPath = "Content/Items/Batch01/SpecialItems/ExplorationTokenGhost";
		if (!base.IsFound)
		{
			Load();
		}
	}

	protected override void SendFindRequest(Vector3 position)
	{
		Singleton<GameNetwork>.Instance.RequestFindCoinToken(_index, position);
	}
}
