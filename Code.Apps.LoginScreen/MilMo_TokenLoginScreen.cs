using System;
using Code.Core.Config;
using Code.Core.Global;
using UnityEngine;

namespace Code.Apps.LoginScreen;

internal class MilMo_TokenLoginScreen : MilMo_Login
{
	public new void Awake()
	{
		base.Awake();
		if (MilMo_Config.Instance.IsSet("Debug.Login.Token"))
		{
			string value = MilMo_Config.Instance.GetValue("Debug.Login.Token");
			if (value != "")
			{
				Debug.LogWarning("Using token from config: " + value);
				MilMo_Login.SetUserToken(value);
			}
		}
		if (MilMo_Global.AuthorizationToken == null)
		{
			throw new NotImplementedException();
		}
		DoTokenLogin(null);
	}
}
