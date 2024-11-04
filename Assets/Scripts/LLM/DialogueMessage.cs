[System.Serializable]
public class DialogueMessage
{
    public string name;
    public string message;

    public DialogueMessage(string name, string message)
    {
        this.name = name;
        this.message = message;
    }

    public override string ToString()
    {
        return $"{name}: {message}";
    }
}