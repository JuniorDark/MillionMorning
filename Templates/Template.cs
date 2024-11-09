using UnityEngine;

namespace Templates;

public abstract class Template : ScriptableObject
{
	public string type = "Template";

	public string category;

	public string path;

	public string identifier;

	public string filePath;

	public string eventTag;

	[TextArea(4, 14)]
	public string raw;
}
