using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.MODELS
{
    public class BaseMessageModel
    {
        public class Root
        {
            public string Key { get; set; }
            public MessageModel Value { get; set; }
        }

    }
}
