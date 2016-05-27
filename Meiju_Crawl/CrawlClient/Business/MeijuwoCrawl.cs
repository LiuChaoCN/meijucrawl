using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CsQuery;
using GalaSoft.MvvmLight.Threading;

namespace CrawlClient
{
    public class MeijuwoCrawl
    {
        #region 私有变量
        private Crawl_ViewModel model;
        private HttpHelper httpHelper = new HttpHelper();
        private HttpItem httpItem = new HttpItem();
        private string host = "";
        private HttpResult episode_result = null;
        private List<string> verify_words;
        private string title = "";
        #endregion

        public MeijuwoCrawl(Crawl_ViewModel viewModel)
        {
            model = viewModel;
            verify_words = model.VerifyWord.Split(',').ToList();
        }

        /// <summary>
        /// 开始抓取
        /// </summary>
        public void Start()
        {
            host = new Uri(model.Url).Host;
            model.EnableControls = false;
            //初始化抓取
            model.Status = "初始化……";
            httpItem.URL = host;
            //初始化成功
            AddProcess(20);

            Episode_List_Crawl(model.Url);

            model.EnableControls = true;
            SetProcess(100);
            model.Status = "抓取完成";
        }

        #region 私有方法
        #region 抓取方法
        /// <summary>
        /// 抓取列表页对象
        /// </summary>
        /// <param name="url"></param>
        private void Episode_List_Crawl(string url)
        {
            httpItem.URL = url;
            var episode_list_result = httpHelper.GetHtml(httpItem);
            if (episode_list_result.StatusCode == HttpStatusCode.OK)
            {
                var episode_list_dom = CQ.CreateDocument(episode_list_result.Html);
                title = episode_list_dom[".article-title"].FirstElement().InnerText.HtmlDecode();
                var episode_list = episode_list_dom[".episode_list a"];
                //抓取集数页面
                for (int i = 0; i < episode_list.Length; i++)
                {
                    var success = false; //是否抓取成功
                    WriteLog(string.Format("正在抓取 {0} 第1页", episode_list[i].InnerText.HtmlDecode()));
                    success = Episode_Crawl(host + episode_list[i].GetAttribute("href"));
                    if (!success)
                    {
                        WriteLog(string.Format("{0} 第1页 未找到有效资源", episode_list[i].InnerText.HtmlDecode()));
                        var episode_dom = CQ.CreateDocument(episode_result.Html);
                        var pages_links = episode_dom["#yw1>li>a"];
                        if (pages_links.Length > 0)
                        {
                            //只要带分页，那就会多出首页、上一页、1、下一页、末页的分页按钮
                            //所以在循环时不抓取这些重复页面
                            for (int j = 3; j < pages_links.Length - 2; j++)
                            {
                                WriteLog(string.Format("正在抓取 {0} 第{1}页",
                                    episode_list[i].InnerText.HtmlDecode(),
                                    pages_links[j].InnerText.HtmlDecode()));
                                var page_link = pages_links[j].GetAttribute("href");
                                success = Episode_Crawl(host + page_link);
                                if (!success)
                                {
                                    WriteLog(string.Format("{0} 第{1}页 未找到有效资源", episode_list[i].InnerText.HtmlDecode(), pages_links[j].InnerText.HtmlDecode()));
                                }
                            }
                        }
                    }
                    else
                    {
                        WriteLog(string.Format("{0} 第1页抓取成功", episode_list[i].InnerText.HtmlDecode()));
                    }
                    AddProcess(70 / episode_list.Length);
                }
            }
        }

        /// <summary>
        /// 抓取下载链接
        /// </summary>
        /// <param name="url"></param>
        private bool Episode_Crawl(string url)
        {
            var success = false; //是否存在字幕链接
            httpItem.URL = url;
            episode_result = httpHelper.GetHtml(httpItem);
            if (episode_result.StatusCode == HttpStatusCode.OK)
            {
                var episode_dom = CQ.CreateDocument(episode_result.Html);
                var link_doms = episode_dom[".resource-more-link a"];
                success = AnalysisLinks(link_doms);
                if (!success)
                {
                    //WriteLog(string.Format("{0}无带字幕资源", episode_list[i].InnerText.HtmlDecode()));
                }
            }
            return success;
        }

        private bool AnalysisLinks(CQ link_doms)
        {
            for (int j = 0; j < link_doms.Length; j++)
            {
                var link = link_doms[j].GetAttribute("href");
                if (VerifyLink(link))
                {
                    SetResult(link);
                    return true;
                }
            }
            return false;
        }

        private bool VerifyLink(string link)
        {
            string temp = link.UrlDecode();
            for (int i = 0; i < verify_words.Count; i++)
            {
                if (temp.Contains(verify_words[i]))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        private void SetResult(string link)
        {
            DispatcherHelper.RunAsync(() =>
            {
                model.Result += link.UrlDecode() + "\r\n";
            });
        }

        private void WriteLog(string str)
        {
            DispatcherHelper.RunAsync(() =>
            {
                model.Log.Add(title + " " + str);
            });
        }

        private void AddProcess(int value)
        {
            model.Process += value;
            model.Status = string.Format("抓取中……{0}%", model.Process);
        }

        private void SetProcess(int value)
        {
            model.Process = value;
            model.Status = string.Format("抓取中……{0}%", model.Process);
        }
        #endregion
    }
}
