using RimWorld;
using Rokk.Playwright.UI;
using Rokk.Playwright.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;

namespace Rokk.Playwright.Components.SpecialConditions
{
    public class DeathExplosionSpecialCondition : SpecialConditionComponent
    {
        public override string Id => "SpecialConditions.DeathExplosion";

        public float Radius = 5.9f;
        private string RadiusBuffer = "5.9";
        public DamageDef Damage = DamageDefOf.Bomb;

        private string DamageLabel => Damage != null ? Damage.LabelCap.ToString() : "-";

        private List<DamageDef> AllowedDamageTypes => new List<DamageDef>()
        {
            DamageDefOf.Bomb,
            DamageDefOf.Flame
        };

        public override void DoSettingsContents(Listing_AutoFitVertical specialConditionContentListing)
        {
            specialConditionContentListing.Label("Playwright.Components.SpecialConditions.DeathExplosion.Radius".Translate());
            specialConditionContentListing.TextFieldNumeric(ref Radius, ref RadiusBuffer, 0f);

            if (Damage == null)
            {
                Damage = DamageDefOf.Bomb;
            }
            specialConditionContentListing.Label("Playwright.Components.SpecialConditions.DeathExplosion.Damage".Translate());
            if (specialConditionContentListing.ButtonText(DamageLabel))
            {
                var options = new List<FloatMenuOption>();
                foreach (DamageDef damageDef in AllowedDamageTypes)
                {
                    options.Add(new FloatMenuOption(damageDef.LabelCap, () => Damage = damageDef));
                }
                PlaywrightUtils.OpenFloatMenu(options);
            }
        }

        public override void MutateScenario(Scenario scenario, List<ScenPart> scenarioParts)
        {
            scenarioParts.Add(ScenPartUtility.MakeOnPawnDeathExplodePart(Damage, Radius));
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Radius, nameof(Radius), 5.9f);
            Scribe_Defs.Look(ref Damage, nameof(Damage));
        }
    }
}
