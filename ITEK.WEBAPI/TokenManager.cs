using SHABS.BUSINESSSERVICES;
using SHABS.COMMON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ITEK.WEBAPI
{
    /// <summary>
    /// 
    /// </summary>
    public class TokenManager : SingletonBase<TokenManager>
    {
        System.Threading.Timer MyTimer;
        public TokenManager()
        {
            Task.Run(() => Initialise());
        }

        public void Initialise()
        {
            MyTimer = new System.Threading.Timer(PerformTokenManagement, null, 0, 10000);
        }

        public void PerformTokenManagement(object state)
        {
            try
            {
                CommonRepository.Current.TokenRepository.RemoveAll(t => t.ExpiresOn < System.DateTime.Now);
            }
            catch (Exception)
            {
                
            }
           
        }
    }
}