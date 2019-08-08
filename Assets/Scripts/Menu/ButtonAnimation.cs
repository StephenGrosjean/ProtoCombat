using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup layoutGroup;
    [SerializeField] private float maxSpacing;
    [SerializeField] private int speed;

    [SerializeField] private int startValue;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("AnimateButtons");   
    }


    IEnumerator AnimateButtons() {
        for(int i = startValue; i < maxSpacing; i+=speed) {
            layoutGroup.spacing = i;
            yield return null;
        }
    }
}
