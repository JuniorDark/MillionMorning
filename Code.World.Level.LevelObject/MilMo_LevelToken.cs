using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.Visual;
using Code.Core.Visual.Effect;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public abstract class MilMo_LevelToken
{
	protected delegate void PostLoad(MilMo_VisualRep visualRep);

	private static readonly Vector3 HeadOffset = new Vector3(0f, 1.1f, 0f);

	private const float REQUEST_COOLDOWN_TIME = 0.1f;

	protected string UnFoundMeshVisualRepPath;

	protected string FoundMeshVisualRepPath;

	protected string TokenFoundEffectName;

	protected string TokenFoundRemotePlayerEffectName;

	private readonly List<MilMo_Effect> _remoteFoundEffects = new List<MilMo_Effect>();

	protected MilMo_Effect TokenFoundEffect;

	protected MilMo_VisualRep VisualRep;

	private float _lastFindRequest;

	protected float SqrFindRadius = 1f;

	private Vector3 _position;

	private bool _unloaded;

	public bool IsFound { get; private set; }

	public void SetPosition(Vector3 value)
	{
		_position = value;
		if (VisualRep != null && VisualRep.GameObject != null)
		{
			VisualRep.GameObject.transform.position = _position;
		}
	}

	protected MilMo_LevelToken(Token token)
	{
		IsFound = token.GetIsFound() == 1;
		SetPosition(new Vector3(token.GetPosition().GetX(), token.GetPosition().GetY(), token.GetPosition().GetZ()));
	}

	protected void Load(PostLoad callback = null)
	{
		MilMo_VisualRepContainer.AsyncCreateVisualRep(IsFound ? FoundMeshVisualRepPath : UnFoundMeshVisualRepPath, _position, Quaternion.identity, delegate(MilMo_VisualRep visualRep)
		{
			if (visualRep == null || visualRep.GameObject == null)
			{
				VisualRep = null;
			}
			else if (_unloaded)
			{
				MilMo_VisualRepContainer.RemoveFromUpdate(visualRep);
				MilMo_VisualRepContainer.DestroyVisualRep(visualRep);
			}
			else
			{
				VisualRep = visualRep;
				VisualRep.GameObject.transform.position = _position;
				if (callback != null)
				{
					callback(visualRep);
				}
			}
		});
	}

	public virtual void Update(Vector3 playerPosition)
	{
		if (VisualRep == null)
		{
			return;
		}
		for (int num = _remoteFoundEffects.Count - 1; num >= 0; num--)
		{
			if (!_remoteFoundEffects[num].Update())
			{
				_remoteFoundEffects[num].DestroyWhenDone();
				_remoteFoundEffects.RemoveAt(num);
			}
		}
		if (TokenFoundEffect != null)
		{
			UpdateTokenFoundEffect();
			return;
		}
		if (!IsFound)
		{
			TestFind(playerPosition);
		}
		VisualRep.Update();
	}

	private void UpdateTokenFoundEffect()
	{
		if (!TokenFoundEffect.Update())
		{
			TokenFoundEffect.DestroyWhenDone();
			TokenFoundEffect = null;
		}
	}

	private void TestFind(Vector3 position)
	{
		if (!(Time.time - _lastFindRequest < 0.1f) && VisualRep != null && !(VisualRep.GameObject == null) && ((VisualRep.GameObject.transform.position - position).sqrMagnitude <= SqrFindRadius || (VisualRep.GameObject.transform.position - (position + HeadOffset)).sqrMagnitude <= SqrFindRadius))
		{
			SendFindRequest(position);
			_lastFindRequest = Time.time;
		}
	}

	public void FoundByRemotePlayer()
	{
		if (VisualRep != null && !(VisualRep.GameObject == null))
		{
			MilMo_Effect milMo_Effect = null;
			if (!string.IsNullOrEmpty(TokenFoundRemotePlayerEffectName))
			{
				milMo_Effect = MilMo_EffectContainer.GetEffect(TokenFoundRemotePlayerEffectName, VisualRep.GameObject);
			}
			if (milMo_Effect != null)
			{
				_remoteFoundEffects.Add(milMo_Effect);
			}
		}
	}

	public void Unload()
	{
		if (VisualRep != null)
		{
			VisualRep.Destroy();
			VisualRep = null;
		}
		_unloaded = true;
	}

	public virtual void SetAsFound()
	{
		if (IsFound)
		{
			return;
		}
		IsFound = true;
		if (VisualRep != null && !(VisualRep.GameObject == null))
		{
			Renderer[] componentsInChildren = VisualRep.GameObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			TokenFoundEffect = MilMo_EffectContainer.GetEffect(TokenFoundEffectName, VisualRep.GameObject);
		}
	}

	protected abstract void SendFindRequest(Vector3 position);
}
