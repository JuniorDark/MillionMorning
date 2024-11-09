using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.BodyPack.HairSystem;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.BodyPack;

public sealed class MilMo_BodyPack : MilMo_Template
{
	public delegate void ContentDone(bool success);

	private bool _isEquipped;

	private bool _isLoadingSharedContent;

	private List<ColorGroup> _colorGroupsSorted;

	private readonly IDictionary<string, IList<int>> _defaultColorIndices = new Dictionary<string, IList<int>>();

	private readonly IList<KeyValuePair<SkinnedMeshRenderer, ContentDone>> _callbacks = new List<KeyValuePair<SkinnedMeshRenderer, ContentDone>>();

	public MilMo_BodyPackGender Gender { get; private set; }

	public bool IsHat { get; private set; }

	public bool IsHair { get; private set; }

	public bool HideHair { get; private set; }

	public List<string> Categories { get; } = new List<string>();


	public List<MilMo_Addon> Addons { get; } = new List<MilMo_Addon>();


	public List<MilMo_SkinLayer> BlendLayers { get; } = new List<MilMo_SkinLayer>();


	public List<MilMo_SkinLayer> SkinLayers { get; } = new List<MilMo_SkinLayer>();


	public List<MilMo_SoftMesh> SoftMeshes { get; } = new List<MilMo_SoftMesh>();


	public List<ColorGroup> ColorGroups { get; } = new List<ColorGroup>();


	public IList<ColorGroup> ColorGroupsSorted
	{
		get
		{
			if (_colorGroupsSorted != null)
			{
				return _colorGroupsSorted;
			}
			_colorGroupsSorted = new List<ColorGroup>(ColorGroups);
			_colorGroupsSorted.Sort((ColorGroup cg1, ColorGroup cg2) => string.Compare(cg1.GroupName, cg2.GroupName, StringComparison.Ordinal));
			return _colorGroupsSorted;
		}
	}

	public MilMo_BodyPack(string category, string path, string filePath)
		: base(category, path, filePath, "BodyPack")
	{
		if (path.Contains("Boy"))
		{
			Gender = MilMo_BodyPackGender.Boy;
		}
		else if (path.Contains("Girl"))
		{
			Gender = MilMo_BodyPackGender.Girl;
		}
	}

	public bool HasContent(SkinnedMeshRenderer renderer)
	{
		if (renderer == null)
		{
			return true;
		}
		if (!HasSharedContent())
		{
			return false;
		}
		return Addons.All((MilMo_Addon add) => add.HasContentInstantiated(renderer));
	}

	private bool HasSharedContent()
	{
		if (BlendLayers.Any((MilMo_SkinLayer sl) => !sl.HasContent))
		{
			return false;
		}
		if (SkinLayers.Any((MilMo_SkinLayer sl) => !sl.HasContent))
		{
			return false;
		}
		if (SoftMeshes.Any((MilMo_SoftMesh sm) => !sm.HasContent))
		{
			return false;
		}
		return Addons.All((MilMo_Addon add) => add.HasContent);
	}

	public bool DoneLoading(SkinnedMeshRenderer renderer)
	{
		if (renderer == null)
		{
			return true;
		}
		if (!DoneLoadingSharedContent())
		{
			return false;
		}
		return Addons.All((MilMo_Addon add) => add.DoneLoadingInstantiatedContent(renderer));
	}

	private bool DoneLoadingSharedContent()
	{
		if (BlendLayers.Any((MilMo_SkinLayer sl) => !sl.DoneLoading))
		{
			return false;
		}
		if (SkinLayers.Any((MilMo_SkinLayer sl) => !sl.DoneLoading))
		{
			return false;
		}
		if (SoftMeshes.Any((MilMo_SoftMesh sm) => !sm.DoneLoading))
		{
			return false;
		}
		return Addons.All((MilMo_Addon ad) => ad.DoneLoading);
	}

	public void Update(SkinnedMeshRenderer renderer)
	{
		foreach (MilMo_Addon addon in Addons)
		{
			addon.Update(renderer);
		}
	}

	public Transform GetFirstAddonTransform(SkinnedMeshRenderer renderer)
	{
		if (Addons.Count <= 0 || Addons[0] == null)
		{
			return null;
		}
		return Addons[0].GetTransform(renderer);
	}

