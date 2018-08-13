﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour {

    public float returnSpeed = 5f;

    private Rigidbody2D rb;
    private bool going = true;
    private Linker linker;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        linker = GetComponentInChildren<Linker>();
        rb.useFullKinematicContacts = true;
    }

    void Update ()
    {
        if(going)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, rb.velocity.normalized);
        }
        else
        {
            if (transform.position.y > linker.firstObject.transform.position.y)
            {
                rb.MoveRotation(Vector2.Angle(transform.position - linker.firstObject.transform.position, Vector2.right) - 90f);
            }
            else
            {
                rb.MoveRotation(-Vector2.Angle(transform.position - linker.firstObject.transform.position, Vector2.right) - 90f);
            }
            rb.MovePosition(Vector3.MoveTowards(transform.position, linker.firstObject.transform.position, returnSpeed * Time.deltaTime));
            if(Vector3.Distance(transform.position, linker.firstObject.transform.position) < 0.2f)
            {
                Cannon cannon = linker.firstObject.GetComponent<Cannon>();
                if(cannon != null)
                {
                    cannon.harpoonAvailable = true;
                }
                else
                {
                    EnemyShipAi enemyShipAi = linker.firstObject.GetComponent<EnemyShipAi>();
                    enemyShipAi.harpoonAvailable = true;
                }
                Destroy(gameObject);
            }
        }
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (going)
        {
            going = false;
            if(rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        if (collision.gameObject.tag == "Fish")
        {
            collision.gameObject.GetComponent<FishAi>().hook = gameObject;
        }

    }
}
