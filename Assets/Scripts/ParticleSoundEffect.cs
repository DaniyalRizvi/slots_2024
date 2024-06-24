using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;

public class ParticleSoundEffect : MonoBehaviour
{
    private ParticleSystem _parentParticleSystem;

    private int _currentNumberOfParticles = 0;

    public AudioClip BornSounds;
    public AudioClip DieSounds;

    private void Awake()
    {
    }

    void Start()
    {
        _parentParticleSystem = this.GetComponent<ParticleSystem>();
        if (_parentParticleSystem == null)
            Debug.LogError("Missing ParticleSystem!", this);
    }

    void Update()
    {
        var amount = Mathf.Abs(_currentNumberOfParticles - _parentParticleSystem.particleCount);

        if (_parentParticleSystem.particleCount < _currentNumberOfParticles)
        {
            SoundMaster.Instance.PlayClip(0f, BornSounds);
        }

        if (_parentParticleSystem.particleCount > _currentNumberOfParticles)
        {
            SoundMaster.Instance.PlayClip(0f, DieSounds);
        }

        _currentNumberOfParticles = _parentParticleSystem.particleCount;
    }
}
