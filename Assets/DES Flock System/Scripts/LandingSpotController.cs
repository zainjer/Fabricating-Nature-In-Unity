using UnityEngine;
using System.Collections;

public class LandingSpotController:MonoBehaviour
{
    public bool _randomRotate = true;
    public Vector2 _autoCatchDelay = new Vector2(10.0f, 20.0f);
    public Vector2 _autoDismountDelay = new Vector2(10.0f, 20.0f);
    public float _maxBirdDistance = 20.0f;
    public float _minBirdDistance = 5.0f;
    public bool _takeClosest;
    public FlockController _flock;
    public bool _landOnStart;
    public bool _soarLand = true;
    public bool _onlyBirdsAbove;
    public float _landingSpeedModifier = .5f;
    public float _landingTurnSpeedModifier = 5.0f;
    public Transform _featherPS;
    public Transform _thisT;
    public int _activeLandingSpots;
    
    public float _snapLandDistance = 0.1f;
    public float _landedRotateSpeed = 0.01f;

	public float _gizmoSize = 0.2f;

    public void Start() {
    	if(_thisT == null) _thisT = transform;
    	if(_flock == null){
    	 _flock = (FlockController)GameObject.FindObjectOfType(typeof(FlockController));
    	 Debug.Log(this + " has no assigned FlockController, a random FlockController has been assigned");
    	 }
    	 
    	#if UNITY_EDITOR
    	if(_autoCatchDelay.x >0 &&(_autoCatchDelay.x < 5||_autoCatchDelay.y < 5)){
    		Debug.Log(this.name + ": autoCatchDelay values set low, this might result in strange behaviours");
    	}
    	#endif
    	
    	if(_landOnStart){
    		StartCoroutine(InstantLandOnStart(.1f));
    	}
    }
    
    public void ScareAll() {
    	ScareAll(0.0f,1.0f);
    }
    
    public void ScareAll(float minDelay,float maxDelay) {
    	for(int i=0;  i< _thisT.childCount; i++){
    		if(_thisT.GetChild(i).GetComponent<LandingSpot>() != null){
    		LandingSpot spot = _thisT.GetChild(i).GetComponent<LandingSpot>();
    		spot.Invoke("ReleaseFlockChild", Random.Range(minDelay,maxDelay));
    		}
    	}
    }
    
    public void LandAll() {
    	for(int i=0;  i< _thisT.childCount; i++){	
    		if(_thisT.GetChild(i).GetComponent<LandingSpot>() != null){		
    		LandingSpot spot = _thisT.GetChild(i).GetComponent<LandingSpot>();
    		StartCoroutine(spot.GetFlockChild(0.0f,2.0f));
    		}
    	}
    }
    
    public IEnumerator InstantLandOnStart(float delay) {
    	yield return new WaitForSeconds(delay);
    	for(int i=0;  i< _thisT.childCount; i++){			
    		if(_thisT.GetChild(i).GetComponent<LandingSpot>() != null){
    		LandingSpot spot = _thisT.GetChild(i).GetComponent<LandingSpot>();
    		spot.InstantLand();
    		}
    	}
    }
    
    public IEnumerator InstantLand(float delay) {
    	yield return new WaitForSeconds(delay);
    	for(int i=0;  i< _thisT.childCount; i++){	
    		if(_thisT.GetChild(i).GetComponent<LandingSpot>() != null){		
    		LandingSpot spot = _thisT.GetChild(i).GetComponent<LandingSpot>();
    		spot.InstantLand();
    		}
    	}
    }
}