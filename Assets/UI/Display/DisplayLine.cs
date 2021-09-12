using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A single line of the segment display.
/// </summary>
public class DisplayLine : Graphic
{
    /// <summary>
    /// The list of character displays, in the order in which they appear on the screen.
    /// </summary>
    [SerializeField]
    private Text[] _characters;

    /// <summary>
    /// The minimal time (in seconds) between swtiching characters.
    /// </summary>
    [SerializeField]
    private float _minCharacterChangeTimeSeconds = 0.05f;

    /// <summary>
    /// The maximal time (in seconds) between switching characters.
    /// </summary>
    [SerializeField]
    private float _maxCharacterChangeTimeSeconds = 0.10f;

    /// <summary>
    /// The minimal number of randomly inserted characters when switching.
    /// </summary>
    [SerializeField]
    private int _minRandomCharacters = 0;

    /// <summary>
    /// The maximal number of randomly inserted characters when switching.
    /// </summary>
    [SerializeField]
    private int _maxRandomCharacters = 1;

    /// <summary>
    /// The number of characters that still do not have thier final value.
    /// </summary>
    private int _characterUpdatesInFlight = 0;

    /// <summary>
    /// The text color of the menu item.
    /// </summary>
    public override Color color
    {
        get => base.color; set
        {
            base.color = color;
            foreach (var character in _characters)
            {
                character.color = value;
            }
        }
    }

    /// <summary>
    /// Animated version of updating the text on the display line. Randomly
    /// switches characters before settling on <paramref name="text"/> in the
    /// end.
    /// 
    /// It is an error to run this method when there is another text change in
    /// flight.
    /// </summary>
    /// <param name="text">The text to be displayed in the end.</param>
    /// <param name="randomizeAll">Whether to randomize and animate all positions.</param>
    public void DisplayTextAnimated(string text, bool randomizeAll)
    {
        Debug.Assert(_characterUpdatesInFlight == 0);
        text = text.ToUpper();

        int displayedChars = Mathf.Min(text.Length, _characters.Length);
        _characterUpdatesInFlight = _characters.Length;
        for (int i = 0; i < displayedChars; ++i)
        {
            StartCoroutine(UpdateCharCoroutine(_characters[i], text[i], randomizeAll));
        }
        for (int i = displayedChars; i < _characters.Length; ++i)
        {
            StartCoroutine(UpdateCharCoroutine(_characters[i], ' ', randomizeAll));
        }
    }

    /// <summary>
    /// Updates the text on the display line immediately.
    /// </summary>
    /// <param name="text">The text to be displayed.</param>
    public void DisplayTextImmediate(string text)
    {
        Debug.Assert(_characterUpdatesInFlight == 0);
        text = text.ToUpper();

        int displayedChars = Mathf.Min(text.Length, _characters.Length);
        for (int i = 0; i < displayedChars; ++i)
        {
            _characters[i].text = text[i].ToString();
        }
        for (int i = displayedChars; i < _characters.Length; ++i)
        {
            _characters[i].text = " ";
        }
    }

    /// <summary>
    /// A coroutine that animates switching of a character.
    /// </summary>
    /// <param name="character">The text field that displays the character.</param>
    /// <param name="finalValue">The final character to display in the text field.</param>
    /// <returns>The coroutine object.</returns>
    /// <param name="randomizeAll">When <c>true</c>, the char is randomized even if
    /// it is a space or it already contains the right value.</param>
    private IEnumerator UpdateCharCoroutine(Text character, char finalValue, bool randomizeAll)
    {
        string finalString = finalValue.ToString();
        if (!randomizeAll && (character.text == finalString || finalValue == ' '))
        {
            character.text = finalString;
            --_characterUpdatesInFlight;
            yield break;
        }

        int numRandomCharacters = Random.Range(_minRandomCharacters, _maxRandomCharacters + 1);
        for (int i = 0; i < numRandomCharacters; ++i)
        {
            yield return new WaitForSecondsRealtime(RandomCharacterWaitTimeSeconds());
            character.text = RandomCharacter().ToString();
        }
        yield return new WaitForSecondsRealtime(RandomCharacterWaitTimeSeconds());
        character.text = finalString;
        --_characterUpdatesInFlight;
    }

    /// <summary>
    /// Returns a random time (in seconds) to wait between two character switches when
    /// animating a character change.
    /// </summary>
    /// <returns>The time between two characters, in seconds.</returns>
    private float RandomCharacterWaitTimeSeconds()
    {
        return Random.Range(_minCharacterChangeTimeSeconds, _maxCharacterChangeTimeSeconds);
    }

    /// <summary>
    /// Returns a random character to display when switching characters on the display.
    /// </summary>
    /// <returns>A random character between 'A' and 'Z'.</returns>
    private char RandomCharacter()
    {
        int lower_bound = (int)'A';
        int upper_bound = (int)'Z' + 1;
        return (char)Random.Range(lower_bound, upper_bound);
    }

    public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha, bool useRGB)
    {
        // This method is called by UnityEngine.UI.Selectable on hover and when clicked.
        // The default implementation changes the color of the underlying mesh renderer,
        // which would have no effect on the text.  We ignore the base and just set the
        // color of the text.
        if (!useRGB && !useAlpha) return;

        // TODO(ondrasej): We ignore the duration and set the target color straight away.
        color = targetColor;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // Do not use any mesh. This is needed to avoid rendering anything for this object.
        vh.Clear();
    }
}
