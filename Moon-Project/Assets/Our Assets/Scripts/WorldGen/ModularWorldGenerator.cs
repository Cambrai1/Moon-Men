﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class ModularWorldGenerator : MonoBehaviour
{
    public GenerationRules generationRules;
    public bool neverStop = false;
    public bool autoModules = true;
	public RoomModule[] loadedModules;
	public RoomModule startModule;
    public RoomModule nullModule;
    public GameObject doorPrefab;
    public GameObject wallPrefab;
    public bool visibleIterations = true;
    public bool showDebug = true;
    [Range(1,1000)]
    public int maximumAttempts = 100;

    private int m_totalRarity;
    private List<RoomModule> m_spawnedModules;
    private List<DoorController> m_spawnedDoors;
    private List<GameObject> m_spawnedWalls;
    private int m_roomCount = 0;
    private int m_currentAttempts = 0;
    private int m_roomTry = 0;
    private List<ModuleConnector> m_pendingConnections;

    private Text timeUi, genUi, modulesUi;

    public bool generationComplete = true;

    [SerializeField]
    private string worldSeed;

    void Start()
	{
        timeUi = GameObject.Find("Time").GetComponent<Text>();
        genUi = GameObject.Find("Generation").GetComponent<Text>();
        modulesUi = GameObject.Find("Module").GetComponent<Text>();

        generationRules.SetUp();
        if (autoModules) LoadModulesFromChildren();
        foreach (RoomModule mod in loadedModules)
        {
            mod.gameObject.SetActive(false);
        }

        if(worldSeed == "")
        {
            Regenerate();
        }
        else
        {
            GenerateFromSeed(worldSeed);
        }
	}

    public void LoadModulesFromChildren()
    {
        m_totalRarity = 0;
        int childCount = transform.childCount;
        loadedModules = new RoomModule[childCount];
        for (int i = 0; i < childCount; i++)
        {
            loadedModules[i] = transform.GetChild(i).GetComponent<RoomModule>();
        }
        if(!startModule) startModule = loadedModules[0];
        foreach (RoomModule mod in loadedModules)
        {
            mod.SetRarityMinMax(m_totalRarity);
            m_totalRarity += mod.abundance;
        }
        Debug.Log("Loaded " + loadedModules.Length + " modules, total rarity " + m_totalRarity + "!");
    }

    public void ResetWorld()
    {
        if(m_spawnedModules != null)
        {
            foreach (RoomModule mod in m_spawnedModules)
            {
                GameObject.Destroy(mod.gameObject);
            }
        }
        if (m_spawnedDoors != null)
        {
            foreach (DoorController door in m_spawnedDoors)
            {
                GameObject.Destroy(door.gameObject);
            }
        }
        if (m_spawnedWalls != null)
        {
            foreach (GameObject wall in m_spawnedWalls)
            {
                GameObject.Destroy(wall.gameObject);
            }
        }
        m_roomCount = 0;
        m_currentAttempts = 0;
        m_spawnedModules = new List<RoomModule>();
    }
    
    public void Regenerate()
    {
        generationComplete = false;
        ResetWorld();

        if(visibleIterations || neverStop)
        {
            StartCoroutine(RegenerateCoroutine());
        }
        else
        {
            bool success = false;
            string failReason = "";
            int failReasons = 0;
            float startTime = Time.time;

            while (!success && (m_currentAttempts <= maximumAttempts || neverStop))
            {
                success = true;

                if (m_spawnedModules.Count >= 1)
                {
                    foreach (RoomModule mod in m_spawnedModules)
                    {
                        GameObject.Destroy(mod.gameObject);
                    }
                }
                m_spawnedModules = new List<RoomModule>();
                m_roomCount = 0;
                m_roomTry = 0;

                RoomModule startingModule = Instantiate(startModule, transform.position, transform.rotation).GetComponent<RoomModule>();
                startingModule.gameObject.SetActive(true);
                m_spawnedModules.Add(startingModule);

                m_pendingConnections = new List<ModuleConnector>(startingModule.GetExits());
                switch (generationRules.generationMethod)
                {
                    case GenerationMethod.original:
                        {
                            while (m_pendingConnections.Count >= 1 && m_roomCount <= generationRules.maximumRooms)
                            {
                                if (m_pendingConnections[0] != null)
                                {
                                    RoomModule newModule = TrySpawnRandomModule(m_pendingConnections[0]);
                                }
                                else
                                {
                                    Debug.LogWarning("PENDING CONNECTIONS [0] SOMEHOW IS NULL");
                                    m_pendingConnections.RemoveAt(0);
                                }

                                timeUi.text = "Time : " + (Time.time - startTime);
                                modulesUi.text = "Modules : " + m_spawnedModules.Count;
                            }
                        }
                        break;

                    case GenerationMethod.predictive:
                        {

                        }
                        break;
                }

                if (m_roomCount <= generationRules.minimumRooms)
                {
                    failReason += ("\nModule count " + m_roomCount + " below minimum of " + generationRules.minimumRooms + ".");
                    failReasons++;
                    success = false;
                }
                if (m_roomCount >= generationRules.maximumRooms)
                {
                    failReason += ("\nModule count " + m_roomCount + " above maximum of " + generationRules.maximumRooms + ".");
                    failReasons++;
                    success = false;
                }
                foreach (ModuleRule rule in generationRules.moduleRules)
                {
                    if (!TestModuleRule(rule))
                    {
                        success = false;
                        failReason += ("\nModule rule '" + rule.moduleCode + "' violated.");
                        failReasons++;
                    }
                }
                if (neverStop) success = false;
                if (!success && !neverStop && showDebug)
                {
                    if (failReasons >= 2)
                    {
                        failReason = "FAIL REASON :\nMultiple Reasons" + failReason;
                    }
                    else
                    {
                        failReason = "FAIL REASON :" + failReason;
                    }
                    Debug.LogWarning(failReason);
                    failReason = "";
                    failReasons = 0;
                }
                m_currentAttempts++;

                genUi.text = "Generation : " + m_currentAttempts;
            }

            timeUi.text = "Time : " + (Time.time - startTime);
            genUi.text = "Generation : " + m_currentAttempts;
            modulesUi.text = "Modules : " + m_spawnedModules.Count;
            Debug.Log("GENERATED WORLD IN " + (Time.time - startTime) + " SECONDS AFTER " + m_currentAttempts + " ATTEMPTS!");

            CreateWorldSeed();
            StartCoroutine(OnGenerationComplete());
        }
    }
    private IEnumerator RegenerateCoroutine()
    {
        bool success = false;
        string failReason = "";
        int failReasons = 0;
        float startTime = Time.time;

        while(!success && (m_currentAttempts <= maximumAttempts || neverStop))
        {
            success = true;

            if (m_spawnedModules.Count >= 1)
            {
                foreach (RoomModule mod in m_spawnedModules)
                {
                    GameObject.Destroy(mod.gameObject);
                }
            }
            m_spawnedModules = new List<RoomModule>();
            m_roomCount = 0;
            m_roomTry = 0;

            RoomModule startingModule = Instantiate(startModule, transform.position, transform.rotation).GetComponent<RoomModule>();
            startingModule.gameObject.SetActive(true);
            m_spawnedModules.Add(startingModule);

            m_pendingConnections = new List<ModuleConnector>(startingModule.GetExits());
            switch (generationRules.generationMethod)
            {
                case GenerationMethod.original:
                    {
                        while (m_pendingConnections.Count >= 1 && m_roomCount <= generationRules.maximumRooms)
                        {
                            if (m_pendingConnections[0] != null)
                            {
                                RoomModule newModule = TrySpawnRandomModule(m_pendingConnections[0]);
                            }
                            else
                            {
                                Debug.LogWarning("PENDING CONNECTIONS [0] SOMEHOW IS NULL");
                                m_pendingConnections.RemoveAt(0);
                            }

                            timeUi.text = "Time : " + (Time.time - startTime);
                            modulesUi.text = "Modules : " + m_spawnedModules.Count;
                            if (visibleIterations) yield return null;
                        }
                    }
                    break;

                case GenerationMethod.predictive:
                    {

                    }
                    break;
            }

            if (m_roomCount <= generationRules.minimumRooms)
            {
                failReason += ("\nModule count " + m_roomCount + " below minimum of " + generationRules.minimumRooms + ".");
                failReasons++;
                success = false;
            }
            if (m_roomCount >= generationRules.maximumRooms)
            {
                failReason += ("\nModule count " + m_roomCount + " above maximum of " + generationRules.maximumRooms + ".");
                failReasons++;
                success = false;
            }
            foreach(ModuleRule rule in generationRules.moduleRules)
            {
                if (!TestModuleRule(rule))
                {
                    success = false;
                    failReason += ("\nModule rule '" + rule.moduleCode + "' violated.");
                    failReasons++;
                }
            }
            if (neverStop) success = false;
            if(!success && !neverStop && showDebug)
            {
                if(failReasons >= 2)
                {
                    failReason = "FAIL REASON :\nMultiple Reasons" + failReason;
                }
                else
                {
                    failReason = "FAIL REASON :" + failReason;
                }
                Debug.LogWarning(failReason);
                failReason = "";
                failReasons = 0;
            }
            m_currentAttempts++;

            genUi.text = "Generation : " + m_currentAttempts;
        }

        GenerateDoorsAndWalls();

        timeUi.text = "Time : " + (Time.time - startTime);
        genUi.text = "Generation : " + m_currentAttempts;
        modulesUi.text = "Modules : " + m_spawnedModules.Count;
        Debug.Log("GENERATED WORLD IN " + (Time.time - startTime) + " SECONDS AFTER " + m_currentAttempts + " ATTEMPTS!");

        CreateWorldSeed();
        StartCoroutine(OnGenerationComplete());

        yield return null;
    }

    private IEnumerator OnGenerationComplete()
    {
        foreach (RoomModule module in m_spawnedModules)
        {
            module.OnGenerationComplete();
            yield return null;
        }

        generationComplete = true;

        yield return null;
    }


    private void GenerateDoorsAndWalls()
    {
        m_spawnedDoors = new List<DoorController>();
        m_spawnedWalls = new List<GameObject>();
        List<int> proccessedConnectors = new List<int>();
        foreach(RoomModule module in m_spawnedModules)
        {
            foreach(ModuleConnector connector in module.GetConnectors())
            {
                if(connector.linkedModule == null)
                {
                    //  Spawn wall
                    GameObject wall = Instantiate(wallPrefab, connector.transformPoint.position, connector.transformPoint.rotation, module.transform);
                    proccessedConnectors.Add(connector.UniqueId);
                    m_spawnedWalls.Add(wall);
                }
                else
                {
                    if (proccessedConnectors.Contains(connector.LinkedUniqueId) == false)
                    {
                        //  Spawn door
                        if(Random.Range(0f,1f) >= 0.5f)
                        {
                            DoorController cont = Instantiate(doorPrefab, connector.transformPoint.position, connector.transformPoint.rotation, module.transform).GetComponent<DoorController>();
                            m_spawnedDoors.Add(cont);
                        }
                        proccessedConnectors.Add(connector.UniqueId);
                        proccessedConnectors.Add(connector.LinkedUniqueId);                        
                    }
                }
            }
        }
    }

    public List<RoomModule> GetSpawnedModules()
    {
        return m_spawnedModules;
    }

    private RoomModule TrySpawnRandomModule(ModuleConnector _connector)
    {
        m_roomTry = 0;
        List<string> excludedCodes = new List<string>();
        foreach(RoomModule mod in loadedModules)
        {
            bool excluded = true;
            foreach(string code in _connector.allowedCodesArray)
            {
                if (mod.moduleCode == code) excluded = false;
            }
            if (excluded) excludedCodes.Add(mod.moduleCode);
        }
        List<string> remainingCodes = new List<string>(_connector.allowedCodesArray);

        RoomModule newModule = GameObject.Instantiate(GetRandomModuleExcluding(_connector, excludedCodes), transform).GetComponent<RoomModule>();

        bool ready = false;

        while (!ready && remainingCodes.Count >= 1 && m_roomTry <= loadedModules.Length)
        {
            if(newModule.gameObject != null) Destroy(newModule.gameObject);
            newModule = GameObject.Instantiate(GetRandomModuleExcluding(_connector, excludedCodes), transform).GetComponent<RoomModule>();
            AlignConnectors(_connector, newModule.GetEntrance());
            if (TestSafeBox(newModule))
            {
                //  MODULE IS GOOD TO GO, SET IT UP
                m_roomTry = 0;                
                remainingCodes = new List<string>();
                ready = true;
            }
            else
            {
                //  MODULE DOESNT FIT, DESTROY IT
                remainingCodes.Remove(newModule.moduleCode);
                excludedCodes.Add(newModule.moduleCode);
                Destroy(newModule.gameObject);
                m_roomTry++;
            }
        }

        if(!ready)
        {
            newModule = GameObject.Instantiate(nullModule, transform).GetComponent<RoomModule>();
            AlignConnectors(_connector, newModule.GetEntrance());
        }
        LinkModules(_connector, newModule.GetEntrance());
        newModule.SetId(m_roomCount);
        newModule.gameObject.name = ("Room " + m_roomCount + " : " + newModule.moduleCode);

        newModule.gameObject.SetActive(true);
        m_spawnedModules.Add(newModule);
        m_roomCount++;

        m_pendingConnections.RemoveAt(0);

        foreach (ModuleConnector con in newModule.GetExits())
        {
            m_pendingConnections.Add(con);
        }
        m_pendingConnections.Remove(newModule.GetEntrance());

        return newModule;
    }

    private bool TestSafeBox(RoomModule _module)
    {
        RaycastHit hit;
        Ray ray;
        LayerMask mask = LayerMask.GetMask("World");

        for (int i = 0; i < _module.raycastCheckers.Count; i++)
        {
            ray = new Ray(_module.raycastCheckers[i].position, Vector3.down);
            Physics.Raycast(ray, out hit, 50f, mask, QueryTriggerInteraction.UseGlobal);
            if (hit.collider)
            {
                if (hit.collider.gameObject != _module.gameObject) return false;
            }
        }

        return true;
    }

    private bool TestModuleRule(ModuleRule _rule)
    {
        int count = 0;
        foreach(RoomModule mod in m_spawnedModules)
        {
            if(mod.moduleCode == _rule.moduleCode)
            {
                count++;
            }
        }

        if (count < _rule.minimum) return false;
        if (count > _rule.maximum) return false;

        return false;
    }

    private void LinkModules(ModuleConnector _a, ModuleConnector _b)
    {
        if(_a != null && _b != null)
        {
            _a.UniqueId = StaticMethods.GetUniqueInt();
            _b.UniqueId = StaticMethods.GetUniqueInt();
            _a.linkedModule = _b.parentModule;
            _b.linkedModule = _a.parentModule;
            _a.LinkedUniqueId = _b.UniqueId;
            _b.LinkedUniqueId = _a.UniqueId;
            Debug.LogWarning("LINKED MODULES : " + _a.UniqueId + ", " + _b.UniqueId);
        }
        else
        {
            Debug.LogWarning("COULD NOT LINK MODULES : " + _a.UniqueId + ", " + _b.UniqueId);
        }
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

    private GameObject GetRandomModule(ModuleConnector _connector)
    {
        return GetRandomModuleExcluding(_connector, new List<string>());
    }
	private GameObject GetRandomModuleExcluding(ModuleConnector _connector, List<string> _excludedCodes)
	{
        bool foundModule = false;
        if (_connector == null)
        {}
        int useModId = 0;
        bool excluded = false;
        int tries = 1;
        for(int i = 0; i < generationRules.moduleRules.Count; i++)
        {
            if(_connector.parentModule.moduleCode == generationRules.moduleRules[i].moduleCode)
            {
                for(int c = 0; c < generationRules.moduleRules[i].bannedCodesArray.Length; c++)
                {
                    _excludedCodes.Add(generationRules.moduleRules[i].bannedCodesArray[c]);
                }
            }
        }
        while (!foundModule && tries <= _excludedCodes.Count)
        {
            int random = Random.Range(0, m_totalRarity);
            for (int i = 0; i < loadedModules.Length; i++)
            {
                excluded = false;
                if (!foundModule)
                {
                    foreach (string code in _excludedCodes)
                    {
                        if (loadedModules[i].moduleCode == code) excluded = true;
                    }
                    if (excluded) {}
                    else if (loadedModules[i].unique) {}
                    else if (random >= loadedModules[i].GetMinRarity() && random <= loadedModules[i].GetMaxRarity())
                    {
                        useModId = i;
                        foundModule = true;
                    }
                }
            }
            tries++;
        }
        if(!foundModule)
        {
            return GetSpecificModule("straight");
        }

        return loadedModules[useModId].gameObject;
    }
    private GameObject GetSpecificModule(string _code)
    {
        if(_code == "null")
        {
            return nullModule.gameObject;
        }
        for (int i = 0; i < loadedModules.Length; i++)
        {
            if (loadedModules[i].moduleCode == _code)
            {
                return loadedModules[i].gameObject;
            }
        }
        return null;
    }

	private static float Azimuth(Vector3 vector)
	{
		return Vector3.Angle(Vector3.forward, vector) * Mathf.Sign(vector.x);
	}


    public void CreateWorldSeed()
    {
        if(m_spawnedModules.Count >= 1)
        {
            SerializedWorld sWorld = new SerializedWorld();
            sWorld.modules = new SerializedModule[m_spawnedModules.Count];
            for (int m = 0; m < m_spawnedModules.Count; m++)
            {
                SerializedModule sMod = new SerializedModule();
                RoomModule mod = m_spawnedModules[m];

                sMod.id = mod.GetId();
                sMod.code = mod.moduleCode;
                sMod.position = mod.transform.position;
                sMod.rotation = Mathf.RoundToInt(mod.transform.eulerAngles.y);
                sMod.connectedModuleId = new int[mod.connectors.Count];
                sMod.connectedModuleCodes = new string[mod.connectors.Count];
                sMod.connections = new SerializedConnector[mod.connectors.Count];

                for (int c = 0; c < mod.connectors.Count; c++)
                {
                    sMod.connectedModuleId[c] = mod.connectors[c].linkedModule.GetId();
                    sMod.connectedModuleCodes[c] = mod.connectors[c].linkedModule.moduleCode;
                    sMod.connections[c] = new SerializedConnector(mod.connectors[c].UniqueId, mod.connectors[c].LinkedUniqueId);
                }

                sWorld.modules[m] = sMod;
            }

            string jsonWorld = JsonUtility.ToJson(sWorld);
            worldSeed = jsonWorld;

            Debug.Log("CREATED WORLD SEED : " + jsonWorld);
        }
    }

    public void GenerateFromSeed(string _jsonSeed)
    {
        ResetWorld();
        SerializedWorld sWorld = JsonUtility.FromJson<SerializedWorld>(_jsonSeed);
        foreach(SerializedModule sMod in sWorld.modules)
        {
            RoomModule newModule = Instantiate(GetSpecificModule(sMod.code), sMod.position, Quaternion.Euler(0, sMod.rotation, 0), transform).GetComponent<RoomModule>();
            newModule.gameObject.SetActive(true);

            m_spawnedModules.Add(newModule);
            m_roomCount++;
            newModule.SetId(m_roomCount);
            newModule.gameObject.name = ("Room " + m_roomCount + " : " + newModule.moduleCode);
        }

        for(int m = 0; m < m_spawnedModules.Count; m++)
        {
            for(int c = 0; c < m_spawnedModules[m].connectors.Count; c++)
            {
                if(m_spawnedModules[m].connectors[c].linkedModule == null)
                {
                    m_spawnedModules[m].connectors[c].parentModule = m_spawnedModules[m];
                    m_spawnedModules[m].connectors[c].UniqueId = sWorld.modules[m].connections[c].uniqueId;
                    m_spawnedModules[m].connectors[c].LinkedUniqueId = sWorld.modules[m].connections[c].linkedUniqueId;
                    LinkModules(GetConnectorFromId(m_spawnedModules[m].connectors[c].UniqueId), GetConnectorFromId(m_spawnedModules[m].connectors[c].LinkedUniqueId));
                }
            }
        }

        timeUi.text = "Time : 0";
        genUi.text = "Generation : " + m_currentAttempts;
        modulesUi.text = "Modules : " + m_spawnedModules.Count;
        Debug.Log("SPAWNED WORLD FROM SEED");
    }

    public void SaveSeed()
    {
        string path = Application.dataPath + "/Seed.txt";
        StreamWriter stream = new StreamWriter(path);
        stream.WriteLine(worldSeed);
        stream.Close();
    }

    public ModuleConnector GetConnectorFromId(int _id)
    {
        foreach(RoomModule module in m_spawnedModules)
        {
            foreach(ModuleConnector connector in module.connectors)
            {
                if(connector.UniqueId == _id)
                {
                    return connector;
                }
            }
        }

        return null;
    }
}

[System.Serializable]
public class SerializedWorld
{
    public SerializedModule[] modules;
}
[System.Serializable]
public class SerializedModule
{
    public int id;
    public string code;
    public Vector3 position;
    public int rotation;
    public int[] connectedModuleId;
    public string[] connectedModuleCodes;
    public SerializedConnector[] connections;
}
[System.Serializable]
public class SerializedConnector
{
    public SerializedConnector(int _uniqueId, int _linkedUniqueId)
    {
        uniqueId = _uniqueId;
        linkedUniqueId = _linkedUniqueId;
    }
    public int uniqueId;
    public int linkedUniqueId;
}