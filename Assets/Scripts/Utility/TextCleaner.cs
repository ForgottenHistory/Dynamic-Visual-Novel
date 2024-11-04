using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

/**
// Examples:
Debug.Log(TextCleaner.CleanupMessage("hello! *waves* how are you?")); 
// Output: "Hello! *Waves.* How are you?"

Debug.Log(TextCleaner.CleanupMessage("*stands up* *walks away to the door")); 
// Output: "*Stands up.* *Walks away to the door.*"

Debug.Log(TextCleaner.CleanupMessage("john: hello there! *waves hand")); 
// Output: "Hello there! *Waves hand.*"
*/

/// <summary>
/// Utility class for cleaning up text messages by applying various formatting rules.
/// </summary>
public class TextCleaner
{
    /// <summary>
    /// Main cleanup function that applies all formatting rules
    /// </summary>
    public static string CleanupMessage(string message, bool removeNewlines = true, bool capitalize = true, bool removeIncomplete = true)
    {
        if (string.IsNullOrWhiteSpace(message))
            return string.Empty;

        // Remove character name prefix if present
        string cleanedMessage = RemoveCharacterPrefix(message);
        cleanedMessage = RemoveQuotationMarks(cleanedMessage);

        // Remove newlines if requested
        if (removeNewlines)
        {
            cleanedMessage = RemoveNewlines(cleanedMessage);
        }

        // Trim any whitespace before processing
        cleanedMessage = cleanedMessage.Trim();

        // Fix incomplete action formatting (asterisks and capitalization)
        cleanedMessage = FixActionFormatting(cleanedMessage);

        // Capitalize sentences if requested
        if (capitalize)
        {
            cleanedMessage = CapitalizeAllSentences(cleanedMessage);
        }

        // Remove incomplete sentences if requested
        if (removeIncomplete)
        {
            cleanedMessage = RemoveIncompleteSentences(cleanedMessage);
        }

        // Final cleanup
        cleanedMessage = cleanedMessage.Trim();

        // Double-check for unclosed asterisks as a final safety check
        if (cleanedMessage.Count(c => c == '*') % 2 != 0)
        {
            int firstAsterisk = cleanedMessage.IndexOf('*');
            if (firstAsterisk == 0 && !cleanedMessage.EndsWith("*"))
            {
                cleanedMessage += "*";
            }
        }

        return cleanedMessage;
    }

    /// <summary>
    /// Removes character name prefix (e.g., "John: Hello" -> "Hello")
    /// </summary>
    private static string RemoveCharacterPrefix(string message)
    {
        int colonIndex = message.IndexOf(':');
        return colonIndex == -1 ? message : message.Substring(colonIndex + 1);
    }

    /// <summary>
    /// Removes newlines and subsequent content
    /// </summary>
    private static string RemoveNewlines(string message)
    {
        int newlineIndex = message.IndexOfAny(new[] { '\r', '\n' });
        return newlineIndex == -1 ? message : message.Substring(0, newlineIndex);
    }

    /// <summary>
    /// Removes incomplete sentences (anything after the last period)
    /// </summary>
    private static string RemoveIncompleteSentences(string message)
    {
        int lastPeriodIndex = message.LastIndexOf('.');
        if (lastPeriodIndex != -1 && lastPeriodIndex < message.Length - 1)
        {
            return message.Substring(0, lastPeriodIndex + 1);
        }
        return message;
    }

    /// <summary>
    /// Capitalizes the first letter of every sentence in the text
    /// </summary>
    private static string CapitalizeAllSentences(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        // Split the text into sentences using regex to handle multiple types of ending punctuation
        string[] sentences = Regex.Split(text, @"(?<=[.!?])\s+");
        
        for (int i = 0; i < sentences.Length; i++)
        {
            if (!string.IsNullOrEmpty(sentences[i]))
            {
                sentences[i] = CapitalizeSentence(sentences[i]);
            }
        }

        return string.Join(" ", sentences);
    }

    /// <summary>
    /// Capitalizes a single sentence, including any actions within it
    /// </summary>
    private static string CapitalizeSentence(string sentence)
    {
        if (string.IsNullOrEmpty(sentence)) return sentence;

        // Handle regular sentence start
        if (sentence.Length > 0 && !sentence.StartsWith("*"))
        {
            sentence = char.ToUpper(sentence[0]) + sentence.Substring(1);
        }

        // Handle actions (text within asterisks)
        return Regex.Replace(sentence, @"\*([^*]+)\*", match =>
        {
            string action = match.Groups[1].Value.Trim();
            if (!string.IsNullOrEmpty(action))
            {
                action = char.ToUpper(action[0]) + action.Substring(1);
            }
            return $"*{action}*";
        });
    }

    /// <summary>
    /// Fixes formatting for actions marked with asterisks
    /// </summary>
    private static string FixActionFormatting(string message)
    {
        if (string.IsNullOrEmpty(message)) return message;

        // Handle single asterisk at the start
        if (message.StartsWith("*") && !message.EndsWith("*"))
        {
            string content = message.TrimStart('*').Trim();
            if (!EndsWithPunctuation(content))
            {
                content += ".";
            }
            
            // Capitalize first letter after asterisk
            if (content.Length > 0)
            {
                content = char.ToUpper(content[0]) + content.Substring(1);
            }
            
            return $"*{content}*";
        }

        // Handle multiple actions in the same message
        string[] parts = message.Split(new[] { "*" }, StringSplitOptions.RemoveEmptyEntries);
        List<string> fixedParts = new List<string>();
        bool wasLastPartAction = false;

        for (int i = 0; i < parts.Length; i++)
        {
            string part = parts[i].Trim();
            bool isAction = message.Contains($"*{part}") || (wasLastPartAction && !message.Contains($" {part}"));

            if (isAction)
            {
                if (!EndsWithPunctuation(part))
                {
                    part += ".";
                }
                
                // Capitalize first letter of action
                if (part.Length > 0)
                {
                    part = char.ToUpper(part[0]) + part.Substring(1);
                }
                
                fixedParts.Add($"*{part}*");
                wasLastPartAction = true;
            }
            else
            {
                fixedParts.Add(part);
                wasLastPartAction = false;
            }
        }

        return string.Join(" ", fixedParts).Trim();
    }

    /// <summary>
    /// Checks if text ends with punctuation
    /// </summary>
    private static bool EndsWithPunctuation(string text)
    {
        if (string.IsNullOrEmpty(text)) return false;
        char lastChar = text[text.Length - 1];
        return lastChar == '.' || lastChar == '!' || lastChar == '?' || lastChar == ':' || lastChar == ';';
    }

    private static string RemoveQuotationMarks(string message)
    {
        return message.Replace("\"", "");
    }
}