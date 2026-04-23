using Rokk.Playwright.Addons;
using Rokk.Playwright.Components.Origins;
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

        /// <summary>
        /// Draw the boon contents. By default, this also draws its settings.
        /// </summary>
        /// <param name="boonContentListing">The Listing that this boon is drawn in.</param>
        /// <param name="origin">The player's currently chosen Origin. Ideally, should ONLY be read from! Avoid writing to the Origin directly.</param>
        public virtual void DoBoonContents(Listing_AutoFitVertical boonContentListing, OriginComponent origin)
        {
            Text.Font = GameFont.Medium;
            boonContentListing.Label(NameTranslated);
            Text.Font = GameFont.Small;
            boonContentListing.Gap();
            boonContentListing.Label(DescriptionTranslated);
            DoSettingsContents(boonContentListing, origin);
        }

        protected virtual void DoSettingsContents(Listing_AutoFitVertical boonContentListing, OriginComponent origin)
        {

        }

        public static List<BoonComponent> GetAvailableBoons()
        {
            List<BoonComponent> result = new List<BoonComponent>();

            List<BoonComponent> playwrightBoons = new List<BoonComponent>()
            {
                new ExtraItemsBoon(),
                new ExtraAnimalsBoon(),
                new ExtraImplantsBoon(),
                new StartingResearchBoon(),
                new NovicesBoon(),
                new ShuttleBoon(),
                new AgeLimitBoon()
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
