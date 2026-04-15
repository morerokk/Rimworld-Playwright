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
            List<BoonComponent> boons = new List<BoonComponent>()
            {
                new ExtraItemsBoon(),
                new BionicsBoon()
            };

            BoonComponent shuttleBoon = new ShuttleBoon();
            if (shuttleBoon.IsAvailable)
            {
                boons.Add(shuttleBoon);
            }

            BoonComponent novicesBoon = new NovicesBoon();
            if (novicesBoon.IsAvailable)
            {
                boons.Add(novicesBoon);
            }

            foreach (BoonComponent boon in ComponentRegistration.Boons)
            {
                if (boon.IsAvailable)
                {
                    boons.Add(boon);
                }
            }

            return boons;
        }
    }
}
