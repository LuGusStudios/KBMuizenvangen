using UnityEngine;
using System.Collections;

public class CatchingMiceGameManager : LugusSingletonExisting<CatchingMiceGameManagerDefault>
{
}
public class CatchingMiceGameManagerDefault : MonoBehaviour 
{
    public bool gameRunning = false;
    protected float timer = 0;
    protected int pickupCount = 0;
    protected int currentWave = 0;
    protected int amountToKill = 0;
     
    protected float preWaveTime = 2.0f;
    protected ILugusCoroutineHandle _gameRoutineHandle = null;

    public enum State
    {
        PreWave = 1, //Let the player place their traps
        Wave = 2,    //Object spawning and game playing
        PostWave = 3,//After wave has been killed or player lost
        Won = 4,     //all waves has been iterated and it's not an infinite level
        Lost = 5,    //cheese has been eaten

        NONE = -1
    }

    public delegate void OnGameStateChange();
    public OnGameStateChange onGameStateChange;

    protected CatchingMiceGameManagerDefault.State _state = State.NONE;

    public CatchingMiceGameManagerDefault.State state
    {
        get 
        {
            return _state;
        }
        set
        {
            State oldState = _state;
            _state = value;
            if (_state != oldState)
            {
                DoNewStateBehaviour(_state);
                if(onGameStateChange!=null)
                    onGameStateChange();
            }
        }
    }
    public void DoNewStateBehaviour(State newState)
    {
        if (_gameRoutineHandle != null)
        {
            _gameRoutineHandle.StopRoutine();
        }
        
        switch (newState)
        {
            case State.PreWave:
                _gameRoutineHandle = LugusCoroutines.use.StartRoutine(PreWavePhase());
                break;
            case State.Wave:
                _gameRoutineHandle = LugusCoroutines.use.StartRoutine(WavePhase());
                break;
            case State.PostWave:
                PostWavePhase();
                break;
            case State.Won:
                Debug.Log("you won.");
                break;
            case State.Lost:
                Debug.Log("you lost");
                break;
            case State.NONE:
                Debug.LogError("State can't be none");
                break;
            default:
                break;
        }
    }
    public IEnumerator PreWavePhase()
    {
        Debug.Log("starting pre wave phase");
        //instantiate every object
        CatchingMiceLevelManager.use.InstantiateWave(currentWave);

        yield return new WaitForSeconds(preWaveTime);
        state = State.Wave;
    }
    public IEnumerator WavePhase()
    {
        Debug.Log("starting wave phase");
        //spawn waves
        CatchingMiceLevelManager.use.SpawnInstantiatedWave(currentWave);
        //wait until wave is done, or cheese has been eaten
        while (amountToKill > 0 && CatchingMiceLevelManager.use.cheeseTiles.Count > 0)
        {

            yield return null;
        }

        state = State.PostWave;
        yield break;
    }
    public void PostWavePhase()
    {
        Debug.Log("starting post wave phase");
        currentWave++;

        //check if start next wave (preWavePhase), cheese has been eaten or waves has been iterated
        if(currentWave > CatchingMiceLevelManager.use.wavesList.Count-1)
        {
            //can be changed so when you want infinite levels
            state = State.Won;
        }
        else if(CatchingMiceLevelManager.use.cheeseTiles.Count <= 0)
        {
            //you lost
            state = State.Lost;
        }
        else
        {
            //still waves left, get next wave
            state = State.PreWave;
        }
    }
    public void EndPhase()
    {
        Debug.Log("starting end phase");
        //check if won, or lost
        if(state == State.Won)
        {
            gameRunning = false;
        }
        if(state == State.Lost)
        {

        }
    }
    public void StartGame()
    {
        CatchingMiceLevelManager.use.ClearLevel();
        CatchingMiceLevelManager.use.BuildLevel(0);
        state = State.PreWave;
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
        StartGame();
	}
    public void ModifyPickUpCount(int modifyValue)
    {
        pickupCount += modifyValue;
    }
    public void SetAmountToKill(int amount)
    {
        amountToKill = amount;
        Debug.LogWarning("amount to kill :" + amountToKill);
    }
    public void ModifyAmountToKill(int modifyValue)
    {
        amountToKill += modifyValue;
        //Debug.Log("Modified, amount is now : " + amountToKill);
    }
	// Update is called once per frame
    protected void Update()
    {
        if (gameRunning)
            timer += Time.deltaTime;
    }
}
