using Core.Utilities;
using UI.AvatarBuilder;
using UI.AvatarBuilder.Handlers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Core.BodyShapes;

public class ShapePicker : MonoBehaviour
{
	[SerializeField]
	private string identifier;

	[SerializeField]
	private AssetReference pickerChoicePrefab;

	[SerializeField]
	private Transform container;

	[SerializeField]
	public ScriptableShape[] shapes;

	[SerializeField]
	private ShapeHandler handler;

	private void Awake()
	{
		if (!pickerChoicePrefab.RuntimeKeyIsValid())
		{
			Debug.LogWarning(base.gameObject.name + ": ColorPicker is not valid!");
		}
	}

	private void Start()
	{
		LoadPickerOptions();
	}

	private void LoadPickerOptions()
	{
		ToggleGroup component = container.GetComponent<ToggleGroup>();
		if (component == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to get ToggleGroup");
		}
		ScriptableShape[] array = shapes;
		foreach (ScriptableShape shape in array)
		{
			ShapeLoader shapeLoader = Instantiator.Instantiate<ShapeLoader>(pickerChoicePrefab, container.transform);
			shapeLoader.SetShape(shape);
			shapeLoader.GetComponent<ShapeApplier>().Init(handler, shape);
			Toggle component2 = shapeLoader.GetComponent<Toggle>();
			if (component2 == null)
			{
				Debug.LogError(base.gameObject.name + ": Unable to get Toggle");
			}
			component.RegisterToggle(component2);
			component2.group = component;
		}
	}
}
