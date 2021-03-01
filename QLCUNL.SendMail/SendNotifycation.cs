using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.ServiceProcess;
using System.Text;
using XMedia;

namespace Timer
{
    public partial class SendNotifycation : ServiceBase
    {
        private System.Timers.Timer _timer = new System.Timers.Timer();
        private System.Timers.Timer _timerAutoSend = new System.Timers.Timer();
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SendNotifycation).Name);

        public SendNotifycation()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _logger.Error("START Check_ProKafka2FB");
                _timer.Elapsed += _timer_Elapsed;
                _timer.AutoReset = false;
                _timer.Start();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        protected override void OnStop()
        {
            _logger.Error("STOP Check_ProKafka2FB");
            _logger.Error("STOP reload email_ProKafka2FB");
        }

        //object sender, System.Timers.ElapsedEventArgs ee
        public void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs ee)
        {
            try
            {
                _logger.Error("START 1");
                string uri = "https://mail.x2convert.com/ajax/CheckStatus.ashx";
                var token = XUtil.GetToken();
                //string param = $"token={token}&obj=";
                List<string> id_mail = new List<string>();
                var data = QLCUNL.BL.LogSendMailBL.TimMailMoiByStt("1");
                foreach (var item in data)
                {
                    id_mail.Add(item.Id);
                }
                if (id_mail.Count > 0)
                {
                    var bute = Encoding.UTF8.GetBytes($"token={token}&obj={JsonConvert.SerializeObject(id_mail)}");
                    _logger.Error(JsonConvert.SerializeObject(id_mail));

                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        var res = wc.UploadData(uri, bute);

                        var re = Encoding.Default.GetString(res);
                        _logger.Error(re);
                        var k = JToken.Parse(JsonConvert.DeserializeObject(re)?.ToString());
                        var data1 = JsonConvert.DeserializeObject(k["Data"].ToString());
                        var list = JsonConvert.DeserializeObject<List<LogSendMail>>(data1.ToString());

                        foreach (var item in list)
                        {
                            QLCUNL.BL.LogSendMailBL.UpdateStatus(item.Id, item.Status);
                        }
                    }
                }

                _logger.Error("START 3");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            finally
            {
                var set_time = System.Configuration.ConfigurationManager.AppSettings["TimeInterval"];
                
                _timer.Interval = TimeSpan.FromMinutes(10).TotalMilliseconds;
                _timer.Interval = int.Parse(set_time);
            }
        }
    }
}