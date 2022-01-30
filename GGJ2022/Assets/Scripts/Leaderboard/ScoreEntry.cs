using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreEntry : MonoBehaviour
{
    public Text rankValueText;
    public Text scoreValueText;
    public List<Text> nameLetterValueTextList;

    public Color inactiveEntryColor;
    public Color activeEntryColor;

    /*
    public float blinkingOnTime;
    public float blinkingOffTime;*/

    //private Coroutine currentBlinkCoroutine;

    private int currentScore;

    private bool inputActive;

    [Header("Runtime")]
    public int activeInputLetterIndex;
    public int activeLetterIndex;

    public static readonly char[] Alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'Å', 'Ä', 'Ö', '.' };

    private string GetRankStr(int rank)
    {
        string rankStr = rank.ToString() + "TH";
        if (rank == 1)
        {
            rankStr = "1ST";
        }
        else if (rank == 2)
        {
            rankStr = "2ND";
        }
        else if (rank == 3)
        {
            rankStr = "3RD";
        }
        return rankStr;
    }

    public void Initialize(int rank, int score, string name, bool active)
    {
        if (active)
        {
            rankValueText.color = activeEntryColor;
            scoreValueText.color = activeEntryColor;
            nameLetterValueTextList[0].color = activeEntryColor;
            nameLetterValueTextList[1].color = activeEntryColor;
            nameLetterValueTextList[2].color = activeEntryColor;

            nameLetterValueTextList[0].text = ".";
            nameLetterValueTextList[1].text = ".";
            nameLetterValueTextList[2].text = ".";

            activeInputLetterIndex = 0;
            activeLetterIndex = 0;
            inputActive = true;

            nameLetterValueTextList[activeInputLetterIndex].enabled = true;

            UpdateActiveLetterText();

            //currentBlinkCoroutine = StartCoroutine(WaitAndHideActiveInputLetter(blinkingOnTime));
        }
        else
        {
            rankValueText.color = inactiveEntryColor;
            scoreValueText.color = inactiveEntryColor;
            nameLetterValueTextList[0].color = inactiveEntryColor;
            nameLetterValueTextList[1].color = inactiveEntryColor;
            nameLetterValueTextList[2].color = inactiveEntryColor;

            nameLetterValueTextList[0].text = name[0].ToString();
            nameLetterValueTextList[1].text = name[1].ToString();
            nameLetterValueTextList[2].text = name[2].ToString();

            activeInputLetterIndex = -1;
            inputActive = false;
        }

        rankValueText.text = GetRankStr(rank);
        scoreValueText.text = score.ToString();

        currentScore = score;
    }

    private void UpdateActiveLetterText()
    {
        nameLetterValueTextList[activeInputLetterIndex].text = Alphabet[activeLetterIndex].ToString();
    }

    public void SwitchLetter(int delta)
    {
        if (inputActive)
        {
            activeLetterIndex += delta;
            activeLetterIndex = ((activeLetterIndex + Alphabet.Length) % Alphabet.Length);
            UpdateActiveLetterText();
        }
    }

    public void ConfirmLetter()
    {
        if (inputActive && activeInputLetterIndex == 2)
        {
            // confirm entry
            inputActive = false;
            nameLetterValueTextList[activeInputLetterIndex].enabled = true;
            /*if (currentBlinkCoroutine != null)
            {
                StopCoroutine(currentBlinkCoroutine);
            }*/

            LeaderboardManager.instance.EnterScore(nameLetterValueTextList[0].text.ToString() + nameLetterValueTextList[1].text.ToString() + nameLetterValueTextList[2].text.ToString(), currentScore);
        }
        else if (inputActive)
        {
            nameLetterValueTextList[activeInputLetterIndex].enabled = true;
            /*if (currentBlinkCoroutine != null)
            {
                StopCoroutine(currentBlinkCoroutine);
            }*/
            activeInputLetterIndex++;
            nameLetterValueTextList[activeInputLetterIndex].enabled = true;
            activeLetterIndex = 0;
            UpdateActiveLetterText();
            //currentBlinkCoroutine = StartCoroutine(WaitAndHideActiveInputLetter(blinkingOnTime));
        }
    }

    /*
    private IEnumerator WaitAndDisplayActiveInputLetter(float delay)
    {
        yield return new WaitForSeconds(delay);

        nameLetterValueTextList[activeInputLetterIndex].enabled = true;

        currentBlinkCoroutine = StartCoroutine(WaitAndHideActiveInputLetter(blinkingOnTime));
    }
    private IEnumerator WaitAndHideActiveInputLetter(float delay)
    {
        yield return new WaitForSeconds(delay);

        nameLetterValueTextList[activeInputLetterIndex].enabled = false;

        currentBlinkCoroutine = StartCoroutine(WaitAndDisplayActiveInputLetter(blinkingOffTime));
    }*/
}
