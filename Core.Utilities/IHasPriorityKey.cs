namespace Core.Utilities;

public interface IHasPriorityKey
{
	int GetPriority();

	bool Equals(IHasPriorityKey other);
}
