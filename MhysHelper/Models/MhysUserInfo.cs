using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;
namespace MhysHelper.Models
{
    [SugarTable("MhysUserInfo")]
    public class MhysUserInfo
    {
        public string LoginName { get; set; }
        public string UserName { get; set; }
        public int LoginCount { get; set; }
    }
}