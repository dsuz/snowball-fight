using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

/// <summary>
/// ゲームを管理する。
/// 接続 > 部屋に入って以降の管理をする。
/// </summary>
public class GameManager : MonoBehaviour, IOnEventCallback
{
    /// </summary>キャラを出現させる位置</summary>
    [SerializeField] Transform[] _spawnPoints = default;
    /// </summary>生成する物のプレハブ名</summary>
    [SerializeField] string _playerPrefabNameA = "";
    [SerializeField] string _playerPrefabNameB = "";
    /// </summary>ゲーム開始フラグ</summary>
    [SerializeField] bool _inGame = false;

    /// </summary>制限時間</summary>
    [SerializeField] float _gameTimer = default;
    /// </summary>時間を表示するText</summary>
    [SerializeField] Text _timerText = default;

    /// </summary>Aチームの得点</summary>
    [SerializeField] int _scoreA;
    /// </summary>Bチームの得点</summary>
    [SerializeField] int _scoreB;
    /// </summary>優勢,劣勢を表示するText</summary>
    [SerializeField] Text _winOrLoseText;

    PhotonView _view;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Start()
    {
        _view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!_view.IsMine)
            return;

        if (PhotonNetwork.InRoom)
        {
            object[] scores = new object[] { _scoreA, _scoreB, _gameTimer };
            _view.RPC("SyncScore", RpcTarget.Others, scores);
        }

        if (_inGame)
        {
            _gameTimer -= Time.deltaTime;
            _timerText.text = _gameTimer.ToString("f1");
            if (_gameTimer < 0)
            {
                _gameTimer = 0;

                RaiseEventOptions raiseEventoptions = new RaiseEventOptions();
                raiseEventoptions.Receivers = ReceiverGroup.All;
                SendOptions sendOptions = new SendOptions();
                PhotonNetwork.RaiseEvent((byte)NetworkEvents.End, null, raiseEventoptions, sendOptions);

                // 結果発表
            }
        }
    }

    public void StartGame()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        Debug.Log($"My ActorNumber: {actorNumber}");
        Transform spawnPoint = _spawnPoints[(actorNumber - 1) % _spawnPoints.Length];
        Debug.Log($"Spawns at {spawnPoint.name}");

        if (actorNumber % 2 == 1) // 奇数偶数でチーム分け
        {
            GameObject teamA = PhotonNetwork.Instantiate(_playerPrefabNameA, spawnPoint.position, spawnPoint.rotation);
            teamA.name = $"Player{actorNumber}";
            teamA.GetComponent<PlayerController>().enabled = true;
        }
        else
        {
            GameObject teamB = PhotonNetwork.Instantiate(_playerPrefabNameB, spawnPoint.position, spawnPoint.rotation);
            teamB.name = $"Player{actorNumber}";
            teamB.GetComponent<PlayerController>().enabled = true;
        }
    }

    /// <summary>
    /// スコアの計算
    /// </summary>
    public void AddScore(int scoreA, int scoreB)
    {
        if (_inGame) // 対戦中のみスコアを計算する
        {
            _scoreA += scoreA;
            _scoreB += scoreB;
            RefreshText();

            object[] scores = new object[] { _scoreA, _scoreB, _gameTimer };
            _view.RPC("SyncScore", RpcTarget.Others, scores);
        }
    }

    /// <summary>
    /// スコア,時間をクライアント間で同期する
    /// </summary>
    [PunRPC]
    void SyncScore(int scoreA, int scoreB, float gameTime)
    {
        _scoreA = scoreA;
        _scoreB = scoreB;
        _gameTimer = gameTime;
        RefreshText();
    }

    /// <summary>
    /// Text関連
    /// </summary>
    void RefreshText()
    {
        if (_inGame) // 対戦中のみに制限時間を表示したかったから
            _timerText.text = _gameTimer.ToString("f1");

        if (PhotonNetwork.LocalPlayer.ActorNumber % 2 == 1)
        {
            // Aチーム
            if (_scoreA > _scoreB)
                _winOrLoseText.text = "優勢";
            else if (_scoreA == _scoreB)
                _winOrLoseText.text = "";
            else
                _winOrLoseText.text = "劣勢";
        }
        else
        {
            // Bチーム
            if (_scoreB > _scoreA)
                _winOrLoseText.text = "優勢";
            else if (_scoreB == _scoreA)
                _winOrLoseText.text = "";
            else
                _winOrLoseText.text = "劣勢";
        }
    }

    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case (byte)NetworkEvents.GameStart:
                Debug.Log("Game Start");
                _inGame = true;
                break;
            case (byte)NetworkEvents.End:
                Debug.Log("Game End");
                _inGame = false;
                break;
            default:
                break;
        }
    }
}

public enum NetworkEvents : byte
{
    GameStart,
    End,
}
