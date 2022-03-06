using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;
using SHABS.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHABS.MODELS
{
    public class MessageCountModel
    {
        public int TotalIncomingMessages { get; set; }
        public int TotalIncomingReadMessages { get; set; }
        public int TotalOutgoingMessages { get; set; }
        public int TotalOutgoingReadMessages { get; set; }

        public MessageCountModel()
        {
            TotalIncomingMessages = 0;
            TotalIncomingReadMessages = 0;
            TotalOutgoingMessages = 0;
            TotalOutgoingReadMessages = 0;

        }

        public static MessageCountModel GetMessageCount(string Direction, string username)
        {
            MessageCountModel temp = new MessageCountModel();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@username", username);
            parameters.Add("@indc", Direction.ToUpper().Trim());


            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("MessageCountByUsername", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    string direction = dt1.Rows[cntr]["Direction"].ToString();
                    string isread = dt1.Rows[cntr]["IsRead"].ToString();
                    if (direction == "IN")
                    {
                        if (isread == "Y")
                        {
                            temp.TotalIncomingReadMessages = Convert.ToInt32(dt1.Rows[cntr]["MessageCount"]);
                        }
                        else
                        {
                            temp.TotalIncomingMessages = Convert.ToInt32(dt1.Rows[cntr]["MessageCount"]);
                        }
                    }
                    else
                    {
                        if (isread == "Y")
                        {
                            temp.TotalOutgoingReadMessages = Convert.ToInt32(dt1.Rows[cntr]["MessageCount"]);
                        }
                        else
                        {
                            temp.TotalOutgoingMessages = Convert.ToInt32(dt1.Rows[cntr]["MessageCount"]);
                        }
                    }
                }
            }
            temp.TotalIncomingMessages += temp.TotalIncomingReadMessages;
            temp.TotalOutgoingMessages += temp.TotalOutgoingReadMessages;
            return temp;
        }
    }
    public class MessageModel : BaseModel
    {
        public MessageModel()
        {

        }

        public async Task<MessageModel> GetUserDataBtwAsync(string from, string to, string subject, string guid)
        {
            OleDBHelper oleDBHelper = new OleDBHelper();
            var data = await oleDBHelper.GetUserDataBtwAsync(from, to, Subject, guid);
            var model = new MessageModel();
            model.MessageID = data["ID"].ToString();
            model.Text = data["Text"].ToString();
            model.From = data["SentFrom"].ToString();
            model.To = data["SendTo"].ToString();
            model.IsReplied = data["IsReplied"] as string;
            model.IsRead = data["IsRead"] as string;
            model.CreatedDate = Convert.ToDateTime(data["createdDate"].ToString());
            model.IsVoice = data["IsVoice"] as string;
            model.Subject = data["Subject"] as string;
            model.MessageStatus = data["Status"].ToString();
            model.ayatollah = data["ayatollah"].ToString();


            try
            {
                model.ContentType = data["ContentType"].ToString();
                model.FileSize = data["FileSize"].ToString();
                model.FileThumbNail = data["FileThumbnailId"].ToString();
                model.FileContextText = data["FileContextText"].ToString();
                model.FileName = data["FileTitle"].ToString();
            }
            catch (Exception)
            {

                // throw;
            }
            return model;
        }

        public string UserID { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string MessageID { get; set; }
        public string MessageStatus { get; set; }
        public string MessageReplyID { get; set; }
        public string IsRead { get; set; }
        public string IsReplied { get; set; }
        public string FromName { get; set; }
        public string FromUserID { get; set; }
        public string ToName { get; set; }
        public string ToUserID { get; set; }
        public string FromImageID { get; set; }
        public string ToImageID { get; set; }

        public string IsVoice { get; set; }
        public string MessageType { get; set; }
        public string FileName { get; set; }
        public string FileID { get; set; }
        public string ayatollah { get; set; }
        public string FromUserType { get; set; }

        public bool IsMine { get; set; }
        public string ContentType { get; set; }
        public string FileSize { get; set; }
        // public string FileName { get; set; }
        public string FileThumbNail { get; set; }

        public string FileContextText { get; set; }
        public DateTime FromLastOnlineTime;
        public DateTime ToLastOnlineTime;

        public string CommonMsgCount { get; set; }
        public string ReplyCount { get; set; }
        public async Task<string> DeleteMessageAsync(MessageModel model)
        {
            FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
            var result = await firebaseDBHelper.DeleteMessageAsync(model.ToDictionary());
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public async Task<string> DeleteMessageToModeratorReviewAsync(MessageModel model)
        {
            FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
            var result = await firebaseDBHelper.DeleteMessageToModeratorReviewAsync(model.ToDictionary());
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public async Task<string> DeleteLatestSubjectAsync(MessageModel model)
        {
            FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
            var result = await firebaseDBHelper.DeleteLatestSubjectAsync(model.ToDictionary());
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public async Task<string> SaveMessageAsync(MessageModel model)
        {
            //int result = (new OleDBHelper()).InsertUpdateData("AddMessage", parameters);
            FirebaseDBHelper firebaseDB = new FirebaseDBHelper();
            int result = await firebaseDB.AddMessageToChatRoomAsync(new Dictionary<string, string>(model.ToDictionary()));
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public async Task<string> UpdateLatestMessageAsync(MessageModel model)
        {
 
            FirebaseDBHelper firebaseDB = new FirebaseDBHelper();
            var result = await firebaseDB.UpdateLatestSubject(new Dictionary<string, string>(model.ToDictionary()));
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public async Task<string> UpdateChatMessageAsync(MessageModel model)
        {

            FirebaseDBHelper firebaseDB = new FirebaseDBHelper();
            var result = await firebaseDB.UpdateMessageToChatRoomAsync(new Dictionary<string, string>(model.ToDictionary()));
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        private byte[] GetPDFThumbNail(byte[] data)
        {
            Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument(data);
            Image bmp = doc.SaveAsImage(0);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
        private byte[] CreateImage(string text, string inputImage)
        {

            string baseImage = @"/9j/4AAQSkZJRgABAQEAYABgAAD/4REQRXhpZgAATU0AKgAAAAgABQESAAMAAAABAAEAAAE7AAIAAAATAAAIVodpAAQAAAABAAAIapydAAEAAAAmAAAQ4uocAAcAAAgMAAAASgAAAAAc6gAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFNheWVkIFNoYWJpZSBBYmJhcwAAAAWQAwACAAAAFAAAELiQBAACAAAAFAAAEMySkQACAAAAAzk2AACSkgACAAAAAzk2AADqHAAHAAAIDAAACKwAAAAAHOoAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAyMDE4OjAyOjIwIDAwOjAyOjUwADIwMTg6MDI6MjAgMDA6MDI6NTAAAABTAGEAeQBlAGQAIABTAGgAYQBiAGkAZQAgAEEAYgBiAGEAcwAAAP/hCyVodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvADw/eHBhY2tldCBiZWdpbj0n77u/JyBpZD0nVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkJz8+DQo8eDp4bXBtZXRhIHhtbG5zOng9ImFkb2JlOm5zOm1ldGEvIj48cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPjxyZGY6RGVzY3JpcHRpb24gcmRmOmFib3V0PSJ1dWlkOmZhZjViZGQ1LWJhM2QtMTFkYS1hZDMxLWQzM2Q3NTE4MmYxYiIgeG1sbnM6ZGM9Imh0dHA6Ly9wdXJsLm9yZy9kYy9lbGVtZW50cy8xLjEvIi8+PHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9InV1aWQ6ZmFmNWJkZDUtYmEzZC0xMWRhLWFkMzEtZDMzZDc1MTgyZjFiIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iPjx4bXA6Q3JlYXRlRGF0ZT4yMDE4LTAyLTIwVDAwOjAyOjUwLjk2MjwveG1wOkNyZWF0ZURhdGU+PC9yZGY6RGVzY3JpcHRpb24+PHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9InV1aWQ6ZmFmNWJkZDUtYmEzZC0xMWRhLWFkMzEtZDMzZDc1MTgyZjFiIiB4bWxuczpkYz0iaHR0cDovL3B1cmwub3JnL2RjL2VsZW1lbnRzLzEuMS8iPjxkYzpjcmVhdG9yPjxyZGY6U2VxIHhtbG5zOnJkZj0iaHR0cDovL3d3dy53My5vcmcvMTk5OS8wMi8yMi1yZGYtc3ludGF4LW5zIyI+PHJkZjpsaT5TYXllZCBTaGFiaWUgQWJiYXM8L3JkZjpsaT48L3JkZjpTZXE+DQoJCQk8L2RjOmNyZWF0b3I+PC9yZGY6RGVzY3JpcHRpb24+PC9yZGY6UkRGPjwveDp4bXBtZXRhPg0KICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAg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";

            if (string.IsNullOrEmpty(inputImage.Trim()))
            {
                baseImage = inputImage.Trim();
            }
            byte[] fileBytes = Convert.FromBase64String(baseImage.Replace(@"\", string.Empty));
            Bitmap bitMapImage;
            using (var ms = new System.IO.MemoryStream(fileBytes))
            {
                bitMapImage = new Bitmap(ms);
            }
            // Bitmap bitMapImage = new System.Drawing.Bitmap("newImage.jpg");
            if (text.Length > 43)
            {
                text = text.Substring(0, 40) + "...";
            }
            Graphics graphicImage = Graphics.FromImage(bitMapImage);
            graphicImage.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphicImage.DrawString(text, new Font("Arial", 10, FontStyle.Bold), SystemBrushes.WindowText, new Point(100, 50));
            //I am drawing a oval around my text.
            //  graphicImage.DrawArc(new Pen(Color.Red, 3), 90, 235, 150, 50, 0, 360);
            System.IO.MemoryStream ms1 = new System.IO.MemoryStream();
            bitMapImage.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
            graphicImage.Dispose();
            bitMapImage.Dispose();
            return ms1.ToArray();
        }

        private byte[] GetFileThumbNail(byte[] data, string fileName, string contentType)
        {
            string filePath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads"), fileName);
            if (contentType == "application/pdf")
            {
                return GetPDFThumbNail(data);
            }
            //if (contentType == "application/vnd.ms-word")
            //{
            //    return CreateImage(fileName, System.Configuration.ConfigurationManager.AppSettings["docXBaseImage"]);
            //}
            System.IO.File.WriteAllBytes(filePath, data);
            string thumbnailPath = filePath + ".jpg";
            ShellFile shellFile = Microsoft.WindowsAPICodePack.Shell.ShellFile.FromFilePath(filePath);
            Bitmap shellThumb = shellFile.Thumbnail.LargeBitmap;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            shellThumb.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            System.IO.File.Delete(filePath);
            return ms.ToArray();
        }

        public string SaveDocumentMessage(string contenttype, byte[] data)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            byte[] thumbnail = GetFileThumbNail(data, FileName, contenttype);
            parameters.Add("@SendTo", To);
            parameters.Add("@SentFrom", From);
            if (MessageReplyID == null)
            {
                MessageReplyID = "0";
            }
            parameters.Add("@ReplyToID", MessageReplyID);
            parameters.Add("@Status", MessageStatus);
            parameters.Add("@IsRead", "N");
            parameters.Add("@createdBy", UserID);
            parameters.Add("@FileName", FileName);
            parameters.Add("@ContentType", contenttype);
            parameters.Add("@Data", data);
            parameters.Add("@Subject", Subject);
            parameters.Add("@Ayatollah", ayatollah);
            parameters.Add("@thumbNail", thumbnail);
            if (string.IsNullOrEmpty(FileContextText))
            {
                FileContextText = string.Empty;
            }
            parameters.Add("@fileContextText", FileContextText.Trim());
            var dt = (new OleDBHelper()).ExecuteScalar("AddDocumentMessage", parameters);
            if (dt != null)
            {
                FileID = dt.ToString();
                System.IO.File.WriteAllBytes(System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads"), FileID + "_thumbnail.jpg"), thumbnail);
                return FileID;
            }
            return "ERROR";

        }

        public string SaveVoiceMessage(string contenttype, byte[] data)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@SendTo", To);
            parameters.Add("@SentFrom", From);
            if (MessageReplyID == null)
            {
                MessageReplyID = "0";
            }
            parameters.Add("@ReplyToID", MessageReplyID);
            parameters.Add("@Status", MessageStatus);
            parameters.Add("@IsRead", "N");
            parameters.Add("@createdBy", UserID);
            parameters.Add("@FileName", FileName);
            parameters.Add("@ContentType", contenttype);
            parameters.Add("@Data", data);
            parameters.Add("@Subject", Subject);
            parameters.Add("@Ayatollah", ayatollah);
            var dt = (new OleDBHelper()).ExecuteScalar("AddVoiceMessage", parameters);
            if (dt != null)
            {
                FileID = dt.ToString();
                return FileID;
            }
            return "ERROR";

        }

        public string MarkMultipleRead()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@ID", MessageID.Replace('~', ','));
            parameters.Add("@IsRead", IsRead);
            parameters.Add("@userID", UserID);

            int result = (new OleDBHelper()).InsertUpdateData("UpdateMultipleRead", parameters);
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }


        public static string MarkMultipleReadCommonMessages(string MessageID, string UserID)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@ID", MessageID.Replace('~', ','));
            parameters.Add("@userID", UserID);
            DataTable dt1 = new DataTable();//RegisterUser
            int result = 0;
            dt1 = (new OleDBHelper()).GetData("UpdateMultipleReadCommonMessages", parameters);
            if (dt1.Rows.Count > 0)
            {
                result = System.Convert.ToInt16(dt1.Rows[0][0].ToString());
            }
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }


        public string UpdateMultipleReply()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@ID", MessageID.Replace('~', ','));
            parameters.Add("@IsReplied", IsReplied);
            parameters.Add("@userID", UserID);

            int result = (new OleDBHelper()).InsertUpdateData("UpdateMultipleReply", parameters);
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }



        public string MarkAsRead()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@ID", MessageID);
            parameters.Add("@IsRead", IsRead);
            parameters.Add("@userID", UserID);

            int result = (new OleDBHelper()).InsertUpdateData("UpdateRead", parameters);
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public string UpdateMessage()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@ID", MessageID);
            parameters.Add("@Status", MessageStatus);
            parameters.Add("@userID", UserID);

            int result = (new OleDBHelper()).InsertUpdateData("UpdateMessage", parameters);
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }

        }



        public static List<MessageModel> GetMessagesForModerator(string status)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<MessageModel> temp = new List<MessageModel>();
            parameters.Add("@Status", status);

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetMessageByStatus", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();
                    x = FillModel(dt1.Rows[cntr]);

                    temp.Add(x);
                }
            }
            return temp;

        }

        public static MessageModel FillModel(DataRow dr)
        {
            MessageModel x = new MessageModel();

            x.MessageID = dr["ID"].ToString();
            x.Text = dr["Text"].ToString();
            x.From = dr["SentFrom"].ToString();
            x.FromName = dr["FromName"].ToString();
            x.ToName = dr["ToName"].ToString();
            x.To = dr["SendTo"].ToString();
            x.IsReplied = dr["IsReplied"] as string;
            x.IsRead = dr["IsRead"] as string;
            x.CreatedDate = Convert.ToDateTime(dr["createdDate"].ToString());
            x.IsVoice = dr["IsVoice"] as string;
            x.Subject = dr["Subject"] as string;
            x.FromImageID = dr["FromImageID"].ToString();
            x.ToImageID = dr["ToImageID"].ToString();
            x.MessageStatus = dr["Status"].ToString();
            x.ayatollah = dr["Ayatollah"].ToString();
            try
            {
                x.ContentType = dr["ContentType"].ToString();
                x.FileSize = dr["FileSize"].ToString();
                x.FileThumbNail = dr["FileThumbnailId"].ToString();
                x.FileName = dr["FileTitle"].ToString();
                x.FileContextText = dr["FileContextText"].ToString();
                String user_from = x.From;
                String user_to = x.To;
                UserDetailsModel fromModel = UserDetailsModel.GetUserDetailsByUsernameAsync(user_from);
                UserDetailsModel toModel = UserDetailsModel.GetUserDetailsByUsernameAsync(user_to); 

                x.FromUserID = fromModel.UserID;
                x.ToUserID = toModel.UserID;
                x.FromUserType = fromModel.UserModel.UserType;
            }
            catch (Exception)
            {

                // throw;
            }
            return x;
        }

        public static List<MessageModel> GetIncomingMessages(string userID)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<MessageModel> temp = new List<MessageModel>();
            parameters.Add("@userID", userID);

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetIncomingMessagesByUserID", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();
                    x = FillModel(dt1.Rows[cntr]);
                    temp.Add(x);
                }
            }
            return temp;
        }


        public static List<MessageModel> GetOutGoingMessages(string userID)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<MessageModel> temp = new List<MessageModel>();
            parameters.Add("@userID", userID);

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetOutGoingMessagesByUserID", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();
                    x = FillModel(dt1.Rows[cntr]);
                    temp.Add(x);
                }
            }
            return temp;
        }

        public static List<MessageModel> GetConversationMessages(string me, string you)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<MessageModel> temp = new List<MessageModel>();
            parameters.Add("@Me", me);
            parameters.Add("@You", you);

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetConversationTree", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();

                    x = FillModel(dt1.Rows[cntr]);
                    x.MessageID = dt1.Rows[cntr]["ID"].ToString();
                    x.Text = dt1.Rows[cntr]["Text"].ToString();
                    x.From = dt1.Rows[cntr]["SentFrom"].ToString();
                    x.FromName = dt1.Rows[cntr]["FromName"].ToString();
                    x.ToName = dt1.Rows[cntr]["ToName"].ToString();
                    x.To = dt1.Rows[cntr]["SendTo"].ToString();
                    x.IsReplied = dt1.Rows[cntr]["IsReplied"] as string;
                    x.IsRead = dt1.Rows[cntr]["IsRead"] as string;
                    x.CreatedDate = Convert.ToDateTime(dt1.Rows[cntr]["createdDate"].ToString());
                    x.IsVoice = dt1.Rows[cntr]["IsVoice"] as string;
                    x.Subject = dt1.Rows[cntr]["Subject"] as string;
                    x.FromImageID = dt1.Rows[cntr]["FromImageID"].ToString();
                    x.ToImageID = dt1.Rows[cntr]["ToImageID"].ToString();
                    x.MessageStatus = dt1.Rows[cntr]["Status"].ToString();
                    x.ayatollah = dt1.Rows[cntr]["Ayatollah"].ToString();
                    x.FromUserType = dt1.Rows[cntr]["FromUserType"].ToString();
                    try
                    {
                        x.ContentType = dt1.Rows[cntr]["ContentType"].ToString();
                        x.FileSize = dt1.Rows[cntr]["FileSize"].ToString();
                        x.FileThumbNail = dt1.Rows[cntr]["FileThumbnailId"].ToString();
                        x.FileContextText = dt1.Rows[cntr]["FileContextText"].ToString();
                        x.FileName = dt1.Rows[cntr]["FileTitle"].ToString();
                    }
                    catch (Exception)
                    {

                        // throw;
                    }
                    temp.Add(x);
                }
            }
            return temp;
        }

        public static List<MessageModel> GetConversationMessagesById(string meId, string youId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<MessageModel> temp = new List<MessageModel>();
            parameters.Add("@MeId", meId);
            parameters.Add("@YouId", youId);

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetConversationTreeById", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();

                    x = FillModel(dt1.Rows[cntr]);
                    x.MessageID = dt1.Rows[cntr]["ID"].ToString();
                    x.Text = dt1.Rows[cntr]["Text"].ToString();
                    x.From = dt1.Rows[cntr]["SentFrom"].ToString();
                    x.FromName = dt1.Rows[cntr]["FromName"].ToString();
                    x.ToName = dt1.Rows[cntr]["ToName"].ToString();
                    x.To = dt1.Rows[cntr]["SendTo"].ToString();
                    x.IsReplied = dt1.Rows[cntr]["IsReplied"] as string;
                    x.IsRead = dt1.Rows[cntr]["IsRead"] as string;
                    x.CreatedDate = Convert.ToDateTime(dt1.Rows[cntr]["createdDate"].ToString());
                    x.IsVoice = dt1.Rows[cntr]["IsVoice"] as string;
                    x.Subject = dt1.Rows[cntr]["Subject"] as string;
                    x.FromImageID = dt1.Rows[cntr]["FromImageID"].ToString();
                    x.ToImageID = dt1.Rows[cntr]["ToImageID"].ToString();
                    x.MessageStatus = dt1.Rows[cntr]["Status"].ToString();
                    x.ayatollah = dt1.Rows[cntr]["Ayatollah"].ToString();
                    x.FromUserType = dt1.Rows[cntr]["FromUserType"].ToString();
                    x.IsMine = dt1.Rows[cntr]["IsMine"].ToString() == "Y";
                    try
                    {
                        x.ContentType = dt1.Rows[cntr]["ContentType"].ToString();
                        x.FileSize = dt1.Rows[cntr]["FileSize"].ToString();
                        x.FileThumbNail = dt1.Rows[cntr]["FileThumbnailId"].ToString();
                        x.FileContextText = dt1.Rows[cntr]["FileContextText"].ToString();
                        x.FileName = dt1.Rows[cntr]["FileTitle"].ToString();
                    }
                    catch (Exception)
                    {

                        // throw;
                    }
                    temp.Add(x);
                }
            }
            return temp;
        }


        public static List<MessageModel> Inbox(string me, string you)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<MessageModel> temp = new List<MessageModel>();
            parameters.Add("@Me", me);
            parameters.Add("@You", you);

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetIncomingMessagesByUserName", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();
                    x = FillModel(dt1.Rows[cntr]);
                    temp.Add(x);
                }
            }
            return temp;
        }

        public async Task<string> DeleteMessageAsync(List<MessageModel> model)
        {
            FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
            var result = await firebaseDBHelper.DeleteMessageAsync(model.ToDictionary());
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "";
            }
        }

        public static List<MessageModel> GetUnansweredQueriesByUsername(string scholar, string user)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<MessageModel> temp = new List<MessageModel>();
            parameters.Add("@usersUsername", user);
            parameters.Add("@scholarUsername", scholar);

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetUnansweredQueriesByUsername", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();
                    x = FillModel(dt1.Rows[cntr]);
                    temp.Add(x);
                }
            }
            return temp;
        }

        public static List<MessageModel> OutBox(string me, string you)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<MessageModel> temp = new List<MessageModel>();
            parameters.Add("@Me", me);
            parameters.Add("@You", you);

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetOutgoingMessagesByUserName", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();
                    x = FillModel(dt1.Rows[cntr]);
                    temp.Add(x);
                }
            }
            return temp;
        }

        public static FileHelper GetFile(string indentifier)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@ID", indentifier);

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetData", parameters);
            if (dt1.Rows.Count > 0)
            {
                FileHelper temp = new FileHelper();
                temp.Data = dt1.Rows[0]["Data"] as byte[];
                temp.FileName = dt1.Rows[0]["FileName"].ToString();
                temp.ContentType = dt1.Rows[0]["ContentType"].ToString();
                return temp;
            }
            return null;
        }



        public static List<MessageModel> GetAllMessages()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<MessageModel> temp = new List<MessageModel>();

            DataTable dt1 = new DataTable();//RegisterUser
            dt1 = (new OleDBHelper()).GetData("GetAllMessages");
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();
                    x = FillModel(dt1.Rows[cntr]);
                    temp.Add(x);
                }
            }
            return temp;

        }

        public static List<MessageModel> GetAllMessagesById(string userId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@userId", userId);
            List<MessageModel> temp = new List<MessageModel>();

            DataTable dt1 = new DataTable();
            dt1 = (new OleDBHelper()).GetData("GetAllMessagesById", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();
                    x = FillModel(dt1.Rows[cntr]);
                    temp.Add(x);
                }
            }
            return temp;

        }

        public static List<MessageModel> GetMessageByKeyWord(string me, string keyword)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<MessageModel> temp = new List<MessageModel>();
            parameters.Add("@Me", me);
            parameters.Add("@KeyWord", keyword);

            DataTable dt1 = new DataTable();
            dt1 = (new OleDBHelper()).GetData("GetConversationByKeyWord", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();

                    x = FillModel(dt1.Rows[cntr]);
                    x.MessageID = dt1.Rows[cntr]["ID"].ToString();
                    x.Text = dt1.Rows[cntr]["Text"].ToString();
                    x.From = dt1.Rows[cntr]["SentFrom"].ToString();
                    x.FromName = dt1.Rows[cntr]["FromName"].ToString();
                    x.ToName = dt1.Rows[cntr]["ToName"].ToString();
                    x.To = dt1.Rows[cntr]["SendTo"].ToString();
                    x.IsReplied = dt1.Rows[cntr]["IsReplied"] as string;
                    x.IsRead = dt1.Rows[cntr]["IsRead"] as string;
                    x.CreatedDate = Convert.ToDateTime(dt1.Rows[cntr]["createdDate"].ToString());
                    x.IsVoice = dt1.Rows[cntr]["IsVoice"] as string;
                    x.Subject = dt1.Rows[cntr]["Subject"] as string;
                    x.FromImageID = dt1.Rows[cntr]["FromImageID"].ToString();
                    x.ToImageID = dt1.Rows[cntr]["ToImageID"].ToString();
                    x.MessageStatus = dt1.Rows[cntr]["Status"].ToString();
                    x.ayatollah = dt1.Rows[cntr]["Ayatollah"].ToString();
                    x.FromUserType = dt1.Rows[cntr]["FromUserType"].ToString();
                    try
                    {
                        x.ContentType = dt1.Rows[cntr]["ContentType"].ToString();
                        x.FileSize = dt1.Rows[cntr]["FileSize"].ToString();
                        x.FileThumbNail = dt1.Rows[cntr]["FileThumbnailId"].ToString();
                        x.FileContextText = dt1.Rows[cntr]["FileContextText"].ToString();
                        x.FileName = dt1.Rows[cntr]["FileTitle"].ToString();
                    }
                    catch (Exception)
                    {

                        // throw;
                    }
                    temp.Add(x);
                }
            }
            return temp;
        }




        public static List<MessageModel> GetLatestMessagesOfUser(string me)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<MessageModel> temp = new List<MessageModel>();
            parameters.Add("@Me", me);

            DataTable dt1 = new DataTable();
            dt1 = (new OleDBHelper()).GetData("GetLatestConversation", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();

                    x = FillModel(dt1.Rows[cntr]);
                    x.MessageID = dt1.Rows[cntr]["ID"].ToString();
                    x.Text = dt1.Rows[cntr]["Text"].ToString();
                    x.From = dt1.Rows[cntr]["SentFrom"].ToString();
                    x.FromName = dt1.Rows[cntr]["FromName"].ToString();
                    x.ToName = dt1.Rows[cntr]["ToName"].ToString();
                    x.To = dt1.Rows[cntr]["SendTo"].ToString();
                    x.IsReplied = dt1.Rows[cntr]["IsReplied"] as string;
                    x.IsRead = dt1.Rows[cntr]["IsRead"] as string;
                    x.CreatedDate = Convert.ToDateTime(dt1.Rows[cntr]["createdDate"].ToString());
                    x.IsVoice = dt1.Rows[cntr]["IsVoice"] as string;
                    x.Subject = dt1.Rows[cntr]["Subject"] as string;
                    x.FromImageID = dt1.Rows[cntr]["FromImageID"].ToString();
                    x.ToImageID = dt1.Rows[cntr]["ToImageID"].ToString();
                    x.MessageStatus = dt1.Rows[cntr]["Status"].ToString();
                    x.ayatollah = dt1.Rows[cntr]["Ayatollah"].ToString();
                    x.FromUserType = dt1.Rows[cntr]["FromUserType"].ToString();
                    try
                    {
                        x.ContentType = dt1.Rows[cntr]["ContentType"].ToString();
                        x.FileSize = dt1.Rows[cntr]["FileSize"].ToString();
                        x.FileThumbNail = dt1.Rows[cntr]["FileThumbnailId"].ToString();
                        x.FileContextText = dt1.Rows[cntr]["FileContextText"].ToString();
                        x.FileName = dt1.Rows[cntr]["FileTitle"].ToString();
                    }
                    catch (Exception)
                    {

                        // throw;
                    }
                    temp.Add(x);
                }
            }
            return temp;
        }



        public static List<MessageModel> GetLatestSubjectMsg(string me)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<MessageModel> temp = new List<MessageModel>();
            parameters.Add("@Me", me);

            DataTable dt1 = new DataTable();
            dt1 = (new OleDBHelper()).GetData("GetLatestConversationBySubject", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();

                    x = FillModel(dt1.Rows[cntr]);
                    x.MessageID = dt1.Rows[cntr]["ID"].ToString();
                    x.Text = dt1.Rows[cntr]["Text"].ToString();
                    x.From = dt1.Rows[cntr]["SentFrom"].ToString();
                    x.FromName = dt1.Rows[cntr]["FromName"].ToString();
                    x.ToName = dt1.Rows[cntr]["ToName"].ToString();
                    x.To = dt1.Rows[cntr]["SendTo"].ToString();
                    x.IsReplied = dt1.Rows[cntr]["IsReplied"] as string;
                    x.IsRead = dt1.Rows[cntr]["IsRead"] as string;
                    x.CreatedDate = Convert.ToDateTime(dt1.Rows[cntr]["createdDate"].ToString());
                    x.IsVoice = dt1.Rows[cntr]["IsVoice"] as string;
                    x.Subject = dt1.Rows[cntr]["Subject"] as string;
                    x.FromImageID = dt1.Rows[cntr]["FromImageID"].ToString();
                    x.ToImageID = dt1.Rows[cntr]["ToImageID"].ToString();
                    x.MessageStatus = dt1.Rows[cntr]["Status"].ToString();
                    x.ayatollah = dt1.Rows[cntr]["Ayatollah"].ToString();
                    x.FromUserType = dt1.Rows[cntr]["FromUserType"].ToString();
                    x.FromLastOnlineTime = (DateTime)dt1.Rows[cntr]["FromLastOnlineTime"];
                    x.ToLastOnlineTime = (DateTime)dt1.Rows[cntr]["ToLastOnlineTime"];
                    x.ReplyCount = dt1.Rows[cntr]["ReplyCount"].ToString();
                    x.CommonMsgCount = dt1.Rows[cntr]["CommonMsgCount"].ToString();


                    try
                    {
                        x.ContentType = dt1.Rows[cntr]["ContentType"].ToString();
                        x.FileSize = dt1.Rows[cntr]["FileSize"].ToString();
                        x.FileThumbNail = dt1.Rows[cntr]["FileThumbnailId"].ToString();
                        x.FileContextText = dt1.Rows[cntr]["FileContextText"].ToString();
                        x.FileName = dt1.Rows[cntr]["FileTitle"].ToString();
                    }
                    catch (Exception)
                    {

                        // throw;
                    }
                    temp.Add(x);
                }
            }
            return temp;
        }

        public static List<MessageModel> GetUserMsgForSubject(string myUsername, string MsgID)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<MessageModel> temp = new List<MessageModel>();
            parameters.Add("@Me", myUsername);
            parameters.Add("@MsgID", MsgID);
            DataTable dt1 = new DataTable();
            dt1 = (new OleDBHelper()).GetData("GetUserMsgForSubject", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();

                    x = FillModel(dt1.Rows[cntr]);
                    x.MessageID = dt1.Rows[cntr]["ID"].ToString();
                    x.Text = dt1.Rows[cntr]["Text"].ToString();
                    x.FromName = dt1.Rows[cntr]["FromName"].ToString();
                    x.ToName = dt1.Rows[cntr]["ToName"].ToString();
                    x.IsReplied = dt1.Rows[cntr]["IsReplied"] as string;
                    x.IsRead = dt1.Rows[cntr]["IsRead"] as string;
                    x.CreatedDate = Convert.ToDateTime(dt1.Rows[cntr]["createdDate"].ToString());
                    x.IsVoice = dt1.Rows[cntr]["IsVoice"] as string;
                    x.Subject = dt1.Rows[cntr]["Subject"] as string;
                    x.FromImageID = dt1.Rows[cntr]["FromImageID"].ToString();
                    x.ToImageID = dt1.Rows[cntr]["ToImageID"].ToString();
                    x.MessageStatus = dt1.Rows[cntr]["Status"].ToString();
                    x.ayatollah = dt1.Rows[cntr]["Ayatollah"].ToString();


                    try
                    {
                        x.ContentType = dt1.Rows[cntr]["ContentType"].ToString();
                        x.FileSize = dt1.Rows[cntr]["FileSize"].ToString();
                        x.FileThumbNail = dt1.Rows[cntr]["FileThumbnailId"].ToString();
                        x.FileContextText = dt1.Rows[cntr]["FileContextText"].ToString();
                        x.FileName = dt1.Rows[cntr]["FileTitle"].ToString();
                    }
                    catch (Exception)
                    {

                        // throw;
                    }
                    temp.Add(x);
                }
            }
            return temp;
        }



        public static async Task<MessageModel> GetMsgByIdAsync(string MsgID)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            List<MessageModel> temp = new List<MessageModel>();
            // parameters.Add("@Me", myUsername);
            parameters.Add("@MsgID", MsgID.Replace('~', ','));
            DataTable dt1 = new DataTable();
            dt1 = (new OleDBHelper()).GetData("GetMsgById", parameters);
            if (dt1.Rows.Count > 0)
            {
                for (int cntr = 0; cntr < dt1.Rows.Count; cntr++)
                {
                    MessageModel x = new MessageModel();

                    x = FillModel(dt1.Rows[cntr]);
                    x.MessageID = dt1.Rows[cntr]["ID"].ToString();
                    x.Text = dt1.Rows[cntr]["Text"].ToString();
                    x.From = dt1.Rows[cntr]["SentFrom"].ToString();
                    x.FromName = dt1.Rows[cntr]["FromName"].ToString();
                    x.ToName = dt1.Rows[cntr]["ToName"].ToString();
                    x.To = dt1.Rows[cntr]["SendTo"].ToString();
                    x.IsReplied = dt1.Rows[cntr]["IsReplied"] as string;
                    x.IsRead = dt1.Rows[cntr]["IsRead"] as string;
                    x.CreatedDate = Convert.ToDateTime(dt1.Rows[cntr]["createdDate"].ToString());
                    x.IsVoice = dt1.Rows[cntr]["IsVoice"] as string;
                    x.Subject = dt1.Rows[cntr]["Subject"] as string;
                    x.FromImageID = dt1.Rows[cntr]["FromImageID"].ToString();
                    x.ToImageID = dt1.Rows[cntr]["ToImageID"].ToString();
                    x.MessageStatus = dt1.Rows[cntr]["Status"].ToString();
                    x.ayatollah = dt1.Rows[cntr]["Ayatollah"].ToString();


                    try
                    {
                        x.ContentType = dt1.Rows[cntr]["ContentType"].ToString();
                        x.FileSize = dt1.Rows[cntr]["FileSize"].ToString();
                        x.FileThumbNail = dt1.Rows[cntr]["FileThumbnailId"].ToString();
                        x.FileContextText = dt1.Rows[cntr]["FileContextText"].ToString();
                        x.FileName = dt1.Rows[cntr]["FileTitle"].ToString();
                    }
                    catch (Exception)
                    {

                        // throw;
                    }
                    temp.Add(x);
                }
            }
            return temp.First();
        }
        public async Task<MessageModel> GetJustSentMessageAsync(string guid)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            MessageModel x = new MessageModel();
            parameters.Add("@Me", From);
            parameters.Add("@To", To);
            parameters.Add("@Subject", Subject);
            FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
            await firebaseDBHelper.WriteLog(guid, "GetJustSentMessageData", JsonConvert.SerializeObject(parameters));
            DataTable dt1 = new DataTable();
            dt1 = (new OleDBHelper()).GetData("GetJustSentMessage", parameters);
            int cntr = 0;
            await firebaseDBHelper.WriteLog(guid, "GetJustSentMessageDataCount", dt1.Rows.Count.ToString());
            if (dt1.Rows.Count > 0)
            {
                x = FillModel(dt1.Rows[cntr]);
                x.MessageID = dt1.Rows[cntr]["ID"].ToString();
                x.Text = dt1.Rows[cntr]["Text"].ToString();
                x.From = dt1.Rows[cntr]["SentFrom"].ToString();
                x.FromName = dt1.Rows[cntr]["FromName"].ToString();
                x.ToName = dt1.Rows[cntr]["ToName"].ToString();
                x.To = dt1.Rows[cntr]["SendTo"].ToString();
                x.IsReplied = dt1.Rows[cntr]["IsReplied"] as string;
                x.IsRead = dt1.Rows[cntr]["IsRead"] as string;
                x.CreatedDate = Convert.ToDateTime(dt1.Rows[cntr]["createdDate"].ToString());
                x.IsVoice = dt1.Rows[cntr]["IsVoice"] as string;
                x.Subject = dt1.Rows[cntr]["Subject"] as string;
                x.FromImageID = dt1.Rows[cntr]["FromImageID"].ToString();
                x.ToImageID = dt1.Rows[cntr]["ToImageID"].ToString();
                x.MessageStatus = dt1.Rows[cntr]["Status"].ToString();
                x.ayatollah = dt1.Rows[cntr]["Ayatollah"].ToString();


                try
                {
                    x.ContentType = dt1.Rows[cntr]["ContentType"].ToString();
                    x.FileSize = dt1.Rows[cntr]["FileSize"].ToString();
                    x.FileThumbNail = dt1.Rows[cntr]["FileThumbnailId"].ToString();
                    x.FileContextText = dt1.Rows[cntr]["FileContextText"].ToString();
                    x.FileName = dt1.Rows[cntr]["FileTitle"].ToString();
                }
                catch (Exception)
                {

                    // throw;
                }

            }
            return x;
        }

        public static string UpdateIsReply(String MessageID, String IsReplied, String UserID, String ScholarId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@ID", MessageID.Replace('~', ','));
            parameters.Add("@IsReplied", IsReplied);
            parameters.Add("@userID", UserID);
            parameters.Add("@ScholarId", ScholarId);

            int result = (new OleDBHelper()).InsertUpdateData("UpdateMultipleReplyByMod", parameters);
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }

        public static string DeleteMessageByID(string me, string msgID)//EmailID and Message ID
        {
            string result = string.Empty;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            List<MessageModel> temp = new List<MessageModel>();
            parameters.Add("@Me", me);
            parameters.Add("@MsgId", msgID);
            try
            {
                DataTable dt1 = new DataTable();
                dt1 = (new OleDBHelper()).GetData("DeleteMessage", parameters);
                if (dt1.Rows.Count > 0)
                {
                    result = dt1.Rows[0][0].ToString();
                }
            }
            catch (Exception)
            {
                result = "Error";
            }
            return result;
        }

        public async Task<string> SaveMessageAsync(string guid)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("@Text", Text);
            parameters.Add("@SendTo", To);
            parameters.Add("@SentFrom", From);
            if (MessageReplyID == null)
            {
                MessageReplyID = "0";
            }
            parameters.Add("@ReplyToID", MessageReplyID);
            parameters.Add("@Status", MessageStatus);
            parameters.Add("@IsRead", "N");
            parameters.Add("@createdBy", UserID);
            parameters.Add("@Subject", Subject);
            parameters.Add("@Ayatollah", ayatollah);
            int result = await (new OleDBHelper()).InsertUpdateDataAsync("AddMessage", parameters, guid);
            FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
            await firebaseDBHelper.WriteLog(guid, "SaveMessageAsync", result.ToString());
            await firebaseDBHelper.WriteLog(guid, "SaveMessageAsyncData", JsonConvert.SerializeObject(parameters));
            if (result > 0)
            {
                return "SUCCESS";
            }
            else
            {
                return "ERROR";
            }
        }
    }
    public static class ObjectToDictionaryHelper
    {
        public static IDictionary<string, string> ToDictionary(this object source)
        {
            return source.ToDictionary<string>();
        }

        public static IDictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null)
                ThrowExceptionWhenSourceArgumentIsNull();

            var dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToDictionary<T>(property, source, dictionary);
            return dictionary;
        }

        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value))
                dictionary.Add(property.Name, (T)value);
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
        }
    }
}
