using Code.Core.ResourceSystem;
using Code.Core.Utility;

namespace Core.Utilities;

public static class ValidNameGenerator
{
	public static string GetValidName()
	{
		bool flag = false;
		string name;
		do
		{
			name = MilMo_NameGenerator.GetName();
			if (MilMo_BadWordFilter.GetStringIntegrity(name) == MilMo_BadWordFilter.StringIntegrity.OK)
			{
				flag = true;
			}
		}
		while (!flag);
		return name;
	}
}
