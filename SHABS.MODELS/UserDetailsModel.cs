using Firebase.Database;
using Firebase.Database.Query;
using ITEK.EMAILHELPER;
using Newtonsoft.Json;
using SHABS.DB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
namespace SHABS.MODELS
{

    public class UserDetailsModel : BaseModel, IBase
    {
        public UserModel UserModel { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Nationality { get; set; }
        public string ContactNumber { get; set; }
        public string Details { get; set; }
        public string Gender { get; set; }
        public string ImageText { get; set; }
        public string ImageFileName { get; set; }

        public string ImageID { get; set; }
        public DateTime? LastOnlineTime { get; set; }
        private string _age;
        public string Age
        {
            get
            {
                try
                {
                    return
                    Convert.ToInt32(Convert.ToDecimal(_age)).ToString();
                }
                catch (Exception)
                {

                    return "0";
                }
            }
            set
            {
                if (value == null)
                {
                    _age = "0";
                }
                else
                {
                    _age = value;
                }
            }
        }
        //Extra new 3 veriables
        public string Language { get; set; }
        public string ProfileId
        {
            get;
            set;
        }
        public string SpecialisationIn { get; set; }
        public string StudiesAt { get; set; }
        public string TypicallyRepliesIn
        {
            get;
            set;
        }
        private string _archiveUrl;
        public string ArchiveUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_archiveUrl))
                {
                    return "     ";
                }
                return _archiveUrl;
            }
            set { _archiveUrl = value; }
        }

        public string UnreadMessageCount { get; set; }

        public UserDetailsModel()
        {
            UserModel = new UserModel();
        }
        public static void SetValue(UserDetailsModel temp, DataRow dr)
        {
            temp.UserID = dr["UserID"].ToString();
            temp.Name = dr["Name"].ToString();
            temp.UserModel.UserId = dr["UserID"].ToString();
            temp.UserModel.UserName = dr["UserName"].ToString();
            temp.Location = dr["Location"].ToString();
            temp.Nationality = dr["Nationality"].ToString();
            temp.Details = dr["Details"] as string;
            temp.Age = dr["Age"].ToString();
            temp.UserModel.UserLoginStatus = dr["UserLoginStatus"] as string;
            temp.UserModel.UserType = dr["UserType"] as string;
            temp.Gender = dr["Gender"] as string;
            temp.ImageID = dr["ImageID"].ToString();
            if (!string.IsNullOrEmpty(dr["lastOnlineTime"].ToString()))
            {
                temp.LastOnlineTime = Convert.ToDateTime(dr["lastOnlineTime"].ToString());
            }
            temp.Language = dr["Language"].ToString();
            temp.SpecialisationIn = dr["SpecialisationIn"].ToString();
            temp.StudiesAt = dr["StudiesAt"].ToString();

        }

        public static void SetAllValue(UserDetailsModel temp, DataRow dr)
        {
            temp.UserID = dr["UserID"].ToString();
            temp.Name = dr["Name"].ToString();
            temp.UserModel.UserId = dr["UserID"].ToString();
            temp.UserModel.UserName = dr["UserName"].ToString();
            temp.UserModel.UserType = dr["UserType"].ToString();
            temp.UserModel.UserStatus = dr["UserStatus"].ToString();
            if (temp.UserModel.UserStatus == "ACTV")
            {
                temp.UserModel.UserStatus = "Active";
            }
            else
            {
                temp.UserModel.UserStatus = "InActive";
            }
            temp.UserModel.UserLoginStatus = dr["UserLoginStatus"] as string;
            temp.Location = dr["Location"].ToString();
            temp.Nationality = dr["Nationality"].ToString();
            temp.Details = dr["Details"] as string;
            temp.Age = dr["Age"].ToString();
            temp.ContactNumber = dr["mobilenumber"].ToString();
            temp.Gender = dr["Gender"] as string;
            temp.ImageID = dr["ImageID"].ToString();
            if (!string.IsNullOrEmpty(dr["lastOnlineTime"].ToString()))
            {
                temp.LastOnlineTime = Convert.ToDateTime(dr["lastOnlineTime"].ToString());
            }
            temp.Language = dr["Language"].ToString();
            temp.SpecialisationIn = dr["SpecialisationIn"].ToString();
            temp.StudiesAt = dr["StudiesAt"].ToString();
            if (!string.IsNullOrEmpty(dr["createdBy"].ToString()))
            {
                temp.UserModel.CreatorUserId = dr["createdBy"].ToString();
            }
            if (!string.IsNullOrEmpty(dr["modifiedBy"].ToString()))
            {
                temp.UserModel.ModifierUserId = dr["modifiedBy"].ToString();
            }
            if (!string.IsNullOrEmpty(dr["UserProfileID"].ToString()))
            {
                temp.ProfileId = dr["UserProfileID"].ToString();
            }
            temp.TypicallyRepliesIn = dr["TypicallyRepliesIn"].ToString();
            try
            {
                temp.ArchiveUrl = dr["ArchiveUrl"].ToString();
            }
            catch
            {

            }

        }

        public static List<UserDetailsModel> AlimsByLocation(string location)
        {
            List<UserDetailsModel> modelList = new List<UserDetailsModel>();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@location", location);
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetAllALimsByCountry", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    UserDetailsModel temp = new UserDetailsModel();
                    SetValue(temp, dt1.Rows[cntr]);
                    modelList.Add(temp);
                }
            }
            return modelList;
        }


        public static List<IBase> GetChatHistoryUsers(string userID)
        {
            List<IBase> modelList = new List<IBase>();
            //GetAllALims
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@userID", userID);
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetChatHistoryUsers", parameters);
            if (dt1.Rows.Count > 0)
            {
                if (dt1.Columns.Contains("QueryBy"))
                {
                    for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                    {
                        ModeratorHistoryUserModel temp = new ModeratorHistoryUserModel();
                        temp.UsersName = dt1.Rows[cntr]["UsersName"].ToString();
                        temp.UsersUserName = dt1.Rows[cntr]["UsersUserName"].ToString();
                        temp.UsersUserID = dt1.Rows[cntr]["UsersUserID"].ToString();
                        temp.UsersImageID = dt1.Rows[cntr]["UsersImageID"].ToString();
                        temp.ScholarsName = dt1.Rows[cntr]["ScholarsName"].ToString();
                        temp.ScholarsUserName = dt1.Rows[cntr]["ScholarsUserName"].ToString();
                        temp.ScholarsUserID = dt1.Rows[cntr]["ScholarsUserID"].ToString();
                        temp.ScholarsImageID = dt1.Rows[cntr]["ScholarsImageID"].ToString();
                        modelList.Add(temp);
                    }
                }
                else
                {
                    for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                    {
                        UserDetailsModel temp = new UserDetailsModel();
                        SetValue(temp, dt1.Rows[cntr]);
                        modelList.Add(temp);
                    }
                }
            }
            return modelList;
        }

        public static List<ModeratorHistoryUserModel> GetChatHistoryUsers2(string userID)
        {
            List<ModeratorHistoryUserModel> modelList = new List<ModeratorHistoryUserModel>();
            //GetAllALims
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@userID", userID);
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetChatHistoryUsers", parameters);
            if (dt1.Rows.Count > 0)
            {
                if (dt1.Columns.Contains("QueryBy"))
                {
                    for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                    {
                        ModeratorHistoryUserModel temp = new ModeratorHistoryUserModel();
                        temp.UsersName = dt1.Rows[cntr]["UsersName"].ToString();
                        temp.UsersUserName = dt1.Rows[cntr]["UsersUserName"].ToString();
                        temp.UsersUserID = dt1.Rows[cntr]["UsersUserID"].ToString();
                        temp.UsersImageID = dt1.Rows[cntr]["UsersImageID"].ToString();
                        temp.ScholarsName = dt1.Rows[cntr]["ScholarsName"].ToString();
                        temp.ScholarsUserName = dt1.Rows[cntr]["ScholarsUserName"].ToString();
                        temp.ScholarsUserID = dt1.Rows[cntr]["ScholarsUserID"].ToString();
                        temp.ScholarsImageID = dt1.Rows[cntr]["ScholarsImageID"].ToString();
                        modelList.Add(temp);
                    }

                }

            }
            return modelList;
        }


        public static List<UserDetailsModel> AllALims()
        {
            List<UserDetailsModel> modelList = new List<UserDetailsModel>();
            //GetAllALims
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetAllALims", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    UserDetailsModel temp = new UserDetailsModel();
                    SetValue(temp, dt1.Rows[cntr]);
                    modelList.Add(temp);
                }
            }
            return modelList;
        }

        public static string StoreRegistrationID(string userID, string registrationID)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@userId", userID);
            parameters.Add("@registrationID", registrationID);
            int result = (new OleDBHelper()).InsertUpdateData("UpdateRegistrationID", parameters);
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public static string GetRegistrationID(string userID, string indc = "0")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@userId", userID);
            parameters.Add("@indc", indc);
            string result = string.Empty;
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetRegistrationID", parameters);
            if (dt1.Rows.Count > 0)
            {
                result = dt1.Rows[0]["RegistrationID"] as string;
            }

            return result ?? string.Empty;
        }

        public static string GetRegistrationScollerID(string messageID)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@messageID", messageID);
            string result = string.Empty;
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetRegistrationScollerID", parameters);
            if (dt1.Rows.Count > 0)
            {
                result = dt1.Rows[0]["RegistrationID"] as string;
            }

            return result ?? string.Empty;
        }

        public RegistrationResponse RegisterUser()
        {
            RegistrationResponse res = new RegistrationResponse();
            //"RegisterUser"
            string response = "ERROR";
            string autoID = string.Empty;
            string userID = string.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("@username", UserModel.UserName);

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetUserByUsername", parameters);
            if (dt1.Rows.Count > 0)
            {
                string status = dt1.Rows[0]["userStatus"].ToString();
                if (status == "ACTV")
                {
                    res.Status = "User already registered.";
                    return res;
                }
                else
                {
                    res.Status = "The User is registered but is not yet activated. Please check your mail for activation.";
                    return res;
                }
            }
            if (!this.CheckScreenNameCount(this.Name, "0"))
            {
                res.Status = "Screen Name Already used. Please use a different name";
                return res;
            }

            parameters.Add("@password", UserModel.UserPassword);
            parameters.Add("@name", Name);
            parameters.Add("@location", Location);
            parameters.Add("@mobileNumber", ContactNumber);
            parameters.Add("@nationality", Nationality);
            parameters.Add("@userType", UserModel.UserType);//USER, ALIM
            parameters.Add("@userStatus", "IACT");
            if (Details == null)
            {
                Details = string.Empty;
            }
            if (string.IsNullOrEmpty(Age))
            {
                Age = "0";
            }

            if (string.IsNullOrEmpty(Gender))
            {
                Gender = string.Empty;
            }
            parameters.Add("@details", Details);
            parameters.Add("@age", Age);
            parameters.Add("@gender", Gender);
            parameters.Add("@Language", Language);
            parameters.Add("@SpecialisationIn", SpecialisationIn);
            parameters.Add("@StudiesAt", StudiesAt);
            //if (string.IsNullOrEmpty(CreatorUserId))
            //{
            //    CreatorUserId = "0";
            //}
            //parameters.Add("@CreatorUserId", CreatorUserId);
            DataTable dt = new DataTable();
            dt = (new OleDBHelper()).GetData("RegisterUser", parameters);
            if (dt.Rows.Count > 0)
            {
                autoID = dt.Rows[0]["passphrase"].ToString();
                res.UserID = userID = dt.Rows[0]["userID"].ToString();
                res.Status = response = "SUCCESS";
            }

            if (!string.IsNullOrEmpty(UserModel.RegistrationID))
            {
                StoreRegistrationID(userID, UserModel.RegistrationID);
            }
            //Send Email here
            if (response == "SUCCESS")
            {
                string SmtpServer = ConfigurationManager.AppSettings["SMTPServer"];
                int SmtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                string EmailUserName = ConfigurationManager.AppSettings["EmailUserName"];
                string EmailPassword = ConfigurationManager.AppSettings["EmailPassword"];
                bool IsSSL = ConfigurationManager.AppSettings["EnableSSL"] == "1";
                string fromEmailID = ConfigurationManager.AppSettings["EmailID"];

                EmailMessage emailClient = new EmailMessage();
                emailClient.Message = generateEmail(userID, autoID);
                //emailClient.Subject = "MyApp: ActivationLink";
                emailClient.Subject = "Ask those who know: Login pending";
                emailClient.To = new List<string> { UserModel.UserName };
                emailClient.SendEmail(SmtpServer, SmtpPort, fromEmailID, IsSSL, EmailUserName, EmailPassword);
            }
            return res;

        }

        public RegistrationResponse RegisterUserWithImage()
        {
            //"RegisterUser"
            RegistrationResponse res = new RegistrationResponse();
            string response = "ERROR";
            string autoID = string.Empty;
            string userID = string.Empty;
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@username", UserModel.UserName);

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetUserByUsername", parameters);
            if (dt1.Rows.Count > 0)
            {
                string status = dt1.Rows[0]["userStatus"].ToString();
                if (status == "ACTV")
                {
                    res.Status = "User already registered.";
                    return res;
                }
                else
                {
                    res.Status = "The User is registered but is not yet activated. Please check your mail for activation.";
                    return res;
                }
            }
            if (!this.CheckScreenNameCount(this.Name, "0"))
            {
                res.Status = "Screen Name Already used. Please use a different name";
                return res;
            }

            parameters.Add("@password", UserModel.UserPassword);
            parameters.Add("@name", Name);
            parameters.Add("@location", Location);
            parameters.Add("@mobileNumber", ContactNumber);
            parameters.Add("@nationality", Nationality);
            parameters.Add("@userType", UserModel.UserType);//USER, ALIM
            parameters.Add("@userStatus", "IACT");
            if (Details == null)
            {
                Details = string.Empty;
            }
            if (string.IsNullOrEmpty(Age))
            {
                Age = "0";
            }

            if (string.IsNullOrEmpty(Gender))
            {
                Gender = string.Empty;
            }
            parameters.Add("@details", Details);
            parameters.Add("@age", Age);
            parameters.Add("@gender", Gender);
            parameters.Add("@FileName", ImageFileName);
            string contenttype = string.Empty;
            switch (System.IO.Path.GetExtension(ImageFileName))
            {
                case ".wav":
                    contenttype = "application/wav";
                    break;
                case ".doc":
                    contenttype = "application/vnd.ms-word";
                    break;
                case ".docx":
                    contenttype = "application/vnd.ms-word";
                    break;
                case ".xls":
                    contenttype = "application/vnd.ms-excel";
                    break;
                case ".xlsx":
                    contenttype = "application/vnd.ms-excel";
                    break;
                case ".jpg":
                    contenttype = "image/jpg";
                    break;
                case ".png":
                    contenttype = "image/png";
                    break;
                case ".gif":
                    contenttype = "image/gif";
                    break;
                case ".pdf":
                    contenttype = "application/pdf";
                    break;
                case ".txt":
                    contenttype = "text/plain";
                    break;
                case ".mp3":
                    contenttype = "audio/mp3";
                    break;
            }
            parameters.Add("@ContentType", contenttype);
            parameters.Add("@Data", Convert.FromBase64String(ImageText));
            parameters.Add("@Language", Language);
            parameters.Add("@SpecialisationIn", SpecialisationIn);
            parameters.Add("@StudiesAt", StudiesAt);
            //if (string.IsNullOrEmpty(CreatorUserId))
            //{
            //    CreatorUserId = "0";
            //}
            //parameters.Add("@CreatorUserId", CreatorUserId);
            DataTable dt = new DataTable();
            dt = (new OleDBHelper()).GetData("RegisterUserWithImage", parameters);
            if (dt.Rows.Count > 0)
            {
                //SaveDataInFirebase(dt);
                autoID = dt.Rows[0]["passphrase"].ToString();
                userID = dt.Rows[0]["userID"].ToString();
                res.UserID = dt.Rows[0]["userID"].ToString();
                res.ImageID = dt.Rows[0]["imgID"].ToString();
                res.Status = response = "SUCCESS";
            }

            if (!string.IsNullOrEmpty(UserModel.RegistrationID))
            {
                StoreRegistrationID(userID, UserModel.RegistrationID);
            }
            //Send Email here
            if (response == "SUCCESS")
            {
                string SmtpServer = ConfigurationManager.AppSettings["SMTPServer"];
                int SmtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                string EmailUserName = ConfigurationManager.AppSettings["EmailUserName"];
                string EmailPassword = ConfigurationManager.AppSettings["EmailPassword"];
                bool IsSSL = ConfigurationManager.AppSettings["EnableSSL"] == "1";
                string fromEmailID = ConfigurationManager.AppSettings["EmailID"];

                EmailMessage emailClient = new EmailMessage();
                emailClient.Message = generateEmail(userID, autoID);
                emailClient.Subject = "Ask those who know: ActivationLink";
                emailClient.To = new List<string> { UserModel.UserName };
                emailClient.SendEmail(SmtpServer, SmtpPort, fromEmailID, IsSSL, EmailUserName, EmailPassword);
            }
            return res;

        }

        private string generateEmail(string userID, string passphrase)
        {
            string str = string.Concat("<html><body><table width='100%'><tr><td align='left'>Please click the bismillah picture  to activate</td></tr><tr><td align='left'><a href=\"{0}\"><img src='", ConfigurationManager.AppSettings["DomainName"], "/MailImags/BISMILLAH.JPG' width='600px' height='150px' /></a></td></tr><tr><td align='left'><p style='text-align:left'>What is this app about?<br />As the name says, an ability to communicate directly with learned Islamic ULEMA ready to spare their precious time to answer your queries related to;<br />1. Islamic Laws and Jurisprudence.<br />2. Personal or Business related issues that need to be resolved in view of Islamic teachings of Ahlulbayt(as)<br />3. The need of self-purification/counseling.<br />4. Clear unwanted thoughts or misconception bothering you.<br />5. Spiritual discussions<br />6.Practical life centered issues about dealing with difficulty, parenting, relationships, forgiveness of Allah swt <br /></p><p>We pray to Allah to accept our small contribution for the welfare of our own selves.<br />Contact us for any suggestions <a href='mailto:admin@askthosewhoknow.org'>admin@askthosewhoknow.org</a></p></td></tr></table></body></html>");
            string[] item = new string[] { ConfigurationManager.AppSettings["DomainName"], "/Activate/", passphrase, "/", userID };
            return string.Format(str, string.Concat(item));
        }

        private static string generateReactivationEmail(string userID, string passphrase)
        {
            string str = string.Concat("<html><body><table width='100%'><tr><td align='left'>Please click the bismillah picture  to reactivate</td></tr><tr><td align='left'><a href=\"{0}\"><img src='", ConfigurationManager.AppSettings["DomainName"], "/MailImags/BISMILLAH.JPG' width='600px' height='150px' /></a></td></tr><tr><td align='left'><p style='text-align:left'>What is this app about?<br />As the name says, an ability to communicate directly with learned Islamic ULEMA ready to spare their precious time to answer your queries related to;<br />1. Islamic Laws and Jurisprudence.<br />2. Personal or Business related issues that need to be resolved in view of Islamic teachings of Ahlulbayt(as)<br />3. The need of self-purification/counseling.<br />4. Clear unwanted thoughts or misconception bothering you.<br />5. Spiritual discussions<br />6.Practical life centered issues about dealing with difficulty, parenting, relationships, forgiveness of Allah swt <br /></p><p>We pray to Allah to accept our small contribution for the welfare of our own selves.<br />Contact us for any suggestions <a href='mailto:admin@askthosewhoknow.org'>admin@askthosewhoknow.org</a></p></td></tr></table></body></html>");
            string[] item = new string[] { ConfigurationManager.AppSettings["DomainName"], "/Activate/", passphrase, "/", userID };
            return string.Format(str, string.Concat(item));
        }

        public static string ActivateUser(string userID, string autoID)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("UserPassword", autoID);
            parameters.Add("UserID", userID);


            int result = (new OleDBHelper()).InsertUpdateData("activateUser", parameters);
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public async Task<string> UpdateProfileAsync()
        {
            //UpdateProfile
            if (Details == null)
            {
                Details = string.Empty;
            }
            if (string.IsNullOrEmpty(Age))
            {
                Age = "0";
            }
            if (string.IsNullOrEmpty(Gender))
            {
                Gender = string.Empty;
            }
            if (string.IsNullOrEmpty(this.TypicallyRepliesIn))
            {
                this.TypicallyRepliesIn = string.Empty;
            }
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("UserID", UserID);
            parameters.Add("Name", Name);
            parameters.Add("Location", Location);
            parameters.Add("Details", Details);
            parameters.Add("Gender", Gender);
            parameters.Add("Language", Language);
            parameters.Add("SpecialisationIn", SpecialisationIn);
            parameters.Add("StudiesAt", StudiesAt);
            parameters.Add("ImageID", ImageID);
            parameters.Add("LastOnlineTime", LastOnlineTime.Value.ToString("dd-MM-yyyy"));
            parameters.Add("Nationality", Nationality);
            parameters.Add("UserLoginStatus", "ONLINE");
            parameters.Add("UserType", UserModel.UserType);
            //int result = (new OleDBHelper()).InsertUpdateData("UpdateProfile", parameters);
            FirebaseDBHelper firebaseDB = new FirebaseDBHelper();
            var result = await firebaseDB.UpdateProfileAsync(parameters);
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "The profile cannot be updated. Please try again later.";
            }
        }

        public async Task<string> UpdateProfileImageAsync()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userid", UserID);
            parameters.Add("@FileName", ImageFileName);
            string contenttype = string.Empty;
            switch (System.IO.Path.GetExtension(ImageFileName))
            {
                case ".wav":
                    contenttype = "application/wav";
                    break;
                case ".doc":
                    contenttype = "application/vnd.ms-word";
                    break;
                case ".docx":
                    contenttype = "application/vnd.ms-word";
                    break;
                case ".xls":
                    contenttype = "application/vnd.ms-excel";
                    break;
                case ".xlsx":
                    contenttype = "application/vnd.ms-excel";
                    break;
                case ".jpg":
                    contenttype = "image/jpg";
                    break;
                case ".png":
                    contenttype = "image/png";
                    break;
                case ".gif":
                    contenttype = "image/gif";
                    break;
                case ".pdf":
                    contenttype = "application/pdf";
                    break;
                case ".txt":
                    contenttype = "text/plain";
                    break;
                case ".mp3":
                    contenttype = "audio/mp3";
                    break;
            }
            parameters.Add("@ContentType", contenttype);
            parameters.Add("@Data", Convert.FromBase64String(ImageText));
            (new OleDBHelper()).InsertUpdateData("UpdateProfileImage", parameters);
            Dictionary<string, object> firebaseParameters = new Dictionary<string, object>();
            firebaseParameters.Add("UserID", UserID);
            UserDetailsModel temp = new UserDetailsModel();
            Dictionary<string, string> userDetailsparam = new Dictionary<string, string>();
            userDetailsparam.Add("userID", UserID);
            DataTable dt = new DataTable();
            dt = (new OleDBHelper()).GetData("GetUserDetails", userDetailsparam);
            firebaseParameters.Add("ImageID", dt.Rows[0]["ImageID"]);
            //int result = (new OleDBHelper()).InsertUpdateData("UpdateProfileImage", parameters);
            FirebaseDBHelper firebaseDB = new FirebaseDBHelper();
            int result = await firebaseDB.UpdateProfileImageAsync(firebaseParameters);
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "The profile cannot be updated. Please try again later.";
            }
        }

        public static async Task<UserDetailsModel> GetUserDetailsAsync(string userID)
        {
            DataTable dt = new DataTable();
            //dt = (new OleDBHelper()).GetData("GetUserDetails", parameters);
            FirebaseDBHelper firebaseDB = new FirebaseDBHelper();
            Dictionary<string, object> temp = await firebaseDB.GetUserDetailsAsync(userID);
            //var userDetails = ObjectFromDictionary<UserDetailsModel>(temp);
            var userDetailsJson = JsonConvert.SerializeObject(temp);
            var userDetailsData = JsonConvert.DeserializeObject<UserDetailsModel>(userDetailsJson);

            userDetailsData.UserModel.UserId = userDetailsData.UserID;
           
            bool keyExists1 = temp.ContainsKey("UserLoginStatus");
            if (keyExists1)
            {
                userDetailsData.UserModel.UserLoginStatus = temp["UserLoginStatus"].ToString();
            }

            bool userNameKeyExists = temp.ContainsKey("UserName");
            if (userNameKeyExists)
            {
                userDetailsData.UserModel.UserName = temp["UserName"].ToString();
            }

            bool userMembershipIdKeyExists = temp.ContainsKey("UserMemberShipID");
            if (userMembershipIdKeyExists)
            {
                userDetailsData.UserModel.UserMemberShipID = temp["UserMemberShipID"].ToString();
            }

            bool keyExists = temp.ContainsKey("UserType");
            if (keyExists)
            {
                userDetailsData.UserModel.UserType = temp["UserType"].ToString();
            }

            bool userStatusKeyExists = temp.ContainsKey("UserStatus");
            if (userStatusKeyExists)
            {
                userDetailsData.UserModel.UserStatus = temp["UserStatus"].ToString();
            }

            bool userPassphraseKeyExists = temp.ContainsKey("Passphrase");
            if (userPassphraseKeyExists)
            {
                userDetailsData.UserModel.Passphrase = temp["Passphrase"].ToString();
            }

            return userDetailsData;
        }


        public static UserDetailsModel GetUserDetails(string userID)
        {
            UserDetailsModel temp = new UserDetailsModel();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("userID", userID);
            DataTable dt = new DataTable();
            dt = (new OleDBHelper()).GetData("GetUserDetails", parameters);
            if (dt.Rows.Count > 0)
            {
                // SetValue(temp, dt.Rows[0]);
                temp.Name = dt.Rows[0]["name"].ToString();
                temp.Nationality = dt.Rows[0]["Nationality"].ToString();
                temp.ContactNumber = dt.Rows[0]["mobileNumber"].ToString();
                temp.Location = dt.Rows[0]["location"].ToString();
                temp.Age = dt.Rows[0]["Age"].ToString();
                temp.Gender = dt.Rows[0]["Gender"] as string;
                temp.Details = dt.Rows[0]["Details"] as string;
                temp.UserModel.UserId = dt.Rows[0]["userID"].ToString();
                temp.UserModel.UserName = dt.Rows[0]["username"].ToString();
                temp.UserModel.UserType = dt.Rows[0]["UserType"].ToString();
                temp.UserModel.UserStatus = dt.Rows[0]["UserStatus"].ToString();
                temp.ImageID = dt.Rows[0]["ImageID"].ToString();
                if (!string.IsNullOrEmpty(dt.Rows[0]["lastOnlineTime"].ToString()))
                {
                    temp.LastOnlineTime = Convert.ToDateTime(dt.Rows[0]["lastOnlineTime"].ToString());
                }
                temp.Language = dt.Rows[0]["Language"].ToString();
                temp.SpecialisationIn = dt.Rows[0]["SpecialisationIn"].ToString();
                temp.StudiesAt = dt.Rows[0]["StudiesAt"].ToString();
                temp.TypicallyRepliesIn = dt.Rows[0]["TypicallyRepliesIn"].ToString();

                try
                {
                    temp.ArchiveUrl = dt.Rows[0]["ArchiveUrl"].ToString();
                }
                catch
                {

                }
            }
            return temp;
        }
        public static List<UserDetailsModel> AllUsers()
        {
            List<UserDetailsModel> modelList = new List<UserDetailsModel>();
            //GetAllALims
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetAllUsers", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    UserDetailsModel temp = new UserDetailsModel();
                    SetValue(temp, dt1.Rows[cntr]);
                    modelList.Add(temp);
                }
            }
            return modelList;
        }

        private bool CheckScreenNameCount(string name, string userID)
        {
            bool flag = true;
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "@name", name },
                { "@userID", userID }
            };
            DataTable dataTable = new DataTable();
            if (Convert.ToInt16((new OleDBHelper()).GetData("ScreenNameCount", parameters).Rows[0][0]) != 0)
            {
                flag = false;
            }
            return flag;
        }

        public static List<IBase> GetIncomingMessageUsers(string userID)
        {
            List<IBase> modelList = new List<IBase>();
            //GetAllALims
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@userID", userID);
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetIncomingMessageUsers", parameters);
            if (dt1.Rows.Count > 0)
            {
                if (dt1.Columns.Contains("QueryBy"))
                {
                    for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                    {
                        ModeratorHistoryUserModel temp = new ModeratorHistoryUserModel();
                        temp.UsersName = dt1.Rows[cntr]["UsersName"].ToString();
                        temp.UsersUserName = dt1.Rows[cntr]["UsersUserName"].ToString();
                        temp.UsersUserID = dt1.Rows[cntr]["UsersUserID"].ToString();
                        temp.UsersImageID = dt1.Rows[cntr]["UsersImageID"].ToString();
                        temp.ScholarsName = dt1.Rows[cntr]["ScholarsName"].ToString();
                        temp.ScholarsUserName = dt1.Rows[cntr]["ScholarsUserName"].ToString();
                        temp.ScholarsUserID = dt1.Rows[cntr]["ScholarsUserID"].ToString();
                        temp.ScholarsImageID = dt1.Rows[cntr]["ScholarsImageID"].ToString();
                        temp.UnreadMessageCount = dt1.Rows[cntr]["unreadMessages"].ToString();
                        modelList.Add(temp);
                    }
                }
                else
                {
                    for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                    {
                        UserDetailsModel temp = new UserDetailsModel();
                        SetValue(temp, dt1.Rows[cntr]);
                        temp.UnreadMessageCount = dt1.Rows[cntr]["unreadMessages"].ToString();
                        modelList.Add(temp);
                    }
                }
            }
            return modelList;
        }

        public static List<IBase> GetUnansweredQueryUsers()
        {
            List<IBase> bases = new List<IBase>();
            Dictionary<string, string> strs = new Dictionary<string, string>();
            DataTable dataTable = new DataTable();
            dataTable = (new OleDBHelper()).GetData("GetUnansweredAlims", strs);
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ModeratorHistoryUserModel moderatorHistoryUserModel = new ModeratorHistoryUserModel()
                    {
                        UsersName = dataTable.Rows[i]["UsersName"].ToString(),
                        UsersUserName = dataTable.Rows[i]["UsersUserName"].ToString(),
                        UsersUserID = dataTable.Rows[i]["UsersUserID"].ToString(),
                        UsersImageID = dataTable.Rows[i]["UsersImageID"].ToString(),
                        ScholarsName = dataTable.Rows[i]["ScholarsName"].ToString(),
                        ScholarsUserName = dataTable.Rows[i]["ScholarsUserName"].ToString(),
                        ScholarsUserID = dataTable.Rows[i]["ScholarsUserID"].ToString(),
                        ScholarsImageID = dataTable.Rows[i]["ScholarsImageID"].ToString()
                    };
                    bases.Add(moderatorHistoryUserModel);
                }
            }
            return bases;
        }

        public static List<UserDetailsModel> GetOutgoingMessageUsers(string userID)
        {
            List<UserDetailsModel> modelList = new List<UserDetailsModel>();
            //GetAllALims
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@userID", userID);
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetOutgoingMessageUsers", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    UserDetailsModel temp = new UserDetailsModel();
                    SetValue(temp, dt1.Rows[cntr]);
                    modelList.Add(temp);
                }
            }
            return modelList;
        }

        public static async Task<string> DeActivateUserAsync(string userID)
        {
            FirebaseDBHelper firebaseDB = new FirebaseDBHelper();
            //int result = (new OleDBHelper()).InsertUpdateData("deActivateUser", parameters);
            int result = await firebaseDB.DeActivateUserAsync(userID);
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public static string ReActivateUser(string username)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@username", username);

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetUserByUsername", parameters);
            if (dt1.Rows.Count > 0)
            {
                string status = dt1.Rows[0]["userStatus"].ToString();
                string userID = dt1.Rows[0]["userid"].ToString();
                string autoID = dt1.Rows[0]["passphrase"].ToString();
                if (status == "ACTV")
                {
                    return "User is already active";
                }
                else
                {
                    string SmtpServer = ConfigurationManager.AppSettings["SMTPServer"];
                    int SmtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                    string EmailUserName = ConfigurationManager.AppSettings["EmailUserName"];
                    string EmailPassword = ConfigurationManager.AppSettings["EmailPassword"];
                    bool IsSSL = ConfigurationManager.AppSettings["EnableSSL"] == "1";
                    string fromEmailID = ConfigurationManager.AppSettings["EmailID"];

                    EmailMessage emailClient = new EmailMessage();
                    emailClient.Message = generateReactivationEmail(userID, autoID);
                    emailClient.Subject = "Ask those who know: ReActivationLink";
                    emailClient.To = new List<string> { username };
                    if (emailClient.SendEmail(SmtpServer, SmtpPort, fromEmailID, IsSSL, EmailUserName, EmailPassword))
                    {
                        return "SUCCESS";
                    }
                    else
                    {
                        return "Sorry the reactivation link cannot be sent now. Please try again after some time or send a mail to administrators.";
                    }
                }
            }

            return "Sorry, This username does not exist. Please register to create an account.";
        }

        public static UserDetailsModel GetUserDetailsByUsernameAsync(string username)
        {
            var data = (new OleDBHelper()).GetUserDetailsByUsername(username);
            var temp = new UserDetailsModel();
            temp.UserModel.UserType = data["userType"].ToString();
            temp.UserID = data["userID"].ToString();
            return temp;
        }

        public static List<UserDetailsModel> AllAccessors(string userType)
        {
            List<UserDetailsModel> modelList = new List<UserDetailsModel>();
            //GetAllALims
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@userType", userType);
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetAllAccesors", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    UserDetailsModel temp = new UserDetailsModel();
                    SetAllValue(temp, dt1.Rows[cntr]);
                    modelList.Add(temp);
                }
            }
            return modelList;
        }

        public static string DeleteUser(string identifier, int indc = 0)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@userID", identifier);
            parameters.Add("@indc", indc.ToString());
            int result = (new OleDBHelper()).InsertUpdateData("deleteUser", parameters);
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public static async Task<string> UpdateUserStatusAsync(string identifier, string status)
        {
            //updateUserStatus
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("UserStatus", status);
            //int result = (new OleDBHelper()).InsertUpdateData("updateUserStatus", parameters);
            FirebaseDBHelper firebaseDB = new FirebaseDBHelper();
            int result = await firebaseDB.UpdateUserStatusAsync(identifier, status);
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public static dynamic GetHomePageCount()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetHomePageDetails", parameters);
            dynamic tempObject = new System.Dynamic.ExpandoObject(); ;
            tempObject.UsersCount = dt1.Rows[0]["USER"];
            tempObject.AlimsCount = dt1.Rows[0]["ALIM"];
            tempObject.ModeratorsCount = dt1.Rows[0]["MODERATOR"];
            tempObject.MessagesCount = dt1.Rows[0]["MESSAGE"];

            return tempObject;
        }

        public string UpdateTypicallyRepliesIn()
        {
            Dictionary<string, string> strs = new Dictionary<string, string>()
            {
                { "@userid", this.UserID },
                { "@TypicallyRepliesIn", this.TypicallyRepliesIn }
            };
            if ((new OleDBHelper()).InsertUpdateData("UpdateTypicallyRepliesIn", strs) > 0)
            {
                return "SUCCESS";
            }
            return "The data cannot be updated. Please try again later.";
        }

        public static List<IBase> GetPendingQueryAlimsWithCount()
        {
            List<IBase> bases = new List<IBase>();
            Dictionary<string, string> strs = new Dictionary<string, string>();
            DataTable dataTable = new DataTable();
            dataTable = (new OleDBHelper()).GetData("GetPendingQueryAlimsWithCount", strs);
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ModeratorHistoryUserModel moderatorHistoryUserModel = new ModeratorHistoryUserModel()
                    {
                        ScholarsName = dataTable.Rows[i]["ScholarsName"].ToString(),
                        ScholarsUserName = dataTable.Rows[i]["ScholarsUserName"].ToString(),
                        ScholarsUserID = dataTable.Rows[i]["ScholarsUserID"].ToString(),
                        ScholarsImageID = dataTable.Rows[i]["ScholarsImageID"].ToString(),
                        UnreadMessageCount = dataTable.Rows[i]["unreadCount"].ToString()
                    };
                    bases.Add(moderatorHistoryUserModel);
                }
            }
            return bases;
        }

        public static List<IBase> GetPendingQueryWCByAlim(string scholarUsername)
        {
            List<IBase> bases = new List<IBase>();
            Dictionary<string, string> strs = new Dictionary<string, string>();
            strs.Add("@scholarUsername", scholarUsername);
            DataTable dataTable = new DataTable();
            dataTable = (new OleDBHelper()).GetData("GetPendingQueryWCByAlim", strs);
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ModeratorHistoryUserModel moderatorHistoryUserModel = new ModeratorHistoryUserModel()
                    {
                        UsersName = dataTable.Rows[i]["UsersName"].ToString(),
                        UsersUserName = dataTable.Rows[i]["UsersUserName"].ToString(),
                        UsersUserID = dataTable.Rows[i]["UsersUserID"].ToString(),
                        UsersImageID = dataTable.Rows[i]["UsersImageID"].ToString(),
                        ScholarsName = dataTable.Rows[i]["ScholarsName"].ToString(),
                        ScholarsUserName = dataTable.Rows[i]["ScholarsUserName"].ToString(),
                        ScholarsUserID = dataTable.Rows[i]["ScholarsUserID"].ToString(),
                        ScholarsImageID = dataTable.Rows[i]["ScholarsImageID"].ToString(),
                        UnreadMessageCount = dataTable.Rows[i]["unreadCount"].ToString()
                    };
                    bases.Add(moderatorHistoryUserModel);
                }
            }
            return bases;
        }
        /// <summary>
        /// Author : Zainab Rizvi
        /// Creation Date : 15 may 2019
        /// Purpose : This Method will return alim userid with their question avg response time.
        /// </summary>
        /// <returns>list of response time</returns>
        public static List<IResponseTimeModel> GetMessageResponseTime()
        {
            List<IResponseTimeModel> bases = new List<IResponseTimeModel>();
            Dictionary<string, string> strs = new Dictionary<string, string>();
            DataTable dataTable = new DataTable();
            dataTable = (new OleDBHelper()).GetData("[GETMESSAGERESPONSETIME]", strs);
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ResponseTimeModel responseTimeModel = new ResponseTimeModel()
                    {
                        UserID = dataTable.Rows[i]["USERID"].ToString(),
                        MessageCount = dataTable.Rows[i]["MESSAGECOUNT"].ToString(),
                        ResponseTime = dataTable.Rows[i]["RESPONSETIME"].ToString(),
                    };
                    bases.Add(responseTimeModel);
                }
            }
            return bases;
        }
        private static T ObjectFromDictionary<T>(IDictionary<string, object> dict)
        where T : class
        {
            Type type = typeof(T);
            T result = (T)Activator.CreateInstance(type);
            foreach (var item in dict)
            {
                type.GetProperty(item.Key).SetValue(result, item.Value, null);
            }
            return result;
        }
    }

}