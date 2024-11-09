using System.Collections.Generic;
using System.Linq;
using Code.Core.Global;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Code.World.Player;
using UnityEngine;

namespace Code.World.AmbientSound;

public class MilMo_AmbientSoundManager
{
	private const int SAMPLE_RADIUS = 2;

	private Texture2D _loopingSoundMap;

	private Texture2D _randomSoundMap;

	private readonly Vector3 _terrainPosition;

	private readonly Vector2 _terrainSize;

	private float _terrainHeight;

	private bool _started;

	private bool _muted;

	private readonly string _worldContentName;

	private readonly string _levelContentName;

	private readonly Vector2[][] _directions;

	private readonly float[][] _weights;

	private readonly List<MilMo_AmbientSound> _loopingSounds = new List<MilMo_AmbientSound>();

	private readonly List<MilMo_AmbientSound> _randomSounds = new List<MilMo_AmbientSound>();

	public MilMo_AmbientSoundManager(string worldContentName, string levelContentName)
	{
		_worldContentName = worldContentName;
		_levelContentName = levelContentName;
		Terrain activeTerrain = Terrain.activeTerrain;
		_terrainPosition = activeTerrain.transform.position;
		TerrainData terrainData = activeTerrain.terrainData;
		_terrainSize = new Vector2(terrainData.size.x, terrainData.size.z);
		_directions = new Vector2[5][];
		for (int i = 0; i < _directions.Length; i++)
		{
			_directions[i] = new Vector2[5];
		}
		_weights = new float[5][];
		for (int j = 0; j < _weights.Length; j++)
		{
			_weights[j] = new float[5];
		}
		for (int k = 0; k < _directions.Length && k < _weights.Length; k++)
		{
			for (int l = 0; l < _directions[k].Length; l++)
			{
				if (k >= _weights[k].Length)
				{
					break;
				}
				int num = l - 2;
				int num2 = 2 - k;
				Vector3 a = new Vector3(num, 0f, num2);
				float num3 = Mathf.Max(0.5f, a.magnitude);
				float num4 = 1f / num3;
				if (!MilMo_Utility.Equals(a, Vector3.zero))
				{
					a.Normalize();
				}
				else
				{
					a = new Vector3(0f, 0f, 1f);
				}
				_directions[k][l] = new Vector2(a.x, a.z) * num4;
				_weights[k][l] = num4;
			}
		}
	}

	public void Start()
	{
		_started = true;
		string text = "Content/Worlds/" + _worldContentName + "/Levels/" + _levelContentName + "/";
		MilMo_SimpleFormat.AsyncLoad(text + "LoopingAmbience", "Level", MilMo_ResourceManager.Priority.Low, LoopingSoundConfigArrived);
		MilMo_SimpleFormat.AsyncLoad(text + "RandomAmbience", "Level", MilMo_ResourceManager.Priority.Low, RandomSoundConfigArrived);
		LoadLoopingSoundMapAsync(text);
		LoadRandomSoundMapAsync(text);
		foreach (MilMo_AmbientSound loopingSound in _loopingSounds)
		{
			loopingSound.Play();
		}
		foreach (MilMo_AmbientSound randomSound in _randomSounds)
		{
			randomSound.Play();
		}
	}

	private async void LoadRandomSoundMapAsync(string contentPath)
	{
		RandomSoundMapArrived(await MilMo_ResourceManager.Instance.LoadTextureAsync(contentPath + "RandomAmbience", "Level", MilMo_ResourceManager.Priority.Low));
	}

	private async void LoadLoopingSoundMapAsync(string contentPath)
	{
		LoopingSoundMapArrived(await MilMo_ResourceManager.Instance.LoadTextureAsync(contentPath + "LoopingAmbience", "Level", MilMo_ResourceManager.Priority.Low));
	}

	public void Stop()
	{
		_started = false;
		foreach (MilMo_AmbientSound loopingSound in _loopingSounds)
		{
			loopingSound.Destroy();
		}
		foreach (MilMo_AmbientSound randomSound in _randomSounds)
		{
			randomSound.Destroy();
		}
	}

