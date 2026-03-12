using Rokk.Playwright.Components.Origins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokk.Playwright.Addons
{
    /// <summary>
    /// Helper class that can be used to register components from mod compatibility layers, or from addons.
    /// </summary>
    public static class ComponentRegistration
    {
        private static List<OriginComponent> RegisteredOrigins = new List<OriginComponent>();

        public static IEnumerable<OriginComponent> Origins => RegisteredOrigins.AsReadOnly();

        /// <summary>
        /// Register a new origin to be usable in Playwright.
        /// </summary>
        /// <param name="origin">The origin to register.</param>
        public static void RegisterOrigin(OriginComponent origin)
        {
            RegisteredOrigins.Add(origin);
        }
    }
}
