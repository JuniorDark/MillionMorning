using Code.Core.ResourceSystem;

namespace Code.World.Tutorial;

public interface ITutorialData
{
	string TargetImagePath { get; }

	string KeyBindImagePath { get; }

	MilMo_LocString Headline { get; }

	MilMo_LocString Text { get; }

	string ArrowTarget { get; }
}
