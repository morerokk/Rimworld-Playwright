using Rokk.Playwright.Addons;
using Rokk.Playwright.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace Rokk.Playwright.Components.SpecialConditions
{
    public abstract class SpecialConditionComponent : PlaywrightComponent
    {
        /// <summary>
        /// If true, a question mark button that shows help text will be shown on the component UI.
        /// Expects the translation key "Playwright.Components.{ComponentId}.Help" to be present.
        /// </summary>
        public virtual bool HasHelp => false;

        public virtual void DoSpecialConditionContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            Text.Font = GameFont.Medium;
            specialConditionContentListing.Label(NameTranslated);
            Text.Font = GameFont.Small;
            specialConditionContentListing.Gap();
            specialConditionContentListing.Label(DescriptionTranslated);
            DoSettingsContents(specialConditionContentListing);
        }

        public virtual void DoSettingsContents(Listing_AutoFitVertical specialConditionContentListing)
        {

        }

        public static List<SpecialConditionComponent> GetAvailableSpecialConditions()
        {
            List<SpecialConditionComponent> specialConditions = new List<SpecialConditionComponent>()
            {
                new PlanetkillerSpecialCondition()
            };

            SpecialConditionComponent xenotypeSwapCondition = new XenotypeSwapSpecialCondition();
            if (xenotypeSwapCondition.IsAvailable)
            {
                specialConditions.Add(xenotypeSwapCondition);
            }

            specialConditions.Add(new PrepareHaphazardlySpecialCondition());
            specialConditions.Add(new DeathExplosionSpecialCondition());

            SpecialConditionComponent disableMechanoidSignalCondition = new DisableMechanoidSignalSpecialCondition();
            if (disableMechanoidSignalCondition.IsAvailable)
            {
                specialConditions.Add(disableMechanoidSignalCondition);
            }
            SpecialConditionComponent pursuingMechanoidsCondition = new PursuingMechanoidsSpecialCondition();
            if (pursuingMechanoidsCondition.IsAvailable)
            {
                specialConditions.Add(pursuingMechanoidsCondition);
            }

            foreach (SpecialConditionComponent specialCondition in ComponentRegistration.SpecialConditions)
            {
                if (specialCondition.IsAvailable)
                {
                    specialConditions.Add(specialCondition);
                }
            }

            return specialConditions;
        }
    }
}
