using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public Text mainText;
    public Text lapsText;

    public void SetMainText(string value)
    {
        mainText.text = value;
    }

    public void SetLapRecord(int lap, int max)
    {
        string value = String.Format("{0}/{1}", lap, max);
        lapsText.text = value;
    }
}
