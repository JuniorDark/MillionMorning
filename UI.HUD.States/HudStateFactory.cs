namespace UI.HUD.States;

public class HudStateFactory
{
	private readonly HUD _context;

	public HudStateFactory(HUD currentContext)
	{
		_context = currentContext;
	}

	public HudState Initial()
	{
		return new InitialHudState(_context);
	}

	public HudState InHome()
	{
		return new HomeHudState(_context);
	}

	public HudState InLevel()
	{
		return new LevelHudState(_context);
	}

	public HudState InStartLevel()
	{
		return new StartLevelHudState(_context);
	}

	public HudState InPVP()
	{
		return new PVPHudState(_context);
	}

	public HudState InPVPAbilities()
	{
		return new PVPAbilitiesHudState(_context);
	}
}
