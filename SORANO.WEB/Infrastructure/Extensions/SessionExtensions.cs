using Microsoft.AspNetCore.Http;
using System.Linq;
using Newtonsoft.Json;

namespace SORANO.WEB.Infrastructure.Extensions
{
    public static class SessionExtensions
    {
        public static bool GetBool(this ISession session, string key)
        {
            if (!session.Keys.Contains(key))
            {
                return false;
            }

            bool.TryParse(session.GetString(key), out bool value);

            return value;
        }

        public static void SetBool(this ISession session, string key, bool value)
        {
            session.SetString(key, value ? "true" : "false");
        }

        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
