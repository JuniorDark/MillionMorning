using Code.Core.EventSystem;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Template;
using Code.Core.Visual;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.World.Gameplay;

public class MilMo_GameplayObjectMover
{
	private static GameObject _playerOnPlatform;

	private static int _playerOnPlatformLastUpdatedFrame;

	private static float _playerOnPlatformDistance;

	private readonly MilMo_GameplayObject _gameplayObject;

	private MilMo_GameObjectSplineTemplate _splineTemplate;

	private MilMo_GameObjectSpline _spline;

	private MilMo_GameObjectSpline _invertedSpline;

	private int _direction;

	private float _time;

	private bool _isAtEnd;

	private MilMo_VisualRep _objectVisualRep;

	private MilMo_GameObjectSpline.SplinePoint _currentPoint;

	private GameObject _soundObject;

	private AudioSourceWrapper _soundObjectAudioComponent;

	private AudioClip _backgroundSound;

	private bool _startBackgroundSound;

	private bool _didStartBackgroundSound;

	private float _initialPositionTime;

	private bool _initialized;

	private float _lastServerUpdateTime;

	private MilMo_GenericReaction _activateWorldReaction;

	public MilMo_GameplayObjectMover(MilMo_GameplayObject gameplayObject, TemplateReference splineRef, float time)
	{
		_gameplayObject = gameplayObject;
		_direction = (int)Mathf.Sign(time);
		_time = Mathf.Abs(time);
		_initialPositionTime = Time.time;
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(splineRef, SplineTemplateArrived);
	}

	private void SplineTemplateArrived(MilMo_Template template, bool timeOut)
	{
		if (timeOut || template == null)
		{
			Debug.LogWarning("Failed to load spline template for gameplay object mover");
			return;
		}
		_splineTemplate = template as MilMo_GameObjectSplineTemplate;
		if (_splineTemplate == null)
		{
			Debug.LogWarning("Got wrong template type when loading spline template for gameplay object mover");
		}
		else
		{
			CreateSplines();
		}
	}

	private void CreateSplines()
	{
		if (_spline != null)
		{
			_spline.Clear();
		}
		if (_invertedSpline != null)
		{
			_invertedSpline.Clear();
		}
		_spline = new MilMo_GameObjectSpline(_splineTemplate);
		_invertedSpline = _spline.GetInvertedSpline();
		if (!string.IsNullOrEmpty(_spline.Template.BackgroundSound))
		{
			LoadBackgroundSoundAsync(_spline.Template.BackgroundSound);
		}
		if (_objectVisualRep != null && _objectVisualRep.GameObject != null)
		{
			Initialize();
		}
	}

	private async void LoadBackgroundSoundAsync(string path)
	{
		_backgroundSound = await MilMo_ResourceManager.Instance.LoadAudioAsync(path);
	}

	private void Initialize()
	{
		if (_initialized)
		{
			return;
		}
		MilMo_GameObjectSpline milMo_GameObjectSpline = ((_direction == -1) ? _invertedSpline : _spline);
		if (_objectVisualRep == null || _objectVisualRep.GameObject == null || milMo_GameObjectSpline == null)
		{
			return;
		}
		_time += Time.time - _initialPositionTime;
		_lastServerUpdateTime = Time.time;
		_currentPoint = milMo_GameObjectSpline.GetPointAtTime(_time);
		_isAtEnd = milMo_GameObjectSpline.IsAtEnd(_time);
		if (!string.IsNullOrEmpty(_currentPoint.Animation))
		{
			_objectVisualRep.PlayAnimation(_currentPoint.Animation, WrapMode.Loop);
		}
		if (_soundObject == null)
		{
			_soundObject = new GameObject("MoverSound");
			_soundObject.transform.parent = _objectVisualRep.GameObject.transform;
			_soundObject.transform.localPosition = Vector3.zero;
			_soundObjectAudioComponent = _soundObject.AddComponent<AudioSourceWrapper>();
			_soundObjectAudioComponent.Loop = false;
			AudioSourceWrapper component = _objectVisualRep.GameObject.GetComponent<AudioSourceWrapper>();
			if (component != null)
			{
				_soundObjectAudioComponent.MaxDistance = component.MaxDistance;
				_soundObjectAudioComponent.RolloffMode = component.RolloffMode;
				_soundObjectAudioComponent.Volume = component.Volume;
			}
		}
		_startBackgroundSound = true;
		_activateWorldReaction = MilMo_EventSystem.Listen("world_activated", Unpause);
		_activateWorldReaction.Repeating = true;
		_initialized = true;
	}

	public void SetGameObjectVisualRep(MilMo_VisualRep objectVisualRep)
	{
		_objectVisualRep = objectVisualRep;
		Initialize();
		if (_objectVisualRep != null && _objectVisualRep.GameObject != null)
		{
			_objectVisualRep.GameObject.layer = 21;
		}
	}

	public void GotStartMovingMessage(TemplateReference splineRef)
	{
		_time = 0f;
		_direction = 1;
		if (_splineTemplate != null && splineRef.GetPath().Equals(_splineTemplate.Path) && splineRef.GetCategory().Equals(_splineTemplate.Category))
		{
			_lastServerUpdateTime = Time.time;
			PlayBackgroundSound();
		}
		else
		{
			_initialPositionTime = Time.time;
			_initialized = false;
			Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(splineRef, SplineTemplateArrived);
		}
	}

