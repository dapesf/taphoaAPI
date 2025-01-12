namespace InterFace
{
    public interface IResponeseResult
    {
        string codeRtr { get; set; }
        string messageRtr { get; set; }
        object dataRtn { get; set; }
    }
}