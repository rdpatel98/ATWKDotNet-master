using SHABS.COMMON;
using SHABS.DB;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SHABS.MODELS
{
	public class RedirectMessageModel
	{
		public string ById
		{
			get;
			set;
		}

		public string FromId
		{
			get;
			set;
		}

		public string FromUserName
		{
			get;
			set;
		}

		public string MessageId
		{
			get;
			set;
		}

		public string Reason
		{
			get;
			set;
		}

		public string ToId
		{
			get;
			set;
		}

		public RedirectMessageModel()
		{
		}

		public string Redirect()
		{
			Dictionary<string, string> strs = new Dictionary<string, string>()
			{
				{ "@messageId", this.MessageId },
				{ "@toId", this.ToId },
				{ "@byId", this.ById },
				{ "@remarks", this.Reason }
			};
			if (SingletonBase<DBFactory>.Current.GetDBHelper(0).InsertUpdateData("RedirectMessage", strs) > 0)
			{
				return "SUCCESS";
			}
			return "ERROR";
		}
	}
}