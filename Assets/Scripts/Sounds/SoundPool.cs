using System.Collections.Generic;
using UnityEngine;

public class SoundPool
{
    GameObject _parent = null;
    List<SoundEffect> _soundsPool = new List<SoundEffect>();
    int _count = 1;

    public void Create(int count)
    {
        if (_parent == null) _parent = new GameObject("SoundsPool");

        for (int i = 0; i < count; i++)
        {
            GameObject obj = new GameObject($"SE.No:{_count}");
            obj.transform.SetParent(_parent.transform);
            _soundsPool.Add(obj.AddComponent<SoundEffect>());
            obj.SetActive(false);

            _count++;
        }
    }

    public SoundEffect Use()
    {
        foreach (SoundEffect s in _soundsPool)
        {
            if (!s.gameObject.activeSelf)
            {
                s.gameObject.SetActive(true);
                s.SetUp(Delete);
                return s;
            }
        }

        Create(10);
        return Use();
    }

    void Delete(SoundEffect set)
    {
        set.gameObject.SetActive(false);
    }
}
