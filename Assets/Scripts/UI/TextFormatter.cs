using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

public class TextFormatter : MonoBehaviour
{

    [System.Serializable]
    public class TextStyle
    {
        public string name;
        public Color color = Color.white;
        public bool isBold;
        public bool isItalic;
        public string openMarker;
        public string closeMarker;

        public string GetHexColor()
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }
    }
    [Header("Text Styles")]
    [SerializeField] private List<TextStyle> textStyles = new List<TextStyle>();

    private void Reset()
    {
        textStyles = new List<TextStyle>
        {
            new TextStyle
            {
                name = "Action",
                color = new Color(0.678f, 0.847f, 0.902f), // Light blue
                isItalic = false,
                isBold = false,
                openMarker = "*",
                closeMarker = "*"
            },
            /**
            new TextStyle
            {
                name = "Thought",
                color = new Color(0.827f, 0.827f, 0.827f),
                isItalic = true,
                isBold = false,
                openMarker = "~",
                closeMarker = "~"
            },
            new TextStyle
            {
                name = "Emphasis",
                color = new Color(1f, 0.753f, 0.796f), // Light pink
                isItalic = false,
                isBold = false,
                openMarker = "_",
                closeMarker = "_"
            },
            new TextStyle
            {
                name = "Shout",
                color = new Color(1f, 0.271f, 0.271f),
                isItalic = false,
                isBold = true,
                openMarker = "!",
                closeMarker = "!"
            },
            new TextStyle
            {
                name = "Whisper",
                color = new Color(0.741f, 0.718f, 0.42f),
                isItalic = true,
                isBold = false,
                openMarker = "#",
                closeMarker = "#"
            }
            */
        };
    }

    private string CleanExistingColorTags(string input)
    {
        string cleanText = input;

        // Remove all variations of color tags
        cleanText = Regex.Replace(cleanText, @"<color=[^>]*>", "", RegexOptions.IgnoreCase);
        cleanText = Regex.Replace(cleanText, @"<color=""[^""]*"">", "", RegexOptions.IgnoreCase);
        cleanText = Regex.Replace(cleanText, @"</color>", "", RegexOptions.IgnoreCase);
        cleanText = Regex.Replace(cleanText, @"<#[A-Fa-f0-9]{6}>", "", RegexOptions.IgnoreCase);
        cleanText = Regex.Replace(cleanText, @"<[/]?[bi]>", "", RegexOptions.IgnoreCase);

        return cleanText;
    }

    public string FormatText(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        Debug.Log($"Formatting text: {input}");
        string formattedText = CleanExistingColorTags(input);

        // First ensure all style markers are properly paired
        formattedText = EnsureCompleteMarkers(formattedText);

        var sortedStyles = new List<TextStyle>(textStyles);
        sortedStyles.Sort((a, b) => b.openMarker.Length.CompareTo(a.openMarker.Length));

        var replacements = new List<(int start, int length, string replacement)>();

        foreach (var style in sortedStyles)
        {
            try
            {
                string escapedOpenMarker = Regex.Escape(style.openMarker);
                string escapedCloseMarker = Regex.Escape(style.closeMarker);
                string pattern = $"{escapedOpenMarker}(.*?){escapedCloseMarker}";

                var matches = Regex.Matches(formattedText, pattern);

                foreach (Match match in matches)
                {
                    string content = match.Groups[1].Value;

                    StringBuilder tags = new StringBuilder();
                    StringBuilder closingTags = new StringBuilder();

                    tags.Append($"<color=#{style.GetHexColor()}>");
                    if (style.isItalic) tags.Append("<i>");
                    if (style.isBold) tags.Append("<b>");

                    if (style.isBold) closingTags.Append("</b>");
                    if (style.isItalic) closingTags.Append("</i>");
                    closingTags.Append("</color>");

                    string replacement = tags.ToString() + content + closingTags.ToString();
                    replacements.Add((match.Index, match.Length, replacement));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error formatting text with style {style.name}: {e.Message}");
                return input;
            }
        }

        replacements.Sort((a, b) => b.start.CompareTo(a.start));
        foreach (var (start, length, replacement) in replacements)
        {
            formattedText = formattedText.Remove(start, length).Insert(start, replacement);
        }

        // Final check for unmatched style markers after all formatting
        formattedText = DoFinalMarkerCheck(formattedText);

        Debug.Log($"Final formatted text: {formattedText}");
        return formattedText;
    }

    /// <summary>
    /// Ensures all style markers are properly paired
    /// </summary>
    private string EnsureCompleteMarkers(string text)
    {
        foreach (var style in textStyles)
        {
            // Count occurrences of markers
            int openCount = CountOccurrences(text, style.openMarker);
            int closeCount = CountOccurrences(text, style.closeMarker);

            // If there's an unmatched opening marker
            while (openCount > closeCount)
            {
                text += style.closeMarker;
                closeCount++;
            }
        }
        return text;
    }

    /// <summary>
    /// Does a final check for any unmatched style markers after color formatting
    /// </summary>
    private string DoFinalMarkerCheck(string text)
    {
        foreach (var style in textStyles)
        {
            // Find actual markers (not inside color tags)
            var openMatches = Regex.Matches(text, $@"(?<!<[^>]*){Regex.Escape(style.openMarker)}");
            var closeMatches = Regex.Matches(text, $@"(?<!<[^>]*){Regex.Escape(style.closeMarker)}");

            if (openMatches.Count > closeMatches.Count)
            {
                // If we have an unmatched opening marker, add a closing marker at the end
                text += style.closeMarker;
            }
        }
        return text;
    }

    private int CountOccurrences(string text, string marker)
    {
        return Regex.Matches(text, Regex.Escape(marker)).Count;
    }

    public void AddStyle(string name, Color color, bool isBold, bool isItalic, string openMarker, string closeMarker)
    {
        textStyles.Add(new TextStyle
        {
            name = name,
            color = color,
            isBold = isBold,
            isItalic = isItalic,
            openMarker = openMarker,
            closeMarker = closeMarker
        });
    }

    public void UpdateStyle(string name, Color? color = null, bool? isBold = null, bool? isItalic = null)
    {
        var style = textStyles.Find(s => s.name == name);
        if (style != null)
        {
            if (color.HasValue) style.color = color.Value;
            if (isBold.HasValue) style.isBold = isBold.Value;
            if (isItalic.HasValue) style.isItalic = isItalic.Value;
        }
    }

    public string StripFormatting(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        foreach (var style in textStyles)
        {
            string pattern = $@"{Regex.Escape(style.openMarker)}(.*?){Regex.Escape(style.closeMarker)}";
            input = Regex.Replace(input, pattern, "$1");
        }
        return input;
    }
}