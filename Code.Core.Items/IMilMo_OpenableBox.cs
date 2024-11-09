namespace Code.Core.Items;

public interface IMilMo_OpenableBox
{
	string IconPathClosed { get; }

	string IconPathOpen { get; }

	void PostOpenEvent();
}
