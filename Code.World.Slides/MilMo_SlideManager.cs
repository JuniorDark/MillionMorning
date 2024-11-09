using System;
using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.GUI.Core;
using Code.Core.Network;
using Code.Core.Template;
using Code.World.GUI.Slides;
using Core;
using Core.Analytics;
using UnityEngine;

namespace Code.World.Slides;

public sealed class MilMo_SlideManager
{
	private static MilMo_SlideManager _instance;

	private IList<string> _seenSlides = new List<string>();

	private bool _inSlideShow;

	private readonly bool _showSlides;

	public static MilMo_SlideManager Instance => _instance ?? (_instance = new MilMo_SlideManager());

	private MilMo_SlideManager()
	{
		_showSlides = true;
	}

	public void SetSeenSlides(IList<string> seenSlides)
	{
		_seenSlides = seenSlides;
	}

	public void StartSlideShow(string path, bool dontShowIfAlreadyShown, MilMo_EventSystem.MilMo_Callback slidesDoneCallback)
	{
		if (_inSlideShow)
		{
			throw new InvalidOperationException("Can't start a new slide show when a slide show is already running");
		}
		if (!_showSlides || (dontShowIfAlreadyShown && _seenSlides.Contains(path)))
		{
			slidesDoneCallback?.Invoke();
			return;
		}
		MilMo_Global.Camera.backgroundColor = Color.black;
		if (!(Singleton<MilMo_TemplateContainer>.Instance.GetTemplate("Slides", "Slides." + path) is MilMo_SlidesTemplate template))
		{
			Debug.LogWarning("Failed to start slide show " + path + ": The template could not be found.");
			slidesDoneCallback?.Invoke();
			return;
		}
		MilMo_UserInterface ui = GetSlidesUI();
		ui.Enabled = true;
		Analytics.CutsceneStart("Intro");
		new MilMo_SlidesView(ui, template, delegate
		{
			ui.Enabled = false;
			_inSlideShow = false;
			Analytics.CutsceneEnd("Intro");
			if (Singleton<GameNetwork>.Instance.IsConnectedToGameServer)
			{
				Singleton<GameNetwork>.Instance.RequestSaveSeenSlide(path);
			}
			slidesDoneCallback?.Invoke();
		});
	}

	private static MilMo_UserInterface GetSlidesUI()
	{
		MilMo_UserInterface userInterface = MilMo_UserInterfaceManager.GetUserInterface("SlidesUI");
		if (userInterface != null)
		{
			return userInterface;
		}
		userInterface = MilMo_UserInterfaceManager.CreateUserInterface("SlidesUI");
		MilMo_UserInterfaceManager.SetUserInterfaceDepth(userInterface, -3001);
		userInterface.ResetLayout();
		userInterface.ScreenSizeDirty = true;
		return userInterface;
	}
}
