using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; } // singleton
    public AudioSource gunSound;

    void Awake()
    {
        Debug.Log("Awake in SoundManager called");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Ensure the SoundManager persists between scene loads.
            Debug.Log("Instance was null, now set");
        }
        else if (Instance != this)
        {
            Debug.Log("Another instance found, destroying this one");
            Destroy(gameObject);
        }

        if (gunSound == null)
        {
            Debug.LogError("gunSound is not assigned on " + gameObject.name);
        }
        else
        {
            Debug.Log("gunSound is correctly assigned");
        }
    }


}
