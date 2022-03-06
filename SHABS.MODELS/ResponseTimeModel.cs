namespace SHABS.MODELS
{
    /*Created By Zainab Rizvi for Avg Message Response Time*/
    public class ResponseTimeModel : IResponseTimeModel
    {
        public string UserID;
        public string MessageCount;
        public string ResponseTime;
    }
    public interface IResponseTimeModel { }
}
