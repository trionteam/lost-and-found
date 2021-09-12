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
    /// The text shown in the display panel.
    /// </summary>
    [SerializeField, Multiline]
    private string _text;

    /// <summary>
    /// When set to <c>true</c>, all characters (i.e. even spaces) are randomized
    /// when the display is shown.
    /// </summary>
    [SerializeField]
    private bool _randomizeAll;

    /// <summary>
    /// Displays the given text in the display, with an animation. The text is split
    /// along the newline characters.
    /// </summary>
    /// <param name="text">The text to be displayed.</param>
    /// <param name="randomizeAll">Whether to randomize and animate all positions.</param>
    public void DisplayTextAnimated(string text, bool randomizeAll)
    {
        string[] lines = text.Split('\n');
        int displayLines = Mathf.Min(lines.Length, _lines.Length);
        for (int i = 0; i < displayLines; ++i)
        {
            _lines[i].DisplayTextAnimated(lines[i], randomizeAll);
        }
        for (int i = displayLines; i < _lines.Length; ++i)
        {
            _lines[i].DisplayTextAnimated(string.Empty, randomizeAll);
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

    /// <summary>
    /// Shows the display panel, and animates the characters to show the text entered
    /// in the editor.
    /// </summary>
    public void ShowAndAnimate()
    {
        gameObject.SetActive(true);
        DisplayTextAnimated(_text, _randomizeAll);
    }

    /// <summary>
    /// Hides the display panel.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        DisplayTextImmediate(_text);
    }
}
