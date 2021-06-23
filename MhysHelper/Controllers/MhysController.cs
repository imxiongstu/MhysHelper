using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using MhysHelper.Models;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace MhysHelper.Controllers
{
    public class MhysController : ApiController
    {
        /*
         By：熊沐风同学
         QQ：1648545292
         博客：www.zejang.cn
         写个锤子，不写了，不更新了，难受，不搞了，重新看网课，后面两个月都没学习了，重新看网课！！！！
        */

        private static string url_api = "http://39.105.204.53/";
        private static string url_api2 = "https://www.51moot.net/";
        private static string sign = "DBF067ACBE5A01AB5BA976D5EB0990D4";
        private static CookieContainer myCookieContainer = new CookieContainer();

        /// <summary>
        /// H5PC协议，获取Cookie
        /// </summary>
        /// <param name="login_name"></param>
        /// <param name="login_pass"></param>
        [HttpGet]
        public void LoginInfoPC(string login_name, string login_pass)
        {
            GetUrlContentPost(url_api2 + "main/login_validate?login_name=" + login_name + "&login_pass=" + login_pass + "&auto_login=true", out myCookieContainer);
        }

        /// <summary>
        /// 课程列表查询
        /// </summary>
        /// <param name="login_name"></param>
        /// <param name="login_pass"></param>
        /// <returns></returns>
        [HttpGet]
        public UserInfo CourseQuery(string login_name, string login_pass)
        {
            LoginInfoPC(login_name, login_pass);//顺便把PC协议的COOKIE也留下来

            UserInfo myUserInfo = new UserInfo();
            JObject LoginInfo = JObject.Parse(GetUrlContent(url_api + "api//v1/web_user?" + "login_name=" + login_name + "&login_pass=" + login_pass + "&sign=" + sign));
            string userId = LoginInfo["data"]["id"].ToString();
            string courseIdList = "";
            for (int i = 0; i < LoginInfo["data"]["course_id_list"].ToArray().Length; i++)
            {
                courseIdList += LoginInfo["data"]["course_id_list"].ToArray()[i].ToString();
                if (i < LoginInfo["data"]["course_id_list"].ToArray().Length - 1)
                {
                    courseIdList += ",";
                }
            }
            courseIdList = System.Web.HttpUtility.UrlEncode(courseIdList);
            JObject courseInfoData = JObject.Parse(GetUrlContent(url_api + "api//v1/course_info?&id_list=" + courseIdList + "&user_id=" + userId + "&sign=" + sign));
            for (int i = 0; i < courseInfoData["data"].Count(); i++)
            {
                myUserInfo.CourseInfoList.Add(new CourseInfo
                {
                    Id = courseInfoData["data"][i]["id"].ToString(),
                    Name = courseInfoData["data"][i]["name"].ToString()
                });
            }
            myUserInfo.UserId = LoginInfo["data"]["id"].ToString();
            myUserInfo.UserName = LoginInfo["data"]["name"].ToString();

            //添加到数据库，用于统计
            #region
            try
            {
                var db = new Common.DBContext().Instance;
                var getall = db.Queryable<MhysUserInfo>().ToList();
                if (!getall.Any(x => x.LoginName == login_name))
                {
                    db.Insertable(new MhysUserInfo()
                    {
                        LoginName = login_name,
                        UserName = myUserInfo.UserName,
                        LoginCount = 1
                    }).ExecuteCommand();
                }
                else
                {
                    int count = getall.Where(x => x.LoginName == login_name).Select(f => f.LoginCount).ToList()[0];
                    db.Updateable(new MhysUserInfo() { LoginName = login_name, UserName = myUserInfo.UserName, LoginCount = count + 1 }).Where(f => f.LoginName == login_name).ExecuteCommand();
                }
            }
            catch (Exception)
            {

            }


            #endregion

            return myUserInfo;
        }

        /// <summary>
        /// 课程章节列表查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public JObject ChapterQuery(string id)
        {
            JObject ChapterInfoData = JObject.Parse(GetUrlContent(url_api + "api//v1/course_dirctory?course_id=" + id + "&sign=" + sign));
            return ChapterInfoData;
        }

        /// <summary>
        /// 获取视频详细信息
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetVideoInfo(string courseId, string chapterId, string uid)
        {
            JObject ChapterInfoData = ChapterQuery(courseId);
            int index = 0;
            for (int i = 0; i < ChapterInfoData["data"].Count(); i++)
            {
                if (ChapterInfoData["data"][i]["id"].ToString() == chapterId.ToString())
                {
                    index = i;
                }
            }

            JObject VideoJobject = new JObject();
            JArray j = new JArray();
            foreach (var item in ChapterInfoData["data"][index]["child"])
            {
                JObject myVideoInfodata = JObject.Parse(GetUrlContent(url_api + "api/v1/course_dirctory?id=" + item["id"] + "&is_dirctory=true&sign=" + sign));
                string pcdata = GetUrlContent("http://www.51moot.net/server_hall_2/server_hall_2/video_play?dir_id=" + myVideoInfodata["data"]["id"], myCookieContainer);
                Match vid2 = Regex.Match(pcdata, @"(?<='vid': ')([\s\S]*?)(?=',\n'ts')");
                Match ts2 = Regex.Match(pcdata, @"(?<='ts': ')([\s\S]*?)(?=',\n'sign')");
                Match sign2 = Regex.Match(pcdata, @"(?<='sign': ')([\s\S]*?)(?=',\n'session_id')");
                Match session_id2 = Regex.Match(pcdata, @"(?<='session_id': ')([\s\S]*?)(?=',\n'playsafe')");
                Match playsafe2 = Regex.Match(pcdata, @"(?<='playsafe': ')([\s\S]*?)(?=',\n'speed')");
                j.Add(new JObject{
                   {"id", myVideoInfodata["data"]["id"] },
                   {"video_id", myVideoInfodata["data"]["video_id"] },
                   {"ts", myVideoInfodata["data"]["ts"] },
                   {"sign", myVideoInfodata },
                   {"ts2", ts2.ToString() },
                   {"sign2", sign2.ToString() },
                   {"session_id2", session_id2.ToString() },
                   {"vid2", vid2.ToString() },
                   {"playsafe2", playsafe2.ToString() },
                   {"duration", item["video_duration"] }
                });
            }
            VideoJobject.Add("uid", uid);
            VideoJobject.Add("data", j);
            return VideoJobject;
        }

        public JObject GetExamAnswer(string examid)
        {
            JObject returnJson = new JObject();
            JArray returnJarry = new JArray();
            JObject mhysJson = JObject.Parse(GetUrlContent(url_api + "api/v1/example_subject?example_id=" + examid + "&is_random=false&sign=" + sign));
            for (int i = 0; i < mhysJson["data"].Count(); i++)
            {
                returnJarry.Add(new JObject()
                {
                    { "Title",mhysJson["data"][i]["title"]},
                    { "item_list",mhysJson["data"][i]["item_list"]},
                    { "answer_list",mhysJson["data"][i]["answer_list"]}
                });
            }
            returnJson.Add("Data", returnJarry);
            return returnJson;
        }

        /// <summary>
        /// 获取网页内容Get
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlContent(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "Get";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
        public static string GetUrlContent(string url, CookieContainer myCookieContainer)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            request.Method = "Get";
            request.CookieContainer = myCookieContainer;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 获取网页内容Post
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrlContentPost(string url, out CookieContainer myCookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            request.Method = "Post";
            request.ContentLength = 0;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                myCookie = request.CookieContainer;
                return reader.ReadToEnd();
            }
        }


        /// <summary>
        /// 只是为了解决跨域问题特此设置的方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string HttpRequestCors([FromBody] string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "Get";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }

    }
}