using RimWorld;
using Rokk.Playwright.Addons;
using Rokk.Playwright.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.Components.Boons
{
    public abstract class BoonComponent : PlaywrightComponent
    {
        public virtual string DescriptionShort => "Playwright.Components." + this.Id + ".DescriptionShort";
        public virtual string DescriptionShortTranslated => DescriptionShort.Translate();

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
                new BionicArmBoon()
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
