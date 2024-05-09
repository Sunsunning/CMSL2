using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CMSL.utils
{
    class CSettings
    {
        private ApplicationDataContainer _settings;
        public Dictionary<string,object> Original = new Dictionary<string, object>
            {
                { "notices", "Toast(Win10+)" },
                { "SaveDirectory", @"D:\CMSL" },
                {"DownloadSourceAPI","https://gitee.com/chuan_yu/cmsl" },
                {"DownloadTaskCount",64 }
            };
        

        public CSettings()
        {
            _settings = ApplicationData.Current.LocalSettings;
        }

        // 泛型方法用于设置值
        public void SetValue<T>(string key, T value)
        {
            _settings.Values[key] = value;
        }

        // 泛型方法用于获取值，返回null如果键不存在或类型不匹配
        public T GetValue<T>(string key)
        {
            if (_settings.Values.ContainsKey(key) && _settings.Values[key] is T)
            {
                return (T)_settings.Values[key];
            }
            return default(T);
        }

        // 检查键是否存在
        public bool ContainsKey(string key)
        {
            return _settings.Values.ContainsKey(key);
        }

        public bool ToastSetting()
        {
            if (GetValue<string>("notices") != null && GetValue<string>("notices").Equals("关闭"))
            {
                return false;
            }

            return true;


        }
    }
}
