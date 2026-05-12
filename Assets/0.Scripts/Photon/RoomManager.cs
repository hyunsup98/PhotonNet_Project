using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string gameSceneName = "GardenScene";
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private TMP_Text playerListText;
    [SerializeField] private Button enterGameButton;

    // 룸 화면이 시작될 때 현재 플레이어 목록 UI를 갱신한다.
    private void Start()
    {
        RefreshPlayerList();
    }

    // 새 플레이어가 방에 입장하면 플레이어 목록 UI를 갱신한다.
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RefreshPlayerList();
    }

    // 플레이어가 방을 나가면 플레이어 목록 UI를 갱신한다.
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RefreshPlayerList();
    }

    // 현재 클라이언트가 방에 입장하면 플레이어 목록 UI를 갱신한다.
    public override void OnJoinedRoom()
    {
        RefreshPlayerList();
    }

    // 마스터 클라이언트가 변경되면 플레이어 목록 UI를 갱신한다.
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        RefreshPlayerList();
    }

    // 게임 입장 버튼 클릭 시 Photon 방 상태에 맞게 게임 씬으로 이동한다.
    public void OnEnterGameButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LoadLevel(gameSceneName);
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    // 현재 방 정보와 플레이어 목록을 UI 텍스트에 반영한다.
    private void RefreshPlayerList()
    {
        if (roomNameText == null || playerCountText == null || playerListText == null)
        {
            return;
        }

        if (!PhotonNetwork.InRoom)
        {
            roomNameText.text = "Not In Room";
            playerCountText.text = "0 players";
            playerListText.text = string.Empty;
            return;
        }

        Room room = PhotonNetwork.CurrentRoom;
        roomNameText.text = room.Name;
        playerCountText.text = $"{room.PlayerCount}/{room.MaxPlayers} players online";

        playerListText.text = string.Join(
            "\n",
            room.Players.Values
                .OrderBy(player => player.ActorNumber)
                .Select(player => FormatPlayerName(player)));

        if (enterGameButton != null)
        {
            enterGameButton.interactable = room.PlayerCount > 0;
        }
    }

    // 플레이어의 표시 이름을 만들고 로컬 플레이어에는 표시를 덧붙인다.
    private string FormatPlayerName(Player player)
    {
        string nickname = string.IsNullOrWhiteSpace(player.NickName)
            ? $"Player {player.ActorNumber}"
            : player.NickName;

        return player.IsLocal ? $"{nickname} (Me)" : nickname;
    }
}
