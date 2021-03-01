using Newtonsoft.Json;
using QLCUNL.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLCUNL.API
{
    public class DataResponsePaging : DataResponse
    {
        public long total { get; set; }
    }
   
    public class DataResponse
    {
        private object _data;
        public bool success { get; set; }
        public object data
        {
            get { return _data; }
            set
            {
                if (value!=null && value.GetType() == typeof(string))
                {                    
                    try
                    {
                        _data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Convert.ToString(value));
                    }
                    catch (Exception)
                    {
                        _data = value;
                    }
                }
                else
                {
                    var settings = new JsonSerializerSettings() { ContractResolver = new NullToEmptyStringResolver() };

                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(value, settings);
                    
                    _data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
                }
            }
        }
        public string msg { get; set; }
    }
    public class DataResponseExt : DataResponse
    {
        public object value { get; set; }
    }
}
