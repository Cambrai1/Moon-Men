using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MissionLog : MonoBehaviour
{
    [SerializeField]
    private List<Mission> m_allMissions;            //  A list of all loaded missions
    [SerializeField]
    private List<Mission> m_activeMissions;         //  A list of all active missions
    [SerializeField]
    private List<Mission> m_completedMissions;      //  A list of all completed missions

    [SerializeField]
    private GameObject m_missionUiPrefab;           //  The prefab to use when creating new missions

    [SerializeField]
    private Transform m_activeMissionsPanel;       //  The panel containing active missions
    [SerializeField]
    private Transform m_completedMissionsPanel;    //  The panel containing completed missions
    [SerializeField]
    private Text m_missionDetailsText;              //  The text detailing the selected mission


    private void Start()
    {
        foreach(Mission m in m_allMissions)
        {
            if (m.activeByDefault) ActivateMission(m);
        }
    }


    public void ActivateMission(string _missionName)
    {
        foreach(Mission m in m_allMissions)
        {
            if(m.title == _missionName)
            {
                ActivateMission(m);
                return;
            }
        }
    }
    public void ActivateMission(Mission _mission)
    {
        if(m_activeMissions.Contains(_mission))
        {
            Debug.Log("'" + _mission.title + "' already active.");
        }
        else
        {
            ForceActivateMission(_mission);
        }
    }
    public void ForceActivateMission(Mission mission)
    {
        m_activeMissions.Add(mission);
        RefreshUi();
    }

    public void CompleteMission(string _missionName)
    {
        foreach(Mission m in m_activeMissions)
        {
            if(m.title == _missionName)
            {
                CompleteMission(m);
                return;
            }
        }
    }
    public void CompleteMission(Mission _mission)
    {
        if(m_completedMissions.Contains(_mission))
        {
            Debug.Log("'" + _mission.title + "' already completed.");
        }
        else
        {
            ForceCompleteMission(_mission);
        }
    }
    public void ForceCompleteMission(Mission _mission)
    {
        m_completedMissions.Add(_mission);
        m_activeMissions.Remove(_mission);
        RefreshUi();
    }



    public void RefreshUi()
    {
        foreach(Transform t in m_activeMissionsPanel.GetComponentsInChildren<Transform>())
        {
            if(t != m_activeMissionsPanel.transform)
            GameObject.Destroy(t.gameObject);
        }
        foreach (Transform t in m_completedMissionsPanel.GetComponentsInChildren<Transform>())
        {
            if (t != m_completedMissionsPanel.transform)
                GameObject.Destroy(t.gameObject);
        }

        foreach(Mission m in m_activeMissions)
        {
            GameObject missionUi = Instantiate(m_missionUiPrefab, m_activeMissionsPanel);
            missionUi.GetComponentInChildren<Text>().text = m.title;
        }
        foreach (Mission m in m_completedMissions)
        {
            GameObject missionUi = Instantiate(m_missionUiPrefab, m_completedMissionsPanel);
            missionUi.GetComponentInChildren<Text>().text = m.title;
        }

        SelectMission(m_activeMissions[0]);
    }
    public void SelectMission(Text _missionText)
    {
        foreach (Mission m in m_allMissions)
        {
            if (m.title == _missionText.text)
            {
                SelectMission(m);
                return;
            }
        }
    }
    public void SelectMission(Mission _mission)
    {
        m_missionDetailsText.text = _mission.description;
    }
}

[CreateAssetMenu(fileName = "NewMission", menuName = "Mission", order = 1)]
public class Mission : ScriptableObject
{
    public string title = "NewMission";
    public string description = "Do a thing";
    public List<ScriptableObject> unlocksMissions;
    public bool activeByDefault = false;
}