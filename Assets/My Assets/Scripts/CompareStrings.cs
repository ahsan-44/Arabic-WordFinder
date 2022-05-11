using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTLTMPro;

public class CompareStrings : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_InputField inputText1, inputText2;
    [SerializeField]
    private RTLTextMeshPro textView;
    [SerializeField]
    private string title;

    void Start()
    {
        textView.text = title;
    }

    public void Compare()
    {
        if (string.Equals(inputText1.text, inputText2.text))
        {
            Debug.Log("Strings are equal");
            textView.text = "يوجد تطابق";
        }
        else
        {
            Debug.Log("Strings are not equal");
            textView.text = "لا يوجد تطابق";
        }
    }
}
