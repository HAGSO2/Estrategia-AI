using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public void SelctAI(int AI_ID)
    {
        OptionsSettings.SelectedAI = AI_ID;
    }
}

public static class OptionsSettings
{
    public static int SelectedAI { get; set; }
}
