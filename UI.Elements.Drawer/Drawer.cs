using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Elements.Drawer;

public abstract class Drawer : MonoBehaviour
{
	[Header("Assets")]
	[SerializeField]
	protected AssetReference sectionPrefab;

	private readonly Dictionary<Enum, Section> _sections = new Dictionary<Enum, Section>();

	private int _loadingSections;

	private ContentSizeFitter _fitter;

	private RectTransform _rectTransform;

	protected readonly List<RectTransform> UpdatedRectTransforms = new List<RectTransform>();

	protected virtual void Awake()
	{
		DestroyContent();
		_fitter = GetComponentInParent<ContentSizeFitter>();
		_rectTransform = GetComponent<RectTransform>();
	}

	protected virtual void OnEnable()
	{
		EnableSections();
	}

	protected virtual void OnDisable()
	{
		DisableSections();
	}

	protected abstract string GetSectionLocaleKey(Enum sectionIdentifier);

	private void EnableFitter()
	{
		if (_fitter != null)
		{
			_fitter.enabled = true;
		}
	}

	private void DisableFitter()
	{
		if (_fitter != null)
		{
			_fitter.enabled = false;
		}
	}

	private void EnableSections()
	{
		List<Section> list = _sections.Values.Where((Section section) => !section.gameObject.activeSelf).ToList();
		_loadingSections = list.Count;
		if (_loadingSections < 1)
		{
			EnableFitter();
			return;
		}
		DisableFitter();
		foreach (Section item in list)
		{
			item.SectionEnabled = (UnityAction<Section>)Delegate.Combine(item.SectionEnabled, new UnityAction<Section>(SectionEnabled));
			item.gameObject.SetActive(value: true);
		}
	}

	private void SectionEnabled(Section section)
	{
		section.SectionEnabled = (UnityAction<Section>)Delegate.Remove(section.SectionEnabled, new UnityAction<Section>(SectionEnabled));
		_loadingSections--;
		if (_loadingSections == 0)
		{
			EnableFitter();
			StartCoroutine(RefreshUpdatedSections());
		}
	}

	private void DisableSections()
	{
		DisableFitter();
		foreach (Section item in _sections.Values.Where((Section section) => section.gameObject.activeSelf).ToList())
		{
			item.gameObject.SetActive(value: false);
		}
	}

	protected Section GetSection(Enum sectionIdentifier)
	{
		_sections.TryGetValue(sectionIdentifier, out var value);
		return value;
	}

	protected Section GetOrCreateSection(Enum sectionIdentifier)
	{
		Section section = GetSection(sectionIdentifier);
		if (section != null)
		{
			return section;
		}
		section = CreateSection();
		if (section == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Could not create section " + sectionIdentifier.ToString() + "!");
			return null;
		}
		string sectionLocaleKey = GetSectionLocaleKey(sectionIdentifier);
		section.SetLabelLocaleKey(sectionLocaleKey);
		Section section2 = section;
		section2.SectionUpdated = (UnityAction<RectTransform>)Delegate.Combine(section2.SectionUpdated, new UnityAction<RectTransform>(OnSectionUpdated));
		_sections.Add(sectionIdentifier, section);
		return section;
	}

	protected Section CreateSection()
	{
		Section section = Instantiator.Instantiate<Section>(sectionPrefab, base.transform);
		if (!section)
		{
			return null;
		}
		section.gameObject.SetActive(base.gameObject.activeInHierarchy);
		return section;
	}

	private void OnSectionUpdated(RectTransform sectionRectTransform)
	{
		if (!(sectionRectTransform == null) && !UpdatedRectTransforms.Contains(sectionRectTransform))
		{
			UpdatedRectTransforms.Add(sectionRectTransform);
		}
	}

	protected IEnumerator RefreshUpdatedSections()
	{
		List<RectTransform> list = UpdatedRectTransforms.ToList();
		UpdatedRectTransforms.Clear();
		foreach (RectTransform updatedRectTransform in list)
		{
			yield return new WaitForFixedUpdate();
			LayoutRebuilder.ForceRebuildLayoutImmediate(updatedRectTransform);
		}
		if ((bool)_rectTransform)
		{
			yield return new WaitForFixedUpdate();
			LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
		}
	}

	protected void DestroyContent()
	{
		foreach (Transform item in base.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		_sections.Clear();
	}
}
