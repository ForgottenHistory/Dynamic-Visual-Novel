/// <summary>
/// Interface for objects that have a description.
/// Create a string that describes the object in a way that an LLM can understand.
/// </summary>
public interface IGetDescription
{
    public string GetDescription();
}