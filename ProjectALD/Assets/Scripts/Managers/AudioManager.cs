using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AudioManager : SingleTon<AudioManager>
{
    private AudioSource _audioBgmSource;
    private AudioSource _audioSource;
    private Dictionary<string, AudioClip> _data = new();
    private string _curBgmKey;
    private Coroutine _playBgmCoroutine;
    public bool loadingCompleted = false;

    private void Awake()
    {
        SingleTonInit();
        Init();
        AudioSource[] sources = GetComponents<AudioSource>(); 
        _audioBgmSource = sources[0];
        _audioSource = sources[1];
    }

    public void Play(string key)
    {
        if (!_data.ContainsKey(key)) return;
        _audioSource.clip = _data[key];
        _audioSource.Play();
    }
    
    public void PlayOneShot(string key)
    {
        if (!_data.ContainsKey(key)) return;
        _audioSource.PlayOneShot(_data[key]);
    }
    
    public void PlayBgm(string key)
    {
        if (!_data.ContainsKey(key)) return;
        if (_curBgmKey != key)
        {
            if (_playBgmCoroutine != null) StopCoroutine(_playBgmCoroutine);
            _playBgmCoroutine = StartCoroutine(PlayBgmCoroutine(key));
        }
    }

    private IEnumerator PlayBgmCoroutine(string key)
    {
        while (true)
        {
            _audioBgmSource.clip = _data[key];
            _audioBgmSource.Play();
            yield return new WaitForSeconds(_data[key].length + 3f);
        }
    }

    public void StopBgm()
    {
        if (_playBgmCoroutine != null) StopCoroutine(_playBgmCoroutine);
    } 

    public void PlayClickSound()
    {
        PlayOneShot("Click");
    }

    private async Task Init()
    {
        await InitDataAsync();
    }

    private async Task InitDataAsync()
    {
        // IResourceLocation; Asset 위치에 대한 메타 정보
        var locHandle = Addressables.LoadResourceLocationsAsync("Sounds");
        var locations = await locHandle.Task;
        
        foreach (var location in locations)
        {
            var handle = Addressables.LoadAssetAsync<AudioClip>(location);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var clip = handle.Result;
                _data.Add(location.PrimaryKey, clip);
            }
        }
        Addressables.Release(locHandle);
        loadingCompleted = true;
    }
}
