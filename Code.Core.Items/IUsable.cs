using System;

namespace Code.Core.Items;

public interface IUsable
{
	bool Use(int entryId);

	void RegisterOnUsed(Action onUsed);

	void UnregisterOnUsed(Action onUsed);

	void RegisterOnFailedToUse(Action onFail);

	void UnregisterOnFailedToUse(Action onFail);
}
