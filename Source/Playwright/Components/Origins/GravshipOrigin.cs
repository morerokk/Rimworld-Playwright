using RimWorld;
using Rokk.Playwright.Components.SpecialConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace Rokk.Playwright.Components.Origins
{
    public class GravshipOrigin : OriginComponent
    {
        public override string Id => "Origins.Gravship";
        public override ScenarioDef BasedOnScenario => DefOfs.ScenarioDefOf.TheGravship;
        public override bool IsAvailable => ModsConfig.OdysseyActive;
        public override List<SpecialConditionComponent> DefaultSpecialConditions => new List<SpecialConditionComponent>()
        {
            new PursuingMechanoidsSpecialCondition()
        };

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            // Remove "hunted by mechanoids" part, since this origin adds it as default special condition anyway.
            // This lets the player disable pursuing mechanoids (or enable them on other scenarios!)
            ScenPart_PursuingMechanoids pursuingMechanoidsPart = scenarioParts
                .Where(part => part.def == DefOfs.ScenPartDefOf.PursuingMechanoids)
                .Cast<ScenPart_PursuingMechanoids>()
                .FirstOrDefault();
            if (pursuingMechanoidsPart != null)
            {
                scenarioParts.Remove(pursuingMechanoidsPart);
            }
        }
    }
}
