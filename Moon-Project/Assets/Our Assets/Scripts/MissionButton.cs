using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionButton : MonoBehaviour
{
    public MissionLog missionLog;
    private Text m_text;

    private void Start()
    {
        if (missionLog == null) missionLog = GameObject.FindGameObjectWithTag("Player").GetComponent<MissionLog>();
        m_text = GetComponentInChildren<Text>();
    }

    public void SelectMission()
    {
        missionLog.SelectMission(m_text);
    }
}