namespace PlantCareMobile.Services;

public static class PlantMessenger
{
    private static readonly Dictionary<string, List<Action<object>>> _subscriptions = new();

    public static void Subscribe(string message, Action<object> callback)
    {
        if (!_subscriptions.ContainsKey(message))
        {
            _subscriptions[message] = new List<Action<object>>();
        }
        _subscriptions[message].Add(callback);
    }

    public static void Unsubscribe(string message, Action<object> callback)
    {
        if (_subscriptions.ContainsKey(message))
        {
            _subscriptions[message].Remove(callback);
        }
    }

    public static void Send(string message, object args)
    {
        if (_subscriptions.ContainsKey(message))
        {
            foreach (var callback in _subscriptions[message].ToList())
            {
                callback(args);
            }
        }
    }
}