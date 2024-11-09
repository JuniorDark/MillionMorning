using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Core;
using UnityEngine;

namespace Code.Core.Template;

public class MilMo_TemplateContainer : Singleton<MilMo_TemplateContainer>
{
	public delegate MilMo_Template MilMo_TemplateCreator(string category, string path, string filePath);

	public delegate void TemplateArrivedCallback(MilMo_Template template, bool timeOut);

	private readonly Dictionary<string, Dictionary<string, MilMo_Template>> _categories = new Dictionary<string, Dictionary<string, MilMo_Template>>();

	private readonly Dictionary<string, MilMo_TemplateCreator> _creators = new Dictionary<string, MilMo_TemplateCreator>(StringComparer.InvariantCultureIgnoreCase);

	private readonly Dictionary<string, bool> _pendingRequests = new Dictionary<string, bool>();

	private bool _initialized;

	private MilMo_GenericReaction _listener;

	public void Init()
	{
		if (!_initialized)
		{
			_initialized = true;
			_listener = MilMo_EventSystem.Listen("template_received", Singleton<MilMo_TemplateContainer>.Instance.TemplateReceivedFromNetwork);
			_listener.Repeating = true;
			MilMo_TemplateCreators.AddCreators();
		}
	}

	private void Start()
	{
		Init();
	}

	private void OnDestroy()
	{
		MilMo_EventSystem.RemoveReaction(_listener);
		_listener = null;
	}

	public void AddCreator(string type, MilMo_TemplateCreator creator)
	{
		_creators.Add(type, creator);
	}

	private MilMo_Template AddTemplate(MilMo_Template template)
	{
		if (!_categories.TryGetValue(template.Category, out var value))
		{
			value = new Dictionary<string, MilMo_Template>();
			_categories.Add(template.Category, value);
		}
		if (value.TryGetValue(template.Path, out var value2))
		{
			return value2;
		}
		value.Add(template.Path, template);
		return template;
	}

	private MilMo_Template GetTemplateFromContainer(string category, string path)
	{
		if (!_categories.TryGetValue(category, out var value))
		{
			return null;
		}
		value.TryGetValue(path, out var value2);
		return value2;
	}

	public async Task<MilMo_Template> GetTemplateAsync(TemplateReference reference)
	{
		TaskCompletionSource<MilMo_Template> tcs = new TaskCompletionSource<MilMo_Template>();
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(reference, delegate(MilMo_Template template, bool timeout)
		{
			if (template == null || timeout)
			{
				string text = reference.GetCategory() + ":" + reference.GetIdentifier();
				Debug.LogWarning("Failed to load: " + text);
			}
			tcs.TrySetResult(template);
		});
		return await tcs.Task;
	}

	public async Task<MilMo_Template> GetTemplateAsync(string identifier)
	{
		TaskCompletionSource<MilMo_Template> tcs = new TaskCompletionSource<MilMo_Template>();
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(identifier, delegate(MilMo_Template template, bool timeout)
		{
			if (template == null || timeout)
			{
				Debug.LogWarning(new Exception("Failed to load: " + identifier));
			}
			tcs.TrySetResult(template);
		});
		return await tcs.Task;
	}

	public void GetTemplate(string identifier, TemplateArrivedCallback callback)
	{
		string[] array = identifier.Split(':');
		if (array.Length < 2)
		{
			callback?.Invoke(null, timeOut: false);
		}
		else
		{
			GetTemplate(array[0], array[1], callback);
		}
	}

	public void GetTemplate(TemplateReference reference, TemplateArrivedCallback callback)
	{
		string category = reference.GetCategory();
		string path = reference.GetPath();
		GetTemplate(category, path, callback);
	}

	public void GetTemplate(string category, string path, TemplateArrivedCallback callback)
	{
		MilMo_Template templateFromContainer = GetTemplateFromContainer(category, path);
		if (templateFromContainer != null)
		{
			callback(templateFromContainer, timeOut: false);
			return;
		}
		templateFromContainer = LoadSFFTemplateFromResources(category, path);
		if (templateFromContainer != null)
		{
			callback(templateFromContainer, timeOut: false);
			return;
		}
		string identifier = category + path;
		MilMo_TimerEvent timeoutListener = null;
		MilMo_GenericReaction reaction = MilMo_EventSystem.Listen("template_received" + identifier, delegate(object templateAsObject)
		{
			_pendingRequests.Remove(identifier);
			MilMo_EventSystem.RemoveTimerEvent(timeoutListener);
			callback(templateAsObject as MilMo_Template, timeOut: false);
		});
		if (!_pendingRequests.ContainsKey(identifier))
		{
			Singleton<GameNetwork>.Instance.RequestTemplate(category, path);
			_pendingRequests.Add(identifier, value: true);
		}
		timeoutListener = MilMo_EventSystem.At(10f, delegate
		{
			Debug.LogWarning("Template timed out, " + identifier);
			_pendingRequests.Remove(identifier);
			MilMo_EventSystem.RemoveReaction(reaction);
			callback(null, timeOut: true);
		});
	}

