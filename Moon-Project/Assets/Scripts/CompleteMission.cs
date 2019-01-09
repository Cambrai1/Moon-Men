using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteMission : MonoBehaviour
{

    public MissionLog missionLog;

    private void Start()
    {
        if (missionLog == null) missionLog = GameObject.FindGameObjectWithTag("Player").GetComponent<MissionLog>();
    }

    public void MissionComplete(string missionName)
    {
        missionLog.CompleteMission(missionName);
    }
}
