using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ベース 5e00786e-b5d1-4cf2-91d3-b531b92943fa
public class SoundManager : MonoBehaviour
{
    // どこからでも呼び出せるように
    private static SoundManager _instance = null;
    public static SoundManager Instance => _instance;

    [System.Serializable]
    public class SoundData
    {
        public string Name;
        public int ID;
        public AudioClip Clip;
        [Range(1, 10)] public float VolumRate;
        [Range(0, 1)] public float SpatialBlend;
        public bool IsLoop;
    }

    [SerializeField] int _poolCount;
    [SerializeField] List<SoundData> _datas = new List<SoundData>();

    SoundPool _pool;

    private void Awake()
    {
        _instance = this;
        SetUp();
    }

    void SetUp()
    {
        _pool = new SoundPool();
        _pool.Create(_poolCount);
    }

    public static void UseRequest(int id)
    {
        foreach (SoundData data in Instance._datas)
        {
            if (data.ID == id)
            {
                SoundEffect sound = Instance._pool.Use();
                Debug.Log(sound);
                sound.Play(data);
                return;
            }
        }
    }

    public static void UseRequest(string name)
    {
        foreach (SoundData data in Instance._datas)
        {
            if (data.Name == name)
            {
                SoundEffect sound = Instance._pool.Use();
                sound.Play(data);
                return;
            }
        }
    }
}
