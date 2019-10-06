using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sapient : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        RandomizeAnimationFrame("LifeFormSapient"); // Set spectators to move in random phase
    }

    public void RandomizeAnimationFrame(string animName)
    {
        int rnd = Random.Range(0, 60);
        anim.Play(animName, 0, (1f / 60) * rnd);
    }
}