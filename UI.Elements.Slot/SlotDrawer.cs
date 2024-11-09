using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.Elements.Drawer;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.Elements.Slot;

public abstract class SlotDrawer : UI.Elements.Drawer.Drawer
{
	[Header("Slots")]
	[SerializeField]
	protected AssetReference slotPrefab;

	private GameObject _slotObject;

	private readonly List<ISlotItemEntry> _entriesInQueue = new List<ISlotItemEntry>();

	private Coroutine _processing;

	protected override void Awake()
	{
		base.Awake();
		if (!slotPrefab.RuntimeKeyIsValid())
		{
			Debug.LogError(base.gameObject.name + ": Missing slot prefab!");
			return;
		}
		_slotObject = slotPrefab.LoadAssetAsync<GameObject>().WaitForCompletion();
		if (!_slotObject)
		{
			Debug.LogError(base.gameObject.name + ": Could not load slot prefab!");
		}
	}

	protected override void OnEnable()
	{
		if (_entriesInQueue.Count > 0)
		{
			ProcessQueue();
		}
		base.OnEnable();
	}

	public void AddSlots(List<ISlotItemEntry> entries)
	{
		_entriesInQueue.AddRange(entries);
		ProcessQueue();
	}

	protected virtual void AddSlot(ISlotItemEntry entry)
	{
		_entriesInQueue.Add(entry);
		ProcessQueue();
	}

	protected virtual void RemoveSlot(ISlotItemEntry entry)
	{
		RemoveSlotFromSection(entry);
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(RefreshUpdatedSections());
		}
	}

	private void ProcessQueue()
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (_processing != null)
			{
				StopCoroutine(_processing);
			}
			_processing = StartCoroutine(ProcessEntriesInQueue());
		}
	}

	private IEnumerator ProcessEntriesInQueue()
	{
		List<ISlotItemEntry> entries = _entriesInQueue.ToList();
		int i = 0;
		int num = 0;
		while (i < entries.Count)
		{
			AddSlotToSection(entries[i]);
			if (num > 10)
			{
				yield return new WaitForEndOfFrame();
				num = 0;
			}
			i++;
			num++;
		}
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(RefreshUpdatedSections());
		}
		_processing = null;
	}

	private void AddSlotToSection(ISlotItemEntry entry)
	{
		_entriesInQueue.Remove(entry);
		if (entry == null)
		{
			return;
		}
		Section orCreateSection = GetOrCreateSection(entry.GetSection());
		if (!(orCreateSection == null))
		{
			Slot slot = CreateSlot();
			if (slot == null)
			{
				Debug.LogWarning("Unable to create slot");
				return;
			}
			slot.SetSlotItemEntry(entry);
			orCreateSection.AddEntry(slot);
		}
	}

	private Slot CreateSlot()
	{
		GameObject gameObject = Object.Instantiate(_slotObject, base.transform);
		if (!gameObject)
		{
			Debug.LogWarning(base.gameObject.name + ": Unable to instantiate InventorySlot!");
			return null;
		}
		Slot component = gameObject.GetComponent<Slot>();
		if (!component)
		{
			Debug.LogWarning(base.gameObject.name + ": Unable to GetComponent InventorySlot!");
			Object.Destroy(gameObject);
			return null;
		}
		gameObject.SetActive(base.gameObject.activeInHierarchy);
		return component;
	}

	private void RemoveSlotFromSection(ISlotItemEntry entry)
	{
		if (entry == null)
		{
			return;
		}
		Section section = GetSection(entry.GetSection());
		if (!(section == null))
		{
			Slot slot = section.GetEntries<Slot>().FirstOrDefault((Slot s) => s.GetSlotItemEntry() == entry);
			if (!(slot == null))
			{
				slot.ClearSlotItem();
				section.RemoveEntry(slot);
			}
		}
	}
}
