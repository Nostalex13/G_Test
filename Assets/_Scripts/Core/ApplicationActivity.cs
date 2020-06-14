using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationActivity : MonoBehaviour
{
    public static event System.Action<bool> OnAppFocus;

    private void OnApplicationFocus(bool focus)
    {
        OnAppFocus?.Invoke(focus);
    }

    private void OnApplicationPause(bool pause)
    {
        OnAppFocus?.Invoke(pause);
    }
}
