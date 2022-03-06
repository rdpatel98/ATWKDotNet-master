using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHABS.MODELS
{
    public class BaseModel
    {

        private DateTime _createdDate;
        private DateTime _modifiedDate;
        private string _creatorUserId;
        private string _modifierUserId;
        private string _error;
        


        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
            }
        }
        public DateTime CreatedDate
        {
            get
            {
                return _createdDate;
            }
            set
            {
                _createdDate = value;
            }
        }
        public DateTime ModifiedDate
        {
            get
            {
                return _modifiedDate;
            }
            set
            {
                _modifiedDate = value;
            }
        }

        public string CreatorUserId
        {
            get
            {
                return _creatorUserId;
            }
            set
            {
                _creatorUserId = value;
            }
        }
        public string ModifierUserId
        {
            get
            {
                return _modifierUserId;
            }
            set
            {
                _modifierUserId = value;
            }
        }
    }
}