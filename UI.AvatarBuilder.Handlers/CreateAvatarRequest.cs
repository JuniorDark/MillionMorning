using System;
using System.Threading.Tasks;
using Code.Core.EventSystem;
using Code.Core.Global;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.World.CharBuilder;
using Core;
using UnityEngine;

namespace UI.AvatarBuilder.Handlers;

public class CreateAvatarRequest
{
	private static bool _requestInProcess;

	private MilMo_GenericReaction _createSuccessResponse;

	private MilMo_GenericReaction _createFailedResponse;

	private event Action<RequestStatus> OnComplete;

	public async Task<RequestStatus> Check(AvatarSelection selection)
	{
		if (selection == null)
		{
			return RequestStatus.Invalid;
		}
		if (_requestInProcess)
		{
			return RequestStatus.InProgress;
		}
		_requestInProcess = true;
		AddListener();
		TaskCompletionSource<RequestStatus> tcs = new TaskCompletionSource<RequestStatus>();
		this.OnComplete = delegate(RequestStatus result)
		{
			tcs.TrySetResult(result);
		};
		Singleton<GameNetwork>.Instance.RequestCreateAvatar(selection);
		if (await Task.WhenAny(tcs.Task, Task.Delay(25000)) == tcs.Task)
		{
			RemoveListener();
			_requestInProcess = false;
			return await tcs.Task;
		}
		RemoveListener();
		_requestInProcess = false;
		return RequestStatus.Timeout;
	}

	private void AddListener()
	{
		_createSuccessResponse = MilMo_EventSystem.Listen("avatar_created", OnSuccess);
		_createFailedResponse = MilMo_EventSystem.Listen("avatar_created_failed", OnFailure);
	}

	private void RemoveListener()
	{
		MilMo_EventSystem.RemoveReaction(_createSuccessResponse);
		_createSuccessResponse = null;
		MilMo_EventSystem.RemoveReaction(_createFailedResponse);
		_createFailedResponse = null;
	}

	private void OnSuccess(object obj)
	{
		if (obj is ServerAvatarCreated serverAvatarCreated)
		{
			Debug.Log("MilMo_Player::AvatarCreated");
			MilMo_Global.AuthorizationToken = serverAvatarCreated.getToken();
			this.OnComplete?.Invoke(RequestStatus.Valid);
		}
	}

	private void OnFailure(object msgAsObject)
	{
		Debug.Log("MilMo_Player::AvatarCreatedFailed");
		this.OnComplete?.Invoke(RequestStatus.Invalid);
	}
}
