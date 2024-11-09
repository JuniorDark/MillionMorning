namespace Code.Core.Network.nexus.actions;

public interface IAction
{
	void Accept(INexusListener listener);
}
