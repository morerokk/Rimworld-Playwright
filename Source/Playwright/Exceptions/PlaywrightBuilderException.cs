using Rokk.Playwright.Composer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rokk.Playwright.Exceptions
{
    public class PlaywrightBuilderException : Exception
    {
        public PlaywrightStructure Playwright { get; private set; }

        public PlaywrightBuilderException(string message) : base(message)
        {
            
        }

        public PlaywrightBuilderException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public PlaywrightBuilderException(string message, PlaywrightStructure playwright) : base(message)
        {
            Playwright = playwright;
        }

        public PlaywrightBuilderException(string message, PlaywrightStructure playwright, Exception innerException) : base(message, innerException)
        {
            Playwright = playwright;
        }
    }
}
