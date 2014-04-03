﻿using UnityEngine;
using System.Collections;

public class CatchingMiceGameManager : LugusSingletonExisting<CatchingMiceGameManagerDefault>
{
}
public class CatchingMiceGameManagerDefault : MonoBehaviour 
{
    public bool gameRunning = false;
    private bool firstFrame = true;
    private int currentIndex = 0;
    protected float timer = 0;
    protected int pickupCount = 0;
    public void StartGame()
    {

    }
    public void StopGame()
    {

    }
    public virtual void ReloadLevel()
    {
        Application.LoadLevel(Application.loadedLevelName);
    }

    public bool GameRunning
    {
        get { return gameRunning; }
    }


    protected bool _paused = false;
    public bool Paused
    {
        get { return _paused; }
        set
        {
            SetPaused(value);
        }
    }

    public void SetPaused(bool pause)
    {
        Debug.Log("GameManager : setPaused : " + pause);
        if (pause)
        {
            // Try pause
            if (Paused)
            {
                Debug.LogError(transform.Path() + " : IGameManager:SetPaused : game was already paused. Doing nothing");
                return;
            }

            // pause
            Time.timeScale = 0.0001f;
            // update the physics timestep as well
            // otherwhise, moving objects with colliders (all our Buttons) wouldn't update collision correctly!
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            _paused = true;

        }
        else
        {
            // Try unpause
            if (!Paused)
            {
                Debug.LogWarning("GameManager:SetPaused : game was already UNpaused. Doing nothing");
                return;
            }

            // unpause
            Time.timeScale = 1.0f;
            // update the physics timestep as well
            // otherwhise, moving objects with colliders (all our Buttons) wouldn't update collision correctly!
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            _paused = false;
        }
    }
	// Use this for initialization
	void Start () 
    {
        CatchingMiceLevelManager.use.ClearLevel();
        CatchingMiceLevelManager.use.BuildLevel(0);
	}
    public void ModifyPickUpCount(int modifyValue)
    {
        pickupCount += modifyValue;
    }
	// Update is called once per frame
    protected void Update()
    {
        if (gameRunning)
            timer += Time.deltaTime;
    }
}
