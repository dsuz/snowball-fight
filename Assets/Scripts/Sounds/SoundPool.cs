using System.Collections.Generic;
using UnityEngine;

namespace Sounds
{
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

        public SoundEffect Use(Transform target)
        {
            foreach (SoundEffect s in _soundsPool)
            {
                if (!s.gameObject.activeSelf)
                {
                    s.gameObject.SetActive(true);
                    if (target != null) s.transform.SetParent(target);
                    s.SetUp(Delete);
                    return s;
                }
            }

            Create(10);
            return Use(target);
        }

        void Delete(SoundEffect set)
        {
            set.transform.SetParent(_parent);
            set.gameObject.SetActive(false);
        }
    }
}