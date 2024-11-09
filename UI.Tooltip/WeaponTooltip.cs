using Core.Utilities;
using TMPro;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UI.Tooltip;

public class WeaponTooltip : Tooltip
{
	[Header("Elements")]
	[SerializeField]
	private TMP_Text title;

	[SerializeField]
	private TMP_Text description;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private GameObject statsContainer;

	[Header("Prefabs")]
	[SerializeField]
	private AssetReference statPrefab;

	private Stat _stat;

	private void Awake()
	{
		if (!statPrefab.RuntimeKeyIsValid())
		{
			Debug.LogError("Missing stat prefab!");
			return;
		}
		if (_stat == null)
		{
			GameObject gameObject = statPrefab.LoadAssetAsync<GameObject>().WaitForCompletion();
			if (!gameObject)
			{
				Debug.LogError("Failed to load stat prefab!");
				return;
			}
			_stat = gameObject.GetComponent<Stat>();
		}
		if (!statsContainer)
		{
			Debug.LogError("Missing stats container!");
		}
		else
		{
			RemoveStats();
		}
	}

	public override void Show()
	{
		if (!string.IsNullOrEmpty(title.text) && !string.IsNullOrEmpty(description.text))
		{
			base.Show();
		}
	}

	private void RemoveStats()
	{
		for (int i = 0; i < statsContainer.transform.childCount; i++)
		{
			Transform child = statsContainer.transform.GetChild(i);
			if ((bool)child)
			{
				Object.Destroy(child.gameObject);
			}
		}
	}

	public override void SetData(TooltipData data)
	{
		title.text = data.GetTitle();
		if (!statsContainer)
		{
			return;
		}
		RemoveStats();
		WeaponTooltipData weaponTooltipData = (WeaponTooltipData)data;
		description.text = weaponTooltipData.GetDescription();
		Core.Utilities.UI.SetIcon(icon, weaponTooltipData.GetIcon());
		StatData[] stats = weaponTooltipData.GetStats();
		foreach (StatData statData in stats)
		{
			Stat stat = Object.Instantiate(_stat, statsContainer.transform);
			if (!stat)
			{
				break;
			}
			stat.SetText(statData.GetLabel());
			stat.SetValue(statData.GetValue());
		}
	}
}
