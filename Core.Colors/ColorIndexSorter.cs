using System.Collections.Generic;

namespace Core.Colors;

public class ColorIndexSorter : IComparer<ScriptableColor>
{
	public int Compare(ScriptableColor x, ScriptableColor y)
	{
		if (x == null)
		{
			return -1;
		}
		if (y == null)
		{
			return -1;
		}
		int num = int.Parse(x.GetIdentifier());
		int num2 = int.Parse(y.GetIdentifier());
		if (num > num2)
		{
			return 1;
		}
		if (num != num2)
		{
			return -1;
		}
		return 0;
	}
}
