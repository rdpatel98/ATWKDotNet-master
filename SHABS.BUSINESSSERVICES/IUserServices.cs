using SHABS.MODELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.BUSINESSSERVICES
{
    public interface IUserServices
    {
        Task<int> AuthenticateAsync(string username, string password);

        RegistrationResponse RegisterUser(UserDetailsModel model);
        string ActivateUser(string userID, string autoID);
        Task<string> DeActivateUserAsync(string userID);
        Task<UserDetailsModel> GetUserDetailsAsync(string userID);
        Task<string> UpdateProfileAsync(UserDetailsModel model);
        Task<int> UpdateUserStatusAsync(string UserID, string status);
    }
}
