using UnityEngine;
using System;

public class SoundEffect : MonoBehaviour
{
    AudioSource _source = null;
    Action<SoundEffect> _callBack = null;

    bool _isPlaying = false;

    void Update()
    {
        if (!_isPlaying) return;

        if (!_source.isPlaying)
        {
            _isPlaying = false;
            _callBack.Invoke(this);
        }
    }

    public void SetUp(Action<SoundEffect> action)
    {
        if (_source == null) _source = gameObject.AddComponent<AudioSource>();
        _callBack = action;
    }

    public void Play(SoundManager.SoundData data)
    {
        _source.clip = data.Clip;
        _source.volume = data.VolumRate / 100;
        _source.loop = data.IsLoop;
        _source.spatialBlend = data.SpatialBlend;

        _source.Play();
        _isPlaying = true;
    }
}