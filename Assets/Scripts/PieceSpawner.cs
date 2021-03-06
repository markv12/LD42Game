﻿using System.Collections;
using UnityEngine;

public class PieceSpawner : MonoBehaviour {
    public PieceSet set;
    public Transform[] spawnLocations;
    public AudioSource mainGameAudio;

    private Ossan[] allOssan;
    private Wood[] theWoods;

    void Awake()
    {
        allOssan = set.ossan;
        theWoods = set.woods;
        SortInPlaceRandom(spawnOrder);
    }

    public void StartSpawning()
    {
        HeatManager.instance.enabled = true;
        spawnRoutine = StartCoroutine(SpawnPieces());
    }

    private int ossanSinceLastWood = 2;
    private Coroutine spawnRoutine;
    private static readonly WaitForSeconds startWait = new WaitForSeconds(1f);
    private IEnumerator SpawnPieces()
    {
        yield return startWait;
        StartCoroutine(TitleScreenManager.FadeAudioSourceVolume(mainGameAudio, 0.8f, 2f));
        while (true)
        {
            Ossan ossan = allOssan[Random.Range(0, allOssan.Length)];
            SpawnPiece(ossan);
            ossanSinceLastWood++;
            if (ossanSinceLastWood >= 4)
            {
                SpawnPiece(theWoods[Random.Range(0, theWoods.Length)]);
                ossanSinceLastWood = 0;
            }
            yield return new WaitForSeconds(Random.Range(3f, 5f));
        }
    }

    private void SpawnPiece(DragablePiece piece)
    {
        DragablePiece newPiece = Instantiate(piece);
        Vector3 spawnPos = RandomSpawnPosition();
        newPiece.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        if (Random.Range(0f, 1f) >= 0.5f)
        {
            Vector3 currentScale = newPiece.transform.localScale;
            newPiece.transform.localScale = new Vector3(-currentScale.x, currentScale.y, currentScale.z);
        }
        newPiece.MovePieceIn(spawnPos * 2, spawnPos);
    }

    public void StopSpawning()
    {
        HeatManager.instance.enabled = false;
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    public bool IsSpawning()
    {
        return spawnRoutine != null;
    }

    private int currentIndex = 0;
    private int[] spawnOrder = new int[] { 0, 1, 2, 3, 4, 5 };
    private Vector3 RandomSpawnPosition()
    {
        int loc = spawnOrder[currentIndex];
        currentIndex++;
        if(currentIndex >= spawnOrder.Length)
        {
            currentIndex = 0;
            SortInPlaceRandom(spawnOrder);
        }

        return spawnLocations[loc].position;
    }

    public static void SortInPlaceRandom(int[] theArray)
    {
        for (int t = 0; t < theArray.Length; t++)
        {
            int tmp = theArray[t];
            int r = Random.Range(t, theArray.Length);
            theArray[t] = theArray[r];
            theArray[r] = tmp;
        }
    }
}
