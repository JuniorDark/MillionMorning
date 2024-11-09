using System.Collections.Generic;
using Code.Core.EventSystem;
using UI.HUD.Dialogues;

namespace Code.World.Achievements;

public class MilMo_MedalCategory
{
	public enum MedalCategory
	{
		Gems,
		Wealth,
		StarTokens,
		PVP,
		LevelExploration,
		SpookyExploration,
		W00Exploration,
		W03Exploration,
		Exploration,
		Events,
		Axes,
		Bows,
		Clubs,
		Guns,
		Swords,
		Wands,
		Mining,
		Questing,
		Miscellaneous,
		Achievements,
		Excavation,
		Death,
		Warrior,
		Converters,
		Spending,
		Gifts,
		OpenLockBox,
		OpenMysteryBox,
		Devourers,
		Invaders,
		Aliens,
		Beast,
		Bees,
		Birds,
		Bunny,
		Butterflies,
		Crabs,
		Dino,
		Snails,
		Ghosts,
		Goats,
		Hedgehogs,
		Insects,
		Onions,
		BallBird,
		RedBalls,
		Rhubarb,
		Robots,
		Skulls,
		Statues,
		Trunks,
		WarriorBird,
		Weeds,
		Ladybugs,
		Scarabs,
		Piranhas,
		Snakes,
		Golems,
		Vamps,
		Unknown
	}

	public static readonly Dictionary<MedalCategory, string> MedalCategoryLocales = new Dictionary<MedalCategory, string>
	{
		{
			MedalCategory.Gems,
			"MedalCategories_311"
		},
		{
			MedalCategory.Wealth,
			"MedalCategories_9599"
		},
		{
			MedalCategory.StarTokens,
			"MedalCategories_12760"
		},
		{
			MedalCategory.PVP,
			"MedalCategories_9598"
		},
		{
			MedalCategory.LevelExploration,
			"NPCs_W00_L00_8597"
		},
		{
			MedalCategory.SpookyExploration,
			"NPCs_W00_L00_8600"
		},
		{
			MedalCategory.W00Exploration,
			"MedalCategories_285"
		},
		{
			MedalCategory.W03Exploration,
			"NPCs_W00_L00_8603"
		},
		{
			MedalCategory.Exploration,
			"MedalCategories_299"
		},
		{
			MedalCategory.Events,
			"MedalCategories_2470"
		},
		{
			MedalCategory.Axes,
			"MedalCategories_293"
		},
		{
			MedalCategory.Bows,
			"MedalCategories_295"
		},
		{
			MedalCategory.Clubs,
			"MedalCategories_286"
		},
		{
			MedalCategory.Guns,
			"MedalCategories_304"
		},
		{
			MedalCategory.Swords,
			"MedalCategories_307"
		},
		{
			MedalCategory.Wands,
			"MedalCategories_13277"
		},
		{
			MedalCategory.Mining,
			"MedalCategories_12315"
		},
		{
			MedalCategory.Questing,
			"MedalCategories_303"
		},
		{
			MedalCategory.Miscellaneous,
			"MedalCategories_301"
		},
		{
			MedalCategory.Achievements,
			"MedalCategories_291"
		},
		{
			MedalCategory.Excavation,
			"MedalCategories_298"
		},
		{
			MedalCategory.Death,
			"MedalCategories_297"
		},
		{
			MedalCategory.Warrior,
			"MedalCategories_309"
		},
		{
			MedalCategory.Converters,
			"MedalCategories_12761"
		},
		{
			MedalCategory.Spending,
			"MedalCategories_7947"
		},
		{
			MedalCategory.Gifts,
			"MedalCategories_5647"
		},
		{
			MedalCategory.OpenLockBox,
			"MedalCategories_8458"
		},
		{
			MedalCategory.OpenMysteryBox,
			"MedalCategories_8459"
		},
		{
			MedalCategory.Devourers,
			"MedalCategories_8025"
		},
		{
			MedalCategory.Invaders,
			"MedalCategories_12983"
		},
		{
			MedalCategory.Aliens,
			"MedalCategories_292"
		},
		{
			MedalCategory.Beast,
			"MedalCategories_9376"
		},
		{
			MedalCategory.Bees,
			"MedalCategories_294"
		},
		{
			MedalCategory.Birds,
			"MedalCategories_287"
		},
		{
			MedalCategory.Bunny,
			"MedalCategories_7675"
		},
		{
			MedalCategory.Butterflies,
			"MedalCategories_296"
		},
		{
			MedalCategory.Crabs,
			"MedalCategories_288"
		},
		{
			MedalCategory.Dino,
			"MedalCategories_8788"
		},
		{
			MedalCategory.Snails,
			"MedalCategories_289"
		},
		{
			MedalCategory.Ghosts,
			"MedalCategories_4902"
		},
		{
			MedalCategory.Goats,
			"MedalCategories_290"
		},
		{
			MedalCategory.Hedgehogs,
			"MedalCategories_300"
		},
		{
			MedalCategory.Insects,
			"MedalCategories_4903"
		},
		{
			MedalCategory.Onions,
			"MedalCategories_302"
		},
		{
			MedalCategory.BallBird,
			"MedalCategories_8457"
		},
		{
			MedalCategory.RedBalls,
			"MedalCategories_4654"
		},
		{
			MedalCategory.Rhubarb,
			"MedalCategories_305"
		},
		{
			MedalCategory.Robots,
			"MedalCategories_5601"
		},
		{
			MedalCategory.Skulls,
			"MedalCategories_306"
		},
		{
			MedalCategory.Statues,
			"MedalCategories_6584"
		},
		{
			MedalCategory.Trunks,
			"MedalCategories_308"
		},
		{
			MedalCategory.WarriorBird,
			"MedalCategories_11705"
		},
		{
			MedalCategory.Weeds,
			"MedalCategories_310"
		},
		{
			MedalCategory.Ladybugs,
			"MedalCategories_Ladybug"
		},
		{
			MedalCategory.Scarabs,
			"MedalCategories_Scarab"
		},
		{
			MedalCategory.Piranhas,
			"MedalCategories_Piranha"
		},
		{
			MedalCategory.Snakes,
			"MedalCategories_Snake"
		},
		{
			MedalCategory.Golems,
			"MedalCategories_Golem"
		},
		{
			MedalCategory.Vamps,
			"MedalCategories_Vamp"
		}
	};

