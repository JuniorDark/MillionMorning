using System;
using System.Collections.Generic;
using TMPro;
using UI.Tooltip.Stars;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.Tooltip;

[Serializable]
public class Stat : MonoBehaviour
{
	[Header("Elements")]
	[SerializeField]
	private GameObject starContainer;

	[SerializeField]
	private TMP_Text text;

	[Header("Prefab")]
	[SerializeField]
	private AssetReference starPrefab;

	private Star _star;

	[Header("Config")]
	[SerializeField]
	private int maxStars = 5;

	private List<Star> _stars;

	private int _value;

	private void Awake()
	{
		if (_star == null)
		{
			GameObject gameObject = starPrefab.LoadAssetAsync<GameObject>().WaitForCompletion();
			if (!gameObject)
			{
				Debug.LogError("Failed to load stat prefab!");
				return;
			}
			_star = gameObject.GetComponent<Star>();
		}
		if (!starContainer)
		{
			Debug.LogError("Missing stats container!");
		}
	}

	private void Start()
	{
		UpdateValue(_value);
	}

	private void UpdateValue(int val)
	{
		RemoveStars();
		CreateStars();
		Replenish(val);
	}

	public void SetText(string newText)
	{
		text.text = newText;
	}

	private void RemoveStars()
	{
		for (int i = 0; i < starContainer.transform.childCount; i++)
		{
			Transform child = starContainer.transform.GetChild(i);
			if ((bool)child)
			{
				UnityEngine.Object.Destroy(child.gameObject);
			}
		}
	}

	public void SetValue(int newValue)
	{
		_value = newValue;
	}

	public void CreateStars()
	{
		_stars = new List<Star>();
		if (!starContainer)
		{
			Debug.LogWarning("Unable to find StarContainer");
			return;
		}
		for (int i = 0; i < maxStars; i++)
		{
			Star star = UnityEngine.Object.Instantiate(_star, starContainer.transform);
			if (!star)
			{
				break;
			}
			star.SetFillAmount(0f);
			_stars.Add(star);
		}
	}

	public void Replenish(int pieces)
	{
		foreach (Star star in _stars)
		{
			int numberOfPieces = ((pieces < star.EmptyPieces) ? pieces : star.EmptyPieces);
			pieces -= star.EmptyPieces;
			star.Replenish(numberOfPieces);
			if (pieces <= 0)
			{
				break;
			}
		}
	}
}