	public void Mute()
	{
		_muted = true;
		foreach (MilMo_AmbientSound loopingSound in _loopingSounds)
		{
			loopingSound.Mute();
		}
		foreach (MilMo_AmbientSound randomSound in _randomSounds)
		{
			randomSound.Mute();
		}
	}

	public void Unmute()
	{
		_muted = false;
		foreach (MilMo_AmbientSound loopingSound in _loopingSounds)
		{
			loopingSound.Unmute();
		}
		foreach (MilMo_AmbientSound randomSound in _randomSounds)
		{
			randomSound.Unmute();
		}
	}

	public void Update()
	{
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.Avatar == null || MilMo_Player.Instance.Avatar.GameObject == null || ((_loopingSoundMap == null || _loopingSounds.Count == 0) && (_randomSoundMap == null || _randomSounds.Count == 0)))
		{
			return;
		}
		if (!_muted)
		{
			Vector3 position = MilMo_Global.Camera.transform.position;
			Vector2 vector = default(Vector2);
			if (_loopingSoundMap != null && _loopingSounds.Count > 0)
			{
				Vector2 vector2 = new Vector2(_terrainSize.x / (float)_loopingSoundMap.width, _terrainSize.y / (float)_loopingSoundMap.height);
				vector.x = (position.x - _terrainPosition.x) / vector2.x;
				vector.y = (position.z - _terrainPosition.z) / vector2.y;
				vector.x = Mathf.Round(vector.x);
				vector.y = Mathf.Round(vector.y);
				Color[][] sample = GetSample(_loopingSoundMap, vector);
				Vector2[] smoothedDirections = GetSmoothedDirections(sample, _loopingSounds);
				float[] volumes = GetVolumes(sample, vector2.x, position.y, _loopingSounds);
				Vector3[] array = new Vector3[_loopingSounds.Count];
				for (int i = 0; i < smoothedDirections.Length && i < array.Length; i++)
				{
					Vector2 vector3 = vector + smoothedDirections[i];
					Vector3 vector4 = new Vector3(vector.x * vector2.x + _terrainPosition.x, 0f, vector.y * vector2.y + _terrainPosition.z);
					Vector3 vector5 = new Vector3(vector3.x * vector2.x + _terrainPosition.x, 0f, vector3.y * vector2.y + _terrainPosition.z);
					array[i] = position + (vector5 - vector4).normalized;
				}
				SetAudioSourcesVolumeAndPosition(array, volumes, _loopingSounds);
			}
			if (_randomSoundMap != null && _randomSounds.Count > 0)
			{
				Vector2 vector6 = new Vector2(_terrainSize.x / (float)_randomSoundMap.width, _terrainSize.y / (float)_randomSoundMap.height);
				vector.x = (position.x - _terrainPosition.x) / vector6.x;
				vector.y = (position.z - _terrainPosition.z) / vector6.y;
				vector.x = Mathf.Round(vector.x);
				vector.y = Mathf.Round(vector.y);
				Color[][] sample2 = GetSample(_randomSoundMap, vector);
				Vector2[] smoothedDirections2 = GetSmoothedDirections(sample2, _randomSounds);
				float[] volumes2 = GetVolumes(sample2, vector6.x, position.y, _randomSounds);
				Vector3[] array2 = new Vector3[_randomSounds.Count];
				for (int j = 0; j < smoothedDirections2.Length && j < array2.Length; j++)
				{
					Vector2 vector7 = vector + smoothedDirections2[j];
					Vector3 vector8 = new Vector3(vector.x * vector6.x + _terrainPosition.x, 0f, vector.y * vector6.y + _terrainPosition.z);
					Vector3 vector9 = new Vector3(vector7.x * vector6.x + _terrainPosition.x, 0f, vector7.y * vector6.y + _terrainPosition.z);
					array2[j] = position + (vector9 - vector8).normalized;
				}
				SetAudioSourcesVolumeAndPosition(array2, volumes2, _randomSounds);
			}
		}
		foreach (MilMo_AmbientSound loopingSound in _loopingSounds)
		{
			loopingSound.Update();
		}
		foreach (MilMo_AmbientSound randomSound in _randomSounds)
		{
			randomSound.Update();
		}
	}

	private static void SetAudioSourcesVolumeAndPosition(Vector3[] worldPositions, float[] volumes, List<MilMo_AmbientSound> sounds)
	{
		for (int i = 0; i < worldPositions.Length && i < volumes.Length && i < sounds.Count; i++)
		{
			sounds[i].Volume = volumes[i];
			sounds[i].Position = worldPositions[i];
		}
	}

	private float[] GetVolumes(Color[][] pixels, float cellSize, float height, List<MilMo_AmbientSound> sounds)
	{
		float num = 1f / cellSize;
		float[] array = new float[sounds.Count];
		float[] array2 = new float[sounds.Count];
		float[] array3 = new float[sounds.Count];
		for (int i = 0; i < sounds.Count && i < array3.Length; i++)
		{
			if (height >= sounds[i].HeightRange.x && height <= sounds[i].HeightRange.y)
			{
				array3[i] = 1f;
				continue;
			}
			float num2 = Mathf.Min(Mathf.Abs(sounds[i].HeightRange.x - height), Mathf.Abs(sounds[i].HeightRange.y - height));
			if (num2 > 1f)
			{
				array3[i] = 1f / num2;
			}
			else
			{
				array3[i] = 1f;
			}
		}
		for (int j = 0; j < pixels.Length && j < _weights.Length; j++)
		{
			for (int k = 0; k < pixels[j].Length && k < _weights[j].Length; k++)
			{
				Color color = pixels[j][k];
				float num3 = ((_weights[j][k] > 1f) ? _weights[j][k] : (_weights[j][k] * num));
				for (int l = 0; l < sounds.Count && l < array.Length && l < array2.Length; l++)
				{
					float num4 = 0f;
					if (sounds[l].Channel == 0)
					{
						num4 = color.r;
					}
					else if (sounds[l].Channel == 1)
					{
						num4 = color.g;
					}
					else if (sounds[l].Channel == 2)
					{
						num4 = color.b;
					}
					else if (sounds[l].Channel == 3)
					{
						num4 = color.a;
					}
					array2[l] += num4;
					if (num3 > array[l])
					{
						array[l] = num3;
					}
				}
			}
		}
		int num5 = pixels.Sum((Color[] pixelRow) => pixelRow.Length);
		for (int m = 0; m < array2.Length && m < array.Length && m < array3.Length; m++)
		{
			array2[m] = Mathf.Clamp01(array2[m] / (float)num5 * array[m]) * array3[m];
		}
		return array2;
	}

	private Vector2[] GetSmoothedDirections(Color[][] pixels, List<MilMo_AmbientSound> sounds)
	{
		Vector2[] array = new Vector2[_loopingSounds.Count];
		for (int i = 0; i < pixels.Length && i < _directions.Length; i++)
		{
			for (int j = 0; j < pixels[i].Length && j < _directions[i].Length; j++)
			{
				if (i == 2 && j == 2)
				{
					continue;
				}
				Color color = pixels[i][j];
				for (int k = 0; k < sounds.Count && k < array.Length; k++)
				{
					float num = 0f;
					if (sounds[k].Channel == 0)
					{
						num = color.r;
					}
					else if (sounds[k].Channel == 1)
					{
						num = color.g;
					}
					else if (sounds[k].Channel == 2)
					{
						num = color.b;
					}
					else if (sounds[k].Channel == 3)
					{
						num = color.a;
					}
					array[k] += _directions[i][j] * num;
				}
			}
		}
		Vector2[] array2 = new Vector2[array.Length];
		int num2 = pixels.Sum((Color[] sampleRow) => sampleRow.Length);
		num2--;
		for (int l = 0; l < array.Length; l++)
		{
			Vector2 direction = array[l] / num2;
			array2[l] = _loopingSounds[l].SmoothDirection(direction);
		}
		return array2;
	}

	private static Color[][] GetSample(Texture2D map, Vector2 center)
	{
		Vector2 vector = center - new Vector2(2f, 2f);
		vector.x = Mathf.Min(Mathf.Max(vector.x, 0f), map.width - 1);
		vector.y = Mathf.Min(Mathf.Max(vector.y, 0f), map.height - 1);
		Vector2 vector2 = center + new Vector2(2f, 2f);
		vector2.x = Mathf.Min(Mathf.Max(vector2.x, 0f), map.width - 1);
		vector2.y = Mathf.Min(Mathf.Max(vector2.y, 0f), map.height - 1);
		Vector2 vector3 = vector2 - vector + new Vector2(1f, 1f);
		Color[] array = new Color[0];
		int num = 0;
		int num2 = 0;
		if (vector3.x > 0f && vector3.y > 0f)
		{
			array = map.GetPixels((int)vector.x, (int)vector.y, (int)vector3.x, (int)vector3.y);
			num = 2 - (int)(center.y - vector.y);
			num2 = 2 - (int)(center.x - vector.x);
		}
		Color[][] array2 = new Color[5][];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = new Color[5];
		}
		for (int j = 0; j < array2.Length; j++)
		{
			for (int k = 0; k < array2[j].Length; k++)
			{
				int num3 = array2.Length - 1 - j - num;
				int num4 = k - num2;
				int num5 = (int)((float)num4 + vector3.x * (float)num3);
				if (vector3.x > 0f && vector3.y > 0f && num3 >= 0 && (float)num3 < vector3.x && num4 >= 0 && (float)num4 < vector3.y && num5 < array.Length)
				{
					array2[j][k] = array[num5];
				}
				else
				{
					array2[j][k] = new Color(0f, 0f, 0f, 0f);
				}
			}
		}
		return array2;
	}

	private void LoopingSoundConfigArrived(MilMo_SFFile file)
	{
		if (file == null)
		{
			return;
		}
		int num = 0;
		while (file.NextRow())
		{
			if (!file.IsNext("<AMBIENTSOUND>"))
			{
				continue;
			}
			MilMo_AmbientSoundLooping milMo_AmbientSoundLooping = new MilMo_AmbientSoundLooping(num);
			if (milMo_AmbientSoundLooping.Read(file))
			{
				milMo_AmbientSoundLooping.SetSource(new GameObject("LoopingAmbience" + num));
				if (_started)
				{
					milMo_AmbientSoundLooping.Play();
				}
				_loopingSounds.Add(milMo_AmbientSoundLooping);
			}
			else
			{
				Debug.LogWarning("Failed to read looping ambience sound config for channel " + num);
			}
			num++;
		}
	}

	private void RandomSoundConfigArrived(MilMo_SFFile file)
	{
		if (file == null)
		{
			return;
		}
		int num = 0;
		while (file.NextRow())
		{
			if (!file.IsNext("<AMBIENTSOUND>"))
			{
				continue;
			}
			MilMo_AmbientSoundRandom milMo_AmbientSoundRandom = new MilMo_AmbientSoundRandom(num);
			if (milMo_AmbientSoundRandom.Read(file))
			{
				milMo_AmbientSoundRandom.SetSource(new GameObject("RandomAmbience" + num));
				if (_started)
				{
					milMo_AmbientSoundRandom.Play();
				}
				_randomSounds.Add(milMo_AmbientSoundRandom);
			}
			else
			{
				Debug.LogWarning("Failed to read random ambience sound config for channel " + num);
			}
			num++;
		}
	}

	private void RandomSoundMapArrived(Texture2D map)
	{
		_randomSoundMap = map;
	}

	private void LoopingSoundMapArrived(Texture2D map)
	{
		_loopingSoundMap = map;
	}
}
