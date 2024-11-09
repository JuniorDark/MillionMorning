using TMPro;
using UnityEngine;

namespace UI.Elements;

public abstract class DropdownElement : MonoBehaviour
{
	[SerializeField]
	protected TMP_Dropdown dropdown;

	protected int SelectedDropdownIndex;

	protected int LastDropdownIndex;

	private void OnEnable()
	{
		if (!dropdown)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing dropdown!");
			return;
		}
		CreateDropdown();
		UpdateLast();
	}

	protected abstract void CreateDropdown();

	protected abstract void SetSettings();

	private void UpdateLast()
	{
		LastDropdownIndex = SelectedDropdownIndex;
	}

	public void Apply()
	{
		UpdateLast();
		SetSettings();
	}

	public void Revert()
	{
		SelectedDropdownIndex = LastDropdownIndex;
		dropdown.value = SelectedDropdownIndex;
		dropdown.RefreshShownValue();
		SetSettings();
	}
}
