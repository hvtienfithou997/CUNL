using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QLCUNL.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.ServiceProcess;
using System.Text;
using XMedia;

namespace Timer
{
    public partial class CheckStatusEmail : ServiceBase
    {
        private System.Timers.Timer _timer = new System.Timers.Timer();
        private System.Timers.Timer _timerAutoSend = new System.Timers.Timer();
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CheckStatusEmail).Name);

        public CheckStatusEmail()
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
        }

        //object sender, System.Timers.ElapsedEventArgs ee
        public void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs ee)
        {
            try
            {
                string uri = System.Configuration.ConfigurationManager.AppSettings["MailAPI"];
                var token = XUtil.GetToken();
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
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback =
           new RemoteCertificateValidationCallback(
                delegate
                { return true; }
            );
                    using (var wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        var res = wc.UploadData(uri, bute);

                        var re = Encoding.Default.GetString(res);
                        var k = JToken.Parse(JsonConvert.DeserializeObject(re)?.ToString());
                        var data1 = JsonConvert.DeserializeObject(k["Data"].ToString());
                        var list = JsonConvert.DeserializeObject<List<LogSendMail>>(data1.ToString());

                        foreach (var item in list)
                        {
                            QLCUNL.BL.LogSendMailBL.UpdateStatus(item.Id, item.Status);
                            _logger.Error("Updated " + item.Email);
                        }
                    }
                }
                else
                {
                    _logger.Error("No emails were found");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                var set_time = System.Configuration.ConfigurationManager.AppSettings["TimeIntervalMinutes"];
                _timer.Interval = TimeSpan.FromMinutes(int.Parse(set_time)).TotalMilliseconds;
            }
        }
    }
}