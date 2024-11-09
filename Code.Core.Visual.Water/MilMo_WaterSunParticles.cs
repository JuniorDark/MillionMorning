using Code.Core.EventSystem;
using UnityEngine;

namespace Code.Core.Visual.Water;

public class MilMo_WaterSunParticles : MonoBehaviour
{
	public int numParticles = 128;

	public int numAmbientParticles = 128;

	public float lifeTime = 0.4f;

	public float particleSize = 0.2f;

	public float sunTrailLength = 128f;

	public float sunTrailWidthOffset = 3.2f;

	public float ambientLength = 48f;

	public float emitDelay;

	public float emitAmbientDelay;

	public bool enableSunSparkles;

	public bool enableAmbientSparkles;

	private GameObject _water;

	private GameObject _sunLight;

	public GameObject sunSpark;

	private ParticleSystem _emitter;

	private float _lastEmit;

	private int _numEmitted;

	private float _lastAmbientEmit;

	private int _numAmbientEmitted;

	public void Start()
	{
		sunSpark = MilMo_ParticleContainer.GetParticle("WaterSparkle", default(Vector3));
		sunSpark.name = "WaterSparkle";
		_emitter = sunSpark.GetComponentInChildren<ParticleSystem>();
		if (_emitter != null)
		{
			_emitter.gameObject.layer = 10;
		}
	}

	public void SetWater(GameObject water)
	{
		_water = water;
	}

	public void SetSunLight(GameObject sunLight)
	{
		_sunLight = sunLight;
	}

	public void Destroy()
	{
		if ((bool)sunSpark)
		{
			Object.Destroy(sunSpark);
			sunSpark = null;
		}
	}

	public void FixedUpdate()
	{
		if ((bool)_water && (bool)_sunLight && (bool)_emitter)
		{
			if (enableSunSparkles)
			{
				UpdateSunSparks();
			}
			if (enableAmbientSparkles)
			{
				UpdateAmbientSparks();
			}
		}
	}

	private void UpdateSunSparks()
	{
		UnityEngine.Camera current = UnityEngine.Camera.current;
		if (!current || Time.realtimeSinceStartup <= _lastEmit + emitDelay)
		{
			return;
		}
		float y = _water.transform.position.y;
		Vector3 position = current.transform.position;
		position.y = y;
		int count = 0;
		int num = Random.Range(4, 12);
		while (_numEmitted < numParticles && count < num)
		{
			Vector3 b = position - _sunLight.transform.forward * (sunTrailLength * Random.Range(0.8f, 1f));
			position.y = y;
			b.y = y;
			Vector3 vector = Vector3.Lerp(position, b, Random.value);
			vector.x += RandomSmallOffset(sunTrailWidthOffset);
			vector.z += RandomSmallOffset(sunTrailWidthOffset);
			vector.y = y;
			if (!InsideWater(vector))
			{
				return;
			}
			sunSpark.transform.position = vector;
			ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
			emitParams.position = vector;
			emitParams.velocity = default(Vector3);
			emitParams.startSize = particleSize + RandomSmallOffset(0.1f);
			emitParams.startLifetime = lifeTime + RandomSmallOffset(0.1f);
			emitParams.startColor = new Color(1f, 1f, 1f);
			ParticleSystem.EmitParams emitParams2 = emitParams;
			_emitter.Emit(emitParams2, 1);
			_numEmitted++;
			count++;
		}
		if (count > 0)
		{
			MilMo_EventSystem.At(lifeTime, delegate
			{
				_numEmitted -= count;
			});
		}
		_lastEmit = Time.realtimeSinceStartup;
	}

	private void UpdateAmbientSparks()
	{
		UnityEngine.Camera current = UnityEngine.Camera.current;
		if (!current || !(Time.realtimeSinceStartup > _lastAmbientEmit + emitAmbientDelay) || _numAmbientEmitted >= numAmbientParticles)
		{
			return;
		}
		Vector3 position = current.transform.position;
		Vector2 vector = Random.insideUnitCircle * ambientLength;
		position.x += vector.x;
		position.z += vector.y;
		position.y = _water.transform.position.y;
		if (InsideWater(position))
		{
			ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
			emitParams.position = position;
			emitParams.velocity = default(Vector3);
			emitParams.startSize = particleSize + RandomSmallOffset(0.1f);
			emitParams.startLifetime = lifeTime + RandomSmallOffset(0.1f);
			emitParams.startColor = new Color(1f, 1f, 1f);
			ParticleSystem.EmitParams emitParams2 = emitParams;
			_emitter.Emit(emitParams2, 1);
			_numAmbientEmitted++;
			MilMo_EventSystem.At(lifeTime, delegate
			{
				_numAmbientEmitted--;
			});
			_lastAmbientEmit = Time.realtimeSinceStartup;
		}
	}

	private bool InsideWater(Vector3 pos)
	{
		Vector3 localScale = _water.transform.localScale;
		return (pos - _water.transform.position).sqrMagnitude < localScale.x * localScale.x;
	}

	private static float RandomSmallOffset(float scale)
	{
		return (Random.value - 0.5f) * scale;
	}
}
