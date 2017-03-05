using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using SEOAnalyser.Helpers;
using System.Collections;

namespace SEOAnalyser.Models
{
    public class HtmlAnayser : TextSEOAnalyser
    {
        public string path { get; set; }
        private string _htmlCode { get; set; }
        public string _cleanedText { get; set; }

        public Dictionary<string, int> MetaWordsCount = new Dictionary<string, int>();
        public Dictionary<string, int> ExternalLinks = new Dictionary<string, int>();

        private List<string> _metalist = new List<string>();

        public async Task<string> GetHtmlFromUrl(string url)
        {
            try
            {
                WebClient client = new WebClient();
                Uri _uri = new Uri(url);
                try
                {
                    _htmlCode = await client.DownloadStringTaskAsync(_uri);
                }
                catch (Exception)
                {

                    throw new Exception("$Something went wrong when downloading html from " + url);
                }
              
                _cleanedText = RemoveScriptsTags(_htmlCode);
                _cleanedText = RemoveStylesTags(_cleanedText);

                _cleanedText = _cleanedText.RemoveAllHtmlTags();

                _cleanedText = _cleanedText.RemoveAllMultipleWhiteSpaces();

                _cleanedText = _cleanedText.RemoveAllNonCharacters();

                
                Text = _cleanedText;
                return _cleanedText;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void GetMetatagWords()
        {
            try
            {
                if (string.IsNullOrEmpty(_htmlCode)) throw new ArgumentNullException();

                Regex metaTag = new Regex(@"<meta.*?>");
                Dictionary<string, string> metaInformation = new Dictionary<string, string>();
                

                foreach (Match m in metaTag.Matches(_htmlCode))
                {
                    string value = m.Groups[0].Value;

                    Match m2 = Regex.Match(m.Value, @"content=\""(.*?)\""");
                    var contentValue = m2.Groups[1].Value;


                    if (!contentValue.IsValidUrl())
                        _metalist.Add(contentValue.RemoveAllNonCharacters());

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void CalculateMetaWordsCount()
        {
            try
            {
                if (string.IsNullOrEmpty(Text)) throw new ArgumentNullException("Text");

                string[] _totalMetaWords;
                ArrayList list = new ArrayList();
                foreach (var item in _metalist)
                {
                    var wordArray = item.ToWordsArray().Distinct().ToArray();
                    if (wordArray.Count() > 0)
                    {
                        list.AddRange(wordArray);
                    }
                }

                _totalMetaWords = (string[])list.ToArray(typeof(string));

                if (TotalWords.Count() == 0) TotalWords = Text.ToWordsArray();

                foreach (var item in _totalMetaWords)
                {
                    var count = TotalWords.Where(m => m.ToLower().Equals(item.ToLower())).Count();
                    if (!MetaWordsCount.ContainsKey(item.ToLower()))
                    {
                        MetaWordsCount.Add(item.ToLower(), count);
                    }

                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Remove inline scripts tag
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string RemoveScriptsTags(string text)
        {
            try
            {
                Regex regxScriptRemoval = new Regex(@"<script(.+)</script>");

                text = regxScriptRemoval.Replace(text, string.Empty);

                int from = 0, length = 0;
                while (text.IndexOf("<script") != -1)
                {
                    from = text.IndexOf("<script");
                    length = text.IndexOf("</script") - from + 9;
                    var _scripts = text.Substring(from, length);

                    text = text.Replace(_scripts, string.Empty);
                }

                return text; 
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Remove inline style tags. It is not removed inline style tags by regex
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string RemoveStylesTags(string text)
        {
            try
            {
                Regex regxStyleRemoval = new Regex(@"<style(.+)</style>");

                text = regxStyleRemoval.Replace(text, string.Empty);


                int from = 0, length = 0;
                while (text.IndexOf("<style") != -1)
                {
                    from = text.IndexOf("<style");
                    length = text.IndexOf("</style") - from + 8;
                    var _style = text.Substring(from, length);

                    text = text.Replace(_style, string.Empty);
                }

                return text;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void CalclulateExternelLinks()
        {
            try
            {
                List<string> _externalLinks = new List<string>();
                Uri _uri;
                string _path = string.Empty, pattern = string.Empty;


                MatchCollection m1 = Regex.Matches(_htmlCode, @"(<a.*?>.*?</a>)",RegexOptions.Singleline);
               
                foreach (Match m in m1)
                {
                    string value = m.Groups[1].Value;
                   
                    Match m2 = Regex.Match(value, @"href=\""(.*?)\""", RegexOptions.Singleline);
                    if (m2.Success)
                    {
                        var href = m2.Groups[1].Value;
                        Match isContainHttp = Regex.Match(href, @"http(.*)", RegexOptions.Singleline);

                        if (isContainHttp.Success)
                        {
                             _uri = new Uri(path);
                             _path = _uri.Scheme + "://" + _uri.Authority; 
                             pattern = string.Format(@"{0}(.*)", _path);

                            Match isinternal = Regex.Match(href, pattern, RegexOptions.Singleline);
                            if (!isinternal.Success)
                            {
                                _externalLinks.Add(href);
                            }
                        }

                    }

                }


                ExternalLinks = _externalLinks.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}