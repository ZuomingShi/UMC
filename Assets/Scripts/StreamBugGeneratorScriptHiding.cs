﻿using UnityEngine;
using System.Collections;

public class StreamBugGeneratorScriptHiding : StreamBugGeneratorScriptParent {

    public float sinOscillateAmpRange;
    public float sinOscillateSpeedPerSecond;
    public float totalAngleChangeRangeMin;
    public float totalAngleChangeRangeMax;
    protected float totalAngleChange;
    protected float sinOscillateAmp;
    protected float sinOscillateSpeed;

    public override void Start () {
		totalBugs = VariablesManager.HidingTotalBugs;
		arrivalTime = VariablesManager.HidingArrivalTime;
		totalDuration = VariablesManager.HidingDuration;
		speed = VariablesManager.HidingSpeed;
		initialAngle = Random.Range(-VariablesManager.HidingAngleRange, VariablesManager.HidingAngleRange);
        totalAngleChange = Random.Range(totalAngleChangeRangeMin, totalAngleChangeRangeMax);
        totalAngleChange = Mathf.FloorToInt(Random.Range(0f, 2f)) == 1 ? totalAngleChange : -totalAngleChange;
        sinOscillateAmp = Random.Range(0f, sinOscillateAmpRange);
        sinOscillateSpeed = Random.Range(0f, sinOscillateSpeedPerSecond) / 2f / Mathf.PI;
        float lt = totalDuration/totalBugs * (bugCounter - 1);
        aimedAngle =  initialAngle + totalAngleChange * lt / totalDuration + sinOscillateAmp * Mathf.Sin(sinOscillateSpeed * lt);
        base.Start();

    }

    protected override void AngleChange() {
        bugCounter++;
        float lt = totalDuration/totalBugs * (bugCounter - 1);
        aimedAngle =  initialAngle + totalAngleChange * lt / totalDuration + sinOscillateAmp * Mathf.Sin(sinOscillateSpeed * lt);
    }
}
