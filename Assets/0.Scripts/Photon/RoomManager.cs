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

    private void Start()
    {
        RefreshPlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RefreshPlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RefreshPlayerList();
    }

    public override void OnJoinedRoom()
    {
        RefreshPlayerList();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        RefreshPlayerList();
    }

    public void OnEnterGameButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LoadLevel(gameSceneName);
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

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

    private string FormatPlayerName(Player player)
    {
        string nickname = string.IsNullOrWhiteSpace(player.NickName)
            ? $"Player {player.ActorNumber}"
            : player.NickName;

        return player.IsLocal ? $"{nickname} (Me)" : nickname;
    }
}
