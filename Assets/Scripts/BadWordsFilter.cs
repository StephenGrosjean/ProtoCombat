using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BadWordsFilter : MonoBehaviour
{
    [SerializeField]  List<string> badWords;
    [SerializeField] TextAsset badWordsFile;

    void Awake()
    {
        badWords = new List<string>();
        string[] lines = badWordsFile.text.Split('\n');
        foreach (string line in lines) {
            badWords.Add(line);
        }
    }

    public bool CheckIfIsBadWord(string s) {
        bool badword = true;
        foreach(string st in badWords) {
            if(st == s) {
                Debug.Log("BAD");
                badword = true;
                break;
            }
            else {
                Debug.Log(st);
                badword = false;
            }
        }

        return badword;
    }
}
