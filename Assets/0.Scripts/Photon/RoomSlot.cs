using System;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_Text joinButtonText;

    private void Awake()
    {
        if (roomNameText == null)
        {
            roomNameText = transform.Find("Room Info/Room Name Text")?.GetComponent<TMP_Text>();
        }

        if (playerCountText == null)
        {
            playerCountText = transform.Find("Room Info/Player Count Text")?.GetComponent<TMP_Text>();
        }

        if (joinButton == null)
        {
            joinButton = GetComponentInChildren<Button>();
        }

        if (joinButtonText == null && joinButton != null)
        {
            joinButtonText = joinButton.GetComponentInChildren<TMP_Text>();
        }
    }

    public void Initialize(RoomInfo room, Action<string> onJoinRoom)
    {
        if (roomNameText != null)
        {
            roomNameText.text = room.Name;
        }

        if (playerCountText != null)
        {
            playerCountText.text = $"{room.PlayerCount}/{room.MaxPlayers} players";
        }

        bool isFull = room.PlayerCount >= room.MaxPlayers;

        if (joinButtonText != null)
        {
            joinButtonText.text = isFull ? "Full" : "Join";
        }

        if (joinButton != null)
        {
            joinButton.interactable = !isFull;
            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(() => onJoinRoom?.Invoke(room.Name));
        }
    }

    public void Release()
    {
        if (joinButton != null)
        {
            joinButton.onClick.RemoveAllListeners();
        }

        gameObject.SetActive(false);
    }
}
