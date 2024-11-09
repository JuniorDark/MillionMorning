using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using UnityEngine;

namespace Code.Core.Emote;

public sealed class MilMo_Emote : MilMo_Template
{
	public class FrameAction
	{
		public readonly MilMo_EmoteAction Action;

		public readonly float FrameTime;

		public FrameAction(MilMo_EmoteAction action, float frameTime)
		{
			Action = action;
			FrameTime = frameTime;
		}

		public FrameAction(FrameAction frameAction)
		{
			Action = frameAction.Action.Instantiate();
			FrameTime = frameAction.FrameTime;
		}
	}

	private readonly List<FrameAction> _actions = new List<FrameAction>();

	private readonly List<MilMo_TimerEvent> _queuedEvents = new List<MilMo_TimerEvent>();

	public string IconPath => "Generic/Characters/Icons/" + base.Name;

	public float Duration => _actions[_actions.Count - 1].FrameTime;

	public MilMo_Emote(string category, string path, string filePath)
		: base(category, path, filePath, "Emote")
	{
	}

	public MilMo_Emote(MilMo_Emote emote)
		: base(emote.Category, emote.Path, emote.FilePath, "Emote")
	{
		foreach (FrameAction action in emote._actions)
		{
			_actions.Add(new FrameAction(action));
		}
		foreach (MilMo_TimerEvent queuedEvent in emote._queuedEvents)
		{
			_queuedEvents.Add(new MilMo_TimerEvent(queuedEvent.Time, queuedEvent.Action));
		}
	}

	public MilMo_Emote(MilMo_Face face)
		: base("<mood>", "<mood>", "<mood>", "Emote")
	{
		foreach (FrameAction action in face.Actions)
		{
			_actions.Add(new FrameAction(action));
		}
	}

	public void Execute(MilMo_EmoteManager manager)
	{
		foreach (FrameAction action in _actions)
		{
			_queuedEvents.Add(MilMo_EventSystem.At(action.FrameTime, action.Action.Execute, manager));
		}
	}

	public void Abort(MilMo_EmoteManager manager)
	{
		foreach (MilMo_TimerEvent queuedEvent in _queuedEvents)
		{
			MilMo_EventSystem.RemoveTimerEvent(queuedEvent);
		}
		_queuedEvents.Clear();
	}

	public void Pause()
	{
		foreach (MilMo_TimerEvent queuedEvent in _queuedEvents)
		{
			queuedEvent.Pause();
		}
	}

	public void Unpause()
	{
		foreach (MilMo_TimerEvent queuedEvent in _queuedEvents)
		{
			queuedEvent.Unpause();
		}
	}

	public override bool ReadLine(MilMo_SFFile file)
	{
		file.GetString();
		float @float = file.GetFloat();
		switch (file.GetString())
		{
		case "SnapUV":
		{
			MilMo_EmoteAction milMo_EmoteAction4 = new MilMo_EmoteActionSnapUV();
			milMo_EmoteAction4.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction4, @float));
			break;
		}
		case "SnapU":
		{
			MilMo_EmoteAction milMo_EmoteAction3 = new MilMo_EmoteActionSnapU();
			milMo_EmoteAction3.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction3, @float));
			break;
		}
		case "SnapV":
		{
			MilMo_EmoteAction milMo_EmoteAction16 = new MilMo_EmoteActionSnapV();
			milMo_EmoteAction16.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction16, @float));
			break;
		}
		case "GotoUV":
		{
			MilMo_EmoteAction milMo_EmoteAction15 = new MilMo_EmoteActionGotoUV();
			milMo_EmoteAction15.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction15, @float));
			break;
		}
		case "Rotate":
		{
			MilMo_EmoteAction milMo_EmoteAction14 = new MilMo_EmoteActionUVRotate();
			milMo_EmoteAction14.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction14, @float));
			break;
		}
		case "SnapRotate":
		{
			MilMo_EmoteAction milMo_EmoteAction13 = new MilMo_EmoteActionUVRotateSnap();
			milMo_EmoteAction13.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction13, @float));
			break;
		}
		case "PlayAnim":
		{
			MilMo_EmoteAction milMo_EmoteAction12 = new MilMo_EmoteActionPlayAnim();
			milMo_EmoteAction12.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction12, @float));
			break;
		}
		case "GotoBone":
		{
			MilMo_EmoteAction milMo_EmoteAction11 = new MilMo_EmoteActionGotoBone();
			milMo_EmoteAction11.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction11, @float));
			break;
		}
		case "EndEmote":
		{
			MilMo_EmoteAction milMo_EmoteAction10 = new MilMo_EmoteActionEnd();
			milMo_EmoteAction10.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction10, @float));
			break;
		}
		case "SetMood":
		{
			MilMo_EmoteAction milMo_EmoteAction9 = new MilMo_EmoteActionSetMood();
			milMo_EmoteAction9.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction9, @float));
			break;
		}
		case "DisableSuperAliveness":
		{
			MilMo_EmoteAction milMo_EmoteAction8 = new MilMo_EmoteActionDisableSuperAliveness();
			milMo_EmoteAction8.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction8, @float));
			break;
		}
		case "EnableSuperAliveness":
		{
			MilMo_EmoteAction milMo_EmoteAction7 = new MilMo_EmoteActionEnableSuperAliveness();
			milMo_EmoteAction7.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction7, @float));
			break;
		}
		case "DisableSuperAlivenessEyes":
		{
			MilMo_EmoteAction milMo_EmoteAction6 = new MilMo_EmoteActionDisableSuperAlivenessEyes();
			milMo_EmoteAction6.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction6, @float));
			break;
		}
		case "EnableSuperAlivenessEyes":
		{
			MilMo_EmoteAction milMo_EmoteAction5 = new MilMo_EmoteActionEnableSuperAlivenessEyes();
			milMo_EmoteAction5.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction5, @float));
			break;
		}
		case "Face":
		{
			string @string = file.GetString();
			MilMo_Face face = MilMo_Face.GetFace(@string);
			if (face == null)
			{
				Debug.LogWarning("Face '" + @string + "' does not exist");
				break;
			}
			foreach (FrameAction action in face.Actions)
			{
				_actions.Add(new FrameAction(action.Action, @float + action.FrameTime));
			}
			break;
		}
		case "Wield":
		{
			MilMo_EmoteAction milMo_EmoteAction2 = new MilMo_EmoteActionWield();
			milMo_EmoteAction2.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction2, @float));
			break;
		}
		case "Scale":
		{
			MilMo_EmoteAction milMo_EmoteAction = new MilMo_EmoteActionScale();
			milMo_EmoteAction.Read(file);
			_actions.Add(new FrameAction(milMo_EmoteAction, @float));
			break;
		}
		}
		return true;
	}
}
