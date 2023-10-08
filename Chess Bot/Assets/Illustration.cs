using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Linq;

public class Illustration : MonoBehaviour
{
    
    private void Start()
    {
        /*Debug.Log("start");
        string input = "-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;21;19;20;22;17;20;19;21;-1;-1;18;18;18;18;18;18;18;18;-1;-1;0;0;0;0;0;0;0;0;-1;-1;0;0;0;0;0;0;0;0;-1;-1;0;0;0;0;0;0;0;0;-1;-1;0;0;0;0;0;0;0;0;-1;-1;10;10;10;10;10;10;10;10;-1;-1;13;11;12;14;9;12;11;13;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;-1;";
        string[] _tokens = input.Split(';');
        string[] tokens = new string[_tokens.Length - 1];
        Array.Copy(_tokens, tokens, tokens.Length);
        if (tokens.Length != 120)
        {
            Debug.LogWarning("Der String enthält nicht die erwartete Anzahl von Elementen.");
            return;
        }

        int[] intArray = new int[120];
        for (int i = 0; i < tokens.Length; i++)
        {
            if (int.TryParse(tokens[i], out int value))
            {
                intArray[i] = value;
            }
            else
            {
                Debug.LogWarning("Fehler bei der Konvertierung des Strings in Ganzzahlen.");
                return;
            }
            intArray[i] = i;
        }

        for (int i = 0; i < intArray.Length; i++)
        {
            Debug.Log(i + " : " + intArray[i]);
            transform.GetChild(i).GetChild(3).GetComponent<Text>().text = intArray[i].ToString();
        }*/



    }
}
