using Core.Utilities;
using UI.AvatarBuilder;
using UI.AvatarBuilder.Handlers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Core.Colors;

public class ColorGroupPicker : MonoBehaviour
{
	[SerializeField]
	private string identifier;

	[SerializeField]
	private AssetReference colorChoicePrefab;

	[SerializeField]
	private Transform container;

	[SerializeField]
	public ScriptableColorGroup colorGroup;

	[SerializeField]
	private ColorHandler handler;

	private GameObject _colorChoiceObject;

	private void Awake()
	{
		if (!colorChoicePrefab.RuntimeKeyIsValid())
		{
			Debug.LogWarning(base.gameObject.name + ": colorChoicePrefab is not valid!");
		}
	}

	private void Start()
	{
		LoadColorOptions();
	}

	private void LoadColorOptions()
	{
		ToggleGroup component = container.GetComponent<ToggleGroup>();
		if (component == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to get ToggleGroup");
			return;
		}
		ScriptableColor[] colors = colorGroup.colors;
		foreach (ScriptableColor color in colors)
		{
			ColorLoader colorLoader = Instantiator.Instantiate<ColorLoader>(colorChoicePrefab, container.transform);
			colorLoader.SetColor(color);
			colorLoader.GetComponent<ColorApplier>().Init(handler, color);
			Toggle component2 = colorLoader.GetComponent<Toggle>();
			if (component2 == null)
			{
				Debug.LogError(base.gameObject.name + ": Unable to get Toggle");
				continue;
			}
			component.RegisterToggle(component2);
			component2.group = component;
		}
	}
}
