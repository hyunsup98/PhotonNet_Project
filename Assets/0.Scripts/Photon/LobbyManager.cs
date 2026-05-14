using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _createRoomButton;      // 빠른 방 생성 버튼

    [SerializeField] private Transform _roomListParent;     // 방 목록이 표시될 부모 트랜스폼
    [SerializeField] private RoomSlot _roomPrefab;          // 방 슬롯 프리팹

    private Dictionary<string, RoomInfo> _cachedRoomDic = new Dictionary<string, RoomInfo>();   // 방 목록 캐시 딕셔너리

    private void Start()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    // 빠른 방 생성 버튼 클릭 시 랜덤한 방 이름으로 방을 생성 또는 입장
    public void CreateRoom()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    private void RefreshRoomList()
    {
        // 기존 방 목록 UI 제거
        foreach (Transform child in _roomListParent)
        {
            Destroy(child.gameObject);
        }

        // 캐시된 방 목록을 기반으로 UI 생성
        foreach (RoomInfo room in _cachedRoomDic.Values)
        {
            if (!room.IsOpen || !room.IsVisible) continue;

            RoomSlot newSlot = Instantiate(_roomPrefab, _roomListParent);
            newSlot.Initialize(room);
        }


    }

    #region 포톤 콜백함수들
    public override void OnJoinedLobby()
    {
        // 로비 입장 시
        RefreshRoomList();
    }

    public override void OnLeftLobby()
    {
        // 로비 퇴장 시
        Debug.Log("로비에서 퇴장하였습니다.");
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("Room");
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 방 목록 업데이트 시
        _cachedRoomDic.Clear();

        foreach (RoomInfo room in roomList)
        {
            _cachedRoomDic[room.Name] = room;
        }

        RefreshRoomList();
    }
    #endregion
}
