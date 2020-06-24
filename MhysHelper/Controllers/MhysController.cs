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

namespace MhysHelper.Controllers
{
    public class MhysController : ApiController
    {
        /*
         By：熊沐风同学
         QQ：1648545292
         博客：www.zejang.cn
        */

        private static string url_api = "http://ddossiquanjiameiyoudaode123789.51moot.cn/";
        private static string sign = "DBF067ACBE5A01AB5BA976D5EB0990D4";

        /// <summary>
        /// 课程列表查询
        /// </summary>
        /// <param name="login_name"></param>
        /// <param name="login_pass"></param>
        /// <returns></returns>
        [HttpGet]
        public UserInfo CourseQuery(string login_name, string login_pass)
        {
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

                j.Add(new JObject{
                   {"id", myVideoInfodata["data"]["id"] },
                   {"video_id", myVideoInfodata["data"]["video_id"] },
                   {"ts", myVideoInfodata["data"]["ts"] },
                   {"sign", myVideoInfodata["data"]["sign"] },
                });
            }
            VideoJobject.Add("uid", uid);
            VideoJobject.Add("data", j);
            return VideoJobject;
        }

        /// <summary>
        /// 获取网页内容
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

    }
}