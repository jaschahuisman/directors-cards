using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public List<BriefingScriptable> briefings = new List<BriefingScriptable>();
    public List<ImprovCardScriptable> improvCards = new List<ImprovCardScriptable>();

    public static Database Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}