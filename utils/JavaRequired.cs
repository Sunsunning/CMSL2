using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
namespace CMSL.utils
{
    class JavaRequired
    {
        public enum JavaVersion
        {
            Java8,
            Java16,
            Java17,
            Exception
        }
        private static List<string> paper_version = new List<string>();
        private static List<string> spigot_version = new List<string>();
        public static async Task<JavaVersion> Start(string version, Server.Side side)
        {
            await Initialize();
            if (version == null)
            {
                return JavaVersion.Exception;
            }
            if (side.Equals(Server.Side.Spigot))
            {
                return DetermineJavaVersion(version, spigot_version);
            }
            else
            {
                return DetermineJavaVersion(version, paper_version);
            }
        }
        private static JavaVersion DetermineJavaVersion(string version, List<string> list)
        {
            int currentIndex = list.IndexOf(version);
            int indexJava8 = list.IndexOf("1.12.2");
            int indexJava16 = list.IndexOf("1.17.1");
            int indexJava17 = list.IndexOf("1.18");
            if (indexJava8 == -1 || indexJava16 == -1 || indexJava17 == -1 || currentIndex == -1)
            {
                return JavaVersion.Exception;
            }
            JavaVersion determinedJavaVersion = currentIndex == -1 ? JavaVersion.Exception : currentIndex <= indexJava17 ? JavaVersion.Java17 : currentIndex >= indexJava16 && currentIndex < indexJava8 ? JavaVersion.Java16 : JavaVersion.Java8;
            return determinedJavaVersion;
        }
        public static async Task Initialize()
        {
            try
            {
                string json = await FetchJsonFromUrlAsync("https://gitee.com/chuan_yu/cmsl/raw/master/servers.json");
                JObject jsonObject = JObject.Parse(json);
                JArray versionsArray;
                // spigot
                versionsArray = (JArray)jsonObject["release"]["spigot"];
                spigot_version = versionsArray.ToObject<List<string>>();
                // paper
                versionsArray = (JArray)jsonObject["release"]["paper"]["version"];
                paper_version = versionsArray.ToObject<List<string>>();
            }
            catch (JsonException jsonEx)
            {
                throw new Exception("JSON 解析失败", jsonEx);
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception("无法从指定的 URL 获取数据", httpEx);
            }
        }

        private static async Task<string> FetchJsonFromUrlAsync(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    return await client.GetStringAsync(url);
                }
                catch (HttpRequestException e)
                {
                    throw new Exception("无法从指定的 URL 获取数据。", e);
                }
            }
        }

    }
}
