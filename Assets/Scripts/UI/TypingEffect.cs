using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

public class TypingEffect : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 0.05f;
    
    private TextMeshProUGUI targetText;
    private Coroutine currentTypingCoroutine;
    private string currentFullText = "";

    public UnityEvent onTypingComplete;

    private void Awake()
    {
        targetText = GetComponent<TextMeshProUGUI>();
        if (targetText == null)
        {
            Debug.LogError("TypingEffect requires a TextMeshProUGUI component!");
        }
        
        if (onTypingComplete == null)
        {
            onTypingComplete = new UnityEvent();
        }
    }

    /// <summary>
    /// Starts typing the provided text
    /// </summary>
    /// <param name="text">Text to type out</param>
    public void TypeText(string text)
    {
        currentFullText = text;
        
        if (currentTypingCoroutine != null)
        {
            StopCoroutine(currentTypingCoroutine);
        }
        
        currentTypingCoroutine = StartCoroutine(TypeTextCoroutine(text));
    }

    /// <summary>
    /// Types out the message character by character
    /// </summary>
    private IEnumerator TypeTextCoroutine(string message)
    {
        targetText.text = "";
        
        foreach (char letter in message.ToCharArray())
        {
            targetText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        currentTypingCoroutine = null;
        onTypingComplete.Invoke();
    }

    /// <summary>
    /// Skips the typing effect and shows the full text immediately
    /// </summary>
    public void SkipTyping()
    {
        if (currentTypingCoroutine != null)
        {
            StopCoroutine(currentTypingCoroutine);
            targetText.text = currentFullText;
            currentTypingCoroutine = null;
            onTypingComplete.Invoke();
        }
    }

    /// <summary>
    /// Clears the text
    /// </summary>
    public void Clear()
    {
        if (currentTypingCoroutine != null)
        {
            StopCoroutine(currentTypingCoroutine);
            currentTypingCoroutine = null;
        }
        
        targetText.text = "";
        currentFullText = "";
    }

    /// <summary>
    /// Sets the typing speed
    /// </summary>
    public void SetTypingSpeed(float speed)
    {
        typingSpeed = Mathf.Max(0.001f, speed);
    }
}