	public GameObject GetFirstAddonGameObject(SkinnedMeshRenderer renderer)
	{
		if (Addons.Count <= 0 || Addons[0] == null)
		{
			return null;
		}
		return Addons[0].GetGameObject(renderer);
	}

	public bool IsCategory(MilMo_BodyPack bodypack)
	{
		if (bodypack == null)
		{
			return false;
		}
		return Categories.Any((string cat) => bodypack.Categories.Any((string otherCat) => cat == otherCat));
	}

	public IDictionary<string, int> GetDefaultColorIndices()
	{
		IDictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (ColorGroup colorGroup in ColorGroups)
		{
			int defaultColorIndex = GetDefaultColorIndex(colorGroup);
			if (defaultColorIndex != -1)
			{
				dictionary.Add(base.Name + ":" + colorGroup.GroupName, defaultColorIndex);
			}
		}
		return dictionary;
	}

	public int GetDefaultColorIndex(ColorGroup colorGroup)
	{
		if (colorGroup == null)
		{
			return -1;
		}
		if (_defaultColorIndices.ContainsKey(colorGroup.GroupName))
		{
			IList<int> list = _defaultColorIndices[colorGroup.GroupName];
			switch (list.Count)
			{
			case 1:
				return list[0];
			default:
				return list[MilMo_Utility.RandomInt(0, list.Count - 1)];
			case 0:
				break;
			}
		}
		if (colorGroup.ColorIndices == null || colorGroup.ColorIndices.Count == 0)
		{
			return -1;
		}
		return colorGroup.ColorIndices[MilMo_Utility.RandomInt(0, colorGroup.ColorIndices.Count - 1)];
	}

