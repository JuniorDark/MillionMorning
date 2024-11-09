using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Input;
using UI.Elements;
using UI.Elements.Window;
using UI.HUD;
using UI.HUD.Dialogues;
using UI.HUD.QuickInfo;
using UI.Tooltip;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI;

public class UIManager : Singleton<UIManager>
{
	[Serializable]
	public struct UIReference
	{
		public AssetReference prefab;

		public UIWindow LoadedAsset { get; set; }

		public UIWindow CurrentInstance { get; set; }
	}

	[Serializable]
	public struct PanelReference
	{
		public AssetReference prefab;

		public Panel LoadedAsset { get; set; }

		public Panel CurrentInstance { get; set; }
	}

	[Header("UI Windows")]
	[SerializeField]
	private UIReference[] uiReferences;

	[Header("Setup")]
	[SerializeField]
	private Canvas canvas;

	private Stack<UIWindow> _windowStack;

	[Header("Loading")]
	[SerializeField]
	private AssetReference loadingScreen;

	[Header("HUD")]
	[SerializeField]
	private AssetReference hud;

	[Header("Panels")]
	[SerializeField]
	private PanelReference[] panelReferences;

	[Header("Managers")]
	[SerializeField]
	private AssetReference tooltipManager;

	[SerializeField]
	private AssetReference dialogueManager;

	[SerializeField]
	private AssetReference quickInfoManager;

	private UI.HUD.HUD _hud;

	private TooltipManager _tooltipManager;

	private DialogueManager _dialogueManager;

	private QuickInfoManager _quickInfoManager;

	public event UnityAction NavigationResetEvent = delegate
	{
	};

	public event UnityAction NavigationStartEvent = delegate
	{
	};

	protected void Awake()
	{
		if (canvas == null)
		{
			canvas = base.gameObject.AddComponent<Canvas>();
		}
		if (canvas == null)
		{
			Debug.LogWarning(base.gameObject.name + " has no Canvas!");
			return;
		}
		if (_windowStack == null)
		{
			_windowStack = new Stack<UIWindow>();
		}
		if (!ValidateAssetReferences())
		{
			Debug.LogWarning(base.gameObject.name + " contains invalid asset references!");
		}
		else if (uiReferences.Any((UIReference p) => p.LoadedAsset == null) && !LoadUIWindows())
		{
			Debug.LogWarning(base.gameObject.name + " Failed to load window prefabs");
		}
		else if (panelReferences.Any((PanelReference p) => p.LoadedAsset == null) && !LoadPanels())
		{
			Debug.LogWarning(base.gameObject.name + " Failed to load panel prefabs");
		}
		else if (!ValidateInstances())
		{
			if (!FindInstances() && !Instantiate())
			{
				Debug.LogWarning(base.gameObject.name + " failed to initialize components!");
				return;
			}
			Initialize();
			InstantiateAndInitializeUIWindows();
			InstantiateAndInitializePanels();
		}
	}

	private bool ValidateAssetReferences()
	{
		if (loadingScreen.RuntimeKeyIsValid() && hud.RuntimeKeyIsValid() && tooltipManager.RuntimeKeyIsValid() && uiReferences.All((UIReference p) => p.prefab.RuntimeKeyIsValid()))
		{
			return panelReferences.All((PanelReference p) => p.prefab.RuntimeKeyIsValid());
		}
		return false;
	}

	private bool ValidateInstances()
	{
		if (_hud != null && _tooltipManager != null && _dialogueManager != null && uiReferences.All((UIReference p) => p.CurrentInstance != null))
		{
			return panelReferences.All((PanelReference p) => p.CurrentInstance != null);
		}
		return false;
	}

	private bool FindInstances()
	{
		_hud = UnityEngine.Object.FindObjectOfType<UI.HUD.HUD>(includeInactive: true);
		_tooltipManager = UnityEngine.Object.FindObjectOfType<TooltipManager>(includeInactive: true);
		_dialogueManager = UnityEngine.Object.FindObjectOfType<DialogueManager>(includeInactive: true);
		return ValidateInstances();
	}

	private bool Instantiate()
	{
		if (_hud == null)
		{
			_hud = Instantiate<UI.HUD.HUD>(hud);
			if (_hud == null)
			{
				return false;
			}
		}
		if (_tooltipManager == null)
		{
			_tooltipManager = Instantiate<TooltipManager>(tooltipManager);
			if (_tooltipManager == null)
			{
				return false;
			}
		}
		if (_dialogueManager == null)
		{
			_dialogueManager = Instantiate<DialogueManager>(dialogueManager);
			if (_dialogueManager == null)
			{
				return false;
			}
		}
		if (_quickInfoManager == null)
		{
			_quickInfoManager = Instantiate<QuickInfoManager>(quickInfoManager);
			if (_quickInfoManager == null)
			{
				return false;
			}
		}
		return true;
	}

	private T Instantiate<T>(AssetReference assetReference)
	{
		GameObject gameObject = assetReference.InstantiateAsync().WaitForCompletion();
		gameObject.SetActive(value: false);
		AddToCanvas(gameObject);
		return gameObject.GetComponent<T>();
	}

