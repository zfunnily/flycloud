using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraNS
{

public class FollowTarget: MonoBehaviour
{
    private Transform player;
    private Vector3 offset;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        offset = player.position - transform.position ;
    }

    void Update() {
        this.transform.position = player.position - offset;
    }
}

}