	public override bool ReadLine(MilMo_SFFile file)
	{
		if (file.IsNext("ColorGroup"))
		{
			string @string = file.GetString();
			List<int> list = new List<int>();
			IDictionary<int, int> dictionary = new Dictionary<int, int>();
			while (file.HasMoreTokens())
			{
				string string2 = file.GetString();
				if (int.TryParse(string2, out var result))
				{
					if (MilMo_BodyPackSystem.GetColorFromIndex(result) == null)
					{
						Debug.LogWarning("Invalid Color Index " + string2);
					}
					else if (!dictionary.ContainsKey(result))
					{
						list.Add(result);
						dictionary.Add(result, result);
					}
					continue;
				}
				IList<int> templateColorIndices = MilMo_BodyPackSystem.GetTemplateColorIndices(string2);
				if (templateColorIndices == null)
				{
					Debug.LogWarning("Invalid ColorTemplate " + string2);
					continue;
				}
				foreach (int item5 in templateColorIndices)
				{
					if (!dictionary.ContainsKey(item5))
					{
						list.Add(item5);
						dictionary.Add(item5, item5);
					}
				}
			}
			ColorGroups.Add(new ColorGroup(@string, list));
		}
		else if (file.IsNext("ColorGroupDefault"))
		{
			string colorGroupName = file.GetString();
			ColorGroup colorGroup = ColorGroups.Find((ColorGroup cg) => cg.GroupName.Equals(colorGroupName));
			if (colorGroup != null)
			{
				List<int> list2 = new List<int>();
				while (file.HasMoreTokens())
				{
					int @int = file.GetInt();
					if (colorGroup.ColorIndices.Contains(@int))
					{
						list2.Add(@int);
					}
					else
					{
						Debug.LogWarning("Invalid default color in BodyPack " + base.Name);
					}
				}
				_defaultColorIndices.Add(colorGroupName, list2);
			}
			else
			{
				Debug.LogWarning("Non existing ColorGroup " + colorGroupName + " for ColorGroupDefault in BodyPack " + base.Name);
			}
		}
		else if (file.IsNext("BlendLayer"))
		{
			string string3 = file.GetString();
			string string4 = file.GetString();
			string colorGroupName = file.GetString();
			ColorGroup colorGroup2 = ColorGroups.Find((ColorGroup cg) => cg.GroupName.Equals(colorGroupName));
			if (colorGroup2 == null && !colorGroupName.Equals("NoBlend", StringComparison.InvariantCultureIgnoreCase))
			{
				Debug.LogWarning("Invalid blend layer in bodypack '" + base.Name + "' (" + colorGroupName + ")");
			}
			MilMo_SkinLayer item = new MilMo_SkinLayer(string3, string4, colorGroup2, Path, Vector2.zero);
			BlendLayers.Add(item);
		}
		else if (file.IsNext("SoftMesh"))
		{
			string string5 = file.GetString();
			IList<MilMo_TextureColorGroupPair> list3 = new List<MilMo_TextureColorGroupPair>();
			while (file.HasMoreTokens())
			{
				string string6 = file.GetString();
				if (!file.HasMoreTokens())
				{
					Debug.LogWarning("Textures and color groups must be given in pairs: '" + base.Name + "'");
					return false;
				}
				string colorGroupName = file.GetString();
				ColorGroup colorGroup3 = ColorGroups.Find((ColorGroup cg) => cg.GroupName.Equals(colorGroupName));
				if (colorGroup3 == null && !colorGroupName.Equals("NoBlend", StringComparison.InvariantCultureIgnoreCase))
				{
					Debug.LogWarning("Invalid SoftMesh layer in bodypack '" + base.Name + "'");
				}
				Vector2 uvOffset = Vector2.zero;
				if (file.IsNext("Offset"))
				{
					uvOffset = file.GetVector2();
				}
				list3.Add(new MilMo_TextureColorGroupPair(string6, colorGroup3, uvOffset));
			}
			MilMo_SoftMesh item2 = new MilMo_SoftMesh(string5, list3, Path);
			SoftMeshes.Add(item2);
		}
		else if (file.IsNext("SkinLayer"))
		{
			string string7 = file.GetString();
			string string8 = file.GetString();
			string colorGroupName = file.GetString();
			ColorGroup colorGroup4 = ColorGroups.Find((ColorGroup cg) => cg.GroupName.Equals(colorGroupName));
			if (colorGroup4 == null && !colorGroupName.Equals("NoBlend", StringComparison.InvariantCultureIgnoreCase))
			{
				Debug.LogWarning("Invalid skin layer in bodypack '" + base.Name + "'");
			}
			Vector2 uvOffset2 = new Vector2(0f, 0f);
			if (file.IsNext("Offset"))
			{
				uvOffset2 = file.GetVector2();
				uvOffset2.x /= 1024f;
				uvOffset2.y /= 1024f;
			}
			MilMo_SkinLayer item3 = new MilMo_SkinLayer(string7, string8, colorGroup4, Path, uvOffset2);
			SkinLayers.Add(item3);
		}
		else if (file.IsNext("Addon"))
		{
			string string9 = file.GetString();
			string string10 = file.GetString();
			bool scale = true;
			Vector3 boyOffset = Vector3.zero;
			Vector3 girlOffset = Vector3.zero;
			IList<MilMo_TextureColorGroupPair> list4 = new List<MilMo_TextureColorGroupPair>();
			while (file.HasMoreTokens())
			{
				if (file.IsNext("Scale"))
				{
					scale = false;
					continue;
				}
				if (file.IsNext("BoyOffset"))
				{
					boyOffset = file.GetVector3();
					continue;
				}
				if (file.IsNext("GirlOffset"))
				{
					girlOffset = file.GetVector3();
					continue;
				}
				string string11 = file.GetString();
				if (!file.HasMoreTokens())
				{
					Debug.LogWarning("Textures and color groups must be given in pairs: '" + base.Name + "'");
					return false;
				}
				string colorGroupName = file.GetString();
				ColorGroup colorGroup5 = ColorGroups.Find((ColorGroup cg) => cg.GroupName.Equals(colorGroupName));
				if (colorGroup5 == null && !colorGroupName.Equals("NoBlend", StringComparison.InvariantCultureIgnoreCase))
				{
					Debug.LogWarning("Invalid Addon layer in bodypack '" + base.Name + "'");
				}
				Vector2 uvOffset3 = Vector2.zero;
				if (file.IsNext("Offset"))
				{
					uvOffset3 = file.GetVector2();
				}
				list4.Add(new MilMo_TextureColorGroupPair(string11, colorGroup5, uvOffset3));
			}
			MilMo_Addon item4 = (IsHair ? new MilMo_HairAddon(string9, string10, scale, Path, list4, boyOffset, girlOffset) : new MilMo_Addon(string9, string10, scale, Path, list4, boyOffset, girlOffset));
			Addons.Add(item4);
		}
		else if (file.IsNext("Category"))
		{
			string string12 = file.GetString();
			if (!Enum.TryParse<BodyPackCategory>(string12, ignoreCase: true, out var result2))
			{
				Debug.LogWarning("Category is not defined as a BodyPackCategory: " + string12);
			}
			if (result2 == BodyPackCategory.Hair)
			{
				IsHair = true;
			}
			Categories.Add(string12);
		}
		else if (file.IsNext("HairAddon"))
		{
			Debug.LogWarning("HairAddons is deprecated. " + file.Path + "[" + file.GetLineNumber() + "]");
		}
		else if (file.IsNext("HairSkinLayer"))
		{
			Debug.LogWarning("HairSkinLayers is deprecated. " + file.Path + "[" + file.GetLineNumber() + "]");
		}
		else if (file.IsNext("Hat"))
		{
			IsHat = true;
		}
		else if (file.IsNext("HideHair"))
		{
			HideHair = true;
		}
		else if (file.IsNext("Boy"))
		{
			Gender = MilMo_BodyPackGender.Boy;
		}
		else
		{
			if (!file.IsNext("Girl"))
			{
				return base.ReadLine(file);
			}
			Gender = MilMo_BodyPackGender.Girl;
		}
		return true;
	}

