using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomModule : MonoBehaviour
{
    public string moduleCode = "untagged";
    public bool unique = false;
    public int rarity = 1;
    public List<ModuleConnector> connectors;    

    private bool m_singleEntranceEnforced = false;
    private ModuleConnector m_entranceConnector;
    private int m_roomId = 0;
    private int m_rareMin, m_rareMax;
    private bool m_initialised;
    private List<ModuleConnector> m_remainingExits;

    public List<ModuleConnector> GetConnectors()
    {
        return connectors;
    }

    public List<ModuleConnector> GetExits()
    {
        EvaluateExits();
        return m_remainingExits;
    }

    public void EvaluateExits()
    {
        EnforceSingleEntrance();

        m_remainingExits = GetConnectors();

        for (int i = connectors.Count - 1; i > 1; i--)
        {
            if (i < 0 || i >= connectors.Count || i >= m_remainingExits.Count) return;
            if (connectors[i].linkedModule != null) { m_remainingExits.RemoveAt(i); return; }
            if (connectors[i].isEntrance == true) { m_remainingExits.RemoveAt(i); return; }
        }
    }

    public void SetRarityMinMax(int _min)
    {
        m_rareMin = _min;
        m_rareMax = _min + rarity;
    }
    public int GetMinRarity() { return m_rareMin; }
    public int GetMaxRarity() { return m_rareMax; }

    private void EnforceSingleEntrance()
    {
        if (m_singleEntranceEnforced) return;
        for(int i = 0; i < connectors.Count; i++)
        {
            if (connectors[i].isEntrance)
            {
                if (!m_singleEntranceEnforced)
                {
                    m_entranceConnector = connectors[i];
                }
                else
                {
                    connectors[i].isEntrance = false;
                }
                m_singleEntranceEnforced = true;
            }
        }
        m_singleEntranceEnforced = true;
    }
    public ModuleConnector GetEntrance()
    {
        if (m_entranceConnector != null) return m_entranceConnector;
        else return connectors[0];
    }

    public void SetUp()
    {
        foreach (ModuleConnector con in connectors)
        {
            con.allowedCodesArray = con.allowedCodes.Split(',');
            con.parentModule = this;
        }
    }

    public void SetId(int _id)
    {
        m_roomId = _id;
    }
    public int GetId()
    {
        return m_roomId;
    }
}

[System.Serializable]
public class ModuleConnector
{
    public Transform transformPoint;
    public bool isEntrance = false;
    public string allowedCodes;
    public string[] allowedCodesArray;

    public RoomModule parentModule;
    public RoomModule linkedModule;
}