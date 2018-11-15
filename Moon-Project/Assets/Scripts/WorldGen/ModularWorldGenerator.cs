﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ModularWorldGenerator : MonoBehaviour
{
    public GenerationRules generationRules;
    public bool neverStop = false;
    public bool autoModules = true;
	public RoomModule[] modules;
	public RoomModule startModule;
    public bool visibleIterations = true;
    public int maximumAttempts = 100;

    private int m_totalRarity;
    private List<GameObject> m_allRooms;

	void Start()
	{
        if (autoModules) GetAutoModules();
        foreach(RoomModule mod in modules)
        {
            mod.SetRarityMinMax(m_totalRarity);
            m_totalRarity += mod.rarity;
        }
        Debug.Log("Loaded " + modules.Length + " modules, total rarity " + m_totalRarity + "!");

        Regenerate();
	}

    private void GetAutoModules()
    {
        int childCount = transform.childCount;
        modules = new RoomModule[childCount];
        for (int i = 0; i < childCount; i++)
        {
            modules[i] = transform.GetChild(i).GetComponent<RoomModule>();
        }
        startModule = modules[0];
    }
    
    public void Regenerate()
    {
        StartCoroutine(Regenerate(generationRules.iterations));
    }
    private IEnumerator Regenerate(int _iterations)
    {
        GenerationMethod method = generationRules.generationMethod;
        RoomModule startingModule = Instantiate(startModule, transform.position, transform.rotation).GetComponent<RoomModule>();
        startingModule.gameObject.SetActive(true);
        bool success = false;
        int roomCount = 0;
        int currentAttempts = 0;
        m_allRooms = new List<GameObject>();
        float startTime = Time.time;

        while(!success && currentAttempts <= maximumAttempts)
        {
            success = true;

            if(m_allRooms.Count >= 1)
            {
                foreach (GameObject obj in m_allRooms)
                {
                    GameObject.Destroy(obj);
                }
            }
            m_allRooms = new List<GameObject>();
            roomCount = 0;
            List<ModuleConnector> pendingConnections = new List<ModuleConnector>(startModule.GetExits());
            switch (method)
            {
                case GenerationMethod.widthFirst:
                    {
                        for (int i = 0; i < _iterations; i++)
                        {
                            while (pendingConnections.Count >= 1 && roomCount <= generationRules.maximumRooms)
                            {
                                if (pendingConnections[0] != null)
                                {
                                    RoomModule newModule = GameObject.Instantiate(GetRandomModuleForConnector(pendingConnections[0], true), transform).GetComponent<RoomModule>();
                                    AlignConnectors(pendingConnections[0], newModule.GetEntrance());
                                    newModule.gameObject.SetActive(true);
                                    LinkModules(pendingConnections[0], newModule.GetEntrance());
                                    newModule.SetId(roomCount);
                                    Debug.Log("Spawned Room " + roomCount + ": " + newModule.moduleCode);
                                    newModule.gameObject.name = ("Room " + roomCount + " : " + newModule.moduleCode);
                                    m_allRooms.Add(newModule.gameObject);
                                    roomCount++;
                                    pendingConnections.RemoveAt(0);

                                    foreach (ModuleConnector con in newModule.GetExits())
                                    {
                                        pendingConnections.Add(con);
                                    }
                                    pendingConnections.Remove(newModule.GetEntrance());

                                    if (visibleIterations) yield return new WaitForEndOfFrame();
                                }
                                else
                                {
                                    Debug.LogWarning("PENDING CONNECTIONS [0] SOMEHOW IS NULL");
                                }
                            }
                        }
                    }
                    break;

                case GenerationMethod.depthFirst:
                    {
                        for (int i = 0; i < _iterations; i++)
                        {
                            while (pendingConnections.Count >= 1 && roomCount <= generationRules.maximumRooms)
                            {
                                int c = pendingConnections.Count - 1;
                                if (pendingConnections[c] != null)
                                {
                                    RoomModule newModule = Instantiate(GetRandomModuleForConnector(pendingConnections[c], true), transform).GetComponent<RoomModule>();
                                    AlignConnectors(pendingConnections[c], newModule.GetEntrance());
                                    newModule.gameObject.SetActive(true);
                                    LinkModules(pendingConnections[c], newModule.GetEntrance());
                                    newModule.SetId(roomCount);
                                    Debug.Log("Spawned Room " + roomCount + ": " + newModule.moduleCode);
                                    newModule.gameObject.name = ("Room " + roomCount + " : " + newModule.moduleCode); 
                                    m_allRooms.Add(newModule.gameObject);
                                    roomCount++;
                                    pendingConnections.RemoveAt(c);

                                    foreach (ModuleConnector con in newModule.GetExits())
                                    {
                                        pendingConnections.Add(con);
                                    }
                                    pendingConnections.Remove(newModule.GetEntrance());

                                    if (visibleIterations) yield return new WaitForEndOfFrame();
                                }
                                else
                                {
                                    Debug.LogWarning("PENDING CONNECTIONS [" + c + "] SOMEHOW IS NULL");
                                }
                            }
                        }
                    }
                    break;
            }

            if (roomCount <= generationRules.minimumRooms) success = false;
            if (roomCount >= generationRules.maximumRooms) success = false;
            if (neverStop) success = false;

            currentAttempts++;
        }

        Debug.Log("GENERATED WORLD IN " + (Time.time - startTime) + " SECONDS");

        yield return null;
    }

    private void LinkModules(ModuleConnector _a, ModuleConnector _b)
    {
        _a.linkedModule = _b.parentModule;
        _b.linkedModule = _a.parentModule;
    }

	private void AlignConnectors(ModuleConnector oldExit, ModuleConnector newExit)
	{
        Transform newModule = newExit.parentModule.gameObject.transform;
		var forwardVectorToMatch = -oldExit.transformPoint.forward;
		var correctiveRotation = Azimuth(forwardVectorToMatch) - Azimuth(newExit.transformPoint.forward);
		newModule.RotateAround(newExit.transformPoint.position, Vector3.up, correctiveRotation);
		var correctiveTranslation = oldExit.transformPoint.position - newExit.transformPoint.position;
		newModule.transform.position += correctiveTranslation;
	}

	private GameObject GetRandomModuleForConnector(ModuleConnector _connector, bool _useRarity)
	{
        bool foundModule = false;

        if (_connector == null) return GetSpecificModule("straight");

        int useModId = 0;

        while (!foundModule)
        {
            if(_useRarity)
            {
                int random = Random.Range(0, m_totalRarity);
                for (int i = 0; i < modules.Length; i++)
                {
                    if (!foundModule)
                    {
                        if(modules[i].unique)
                        {

                        }
                        else if (random >= modules[i].GetMinRarity() && random <= modules[i].GetMaxRarity())
                        {
                            useModId = i;
                            foundModule = true;
                        }
                    }
                }
            }
            else
            {
                int random = Random.Range(0, _connector.allowedCodesArray.Length);
                string randomModuleCode = _connector.allowedCodesArray[random];
                for (int i = 0; i < modules.Length; i++)
                {
                    if (modules[i].unique)
                    {

                    }
                    else if (modules[i].moduleCode == randomModuleCode)
                    {
                        useModId = i;
                        foundModule = true;
                    }
                }
            }
        }

        return modules[useModId].gameObject;
    }
    private GameObject GetSpecificModule(string _code)
    {
        for (int i = 0; i < modules.Length; i++)
        {
            if (modules[i].moduleCode == _code)
            {
                return modules[i].gameObject;
            }
        }
        return null;
    }

	private static float Azimuth(Vector3 vector)
	{
		return Vector3.Angle(Vector3.forward, vector) * Mathf.Sign(vector.x);
	}
}
