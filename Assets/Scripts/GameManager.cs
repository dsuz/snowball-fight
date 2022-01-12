using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// ゲームを管理する。
/// 接続 > 部屋に入って以降の管理をする。
/// </summary>
public class GameManager : MonoBehaviour, IOnEventCallback
{
    /// </summary>キャラを出現させる位置</summary>
    [SerializeField] Transform[] _spawnPoints = default;
    /// </summary>生成する物のプレハブ名</summary>
    [SerializeField] string _playerPrefabName = "";
    /// </summary>ゲーム開始フラグ</summary>
    [SerializeField] bool _inGame = false;

    /// </summary>制限時間</summary>
    [SerializeField] float _gameTimer = default;
    /// </summary>時間を表示するテキスト</summary>
    [SerializeField] Text _timerText = default;
    /// </summary>終わる時間</summary>
    float _time = 0;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void StartGame()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        Debug.Log($"My ActorNumber: {actorNumber}");
        Transform spawnPoint = _spawnPoints[(actorNumber - 1) % _spawnPoints.Length];
        Debug.Log($"Spawns at {spawnPoint.name}");
        GameObject player = PhotonNetwork.Instantiate(_playerPrefabName, spawnPoint.position, spawnPoint.rotation);
        player.name = $"Player{actorNumber}";
        player.GetComponent<PlayerController>().enabled = true;
    }

    void IOnEventCallback.OnEvent(ExitGames.Client.Photon.EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case (byte)NetworkEvents.GameStart:
                Debug.Log("Game Start");
                _inGame = true;
                break;
            case (byte)NetworkEvents.Die:
                break;
            default:
                break;
        }
    }
}

public enum NetworkEvents : byte
{
    GameStart,
    Die,
}
