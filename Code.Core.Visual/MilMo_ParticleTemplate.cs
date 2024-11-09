using System.Collections.Generic;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Visual;

public class MilMo_ParticleTemplate : MonoBehaviour
{
	public string effectName;

	public string type;

	public Mesh mesh;

	public Vector3 scale;

	public bool emit;

	public float minSize;

	public float maxSize;

	public float minEnergy;

	public float maxEnergy;

	public float minEmission;

	public float maxEmission;

	public Vector3 worldVelocity = Vector3.zero;

	public Vector3 localVelocity = Vector3.zero;

	public Vector3 rndVelocity = Vector3.zero;

	public float emitterVelocityScale;

	public bool useWorldSpace;

	public bool oneShot;

	public bool doesAnimateColor;

	public Vector3 worldRotationAxis = Vector3.zero;

	public Vector3 localRotationAxis = Vector3.zero;

	public float sizeGrow;

	public Vector3 rndForce = Vector3.zero;

	public Vector3 force = Vector3.zero;

	public float damping = 1f;

	public bool autoDestruct;

	public Color[] color;

	public float cameraVelocityScale;

	public string particleRenderMode = "";

	public float lengthScale;

	public float velocityScale;

	public float maxParticleSize;

	public int uvAnimationXTile = 1;

	public int uvAnimationYTile = 1;

	public float uvAnimationCycles;

	public Material[] sharedMaterials;

	public bool trail;

	public Material trailMaterial;

	public bool trailEmit;

	public float trailEmitTime;

	public float trailLifeTime;

	public float trailUVLengthScale;

	public bool trailHigherQualityUVs;

	public int trailMovePixelsForRebuild;

	public float trailMaxRebuildTime;

	public float trailMinVertexDistance;

	public float trailMaxVertexDistance;

	public float trailMaxAngle;

	public bool trailAutoDestruct;

	public Color[] trailColors;

	public float[] trailSizes;

	public Vector3 velocity;

	public ParticleSystemSimulationSpace velocitySpace;

	public ParticleSystemRenderSpace renderAlignment;

	public Vector3 rotation;

	public bool manualEmit;

	public int lights;

	public float sizeStart = 1f;

	public float sizeEnd = 1f;

	public bool preWarm;

	public float radius = 1f;

	public float radiusThickness = 1f;

	public Vector3 circleRotation = Vector3.zero;

