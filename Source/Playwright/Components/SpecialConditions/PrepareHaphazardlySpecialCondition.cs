using RimWorld;
using Rokk.Playwright.Exceptions;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Rokk.Playwright.Components.SpecialConditions
{
    public class PrepareHaphazardlySpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.PrepareHaphazardly";

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            // Add twice as many colonist choices
            var configurePart = ScenPartUtils.GetConfigureStartingPawnsPart(scenarioParts);
            if (configurePart == null)
            {
                throw new PlaywrightBuilderException("Playwright.Components.SpecialConditions.PrepareHaphazardly.ErrorUnsupportedConfigPage".Translate());
            }
            configurePart.pawnChoiceCount *= 2;
            // Disable rerolls
            scenarioParts.Add(ScenPartUtils.MakeNoColonistRerollsPart());
        }
    }
}
