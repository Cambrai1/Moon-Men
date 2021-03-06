﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomModule : MonoBehaviour
{
    public string moduleCode = "error";
    public bool unique = false;
    public bool includeInCount = true;
    public int abundance = 1;
    public List<ModuleConnector> connectors;
    public BoxCollider safetyBox;
    public List<Transform> raycastCheckers;
    public List<ReflectionProbe> reflectionProbes;

    private bool m_singleEntranceEnforced = false;
    private ModuleConnector m_entranceConnector;
    private int m_roomId = 0;
    private int m_rareMin, m_rareMax;
    private bool m_initialised;
    private List<ModuleConnector> m_remainingExits;

    private GameObject m_mapObject;

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
        m_rareMax = _min + abundance;
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
        if (!safetyBox) safetyBox = GetComponent<BoxCollider>();
        safetyBox.enabled = false;
        reflectionProbes = new List<ReflectionProbe>(GetComponentsInChildren<ReflectionProbe>());        
    }

    private void SetMapObject()
    {
        m_mapObject = transform.Find("MapObject").gameObject;
    }
    public GameObject GetMapObject()
    {
        return m_mapObject;
    }

    public void SetId(int _id)
    {
        m_roomId = _id;
    }
    public int GetId()
    {
        return m_roomId;
    }

    public void OnGenerationComplete()
    {
        foreach(ReflectionProbe probe in reflectionProbes)
        {
            probe.RenderProbe();
        }

        if (!m_mapObject && moduleCode != "null") SetMapObject();
        if (m_mapObject)
        {
            m_mapObject.layer = LayerMask.NameToLayer("Map");
            Collider col = m_mapObject.GetComponent<Collider>();
            if (col) col.enabled = false;
            m_mapObject.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            m_mapObject.GetComponent<Renderer>().receiveShadows = false;
        }
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
    private int linkedUniqueId = 0;
    private int uniqueId = 0;

    public int LinkedUniqueId
    {
        get{return linkedUniqueId;}
        set{linkedUniqueId = value;}
    }
    public int UniqueId
    {
        get{return uniqueId;}
        set{uniqueId = value;}
    }
}