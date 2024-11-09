namespace Code.Core.Network;

public class MilMo_LoginInfo
{
	public LoginResponse LoginResponse { get; private set; }

	public string BanReason { get; private set; }

	public string BanExpiration { get; private set; }

	public bool Success => LoginResponse == LoginResponse.Success;

	public MilMo_LoginInfo(LoginResponse loginResponse)
	{
		LoginResponse = loginResponse;
	}

	public MilMo_LoginInfo(LoginResponse loginResponse, string banReason, string banExpiration)
	{
		LoginResponse = loginResponse;
		BanReason = banReason;
		BanExpiration = banExpiration;
	}
}
