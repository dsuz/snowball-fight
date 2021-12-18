using System.Collections.Generic;
using UnityEngine;

namespace Sounds
{
    /// <summary>
    /// SoundPoolの管理を行う
    /// </summary>
    public class SoundPool
    {
        Transform _parent = null;
        List<SoundEffect> _soundsPool = new List<SoundEffect>();
        int _count = 1;

        public void Create(int count)
        {
            if (_parent == null) _parent = new GameObject("SoundsPool").transform;

            for (int i = 0; i < count; i++)
            {
                GameObject obj = new GameObject($"SE.No:{_count}");
                obj.transform.SetParent(_parent);
                _soundsPool.Add(obj.AddComponent<SoundEffect>());
                obj.SetActive(false);

                _count++;
            }
        }

        public SoundEffect Use(Vector3 position)
        {
            foreach (SoundEffect s in _soundsPool)
            {
                if (!s.gameObject.activeSelf)
                {
                    s.gameObject.SetActive(true);
                    s.transform.parent = null;
                    s.transform.position = position;
                    s.SetUp(Delete);
                    return s;
                }
            }

            Create(10);
            return Use(position);
        }

        void Delete(SoundEffect set)
        {
            set.transform.SetParent(_parent);
            set.gameObject.SetActive(false);
        }
    }
}