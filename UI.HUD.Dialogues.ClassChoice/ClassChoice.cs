using System.Collections.Generic;
using Core.Utilities;
using UI.HUD.Dialogues.ClassChoice.Abilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.HUD.Dialogues.ClassChoice;

public class ClassChoice : MonoBehaviour
{
	[SerializeField]
	private AssetReference prefab;

	[SerializeField]
	private Transform rewardContainer;

	private ClassAbility[] _abilities;

	private readonly List<RewardAbility> _rewards = new List<RewardAbility>();

	public void Init(ClassAbility[] abilities)
	{
		_abilities = abilities;
		for (int i = 0; i < rewardContainer.childCount; i++)
		{
			Transform child = rewardContainer.GetChild(i);
			if ((bool)child)
			{
				Object.Destroy(child.gameObject);
			}
		}
		ClassAbility[] abilities2 = _abilities;
		foreach (ClassAbility newAbility in abilities2)
		{
			RewardAbility rewardAbility = Instantiator.Instantiate<RewardAbility>(prefab, rewardContainer);
			rewardAbility.Init(newAbility);
			_rewards.Add(rewardAbility);
		}
	}

	private void OnDestroy()
	{
	}
}