	public MilMo_Template GetTemplate(TemplateReference reference)
	{
		string category = reference.GetCategory();
		string path = reference.GetPath();
		return GetTemplate(category, path);
	}

	public MilMo_Template GetTemplate(string category, string path)
	{
		return GetTemplateFromContainer(category, path) ?? LoadSFFTemplateFromResources(category, path);
	}

	private void TemplateReceivedFromNetwork(object msgAsObj)
	{
		if (!(msgAsObj is ServerTemplate serverTemplate))
		{
			Debug.LogWarning("Got non ServerTemplate when trying to read template from network message");
			return;
		}
		MilMo_Template milMo_Template;
		try
		{
			milMo_Template = LoadTemplateFromNetworkMessage(serverTemplate.GetTemplate());
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Failed to load template from network message\n" + ex);
			return;
		}
		if (milMo_Template == null)
		{
			Debug.LogWarning("Failed to load template from network message");
		}
		else
		{
			MilMo_EventSystem.Instance.PostEvent("template_received" + milMo_Template.Category + milMo_Template.Path, milMo_Template);
		}
	}

	public MilMo_Template LoadTemplateFromNetworkMessage(Code.Core.Network.types.Template t)
	{
		string templateType = t.GetTemplateType();
		if (!_creators.TryGetValue(templateType, out var value))
		{
			Debug.LogWarning("Trying to create template with unknown type " + templateType);
			return null;
		}
		string category = t.GetReference().GetCategory();
		string path = t.GetReference().GetPath();
		string filePath = path.Replace('.', '/');
		MilMo_Template milMo_Template = value(category, path, filePath);
		if (milMo_Template == null)
		{
			Debug.LogWarning("Failed to create template from network message. Creator returned null for template " + path);
			return null;
		}
		if (!milMo_Template.LoadFromNetwork(t))
		{
			Debug.LogWarning("Failed to load template " + path + " from network message");
			return null;
		}
		return AddTemplate(milMo_Template);
	}

	private MilMo_Template LoadSFFTemplateFromResources(string category, string path)
	{
		MilMo_SFFile milMo_SFFile = MilMo_SimpleFormat.LoadLocal(path.Replace('.', '/'));
		if (milMo_SFFile == null)
		{
			return null;
		}
		if (!milMo_SFFile.NextRow())
		{
			Debug.LogWarning("Missing rows, empty template, " + milMo_SFFile.Path);
			return null;
		}
		if (!milMo_SFFile.GetString().Equals("Template", StringComparison.InvariantCultureIgnoreCase))
		{
			Debug.LogWarning("Missing rows, no template keyword in file, " + milMo_SFFile.Path);
			return null;
		}
		milMo_SFFile.NextRow();
		string @string = milMo_SFFile.GetString();
		if (!_creators.TryGetValue(@string, out var value))
		{
			Debug.LogWarning("Trying to load template of unknown type " + @string + " in file " + milMo_SFFile.Path + " at line " + milMo_SFFile.GetLineNumber());
			return null;
		}
		MilMo_Template milMo_Template = value(category, path, milMo_SFFile.Path);
		if (milMo_Template == null)
		{
			return null;
		}
		while (milMo_SFFile.NextRow())
		{
			if (!milMo_Template.ReadLine(milMo_SFFile))
			{
				Debug.LogWarning("Failed reading template " + category + ":" + path + " at line " + milMo_SFFile.GetLineNumber());
				milMo_Template = null;
				break;
			}
		}
		if (milMo_Template != null && milMo_Template.FinishLoading())
		{
			return AddTemplate(milMo_Template);
		}
		Debug.LogWarning("Failed to finalize load of template " + category + ":" + path);
		return null;
	}

	public static MilMo_TemplateContainer Get()
	{
		return Singleton<MilMo_TemplateContainer>.Instance;
	}
}
