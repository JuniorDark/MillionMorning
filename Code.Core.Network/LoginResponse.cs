namespace Code.Core.Network;

public enum LoginResponse
{
	Success,
	BadUserOrPassword,
	UserAlreadyLoggedOn,
	Disconnected,
	ServerCantBeTrusted,
	UnknownError,
	TimeOut,
	NotConnected,
	AlreadyLoggedIn,
	AlreadyLoggingIn,
	LoggingOut,
	Banned
}
