﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour {

    public float returnSpeed = 5f;
    public GameObject fish;

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
            Cannon cannon = linker.firstObject.GetComponent<Cannon>();
            if(cannon != null)
            {
                rb.MovePosition(Vector3.MoveTowards(transform.position, linker.firstObject.transform.parent.position, returnSpeed * Time.deltaTime));
                if(Physics2D.OverlapPoint(transform.position) == linker.firstObject.transform.parent.GetComponent<Collider2D>())
                {
                    Invoke("SelfDestruct", 0.1f);
                }
            }
            else
            {
                rb.MovePosition(Vector3.MoveTowards(transform.position, linker.firstObject.transform.position, returnSpeed * Time.deltaTime));
                if(Physics2D.OverlapPoint(transform.position) == linker.firstObject.transform.GetComponent<Collider2D>())
                {
                    Invoke("SelfDestruct", 0.1f);
                }
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
            FishAi fishAi = collision.gameObject.GetComponent<FishAi>();
            fishAi.hook = gameObject;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        FishCatcher fc = other.gameObject.GetComponentInParent<FishCatcher>();
        if (fc != null && fc.fishCaught > 0 && other.gameObject.layer != gameObject.layer)
        {
            fc.LoseFish();
            Invoke("SpawnCaughtFish", 0.1f);
        }
    }

    void SelfDestruct()
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

    void SpawnCaughtFish()
    {
        GameObject newFish = Instantiate(fish);
        newFish.transform.position = transform.position;
        newFish.GetComponent<FishAi>().sea = GameObject.FindGameObjectWithTag("SeaTextures");
        newFish.GetComponent<FishAi>().hook = gameObject;
    }
}
