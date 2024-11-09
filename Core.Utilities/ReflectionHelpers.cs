using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Utilities;

public static class ReflectionHelpers
{
	public static Type[] GetAllDerivedTypes(this AppDomain aAppDomain, Type aType)
	{
		List<Type> list = new List<Type>();
		Assembly[] assemblies = aAppDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			Type[] types = assemblies[i].GetTypes();
			foreach (Type type in types)
			{
				if (type.IsSubclassOf(aType))
				{
					list.Add(type);
				}
			}
		}
		return list.ToArray();
	}

	public static Type[] GetAllDerivedTypes<T>(this AppDomain aAppDomain)
	{
		return aAppDomain.GetAllDerivedTypes(typeof(T));
	}

	public static Type[] GetTypesWithInterface(this AppDomain aAppDomain, Type aInterfaceType)
	{
		List<Type> list = new List<Type>();
		Assembly[] assemblies = aAppDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			Type[] types = assemblies[i].GetTypes();
			foreach (Type type in types)
			{
				if (aInterfaceType.IsAssignableFrom(type))
				{
					list.Add(type);
				}
			}
		}
		return list.ToArray();
	}

	public static Type[] GetTypesWithInterface<T>(this AppDomain aAppDomain)
	{
		return aAppDomain.GetTypesWithInterface(typeof(T));
	}
}
