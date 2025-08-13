using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTinkering : MonoBehaviour
{
    private Color32 startingColor;
    private Color32 targetColor;

    void Start(){
        startingColor = gameObject.GetComponent<Image>().color;
        targetColor = new Color32((byte)(startingColor.r-32),(byte)(startingColor.g-32),(byte)(startingColor.b-32),255);
    }

    void FixedUpdate()
    {
        gameObject.GetComponent<Image>().color = Color32.Lerp(targetColor, startingColor, Mathf.PingPong(Time.time/3, 1));
    }
}
