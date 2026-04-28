using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _map = new();

    public static void Register<T>(T service) => _map[typeof(T)] = service;
    public static T Get<T>() => _map.TryGetValue(typeof(T), out var s) ? (T)s : default;
    public static bool Has<T>() => _map.ContainsKey(typeof(T));
}
