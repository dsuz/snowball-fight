using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using Sounds;

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
    [SerializeField] List<SoundData> _datas;

    PhotonView _view;
    SoundPool _pool;

    private void Awake()
    {
        _instance = this;
        _view = GetComponent<PhotonView>();
        _pool = new SoundPool();
        _pool.Create(_poolCount);
    }

    public static void UseRequest(int id, bool isNetwork = false, Transform target = null)
    {
        foreach (SoundData data in Instance._datas)
        {
            if (data.ID == id)
            {
                SoundEffect se = Instance._pool.Use(target);
                se.Play(data);
                if (isNetwork)
                {
                    Debug.Log("IsNetwork");
                    object[] datas = { data, target };
                    Debug.Log($"{datas[0]}  {datas[1]}");
                    Instance._view.RPC(nameof(Instance.NekworkPlaySound), RpcTarget.Others, datas);
                }

                return;
            }
        }
    }

    public static void UseRequest(string name, bool isNetwork = false, Transform target = null)
    {
        foreach (SoundData data in Instance._datas)
        {
            if (data.Name == name)
            {
                SoundEffect se = Instance._pool.Use(target);
                se.Play(data);
                if (isNetwork)
                {
                    object[] datas = { data, target };
                    Instance._view.RPC(nameof(NekworkPlaySound), RpcTarget.Others, datas);
                }

                return;
            }
        }
    }

    [PunRPC]
    void NekworkPlaySound(object[] datas)
    {
        Debug.Log("IsNetworkCall");

        SoundData data = (SoundData)datas[0];
        Transform target = (Transform)datas[1];

        SoundEffect se = Instance._pool.Use(target);
        se.Play(data);
    }
}
