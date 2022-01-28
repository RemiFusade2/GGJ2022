using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("References")]
    public GameObject startPanel;
    public Text scoreValueText;
    public Text livesValueText;
    public Slider dayNightCycleSlider;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateDayNightSliderValue(float remaningTime)
    {
        dayNightCycleSlider.value = Mathf.CeilToInt(remaningTime);
    }

    public void ShowStartGamePanel(bool show)
    {
        startPanel.SetActive(show);
    }

    public void UpdateScoreValueText(int newValue)
    {
        scoreValueText.text = newValue.ToString();
    }
    public void UpdateLivesValueText(int newValue)
    {
        livesValueText.text = newValue.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
