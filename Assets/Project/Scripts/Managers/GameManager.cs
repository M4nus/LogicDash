using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject portal;

    public float spawnDelay = 0.2f;

    public bool canMove = true;
    public bool isDead = false;


    public void Spawn(Vector3 spawnPoint)
    {
        PlayerMovement player = PlayerMovement.Instance;

        Instantiate(portal, new Vector2(spawnPoint.x, spawnPoint.y - 0.3f), Quaternion.Euler(-90f, 0f, 0f));
        canMove = false;
        player.rb.isKinematic = true;
        player.rb.velocity = Vector2.zero;
        player.transform.localPosition = spawnPoint;
        player.transform.localRotation = Quaternion.identity;
        player.anim.SetBool("isDead", false);

        if(!player.hasNut)
        {
            player.nut.transform.localPosition = player.nutSpawnPoint.localPosition;
        }
        CameraShake.Instance.Shake(0.1f, spawnDelay);
        StartCoroutine(WaitForSpawn(spawnDelay));
    }

    public void Death()
    {
        PlayerMovement player = PlayerMovement.Instance;

        canMove = false;
        isDead = true;
        player.anim.SetBool("isDead", true);
    }

    public IEnumerator WaitForSpawn(float delay)
    {
        PlayerMovement player = PlayerMovement.Instance;
        yield return new WaitForSeconds(delay);
        player.rb.isKinematic = false;
        canMove = true;
    }
}
