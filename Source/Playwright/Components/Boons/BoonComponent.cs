using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components.Boons
{
    public abstract class BoonComponent : PlaywrightComponent
    {
        public static List<BoonComponent> GetAvailableBoons()
        {
            var list = new List<BoonComponent>();

            BoonComponent shuttleBoon = new ShuttleBoon();
            if (shuttleBoon.IsAvailable)
            {
                list.Add(shuttleBoon);
            }

            return list;
        }
    }
}
