using System;
using HtmlAgilityPack;
using System.Net;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LASHON
{
    class GetData
    {
        private static CookieContainer _cookieContainer;

        public static string getEnglishToHebrew(string heb)
        {
            if (string.IsNullOrEmpty(heb) || string.IsNullOrWhiteSpace(heb)) return null;
            char startingChar = heb[0];

            var data = new NameValueCollection
            {
                { "l" , startingChar + ""},
                { "numofcol", "2" },
                { "tabindex" , "-1"},
            };

            var url = GetStringWithParamBuilder("http://hebrew-academy.huji.ac.il/milim/_layouts/AcademApps/HowToSayInHebrew/GetLetterWords.aspx", data);

            var ans = "";
            using (var respons = Request(url,
                "GET", "", "hebrew-academy.huji.ac.il", "text/html;application/xml;application/json", null, false, false))
            {

                var resultStream = respons.GetResponseStream();
                Encoding enc = System.Text.Encoding.UTF8;

                // Pipe the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(resultStream, enc);

                HtmlDocument doc = new HtmlDocument();
                doc.Load(readStream);

                doc.Save("a.html", Encoding.Unicode);

                foreach (var word in doc.DocumentNode.SelectNodes("//div[@class='divCell']"))
                {
                    var node = word.SelectSingleNode("./descendant::span[1]");
                    if (node != null && removeNikud(node.InnerText).Contains(heb))
                    {
                        var a = word.SelectSingleNode("./descendant::span[3]").InnerText;
                        var b = word.SelectSingleNode(".//div[@class='explanation']")?.InnerText;
                        ans = a + "\n" + b;
                        break;
                    }
                }

                
                readStream.Close();
            }

            return ans.Equals("") ? "Not Found :(" : ans.Replace("&nbsp;","").Replace("&quot;","\"");
        }


        public static System.Collections.Generic.List<string[]> getByLetter(char letter)
        {
            var data = new NameValueCollection
            {
                { "l" , letter + ""},
                { "numofcol", "2" },
                { "tabindex" , "-1"},
            };

            var url = GetStringWithParamBuilder("http://hebrew-academy.huji.ac.il/milim/_layouts/AcademApps/HowToSayInHebrew/GetLetterWords.aspx", data);

            var list = new System.Collections.Generic.List<string[]>();
            var ans = "";
            using (var respons = Request(url,
                "GET", "", "hebrew-academy.huji.ac.il", "text/html;application/xml;application/json", null, false, false))
            {

                var resultStream = respons.GetResponseStream();
                Encoding enc = System.Text.Encoding.UTF8;

                // Pipe the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(resultStream, enc);

                HtmlDocument doc = new HtmlDocument();
                doc.Load(readStream);

                doc.Save("RequestLetter.html", Encoding.Unicode);

                foreach (var word in doc.DocumentNode.SelectNodes("//div[@class='divCell']"))
                {
                    var node = word.SelectSingleNode("./descendant::span[1]");
                    if (node != null)
                    {
                        var a = word.SelectSingleNode("./descendant::span[3]").InnerText;
                        list.Add(new []{ node.InnerText, a });
                    }
                }


                readStream.Close();
            }

            return list;
        }


    public static string[] random()
        {
            Random r = new Random();

            var data = new NameValueCollection
            {
                { "l" , ((char) (r.Next(0,22) + 'א')) + ""},
                { "numofcol", "2" },
                { "tabindex" , "-1"},
            };

            var stringToBuild = "";

            var items = data.AllKeys.SelectMany(data.GetValues, (k, v) => new { key = k, value = v });
            foreach (var item in items)
                stringToBuild += string.Format("{0} {1} \n", item.key, item.value);

            Debug.WriteLine("Param : "+ stringToBuild);

            var url = GetStringWithParamBuilder("http://hebrew-academy.huji.ac.il/milim/_layouts/AcademApps/HowToSayInHebrew/GetLetterWords.aspx", data);

            var ans = "";
            var wordSelected = "";
            using (var respons = Request(url,
                "GET", "", "hebrew-academy.huji.ac.il", "text/html;application/xml;application/json", null, false, false))
            {

                var resultStream = respons.GetResponseStream();
                Encoding enc = System.Text.Encoding.UTF8;

                // Pipe the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(resultStream, enc);

                HtmlDocument doc = new HtmlDocument();
                doc.Load(readStream);

                doc.Save("requestDebug.html", Encoding.Unicode);

                var wordList = doc.DocumentNode.SelectNodes("//div[@class='divCell']");
                int selectedWord = r.Next(0, wordList.Count);
                int index = 0;

                foreach (var word in wordList)
                {
                    if(index == selectedWord)
                    {
                        var node = word.SelectSingleNode("./descendant::span[1]");
                        if (node != null)
                        {
                            wordSelected = node.InnerText;
                            var a = word.SelectSingleNode("./descendant::span[3]").InnerText;
                            var b = word.SelectSingleNode(".//div[@class='explanation']")?.InnerText;
                            ans = a + "\n" + b;
                            break;
                        }
                    }
                    index++;
                }

                readStream.Close();
            }

            string[] toReturn = { wordSelected, ans.Replace("&nbsp;", "").Replace("&quot;", "\"") };
            return toReturn;
        }

        private static string removeNikud(string inHeb)
        {
            string ans = "";
            foreach (var c in inHeb)
            {
                if (c >= '\u0591' && c <= '\u05C7')
                {
                    Debug.WriteLine("Nikud {0}", c);
                }
                else
                    ans += c;
            }
            return ans;
        }


        /// <summary>
        /// Returns Get string with given Params 
        /// </summary>
        /// <param name="url">String to build into</param>
        /// <param name="ParamToBuild">Parameters to add to the string </param>
        /// <returns>Builded url</returns>
        private static string GetStringWithParamBuilder(string url, NameValueCollection ParamToBuild)
        {
            url += "?";
            foreach (string key in ParamToBuild)
            {
                url += "&" + key + "=" + ParamToBuild[key];
            }
            return url;
        }

        /// <summary>
        /// The formal method to use HTTPWebRequests.
        /// I got the headers out of Fiddler. I highly recommend this tool to follow HTTPWebRequests.
        /// Some Headers are unnecessary but i will use them for a natural-looking HTTPWebRequests.
        /// </summary>
        /// <param name="inpUrl">URL of the HTTPWebRequest.</param>
        /// <param name="inpMethod">Method of the HTTPWebRequest.
        /// Example: "POST", "GET", "PUT",...</param>
        /// <param name="inpReferer">The Referer of the URL. Sometimes its needed.</param>
        /// <param name="inpHost">The Host of the request.</param>
        /// <param name="inpAccept">Accept format of the request.</param>
        /// <param name="inpNvc">Data which will be wrote if you will do a "Post".</param>
        /// <param name="inpXml">If Ajax, then this is required.</param>
        /// <param name="autoRedirect">Auto-Redirect.</param>
        /// <returns>A HttpWebResponse of the request.</returns>
        private static HttpWebResponse Request(string inpUrl, string inpMethod, string inpReferer, string inpHost, string inpAccept, System.Collections.Specialized.NameValueCollection inpNvc, bool inpXml, bool autoRedirect)
        {
            var request = (HttpWebRequest)WebRequest.Create(inpUrl);
            request.Accept = inpAccept;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
            request.Timeout = 20000;
            request.Headers.Add("Accept-Language", "en");
            request.AllowAutoRedirect = autoRedirect;
            request.CookieContainer = _cookieContainer;
            request.Method = inpMethod;
            //Volatile variables

            if (inpHost != "")
            {
                request.Host = inpHost;
            }

            if (inpReferer != "")
            {
                request.Referer = inpReferer;
            }
            if (inpXml)
            {
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                request.Headers.Add("X-Prototype-Version", "1.7");
                request.Headers.Add("Cache-Control", "no-cache");
                request.Headers.Add("Pragma", "no-cache");
            }

            if (inpMethod != "POST") return request.GetResponse() as HttpWebResponse;
            var dataString = (inpNvc == null ? null : string.Join("&", Array.ConvertAll(inpNvc.AllKeys, key =>
                $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(inpNvc[key])}"
                )));
            if (dataString == null) return request.GetResponse() as HttpWebResponse;
            var dataBytes = Encoding.UTF8.GetBytes(dataString);
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.ContentLength = dataBytes.Length;
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(dataBytes, 0, dataBytes.Length);
            }
            return request.GetResponse() as HttpWebResponse;
        }
    }
}
