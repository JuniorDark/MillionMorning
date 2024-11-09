using Code.Core.Visual;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_AttachMeshEffect : MilMo_ObjectEffect
{
	private float _time;

	private MilMo_VisualRep _mesh;

	private bool _isLoading = true;

	private bool _isDestroyed;

	private MilMo_AttachMeshEffectTemplate Template => EffectTemplate as MilMo_AttachMeshEffectTemplate;

	public override float Duration => Template.Duration;

	public MilMo_AttachMeshEffect(GameObject gameObject, MilMo_AttachMeshEffectTemplate template)
		: base(gameObject, template)
	{
		MilMo_VisualRepContainer.AsyncCreateVisualRep(template.VisualRepPath, ((gameObject != null) ? gameObject.transform.position : Vector3.zero) + template.Offset, Quaternion.identity, delegate(MilMo_VisualRep newRep)
		{
			if (newRep != null)
			{
				if (_isDestroyed)
				{
					newRep.Destroy();
				}
				else if (newRep.GameObject != null)
				{
					_mesh = newRep;
				}
			}
			_isLoading = false;
		});
	}

	public override bool Update()
	{
		if (_mesh != null)
		{
			if (_mesh.GameObject != null)
			{
				Vector3 position = ((GameObject != null) ? GameObject.transform.position : Vector3.zero);
				position.y += Template.Offset.y;
				_mesh.GameObject.transform.position = position;
			}
			_mesh.Update();
			if (_mesh.GameObject != null)
			{
				_mesh.GameObject.transform.position = _mesh.GameObject.transform.TransformPoint(new Vector3(Template.Offset.x, 0f, Template.Offset.z));
			}
		}
		_time += Time.deltaTime;
		if (_time > Template.Duration && Template.Duration > 0f)
		{
			if (_mesh != null)
			{
				_mesh.Destroy();
				_mesh = null;
			}
			if (_isLoading)
			{
				return true;
			}
			Destroy();
			return false;
		}
		return true;
	}

	public override void Destroy()
	{
		base.Destroy();
		if (_mesh != null)
		{
			_mesh.Destroy();
			_mesh = null;
		}
		_isDestroyed = true;
	}
}
