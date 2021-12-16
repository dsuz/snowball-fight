using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Photon.Pun;
using System;
using System.Linq;

/// <summary>
/// プレイヤーを制御するコンポーネント
/// ・通常時は上下左右で動ける
/// ・敵に近づくとロックオンできる
/// ・ロックオン中に Fire1 を押すと、対象に向かってダッシュする
/// ・ダッシュ中は操作できない
/// ・ダッシュの対象に体当たりできたら、ダッシュ状態は解除されて操作可能な通常状態に戻る
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>仮想カメラの Follow</summary>
    [SerializeField] Transform _cameraFollowTarget = default;
    /// <summary>仮想カメラの LookAt</summary>
    [SerializeField] Transform _cameraLookAtTarget = default;
    /// <summary>移動する力</summary>
    [SerializeField] float _movePower = 3;
    /// <summary>手の IK を制御するコンポーネント</summary>
    [SerializeField] HandIK _handIK = default;
    /// <summary>手に持っている雪玉（表示のみ）</summary>
    [SerializeField] GameObject _snowball = default;
    /// <summary>ボールを作るゲージの動く速さ</summary>
    [SerializeField] float _createSnowballSpeed = 10f;
    /// <summary>ボールを作るゲージ（いっぱいになったら雪玉作成完了）</summary>
    [SerializeField] Slider _snowballGauge = default;
    /// <summary>雪玉のプレハブ名</summary>
    [SerializeField] string _snowballPrefabName = "SnowballPrefab";
    /// <summary>雪玉を発射する場所と向きを指定する Transform</summary>
    [SerializeField] Transform _snowballMuzzle = default;
    /// <summary>UI</summary>
    [SerializeField] Canvas _ui = default;
    /// <summary>雪玉を持っている（作成完了している）か</summary>
    bool _hasSnowball = false;
    /// <summary>雪玉を生成するメーターが溜まっている量（0.99 を超えたら生成完了とする）</summary>
    float _snowballMeter = 0f;
    Rigidbody _rb = default;
    Animator _anim = default;
    /// <summary>入力された方向の XZ 平面でのベクトル</summary>
    Vector3 _dir;
    PhotonView _view = default;
    PlayerStatus _status = PlayerStatus.None;

    /// <summary>ロックオンしているターゲット</summary>
    Transform _lockedTarget = default;
    /// <summary>ダッシュ・体当たりの対象となっているターゲット</summary>
    Transform _dashTarget = default;

    /// <summary>
    /// ロックオンしているターゲット。何もロックオンしていない時は null
    /// </summary>
    public Transform LockedTarget
    {
        get { return _lockedTarget; }
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _view = GetComponent<PhotonView>();

        if (_view.IsMine)
        {
            SetUpVirtualCamera();
        }
        else
        {
            // 他人の UI は消す
            _ui.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 仮想カメラに対して、Follow, LookAt を設定する
    /// </summary>
    void SetUpVirtualCamera()
    {
        Array.ForEach(GameObject.FindObjectsOfType<CinemachineVirtualCameraBase>(), c =>
        {
            c.transform.forward = this.transform.forward;
            c.Follow = _cameraFollowTarget;
            c.LookAt = _cameraLookAtTarget;
        });
    }

    /// <summary>
    /// 入力を制御する
    /// </summary>
    void Update()
    {
        if (!_view.IsMine) return;

        // 入力を受け付ける
        float h = Input.GetAxisRaw("Horizontal");
        _dir = Vector3.right * h;
        // カメラのローカル座標系を基準に dir を変換する
        _dir = Camera.main.transform.TransformDirection(_dir);
        // カメラは斜め下に向いているので、Y 軸の値を 0 にして「XZ 平面上のベクトル」にする
        _dir.y = 0;

        if (_hasSnowball && Input.GetButtonDown("Fire1"))
        {
            _handIK.enabled = false;
            _anim?.SetBool("Throw", true);
            _status = PlayerStatus.Throwing;
            _hasSnowball = false;
            _snowball.SetActive(false);
            PhotonNetwork.Instantiate(_snowballPrefabName, this._snowballMuzzle.position, this._snowballMuzzle.rotation);
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            _anim?.SetBool("Throw", false);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            _handIK.enabled = false;
            _anim?.SetBool("Crouch", true);
            _status = PlayerStatus.Crouch;
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            _anim?.SetBool("Crouch", false);
            _status = PlayerStatus.None;

            if (_snowballMeter > 0.99f)
            {
                _hasSnowball = true;
                _snowball.SetActive(true);
            }

            if (_hasSnowball)
            {
                _handIK.enabled = true;
            }

            _snowballMeter = 0;
        }

        if (Input.GetButton("Fire2"))
        {
            _snowballMeter += Time.deltaTime * _createSnowballSpeed;
        }
    }

    /// <summary>
    /// ステータスを戻す
    /// 投げるモーションの最後にアニメーションイベントから呼ばれることを想定している
    /// </summary>
    void ResetStatus()
    {
        _status = PlayerStatus.None;
        _anim?.SetBool("Throw", false);
    }

    /// <summary>
    /// 物理挙動を制御する
    /// </summary>
    void FixedUpdate()
    {
        if (!_view.IsMine) return;

        if (_status == PlayerStatus.None)
        {
            _rb.AddForce(_dir.normalized * _movePower, ForceMode.Force);
        }
        else if (_status == PlayerStatus.Throwing || _status == PlayerStatus.Crouch)
        {
            _rb.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// 見た目を制御する
    /// </summary>
    void LateUpdate()
    {
        if (!_view.IsMine) return;

        if (_anim)
        {
            Vector3 localVelocity = this.transform.InverseTransformDirection(_rb.velocity);
            _anim.SetFloat("Right", localVelocity.x);
        }

        if (_snowballGauge)
        {
            _snowballGauge.value = _snowballMeter;
        }
    }
}

/// <summary>
/// プレイヤーの状態を表す
/// </summary>
enum PlayerStatus
{
    None,
    Crouch,
    Throwing,
}
