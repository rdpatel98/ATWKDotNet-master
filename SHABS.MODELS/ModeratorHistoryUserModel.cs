using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.MODELS
{
    public class ModeratorHistoryUserModel : IBase
    {
        public string UsersName;
        public string UsersUserName;
        public string UsersUserID;
        public string UsersImageID;

        public string ScholarsName;
        public string ScholarsUserName;
        public string ScholarsUserID;
        public string ScholarsImageID;

        public string UnreadMessageCount { get; set; }
    }

    public interface IBase { }
}
