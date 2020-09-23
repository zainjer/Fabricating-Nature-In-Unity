using UnityEngine;
using System.Collections.Generic;

public class FlockController:MonoBehaviour
{    
    public FlockChild _childPrefab;
    public int _childAmount = 250;
    public bool _slowSpawn;	
    public float _spawnSphere = 3.0f;
    public float _spawnSphereHeight = 3.0f;
    public float _spawnSphereDepth = -1.0f;
    public float _minSpeed = 6.0f;
    public float _maxSpeed = 10.0f;
    public float _minScale = .7f;
    public float _maxScale = 1.0f;
    public float _soarFrequency = 0.0f;
    public string _soarAnimation="Soar";
    public string _flapAnimation="Flap";
    public string _idleAnimation="Idle";
    public float _diveValue = 7.0f;
    public float _diveFrequency = 0.5f;
    public float _minDamping = 1.0f;
    public float _maxDamping = 2.0f;
    public float _waypointDistance = 1.0f;
    public float _minAnimationSpeed = 2.0f;
    public float _maxAnimationSpeed = 4.0f;
    public float _randomPositionTimer = 10.0f;
    public float _positionSphere = 25.0f;
    public float _positionSphereHeight = 25.0f;
    public float _positionSphereDepth = -1.0f;
    public bool _childTriggerPos;
    public bool _forceChildWaypoints;
    public float _forcedRandomDelay = 1.5f;
    public bool _flatFly;
    public bool _flatSoar;
    public bool _birdAvoid;
    public int _birdAvoidHorizontalForce = 1000;
    public bool _birdAvoidDown;
    public bool _birdAvoidUp;
    public int _birdAvoidVerticalForce = 300;
    public float _birdAvoidDistanceMax = 4.5f;
    public float _birdAvoidDistanceMin = 5.0f;
    public float _soarMaxTime;
    public LayerMask _avoidanceMask = (LayerMask)(-1);
    public List<FlockChild> _roamers;
    public Vector3 _posBuffer;
    public int _updateDivisor = 1;
    public float _newDelta;
    public int _updateCounter;
    public float _activeChildren;
    public bool _groupChildToNewTransform;
    public Transform _groupTransform;
    public string _groupName = "";
    public bool _groupChildToFlock;
    public Vector3 _startPosOffset;
    public Transform _thisT;
    
    public void Start() {
    	_thisT = transform;
    	if(_positionSphereDepth == -1){
    		_positionSphereDepth = _positionSphere;
    	}	
    	if(_spawnSphereDepth == -1){
    		_spawnSphereDepth = _spawnSphere;
    	}
    	///FIX	
    	_posBuffer = _thisT.position+_startPosOffset;
    	if(!_slowSpawn){
    		AddChild(_childAmount);
    	}
    	if(_randomPositionTimer > 0) InvokeRepeating("SetFlockRandomPosition", _randomPositionTimer, _randomPositionTimer); // > C
    }
    
    public void AddChild(int amount){
    	if(_groupChildToNewTransform)InstantiateGroup();
    	for(int i=0;i<amount;i++){
    		FlockChild obj = (FlockChild)Instantiate(_childPrefab);	
    	    obj._spawner = this;
    	    _roamers.Add(obj);
    	   AddChildToParent(obj.transform);
    	}	
    }
    
    public void AddChildToParent(Transform obj){	
        if(_groupChildToFlock){
    		obj.parent = transform;
    		return;
    	}
    	if(_groupChildToNewTransform){
    		obj.parent = _groupTransform;
    		return;
    	}
    }
    
    public void RemoveChild(int amount){
    	for(int i=0;i<amount;i++){
    		FlockChild dObj = _roamers[_roamers.Count-1];
    		_roamers.RemoveAt(_roamers.Count-1);
    		Destroy(dObj.gameObject);
    	}
    }
    
    public void Update() {
    	if(_activeChildren > 0){
    		if(_updateDivisor > 1){
    			_updateCounter++;
    		    _updateCounter = _updateCounter % _updateDivisor;	
    			_newDelta = Time.deltaTime*_updateDivisor;	
    		}else{
    			_newDelta = Time.deltaTime;
    		}	
    	}
    	UpdateChildAmount();
    }
    
    public void InstantiateGroup(){
    	if(_groupTransform != null) return;
    	GameObject g = new GameObject();
    
    	_groupTransform = g.transform;
    	_groupTransform.position = _thisT.position;
    	
    	if(_groupName != ""){
    		g.name = _groupName;
    		return;
    	}	
    	g.name = _thisT.name + " Fish Container";
    }
    
    public void UpdateChildAmount(){	
    	if(_childAmount>= 0 && _childAmount < _roamers.Count){
    		RemoveChild(1);
    		return;
    	}
    	if (_childAmount > _roamers.Count){	
    		AddChild(1);
    	}
    }
    
    public void OnDrawGizmos() {
    	if(_thisT == null) _thisT = transform;
    		if(!Application.isPlaying && _posBuffer != _thisT.position+_startPosOffset){
    			_posBuffer = _thisT.position+_startPosOffset;
           		
           	}
           	if(_positionSphereDepth == -1){
    				_positionSphereDepth = _positionSphere;
    			}	
    			if(_spawnSphereDepth == -1){
    				_spawnSphereDepth = _spawnSphere;
    			}
           	Gizmos.color = Color.blue;
           	Gizmos.DrawWireCube (_posBuffer, new Vector3(_spawnSphere*2, _spawnSphereHeight*2 ,_spawnSphereDepth*2));
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube (_thisT.position, new Vector3((_positionSphere*2)+_spawnSphere*2, (_positionSphereHeight*2)+_spawnSphereHeight*2 ,(_positionSphereDepth*2)+_spawnSphereDepth*2));
        }
    
    //Set waypoint randomly inside box
    public void SetFlockRandomPosition() {
    	Vector3 t = Vector3.zero;
    	t.x = Random.Range(-_positionSphere, _positionSphere) + _thisT.position.x;
    	t.z = Random.Range(-_positionSphereDepth, _positionSphereDepth) + _thisT.position.z;
    	t.y = Random.Range(-_positionSphereHeight, _positionSphereHeight) + _thisT.position.y;
    	_posBuffer = t;	
    	if(_forceChildWaypoints){
    		for(int i = 0; i < _roamers.Count; i++) {
      		 	(_roamers[i]).Wander(Random.value*_forcedRandomDelay);
    		}	
    	}
    }
    
    public void destroyBirds() {
    		for(int i = 0; i < _roamers.Count; i++) {
    			Destroy((_roamers[i]).gameObject);	
    		}
    		_childAmount = 0;
    		_roamers.Clear();
    }
}