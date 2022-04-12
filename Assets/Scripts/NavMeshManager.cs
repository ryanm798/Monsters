using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NavMeshManager : MonoBehaviour
{
    /***** SINGLETON SETUP *****/
    private static NavMeshManager _instance;
    public static NavMeshManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    private void OnDestroy()
    {
        if (this == _instance)
        {
            _instance = null;
        }
    }
    /******************************/

    int maxAgentIndex;
    public string destructibleLayerName = "Destructible";
    public int destructiveMonsterSize = 10;

    private void Start()
    {
        maxAgentIndex = NavMesh.GetSettingsCount() - 1;
    }

    public int GetAgentId(float monsterScale)
    {
        int size = Mathf.RoundToInt(monsterScale);
        int index = size;
        if (size > maxAgentIndex)
            index = maxAgentIndex;
        return NavMesh.GetSettingsByIndex(index).agentTypeID;
    }
}