	public void Load(MilMo_SFFile file)
	{
		if (file == null)
		{
			return;
		}
		while (file.NextRow())
		{
			if (!file.IsNext("<PARTICLE>"))
			{
				continue;
			}
			List<Color> list = new List<Color>();
			color = new Color[5]
			{
				Color.red,
				Color.yellow,
				Color.green,
				Color.blue,
				Color.magenta
			};
			color[0].a = 0f;
			color[1].a = 0f;
			color[2].a = 0f;
			color[3].a = 0.75f;
			color[4].a = 0f;
			List<Material> list2 = new List<Material>();
			while (file.NextRow() && !file.IsNext("</PARTICLE>"))
			{
				if (file.IsNext("Name"))
				{
					effectName = file.GetString();
				}
				else if (file.IsNext("<EMITTER>"))
				{
					file.NextRow();
					if (!file.IsNext("Type"))
					{
						Debug.LogWarning("Missing emitter type in particle " + effectName + ". Please re-export the script.");
						continue;
					}
					type = file.GetString();
					while (file.NextRow() && !file.IsNext("</EMITTER>"))
					{
						if (file.IsNext("Mesh"))
						{
							string text = "Particles/Meshes/" + file.GetString();
							Mesh mesh = MilMo_ResourceManager.Instance.LoadMeshLocal(text);
							if (!mesh)
							{
								Debug.LogWarning("Failed to load mesh " + text);
							}
							else
							{
								this.mesh = mesh;
							}
						}
						else if (file.IsNext("Scale"))
						{
							scale = file.GetVector3();
						}
						else if (file.IsNext("Emit"))
						{
							emit = file.GetBool();
						}
						else if (file.IsNext("MinSize"))
						{
							minSize = file.GetFloat();
						}
						else if (file.IsNext("MaxSize"))
						{
							maxSize = file.GetFloat();
						}
						else if (file.IsNext("MinEnergy"))
						{
							minEnergy = file.GetFloat();
						}
						else if (file.IsNext("MaxEnergy"))
						{
							maxEnergy = file.GetFloat();
						}
						else if (file.IsNext("MinEmission"))
						{
							minEmission = file.GetFloat();
						}
						else if (file.IsNext("MaxEmission"))
						{
							maxEmission = file.GetFloat();
						}
						else if (file.IsNext("WorldVelocity"))
						{
							worldVelocity = file.GetVector3();
						}
						else if (file.IsNext("LocalVelocity"))
						{
							localVelocity = file.GetVector3();
						}
						else if (file.IsNext("RndVelocity"))
						{
							rndVelocity = file.GetVector3();
						}
						else if (file.IsNext("EmitterVelocityScale"))
						{
							emitterVelocityScale = file.GetFloat();
						}
						else if (file.IsNext("SimulateInWorldSpace"))
						{
							useWorldSpace = file.GetBool();
						}
						else if (file.IsNext("OneShot"))
						{
							oneShot = true;
						}
					}
				}
				else if (file.IsNext("<ANIMATOR>"))
				{
					while (file.NextRow() && !file.IsNext("</ANIMATOR>"))
					{
						if (file.IsNext("AnimateColors"))
						{
							doesAnimateColor = file.GetBool();
						}
						else if (file.IsNext("ColorAnimation"))
						{
							list.Add(file.GetColor());
						}
						else if (file.IsNext("WorldRotationAxis"))
						{
							worldRotationAxis = file.GetVector3();
						}
						else if (file.IsNext("LocalRotationAxis"))
						{
							localRotationAxis = file.GetVector3();
						}
						else if (file.IsNext("SizeGrow"))
						{
							sizeGrow = file.GetFloat();
						}
						else if (file.IsNext("RndForce"))
						{
							rndForce = file.GetVector3();
						}
						else if (file.IsNext("Force"))
						{
							force = file.GetVector3();
						}
						else if (file.IsNext("Damping"))
						{
							damping = file.GetFloat();
						}
						else if (file.IsNext("AutoDestruct"))
						{
							autoDestruct = file.GetBool();
						}
					}
				}
				else if (file.IsNext("<RENDERER>"))
				{
					while (file.NextRow() && !file.IsNext("</RENDERER>"))
					{
						if (file.IsNext("<MAT>"))
						{
							file.NextRow();
							MilMo_Material material = MilMo_Material.GetMaterial(file.GetString());
							if (material != null)
							{
								material.Load(file);
								if (Application.isPlaying)
								{
									material.Create("Content/Particles/", file.Name, "Particles", async: true);
								}
								else
								{
									material.Create("Content/Particles/", file.Name);
								}
								list2.Add(material.Material);
							}
							else
							{
								Debug.LogWarning("Got null material in " + file.Path + " at " + file.GetLineNumber() + ".");
							}
						}
						else if (file.IsNext("CameraVelocityScale"))
						{
							cameraVelocityScale = file.GetFloat();
						}
						else if (file.IsNext("ParticleRenderMode"))
						{
							particleRenderMode = file.GetString();
						}
						else if (file.IsNext("LengthScale"))
						{
							lengthScale = file.GetFloat();
						}
						else if (file.IsNext("VelocityScale"))
						{
							velocityScale = file.GetFloat();
						}
						else if (file.IsNext("MaxParticleSize"))
						{
							maxParticleSize = file.GetFloat();
						}
						else if (file.IsNext("UVAnimationTileX"))
						{
							uvAnimationXTile = file.GetInt();
						}
						else if (file.IsNext("UVAnimationTileY"))
						{
							uvAnimationYTile = file.GetInt();
						}
						else if (file.IsNext("UVAnimationCycles"))
						{
							uvAnimationCycles = file.GetFloat();
						}
					}
				}
				else if (file.IsNext("<TRAIL>"))
				{
					trail = true;
					List<Color> list3 = new List<Color>();
					List<float> list4 = new List<float>();
					while (file.NextRow() && !file.IsNext("</TRAIL>"))
					{
						if (file.IsNext("<MAT>"))
						{
							file.NextRow();
							MilMo_Material material2 = MilMo_Material.GetMaterial(file.GetString());
							if (material2 != null)
							{
								material2.Load(file);
								if (Application.isPlaying)
								{
									material2.Create("Content/Particles/", file.Name, "Particles", async: true);
								}
								else
								{
									material2.Create("Content/Particles/", file.Name);
								}
								trailMaterial = material2.Material;
							}
							else
							{
								Debug.LogWarning("Got null material in " + file.Path + " at " + file.GetLineNumber() + ".");
							}
						}
						else if (file.IsNext("Emit"))
						{
							trailEmit = file.GetBool();
						}
						else if (file.IsNext("EmitTime"))
						{
							trailEmitTime = file.GetFloat();
						}
						else if (file.IsNext("LifeTime"))
						{
							trailLifeTime = file.GetFloat();
						}
						else if (file.IsNext("Color"))
						{
							list3.Add(file.GetColor());
						}
						else if (file.IsNext("Size"))
						{
							list4.Add(file.GetFloat());
						}
						else if (file.IsNext("UVLengthScale"))
						{
							trailUVLengthScale = file.GetFloat();
						}
						else if (file.IsNext("HigherQualityUVs"))
						{
							trailHigherQualityUVs = file.GetBool();
						}
						else if (file.IsNext("MovePixelsForRebuild"))
						{
							trailMovePixelsForRebuild = file.GetInt();
						}
						else if (file.IsNext("MaxRebuildTime"))
						{
							trailMaxRebuildTime = file.GetFloat();
						}
						else if (file.IsNext("MinVertexDistance"))
						{
							trailMinVertexDistance = file.GetFloat();
						}
						else if (file.IsNext("MaxVertexDistance"))
						{
							trailMaxVertexDistance = file.GetFloat();
						}
						else if (file.IsNext("MaxAngle"))
						{
							trailMaxAngle = file.GetFloat();
						}
						else if (file.IsNext("AutoDestruct"))
						{
							trailAutoDestruct = file.GetBool();
						}
					}
					trailColors = list3.ToArray();
					trailSizes = list4.ToArray();
				}
				else if (file.IsNext("Type"))
				{
					type = file.GetString();
				}
				else if (file.IsNext("Mesh"))
				{
					string text2 = "Particles/Meshes/" + file.GetString();
					Mesh mesh2 = MilMo_ResourceManager.Instance.LoadMeshLocal(text2);
					if (!mesh2)
					{
						Debug.LogWarning("Failed to load mesh " + text2);
						continue;
					}
					this.mesh = mesh2;
					type = "MeshParticleEmitter";
				}
				else if (file.IsNext("Radius"))
				{
					radius = file.GetFloat();
				}
				else if (file.IsNext("RadiusThickness"))
				{
					radiusThickness = file.GetFloat();
				}
				else if (file.IsNext("CircleRotation"))
				{
					circleRotation = file.GetVector3();
				}
				else if (file.IsNext("Scale"))
				{
					scale = file.GetVector3();
				}
				else if (file.IsNext("Rotation"))
				{
					rotation = file.GetVector3();
				}
				else if (file.IsNext("PlayOnAwake"))
				{
					emit = file.GetBool();
				}
				else if (file.IsNext("PreWarm"))
				{
					preWarm = file.GetBool();
				}
				else if (file.IsNext("ManualEmit"))
				{
					manualEmit = file.GetBool();
				}
				else if (file.IsNext("Lights"))
				{
					lights = file.GetInt();
				}
				else if (file.IsNext("MinSize"))
				{
					minSize = file.GetFloat();
				}
				else if (file.IsNext("MaxSize"))
				{
					maxSize = file.GetFloat();
				}
				else if (file.IsNext("MinEnergy"))
				{
					minEnergy = file.GetFloat();
				}
				else if (file.IsNext("MaxEnergy"))
				{
					maxEnergy = file.GetFloat();
				}
				else if (file.IsNext("MinEmission"))
				{
					minEmission = file.GetFloat();
				}
				else if (file.IsNext("MaxEmission"))
				{
					maxEmission = file.GetFloat();
				}
				else if (file.IsNext("WorldVelocity"))
				{
					worldVelocity = file.GetVector3();
				}
				else if (file.IsNext("LocalVelocity"))
				{
					localVelocity = file.GetVector3();
				}
				else if (file.IsNext("RndVelocity"))
				{
					rndVelocity = file.GetVector3();
				}
				else if (file.IsNext("EmitterVelocityScale"))
				{
					emitterVelocityScale = file.GetFloat();
				}
				else if (file.IsNext("SimulateInWorldSpace"))
				{
					useWorldSpace = file.GetBool();
				}
				else if (file.IsNext("OneShot"))
				{
					oneShot = true;
				}
				else if (file.IsNext("AnimateColors"))
				{
					doesAnimateColor = file.GetBool();
				}
				else if (file.IsNext("ColorAnimation"))
				{
					list.Add(file.GetColor());
				}
				else if (file.IsNext("WorldRotationAxis"))
				{
					worldRotationAxis = file.GetVector3();
				}
				else if (file.IsNext("LocalRotationAxis"))
				{
					localRotationAxis = file.GetVector3();
				}
				else if (file.IsNext("SizeGrow"))
				{
					sizeGrow = file.GetFloat();
				}
				else if (file.IsNext("SizeStart"))
				{
					sizeStart = file.GetFloat();
				}
				else if (file.IsNext("SizeEnd"))
				{
					sizeEnd = file.GetFloat();
				}
				else if (file.IsNext("RndForce"))
				{
					rndForce = file.GetVector3();
				}
				else if (file.IsNext("Force"))
				{
					force = file.GetVector3();
				}
				else if (file.IsNext("Damping"))
				{
					damping = file.GetFloat();
				}
				else if (file.IsNext("AutoDestruct"))
				{
					autoDestruct = file.GetBool();
				}
				else if (file.IsNext("<MAT>"))
				{
					file.NextRow();
					MilMo_Material material3 = MilMo_Material.GetMaterial(file.GetString());
					if (material3 != null)
					{
						material3.Load(file);
						if (Application.isPlaying)
						{
							material3.Create("Content/Particles/", file.Name, "Particles", async: true);
						}
						else
						{
							material3.Create("Content/Particles/", file.Name);
						}
						list2.Add(material3.Material);
					}
					else
					{
						Debug.LogWarning("Got null material in " + file.Path + " at " + file.GetLineNumber() + ".");
					}
				}
				else if (file.IsNext("CameraVelocityScale"))
				{
					cameraVelocityScale = file.GetFloat();
				}
				else if (file.IsNext("ParticleRenderMode"))
				{
					particleRenderMode = file.GetString();
				}
				else if (file.IsNext("RenderAlignment"))
				{
					if (file.GetString().ToUpper() == "LOCAL")
					{
						renderAlignment = ParticleSystemRenderSpace.Local;
					}
				}
				else if (file.IsNext("LengthScale"))
				{
					lengthScale = file.GetFloat();
				}
				else if (file.IsNext("VelocityScale"))
				{
					velocityScale = file.GetFloat();
				}
				else if (file.IsNext("MaxParticleSize"))
				{
					maxParticleSize = file.GetFloat();
				}
				else if (file.IsNext("UVAnimationTileX"))
				{
					uvAnimationXTile = file.GetInt();
				}
				else if (file.IsNext("UVAnimationTileY"))
				{
					uvAnimationYTile = file.GetInt();
				}
				else if (file.IsNext("UVAnimationCycles"))
				{
					uvAnimationCycles = file.GetFloat();
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				color[i] = list[i];
			}
			sharedMaterials = list2.ToArray();
		}
	}
}