	public void AsyncLoadContent(SkinnedMeshRenderer renderer, ContentDone callback)
	{
		if (HasContent(renderer))
		{
			if (callback != null)
			{
				callback(success: true);
			}
			return;
		}
		if (HasSharedContent())
		{
			AsyncLoadInstantiatedContent(renderer, delegate
			{
				callback(HasContent(renderer));
			});
			return;
		}
		if (callback != null)
		{
			_callbacks.Add(new KeyValuePair<SkinnedMeshRenderer, ContentDone>(renderer, callback));
		}
		if (_isLoadingSharedContent)
		{
			return;
		}
		_isLoadingSharedContent = true;
		foreach (MilMo_SkinLayer blendLayer in BlendLayers)
		{
			blendLayer.AsyncLoadContent(delegate
			{
				CheckIfDoneLoadingSharedContent();
			});
		}
		foreach (MilMo_SkinLayer skinLayer in SkinLayers)
		{
			skinLayer.AsyncLoadContent(delegate
			{
				CheckIfDoneLoadingSharedContent();
			});
		}
		foreach (MilMo_SoftMesh softMesh in SoftMeshes)
		{
			softMesh.AsyncLoadContent(delegate
			{
				CheckIfDoneLoadingSharedContent();
			});
		}
		foreach (MilMo_Addon addon in Addons)
		{
			addon.AsyncLoadContent(delegate
			{
				CheckIfDoneLoadingSharedContent();
			});
		}
	}

	private void AsyncLoadInstantiatedContent(SkinnedMeshRenderer renderer, ContentDone callback)
	{
		int addonsLoaded = 0;
		bool hasAllSucceeded = true;
		foreach (MilMo_Addon addon in Addons)
		{
			addon.AsyncLoadVisualRep(renderer, delegate(bool success)
			{
				int num = addonsLoaded + 1;
				addonsLoaded = num;
				hasAllSucceeded &= success;
				if (addonsLoaded >= Addons.Count)
				{
					callback(hasAllSucceeded);
				}
			});
		}
	}

	private void CheckIfDoneLoadingSharedContent()
	{
		if (!DoneLoadingSharedContent())
		{
			return;
		}
		_isLoadingSharedContent = false;
		bool flag = Addons.Count > 0;
		foreach (KeyValuePair<SkinnedMeshRenderer, ContentDone> callback in _callbacks)
		{
			KeyValuePair<SkinnedMeshRenderer, ContentDone> keyValuePair = callback;
			if (flag)
			{
				AsyncLoadInstantiatedContent(keyValuePair.Key, keyValuePair.Value);
			}
			else
			{
				callback.Value(HasContent(keyValuePair.Key));
			}
		}
	}

	public void UnloadContent(SkinnedMeshRenderer renderer)
	{
		foreach (MilMo_Addon addon in Addons)
		{
			addon.UnloadContent(renderer);
		}
	}
}
