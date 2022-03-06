using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHABS.COMMON
{
    public class RandomPasswordGenerator :SingletonBase<RandomPasswordGenerator>
    {
        int length;
        int numberOfNonAlphanumericCharacters;

        private RandomPasswordGenerator()
        {
            length = 8;
            numberOfNonAlphanumericCharacters = 1;
        }


        public string GetRandomPassword()
        {
           // System.Web.Security.Membership.GeneratePassword(int length, int numberOfNonAlphanumericCharacters).
            return System.Web.Security.Membership.GeneratePassword( length,  numberOfNonAlphanumericCharacters);
        }
    }
}
