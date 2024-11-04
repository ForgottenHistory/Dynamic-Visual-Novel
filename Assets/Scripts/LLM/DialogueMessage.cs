[System.Serializable]
public class DialogueMessage
{
    public string sender;
    public string message;

    public DialogueMessage(string name, string message)
    {
        this.sender = name;
        this.message = message;
    }

    public override string ToString()
    {
        return $"{sender}: {message}";
    }
}