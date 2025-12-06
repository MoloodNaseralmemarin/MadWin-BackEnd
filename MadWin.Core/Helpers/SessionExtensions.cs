using Microsoft.AspNetCore.Http;
using System.Text.Json;

public static class SessionExtensions
{

    public static void SetJson(this ISession session, string key, object value) =>
        session.SetString(key, JsonSerializer.Serialize(value));

    public static T GetObjectFromJson<T>(this ISession session, string key) =>
        session.GetString(key) is string value ? JsonSerializer.Deserialize<T>(value) : default;

    // ذخیره آبجکت به صورت JSON
    public static void SetJson<T>(this ISession session, string key, T value)
    {
        if (value == null)
            session.Remove(key); // اگر مقدار null بود، کلید رو حذف کن
        else
            session.SetString(key, JsonSerializer.Serialize(value));
    }

    // دریافت آبجکت از JSON
    public static T GetJson<T>(this ISession session, string key)
    {
        var json = session.GetString(key);
        return json != null ? JsonSerializer.Deserialize<T>(json) : default;
    }

}
