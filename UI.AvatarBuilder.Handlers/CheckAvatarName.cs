using System;
using System.Threading.Tasks;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.World.Player;
using Core;

namespace UI.AvatarBuilder.Handlers;

public class CheckAvatarName
{
	private MilMo_GenericReaction _nameResponseReaction;

	private bool _isCheckingAvatarName;

	private event Action<AvatarNameResponse> OnComplete;

	private void OnResponse(object msgAsObject)
	{
		_isCheckingAvatarName = false;
		if (!(msgAsObject is ServerCheckAvatarNameResponse serverCheckAvatarNameResponse))
		{
			this.OnComplete?.Invoke(AvatarNameResponse.Invalid);
			return;
		}
		switch (serverCheckAvatarNameResponse.getResponse())
		{
		case 1:
			this.OnComplete?.Invoke(AvatarNameResponse.Valid);
			break;
		case 2:
			this.OnComplete?.Invoke(AvatarNameResponse.Invalid);
			break;
		case 3:
			this.OnComplete?.Invoke(AvatarNameResponse.Taken);
			break;
		default:
			this.OnComplete?.Invoke(AvatarNameResponse.Invalid);
			break;
		}
	}

	public async Task<AvatarNameResponse> Check(string avatarName)
	{
		if (_isCheckingAvatarName)
		{
			return AvatarNameResponse.CheckInProgress;
		}
		if (avatarName == null)
		{
			return AvatarNameResponse.Invalid;
		}
		_isCheckingAvatarName = true;
		AddListener();
		TaskCompletionSource<AvatarNameResponse> tcs = new TaskCompletionSource<AvatarNameResponse>();
		OnComplete += delegate(AvatarNameResponse response)
		{
			tcs.TrySetResult(response);
		};
		Singleton<GameNetwork>.Instance.CheckAvatarName(avatarName);
		if (await Task.WhenAny(tcs.Task, Task.Delay(25000)) == tcs.Task)
		{
			RemoveListener();
			return await tcs.Task;
		}
		RemoveListener();
		return AvatarNameResponse.Timeout;
	}

	private void AddListener()
	{
		_nameResponseReaction = MilMo_EventSystem.Listen("check_avatar_name_response", OnResponse);
	}

	private void RemoveListener()
	{
		MilMo_EventSystem.RemoveReaction(_nameResponseReaction);
		_nameResponseReaction = null;
	}
}
