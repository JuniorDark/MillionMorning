using UnityEngine;

namespace Code.Core.Utility;

public static class MilMo_NameGenerator
{
	public static string GetName()
	{
		string text = "";
		int num = Random.Range(5, 12);
		bool flag = Random.Range(0, 2) != 0;
		for (int i = 0; i < num; i++)
		{
			if (flag)
			{
				char randomVowel = GetRandomVowel();
				text = ((i != 0) ? (text + randomVowel) : (text + randomVowel.ToString().ToUpper()));
				if (Random.Range(0, 25) == 0)
				{
					text += randomVowel;
				}
				flag = false;
			}
			else
			{
				char randomConsonant = GetRandomConsonant();
				text = ((i != 0) ? (text + randomConsonant) : (text + randomConsonant.ToString().ToUpper()));
				flag = true;
			}
		}
		return text;
	}

	private static char GetRandomVowel()
	{
		int num;
		do
		{
			num = Random.Range(1, 27) + 96;
		}
		while ((num != 97 && num != 101 && num != 105 && num != 111 && num != 117) || num == 121);
		return (char)num;
	}

	private static char GetRandomConsonant()
	{
		int num;
		do
		{
			num = Random.Range(1, 27) + 96;
		}
		while (num == 97 || num == 101 || num == 105 || num == 111 || num == 117 || num == 120 || num == 121 || num == 122 || num == 113 || num == 119 || num == 106 || num == 99);
		return (char)num;
	}
}
