using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Core;
using UI.Elements.Slot;
using UnityEngine;

namespace Code.Core.Items;

public abstract class MilMo_Item : IEntryItem
{
	public delegate void IconDone(Texture2D icon);

	public enum ItemGender
	{
		Boy,
		Girl,
		Both
	}

	public delegate void ItemLoaded(MilMo_Item item);

	protected const int ICON_SIZE = 128;

	public const string MODIFIER_SEPARATOR = "#";

	public Texture2D ItemIcon;

	private readonly List<IconDone> _iconCallbacks = new List<IconDone>();

	private bool _isLoadingIcon;

	private bool _iconLoaded;

	public string Identifier { get; private set; }

	public MilMo_ItemTemplate Template { get; set; }

	public Dictionary<string, string> Modifiers { get; protected set; }

	public string PickupAnimation { get; protected set; }

	public virtual string PickupParticle => "";

	public IList<string> ModifiersAsList
	{
		get
		{
			List<string> list = Modifiers.Select((KeyValuePair<string, string> modifier) => modifier.Key + "#" + modifier.Value).ToList();
			list.Sort();
			return list;
		}
	}

	public Item ItemStruct => new Item(Template.TemplateReferenceStruct, ModifiersAsList);

	public virtual ItemGender UseableGender => ItemGender.Both;

	public Texture2D GetItemIcon()
	{
		return ItemIcon;
	}

	public MilMo_LocString GetDisplayName()
	{
		return Template.DisplayName;
	}

	public virtual MilMo_LocString GetDescription()
	{
		return Template.Description;
	}

	public static async Task<MilMo_Item> AsyncGetItem(string itemTemplateIdentifier)
	{
		return ((await Singleton<MilMo_TemplateContainer>.Instance.GetTemplateAsync(itemTemplateIdentifier)) as MilMo_ItemTemplate)?.Instantiate(new Dictionary<string, string>());
	}

	public static void AsyncGetItem(string itemTemplateIdentifier, ItemLoaded callback)
	{
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(itemTemplateIdentifier, Callback);
		void Callback(MilMo_Template template, bool timeOut)
		{
			MilMo_ItemTemplate milMo_ItemTemplate = template as MilMo_ItemTemplate;
			if (timeOut || milMo_ItemTemplate == null)
			{
				callback?.Invoke(null);
			}
			else
			{
				callback?.Invoke(milMo_ItemTemplate.Instantiate(new Dictionary<string, string>()));
			}
		}
	}

	protected MilMo_Item(MilMo_ItemTemplate template, Dictionary<string, string> modifiers)
	{
		Template = template;
		Modifiers = modifiers;
		PickupAnimation = "GenericPickup01";
		RecalculateIdentifier();
	}

	public virtual void Destroy()
	{
	}

	protected void RecalculateIdentifier()
	{
		Identifier = Template.Identifier;
		foreach (string modifiersAs in ModifiersAsList)
		{
			Identifier += modifiersAs;
		}
	}

	public void ChangeModifier(string key, string newValue)
	{
		if (!Modifiers.ContainsKey(key))
		{
			Modifiers.Add(key, newValue);
		}
		else
		{
			Modifiers[key] = newValue;
		}
		RecalculateIdentifier();
		ModifierChanged(key);
	}

	protected virtual void ModifierChanged(string key)
	{
	}

	public bool Equals(MilMo_Item item)
	{
		if (item != null)
		{
			return Identifier.EndsWith(item.Identifier);
		}
		return false;
	}

	public async Task<Texture2D> AsyncGetIcon()
	{
		TaskCompletionSource<Texture2D> tcs = new TaskCompletionSource<Texture2D>();
		AsyncGetIcon(delegate(Texture2D texture)
		{
			if (!texture)
			{
				Debug.LogWarning("could not load icon for: " + Identifier);
			}
			tcs.TrySetResult(texture);
		});
		return await tcs.Task;
	}

	public void AsyncGetIcon(IconDone callback)
	{
		AsyncGetIcon(MilMo_ResourceManager.Priority.High, callback);
	}

	public async void AsyncGetIcon(MilMo_ResourceManager.Priority priority, IconDone callback)
	{
		if (!_iconLoaded)
		{
			if (callback != null)
			{
				_iconCallbacks.Add(callback);
			}
			if (!_isLoadingIcon)
			{
				_isLoadingIcon = true;
				string path = ((!string.IsNullOrEmpty(Template.CustomIdiotIconPath)) ? Template.CustomIdiotIconPath : Template.IconPath);
				IconDoneCallback(await MilMo_ResourceManager.Instance.LoadTextureAsync(path, "Generic", priority));
			}
		}
		else
		{
			callback?.Invoke(ItemIcon);
		}
	}

	private void IconDoneCallback(Texture2D icon)
	{
		ItemIcon = icon;
		OnIconLoaded();
		if (ItemIcon == null)
		{
			Debug.LogWarning("Failed to load icon for item " + Template.Identifier);
		}
		_iconLoaded = true;
		_isLoadingIcon = false;
		foreach (IconDone iconCallback in _iconCallbacks)
		{
			iconCallback(ItemIcon);
		}
		_iconCallbacks.Clear();
	}

	public virtual void UnloadIcon()
	{
	}

	protected virtual void OnIconLoaded()
	{
	}

	public abstract bool IsWieldable();

	public abstract bool IsWearable();

	public virtual bool AutoPickup()
	{
		return Template.IsAutoPickup;
	}

	public virtual bool MayPickUp()
	{
		return true;
	}

	public virtual bool IsUseableByGender(bool isBoy)
	{
		return true;
	}

	public static Dictionary<string, string> ReadModifiers(IEnumerable<string> modifiers)
	{
		return modifiers.Select((string t) => t.Split("#".ToCharArray())).ToDictionary((string[] keyValue) => keyValue[0], (string[] keyValue) => keyValue[1]);
	}
}
