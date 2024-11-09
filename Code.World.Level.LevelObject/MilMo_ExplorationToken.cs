using System;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.types;
using Code.Core.ObjectEffectSystem;
using Code.Core.Visual;
using Core;
using Core.GameEvent;
using UnityEngine;

namespace Code.World.Level.LevelObject;

public class MilMo_ExplorationToken : MilMo_LevelToken
{
	private const string FADE_IN_EFFECT = "FadeInExplorationToken";

	private const string FADE_OUT_EFFECT = "FadeOutExplorationToken";

	private MilMo_ObjectEffect _fadeEffect;

	private bool _isFaded;

	private bool _ghostMaterialsFinished;

	private readonly sbyte _index;

	public MilMo_ExplorationToken(ExplorationToken token, sbyte index)
		: base(token)
	{
		_index = index;
		TokenFoundEffectName = "ExplorationTokenFind";
		TokenFoundRemotePlayerEffectName = "ExplorationTokenRemotePlayerFind";
		UnFoundMeshVisualRepPath = "Content/Items/Batch01/SpecialItems/ExplorationToken";
		FoundMeshVisualRepPath = "Content/Items/Batch01/SpecialItems/ExplorationTokenGhost";
		SqrFindRadius = 2.25f;
		Load(delegate
		{
			if (base.IsFound)
			{
				VisualRep.RegisterMaterialsDoneCallback(GhostMaterialDone);
			}
		});
	}

	public override void Update(Vector3 playerPosition)
	{
		bool flag = TokenFoundEffect != null;
		base.Update(playerPosition);
		if (VisualRep == null || flag)
		{
			return;
		}
		if (base.IsFound && _ghostMaterialsFinished)
		{
			TestFade(playerPosition);
		}
		if (_fadeEffect == null || _fadeEffect.Update())
		{
			return;
		}
		_fadeEffect = null;
		if (_isFaded || VisualRep.Materials.Count <= 0)
		{
			return;
		}
		try
		{
			VisualRep.CurrentLod.ParentVisualRep.Renderer.material.color = VisualRep.Materials[0].MainColor;
		}
		catch (NullReferenceException)
		{
			Debug.LogWarning("Got null reference exception when resetting alpha on exploration token \"ghost\"");
		}
	}

	protected override void SendFindRequest(Vector3 position)
	{
		Singleton<GameNetwork>.Instance.RequestFindExplorationToken(_index, position);
	}

	private void TestFade(Vector3 position)
	{
		if (VisualRep != null && !(VisualRep.GameObject == null) && (VisualRep.GameObject.transform.position - position).sqrMagnitude <= SqrFindRadius != _isFaded)
		{
			if (_isFaded)
			{
				_fadeEffect = MilMo_ObjectEffectSystem.GetObjectEffect(VisualRep.GameObject, "FadeInExplorationToken");
				_isFaded = false;
			}
			else
			{
				_fadeEffect = MilMo_ObjectEffectSystem.GetObjectEffect(VisualRep.GameObject, "FadeOutExplorationToken");
				_isFaded = true;
			}
		}
	}

	public override void SetAsFound()
	{
		base.SetAsFound();
		MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_ReceiveExplorationToken");
		GameEvent.ShowExplorationCounterEvent?.RaiseEvent(args: true);
	}

	private void GhostMaterialDone(MilMo_VisualRep visualRep)
	{
		visualRep.GameObject.SetActive(value: true);
		VisualRep = visualRep;
		_ghostMaterialsFinished = true;
		try
		{
			Material material = visualRep.CurrentLod.ParentVisualRep.Renderer.material;
			Color color = material.color;
			color.a = 0f;
			material.color = color;
			_isFaded = true;
		}
		catch (NullReferenceException)
		{
			Debug.LogWarning("Got null reference exception when setting alpha to 0 on exploration token \"ghost\"");
		}
	}
}
