using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class VoiceManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool transmitEnabled = true;
    [SerializeField] private bool voiceDetection = true;
    [SerializeField] private float speakerVolume = 1f;

    private PunVoiceClient voiceClient;
    private Recorder recorder;

    private void Awake()
    {
        SetupVoiceClient();
        SetupRecorder();
        SetupSpeakerPrefab();
    }

    private void Start()
    {
        TryConnectVoiceRoom();
    }

    public override void OnJoinedRoom()
    {
        TryConnectVoiceRoom();
    }

    public override void OnLeftRoom()
    {
        if (voiceClient != null && voiceClient.Client.IsConnected)
        {
            voiceClient.Disconnect();
        }
    }

    public void SetTransmitEnabled(bool enabled)
    {
        transmitEnabled = enabled;

        if (recorder != null)
        {
            recorder.TransmitEnabled = enabled;
        }
    }

    private void SetupVoiceClient()
    {
        voiceClient = PunVoiceClient.Instance;
        voiceClient.AutoConnectAndJoin = true;
        voiceClient.UsePunAppSettings = true;
        voiceClient.UsePunAuthValues = true;
    }

    private void SetupRecorder()
    {
        recorder = GetComponent<Recorder>();

        if (recorder == null)
        {
            recorder = gameObject.AddComponent<Recorder>();
        }

        recorder.SourceType = Recorder.InputSourceType.Microphone;
        recorder.MicrophoneType = Recorder.MicType.Unity;
        recorder.RecordWhenJoined = true;
        recorder.RecordingEnabled = true;
        recorder.TransmitEnabled = transmitEnabled;
        recorder.VoiceDetection = voiceDetection;

        voiceClient.PrimaryRecorder = recorder;
        voiceClient.AddRecorder(recorder);
    }

    private void SetupSpeakerPrefab()
    {
        if (voiceClient.SpeakerPrefab != null)
        {
            return;
        }

        GameObject speakerPrefab = new GameObject("Voice Speaker");
        speakerPrefab.transform.SetParent(transform, false);

        AudioSource audioSource = speakerPrefab.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = speakerVolume;

        speakerPrefab.AddComponent<Speaker>();
        voiceClient.SpeakerPrefab = speakerPrefab;
    }

    private void TryConnectVoiceRoom()
    {
        if (voiceClient == null || !PhotonNetwork.InRoom || voiceClient.Client.InRoom)
        {
            return;
        }

        voiceClient.ConnectAndJoinRoom();
    }
}
