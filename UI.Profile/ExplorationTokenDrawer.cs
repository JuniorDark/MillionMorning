using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.World.Level.LevelInfo;
using Code.World.Level.LevelObject;
using Code.World.Player;
using UI.Elements.Drawer;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.Profile;

public class ExplorationTokenDrawer : Drawer
{
	[Header("Tokens")]
	[SerializeField]
	protected AssetReference tokenEntryPrefab;

	private GameObject _tokenEntryObject;

	private ProfilePanel _profilePanel;

	private MilMo_Profile _profile;

	protected override void Awake()
	{
		base.Awake();
		if (!tokenEntryPrefab.RuntimeKeyIsValid())
		{
			Debug.LogError(base.gameObject.name + ": Missing token entry prefab!");
			return;
		}
		_tokenEntryObject = tokenEntryPrefab.LoadAssetAsync<GameObject>().WaitForCompletion();
		if (!_tokenEntryObject)
		{
			Debug.LogError(base.gameObject.name + ": Could not load token entry prefab!");
			return;
		}
		_profilePanel = GetComponentInParent<ProfilePanel>();
		if (!_profilePanel)
		{
			Debug.LogWarning(base.gameObject.name + ": Could not find ProfilePanel");
		}
	}

	protected override void OnEnable()
	{
		MilMo_Profile profile = _profile;
		_profile = ((_profilePanel != null) ? _profilePanel.profile : null);
		if (profile == null || _profile == null || profile.playerId != _profile.playerId)
		{
			Redraw();
		}
		base.OnEnable();
	}

	private void Redraw()
	{
		DestroyContent();
		GetExplorationTokenEntries(out var entries);
		AddExplorationTokenEntries(entries);
	}

	protected override string GetSectionLocaleKey(Enum sectionIdentifier)
	{
		throw new NotImplementedException();
	}

	private void AddExplorationTokenEntries(IReadOnlyList<MilMo_ExplorationTokenEntry> entries)
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(AddExplorationTokenEntriesStep(entries));
			return;
		}
		foreach (MilMo_ExplorationTokenEntry entry in entries)
		{
			AddExplorationTokenEntry(entry);
		}
	}

	private IEnumerator AddExplorationTokenEntriesStep(IReadOnlyList<MilMo_ExplorationTokenEntry> entries)
	{
		List<MilMo_ExplorationTokenEntry> toAdd = entries.ToList();
		int i = 0;
		int num = 0;
		while (i < toAdd.Count)
		{
			if (num > 10)
			{
				yield return new WaitForFixedUpdate();
				num = 0;
			}
			AddExplorationTokenEntry(entries[i]);
			i++;
			num++;
		}
		StartCoroutine(RefreshUpdatedSections());
	}

	private void AddExplorationTokenEntry(MilMo_ExplorationTokenEntry explorationTokenEntry)
	{
		Section section = CreateSection();
		if (section == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Could not create token section " + explorationTokenEntry.Level.FullLevelName + "!");
			return;
		}
		section.SetLabelLocaleKey(explorationTokenEntry.GetLevelDisplayNameIdentifier());
		TokenEntry tokenEntry = CreateTokenEntry();
		if (tokenEntry == null)
		{
			Debug.LogWarning("Unable to create token entry");
			UnityEngine.Object.Destroy(section.gameObject);
			return;
		}
		tokenEntry.SetEntry(explorationTokenEntry);
		if (!tokenEntry.IsValid())
		{
			UnityEngine.Object.Destroy(tokenEntry.gameObject);
			UnityEngine.Object.Destroy(section.gameObject);
		}
		else
		{
			section.AddEntry(tokenEntry);
		}
	}

	private TokenEntry CreateTokenEntry()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(_tokenEntryObject, base.transform);
		if (!gameObject)
		{
			Debug.LogWarning(base.gameObject.name + ": Unable to instantiate Token Entry!");
			return null;
		}
		TokenEntry component = gameObject.GetComponent<TokenEntry>();
		if (!component)
		{
			Debug.LogWarning(base.gameObject.name + ": Unable to GetComponent TokenEntry!");
			return null;
		}
		gameObject.SetActive(base.gameObject.activeInHierarchy);
		return component;
	}

	private void GetExplorationTokenEntries(out List<MilMo_ExplorationTokenEntry> entries)
	{
		entries = new List<MilMo_ExplorationTokenEntry>();
		if (_profile?.ExplorationTokens == null || _profile.ExplorationTokens.Count < 1)
		{
			return;
		}
		foreach (KeyValuePair<string, List<bool>> explorationToken in _profile.ExplorationTokens)
		{
			string key = explorationToken.Key;
			List<bool> value = explorationToken.Value;
			MilMo_LevelInfoData levelInfoData = MilMo_LevelInfo.GetLevelInfoData(key);
			if (levelInfoData != null)
			{
				MilMo_ExplorationTokenEntry milMo_ExplorationTokenEntry = new MilMo_ExplorationTokenEntry();
				milMo_ExplorationTokenEntry.Level = levelInfoData;
				milMo_ExplorationTokenEntry.FoundTokens = value;
				entries.Add(milMo_ExplorationTokenEntry);
			}
		}
	}
}
