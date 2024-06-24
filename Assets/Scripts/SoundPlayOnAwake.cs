using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;

public class SoundPlayOnAwake : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    private void Start()
    {
        SoundMaster.Instance.PlayClip(0, audioClip);
    }
}
