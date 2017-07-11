using Microsoft.AspNetCore.Http;
using System.Linq;

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
    }
}
