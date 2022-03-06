using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.DB
{
    public class FirebaseDBHelper
    {

        private string DBUrl { get; set; } = "https://atwk-testapp.firebaseio.com/";
        FirebaseClient firebase = new FirebaseClient("https://atwk-testapp.firebaseio.com/");

        //private string DBUrl { get; set; } = "https://atwk-bb76a-default-rtdb.firebaseio.com/";
        //FirebaseClient firebase = new FirebaseClient("https://atwk-bb76a-default-rtdb.firebaseio.com/");

        public async Task<int> UpdateProfileAsync(Dictionary<string, string> parameters)
        {

            try
            {
                await firebase
                    .Child("atwk_user_info")
                    .Child(parameters["UserID"])
                    .PatchAsync(parameters);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<int> WriteLog(string guid, string key, string txt)
        {
            try
            {
                await firebase.Child("logs").Child(guid).Child(key).PostAsync(txt);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<Dictionary<string, object>> GetUserDetailsByUsername(string username)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>((await firebase
                    .Child("atwk_user_info")
                    .OrderBy("UserName")
                    .EqualTo(username)
                    .OnceSingleAsync<Dictionary<string, object>>()).FirstOrDefault().Value.ToString());
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> UpdateProfileImageAsync(Dictionary<string, object> parameters)
        {
            try
            {
                await firebase
                    .Child("atwk_user_info")
                    .Child(parameters["UserID"].ToString())
                    .PatchAsync(parameters);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> UpdateUserStatusAsync(string UserID, string status)
        {
            try
            {
                Dictionary<string, string> param = new Dictionary<string, string>() { };
                param.Add("UserLoginStatus", status);
                await firebase
                    .Child("atwk_user_info")
                    .Child(UserID)
                    .PatchAsync(param);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> UpdateLatestSubject(Dictionary<string, string> parameters)
        {
            try
            {
                var roomID = "";
                if (parameters["FromUserType"] == "USER")
                {
                    roomID = await GenerateRoomID(parameters["UserID"], parameters["ToUserID"], parameters["Subject"], parameters["ayatollah"]);
                }
                else
                {
                    roomID = await GenerateRoomID(parameters["ToUserID"], parameters["UserID"], parameters["Subject"], parameters["ayatollah"]);
                }
                await firebase
                        .Child("atwk_latest_subject")
                        .Child(parameters["UserID"])
                        .Child(roomID)
                        .PutAsync(parameters);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<Dictionary<object, string>> GetMessageDetails(Dictionary<object, string> parameters)
        {
            var roomID = "";
            if (parameters["FromUserType"] == "User")
            {
                await GenerateRoomID(parameters["UserID"], parameters["ToID"], parameters["Subject"], parameters["Ayatollah"]);
            }
            else
            {
                await GenerateRoomID(parameters["ToID"], parameters["UserID"], parameters["Subject"], parameters["Ayatollah"]);
            }
            return await firebase.Child("atwk_chat_rooms").Child(roomID).Child(parameters["MessageID"]).OnceSingleAsync<Dictionary<object, string>>();
        }

        public async Task<Dictionary<object, string>> GetLatestMessageDetails(Dictionary<string, string> parameters)
        {
            var roomID = "";
            if (parameters["FromUserType"].ToLower() == "User".ToLower())
            {
                roomID = await GenerateRoomID(parameters["UserID"], parameters["ToID"], parameters["Subject"], parameters["Ayatollah"]);
            }
            else
            {
                roomID = await GenerateRoomID(parameters["ToID"], parameters["UserID"], parameters["Subject"], parameters["Ayatollah"]);
            }
            return await firebase.Child("atwk_latest_subject").Child(parameters["UserID"]).Child(roomID).OnceSingleAsync<Dictionary<object, string>>();

        }
        public async Task<string> GenerateRoomID(string UserID, string ToID, string Subject, string Ayatollah)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var str = $"{UserID}:{ToID}:{Subject.ToLowerInvariant()}:{Ayatollah.ToLowerInvariant()}";

                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(str));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString().ToLowerInvariant();
            }
        }

        public async Task<int> DeActivateUserAsync(string UserID)
        {
            try
            {
                await firebase
                    .Child("atwk_user_info")
                    .Child(UserID)
                    .DeleteAsync();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<Dictionary<string, object>> GetUserDetailsAsync(string userID)
        {
            try
            {
                var result = (await firebase
                    .Child("atwk_user_info")
                    .Child(userID)
                    .OnceSingleAsync<Dictionary<string, object>>());
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Dictionary<string, object>> AuthenticateAsync(string userName, string userPassword)
        {
            try
            {
                var tmp = JsonConvert.DeserializeObject<Dictionary<string, object>>((await firebase
                    .Child("atwk_user_info")
                    .OrderBy("UserName")
                    .EqualTo(userName)
                    .OnceSingleAsync<Dictionary<string, object>>()).FirstOrDefault().Value.ToString());
                return tmp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> AddMessageToChatRoomAsync(Dictionary<string, string> parameters)
        {
            try
            {
                var messageID = parameters["MessageID"];
                var roomID = "";
                if (parameters["FromUserType"] == "USER")
                {
                    roomID = await GenerateRoomID(parameters["FromUserID"], parameters["ToUserID"], parameters["Subject"], parameters["ayatollah"]);
                    await WriteToLatestSubject(parameters["FromUserID"], parameters, roomID);
                }
                else
                {
                    roomID = await GenerateRoomID(parameters["ToUserID"], parameters["FromUserID"], parameters["Subject"], parameters["ayatollah"]);
                    await WriteToLatestSubject(parameters["ToUserID"], parameters, roomID);
                    await WriteToLatestSubject(parameters["FromUserID"], parameters, roomID);
                }
                await firebase
                    .Child("atwk_chat_rooms")
                    .Child(roomID)
                    .Child(messageID)
                    .PutAsync(parameters);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<int> UpdateMessageToChatRoomAsync(Dictionary<string, string> parameters)
        {
            try
            {
                var messageID = parameters["MessageID"];
                var roomID = "";
                if (parameters["FromUserType"] == "USER")
                {
                    roomID = await GenerateRoomID(parameters["FromUserID"], parameters["ToUserID"], parameters["Subject"], parameters["ayatollah"]);
                }
                else
                {
                    roomID = await GenerateRoomID(parameters["ToUserID"], parameters["FromUserID"], parameters["Subject"], parameters["ayatollah"]);
                }
                await firebase
                    .Child("atwk_chat_rooms")
                    .Child(roomID)
                    .Child(messageID)
                    .PutAsync(parameters);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<Dictionary<string, object>> GetLatestMessageToChatRoomAsync(Dictionary<string, string> parameters)
        {

            var messageID = parameters["MessageID"];
            var roomID = "";
            if (parameters["FromUserType"] == "USER")
            {
                roomID = await GenerateRoomID(parameters["FromUserID"], parameters["ToUserID"], parameters["Subject"], parameters["Ayatollah"]);

            }
            else
            {
                roomID = await GenerateRoomID(parameters["ToUserID"], parameters["FromUserID"], parameters["Subject"], parameters["Ayatollah"]);

            }
            var tmp = JsonConvert.DeserializeObject<Dictionary<string, object>>((await firebase
                .Child("atwk_chat_rooms")
                .Child(roomID)

                .OnceSingleAsync<Dictionary<string, object>>()).LastOrDefault().Value.ToString());
            return tmp;
        }


        public async Task<int> CheckTotalMessageCount(Dictionary<string, string> parameters)
        {

            var messageID = parameters["MessageID"];
            var roomID = "";
            if (parameters["FromUserType"] == "USER")
            {
                roomID = await GenerateRoomID(parameters["FromUserID"], parameters["ToUserID"], parameters["Subject"], parameters["Ayatollah"]);
            }
            else
            {
                roomID = await GenerateRoomID(parameters["ToUserID"], parameters["FromUserID"], parameters["Subject"], parameters["Ayatollah"]);

            }
            var tmp = (await firebase
                .Child("atwk_chat_rooms")
                .Child(roomID)
                .OnceSingleAsync<Dictionary<string, object>>()).ToList().Count;
            return tmp;
        }

        public async Task<List<KeyValuePair<string, object>>> GetAllChatMessages(Dictionary<string, string> parameters)
        {

            var messageID = parameters["MessageID"];
            var roomID = "";
            if (parameters["FromUserType"] == "USER")
            {
                roomID = await GenerateRoomID(parameters["FromUserID"], parameters["ToUserID"], parameters["Subject"], parameters["Ayatollah"]);
            }
            else
            {
                roomID = await GenerateRoomID(parameters["ToUserID"], parameters["FromUserID"], parameters["Subject"], parameters["Ayatollah"]);

            }
            var tmp = (await firebase
                .Child("atwk_chat_rooms")
                .Child(roomID)
                .OnceSingleAsync<Dictionary<string, object>>()).ToList();
            return tmp;
        }


        private async Task<int> WriteToLatestSubject(string UserID, Dictionary<string, string> parameters, string roomID)
        {
            try
            {
                var sha1 = "";
                var messageId = "";
                await firebase
                    .Child("atwk_latest_subject")
                    .Child(UserID)
                    .Child(roomID)
                    .PutAsync(parameters);
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> AddMessageToModeratorReview(IDictionary<string, string> parameters)
        {
            try
            {
                var messageId = parameters["MessageID"];
                await firebase
                    .Child("atwk_moderator_review")
                    .Child(messageId)
                    .PutAsync(parameters);

                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> DeleteMessageToModeratorReviewAsync(IDictionary<string, string> parameters)
        {
            var guid = Guid.NewGuid().ToString();
            await WriteLog(guid, "parameters", JsonConvert.SerializeObject(parameters));
            try
            {

                await WriteLog(guid, "MessageID", parameters["MessageID"]);
                await firebase
                       .Child("atwk_moderator_review")
                       .Child(parameters["MessageID"])
                       .DeleteAsync();
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> DeleteLatestSubjectAsync(IDictionary<string, string> parameters)
        {
            var guid = Guid.NewGuid().ToString();
            await WriteLog(guid, "parameters", JsonConvert.SerializeObject(parameters));
            try
            {

                await WriteLog(guid, "UserID", parameters["UserID"]);
                await firebase
                       .Child("atwk_latest_subject")
                       .Child(parameters["UserID"])
                       .DeleteAsync();
                return 1;
            }
            catch
            {
                return 0;
            }
        }


        public async Task<int> DeleteMessageAsync(IDictionary<string, string> parameters)
        {
            var guid = Guid.NewGuid().ToString();
            await WriteLog(guid, "parameters", JsonConvert.SerializeObject(parameters));
            try
            {
                var roomID = "";
                if (parameters["FromUserType"] == "USER")
                {
                    roomID = await GenerateRoomID(parameters["FromUserID"], parameters["ToUserID"], parameters["Subject"], parameters["ayatollah"]);
                }
                else
                {
                    roomID = await GenerateRoomID(parameters["ToUserID"], parameters["FromUserID"], parameters["Subject"], parameters["ayatollah"]);
                }
                await WriteLog(guid, "roomid", JsonConvert.SerializeObject(parameters));
                await WriteLog(guid, "MessageID", parameters["MessageID"]);
                await firebase
                       .Child("atwk_chat_rooms")
                       .Child(roomID)
                       .Child(parameters["MessageID"])
                       .DeleteAsync();
                return 1;
            }
            catch
            {
                return 0;
            }
        }
    }
}