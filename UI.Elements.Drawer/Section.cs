using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace UI.Elements.Drawer;

public class Section : MonoBehaviour, ITransformContainer
{
	[Header("Assets")]
	[SerializeField]
	private TMP_Text textObject;

	[SerializeField]
	private GameObject container;

	private string _labelLocaleKey;

	private readonly List<Component> _sectionEntries = new List<Component>();

	public UnityAction<Section> SectionEnabled;

	public UnityAction<RectTransform> SectionUpdated;

	private GridLayoutGroup _gridLayoutGroup;

	private RectTransform _rectTransform;

	protected void Awake()
	{
		if (!textObject)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing textObject!");
			base.enabled = false;
		}
		if (!container)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing container!");
			base.enabled = false;
		}
		_gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
		_rectTransform = GetComponent<RectTransform>();
		LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
	}

	protected void OnEnable()
	{
		StartCoroutine(EnableEntries());
		RefreshLabel();
	}

	protected void OnDisable()
	{
		DisableEntries();
	}

	protected void OnDestroy()
	{
		LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
	}

	private void OnLocaleChanged(Locale locale)
	{
		RefreshLabel();
	}

	public void SetLabelLocaleKey(string localeKey)
	{
		_labelLocaleKey = localeKey;
		RefreshLabel();
	}

	private void RefreshLabel()
	{
		if (!(textObject == null))
		{
			string localeString = LocalizationHelper.GetLocaleString(_labelLocaleKey);
			bool flag = !string.IsNullOrEmpty(localeString) && localeString != "Unknown";
			textObject.text = (flag ? localeString : "");
			if (textObject.gameObject.activeSelf != flag)
			{
				textObject.gameObject.SetActive(flag);
			}
		}
	}

	private void EnableGrid()
	{
		if (_gridLayoutGroup != null)
		{
			_gridLayoutGroup.enabled = true;
		}
	}

	private void DisableGrid()
	{
		if (_gridLayoutGroup != null)
		{
			_gridLayoutGroup.enabled = false;
		}
	}

	private IEnumerator EnableEntries()
	{
		List<Component> entries = _sectionEntries.Where((Component entry) => !entry.gameObject.activeSelf).ToList();
		if (entries.Count == 0)
		{
			EnableGrid();
			SectionEnabled?.Invoke(this);
			yield break;
		}
		DisableGrid();
		yield return new WaitForFixedUpdate();
		int i = 0;
		int num = 0;
		while (i < entries.Count)
		{
			if (num > 10)
			{
				yield return new WaitForFixedUpdate();
				num = 0;
			}
			entries[i].gameObject.SetActive(value: true);
			i++;
			num++;
		}
		yield return new WaitForFixedUpdate();
		EnableGrid();
		SectionUpdated?.Invoke(_rectTransform);
		yield return new WaitForFixedUpdate();
		SectionEnabled?.Invoke(this);
	}

	private void DisableEntries()
	{
		DisableGrid();
		foreach (Component item in _sectionEntries.Where((Component entry) => entry.gameObject.activeSelf).ToList())
		{
			item.gameObject.SetActive(value: false);
		}
	}

	public List<T> GetEntries<T>()
	{
		return _sectionEntries.OfType<T>().ToList();
	}

	public void AddEntry(Component entry)
	{
		Transform containerTransform = GetContainerTransform();
		if ((bool)containerTransform)
		{
			entry.transform.SetParent(containerTransform);
		}
		int count = _sectionEntries.Count;
		_sectionEntries.Add(entry);
		if (RowCountChanged(count, count + 1))
		{
			SectionUpdated?.Invoke(_rectTransform);
		}
	}

	public void RemoveEntry(Component entry)
	{
		int count = _sectionEntries.Count;
		_sectionEntries.Remove(entry);
		Object.Destroy(entry.gameObject);
		if (RowCountChanged(count, count - 1))
		{
			SectionUpdated?.Invoke(_rectTransform);
		}
	}

	private bool RowCountChanged(int before, int now)
	{
		int num = ((_gridLayoutGroup != null) ? _gridLayoutGroup.constraintCount : 0);
		if (num == 0)
		{
			return true;
		}
		if (before == 0 || now == 0)
		{
			return true;
		}
		return before % num != now % num;
	}

	public Transform GetContainerTransform()
	{
		if (!container)
		{
			return null;
		}
		return container.transform;
	}
}
