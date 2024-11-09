using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Code.Core.Global;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.ResourceSystem;

public class MilMo_SFFile
{
	private readonly List<List<string>> _rows;

	private int _currentRow;

	private int _currentToken;

	private string _name = "";

	private string _path = "";

	public string Path => _path;

	public string Name
	{
		get
		{
			if (_name.Length != 0)
			{
				return _name;
			}
			return MilMo_Utility.ExtractNameFromPath(Path);
		}
	}

	public int NumberOfRows => _rows.Count;

	public bool IsEOF => _currentRow >= _rows.Count;

	public MilMo_SFFile(string path = "")
	{
		_rows = new List<List<string>>();
		_currentRow = -1;
		_currentToken = 0;
		_path = path;
	}

	public MilMo_SFFile(MilMo_SFFile file)
	{
		_rows = new List<List<string>>();
		_rows.AddRange(file._rows);
		_currentRow = file._currentRow;
		_currentToken = file._currentToken;
		_path = file.Path;
		_name = file._name;
	}

	public void SetName(string name)
	{
		_name = name;
	}

	public void SetPath(string path)
	{
		_path = path;
	}

	public void Reset()
	{
		_currentRow = -1;
		_currentToken = 0;
	}

	public void ResetLine()
	{
		_currentToken = 0;
	}

	public bool NextRow()
	{
		_currentToken = 0;
		_currentRow++;
		return _currentRow < _rows.Count;
	}

	public bool PrevRow()
	{
		_currentToken = 0;
		_currentRow = ((_currentRow > 0) ? (_currentRow - 1) : 0);
		return true;
	}

	public bool NextToken()
	{
		_currentToken++;
		return _currentToken < _rows[_currentRow].Count;
	}

	public bool HasMoreTokens()
	{
		if (_currentRow < 0 || _rows == null)
		{
			return false;
		}
		if (_currentRow >= _rows.Count)
		{
			return false;
		}
		return _currentToken < _rows[_currentRow].Count;
	}

	public int TokensLeft()
	{
		return _rows[_currentRow].Count - _currentRow - 1;
	}

	public int GetLineNumber()
	{
		return _currentRow + 1;
	}

	public string GetLine()
	{
		string text = "";
		foreach (string item in _rows[_currentRow])
		{
			text += item;
			text += " ";
		}
		return text.Trim();
	}

	public List<string> GetLineAsList()
	{
		return _rows[_currentRow];
	}

	public string GetRestOfLine()
	{
		string text = "";
		while (HasMoreTokens())
		{
			text += GetString();
			text += " ";
		}
		return text.Trim();
	}

	public bool IsNext(string token)
	{
		if (!HasMoreTokens())
		{
			return false;
		}
		if (GetString().Equals(token, StringComparison.InvariantCultureIgnoreCase))
		{
			return true;
		}
		_currentToken--;
		return false;
	}

	public string PeekString()
	{
		return _rows[_currentRow][_currentToken];
	}

	public bool PeekIsNext(string token)
	{
		if (HasMoreTokens())
		{
			return _rows[_currentRow][_currentToken].Equals(token, StringComparison.InvariantCultureIgnoreCase);
		}
		return false;
	}

	public string GetString()
	{
		return _rows[_currentRow][_currentToken++];
	}

	public int GetInt()
	{
		return int.Parse(_rows[_currentRow][_currentToken++]);
	}

	public float GetFloat()
	{
		try
		{
			NumberFormatInfo numberFormatInfo = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
			numberFormatInfo.NumberDecimalSeparator = ".";
			return float.Parse(_rows[_currentRow][_currentToken++], numberFormatInfo);
		}
		catch (FormatException ex)
		{
			Debug.LogWarning("Failed to read float in '" + Path + "(" + _name + ")' at line " + GetLineNumber() + "\n" + ex);
			return 0f;
		}
	}

	public Vector3 GetVector3()
	{
		return new Vector3(GetFloat(), GetFloat(), GetFloat());
	}

	public Vector4 GetVector4()
	{
		return new Vector4(GetFloat(), GetFloat(), GetFloat(), GetFloat());
	}

	public Vector2 GetVector2()
	{
		return new Vector2(GetFloat(), GetFloat());
	}

	public Rect GetRect()
	{
		return new Rect(GetFloat(), GetFloat(), GetFloat(), GetFloat());
	}

	public bool GetBool()
	{
		string @string = GetString();
		return true & !@string.Equals("false", StringComparison.InvariantCultureIgnoreCase) & (@string != "0");
	}

