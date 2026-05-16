using Photon.Pun;
using UnityEngine;

public class PhotonManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;    // 플레이어 프리팹

    private GameObject _playerInstance;    // 생성된 플레이어 인스턴스

    void Awake()
    {
        _playerInstance = PhotonNetwork.Instantiate(_playerPrefab.name, transform.position, transform.rotation);
    }
}
