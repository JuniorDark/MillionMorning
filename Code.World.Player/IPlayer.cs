using Code.Core.Avatar;

namespace Code.World.Player;

public interface IPlayer
{
	MilMo_Avatar Avatar { get; }

	string Id { get; }

	bool IsLocalPlayer { get; }

	bool InSpline { get; }
}
