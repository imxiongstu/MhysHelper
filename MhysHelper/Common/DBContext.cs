using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;
namespace MhysHelper.Common
{
    public class DBContext
    {
        public SqlSugarClient Instance
        {
            get
            {
                return new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = "Server=localhost;Database=MhysHelperDB;Uid=mhyshelper;Pwd=xiongjie520",
                    DbType = DbType.MySql,
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute
                });
            }
        }
    }
}