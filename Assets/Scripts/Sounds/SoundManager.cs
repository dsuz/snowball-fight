using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using Sounds;

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

    [SerializeField] int _poolCount = 10;
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

    /// <summary>
    /// 鳴らしたいSEの申請
    /// </summary>
    /// <param name="id">SoundDataのID</param>
    /// <param name="isNetwork">Networkの同期をするかどうか</param>
    /// <param name="postion">鳴らすObgect</param>
    public static void UseRequest(int id, Vector3 postion, bool isNetwork = false)
    {
        foreach (SoundData data in Instance._datas)
        {
            if (data.ID == id)
            {
                SoundEffect se = Instance._pool.Use(postion);
                se.Play(data);
                if (isNetwork)
                {
                    Debug.Log("IsNetwork");
                    object[] prams = { id, postion };
                    Instance._view.RPC(nameof(NekworkPlayToFindID), RpcTarget.Others, prams);
                }

                return;
            }
        }
    }

    /// <summary>
    /// 鳴らしたいSEの申請
    /// </summary>
    /// <param name="name">SoundDataのName</param>
    /// <param name="isNetwork">Networkの同期をするかどうか</param>
    /// <param name="target">鳴らすObgect</param>
    public static void UseRequest(string name, Vector3 target, bool isNetwork = false)
    {
        foreach (SoundData data in Instance._datas)
        {
            if (data.Name == name)
            {
                SoundEffect se = Instance._pool.Use(target);
                se.Play(data);
                if (isNetwork)
                {
                    object[] prams = { name, target };
                    Instance._view.RPC(nameof(NekworkPlayToFindName), RpcTarget.Others, prams);
                }

                return;
            }
        }
    }

    [PunRPC]
    void NekworkPlayToFindID(object[] send)
    {
        int id = (int)send[0];
        Vector3 position = (Vector3)send[1];

        foreach (SoundData data in Instance._datas)
        {
            if (data.ID == id)
            {
                SoundEffect se = Instance._pool.Use(position);
                se.Play(data);
                return;
            }
        }
    }

    [PunRPC]
    void NekworkPlayToFindName(object[] send)
    {
        Debug.Log("IsNetworkCall");

        string name = (string)send[0];
        Vector3 target = (Vector3)send[1];

        foreach (SoundData data in Instance._datas)
        {
            if (data.Name == name)
            {
                SoundEffect se = Instance._pool.Use(target);
                se.Play(data);
                return;
            }
        }
    }
}
