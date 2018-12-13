using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionLog : MonoBehaviour
{
    public List<Mission> allMissions;
    public List<Mission> activeMissions;
    public List<Mission> completedMissions;

    public GameObject missionUiPrefab;
}

[CreateAssetMenu(fileName = "NewMission", menuName = "Mission", order = 1)]
public class Mission : ScriptableObject
{
    public string title = "New Mission";
    public string description = "Do a thing";
}