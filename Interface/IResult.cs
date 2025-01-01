namespace InterFace
{
    public interface IResponeseResult
    {
        string codeRtr { get; set; }
        string messageRtr { get; set; }
        object data { get; set; }
    }
}