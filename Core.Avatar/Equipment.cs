using System.Collections.Generic;
using Code.Core.BodyPack;
using Core.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Avatar;

[CreateAssetMenu(menuName = "Create Equipment", fileName = "Equipment", order = 0)]
public class Equipment : ScriptableObject
{
	[SerializeField]
	private AssetReference prefab;

	public List<BodyPackCategory> alsoOccupies;

	private List<SkinnedMeshRenderer> _renderers;

	private GameObject _instance;

	private IEnumerable<SkinnedMeshRenderer> GetRenderers()
	{
		if (!(_instance != null))
		{
			return new List<SkinnedMeshRenderer>();
		}
		return _instance.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
	}

	public void Attach(EquipmentHandler context)
	{
		if (!prefab.RuntimeKeyIsValid())
		{
			Debug.LogError("Got no prefab");
			return;
		}
		if (_instance == null)
		{
			_instance = Instantiator.Instantiate<GameObject>(prefab, context.transform);
		}
		Debug.LogWarning("Instance: " + ((_instance != null) ? ((object)_instance.GetInstanceID()) : "Null"));
		foreach (SkinnedMeshRenderer renderer in GetRenderers())
		{
			renderer.rootBone = context.GetRootBone();
			renderer.bones = context.GetBones();
		}
	}

	public void Detach()
	{
		if (_instance != null)
		{
			if (Application.isPlaying)
			{
				Object.Destroy(_instance);
			}
			else
			{
				Object.DestroyImmediate(_instance);
			}
			_instance = null;
		}
	}
}
