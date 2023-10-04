using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Diagnostics : MonoBehaviour
{
    public static Diagnostics Instance;

    [Header("Transposition Table")]
    public Text usagePercentage;
    public Text entryCount;

    private void Awake()
    {
        Instance = this;
    }

    #region Visuals

    public void UpdateTranspositionTableVisuals(double _usagePercentage, int _entryCount)
    {
        usagePercentage.text = $"{_usagePercentage}% used";
        entryCount.text = $"{_entryCount} entries";
    }

    #endregion
}
