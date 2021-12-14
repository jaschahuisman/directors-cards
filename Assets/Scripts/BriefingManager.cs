using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class BriefingManager : MonoBehaviour
{
    [Header("Player 1 UI components")]
    public Text roleTextPlayer1;
    public Text scenarioTextPlayer1;
    [Header("Player 2 UI components")]
    public Text roleTextPlayer2;
    public Text scenarioTextPlayer2;

    [Header("Timeline object bindings")]
    public GameObject rolePanel1;
    public GameObject scenarioPanel1;
    public GameObject rolePanel2;
    public GameObject scenarioPanel2;

    private PlayableDirector playableDirector;

    public static BriefingManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playableDirector = gameObject.GetComponent<PlayableDirector>();
    }

    public void StartBriefing(BriefingScriptable briefing, NetworkPlayer player)
    {
        roleTextPlayer1.text = briefing.playerRole1.role;
        scenarioTextPlayer1.text = briefing.playerRole1.scenario;

        roleTextPlayer2.text = briefing.playerRole2.role;
        scenarioTextPlayer2.text = briefing.playerRole2.scenario;

        TimelineAsset timeline = briefing.timeline;
        playableDirector.playableAsset = timeline;

        foreach (TrackAsset track in timeline.GetOutputTracks())
        {
            track.muted = false;

            if (track.name == "role_player_1") { playableDirector.SetGenericBinding(track, rolePanel1);  }
            if (track.name == "scenario_player_1") { playableDirector.SetGenericBinding(track, scenarioPanel1); }
            if (track.name == "role_player_2") { playableDirector.SetGenericBinding(track, rolePanel2); }
            if (track.name == "scenario_player_2") { playableDirector.SetGenericBinding(track, scenarioPanel2); }

            if (track.name == "audio_player_1") 
            {
                playableDirector.SetGenericBinding(track, player.gameObject.GetComponent<AudioSource>()); 
                if (player.id == PlayerId.Player1) track.muted = true;
            } 

            if (track.name == "audio_player_2") 
            { 
                playableDirector.SetGenericBinding(track, player.GetComponent<AudioSource>()); 
                if (player.id == PlayerId.Player2) track.muted = true;
            }
        }

        playableDirector.Play();
    }


}
