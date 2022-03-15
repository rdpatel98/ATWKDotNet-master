using SHABS.MODELS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.BUSINESSSERVICES
{
    public class TokenServices : ITokenServices
    {

        //public static List<TokenEntity> TokenRepository;
        #region Private member variables.
        #endregion

        #region Public constructor.
        /// <summary>
        /// Public constructor.
        /// </summary>
        public TokenServices()
        {
           
        }
        #endregion


        #region Public member methods.

        /// <summary>
        ///  Function to generate unique token with expiry against the provided userId.
        ///  Also add a record in database for generated token.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public TokenEntity GenerateToken(int userId)
        {
            string token = Guid.NewGuid().ToString();
            DateTime issuedOn = DateTime.Now;
            DateTime expiredOn = DateTime.Now.AddSeconds(
                                              Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
       
            var tokenModel = new TokenEntity()
            {
                UserId = userId,
                IssuedOn = issuedOn,
                ExpiresOn = expiredOn,
                AuthToken = token
            };
            var tempToken = CommonRepository.Current.TokenRepository.FirstOrDefault(x => x.UserId == userId);
            if (tempToken == null)
            {
                CommonRepository.Current.TokenRepository.Add(tokenModel);
            }
            else
            {
                tempToken.ExpiresOn = tokenModel.ExpiresOn;
                tokenModel = tempToken;
            }
           
            return tokenModel;
        }

        /// <summary>
        /// Method to validate token against expiry and existence in database.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public bool ValidateToken(string tokenId)
        {
            var token = CommonRepository.Current.TokenRepository.FirstOrDefault(t => t.AuthToken == tokenId && t.ExpiresOn > DateTime.Now);
            if (token != null && !(DateTime.Now > token.ExpiresOn))
            {
                token.ExpiresOn = DateTime.Now.AddSeconds(
                                              Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to kill the provided token id.
        /// </summary>
        /// <param name="tokenId">true for successful delete</param>
        public bool Kill(string tokenId)
        {
            CommonRepository.Current.TokenRepository.Remove(CommonRepository.Current.TokenRepository.FirstOrDefault(t => t.AuthToken == tokenId));
            var isNotDeleted = CommonRepository.Current.TokenRepository.Any(t => t.AuthToken == tokenId);
            if (isNotDeleted) { return false; }
            return true;
        }

        /// <summary>
        /// Delete tokens for the specific deleted user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>true for successful delete</returns>
        public bool DeleteByUserId(int userId)
        {
            var delete = CommonRepository.Current.TokenRepository.FirstOrDefault(x => x.UserId == userId);
            if (delete != null)
            {
                CommonRepository.Current.TokenRepository.Remove(delete);
            }

            var isNotDeleted = CommonRepository.Current.TokenRepository.Any(x => x.UserId == userId);
            return !isNotDeleted;
        }

        #endregion
    }
}
