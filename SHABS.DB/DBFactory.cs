using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHABS.COMMON;

namespace SHABS.DB
{

    public enum MyDBType
    {
        OleDB=0,
        Sql=1,
        Oracle=2

    }

    public class DBFactory :SingletonBase<DBFactory>
    {
        public IDBHelper GetDBHelper(MyDBType dbType)
        {
            IDBHelper helper = null;
            switch (dbType)
            {
                case MyDBType.OleDB:
                    helper =(new OleDBHelper());
                    break;
                case MyDBType.Sql:
                    break;
                case MyDBType.Oracle:
                    break;
                default:
                    break;
            }

            return helper;
        }

    }
}
