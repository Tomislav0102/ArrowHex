using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using Random = UnityEngine.Random;

public class Fireworks : MonoBehaviour
{
    ParticleSystem[] _ps;
    Vector2Int _areaSize = new Vector2Int(10, 10);
    int _counter;
    float _timer, _maxTime;
    bool _isActive;

    void Awake()
    {
        _ps = Utils.AllChildren<ParticleSystem>(transform);
        List<ParticleSystem> list = Utils.RandomListByType(_ps.ToList());
        _ps = list.ToArray();
    }

    void Start()
    {
        _timer = float.MaxValue;
        _maxTime = Random.Range(0.3f, 1f);
    }

    void OnEnable()
    {
        GameManager.Instance.playerVictoriousNet.OnValueChanged += NetVar_PlayerVictorious;
    }

    void OnDisable()
    {
        GameManager.Instance.playerVictoriousNet.OnValueChanged -= NetVar_PlayerVictorious;
        CancelInvoke();
    }

    void Update()
    {
        if (!_isActive) return;
        _timer += Time.deltaTime;
        if (_timer >= _maxTime)
        {
            _timer = 0f;
            _maxTime = Random.Range(0.3f, 1f);
            SpawnFireworks();
        }
    }

    void NetVar_PlayerVictorious(PlayerFaction previousvalue, PlayerFaction newvalue)
    {
        int myColor = NetworkManager.Singleton.IsHost ? 0 : 1;
        if (myColor != (int)newvalue) return;
        _isActive = true;
    }

    void SpawnFireworks()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-_areaSize.x, _areaSize.x), Random.Range(3, _areaSize.y), Random.Range(-_areaSize.x * 0.5f, _areaSize.x));
        _ps[_counter].transform.position = spawnPos;
        AudioManager.Instance.PlaySfx(AudioManager.Instance.fireworkExplosion, true, spawnPos);
        _ps[_counter].Play();
        _counter = (1 + _counter) % _ps.Length;
    }
}
