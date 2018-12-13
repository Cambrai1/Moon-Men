using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionLog : MonoBehaviour
{
    [SerializeField]
    private List<Mission> m_allMissions;                   //  A list of all loaded missions
    [SerializeField]
    private List<Mission> m_activeMissions;                //  A list of all active missions
    [SerializeField]
    private List<Mission> m_completedMissions;             //  A list of all completed missions

    [SerializeField]
    private GameObject m_missionUiPrefab;               //  The prefab to use when creating new missions

    [SerializeField]
    private GameObject m_activeMissionsPanel;           //  The panel containing active missions
    [SerializeField]
    private GameObject m_completedMissionsPanel;        //  The panel containing completed missions
    [SerializeField]
    private Text m_missionDetailsText;                  //  The text detailing the selected mission




    public void ActivateMission(string _missionName)
    {
        foreach(Mission m in m_allMissions)
        {
            if(m.name == _missionName)
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
            Debug.Log("'" + _mission.name + "' already active.");
        }
        else
        {
            ForceActivateMission(_mission);
        }
    }
    public void ForceActivateMission(Mission mission)
    {
        m_activeMissions.Add(mission);
    }

    public void CompleteMission(string _missionName)
    {
        foreach(Mission m in m_activeMissions)
        {
            if(m.name == _missionName)
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
            Debug.Log("'" + _mission.name + "' already completed.");
        }
        else
        {
            ForceCompleteMission(_mission);
        }
    }
    public void ForceCompleteMission(Mission _mission)
    {

    }
}

[CreateAssetMenu(fileName = "NewMission", menuName = "Mission", order = 1)]
public class Mission : ScriptableObject
{
    public string title = "New Mission";
    public string description = "Do a thing";
    public List<ScriptableObject> unlocksMissions;
}