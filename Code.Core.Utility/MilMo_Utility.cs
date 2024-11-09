using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Core.Utility;

public static class MilMo_Utility
{
	private const float STANDARD_PRECISION = 1E-08f;

	public static void SetUnlockedMode(bool ignoreFullScreen = false)
	{
		if (Screen.fullScreen || ignoreFullScreen)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.lockState = CursorLockMode.Confined;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
		}
	}

	public static string PlatformAsString()
	{
		if (Application.platform == RuntimePlatform.OSXEditor)
		{
			return "OSXEditor";
		}
		if (Application.platform == RuntimePlatform.OSXPlayer)
		{
			return "OSXPlayer";
		}
		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			return "WindowsEditor";
		}
		if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			return "WindowsPlayer";
		}
		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			return "WebGLPlayer";
		}
		return "Unknown";
	}

	public static float Random()
	{
		return UnityEngine.Random.value;
	}

	public static int RandomInt(int min, int max)
	{
		if (min > max)
		{
			int num = min;
			min = max;
			max = num;
		}
		return Mathf.Min(UnityEngine.Random.Range(min, max + 1), max);
	}

	public static float RandomFloat(float min, float max)
	{
		if (min > max)
		{
			float num = min;
			min = max;
			max = num;
		}
		return Mathf.Min(UnityEngine.Random.Range(min, max + 1f), max);
	}

	public static int RandomID()
	{
		return (int)(UnityEngine.Random.value * 100000f);
	}

	public static Color RandomColor()
	{
		return new Color(Random(), Random(), Random(), 1f);
	}

	public static string ReplaceExtension(string path, string newExtension)
	{
		return RemoveExtension(path) + newExtension;
	}

	public static string RemoveExtension(string path)
	{
		int num = path.LastIndexOf('.');
		if (num != -1)
		{
			return path.Substring(0, num);
		}
		return path;
	}

	public static GameObject GetAncestor(GameObject gameObject)
	{
		while ((bool)gameObject.transform.parent)
		{
			gameObject = gameObject.transform.parent.gameObject;
		}
		return gameObject;
	}

	public static Transform GetChildBone(SkinnedMeshRenderer renderer, Transform parent)
	{
		if (!renderer)
		{
			Debug.LogWarning("Can't get child bone: renderer is null");
		}
		if (!parent)
		{
			Debug.LogWarning("Can't get child bone: parent is null");
			return null;
		}
		Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>();
		foreach (Transform child in componentsInChildren)
		{
			if (!(child == parent) && renderer.bones.Any((Transform bone) => child == bone))
			{
				return child;
			}
		}
		Debug.LogWarning("No child bone found");
		return null;
	}

	public static Transform GetChild(GameObject parent, string name)
	{
		return parent.GetComponentsInChildren<Transform>().FirstOrDefault((Transform child) => child != parent && child.name == name);
	}

	public static GameObject GetChildGameObject(GameObject parent, string name, bool includeInactive)
	{
		return (from child in parent.GetComponentsInChildren<Transform>(includeInactive)
			where child != parent && child.name == name
			select child.gameObject).FirstOrDefault();
	}

	public static DateTime GetDateTimeFromMilliseconds(long milliseconds)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(milliseconds);
	}

	public static string RemoveFileNameFromFullPath(string fullPath)
	{
		int num = ReplaceBackSlash(fullPath).LastIndexOf('/');
		if (num == -1)
		{
			return "";
		}
		if (num == fullPath.Length - 1)
		{
			return fullPath;
		}
		return fullPath.Substring(0, num + 1);
	}

	public static string ExtractExtension(string path)
	{
		int num = path.LastIndexOf('.');
		if (num != -1)
		{
			return path.Substring(num);
		}
		return "";
	}

	public static string ExtractNameFromPath(string path)
	{
		int num = Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
		if (num == -1)
		{
			return path;
		}
		if (num == path.Length - 1)
		{
			return "";
		}
		return path.Substring(num + 1, path.Length - num - 1);
	}

	public static string ExtractNameNoExtensionFromPath(string path)
	{
		string text = ExtractNameFromPath(path);
		int num = text.LastIndexOf(".", StringComparison.Ordinal);
		if (num == -1)
		{
			return text;
		}
		return text.Substring(0, num);
	}

	public static string ExtractPathFromStringNoExtension(string path, string subString)
	{
		int num = path.LastIndexOf(subString, StringComparison.Ordinal);
		if (num == -1)
		{
			return path;
		}
		return RemoveExtension(path.Substring(num + 6));
	}

	public static string RemoveLastFolder(string path)
	{
		if (path.EndsWith("/") || path.EndsWith("\\"))
		{
			path = path.Remove(path.Length - 1);
		}
		int num = path.LastIndexOf('\\');
		if (num != -1)
		{
			return path.Substring(0, num);
		}
		num = path.LastIndexOf('/');
		if (num == -1)
		{
			return path;
		}
		return path.Substring(0, num);
	}

	public static string RemoveCloneFromName(string gameObjectName)
	{
		if (gameObjectName.EndsWith("(Clone)"))
		{
			gameObjectName = gameObjectName.Remove(gameObjectName.Length - "(Clone)".Length);
		}
		return gameObjectName.Trim();
	}

	public static bool IsClose(float a, float b, float epsilon)
	{
		return Mathf.Abs(a - b) < epsilon;
	}

	public static bool IsClose(Vector3 a, Vector3 b, float epsilon)
	{
		return (a - b).sqrMagnitude < epsilon * epsilon;
	}

	public static bool IsClose(Quaternion a, Quaternion b, float epsilon)
	{
		return (a.eulerAngles - b.eulerAngles).sqrMagnitude < epsilon * epsilon;
	}

	public static string Md5(string s)
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] bytes = Encoding.ASCII.GetBytes(s);
		bytes = mD5CryptoServiceProvider.ComputeHash(bytes);
		return bytes.Aggregate("", (string current, byte b) => current + b.ToString("x2").ToLower());
	}

	public static string ScramblePassword(string text)
	{
		if (text == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder(text);
		StringBuilder stringBuilder2 = new StringBuilder(text.Length);
		int num = 3;
		for (int i = 0; i < text.Length; i++)
		{
			char c = stringBuilder[i];
			c = (char)(c ^ (num + i));
			stringBuilder2.Append(c);
		}
		return stringBuilder2.ToString();
	}

	public static float StringToFloat(string s)
	{
		NumberFormatInfo numberFormatInfo = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
		numberFormatInfo.NumberDecimalSeparator = ".";
		return float.Parse(s, numberFormatInfo);
	}

	public static int StringToInt(string s)
	{
		NumberFormatInfo numberFormatInfo = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
		numberFormatInfo.NumberDecimalSeparator = ".";
		return int.Parse(s, numberFormatInfo);
	}

	public static Vector3 StringArrayToVector(string[] args, int offset)
	{
		return new Vector3(StringToFloat(args[offset]), StringToFloat(args[offset + 1]), StringToFloat(args[offset + 2]));
	}

	public static bool StringToBool(string s)
	{
		return true & !s.Equals("false", StringComparison.InvariantCultureIgnoreCase) & (s != "0");
	}

	public static string ReplaceBackSlash(string path)
	{
		path = path.Replace("\\\\", "/");
		path = path.Replace("\\", "/");
		return path;
	}

	public static GameObject GetRoot(GameObject gameObject)
	{
		for (int i = 0; i < 100; i++)
		{
			if (gameObject.transform.parent == null)
			{
				return gameObject;
			}
			gameObject = gameObject.transform.parent.gameObject;
		}
		return gameObject;
	}

	public static Vector3 Snap(this Vector3 vector3, float gridSize = 1E-08f)
	{
		return new Vector3(Mathf.Round(vector3.x / gridSize) * gridSize, Mathf.Round(vector3.y / gridSize) * gridSize, Mathf.Round(vector3.z / gridSize) * gridSize);
	}

	public static bool Equals(float a, float b, float threshold)
	{
		return Mathf.Abs(a - b) < threshold;
	}

	public static bool Equals(Color a, Color b)
	{
		if (Mathf.Abs(a.r - b.r) < 0.001f && Mathf.Abs(a.g - b.g) < 0.001f && Mathf.Abs(a.b - b.b) < 0.001f)
		{
			return Mathf.Abs(a.a - b.a) < 0.001f;
		}
		return false;
	}

	public static bool Equals(Vector4 a, Vector4 b, float threshold = 0.001f)
	{
		if (Math.Abs(a.x - b.x) < threshold && Math.Abs(a.y - b.y) < threshold && Math.Abs(a.z - b.z) < threshold)
		{
			return Math.Abs(a.w - b.w) < threshold;
		}
		return false;
	}

	public static bool Equals(Vector3 a, Vector3 b, float threshold = 0.001f)
	{
		if (Math.Abs(a.x - b.x) < threshold && Math.Abs(a.y - b.y) < threshold)
		{
			return Math.Abs(a.z - b.z) < threshold;
		}
		return false;
	}

	public static bool Equals(Vector2 a, Vector2 b, float threshold = 0.001f)
	{
		if (Math.Abs(a.x - b.x) < threshold)
		{
			return Math.Abs(a.y - b.y) < threshold;
		}
		return false;
	}

	public static bool IsEmail(string email)
	{
		return new Regex("^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$").IsMatch(email);
	}

	public static KeyCode GetKeyCode(string keyCodeAsString)
	{
		return keyCodeAsString switch
		{
			"RightShift" => KeyCode.RightShift, 
			"LeftShift" => KeyCode.LeftShift, 
			"RightControl" => KeyCode.RightControl, 
			"LeftControl" => KeyCode.LeftControl, 
			"RightAlt" => KeyCode.RightAlt, 
			"LeftAlt" => KeyCode.LeftAlt, 
			"Mouse0" => KeyCode.Mouse0, 
			"Mouse1" => KeyCode.Mouse1, 
			"Mouse2" => KeyCode.Mouse2, 
			"Mouse3" => KeyCode.Mouse3, 
			"Mouse4" => KeyCode.Mouse4, 
			"Mouse5" => KeyCode.Mouse5, 
			"Mouse6" => KeyCode.Mouse6, 
			"A" => KeyCode.A, 
			"B" => KeyCode.B, 
			"C" => KeyCode.C, 
			"D" => KeyCode.D, 
			"E" => KeyCode.E, 
			"F" => KeyCode.F, 
			"G" => KeyCode.G, 
			"H" => KeyCode.H, 
			"I" => KeyCode.I, 
			"J" => KeyCode.J, 
			"K" => KeyCode.K, 
			"L" => KeyCode.L, 
			"M" => KeyCode.M, 
			"N" => KeyCode.N, 
			"O" => KeyCode.O, 
			"P" => KeyCode.P, 
			"Q" => KeyCode.Q, 
			"R" => KeyCode.R, 
			"S" => KeyCode.S, 
			"T" => KeyCode.T, 
			"U" => KeyCode.U, 
			"V" => KeyCode.V, 
			"W" => KeyCode.W, 
			"X" => KeyCode.X, 
			"Y" => KeyCode.Y, 
			"Z" => KeyCode.Z, 
			"F1" => KeyCode.F1, 
			"F2" => KeyCode.F2, 
			"F3" => KeyCode.F3, 
			"F4" => KeyCode.F4, 
			"F5" => KeyCode.F5, 
			"F6" => KeyCode.F6, 
			"F7" => KeyCode.F7, 
			"F8" => KeyCode.F8, 
			"F9" => KeyCode.F9, 
			"F10" => KeyCode.F10, 
			"F11" => KeyCode.F11, 
			"F12" => KeyCode.F12, 
			"F13" => KeyCode.F13, 
			"F14" => KeyCode.F14, 
			"F15" => KeyCode.F15, 
			"Alpha0" => KeyCode.Alpha0, 
			"Alpha1" => KeyCode.Alpha1, 
			"Alpha2" => KeyCode.Alpha2, 
			"Alpha3" => KeyCode.Alpha3, 
			"Alpha4" => KeyCode.Alpha4, 
			"Alpha5" => KeyCode.Alpha5, 
			"Alpha6" => KeyCode.Alpha6, 
			"Alpha7" => KeyCode.Alpha7, 
			"Alpha8" => KeyCode.Alpha8, 
			"Alpha9" => KeyCode.Alpha9, 
			"Backspace" => KeyCode.Backspace, 
			"Delete" => KeyCode.Delete, 
			"Tab" => KeyCode.Tab, 
			"Clear" => KeyCode.Clear, 
			"Return" => KeyCode.Return, 
			"Pause" => KeyCode.Pause, 
			"Escape" => KeyCode.Escape, 
			"Space" => KeyCode.Space, 
			"Keypad0" => KeyCode.Keypad0, 
			"Keypad1" => KeyCode.Keypad1, 
			"Keypad2" => KeyCode.Keypad2, 
			"Keypad3" => KeyCode.Keypad3, 
			"Keypad4" => KeyCode.Keypad4, 
			"Keypad5" => KeyCode.Keypad5, 
			"Keypad6" => KeyCode.Keypad6, 
			"Keypad7" => KeyCode.Keypad7, 
			"Keypad8" => KeyCode.Keypad8, 
			"Keypad9" => KeyCode.Keypad9, 
			"KeypadPeriod" => KeyCode.KeypadPeriod, 
			"KeypadDivide" => KeyCode.KeypadDivide, 
			"KeypadMultiply" => KeyCode.KeypadMultiply, 
			"KeypadMinus" => KeyCode.KeypadMinus, 
			"KeypadPlus" => KeyCode.KeypadPlus, 
			"KeypadEnter" => KeyCode.KeypadEnter, 
			"KeypadEquals" => KeyCode.KeypadEquals, 
			"UpArrow" => KeyCode.UpArrow, 
			"DownArrow" => KeyCode.DownArrow, 
			"RightArrow" => KeyCode.RightArrow, 
			"LeftArrow" => KeyCode.LeftArrow, 
			"Insert" => KeyCode.Insert, 
			"Home" => KeyCode.Home, 
			"End" => KeyCode.End, 
			"PageUp" => KeyCode.PageUp, 
			"PageDown" => KeyCode.PageDown, 
			"Exclaim" => KeyCode.Exclaim, 
			"DoubleQuote" => KeyCode.DoubleQuote, 
			"Hash" => KeyCode.Hash, 
			"Dollar" => KeyCode.Dollar, 
			"Ampersand" => KeyCode.Ampersand, 
			"Quote" => KeyCode.Quote, 
			"LeftParen" => KeyCode.LeftParen, 
			"RightParen" => KeyCode.RightParen, 
			"Asterisk" => KeyCode.Asterisk, 
			"Plus" => KeyCode.Plus, 
			"Comma" => KeyCode.Comma, 
			"Minus" => KeyCode.Minus, 
			"Period" => KeyCode.Period, 
			"Slash" => KeyCode.Slash, 
			"Colon" => KeyCode.Colon, 
			"Semicolon" => KeyCode.Semicolon, 
			"Less" => KeyCode.Less, 
			"Equals" => KeyCode.Equals, 
			"Greater" => KeyCode.Greater, 
			"Question" => KeyCode.Question, 
			"At" => KeyCode.At, 
			"LeftBracket" => KeyCode.LeftBracket, 
			"Backslash" => KeyCode.Backslash, 
			"RightBracket" => KeyCode.RightBracket, 
			"Caret" => KeyCode.Caret, 
			"Underscore" => KeyCode.Underscore, 
			"BackQuote" => KeyCode.BackQuote, 
			"Numlock" => KeyCode.Numlock, 
			"CapsLock" => KeyCode.CapsLock, 
			"ScrollLock" => KeyCode.ScrollLock, 
			"LeftApple" => KeyCode.LeftMeta, 
			"LeftWindows" => KeyCode.LeftWindows, 
			"RightApple" => KeyCode.RightMeta, 
			"RightWindows" => KeyCode.RightWindows, 
			"AltGr" => KeyCode.AltGr, 
			"Help" => KeyCode.Help, 
			"Print" => KeyCode.Print, 
			"SysReq" => KeyCode.SysReq, 
			"Joystick2Button0" => KeyCode.Joystick1Button0, 
			"Joystick2Button1" => KeyCode.Joystick1Button1, 
			"Joystick2Button2" => KeyCode.Joystick1Button2, 
			"Joystick2Button3" => KeyCode.Joystick1Button3, 
			"Joystick2Button4" => KeyCode.Joystick1Button4, 
			"Joystick2Button5" => KeyCode.Joystick1Button5, 
			"Joystick2Button6" => KeyCode.Joystick1Button6, 
			"Joystick2Button7" => KeyCode.Joystick1Button7, 
			"Joystick2Button8" => KeyCode.Joystick1Button8, 
			"Joystick2Button9" => KeyCode.Joystick1Button9, 
			"Joystick2Button10" => KeyCode.Joystick1Button10, 
			"Joystick2Button11" => KeyCode.Joystick1Button11, 
			"Joystick2Button12" => KeyCode.Joystick1Button12, 
			"Joystick2Button13" => KeyCode.Joystick1Button13, 
			"Joystick2Button14" => KeyCode.Joystick1Button14, 
			"Joystick2Button15" => KeyCode.Joystick1Button15, 
			"Joystick2Button16" => KeyCode.Joystick1Button16, 
			"Joystick2Button17" => KeyCode.Joystick1Button17, 
			"Joystick2Button18" => KeyCode.Joystick1Button18, 
			"Joystick2Button19" => KeyCode.Joystick1Button19, 
			_ => KeyCode.Joystick3Button19, 
		};
	}

	public static Key GetKeyFromKeyCode(KeyCode key)
	{
		return key switch
		{
			KeyCode.RightShift => Key.RightShift, 
			KeyCode.LeftShift => Key.LeftShift, 
			KeyCode.RightControl => Key.RightCtrl, 
			KeyCode.LeftControl => Key.LeftCtrl, 
			KeyCode.RightAlt => Key.RightAlt, 
			KeyCode.LeftAlt => Key.LeftAlt, 
			KeyCode.A => Key.A, 
			KeyCode.B => Key.B, 
			KeyCode.C => Key.C, 
			KeyCode.D => Key.D, 
			KeyCode.E => Key.E, 
			KeyCode.F => Key.F, 
			KeyCode.G => Key.G, 
			KeyCode.H => Key.H, 
			KeyCode.I => Key.I, 
			KeyCode.J => Key.J, 
			KeyCode.K => Key.K, 
			KeyCode.L => Key.L, 
			KeyCode.M => Key.M, 
			KeyCode.N => Key.N, 
			KeyCode.O => Key.O, 
			KeyCode.P => Key.P, 
			KeyCode.Q => Key.Q, 
			KeyCode.R => Key.R, 
			KeyCode.S => Key.S, 
			KeyCode.T => Key.T, 
			KeyCode.U => Key.U, 
			KeyCode.V => Key.V, 
			KeyCode.W => Key.W, 
			KeyCode.X => Key.X, 
			KeyCode.Y => Key.Y, 
			KeyCode.Z => Key.Z, 
			KeyCode.F1 => Key.F1, 
			KeyCode.F2 => Key.F2, 
			KeyCode.F3 => Key.F3, 
			KeyCode.F4 => Key.F4, 
			KeyCode.F5 => Key.F5, 
			KeyCode.F6 => Key.F6, 
			KeyCode.F7 => Key.F7, 
			KeyCode.F8 => Key.F8, 
			KeyCode.F9 => Key.F9, 
			KeyCode.F10 => Key.F10, 
			KeyCode.F11 => Key.F11, 
			KeyCode.F12 => Key.F12, 
			KeyCode.Alpha0 => Key.Digit0, 
			KeyCode.Alpha1 => Key.Digit1, 
			KeyCode.Alpha2 => Key.Digit2, 
			KeyCode.Alpha3 => Key.Digit3, 
			KeyCode.Alpha4 => Key.Digit4, 
			KeyCode.Alpha5 => Key.Digit5, 
			KeyCode.Alpha6 => Key.Digit6, 
			KeyCode.Alpha7 => Key.Digit7, 
			KeyCode.Alpha8 => Key.Digit8, 
			KeyCode.Alpha9 => Key.Digit9, 
			KeyCode.Backspace => Key.Backspace, 
			KeyCode.Delete => Key.Delete, 
			KeyCode.Tab => Key.Tab, 
			KeyCode.Clear => Key.Backspace, 
			KeyCode.Return => Key.Enter, 
			KeyCode.Pause => Key.Pause, 
			KeyCode.Escape => Key.Escape, 
			KeyCode.Space => Key.Space, 
			KeyCode.Keypad0 => Key.Numpad0, 
			KeyCode.Keypad1 => Key.Numpad1, 
			KeyCode.Keypad2 => Key.Numpad2, 
			KeyCode.Keypad3 => Key.Numpad3, 
			KeyCode.Keypad4 => Key.Numpad4, 
			KeyCode.Keypad5 => Key.Numpad5, 
			KeyCode.Keypad6 => Key.Numpad6, 
			KeyCode.Keypad7 => Key.Numpad7, 
			KeyCode.Keypad8 => Key.Numpad8, 
			KeyCode.Keypad9 => Key.Numpad9, 
			KeyCode.KeypadPeriod => Key.Period, 
			KeyCode.KeypadPlus => Key.NumpadPlus, 
			KeyCode.KeypadEnter => Key.Enter, 
			KeyCode.KeypadEquals => Key.Equals, 
			KeyCode.UpArrow => Key.UpArrow, 
			KeyCode.DownArrow => Key.DownArrow, 
			KeyCode.RightArrow => Key.RightArrow, 
			KeyCode.LeftArrow => Key.LeftArrow, 
			KeyCode.Insert => Key.Insert, 
			KeyCode.Home => Key.Home, 
			KeyCode.End => Key.End, 
			KeyCode.PageUp => Key.PageUp, 
			KeyCode.PageDown => Key.PageDown, 
			KeyCode.Plus => Key.NumpadPlus, 
			KeyCode.Comma => Key.Comma, 
			KeyCode.Minus => Key.Minus, 
			KeyCode.Period => Key.Period, 
			KeyCode.Slash => Key.Slash, 
			KeyCode.Semicolon => Key.Semicolon, 
			KeyCode.Equals => Key.Equals, 
			KeyCode.LeftBracket => Key.LeftBracket, 
			KeyCode.Backslash => Key.Backslash, 
			KeyCode.RightBracket => Key.RightBracket, 
			KeyCode.CapsLock => Key.CapsLock, 
			KeyCode.ScrollLock => Key.ScrollLock, 
			KeyCode.LeftMeta => Key.LeftMeta, 
			KeyCode.LeftWindows => Key.LeftMeta, 
			KeyCode.RightMeta => Key.RightMeta, 
			KeyCode.RightWindows => Key.RightMeta, 
			KeyCode.AltGr => Key.RightAlt, 
			KeyCode.Tilde => Key.Backquote, 
			KeyCode.BackQuote => Key.Backquote, 
			_ => Key.OEM5, 
		};
	}

	public static string GetAssetBundlesPath()
	{
		return RemoveLastFolder(Application.dataPath) + "/AssetBundles/";
	}
}
