using System.Collections.Generic;

namespace SEOAnalyser.Models.ViewModels
{
    public class SEOAnalyserViewModel
    {
        public Dictionary<string, int> WordsCount { get; set; }
        public Dictionary<string, int> MetaWordsCount { get; set; }
        public Dictionary<string, int> ExternalLinksCount { get; set; }

        public bool CalcMetaTags { get; set; }
        public bool RemoveStopWords { get; set; }

        public bool CalcExternalLinks { get; set; }

        public bool IsKeyOrderASCT1 { get; set; }
        public bool IsValueOrderASCT1 { get; set; }

        public bool IsKeyOrderASCT2 { get; set; }
        public bool IsValueOrderASCT2 { get; set; }

        public bool IsKeyOrderASCT3 { get; set; }
        public bool IsValueOrderASCT3 { get; set; }

        public int? page { get; set; }

        public SEOAnalyserViewModel()
        {
            this.WordsCount = new Dictionary<string, int>();
            this.MetaWordsCount = new Dictionary<string, int>();
            this.ExternalLinksCount = new Dictionary<string, int>();
        }
    }
}