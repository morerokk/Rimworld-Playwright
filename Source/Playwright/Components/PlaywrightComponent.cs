using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Rokk.Playwright.Components
{
    public abstract class PlaywrightComponent
    {
        public abstract string Id { get; }

        public virtual string Name
        {
            get
            {
                return ("Playwright.Components." + this.Id);
            }
        }
        public virtual string Description
        {
            get
            {
                return ("Playwright.Components." + this.Id + ".Description");
            }
        }
        public virtual string NameTranslated
        {
            get
            {
                return ("Playwright.Components." + this.Id).Translate();
            }
        }
        public virtual string DescriptionTranslated
        {
            get
            {
                return ("Playwright.Components." + this.Id + ".Description").Translate();
            }
        }

        public abstract void MutateScenario(List<ScenPart> scenarioParts);
    }
}
