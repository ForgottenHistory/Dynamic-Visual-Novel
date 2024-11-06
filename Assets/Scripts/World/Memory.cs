using UnityEngine;

public class Memory : IGetDescription
{
    public string dateTime; // The date and time the memory was created
    public string memory; // A short description of the memory
    public Memory( string dateTime, string memory)
    {
        this.memory = memory;
        this.dateTime = dateTime;
    }
    public string GetDescription()
    {
        return dateTime + ": " + memory;
    }
}
