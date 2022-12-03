using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    [SerializeField] GameObject dotsParent;
    [SerializeField] GameObject dotPrefab;
    [SerializeField] int dotsNumber;
    [SerializeField] float dotSpacing;

    Transform[] dotsList;
    Vector2 pos;
    float timeStamp;

    void Start()
    {
        Hide();
        PrepareDots();
    }

    void PrepareDots()
    {
        dotsList = new Transform[dotsNumber];

        for(int i = 0; i < dotsNumber; i++)
        {
            dotsList[i] = Instantiate(dotPrefab, dotsParent.transform).transform;

        }
    }

    public void UpdateDots(Vector2 nutPos, Vector2 forceApplied)
    {
        timeStamp = dotSpacing;

        for(int i = 0; i < dotsNumber; i++)
        {
            pos.x = (nutPos.x + forceApplied.x * timeStamp);
            pos.y = (nutPos.y + forceApplied.y * timeStamp) - (Physics2D.gravity.magnitude * timeStamp * timeStamp) / 2f;

            dotsList[i].position = pos;
            timeStamp += dotSpacing;
        }
    }

    public void Show()
    {
        dotsParent.SetActive(true);
    }

    public void Hide()
    {
        dotsParent.SetActive(false);
    }
}
