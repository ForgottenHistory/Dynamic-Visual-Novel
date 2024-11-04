// Location.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Prompt", menuName = "LLM/Prompt")]
public class Prompt : ScriptableObject
{
    [TextArea(3, 10)]
    public string promptText;
}