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
    public static string CleanupMessage(string message, bool removeNewlines = false, bool capitalize = true, bool removeIncomplete = true)
    {
        if (string.IsNullOrWhiteSpace(message))
            return string.Empty;

        // Remove character name prefix if present
        string cleanedMessage = RemoveCharacterPrefix(message);
        cleanedMessage = cleanedMessage.Replace("\"", "");

        int countUntilFirstNewline = cleanedMessage.IndexOf('\n');
        if(countUntilFirstNewline < 150)
            cleanedMessage = RemoveAfterSecondNewline(cleanedMessage);

        cleanedMessage = cleanedMessage.Replace("\n", " ");
        cleanedMessage = cleanedMessage.Replace("\r", " ");

        // Remove newlines if requested
        if (removeNewlines)
        {
            cleanedMessage = RemoveNewlines(cleanedMessage);
        }

        // Trim any whitespace before processing
        cleanedMessage = cleanedMessage.Trim();

        // Fix incomplete action formatting (asterisks and capitalization)
        // cleanedMessage = FixActionFormatting(cleanedMessage);

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

        // Look for small caps directly next to big caps (as in, without a space in between)
        // Replace them with a period and a space
        for (int i = 0; i < sentences.Length - 1; i++)
        {
            if (char.IsUpper(sentences[i][sentences[i].Length - 1]) && char.IsLower(sentences[i + 1][0]))
            {
                sentences[i] += ".";
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

    private static string RemoveAfterSecondNewline(string message)
    {
        if (string.IsNullOrEmpty(message))
            return message;

        // Find the first newline and check if there's a second one
        // If there is a second one, remove that second one and everything after it
        // Second newline cannot be right after the first one
        int firstNewlineIndex = message.IndexOf('\n');
        if (firstNewlineIndex != -1)
        {
            int relevantNewline = message.IndexOf('\n', firstNewlineIndex + 1);
            // Second newline cannot be right after the first one
            if(relevantNewline == firstNewlineIndex + 1)
            {   
                // Find the next
                relevantNewline = message.IndexOf('\n', relevantNewline + 1);
            }

            if (relevantNewline != -1)
            {
                message = message.Substring(0, relevantNewline);
            }
        }

        return message;
    }
}