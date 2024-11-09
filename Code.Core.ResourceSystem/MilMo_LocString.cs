using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Core.ResourceSystem;

[Serializable]
public sealed class MilMo_LocString
{
	public class Tag
	{
		public int Index;

		public string Content;
	}

	public delegate void OnStringChanged(MilMo_LocString locStringChanged);

	private string _locString;

	private readonly bool _readOnly;

	private object[] _formatArguments;

	private MilMo_LocString _parentLocString;

	public static readonly MilMo_LocString Empty = new MilMo_LocString("", removeTags: false, readOnly: true);

	private readonly List<OnStringChanged> _changedCallbacks = new List<OnStringChanged>();

	private static readonly Dictionary<int, MilMo_LocString> Integers = new Dictionary<int, MilMo_LocString>();

	public List<Tag> Tags { get; private set; } = new List<Tag>();


	public string Identifier { get; }

	public string String
	{
		get
		{
			if (_parentLocString != null)
			{
				_locString = _parentLocString._locString;
			}
			if (_formatArguments == null)
			{
				return _locString;
			}
			try
			{
				return string.Format(_locString, _formatArguments);
			}
			catch (FormatException ex)
			{
				Debug.LogWarning("Got format exception when trying to format loc string " + Identifier);
				Debug.LogWarning(ex.ToString());
				return _locString;
			}
		}
	}

	public int Length => String.Length;

	public bool IsEmpty => String.Length == 0;

	public bool WantsFormatArgs
	{
		get
		{
			if (_parentLocString == null)
			{
				if (_locString.Contains("{"))
				{
					return _locString.Contains("}");
				}
				return false;
			}
			if (_parentLocString._locString.Contains("{"))
			{
				return _parentLocString._locString.Contains("}");
			}
			return false;
		}
	}

	public static MilMo_LocString Integer(int i)
	{
		if (Integers.TryGetValue(i, out var value))
		{
			return value;
		}
		value = new MilMo_LocString(i.ToString(), removeTags: false);
		Integers.Add(i, value);
		return value;
	}

	public override int GetHashCode()
	{
		return String.GetHashCode();
	}

	public static bool operator ==(MilMo_LocString a, MilMo_LocString b)
	{
		if ((object)a == b)
		{
			return true;
		}
		if ((object)a == null || (object)b == null)
		{
			return false;
		}
		return a.String == b.String;
	}

	public override bool Equals(object o)
	{
		return this == (MilMo_LocString)o;
	}

	public static bool operator !=(MilMo_LocString a, MilMo_LocString b)
	{
		return !(a == b);
	}

	public static MilMo_LocString operator +(MilMo_LocString a, MilMo_LocString b)
	{
		return new MilMo_LocString(a.String + b.String, removeTags: false);
	}

	public MilMo_LocString(string locString, bool removeTags)
	{
		_locString = locString;
		if (removeTags)
		{
			ExtractEmoteTags();
		}
	}

	public MilMo_LocString(string locString, string identifier)
	{
		_locString = locString;
		Identifier = identifier;
		ExtractEmoteTags();
	}

	private MilMo_LocString(string locString, bool removeTags, bool readOnly)
		: this(locString, removeTags)
	{
		_readOnly = readOnly;
	}

	public void RegisterOnStringChangedCallback(OnStringChanged callback)
	{
		if (_parentLocString == null)
		{
			_changedCallbacks.Add(callback);
		}
		else
		{
			_parentLocString._changedCallbacks.Add(callback);
		}
	}

	public void UpdateString(string locString)
	{
		if (_readOnly)
		{
			throw new InvalidOperationException("Trying to alter read-only localized string.");
		}
		if (_parentLocString != null)
		{
			throw new InvalidOperationException("Trying to alter copy of localized string.");
		}
		_locString = locString;
		ExtractEmoteTags();
		foreach (OnStringChanged changedCallback in _changedCallbacks)
		{
			changedCallback(this);
		}
	}

	public void SetFormatArgs(params object[] args)
	{
		if (_readOnly)
		{
			throw new InvalidOperationException("Trying to set format arguments on read-only localized string.");
		}
		_formatArguments = args;
	}

	public MilMo_LocString GetCopy()
	{
		if (_readOnly)
		{
			throw new InvalidOperationException("Trying to copy a read-only localized string.");
		}
		MilMo_LocString milMo_LocString = ((_parentLocString == null) ? this : _parentLocString);
		return new MilMo_LocString(milMo_LocString._locString, milMo_LocString.Identifier)
		{
			Tags = milMo_LocString.Tags,
			_parentLocString = milMo_LocString
		};
	}

	public override string ToString()
	{
		return String;
	}

	private void ExtractEmoteTags()
	{
		if (string.IsNullOrEmpty(_locString))
		{
			return;
		}
		char[] separator = new char[2] { '<', '>' };
		string[] array = _locString.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		bool flag = _locString[0] == '<';
		string text = "";
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			if (!flag)
			{
				text += text2;
				flag = true;
				continue;
			}
			Tag item = new Tag
			{
				Content = text2,
				Index = text.Length
			};
			Tags.Add(item);
			flag = false;
		}
		_locString = text;
	}
}
