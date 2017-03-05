using SEOAnalyser.Models;
using SEOAnalyser.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SEOAnalyser.Helpers;

namespace SEOAnalyser.Controllers
{
    [HandleError(View ="Error")]
    public class HomeController : Controller
    {
       
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        // 
        /// </summary>
        /// <param name="selectionType">url or text</param>
        /// <param name="urlvalue">url if selection is url type</param>
        /// <param name="textvalue">url if selection is text type</param
        /// <param name="removestopwords">Whether stop word is removed</param
        /// <param name="calcexternallinks">Whether external link is calculated</param
        /// <param name="calcmetatagword">Whether meta tag words are calclated</param
        /// <returns>view</returns>
        [HttpPost]

        public async Task<ActionResult> Index(string selectionType,
            string urlvalue, string textvalue, bool removestopwords = false,
            bool calcexternallinks = false, bool calcmetatagword = false)
        {

            try
            {
                TextSEOAnalyser _textseoanalyser = null;
                HtmlAnayser _htmlAnalyser = null;
                bool isTextSelection, isHtmlSeleection = false;
                SEOAnalyserViewModel model = new SEOAnalyserViewModel();

                isTextSelection = (selectionType == "text") ? true : false;
                isHtmlSeleection = (selectionType == "url") ? true : false;

                if (!string.IsNullOrEmpty(selectionType) && selectionType == "text")
                {
                    _textseoanalyser = new TextSEOAnalyser();
                    if (string.IsNullOrEmpty(textvalue))
                    {
                        ModelState.AddModelError("invalidText", "Invalid Text");
                    }

                    _textseoanalyser.Text = textvalue;

                }
                else {
                    _htmlAnalyser = new HtmlAnayser();
                    if (!urlvalue.IsValidUrl())
                    {
                        ModelState.AddModelError("invalidUrl", "Invalid Url(Please provide full qualified url ex:https://randomtutes.com/ )");
                    }
                }

                if (ModelState.IsValid)
                {
                    if (isHtmlSeleection)
                    {
                        
                        await _htmlAnalyser.GetHtmlFromUrl(urlvalue);
                        _htmlAnalyser.path = urlvalue;
                        if (removestopwords) _htmlAnalyser.RemoveStopWords();
                        _htmlAnalyser.CalculateOccurencesOfWords();
                        if (calcmetatagword)
                        {
                            _htmlAnalyser.GetMetatagWords();
                            _htmlAnalyser.CalculateMetaWordsCount();
                        }
                        if (calcexternallinks) _htmlAnalyser.CalclulateExternelLinks();

                        model.WordsCount = _htmlAnalyser.WordsCount;
                        model.MetaWordsCount = _htmlAnalyser.MetaWordsCount;
                        model.ExternalLinksCount = _htmlAnalyser.ExternalLinks;

                    }
                    if (isTextSelection)
                    {
                        if (removestopwords) _textseoanalyser.RemoveStopWords();
                        _textseoanalyser.CalculateOccurencesOfWords();

                        model.WordsCount = _textseoanalyser.WordsCount;
                    }
                }
                else {
                    return View();
                }

                model.RemoveStopWords = removestopwords;
                model.CalcMetaTags = calcmetatagword;
                model.CalcExternalLinks = calcexternallinks;

                

                Session.Add("model", model);

                return View("SEOAnalyser", model);

            }
            catch (Exception ex)
            {
                if(ex.Message != null && ex.Message.Contains("$"))
                TempData["error"] = ex.Message;
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public ActionResult SortData(SEOAnalyserViewModel values)
        {

            try
            {
                SEOAnalyserViewModel model = (SEOAnalyserViewModel)Session["model"];
                

                if (model.IsKeyOrderASCT1 != values.IsKeyOrderASCT1)
                {

                    model.WordsCount = (values.IsKeyOrderASCT1) ? model.WordsCount
                           .OrderBy(m => m.Key)
                           .ToDictionary(x => x.Key, x => x.Value)
                           : model.WordsCount
                           .OrderByDescending(m => m.Key)
                           .ToDictionary(x => x.Key, x => x.Value)
                           ;
                }

                if (model.IsValueOrderASCT1 != values.IsValueOrderASCT1)
                {
                    model.WordsCount = (values.IsValueOrderASCT1) ? model.WordsCount.OrderBy(m => m.Value).
                             ToDictionary(x => x.Key, x => x.Value)
                             : model.WordsCount.OrderByDescending(m => m.Value).
                             ToDictionary(x => x.Key, x => x.Value);
                }

                if (model.IsKeyOrderASCT2 != values.IsKeyOrderASCT2)
                {

                    model.MetaWordsCount = (values.IsKeyOrderASCT2) ? model.MetaWordsCount
                           .OrderBy(m => m.Key)
                           .ToDictionary(x => x.Key, x => x.Value)
                           : model.MetaWordsCount
                           .OrderByDescending(m => m.Key)
                           .ToDictionary(x => x.Key, x => x.Value)
                           ;
                }
                if (model.IsValueOrderASCT2 != values.IsValueOrderASCT2)
                {
                    model.MetaWordsCount = (values.IsValueOrderASCT2) ? model.MetaWordsCount.OrderBy(m => m.Value).
                             ToDictionary(x => x.Key, x => x.Value)
                             : model.MetaWordsCount.OrderByDescending(m => m.Value).
                             ToDictionary(x => x.Key, x => x.Value);
                }

                if (model.IsKeyOrderASCT3 != values.IsKeyOrderASCT3)
                {

                    model.ExternalLinksCount = (values.IsKeyOrderASCT3) ? model.ExternalLinksCount
                           .OrderBy(m => m.Key)
                           .ToDictionary(x => x.Key, x => x.Value)
                           : model.ExternalLinksCount
                           .OrderByDescending(m => m.Key)
                           .ToDictionary(x => x.Key, x => x.Value)
                           ;
                }
                if (model.IsValueOrderASCT3 != values.IsValueOrderASCT3)
                {
                    model.ExternalLinksCount = (values.IsValueOrderASCT3) ? model.ExternalLinksCount.OrderBy(m => m.Value).
                             ToDictionary(x => x.Key, x => x.Value)
                             : model.ExternalLinksCount.OrderByDescending(m => m.Value).
                             ToDictionary(x => x.Key, x => x.Value);
                }

                model.IsKeyOrderASCT1 = values.IsKeyOrderASCT1;
                model.IsValueOrderASCT1 = values.IsValueOrderASCT1;
                model.IsKeyOrderASCT2 = values.IsKeyOrderASCT2;
                model.IsValueOrderASCT2 = values.IsValueOrderASCT2;
                model.IsKeyOrderASCT3 = values.IsKeyOrderASCT3;
                model.IsValueOrderASCT3 = values.IsValueOrderASCT3;
               
                Session["model"] = model;

                return View("SEOAnalyser", model);
            }
            catch (Exception)
            {

                return RedirectToAction("Error");
            }
        }

        public ActionResult Error()
        {
            string message = string.Empty; 
            if (TempData["error"] != null)
            {

                 message = Convert.ToString(TempData["error"]);
            }
            ViewBag.Message = "Ooops.. Somthing went wrong <br/>" + message.Replace('$',' ');

            return View();
        }
    }
}