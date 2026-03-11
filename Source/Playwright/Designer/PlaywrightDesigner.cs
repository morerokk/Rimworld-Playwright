using Rokk.Playwright.Composer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rokk.Playwright.Designer
{
    /// <summary>
    /// The designer for a Playwright. Holds the structure and handles the UI stuff,
    /// and eventually hands it over to the builder/composer to compile the scenario.
    /// </summary>
    public class PlaywrightDesigner
    {
        public PlaywrightStructure Playwright { get; private set; } = new PlaywrightStructure();

    }
}
