﻿using UnityEngine;
using System.Collections;


public class StateManagerScript : MonoBehaviour {

	public int currentStage { get; set; }
	public const float secondsPerCutscene = 4f;
	public bool inCutscene = true;
    public float lives {get; set;}
    public _Mono leafLifeIndicator;
    public AudioSource audioSource{get{return GetComponent<AudioSource>();}}

    //private Vector2 originalLifeIndicatorXYScale;

    public bool isGameOver = false;
    public float gameDuration = 0f;
	private int totalSeconds;
    public int secondsForFirtstPart{get; set;}
    public int secondsForSecondPart{get; set;}
    public int secondsForThirdPart{get; set;}
	private float secondTracker; //Keeps track of amount of time passed since last second.
	private GUITimerManagerScript guiTimerManager;
	private TreeManagerScript treeManager;
	private CameraManagerScript cameraManager;
	//private ShieldScript shieldManager;
	private HazardManagerScript hazardManager;


	public void Start(){

		LoggingManager.startLogging ();

		VariablesManager.getConfigValues ();

		Debug.Log ("HidingDuration = " + VariablesManager.HidingDuration + "\nRoundSpeed = " + VariablesManager.RoundSpeed);
		Debug.Log ("DoubleFileTotalBugs = " + VariablesManager.DoubleFileTotalBugs);

        secondsForFirtstPart = 3000;
        secondsForSecondPart = 3000;
        secondsForThirdPart = 15000;

        currentStage = Globals.STAGE_STARTING;
		guiTimerManager = Globals.guiTimerManager;
		treeManager = Globals.treeManager;
		cameraManager = Globals.cameraManager;
		//shieldManager = Globals.shieldManager;
		hazardManager = Globals.hazardManager;

		totalSeconds = secondsForFirtstPart;
		guiTimerManager.SetTotalSeconds (totalSeconds);

        lives = 1f;
		GameStart ();
	}

	public void Update(){
		UpdateTime ();
        if(lives <= 0 && !isGameOver){
            GameOver();
        }

        leafLifeIndicator.xy = Globals.inputManager.normToScreenPoint(0.06f, 0.9f);
        leafLifeIndicator.xys = new Vector2(lives, lives) * Globals.cameraManager.cameraRatio;
	}

	private void UpdateTime(){
        if(!isGameOver){
            gameDuration += Time.deltaTime;

    		secondTracker += Time.deltaTime;
    		if(secondTracker >= 1.0f){
    			if(totalSeconds > 0){
    				totalSeconds -= 1;
    			}
    			secondTracker = 0.0f;
    		}

    		if (totalSeconds == 0 && !inCutscene) {
    			StartCutscene();
    		}

    		guiTimerManager.SetTotalSeconds (totalSeconds);
        }
	}

    private void GameOver() {
        if(!isGameOver){
            float time = (currentStage == Globals.STAGE_THREE) ? Globals.treeManager.GetCutsceneTime() : secondsPerCutscene;

            isGameOver = true;
            guiTimerManager.GameOver ();
            hazardManager.GameOver ();
            cameraManager.GameOver (time);
            treeManager.GameOver(time);
            Invoke("ShowGameOverGUI", time - 1f);
        }
        //treeManager.GameOver(time);

    }

    private void ShowGameOverGUI(){
		LoggingManager.recordComboCount ();
		LoggingManager.recordRewards ();
		LoggingManager.endLogging ();

        Globals.gameOverGUIScript.Show();
    }

	private void GameStart() {
        float time = secondsPerCutscene / 2;
        cameraManager.GameStart (time);
        treeManager.GameStart (time);
        totalSeconds = (int)Mathf.Ceil(time);
        Invoke("EndCutscene", time);
	}

	private void StartCutscene(){
		if (inCutscene) {
			Debug.LogWarning( "Call to starCutscene when cutscene is still active." );
		}
		inCutscene = true;
        
        guiTimerManager.CutsceneStart ();
        cameraManager.CutsceneStart (secondsPerCutscene);
        hazardManager.CutsceneStart ();
        treeManager.CutsceneStart(secondsPerCutscene);
        Invoke("EndCutscene", secondsPerCutscene + 0.1f);
	}
	
	private void EndCutscene(){



		if (!inCutscene) {
			Debug.LogWarning( "Call to endCutscene when cutscene is still active." );
		}
		inCutscene = false;

		guiTimerManager.CutsceneEnd ();
		cameraManager.CutsceneEnd ();
		hazardManager.CutsceneEnd ();
        treeManager.CutsceneEnd();

		currentStage += 1;
		switch (currentStage) {
    		case Globals.STAGE_ONE: {totalSeconds = secondsForFirtstPart; break;}
            case Globals.STAGE_TWO: {totalSeconds = secondsForSecondPart; break;}
            case Globals.STAGE_THREE: {totalSeconds = secondsForThirdPart; break;}
		}

	}

    void OnDestory(){
        Destroy(leafLifeIndicator.gameObject);
    }


}
