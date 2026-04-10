public class Method
{
    public List<(string signature, string text)> Parameters { get; } = new();
    public string ReturnsText { get; private set; } = "";
    public List<string> Exceptions { get; } = new();

    public Method Parameter(string signature, string text)
    {
        Parameters.Add((signature, text));
        return this;
    }

    public Method Returns(string text)
    {
        ReturnsText = text;
        return this;
    }

    public Method Exception(string text)
    {
        Exceptions.Add(text);
        return this;
    }
}