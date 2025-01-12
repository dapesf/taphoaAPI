namespace InterFace
{
    public class ResponseResult : IResponeseResult
    {
        public string codeRtr { get; set; }
        public string messageRtr { get; set; }

        public object dataRtn { get; set; }

        public ResponseResult(string codeRtr, string messageRtr)
        {
            this.codeRtr = codeRtr;
            this.messageRtr = messageRtr;
        }

        public ResponseResult(string codeRtr, string messageRtr, object dataRtn)
        {
            this.codeRtr = codeRtr;
            this.messageRtr = messageRtr;
            this.dataRtn = dataRtn;
        }
    }
}