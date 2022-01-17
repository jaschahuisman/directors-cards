using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using TMPro;

public class TheatreSceneManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;

    [Header("Briefing")]
    [SerializeField] private AudioSource audioSourcePlayer1;
    [SerializeField] private AudioSource audioSourcePlayer2;
    [SerializeField] private TextMeshProUGUI roleTextPlayer1, roleTextPlayer2;
    [SerializeField] private TextMeshProUGUI scenarioTextPlayer1, scenarioTextPlayer2;
    [SerializeField] private GameObject rolePanelPlayer1, rolePanelPlayer2;
    [SerializeField] private GameObject scenarioPanelPlayer1, scenarioPanelPlayer2;
    [SerializeField] private DoorManager door1, door2;

    [Header("Finish")]
    [SerializeField] private TimelineAsset finishTimeline;

    private static TheatreSceneManager instance;
    public static TheatreSceneManager Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }

    public void StartBriefing(Briefing briefing, NetworkPlayer player)
    {
        roleTextPlayer1.text = briefing.playerRole1.role;
        roleTextPlayer2.text = briefing.playerRole2.role;
        scenarioTextPlayer1.text = briefing.playerRole1.scenario;
        scenarioTextPlayer2.text = briefing.playerRole2.scenario;

        TimelineAsset timeline = briefing.timeline;
        playableDirector.playableAsset = timeline;

        foreach (TrackAsset track in timeline.GetOutputTracks())
        {
            track.muted = false;

            switch (track.name)
            {
                case "role_player_1":
                    playableDirector.SetGenericBinding(track, rolePanelPlayer1);
                    break;
                case "role_player_2":
                    playableDirector.SetGenericBinding(track, rolePanelPlayer2);
                    break;
                case "scenario_player_1":
                    playableDirector.SetGenericBinding(track, scenarioPanelPlayer1);
                    break;
                case "scenario_player_2":
                    playableDirector.SetGenericBinding(track, scenarioPanelPlayer2);
                    break;
                case "audio_player_1":
                    playableDirector.SetGenericBinding(track, audioSourcePlayer1);
                    if (player.team == PlayerType.Player2) track.muted = true;
                    break;
                case "audio_player_2":
                    playableDirector.SetGenericBinding(track, audioSourcePlayer2);
                    if (player.team == PlayerType.Player1) track.muted = true;
                    break;
                default:
                    break;
            }
        }

        playableDirector.Play();

        StartCoroutine(NotifyWhenBriefingFinished(player, (float) timeline.duration));
    }

    private IEnumerator NotifyWhenBriefingFinished(NetworkPlayer player, float delay)
    {
        yield return new WaitForSeconds(delay);
        player.FinishBriefing();
        OpenDoors();
    }

    public void OpenDoors()
    {
        door1.SetOpen(true);
        door2.SetOpen(true);
    }

    public void FinishScene(NetworkPlayer player)
    {
        player.playerGameplayManager.DestroyCards();
        playableDirector.playableAsset = finishTimeline;
        playableDirector.Play();

        StartCoroutine(NotifyWhenFinishedDone(player, (float) finishTimeline.duration));
    }

    private IEnumerator NotifyWhenFinishedDone(NetworkPlayer player, float delay)
    {
        yield return new WaitForSeconds(delay);
        player.FinishPlayerSceneDone();
    }
}