	public void GotChangeDirectionMessage(int direction)
	{
		if (!_initialized)
		{
			_initialPositionTime = Time.time;
		}
		else
		{
			_lastServerUpdateTime = Time.time;
		}
		_time = 0f;
		_direction = direction;
		PlayBackgroundSound();
	}

	public void Update()
	{
		if (!_initialized)
		{
			return;
		}
		MilMo_GameObjectSpline milMo_GameObjectSpline = ((_direction == -1) ? _invertedSpline : _spline);
		if (milMo_GameObjectSpline == null || _objectVisualRep == null || _objectVisualRep.GameObject == null)
		{
			_time += Time.deltaTime;
			return;
		}
		Vector3 position = _objectVisualRep.GameObject.transform.position;
		MilMo_GameObjectSpline.SplinePoint pointAtTime = milMo_GameObjectSpline.GetPointAtTime(_time);
		bool flag = GetPlayerOnPlatform() == _objectVisualRep.GameObject;
		Vector3 vector = pointAtTime.Position - position;
		if (vector != Vector3.zero)
		{
			bool flag2 = vector.y <= 0f;
			if (flag2)
			{
				_objectVisualRep.GameObject.transform.position = pointAtTime.Position;
			}
			if (flag)
			{
				MilMo_PlayerControllerBase.PlatformMovePlayer(MilMo_Player.Instance.Avatar.Position + vector, printDebug: false);
				if (!flag2 && _playerOnPlatformDistance < 0.4f)
				{
					Vector3 currentSpeed = MilMo_PlayerControllerBase.CurrentSpeed;
					currentSpeed.y = Mathf.Max(currentSpeed.y, 0f);
					MilMo_PlayerControllerBase.CurrentSpeed = currentSpeed;
				}
			}
			if (!flag2)
			{
				_objectVisualRep.GameObject.transform.position = pointAtTime.Position;
			}
		}
		if (flag && !MilMo_Player.Instance.IsOnMovingPlatform(_gameplayObject))
		{
			MilMo_Player.Instance.EnterGameplayObject(_gameplayObject);
		}
		else if (!flag && MilMo_Player.Instance.IsOnMovingPlatform(_gameplayObject))
		{
			MilMo_Player.Instance.LeaveGameplayObject();
		}
		if (pointAtTime.Animation != _currentPoint.Animation)
		{
			_objectVisualRep.PlayAnimation(pointAtTime.Animation, WrapMode.Loop);
		}
		if (!string.IsNullOrEmpty(pointAtTime.Sound) && pointAtTime.Sound != _currentPoint.Sound)
		{
			LoadAndPlayAsync(pointAtTime.Sound);
		}
		_currentPoint = pointAtTime;
		bool isAtEnd = _isAtEnd;
		_isAtEnd = _spline.IsAtEnd(_time);
		if (_isAtEnd && !isAtEnd)
		{
			_soundObjectAudioComponent.Pause();
		}
		_time += Time.deltaTime;
		if (_startBackgroundSound && !_didStartBackgroundSound && _backgroundSound != null && !_isAtEnd)
		{
			PlayBackgroundSound();
		}
	}

	private async void LoadAndPlayAsync(string path)
	{
		AudioClip audioClip = await MilMo_ResourceManager.Instance.LoadAudioAsync(path);
		if (!(audioClip == null))
		{
			_soundObjectAudioComponent.Clip = audioClip;
			_soundObjectAudioComponent.Loop = false;
			_soundObjectAudioComponent.Play();
		}
	}

	public void Destroy()
	{
		MilMo_EventSystem.RemoveReaction(_activateWorldReaction);
	}

	private void PlayBackgroundSound()
	{
		if (!(_soundObjectAudioComponent == null) && !(_backgroundSound == null))
		{
			_soundObjectAudioComponent.Loop = true;
			_soundObjectAudioComponent.Clip = _backgroundSound;
			_soundObjectAudioComponent.Play();
			_didStartBackgroundSound = true;
		}
	}

	private void Unpause(object o)
	{
		float num = Mathf.Min((float)o, Time.time - _lastServerUpdateTime);
		_time += num;
	}

	private static GameObject GetPlayerOnPlatform()
	{
		if (_playerOnPlatformLastUpdatedFrame == Time.frameCount)
		{
			return _playerOnPlatform;
		}
		_playerOnPlatform = null;
		Vector3 vector = MilMo_Player.Instance.Avatar.Position + new Vector3(0f, 0.3f, 0f);
		Vector3 end = vector - new Vector3(0f, 1f, 0f);
		if (Physics.CheckCapsule(vector, end, 0.4f, 2097152))
		{
			if (Physics.Raycast(vector, Vector3.down, out var hitInfo, 1f, 2097152))
			{
				_playerOnPlatform = hitInfo.collider.gameObject;
				_playerOnPlatformDistance = hitInfo.distance;
			}
			int num = 0;
			while (_playerOnPlatform == null && num < 4)
			{
				if (Physics.Raycast(vector + Quaternion.Euler(0f, num * 90, 0f) * Vector3.forward * 0.4f, Vector3.down, out hitInfo, 1f, 2097152))
				{
					_playerOnPlatform = hitInfo.collider.gameObject;
					_playerOnPlatformDistance = hitInfo.distance;
				}
				num++;
			}
		}
		_playerOnPlatformLastUpdatedFrame = Time.frameCount;
		return _playerOnPlatform;
	}
}
