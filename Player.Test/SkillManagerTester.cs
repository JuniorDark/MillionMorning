using System.Collections.Generic;
using Code.Core.Network.types;
using UnityEngine;

namespace Player.Test;

public class SkillManagerTester : MonoBehaviour
{
	private SkillManager _skillManager;

	private void Awake()
	{
		_skillManager = SkillManager.GetPlayerSkillManager();
	}

	public void AddFakeSkill()
	{
		Debug.LogWarning("AddFakeSkill");
		List<SkillTemplate> list = new List<SkillTemplate>();
		int num = 1;
		List<SkillMode> list2 = new List<SkillMode>();
		string desc = "It's fake";
		string icon = "ClassIconProtect";
		int num2 = 10;
		SkillMode item = new SkillMode("Protect", desc, icon, num2);
		list2.Add(item);
		SkillTemplate item2 = new SkillTemplate("DPS", (sbyte)num, list2);
		list.Add(item2);
		Debug.LogWarning("Template ready");
	}
}
