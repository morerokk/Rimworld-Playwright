using RimWorld;
using Rokk.Playwright.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Rokk.Playwright.ScenParts
{
    public class StartWithNonMinifiedThing : ScenPart_ScatterThings
    {
        protected override bool NearPlayerStart => true;

        /// <summary>
        /// Exposes protected thingDef property
        /// </summary>
        public new ThingDef thingDef
        {
            get
            {
                return base.thingDef;
            }
            set
            {
                base.thingDef = value;
            }
        }

        public override void GenerateIntoMap(Map map)
        {
            // Very similar to ScatterThings, but does not spawn stuff minified
            // Also only on starting map, but that's probably handled by the GameInitData check anyway
            if (Find.GameInitData == null)
            {
                return;
            }
            new GenStep_ScatterThings
            {
                nearPlayerStart = true,
                allowFoggedPositions = false,
                thingDef = this.thingDef,
                stuff = this.stuff,
                count = 1,
                spotMustBeStandable = true,
                minSpacing = 5f,
                quality = this.quality,
                clusterSize = ((this.thingDef.category == ThingCategory.Building) ? 1 : 4),
                allowRoofed = false,
                minify = false,
                isJunk = false,
                onlyOnStartingMap = true
            }.Generate(map, default(GenStepParams));
        }

        // Default implementation skips minifiable stuff, for the sake of not having a HUGE list, only un-minifiable things are shown
        protected override IEnumerable<ThingDef> PossibleThingDefs()
        {
            return DefDatabase<ThingDef>.AllDefs.Where((ThingDef d) => (d.category == ThingCategory.Item && d.scatterableOnMapGen && !d.destroyOnDrop) || (d.category == ThingCategory.Building && !d.Minifiable));
        }
    }
}
