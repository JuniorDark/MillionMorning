namespace Code.Core.Network;

public enum ConnectResponse
{
	Success,
	Failed,
	TimeOut,
	AlreadyConnected,
	BadHostName,
	UnknownError
}
