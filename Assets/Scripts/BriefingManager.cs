using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefingManager : MonoBehaviour
{
    [SerializeField] public TheatersportBriefing testScriptableObject;

    
    void Start()
    {
        Debug.Log(testScriptableObject.spelerRol1.rol);
        Debug.Log(testScriptableObject.spelerRol1.scenario);
        Debug.Log(testScriptableObject.spelerRol2.rol);
        Debug.Log(testScriptableObject.spelerRol1.scenario);
    }

    
    void Update()
    {
        
    }
}
