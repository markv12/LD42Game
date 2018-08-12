﻿using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class PieceMouseManager : MonoBehaviour {

    private const float START_SLEEP_TIME = 25;
    [NonSerialized]
    public float currentSleepTime = START_SLEEP_TIME;

    public EndScreenManager endScreen;
    public PieceSpawner spawner;

    private List<GamePiece> coldPeople = new List<GamePiece>();

    private GamePiece currentPiece;
    private bool piecePickedUp = false;
    private Vector3 pickUpOffset;

    public static PieceMouseManager instance;

    void Awake()
    {
        instance = this;
    }

	void LateUpdate () {
        if (Input.GetMouseButtonDown(0) && currentPiece != null)
        {
            piecePickedUp = true;
            pickUpOffset = currentPiece.transform.position - GetWorldMousePosition();
        }
        if (Input.GetMouseButtonUp(0))
        {
            piecePickedUp = false;
        }
        if(piecePickedUp && currentPiece != null)
        {
            Vector3 v3 = GetWorldMousePosition();
            Vector3 diff = (v3 + pickUpOffset) - currentPiece.transform.position;
            currentPiece.rigid.velocity = diff*15;

            var d = Input.GetAxis("Mouse ScrollWheel");
            currentPiece.transform.Rotate(0, 0, d * 50);
        }
    }

    private static Vector3 GetWorldMousePosition()
    {
        Vector3 v3 = Input.mousePosition;
        v3.z = 10.0f;
        v3 = Camera.main.ScreenToWorldPoint(v3);
        return v3;
    }

    private const int COLD_PERSON_LIMIT = 10;
    public void RegisterColdPerson(GamePiece piece)
    {
        if (!coldPeople.Contains(piece))
        {
            coldPeople.Add(piece);
        }
        if(coldPeople.Count >= COLD_PERSON_LIMIT)
        {
            EndGame();
            coldPeople.Clear();
        }
    }

    private void EndGame()
    {
        LoadingScreen.instance.Show(_EndGame(), 0.666f);
    }

    private IEnumerator _EndGame()
    {
        spawner.StopSpawning();
        GamePiece[] remainingPeople = FindObjectsOfType(typeof(GamePiece)) as GamePiece[];
        for (int i = 0; i < remainingPeople.Length; i++)
        {
            Destroy(remainingPeople[i].gameObject);
        }
        endScreen.ShowScore(HudManager.instance.Score);
        HudManager.instance.Score = 0;
        currentSleepTime = START_SLEEP_TIME;
        endScreen.container.gameObject.SetActive(true);
        yield return null;
    }

    public void UnregisterColdPerson(GamePiece piece)
    {
        coldPeople.Remove(piece);
    }

    public void SetCurrentPiece(GamePiece piece)
    {
        if (!piecePickedUp)
        {
            currentPiece = piece;
        }
    }
}
