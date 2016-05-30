using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CsQuery;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CrawlClient
{
    public class Crawl_ViewModel : ViewModelBase
    {
        #region Property
        private string _Url = "";

        public string Url
        {
            set
            {
                _Url = value;
                RaisePropertyChanged();
            }
            get { return _Url; }
        }
        private string _Status = "";

        public string Status
        {
            set
            {
                _Status = value;
                RaisePropertyChanged();
            }
            get { return _Status; }
        }

        private string _Result = "";

        public string Result
        {
            set
            {
                _Result = value;
                RaisePropertyChanged();
            }
            get { return _Result; }
        }

        private ObservableCollection<string> _Log = new ObservableCollection<string>();

        public ObservableCollection<string> Log
        {
            set
            {
                _Log = value;
                RaisePropertyChanged();
            }
            get { return _Log; }
        }

        private int _Process = 0;

        public int Process
        {
            set
            {
                _Process = value;
                RaisePropertyChanged();
            }
            get { return _Process; }
        }

        private bool _EnableControls = true;

        public bool EnableControls
        {
            set
            {
                _EnableControls = value;
                RaisePropertyChanged();
            }
            get { return _EnableControls; }
        }

        private string _VerifyWord = "";

        public string VerifyWord
        {
            set
            {
                _VerifyWord = value;
                RaisePropertyChanged();
            }
            get { return _VerifyWord; }
        }
        #endregion

        #region Commond

        public RelayCommand CrawlCommand { get; private set; }

        public Crawl_ViewModel()
        {
            if (!IsInDesignMode)
            {
                CrawlCommand = new RelayCommand(() =>
                {
                    if (EnableControls)
                    {
                        #region 重置字段
                        Log.Clear();
                        Result = "";
                        Status = "";
                        Process = 0;
                        #endregion
                        if (string.IsNullOrEmpty(Url))
                        {
                            MessageBox.Show("请先输入抓取地址");
                        }
                        else
                        {
                            Task.Factory.StartNew(() =>
                            {
                                var meijuwoCrawl = new MeijuwoCrawl(this);
                                meijuwoCrawl.Start();
                            });
                        }
                    }
                });
            }
        }
        #endregion
    }
}
