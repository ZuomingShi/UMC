﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HazardManagerScript : MonoBehaviour {

	private bool inCutscene = true;
	private bool test = false;
	private float timeElapsed = 0f;
    private float timeSinceLastHazard = 0f;
    private HazardEnum previousHazard = HazardEnum.NONE;
    private HazardEnum previousPreviousHazard = HazardEnum.NONE;
    private float secondsPerHazard;
    private Tween secondsPerHazardTween;

	public Dictionary<HazardEnum, GameObject> hazardDictionary = new Dictionary<HazardEnum, GameObject>();
	private List< HazardEntry > hazardEntries = new List< HazardEntry >();
    public GameObject blizzardPrefab;
    public GameObject bugPrefab;
    public GameObject tramplePrefab;

    public GameObject lightningPrefab;
    public GameObject heavyWindPrefab;
    public GameObject swarmOfBugsPrefab;

    private HazardEnum[] firstStageHazards = {HazardEnum.BLIZZARD, HazardEnum.TRAMPLE, HazardEnum.BUG};
    private HazardEnum[] secondStageHazards = {HazardEnum.LIGHTNING, HazardEnum.HEAVY_WIND, HazardEnum.SWARM_OF_BUGS};

	// Use this for initialization
	void Start () {
        float initialSecondsPerHazard = 2f;
        float finalSecondsPerHazard = 0.4f;
        secondsPerHazard = initialSecondsPerHazard;
        float totalTimeToWeen = 90f; //In Game Seconds
        secondsPerHazardTween = DOTween.To(() => secondsPerHazard, x => secondsPerHazard = x, finalSecondsPerHazard, totalTimeToWeen);
        secondsPerHazardTween.Pause();
        
        HazardEnum next = firstStageHazards[Mathf.FloorToInt(Random.Range(0f, 3f))];
        AddHazard(next, timeElapsed + secondsPerHazard);
	}
	
	// Update is called once per frame
	void Update () {
		if (!inCutscene){
			HazardEntry toCall = null;
			timeElapsed += Time.deltaTime;
			foreach(HazardEntry he in hazardEntries){
				if(he.time < timeElapsed){
					toCall = he;
					break;
				}
			}

			if (toCall != null) {
				hazardEntries.Remove(toCall);
                switch(Globals.stateManager.currentStage){
                    case(Globals.STAGE_ONE):{
                        HazardEnum next = firstStageHazards[Mathf.FloorToInt(Random.Range(0f, 3f))];

                        //Do not have the same 3 hazards in a row.
                        while(next == previousHazard && next == previousPreviousHazard){
                            next = firstStageHazards[Mathf.FloorToInt(Random.Range(0f, 3f))];
                        }

                        AddHazard(next, timeElapsed + secondsPerHazard);
                        break;
                    }
                    case(Globals.STAGE_TWO):{
                        HazardEnum next = secondStageHazards[Mathf.FloorToInt(Random.Range(0f, 3f))];
                        
                        //Do not have the same 3 hazards in a row.
                        while(next == previousHazard && next == previousPreviousHazard){
                            next = secondStageHazards[Mathf.FloorToInt(Random.Range(0f, 3f))];
                        }
                        
                        AddHazard(next, timeElapsed + secondsPerHazard);

                        break;
                    }
                }
				Debug.Log ("Hazard called");
                Debug.Log ("Seconds per hazards = " + secondsPerHazard);
			}
		}
	}

	public void AddHazard (HazardEnum h, float t) {
		hazardEntries.Add ( new HazardEntry(h, t) );
	}
	
	public void StartCutscene(){
		Debug.Log ("HazardManager.StartCutscene");
        inCutscene = true;
        secondsPerHazardTween.Play();
	}
	
	public void EndCutscene(){
		Debug.Log ("HazardManager.EndCutscene");
        inCutscene = false;
        secondsPerHazardTween.Pause();
	}
}


class HazardEntry{
	public HazardEnum Hazard;
	public float time;
	
	public HazardEntry(HazardEnum h, float t){
		time = t;
		Hazard = h;
	}
}