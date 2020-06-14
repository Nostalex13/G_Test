using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGradient : MonoBehaviour
{
    [SerializeField] Gradient gradient;

    int startIndex;
    int lastIndex;
    float randomLerpVal;
    Color color;

    public Color GetRandomColor()
    {
        color = Color.white;

        if (gradient.colorKeys.Length < 2)
        {
            Debug.Log("<color=red>ERROR</color> At least 2 colors required");
            return color;
        }

        startIndex = Random.Range(0, gradient.colorKeys.Length - 1);
        lastIndex = startIndex + 1;
        randomLerpVal = Random.Range(0f, 1f);
        color = Color.Lerp(gradient.colorKeys[startIndex].color, gradient.colorKeys[lastIndex].color, randomLerpVal);

        return color;
    }
}
