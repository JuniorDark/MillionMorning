using Core.Utilities;
using TMPro;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UI.Tooltip;

public class ConverterTooltip : Tooltip
{
	[Header("Elements")]
	[SerializeField]
	private TMP_Text title;

	[SerializeField]
	private GameObject toolContainer;

	[SerializeField]
	private Image toolIcon;

	[SerializeField]
	private TMP_Text toolText;

	[SerializeField]
	private Image resultIcon;

	[SerializeField]
	private TMP_Text resultName;

	[SerializeField]
	private TMP_Text resultDescription;

	[SerializeField]
	private GameObject ingredientsContainer;

	[Header("Prefabs")]
	[SerializeField]
	private AssetReference ingredientPrefab;

	private Ingredient _ingredient;

	private void Awake()
	{
		if (!ingredientPrefab.RuntimeKeyIsValid())
		{
			Debug.LogError("Missing ingredient prefab!");
			return;
		}
		if (_ingredient == null)
		{
			GameObject gameObject = ingredientPrefab.LoadAssetAsync<GameObject>().WaitForCompletion();
			if (!gameObject)
			{
				Debug.LogError("Failed to load ingredient prefab!");
				return;
			}
			_ingredient = gameObject.GetComponent<Ingredient>();
		}
		if (!ingredientsContainer)
		{
			Debug.LogError("Missing ingredients container!");
		}
		else if (!toolContainer)
		{
			Debug.LogError("Missing tool container!");
		}
	}

	public override void Show()
	{
		if (!string.IsNullOrEmpty(title.text))
		{
			base.Show();
		}
	}

	private void RemoveIngredients()
	{
		for (int i = 0; i < ingredientsContainer.transform.childCount; i++)
		{
			Transform child = ingredientsContainer.transform.GetChild(i);
			if ((bool)child)
			{
				Object.Destroy(child.gameObject);
			}
		}
	}

	public override void SetData(TooltipData data)
	{
		Debug.Log("Converter");
		title.text = data.GetTitle();
		if (!ingredientsContainer)
		{
			return;
		}
		RemoveIngredients();
		ConverterTooltipData converterTooltipData = (ConverterTooltipData)data;
		ItemRequiredToolData requiredTool = converterTooltipData.GetRequiredTool();
		resultName.text = converterTooltipData.GetResult().GetTitle();
		resultDescription.text = converterTooltipData.GetResult().GetDescription();
		if (string.IsNullOrEmpty(requiredTool?.GetText()) || requiredTool?.GetIcon() == null)
		{
			toolContainer.SetActive(value: false);
		}
		else
		{
			toolText.text = requiredTool.GetText();
			Core.Utilities.UI.SetIcon(toolIcon, requiredTool.GetIcon());
			toolContainer.SetActive(value: true);
		}
		Core.Utilities.UI.SetIcon(resultIcon, converterTooltipData.GetResult().GetIcon());
		ItemIngredientData[] itemComponentData = converterTooltipData.GetItemComponentData();
		foreach (ItemIngredientData itemIngredientData in itemComponentData)
		{
			Ingredient ingredient = Object.Instantiate(_ingredient, ingredientsContainer.transform);
			if (!ingredient)
			{
				break;
			}
			ingredient.SetText(itemIngredientData.GetText());
			ingredient.SetIcon(itemIngredientData.GetIcon());
		}
	}
}
