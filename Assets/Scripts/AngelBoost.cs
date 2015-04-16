﻿using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AngelBoost : MonoBehaviour {

    public bool HasAngelBoost;
    public GameObject[] AngelBoostObjects;

    [Range(0, 50)]
    public int AngelsToKillPerBoost = 20;
    [Range(0, 50)]
    public int AngelsUntilNextBoost = 20;

    public float CurrentHeighest;
    [Range(0, 100)]
    public float FallDistanceToSave = 20;
    [Range(0f, 5000f)]
    public float BoostForce = 1000;

    Rigidbody2D targetRigidbody;

    bool playerHasFirstExploded;

    void Start() {
        targetRigidbody = GetComponent<Rigidbody2D>();
        GetComponent<Player>().OnExploded += explosion => {
            if (!playerHasFirstExploded) {
                AngelsUntilNextBoost = AngelsToKillPerBoost;
                playerHasFirstExploded = true;
            }
        };
        FindObjectOfType<GameOver>().OnGameOver += () => {
            CurrentHeighest = 0;
            playerHasFirstExploded = false;
        };
    }

    public void ReportAngelKilled() {
        if (!HasAngelBoost) {
            AngelsUntilNextBoost--;
        }
        if (AngelsUntilNextBoost <= 0) {
            HasAngelBoost = true;
            AngelsUntilNextBoost = AngelsToKillPerBoost;
        }
    }

    void Update() {
        // toggle stuff based on whether Angel Boost is available
        {
            for (int i = 0; i < AngelBoostObjects.Length; i++) {
                AngelBoostObjects[i].SetActive(HasAngelBoost);
            }
        }

        if (!Application.isPlaying) return;

        // keep track of highest position
        {
            CurrentHeighest = Mathf.Max(CurrentHeighest, targetRigidbody.position.y);
        }
        // activate boost if player falls a certain amount below max
        {
            if (HasAngelBoost && (targetRigidbody.position.y + FallDistanceToSave) <= CurrentHeighest) {
                targetRigidbody.velocity = targetRigidbody.velocity.withY(0);
                targetRigidbody.AddForce(new Vector2(0, BoostForce));
                HasAngelBoost = false;
            }
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position.withY(CurrentHeighest - FallDistanceToSave).plusX(-5), transform.position.withY(CurrentHeighest - FallDistanceToSave).plusX(5));
    }
}
