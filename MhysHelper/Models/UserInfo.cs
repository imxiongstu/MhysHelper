using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MhysHelper.Models
{
    public class UserInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<CourseInfo> CourseInfoList = new List<CourseInfo>();

    }
}