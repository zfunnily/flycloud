using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget: MonoBehaviour
{
    private Transform player;
    private Vector3 offset;

    private float smoothing = 3;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        offset = player.position - transform.position ;
    }

    void Update() {
        this.transform.position = player.position - offset;
    }
}
