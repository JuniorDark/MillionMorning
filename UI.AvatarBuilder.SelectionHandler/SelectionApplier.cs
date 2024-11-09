using System.Collections.Generic;
using Code.World.CharBuilder;
using UnityEngine;

namespace UI.AvatarBuilder.SelectionHandler;

public class SelectionApplier : MonoBehaviour
{
	private AvatarEditor _avatarEditor;

	[SerializeField]
	private List<SelectionHandling> handlers;

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		_avatarEditor = Object.FindObjectOfType<AvatarEditor>();
		if (_avatarEditor == null)
		{
			Debug.LogError(base.name + ": Unable to get AvatarEditor!");
			return;
		}
		AddListeners();
		if (_avatarEditor.IsInitialized())
		{
			ReInitHandlers();
		}
	}

	private void OnDestroy()
	{
		RemoveListeners();
	}

	private void AddListeners()
	{
		if ((bool)_avatarEditor)
		{
			_avatarEditor.OnSelectionReset += ReInitHandlers;
			_avatarEditor.OnGenderChanged += OnGenderChange;
			_avatarEditor.OnColorChangedForShape += ColorChangedForShape;
		}
	}

	private void RemoveListeners()
	{
		if ((bool)_avatarEditor)
		{
			_avatarEditor.OnSelectionReset -= ReInitHandlers;
			_avatarEditor.OnGenderChanged -= OnGenderChange;
			_avatarEditor.OnColorChangedForShape -= ColorChangedForShape;
		}
	}

	private void OnGenderChange()
	{
		foreach (SelectionHandling handler in handlers)
		{
			handler.GenderSwitch();
		}
	}

	private void ReInitHandlers()
	{
		foreach (SelectionHandling handler in handlers)
		{
			handler.Init();
		}
	}

	private void ColorChangedForShape(AvatarSelection.Shapes shape)
	{
		foreach (SelectionHandling handler in handlers)
		{
			handler.ColorChanged(shape);
		}
	}
}
