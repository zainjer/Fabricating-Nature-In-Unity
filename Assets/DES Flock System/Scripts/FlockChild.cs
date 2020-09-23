using UnityEngine;

public class FlockChild:MonoBehaviour
{
    [HideInInspector] 
    public FlockController _spawner;
    [HideInInspector] 
    public Vector3 _wayPoint;
    public float _speed;
    [HideInInspector] 		
    public bool _dived =true;
    [HideInInspector] 
    public float _stuckCounter;
    [HideInInspector] 
    public float _damping;
    [HideInInspector] 
    public bool _soar = true;
    [HideInInspector] 
    public bool _landing;
    [HideInInspector] 
    public float _targetSpeed;
    [HideInInspector] 
    public bool _move = true;
    public GameObject _model;
    public Transform _modelT;
    [HideInInspector] 
    public float _avoidValue;
    [HideInInspector] 
    public float _avoidDistance;
    float _soarTimer;	
    bool _instantiated;
    static int _updateNextSeed = 0;		
    int _updateSeed = -1;
    [HideInInspector] 
    public bool _avoid = true;
    public Transform _thisT;

	public Vector3 _landingPosOffset;

    public void Start(){
    	FindRequiredComponents();
    	Wander(0.0f);
    	SetRandomScale();
    	_thisT.position = findWaypoint();	
    	RandomizeStartAnimationFrame();	
    	InitAvoidanceValues();
    	_speed = _spawner._minSpeed;
    	_spawner._activeChildren++;
    	_instantiated = true;
    	if(_spawner._updateDivisor > 1){
    		int _updateSeedCap = _spawner._updateDivisor -1;
    		_updateNextSeed++;
    	    this._updateSeed = _updateNextSeed;
    	    _updateNextSeed = _updateNextSeed % _updateSeedCap;
    	}
    }
    
    public void Update() 
    {
    	if (_spawner._updateDivisor <=1 || _spawner._updateCounter == _updateSeed)
        {
    		SoarTimeLimit();
    		CheckForDistanceToWaypoint();
    		RotationBasedOnWaypointOrAvoidance();
    	    LimitRotationOfModel();
    	}
    }
    
    public void OnDisable() {
    	CancelInvoke();
    	_spawner._activeChildren--;
    }
    
    public void OnEnable() {
    	if(_instantiated){
    		_spawner._activeChildren++;
    		if(_landing){
    			_model.GetComponent<Animation>().Play(_spawner._idleAnimation);
    		}else{
    			_model.GetComponent<Animation>().Play(_spawner._flapAnimation);
    		}		
    	}
    }
    
    public void FindRequiredComponents(){
    	if(_thisT == null)		_thisT = transform;	
    	if(_model == null)		_model = _thisT.Find("Model").gameObject;	
    	if(_modelT == null)	_modelT = _model.transform;
    }
    
    public void RandomizeStartAnimationFrame(){
    	foreach(AnimationState state in _model.GetComponent<Animation>()) {
    	 	state.time = Random.value * state.length;
    	}
    }
    
    public void InitAvoidanceValues(){
    	_avoidValue = Random.Range(.3f, .1f);	
    	if(_spawner._birdAvoidDistanceMax != _spawner._birdAvoidDistanceMin){
    		_avoidDistance = Random.Range(_spawner._birdAvoidDistanceMax , _spawner._birdAvoidDistanceMin);
    		return;
    	}
    	_avoidDistance = _spawner._birdAvoidDistanceMin;
    }
    
    public void SetRandomScale(){
    	float sc = Random.Range(_spawner._minScale, _spawner._maxScale);
    	_thisT.localScale=new Vector3(sc,sc,sc);
    }
    
    public void SoarTimeLimit(){	
    	if(this._soar && _spawner._soarMaxTime > 0){ 		
       		if(_soarTimer > _spawner._soarMaxTime){
       			this.Flap();
       			_soarTimer = 0.0f;
       		}else {
       			_soarTimer+=_spawner._newDelta;
       		}
       	}
    }
    
    public void CheckForDistanceToWaypoint(){
    	if(!_landing && (_thisT.position - _wayPoint).magnitude < _spawner._waypointDistance+_stuckCounter){
            Wander(0.0f);
            _stuckCounter=0.0f;
        }else if(!_landing){
        	_stuckCounter+=_spawner._newDelta;
        }else{
        	_stuckCounter=0.0f;
        }
    }
    
    public void RotationBasedOnWaypointOrAvoidance(){
		
    	Vector3 lookit = _wayPoint - _thisT.position;
        if(_targetSpeed > -1 && lookit != Vector3.zero){
        Quaternion rotation = Quaternion.LookRotation(lookit);
    	
    	_thisT.rotation = Quaternion.Slerp(_thisT.rotation, rotation, _spawner._newDelta * _damping);
    	}
    	
    	if(_spawner._childTriggerPos){
    		if((_thisT.position - _spawner._posBuffer).magnitude < 1){
    			_spawner.SetFlockRandomPosition();
    		}
    	}
    	_speed = Mathf.Lerp(_speed, _targetSpeed, _spawner._newDelta* 2.5f);
    	if(_move){
    		_thisT.position += _thisT.forward*_speed*_spawner._newDelta;
    		if(_avoid && _spawner._birdAvoid) 
    		Avoidance();
    	}
    }
    
