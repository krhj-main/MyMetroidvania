using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casting : MonoBehaviour
{
    [SerializeField] GameObject[] fireball;

    [SerializeField] float appearTime;
    int castingCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TimeToCasting()
    {
        castingCount = 0;
        StartCoroutine(SpellAppearTime());
        if (castingCount > fireball.Length)
        {
            StopCoroutine(SpellAppearTime());
        }
    }
    public void CancelCasting()
    {
        for (int i = 0; i < fireball.Length; i++)
        {
            fireball[i].SetActive(false);
        }
    }
    IEnumerator SpellAppearTime()
    {
        while (castingCount < fireball.Length)
        {
            Debug.Log($"castingCount{castingCount}");
            fireball[castingCount].SetActive(true);
            castingCount++;
            yield return new WaitForSeconds(appearTime);
        }
    }
}
