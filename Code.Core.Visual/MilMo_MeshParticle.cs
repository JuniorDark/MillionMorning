using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.Visual;

public class MilMo_MeshParticle
{
	private MilMo_VisualRep _visualRep;

	private readonly Vector3 _rotationalVelocity;

	private readonly Vector3 _velocity;

	private readonly float _fade;

	private float _currentAlpha;

	private bool _doneLoading;

	private Vector3 _realPosition;

	private Quaternion _realRotation;

	private Material _material;

	private static readonly int TintColor = Shader.PropertyToID("_TintColor");

	public MilMo_ObjectMover Mover { get; private set; }

	public MilMo_MeshParticle(string visualRepPath, Vector3 position, Vector3 rotation, Vector3 scale, float transparency, Vector3 velocity, Vector3 rotationalVelocity, float fade)
	{
		MilMo_MeshParticle milMo_MeshParticle = this;
		_velocity = velocity;
		_rotationalVelocity = rotationalVelocity;
		_fade = fade;
		_currentAlpha = transparency;
		Mover = new MilMo_ObjectMover();
		MilMo_VisualRepContainer.AsyncCreateVisualRep(visualRepPath, position, Quaternion.Euler(rotation), delegate(MilMo_VisualRep visualRep)
		{
			milMo_MeshParticle._visualRep = visualRep;
			if (milMo_MeshParticle._visualRep != null && milMo_MeshParticle._visualRep.GameObject != null)
			{
				milMo_MeshParticle._visualRep.GameObject.transform.localScale = scale;
				if (milMo_MeshParticle._visualRep.Materials != null && milMo_MeshParticle._visualRep.Materials.Count > 0 && milMo_MeshParticle._visualRep.Materials[0] != null && milMo_MeshParticle._visualRep.Materials[0].Material != null)
				{
					milMo_MeshParticle._material = milMo_MeshParticle._visualRep.Materials[0].Material;
					Color color = milMo_MeshParticle._material.GetColor(TintColor);
					color.a = milMo_MeshParticle._currentAlpha;
					milMo_MeshParticle._material.SetColor(TintColor, color);
				}
				milMo_MeshParticle._realPosition = milMo_MeshParticle._visualRep.GameObject.transform.position;
				milMo_MeshParticle._realRotation = milMo_MeshParticle._visualRep.GameObject.transform.rotation;
				milMo_MeshParticle.Mover.Scale = milMo_MeshParticle._visualRep.GameObject.transform.localScale;
			}
			milMo_MeshParticle._doneLoading = true;
		});
	}

	public bool Update()
	{
		if (!_doneLoading)
		{
			return true;
		}
		if (_visualRep == null || !_visualRep.GameObject || !_material || Mover == null)
		{
			return false;
		}
		_visualRep.Update();
		_realRotation.eulerAngles += _rotationalVelocity * Time.deltaTime;
		_realPosition += _velocity * Time.deltaTime;
		_visualRep.GameObject.transform.position = _realPosition + Mover.Pos;
		_visualRep.GameObject.transform.rotation = Quaternion.Euler(_realRotation.eulerAngles + Mover.Angle);
		_visualRep.GameObject.transform.localScale = Mover.Scale;
		_currentAlpha -= _fade * Time.deltaTime;
		Color color = _material.GetColor(TintColor);
		color.a = _currentAlpha;
		_material.SetColor(TintColor, color);
		bool num = _currentAlpha <= 0f;
		if (num)
		{
			Destroy();
		}
		return !num;
	}

	public void FixedUpdate()
	{
		if (_doneLoading && Mover != null)
		{
			Mover.Update();
		}
	}

	public void Destroy()
	{
		if (_visualRep != null && (bool)_visualRep.GameObject)
		{
			Object.Destroy(_visualRep.GameObject);
		}
	}
}
