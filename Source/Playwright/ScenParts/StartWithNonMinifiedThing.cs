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
            // Very similar to ScatterThings, but does not minify
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
                minify = false
            }.Generate(map, default(GenStepParams));
        }
    }
}
