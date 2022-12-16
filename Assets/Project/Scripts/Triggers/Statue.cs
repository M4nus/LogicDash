using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour
{
    public GameObject obstacle;
    public GameObject explosionStatue;
    public GameObject dissolveObstacle;

    private void OnDisable()
    {
        explosionStatue.SetActive(true);
        dissolveObstacle.SetActive(true);
        obstacle.SetActive(false);
    }
}
