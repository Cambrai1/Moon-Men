using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ModularWorldGenerator : MonoBehaviour
{
    public GenerationRules generationRules;
    public bool neverStop = false;
    public bool autoModules = true;
	public RoomModule[] modules;
	public RoomModule startModule;
    public RoomModule nullModule;
    public bool visibleIterations = true;
    [Range(1,1000)]
    public int maximumAttempts = 100;

    private int m_totalRarity;
    private List<RoomModule> m_spawnedModules;

	void Start()
	{
        if (autoModules) GetAutoModules();
        foreach(RoomModule mod in modules)
        {
            mod.SetRarityMinMax(m_totalRarity);
            m_totalRarity += mod.abundance;
            mod.gameObject.SetActive(false);
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
        StartCoroutine(RegenerateCoroutine());
    }
    private IEnumerator RegenerateCoroutine()
    {
        GenerationMethod method = generationRules.generationMethod;
        if (neverStop) visibleIterations = true;
        bool success = false;
        int roomCount = 0;
        int currentAttempts = 0;
        string failReason = "";
        int failReasons = 0;
        m_spawnedModules = new List<RoomModule>();
        float startTime = Time.time;

        while(!success && (currentAttempts <= maximumAttempts || neverStop))
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
            roomCount = 0;
            int roomTry = 0;

            RoomModule startingModule = Instantiate(startModule, transform.position, transform.rotation).GetComponent<RoomModule>();
            startingModule.gameObject.SetActive(true);
            m_spawnedModules.Add(startingModule);

            List<ModuleConnector> pendingConnections = new List<ModuleConnector>(startingModule.GetExits());
            switch (method)
            {
                case GenerationMethod.widthFirst:
                    {
                        while (pendingConnections.Count >= 1 && roomCount <= generationRules.maximumRooms)
                        {
                            if (pendingConnections[0] != null && roomTry <= 10)
                            {
                                RoomModule newModule = GameObject.Instantiate(GetRandomModule(pendingConnections[0]), transform).GetComponent<RoomModule>();
                                AlignConnectors(pendingConnections[0], newModule.GetEntrance());
                                newModule.gameObject.SetActive(true);
                                if (TestSafeBox(newModule))
                                {
                                    roomTry = 0;

                                    LinkModules(pendingConnections[0], newModule.GetEntrance());
                                    newModule.SetId(roomCount);
                                    //Debug.Log("Spawned Room " + roomCount + ": " + newModule.moduleCode);
                                    newModule.gameObject.name = ("Room " + roomCount + " : " + newModule.moduleCode);
                                    m_spawnedModules.Add(newModule);
                                    roomCount++;
                                    pendingConnections.RemoveAt(0);

                                    foreach (ModuleConnector con in newModule.GetExits())
                                    {
                                        pendingConnections.Add(con);
                                    }
                                    pendingConnections.Remove(newModule.GetEntrance());

                                    if (visibleIterations) yield return null;
                                }
                                else
                                {
                                    Destroy(newModule.gameObject);
                                    roomTry++;
                                }
                            }
                            else
                            {
                                Debug.LogWarning("PENDING CONNECTIONS [0] SOMEHOW IS NULL");
                                pendingConnections.RemoveAt(0);
                            }
                        }
                    }
                    break;

                case GenerationMethod.depthFirst:
                    {
                        while (pendingConnections.Count >= 1 && roomCount <= generationRules.maximumRooms)
                        {
                            int c = pendingConnections.Count - 1;
                            if (pendingConnections[c] != null && roomTry <= 10)
                            {
                                RoomModule newModule = Instantiate(GetRandomModule(pendingConnections[c]), transform).GetComponent<RoomModule>();
                                AlignConnectors(pendingConnections[c], newModule.GetEntrance());
                                newModule.gameObject.SetActive(true);
                                
                                if(TestSafeBox(newModule))
                                {
                                    newModule.safetyBox.enabled = true;
                                    LinkModules(pendingConnections[c], newModule.GetEntrance());
                                    newModule.SetId(roomCount);
                                    //Debug.Log("Spawned Room " + roomCount + ": " + newModule.moduleCode);
                                    newModule.gameObject.name = ("Room " + roomCount + " : " + newModule.moduleCode);
                                    m_spawnedModules.Add(newModule);
                                    roomCount++;
                                    pendingConnections.RemoveAt(c);

                                    foreach (ModuleConnector con in newModule.GetExits())
                                    {
                                        pendingConnections.Add(con);
                                    }
                                    pendingConnections.Remove(newModule.GetEntrance());

                                    if (visibleIterations) yield return null;
                                }
                                else
                                {
                                    GameObject.Destroy(newModule.gameObject);
                                    roomTry++;
                                }
                            }
                            else
                            {
                                Debug.LogWarning("PENDING CONNECTIONS [" + c + "] SOMEHOW IS NULL");
                                pendingConnections.RemoveAt(c);
                            }
                        }
                    }
                    break;
            }

            if (roomCount <= generationRules.minimumRooms)
            {
                failReason += ("\nRoom count " + roomCount + " below minimum of " + generationRules.minimumRooms + ".");
                failReasons++;
                success = false;
            }
            if (roomCount >= generationRules.maximumRooms)
            {
                failReason += ("\nRoom count " + roomCount + " above maximum of " + generationRules.maximumRooms + ".");
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

            currentAttempts++;
        }

        Debug.Log("GENERATED WORLD IN " + (Time.time - startTime) + " SECONDS AFTER " + currentAttempts + " ATTEMPTS!");

        yield return null;
    }

    private RoomModule TrySpawnModule(ModuleConnector _connector)
    {

        return new RoomModule();
    }

    private bool TestSafeBox(RoomModule _module)
    {
        Debug.Log("STARTED RAYCAST TEST");

        RaycastHit hit;
        Ray ray;
        LayerMask mask = LayerMask.GetMask("World");

        //foreach(Transform t in _module.raycastCheckers)
        //{
        //    ray = new Ray(t.position, Vector3.down);
        //    Physics.Raycast(ray, out hit, 50f, mask, QueryTriggerInteraction.UseGlobal);
        //    if(hit.collider)
        //    {
        //        if(hit.collider.gameObject != _module.gameObject) return false;
        //    }
        //}

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
        bool foundModule = false;
        if (_connector == null) return GetSpecificModule("straight");
        int useModId = 0;
        bool excluded = false;
        while (!foundModule)
        {
            int random = Random.Range(0, m_totalRarity);
            for (int i = 0; i < modules.Length; i++)
            {
                if (!foundModule)
                {
                    foreach (string code in _excludedCodes)
                    {
                        if (modules[i].moduleCode == code) excluded = true;
                    }
                    if (excluded) { }
                    else if (modules[i].unique) { }
                    else if (random >= modules[i].GetMinRarity() && random <= modules[i].GetMaxRarity())
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
