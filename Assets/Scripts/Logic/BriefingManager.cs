using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.Timeline;


public class BriefingManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private AudioSource audioSourcePlayer1, audioSourcePlayer2;
    [SerializeField] private TextMeshProUGUI roleTextPlayer1, roleTextPlayer2;
    [SerializeField] private TextMeshProUGUI scenarioTextPlayer1, scenarioTextPlayer2;
    [SerializeField] private GameObject rolePanelPlayer1, rolePanelPlayer2;
    [SerializeField] private GameObject scenarioPanelPlayer1, scenarioPanelPlayer2;

    private static BriefingManager instance;
    public static BriefingManager Instance { get { return instance; } }

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
            Debug.Log(track.name);
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
                    if (player.Team == PlayerTeam.P2) track.muted = true;
                    break;
                case "audio_player_2":        
                    playableDirector.SetGenericBinding(track, audioSourcePlayer2);
                    if (player.Team == PlayerTeam.P1) track.muted = true;
                    break;
                default:
                    break;
            }
        }

        playableDirector.Play();
        StartCoroutine(NotifyBriefingFinished(player, (float) timeline.duration));
    }

    private IEnumerator NotifyBriefingFinished(NetworkPlayer player, float delay)
    {
        yield return new WaitForSeconds(delay);
        player.CmdFinishBriefing();
    }
}
