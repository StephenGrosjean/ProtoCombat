using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnEnable : MonoBehaviour
{
    [SerializeField] private SoundManager.SoundList soundToPlay;

    private void OnEnable() {
        SoundManager._instance.PlaySound(soundToPlay);
    }
}