	public readonly LinkedList<MilMo_Medal> Medals = new LinkedList<MilMo_Medal>();

	public string Identifier { get; }

	public MilMo_MedalCategory(string identifier)
	{
		Identifier = identifier;
	}

	public void AddMedal(MilMo_Medal medal)
	{
		LinkedListNode<MilMo_Medal> linkedListNode;
		for (linkedListNode = Medals.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (linkedListNode.Value.Template == medal.Template)
			{
				linkedListNode.Value.Acquired = medal.Acquired;
				return;
			}
		}
		for (linkedListNode = Medals.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (linkedListNode.Value.Template.CategoryIndex > medal.Template.CategoryIndex)
			{
				Medals.AddBefore(linkedListNode, medal);
				MilMo_EventSystem.Instance.PostEvent("medal_added", medal);
				break;
			}
		}
		if (linkedListNode == null)
		{
			Medals.AddLast(medal);
			MilMo_EventSystem.Instance.PostEvent("medal_added", medal);
		}
	}

	public void SetAsCompleted(MilMo_AchievementTemplate template)
	{
		for (LinkedListNode<MilMo_Medal> linkedListNode = Medals.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (linkedListNode.Value.Template == template)
			{
				linkedListNode.Value.Acquired = true;
				DialogueSpawner.SpawnMedalAcquired(linkedListNode.Value.Template);
				break;
			}
		}
	}

	public MilMo_AchievementTemplate GetNext(MilMo_AchievementTemplate medal)
	{
		LinkedListNode<MilMo_Medal> linkedListNode = Medals.First;
		if (linkedListNode == null)
		{
			return null;
		}
		while (linkedListNode.Next != null)
		{
			if (linkedListNode.Value.Template.Identifier.Equals(medal.Identifier))
			{
				return linkedListNode.Next.Value.Template;
			}
			linkedListNode = linkedListNode.Next;
		}
		return null;
	}
}
