using System.IO;
using DoTheSpriteThing.FileProcessors.Interfaces;

namespace DoTheSpriteThing.FileProcessors
{
    internal class CssProcessor : ICssProcessor
    {
        public void CreateCss(string css, string cssFilename)
        {
            File.WriteAllText(cssFilename, css);
        }
    }
}