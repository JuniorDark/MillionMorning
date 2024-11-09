using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Code.Core.ResourceSystem;

public static class MilMo_SimpleFormat
{
	public delegate void SimpleFormatDone(MilMo_SFFile file);

	public static async Task<MilMo_SFFile> RealAsyncLoad(string filename, string tag = "Generic", MilMo_ResourceManager.Priority priority = MilMo_ResourceManager.Priority.High)
	{
		TextAsset textAsset = await MilMo_ResourceManager.Instance.LoadTextAsync(filename, tag, priority);
		if ((object)textAsset == null)
		{
			Debug.LogWarning("Asset " + filename + " was not found");
			return null;
		}
		MilMo_SFFile milMo_SFFile = Parse(textAsset.text);
		milMo_SFFile.SetName(milMo_SFFile.Name);
		milMo_SFFile.SetPath(filename);
		return milMo_SFFile;
	}

	public static void AsyncLoad(string filename, SimpleFormatDone callback)
	{
		AsyncLoad(filename, "Generic", MilMo_ResourceManager.Priority.High, callback);
	}

	public static void AsyncLoad(string filename, string tag, SimpleFormatDone callback)
	{
		AsyncLoad(filename, tag, MilMo_ResourceManager.Priority.High, callback);
	}

	public static async void AsyncLoad(string filename, string tag, MilMo_ResourceManager.Priority priority, SimpleFormatDone callback)
	{
		TextAsset textAsset = await MilMo_ResourceManager.Instance.LoadTextAsync(filename, tag, priority);
		if (callback != null)
		{
			if (textAsset == null)
			{
				callback(null);
				return;
			}
			MilMo_SFFile milMo_SFFile = Parse(textAsset.text);
			milMo_SFFile.SetName(milMo_SFFile.Name);
			milMo_SFFile.SetPath(filename);
			callback(milMo_SFFile);
		}
	}

	public static MilMo_SFFile LoadLocal(string filename)
	{
		TextAsset textAsset = MilMo_ResourceManager.Instance.LoadTextLocal(filename);
		if (!textAsset)
		{
			return null;
		}
		MilMo_SFFile milMo_SFFile = Parse(textAsset.text);
		milMo_SFFile.SetName(textAsset.name);
		milMo_SFFile.SetPath(filename);
		return milMo_SFFile;
	}

	public static IEnumerable<MilMo_SFFile> LoadAllLocal(string path)
	{
		TextAsset[] array = MilMo_ResourceManager.Instance.LoadAllLocal(path);
		MilMo_SFFile[] array2 = new MilMo_SFFile[array.Length];
		int num = 0;
		TextAsset[] array3 = array;
		foreach (TextAsset textAsset in array3)
		{
			array2[num] = LoadFromString(textAsset.text);
			array2[num].SetName(textAsset.name);
			array2[num].SetPath(path + "/" + textAsset.name);
			num++;
		}
		return array2;
	}

	public static MilMo_SFFile LoadFromString(string text)
	{
		return Parse(text);
	}

	public static MilMo_SFFile LoadFromDisk(string file)
	{
		if (file.Length == 0)
		{
			return null;
		}
		StreamReader streamReader;
		try
		{
			streamReader = File.OpenText(file);
		}
		catch (FileNotFoundException)
		{
			Debug.LogWarning("Failed to load simple format file " + file + " from disk. File does not exist.");
			return null;
		}
		MilMo_SFFile result = Parse(streamReader.ReadToEnd());
		streamReader.Close();
		return result;
	}

	private static MilMo_SFFile Parse(string text)
	{
		MilMo_SFFile milMo_SFFile = new MilMo_SFFile();
		if (string.IsNullOrEmpty(text))
		{
			return milMo_SFFile;
		}
		int length = text.Length;
		int i = 0;
		bool flag = false;
		while (i < length)
		{
			List<string> list = new List<string>();
			string text2 = "";
			while (i < length && text[i] != '\n' && text[i] != '\r')
			{
				if (text2 == "//")
				{
					for (; i < length && text[i] != '\n'; i++)
					{
					}
					text2 = "";
					break;
				}
				if (text[i] == '"')
				{
					flag = true;
					i++;
					while (i < length && text[i] != '"')
					{
						if (i + 1 < length && text[i] == '\\' && text[i + 1] == '"')
						{
							i++;
						}
						if (i + 1 < length && text[i] == '\\' && text[i + 1] == 'n')
						{
							text2 += Environment.NewLine;
							i += 2;
						}
						else
						{
							text2 += text[i];
							i++;
						}
					}
					i++;
				}
				else if (text[i] == ' ' || text[i] == '\t')
				{
					list.Add(string.Copy(flag ? text2 : text2.Trim()));
					text2 = "";
					i++;
					flag = false;
				}
				else
				{
					text2 += text[i];
					i++;
				}
			}
			if (!flag)
			{
				text2 = text2.Trim();
			}
			if (text2 != "//" && (text2.Length > 0 || flag))
			{
				list.Add(string.Copy(text2));
			}
			if (list.Count > 0)
			{
				milMo_SFFile.AddRow(list);
			}
			i++;
			flag = false;
		}
		return milMo_SFFile;
	}

	private static string Vector3AsString(Vector3 vec)
	{
		return vec.x + " " + vec.y + " " + vec.z;
	}

	public static string TransformAsString(Transform trans)
	{
		return Vector3AsString(trans.position) + " " + Vector3AsString(trans.rotation.eulerAngles);
	}
}