    public bool Avoidance() {
    	RaycastHit hit = new RaycastHit();
    	Vector3 fwd = _modelT.forward;
    	bool r = false;
    	Quaternion rot = Quaternion.identity;
    	Vector3 rotE = Vector3.zero;
    	Vector3 pos = Vector3.zero;
    	pos = _thisT.position;
    	rot = _thisT.rotation;
    	rotE = _thisT.rotation.eulerAngles;
    	if (Physics.Raycast(_thisT.position, fwd+(_modelT.right*_avoidValue), out hit, _avoidDistance, _spawner._avoidanceMask)){	
    		rotE.y -= _spawner._birdAvoidHorizontalForce*_spawner._newDelta*_damping;
    		rot.eulerAngles = rotE;
    		_thisT.rotation = rot;
    		r= true;
    	}else if (Physics.Raycast(_thisT.position,fwd+(_modelT.right*-_avoidValue), out hit, _avoidDistance, _spawner._avoidanceMask)){
    		rotE.y += _spawner._birdAvoidHorizontalForce*_spawner._newDelta*_damping;
    		rot.eulerAngles = rotE;
    		_thisT.rotation = rot;
    		r= true;		
    	}
    	if (_spawner._birdAvoidDown && !this._landing && Physics.Raycast(_thisT.position, -Vector3.up, out hit, _avoidDistance, _spawner._avoidanceMask)){			
    		rotE.x -= _spawner._birdAvoidVerticalForce*_spawner._newDelta*_damping;
    		rot.eulerAngles = rotE;
    		_thisT.rotation = rot;				
    		pos.y += _spawner._birdAvoidVerticalForce*_spawner._newDelta*.01f;
    		_thisT.position = pos;
    		r= true;			
    	}else if (_spawner._birdAvoidUp && !this._landing && Physics.Raycast(_thisT.position, Vector3.up, out hit, _avoidDistance, _spawner._avoidanceMask)){			
    		rotE.x += _spawner._birdAvoidVerticalForce*_spawner._newDelta*_damping;
    		rot.eulerAngles = rotE;
    		_thisT.rotation = rot;
    		pos.y -= _spawner._birdAvoidVerticalForce*_spawner._newDelta*.01f;
    		_thisT.position = pos;
    		r= true;			
    	}
    	return r;
    }
    
    public void LimitRotationOfModel(){
    	Quaternion rot = Quaternion.identity;
    	Vector3 rotE = Vector3.zero;
    	rot = _modelT.localRotation;
    	rotE = rot.eulerAngles;	
    	if((_soar && _spawner._flatSoar|| _spawner._flatFly && !_soar)&& _wayPoint.y > _thisT.position.y||_landing){	
    		rotE.x = Mathf.LerpAngle(_modelT.localEulerAngles.x, -_thisT.localEulerAngles.x, _spawner._newDelta * 1.75f);
    		rot.eulerAngles = rotE;
    		_modelT.localRotation = rot;
    	}else{	
    		rotE.x = Mathf.LerpAngle(_modelT.localEulerAngles.x, 0.0f, _spawner._newDelta * 1.75f);
    		rot.eulerAngles = rotE;
    		_modelT.localRotation = rot;
    	}
    }
    
    public void Wander(float delay){
    	if(!_landing){
    		_damping = Random.Range(_spawner._minDamping, _spawner._maxDamping);       
    	    _targetSpeed = Random.Range(_spawner._minSpeed, _spawner._maxSpeed);
    	    Invoke("SetRandomMode", delay);
    	}
    }
    
    public void SetRandomMode(){
    	CancelInvoke("SetRandomMode");
    	if(!_dived && Random.value < _spawner._soarFrequency){
    	   	 	Soar();
    		}else if(!_dived && Random.value < _spawner._diveFrequency){	
    			Dive();
    		}else{	
    			Flap();
    		}
    }
    
    public void Flap(){
    	if(_move){
    	 	if(this._model != null) _model.GetComponent<Animation>().CrossFade(_spawner._flapAnimation, .5f);
    		_soar=false;
    		animationSpeed();
    		_wayPoint = findWaypoint();
    		_dived = false;
    	}
    }
    
    public Vector3 findWaypoint(){
    	Vector3 t = Vector3.zero;
    	t.x = Random.Range(-_spawner._spawnSphere, _spawner._spawnSphere) + _spawner._posBuffer.x;
    	t.z = Random.Range(-_spawner._spawnSphereDepth, _spawner._spawnSphereDepth) + _spawner._posBuffer.z;
    	t.y = Random.Range(-_spawner._spawnSphereHeight, _spawner._spawnSphereHeight) + _spawner._posBuffer.y;
    	return t;
    }
    
    public void Soar(){
    	if(_move){
    		 _model.GetComponent<Animation>().CrossFade(_spawner._soarAnimation, 1.5f);
    	   	_wayPoint= findWaypoint();
    	    _soar = true;
        }
    }
    
    public void Dive(){
    	if(_spawner._soarAnimation!=null){
    		_model.GetComponent<Animation>().CrossFade(_spawner._soarAnimation, 1.5f);
    	}else{
    		foreach(AnimationState state in _model.GetComponent<Animation>()) {
       	 		if(_thisT.position.y < _wayPoint.y +25){
       	 			state.speed = 0.1f;
       	 		}
       	 	}
     	}
     	_wayPoint= findWaypoint();
    	_wayPoint.y -= _spawner._diveValue;
    	_dived = true;
    }
    
    public void animationSpeed(){
    	foreach(AnimationState state in _model.GetComponent<Animation>()) {
    		if(!_dived && !_landing){
    			state.speed = Random.Range(_spawner._minAnimationSpeed, _spawner._maxAnimationSpeed);
    		}else{
    			state.speed = _spawner._maxAnimationSpeed;
    		}   
    	}
    }
}