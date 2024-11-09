using System;
using UnityEngine;

namespace Code.Core.EventSystem;

public class MilMo_EventSystemRunner : MonoBehaviour
{
	private void Start()
	{
		if (!MilMo_EventSystem.Instance.SetRunner(this))
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void Update()
	{
		try
		{
			MilMo_EventSystem.Instance.Update();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private void FixedUpdate()
	{
		try
		{
			MilMo_EventSystem.Instance.FixedUpdate();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private void LateUpdate()
	{
		try
		{
			MilMo_EventSystem.Instance.LateUpdate();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private void OnPreRender()
	{
		MilMo_EventSystem.Instance.OnPreRender();
	}
}
