using UnityEngine;
using UnityEngine.Events;

namespace UI.HUD.Dialogues.NPC.Shop;

public class NPCShopTabContent : MonoBehaviour
{
	[SerializeField]
	private GameObject container;

	public UnityAction<NPCShopTabContent> OnEnabled;

	protected void Awake()
	{
		if (container == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing container");
		}
	}

	private void OnEnable()
	{
		OnEnabled?.Invoke(this);
	}

	public Transform GetTransform()
	{
		if (!(container != null))
		{
			return null;
		}
		return container.transform;
	}
}
