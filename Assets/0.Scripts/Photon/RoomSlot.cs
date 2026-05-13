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

    public void Initialize(RoomInfo room, Action<string> onJoinRoom = null)
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

        Destroy(gameObject);
    }
}
