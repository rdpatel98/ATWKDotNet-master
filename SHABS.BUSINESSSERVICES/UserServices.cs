using SHABS.DB;
using SHABS.MODELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.BUSINESSSERVICES
{
    public class UserServices:IUserServices
    {
        public async Task<int> AuthenticateAsync(string username, string password)
        {
            UserModel model = new UserModel();
            model.UserName = username;
            model.UserPassword = password;

            if (await model.IsValidUserAsync() == "SUCCESS")
            {
                return Convert.ToInt32(model.UserId);
            }
            return 0;
        }
        public async Task<string> UpdateProfileAsync(UserDetailsModel model)
        {
            return await model.UpdateProfileAsync();
        }

        public RegistrationResponse RegisterUser(UserDetailsModel model)
        {
            return model.RegisterUser();
        }
        public string ActivateUser(string userID,string autoID)
        {
            return UserDetailsModel.ActivateUser(userID, autoID);
        }

        public async Task<string> DeActivateUserAsync(string userID)
        {
            return await UserDetailsModel.DeActivateUserAsync(userID);
        }

        public async Task<UserDetailsModel> GetUserDetailsAsync(string userID)
        {
            return  UserDetailsModel.GetUserDetails(userID);
        }

        public Task<int> UpdateUserStatusAsync(string UserID, string status)
        {
            FirebaseDBHelper firebaseDB = new FirebaseDBHelper();
            return firebaseDB.UpdateUserStatusAsync(UserID, status);
        }
    }
}