	public Color GetColor()
	{
		float @float = GetFloat();
		float float2 = GetFloat();
		float float3 = GetFloat();
		float a = (HasMoreTokens() ? GetFloat() : 1f);
		return new Color(@float, float2, float3, a);
	}

	public Color GetColorFromInt()
	{
		float r = (float)GetInt() / 255f;
		float g = (float)GetInt() / 255f;
		float b = (float)GetInt() / 255f;
		return new Color(r, g, b);
	}

	public bool WriteToDisk()
	{
		return WriteToDisk(Path);
	}

	public string GetEventTags()
	{
		string text = "";
		while (HasMoreTokens())
		{
			string @string = GetString();
			if (@string == "//")
			{
				break;
			}
			text = text + @string + " ";
		}
		return text.Trim();
	}

	public bool CheckEventTags()
	{
		List<string> list = new List<string>();
		if (IsNext("EventTag"))
		{
			while (HasMoreTokens())
			{
				string @string = GetString();
				if (@string == "//")
				{
					break;
				}
				list.Add(@string);
			}
		}
		foreach (string item in list)
		{
			if (!string.IsNullOrEmpty(item))
			{
				if (item.StartsWith("!") && MilMo_Global.EventTags.Contains(item.Substring(1)))
				{
					return false;
				}
				if (!item.StartsWith("!") && !MilMo_Global.EventTags.Contains(item))
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool WriteToDisk(string path)
	{
		if (!Directory.Exists(MilMo_Utility.RemoveFileNameFromFullPath(path)))
		{
			Directory.CreateDirectory(MilMo_Utility.RemoveFileNameFromFullPath(path));
		}
		StreamWriter streamWriter;
		try
		{
			streamWriter = new StreamWriter(path, append: false);
		}
		catch (IOException ex)
		{
			Debug.LogWarning("Failed to write simple format file '" + path + "' to disk\n" + ex);
			return false;
		}
		try
		{
			foreach (List<string> row in _rows)
			{
				string text = "";
				foreach (string item in row.Where((string token) => token.Length != 0))
				{
					text += item;
					text += " ";
				}
				if (text.Length != 0)
				{
					text = text.Trim();
					streamWriter.WriteLine(text);
				}
			}
		}
		catch (IOException ex2)
		{
			streamWriter.Dispose();
			streamWriter.Close();
			Debug.LogWarning("Failed to write simple format file '" + path + "' to disk\n" + ex2);
			return false;
		}
		streamWriter.Dispose();
		streamWriter.Close();
		return true;
	}

	public void AddAndWrite(string str)
	{
		AddRow();
		Write(str);
	}

	public void AddAndWrite(int i)
	{
		AddRow();
		Write(i);
	}

	public void AddAndWrite(float f)
	{
		AddRow();
		Write(f);
	}

	public void AddRow()
	{
		_rows.Add(new List<string>());
		_currentRow = _rows.Count - 1;
	}

	public void AddRow(List<string> row)
	{
		_rows.Add(row);
	}

	public void Write(string str)
	{
		if (str.Contains(" "))
		{
			WriteAsString(str);
		}
		else
		{
			_rows[_currentRow].Add(str);
		}
	}

	public void WriteAsString(string str)
	{
		_rows[_currentRow].Add("\"" + str + "\"");
	}

	public void WriteLine(string str)
	{
		string[] array = str.Split(' ');
		foreach (string item in array)
		{
			_rows[_currentRow].Add(item);
		}
	}

	public void Write(int i)
	{
		Write(i.ToString());
	}

	public void Write(float f)
	{
		NumberFormatInfo numberFormatInfo = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
		numberFormatInfo.NumberDecimalSeparator = ".";
		Write(((decimal)f).ToString(numberFormatInfo));
	}

	public void Write(bool b)
	{
		Write(b.ToString());
	}

	public void Write(byte b)
	{
		Write(b.ToString());
	}

	public void Write(Vector2 vector)
	{
		Write(vector.x);
		Write(vector.y);
	}

	public void Write(Vector3 vector)
	{
		Write(vector.x);
		Write(vector.y);
		Write(vector.z);
	}

	public void Write(Vector4 vector)
	{
		Write(vector.x);
		Write(vector.y);
		Write(vector.z);
		Write(vector.w);
	}

	public void Write(Color color)
	{
		Write(color.r);
		Write(color.g);
		Write(color.b);
		Write(color.a);
	}
}
