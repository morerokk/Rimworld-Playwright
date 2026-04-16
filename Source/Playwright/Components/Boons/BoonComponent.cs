using Rokk.Playwright.Addons;
using Rokk.Playwright.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;

namespace Rokk.Playwright.Components.Boons
{
    public abstract class BoonComponent : PlaywrightComponent
    {
        /// <summary>
        /// If true, a question mark button that shows help text will be shown on the component UI.
        /// Expects the translation key "Playwright.Components.{ComponentId}.Help" to be present.
        /// </summary>
        public virtual bool HasHelp => false;

        public virtual void DoBoonContents(Listing_AutoFitVertical boonContentListing)
        {
            Text.Font = GameFont.Medium;
            boonContentListing.Label(NameTranslated);
            Text.Font = GameFont.Small;
            boonContentListing.Gap();
            boonContentListing.Label(DescriptionTranslated);
            DoSettingsContents(boonContentListing);
        }

        public virtual void DoSettingsContents(Listing_AutoFitVertical boonContentListing)
        {

        }

        public static List<BoonComponent> GetAvailableBoons()
        {
            List<BoonComponent> result = new List<BoonComponent>();

            List<BoonComponent> playwrightBoons = new List<BoonComponent>()
            {
                new ExtraItemsBoon(),
                new ExtraImplantsBoon(),
                new NovicesBoon(),
                new ShuttleBoon()
            };

            foreach (BoonComponent boon in playwrightBoons)
            {
                if (boon.IsAvailable)
                {
                    result.Add(boon);
                }
            }

            foreach (BoonComponent boon in ComponentRegistration.Boons)
            {
                if (boon.IsAvailable)
                {
                    result.Add(boon);
                }
            }

            return result;
        }
    }
}
