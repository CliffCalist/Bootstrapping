using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class TypeReflectionExtensions
{
    public static IEnumerable<Type> GetTypesSafe(this Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t != null);
        }
    }
}