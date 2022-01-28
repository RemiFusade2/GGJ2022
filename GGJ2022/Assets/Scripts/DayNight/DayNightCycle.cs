using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    private SpriteRenderer renderer;

    [Header("Settings")]
    public Sprite daySprite;
    public Color dayColor;
    [Space]
    public Sprite nightSprite;
    public Color nightColor;

    public CYCLETYPE currentCycle { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void SwitchToDay()
    {
        currentCycle = CYCLETYPE.DAY;
        if (renderer != null)
        {
            if (daySprite != null)
            {
                renderer.sprite = daySprite;
            }
            renderer.color = dayColor;
        }
    }

    public void SwitchToNight()
    {
        currentCycle = CYCLETYPE.NIGHT;
        if (renderer != null)
        {
            if (daySprite != null)
            {
                renderer.sprite = nightSprite;
            }
            renderer.color = nightColor;
        }
    }
}
