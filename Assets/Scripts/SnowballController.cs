using UnityEngine;
using Photon.Pun;

/// <summary>
/// 雪玉を制御するコンポーネント
/// 雪玉は何かに当たるとエフェクトを出して消える
/// </summary>
public class SnowballController : MonoBehaviour
{
    [SerializeField] float _speed = 1f;
    [SerializeField] GameObject _hitEffect = default;
    PhotonView _view = default;

    void Start()
    {
        _view = GetComponent<PhotonView>();

        if (_view.IsMine)
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            if (rb)
            {
                rb.velocity = this.transform.forward * _speed;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_view.IsMine)
        {
            PhotonNetwork.Destroy(_view);
        }
    }

    void OnDestroy()
    {
        Instantiate(_hitEffect, this.transform.position, Quaternion.identity);
    }
}
