using Code.Core.Command;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_MoverEffectTemplate : MilMo_ObjectEffectTemplate
{
	private readonly float _pull;

	private readonly float _drag;

	private readonly Vector2 _startVelocityYRange = new Vector2(0f, 0f);

	private readonly float _xzSpeed;

	private readonly string _bounceEffect;

	private readonly bool _dontUseGroundHeight;

	private static float _bounceMoverPullTweaker;

	private static float _bounceMoverDragTweaker;

	private static float _bounceMoverYImpulseTweaker;

	public float Pull
	{
		get
		{
			if (_bounceMoverPullTweaker != 0f)
			{
				return _bounceMoverPullTweaker;
			}
			return _pull;
		}
	}

	public float Drag
	{
		get
		{
			if (_bounceMoverDragTweaker != 0f)
			{
				return _bounceMoverDragTweaker;
			}
			return _drag;
		}
	}

	public float StartVelocityY
	{
		get
		{
			if (_bounceMoverYImpulseTweaker != 0f)
			{
				return _bounceMoverYImpulseTweaker;
			}
			return Random.Range(_startVelocityYRange.x, _startVelocityYRange.y);
		}
	}

	public float XzSpeed => _xzSpeed;

	public string BounceEffect => _bounceEffect;

	public bool DontUseGroundHeight => _dontUseGroundHeight;

	public MilMo_MoverEffectTemplate(MilMo_SFFile file)
		: base(file)
	{
		while (file.HasMoreTokens())
		{
			if (file.IsNext("Pull"))
			{
				_pull = file.GetFloat();
			}
			else if (file.IsNext("Drag"))
			{
				_drag = file.GetFloat();
			}
			else if (file.IsNext("yImpulse"))
			{
				_startVelocityYRange = new Vector2(1f, 1f) * file.GetFloat();
			}
			else if (file.IsNext("yImpulseRandom"))
			{
				_startVelocityYRange = new Vector2(file.GetFloat(), file.GetFloat());
			}
			else if (file.IsNext("XZSpeed"))
			{
				_xzSpeed = file.GetFloat();
			}
			else if (file.IsNext("BounceEffect"))
			{
				_bounceEffect = file.GetString();
			}
			else if (file.IsNext("DontUseGroundHeight"))
			{
				_dontUseGroundHeight = true;
			}
			else
			{
				file.NextToken();
			}
		}
	}

	public override MilMo_ObjectEffect CreateObjectEffect(GameObject gameObject)
	{
		return new MilMo_MoverEffect(gameObject, this);
	}

	public static void RegisterCommands()
	{
		MilMo_Command.Instance.RegisterCommand("BounceMoverPull", Debug_BounceMoverPull);
		MilMo_Command.Instance.RegisterCommand("BounceMoverDrag", Debug_BounceMoverDrag);
		MilMo_Command.Instance.RegisterCommand("BounceMoverYImpulse", Debug_BounceMoverYImpulse);
	}

	private static string Debug_BounceMoverPull(string[] args)
	{
		if (args.Length < 2)
		{
			return "Global bounce mover pull is " + _bounceMoverPullTweaker;
		}
		_bounceMoverPullTweaker = MilMo_Utility.StringToFloat(args[1]);
		return "Global bounce mover pull set to " + _bounceMoverPullTweaker;
	}

	private static string Debug_BounceMoverDrag(string[] args)
	{
		if (args.Length < 2)
		{
			return "Global bounce mover drag is " + _bounceMoverDragTweaker;
		}
		_bounceMoverDragTweaker = MilMo_Utility.StringToFloat(args[1]);
		return "Global bounce mover drag set to " + _bounceMoverDragTweaker;
	}

	private static string Debug_BounceMoverYImpulse(string[] args)
	{
		if (args.Length < 2)
		{
			return "Global bounce mover Y-impulse is " + _bounceMoverYImpulseTweaker;
		}
		_bounceMoverYImpulseTweaker = MilMo_Utility.StringToFloat(args[1]);
		return "Global bounce mover Y-impulse set to " + _bounceMoverYImpulseTweaker;
	}
}
