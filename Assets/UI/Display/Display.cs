using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A controller for the segment display.
/// </summary>
public class Display : MonoBehaviour
{
    /// <summary>
    /// The lines of the segment display, from top to bottom.
    /// </summary>
    [SerializeField]
    private DisplayLine[] _lines;

    /// <summary>
    /// Displays the given text in the display, with an animation. The text is split
    /// along the newline characters.
    /// </summary>
    /// <param name="text"></param>
    public void DisplayTextAnimated(string text)
    {
        string[] lines = text.Split('\n');
        int displayLines = Mathf.Min(lines.Length, _lines.Length);
        for (int i = 0; i < displayLines; ++i)
        {
            _lines[i].DisplayTextAnimated(lines[i]);
        }
        for (int i = displayLines; i < _lines.Length; ++i)
        {
            _lines[i].DisplayTextAnimated(string.Empty);
        }
    }

    /// <summary>
    /// Displays the given text in the display immediately. The text is split along
    /// the newline characters.
    /// </summary>
    /// <param name="text"></param>
    public void DisplayTextImmediate(string text)
    {
        string[] lines = text.Split('\n');
        int displayLines = Mathf.Min(lines.Length, _lines.Length);
        for (int i = 0; i < displayLines; ++i)
        {
            _lines[i].DisplayTextImmediate(lines[i]);
        }
        for (int i = displayLines; i < _lines.Length; ++i)
        {
            _lines[i].DisplayTextImmediate(string.Empty);
        }
    }
}
