using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ModularWorldGenerator : MonoBehaviour
{
    public GenerationRules generationRules;
    public bool neverStop = false;
    public bool autoModules = true;
	public RoomModule[] loadedModules;
	public RoomModule startModule;
    public RoomModule nullModule;
    public bool visibleIterations = true;
    [Range(1,1000)]
    public int maximumAttempts = 100;

    private int m_totalRarity;
    private List<RoomModule> m_spawnedModules;
    private int m_roomCount = 0;
    private int m_currentAttempts = 0;
    private int m_roomTry = 0;
    private List<ModuleConnector> m_pendingConnections;

    void Start()
	{
        if (autoModules) GetAutoModules();
        foreach(RoomModule mod in loadedModules)
        {
            mod.SetRarityMinMax(m_totalRarity);
            m_totalRarity += mod.abundance;
            mod.gameObject.SetActive(false);
        }
        Debug.Log("Loaded " + loadedModules.Length + " modules, total rarity " + m_totalRarity + "!");

        Regenerate();
	}

    private void GetAutoModules()
    {
        int childCount = transform.childCount;
        loadedModules = new RoomModule[childCount];
        for (int i = 0; i < childCount; i++)
        {
            loadedModules[i] = transform.GetChild(i).GetComponent<RoomModule>();
        }
        startModule = loadedModules[0];
    }
    
    public void Regenerate()
    {
        StartCoroutine(RegenerateCoroutine());
    }
    private IEnumerator RegenerateCoroutine()
    {
        GenerationMethod method = generationRules.generationMethod;
        if (neverStop) visibleIterations = true;
        bool success = false;
        string failReason = "";
        int failReasons = 0;
        m_roomCount = 0;
        m_currentAttempts = 0;
        m_spawnedModules = new List<RoomModule>();
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
            switch (method)
            {
                case GenerationMethod.widthFirst:
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

                            if (visibleIterations) yield return null;
                        }
                    }
                    break;

                case GenerationMethod.depthFirst:
                    {
                        while (m_pendingConnections.Count >= 1 && m_roomCount <= generationRules.maximumRooms)
                        {
                            int c = m_pendingConnections.Count - 1;
                            if (m_pendingConnections[c] != null && m_roomTry <= 10)
                            {
                                RoomModule newModule = Instantiate(GetRandomModule(m_pendingConnections[c]), transform).GetComponent<RoomModule>();
                                AlignConnectors(m_pendingConnections[c], newModule.GetEntrance());
                                newModule.gameObject.SetActive(true);
                                
                                if(TestSafeBox(newModule))
                                {
                                    newModule.safetyBox.enabled = true;
                                    LinkModules(m_pendingConnections[c], newModule.GetEntrance());
                                    newModule.SetId(m_roomCount);
                                    //Debug.Log("Spawned Room " + roomCount + ": " + newModule.moduleCode);
                                    newModule.gameObject.name = ("Room " + m_roomCount + " : " + newModule.moduleCode);
                                    m_spawnedModules.Add(newModule);
                                    m_roomCount++;
                                    m_pendingConnections.RemoveAt(c);

                                    foreach (ModuleConnector con in newModule.GetExits())
                                    {
                                        m_pendingConnections.Add(con);
                                    }
                                    m_pendingConnections.Remove(newModule.GetEntrance());

                                    if (visibleIterations) yield return null;
                                }
                                else
                                {
                                    GameObject.Destroy(newModule.gameObject);
                                    m_roomTry++;
                                }
                            }
                            else
                            {
                                Debug.LogWarning("PENDING CONNECTIONS [" + c + "] SOMEHOW IS NULL");
                                m_pendingConnections.RemoveAt(c);
                            }
                        }
                    }
                    break;
            }

            if (m_roomCount <= generationRules.minimumRooms)
            {
                failReason += ("\nRoom count " + m_roomCount + " below minimum of " + generationRules.minimumRooms + ".");
                failReasons++;
                success = false;
            }
            if (m_roomCount >= generationRules.maximumRooms)
            {
                failReason += ("\nRoom count " + m_roomCount + " above maximum of " + generationRules.maximumRooms + ".");
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
            if(!success && !neverStop)
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
        }

        Debug.Log("GENERATED WORLD IN " + (Time.time - startTime) + " SECONDS AFTER " + m_currentAttempts + " ATTEMPTS!");

        yield return null;
    }

    private RoomModule TrySpawnRandomModule(ModuleConnector _connector)
    {
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

        string r = "";
        foreach (string m in remainingCodes) { r += m + ", "; }
        Debug.Log("REMAINING MODULES : " + r);
        r = "";
        foreach (string m in excludedCodes) { r += m + ", "; }
        Debug.Log("EXCLUDED MODULES : " + r);

        List<string> testList = new List<string>();
        testList.Add("cross");

        RoomModule newModule = GameObject.Instantiate(GetRandomModuleExcluding(_connector, excludedCodes), transform).GetComponent<RoomModule>();
        AlignConnectors(_connector, newModule.GetEntrance());

        bool ready = false;

        //while (!ready)
        //{
        //    string r = "";
        //    foreach (string m in remainingCodes) { r += m + ", "; }
        //    Debug.Log("REMAINING MODULES : " + r);
        //    r = "";
        //    foreach (string m in excludedCodes) { r += m + ", "; }
        //    Debug.Log("EXCLUDED MODULES : " + r);

        //    newModule = GameObject.Instantiate(GetRandomModuleExcluding(_connector, excludedCodes), transform).GetComponent<RoomModule>();
        //    AlignConnectors(_connector, newModule.GetEntrance());
        //    if (TestSafeBox(newModule))
        //    {
        //        Debug.Log("1111111111111111111111111");
        //        //  MODULE IS GOOD TO GO, SET IT UP
        //        m_roomTry = 0;
        //        LinkModules(_connector, newModule.GetEntrance());
        //        newModule.SetId(m_roomCount);
        //        newModule.gameObject.name = ("Room " + m_roomCount + " : " + newModule.moduleCode);
        //        remainingCodes = new List<string>();
        //        ready = true;
        //    }
        //    else
        //    {
        //        Debug.Log("2222222222222222222222222");
        //        //  MODULE DOESNT FIT, DESTROY IT
        //        remainingCodes.Remove(newModule.moduleCode);
        //        excludedCodes.Add(newModule.moduleCode);
        //        Destroy(newModule.gameObject);
        //    }
        //}

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
        Debug.Log("STARTED RAYCAST TEST");

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

    private GameObject GetRandomModule(ModuleConnector _connector)
    {
        return GetRandomModuleExcluding(_connector, new List<string>());
    }
	private GameObject GetRandomModuleExcluding(ModuleConnector _connector, List<string> _excludedCodes)
	{
        Debug.Log(_excludedCodes.Count + " codes");
        bool foundModule = false;
        if (_connector == null)
        {}
        int useModId = 0;
        bool excluded = false;
        int tries = 1;
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
            Debug.Log("STRAIGHT AS AN ARROW");
            return GetSpecificModule("straight");
        }

        return loadedModules[useModId].gameObject;
    }
    private GameObject GetSpecificModule(string _code)
    {
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
}
