using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    List<Enemy> prototypes;
    List<Enemy> currentEnemies;

    void Start()
    {
        prototypes = new List<Enemy>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var obj in objs)
        {
            prototypes.Add(obj.GetComponent<Enemy>());
        }

        currentEnemies = new List<Enemy>();
        foreach (Enemy p in prototypes)
        {
            Enemy e = Instantiate(p);
            currentEnemies.Add(e);

            p.gameObject.SetActive(false);
        }

        StartCoroutine(Refresh());
    }

    IEnumerator Refresh()
    {
        while (true)
        {
            for (int i = 0; i < currentEnemies.Count; i++)
            {
                if (!currentEnemies[i] || currentEnemies[i].state == AIState.Dead)
                {
                    if (currentEnemies[i])
                    {
                        Destroy(currentEnemies[i].gameObject);
                    }

                    Enemy e = Instantiate(prototypes[i]);
                    currentEnemies[i] = e;
                    e.gameObject.SetActive(true);
                }
            }
            yield return new WaitForSeconds(5);
        }
    }
}
