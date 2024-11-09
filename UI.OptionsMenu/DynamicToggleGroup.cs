using UnityEngine;
using UnityEngine.UI;

namespace UI.OptionsMenu;

public class DynamicToggleGroup : MonoBehaviour
{
	[SerializeField]
	private Toggle[] toggles;

	private ToggleGroup _toggleGroup;

	private void Awake()
	{
		if (!_toggleGroup)
		{
			_toggleGroup = base.gameObject.AddComponent<ToggleGroup>();
		}
		GroupToggles();
	}

	private void GroupToggles()
	{
		for (int i = 0; i < toggles.Length; i++)
		{
			toggles[i].group = _toggleGroup;
			toggles[i].onValueChanged.AddListener(OnValueChanged);
		}
	}

	private void OnValueChanged(bool arg0)
	{
		Debug.Log(arg0);
	}
}
