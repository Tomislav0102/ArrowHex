using UnityEngine;
using System.Collections;
using TMPro;


public class TeleType : MonoBehaviour
{
    TMP_Text _display;
    const float CONST_PauseBetweenLetters = 0.05f;
    
    public IEnumerator TeleTypeProcess(TMP_Text display, string textToDisplay)
    {
        _display = display;
        _display.text = textToDisplay;

        // Force and update of the mesh to get valid information.
        _display.ForceMeshUpdate();

        WaitForSeconds wait = Utils.GetWait(CONST_PauseBetweenLetters);

        int totalVisibleCharacters = _display.textInfo.characterCount; // Get # of Visible Character in text object
        int counter = 0;

        while (counter <= totalVisibleCharacters)
        {
            _display.maxVisibleCharacters = counter;
            counter++;

            yield return wait;
        }

        //Debug.Log("Done revealing the text.");

    }
}
