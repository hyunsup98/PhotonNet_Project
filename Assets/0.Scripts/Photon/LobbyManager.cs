using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string roomSceneName = "Room";
    [SerializeField] private byte maxPlayersPerRoom = 2;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private RectTransform roomListContent;
    [SerializeField] private RectTransform roomRowTemplate;
    [SerializeField] private TMP_Text emptyRoomListText;

    private bool isJoining;
    private readonly Dictionary<string, RoomInfo> cachedRooms = new Dictionary<string, RoomInfo>();

    private void Start()
    {
        if (roomRowTemplate != null)
        {
            roomRowTemplate.gameObject.SetActive(false);
        }

        SetStatus(PhotonNetwork.IsConnectedAndReady ? "Lobby Ready" : "Connecting...");
        SetButtonInteractable(PhotonNetwork.IsConnectedAndReady);
        RefreshRoomList();

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        SetStatus("Lobby Ready");
        SetButtonInteractable(true);
    }

    public void OnCreateRoomButtonClicked()
    {
        if (isJoining || !PhotonNetwork.IsConnectedAndReady)
        {
            return;
        }

        isJoining = true;
        SetButtonInteractable(false);
        SetStatus("Finding Room...");

        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinRoomByName(string roomName)
    {
        if (isJoining || !PhotonNetwork.IsConnectedAndReady || string.IsNullOrWhiteSpace(roomName))
        {
            return;
        }

        isJoining = true;
        SetButtonInteractable(false);
        SetStatus($"Joining {roomName}...");
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList || !room.IsVisible || !room.IsOpen)
            {
                cachedRooms.Remove(room.Name);
                continue;
            }

            cachedRooms[room.Name] = room;
        }

        RefreshRoomList();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        SetStatus("Creating Room...");

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = maxPlayersPerRoom,
            IsOpen = true,
            IsVisible = true
        };

        PhotonNetwork.CreateRoom($"Room_{Guid.NewGuid():N}".Substring(0, 13), roomOptions);
    }

    public override void OnJoinedRoom()
    {
        SetStatus($"Joined Room ({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})");
        PhotonNetwork.LoadLevel(roomSceneName);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        isJoining = false;
        SetStatus($"Create Failed: {message}");
        SetButtonInteractable(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        isJoining = false;
        SetStatus($"Join Failed: {message}");
        SetButtonInteractable(true);
    }

    private void SetButtonInteractable(bool interactable)
    {
        if (createRoomButton != null)
        {
            createRoomButton.interactable = interactable;
        }
    }

    private void SetStatus(string status)
    {
        if (statusText != null)
        {
            statusText.text = status;
        }
    }

    private void RefreshRoomList()
    {
        if (roomListContent == null || roomRowTemplate == null)
        {
            return;
        }

        foreach (Transform child in roomListContent)
        {
            if (child != roomRowTemplate)
            {
                Destroy(child.gameObject);
            }
        }

        List<RoomInfo> openRooms = cachedRooms.Values
            .Where(room => room.IsOpen && room.IsVisible)
            .OrderBy(room => room.PlayerCount >= room.MaxPlayers)
            .ThenBy(room => room.Name)
            .ToList();

        if (emptyRoomListText != null)
        {
            emptyRoomListText.gameObject.SetActive(openRooms.Count == 0);
        }

        for (int i = 0; i < openRooms.Count; i++)
        {
            AddRoomRow(openRooms[i], i);
        }
    }

    private void AddRoomRow(RoomInfo room, int index)
    {
        RectTransform row = Instantiate(roomRowTemplate, roomListContent);
        row.gameObject.SetActive(true);
        row.anchorMin = new Vector2(0f, 1f);
        row.anchorMax = new Vector2(1f, 1f);
        row.pivot = new Vector2(0.5f, 1f);
        row.offsetMin = new Vector2(0f, row.offsetMin.y);
        row.offsetMax = new Vector2(0f, row.offsetMax.y);
        row.anchoredPosition = new Vector2(0f, -index * 98f);
        row.sizeDelta = new Vector2(0f, 86f);

        roomListContent.sizeDelta = new Vector2(roomListContent.sizeDelta.x, Mathf.Max(0f, (index + 1) * 98f));

        TMP_Text nameText = row.Find("Room Name Text")?.GetComponent<TMP_Text>();
        TMP_Text countText = row.Find("Player Count Text")?.GetComponent<TMP_Text>();
        Button joinButton = row.GetComponentInChildren<Button>();

        if (nameText != null)
        {
            nameText.text = room.Name;
        }

        if (countText != null)
        {
            countText.text = $"{room.PlayerCount}/{room.MaxPlayers} players";
        }

        if (joinButton != null)
        {
            joinButton.interactable = room.PlayerCount < room.MaxPlayers;
            joinButton.GetComponentInChildren<TMP_Text>().text = room.PlayerCount >= room.MaxPlayers ? "Full" : "Join";

            string roomName = room.Name;
            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(() => JoinRoomByName(roomName));
        }
    }
}