	private void AddToCanvas(GameObject go)
	{
		go.transform.SetParent(canvas.transform, worldPositionStays: false);
	}

	private void Initialize()
	{
		if (!_hud.gameObject.activeSelf)
		{
			_hud.gameObject.SetActive(value: true);
			_hud.gameObject.SetActive(value: false);
		}
		_tooltipManager.Initialize();
		_dialogueManager.Initialize();
		_quickInfoManager.Initialize();
	}

	private bool LoadUIWindows()
	{
		for (int i = 0; i < uiReferences.Length; i++)
		{
			GameObject gameObject = uiReferences[i].prefab.LoadAssetAsync<GameObject>().WaitForCompletion();
			if (gameObject == null)
			{
				Debug.LogWarning(base.gameObject.name + " Failed to load prefab: " + uiReferences[i].prefab.Asset.name);
				return false;
			}
			uiReferences[i].LoadedAsset = gameObject.GetComponent<UIWindow>();
			if (uiReferences[i].LoadedAsset == null)
			{
				Debug.LogWarning(base.gameObject.name + " Failed to load component: " + uiReferences[i].prefab.Asset.name);
				return false;
			}
		}
		return true;
	}

	private void InstantiateAndInitializeUIWindows()
	{
		for (int i = 0; i < uiReferences.Length; i++)
		{
			UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(uiReferences[i].LoadedAsset.GetType(), includeInactive: true);
			if (@object != null)
			{
				uiReferences[i].CurrentInstance = (UIWindow)@object;
				uiReferences[i].CurrentInstance.gameObject.SetActive(value: true);
				continue;
			}
			uiReferences[i].CurrentInstance = UnityEngine.Object.Instantiate(uiReferences[i].LoadedAsset, canvas.transform);
			if (!(uiReferences[i].CurrentInstance != null))
			{
				Debug.LogWarning(base.gameObject.name + " Failed to instantiate loaded asset! " + uiReferences[i].LoadedAsset.name);
				break;
			}
		}
	}

	public void OpenAndAddToHistory(UIWindow window)
	{
		if (_windowStack.Count > 0)
		{
			_windowStack.Peek().gameObject.SetActive(value: false);
		}
		window.gameObject.SetActive(value: true);
		_windowStack.Push(window);
		Singleton<InputController>.Instance.SetMenuController();
		this.NavigationStartEvent();
	}

	public void GoBack()
	{
		if (_windowStack.Count > 0)
		{
			_windowStack.Pop().gameObject.SetActive(value: false);
		}
		if (_windowStack.Count > 0)
		{
			_windowStack.Peek().gameObject.SetActive(value: true);
			this.NavigationStartEvent();
		}
		else
		{
			Singleton<InputController>.Instance.RestorePreviousController();
			EventSystem.current.SetSelectedGameObject(null);
			this.NavigationResetEvent();
		}
	}

	private void InstantiateAndInitializePanels()
	{
		if (panelReferences == null)
		{
			return;
		}
		for (int i = 0; i < panelReferences.Length; i++)
		{
			UnityEngine.Object @object = UnityEngine.Object.FindObjectOfType(panelReferences[i].LoadedAsset.GetType(), includeInactive: true);
			if (@object != null)
			{
				panelReferences[i].CurrentInstance = (Panel)@object;
				panelReferences[i].CurrentInstance.gameObject.SetActive(value: true);
				continue;
			}
			panelReferences[i].CurrentInstance = UnityEngine.Object.Instantiate(panelReferences[i].LoadedAsset, canvas.transform);
			if (!(panelReferences[i].CurrentInstance != null))
			{
				Debug.LogWarning(base.gameObject.name + " Failed to instantiate loaded asset! " + panelReferences[i].LoadedAsset.name);
				break;
			}
		}
	}

	private bool LoadPanels()
	{
		if (panelReferences == null)
		{
			return false;
		}
		for (int i = 0; i < panelReferences.Length; i++)
		{
			GameObject gameObject = panelReferences[i].prefab.LoadAssetAsync<GameObject>().WaitForCompletion();
			if (gameObject == null)
			{
				Debug.LogWarning(base.gameObject.name + " Failed to load prefab: " + panelReferences[i].prefab.Asset.name);
				return false;
			}
			panelReferences[i].LoadedAsset = gameObject.GetComponent<Panel>();
			if (panelReferences[i].LoadedAsset == null)
			{
				Debug.LogWarning(base.gameObject.name + " Failed to load component: " + panelReferences[i].prefab.Asset.name);
				return false;
			}
		}
		return true;
	}

	public void HideAllPanels()
	{
		if (panelReferences == null)
		{
			return;
		}
		PanelReference[] array = panelReferences;
		for (int i = 0; i < array.Length; i++)
		{
			PanelReference panelReference = array[i];
			if (panelReference.CurrentInstance != null)
			{
				panelReference.CurrentInstance.Close();
			}
		}
	}

	public static UIManager Get()
	{
		return Singleton<UIManager>.Instance;
	}

	public static Canvas GetCanvas()
	{
		return Singleton<UIManager>.Instance.canvas;
	}
}
