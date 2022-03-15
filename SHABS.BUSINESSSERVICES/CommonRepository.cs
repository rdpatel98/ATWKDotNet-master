using SHABS.COMMON;
using SHABS.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.BUSINESSSERVICES
{
    public class CommonRepository : SingletonBase<CommonRepository>
    {
        public  List<TokenEntity> TokenRepository;
        public CommonRepository()
        {
            TokenRepository = new List<TokenEntity>();
        }


    }
}
