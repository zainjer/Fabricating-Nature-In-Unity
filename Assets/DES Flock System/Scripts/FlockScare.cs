using UnityEngine;
using System.Collections;

public class FlockScare : MonoBehaviour {

	public LandingSpotController[] landingSpotControllers;
	public float scareInterval = 0.1f;
	public float distanceToScare = 2f;
	public int checkEveryNthLandingSpot = 1;
	public int InvokeAmounts = 1;
	private int lsc;
	private int ls;
	private LandingSpotController currentController;

	void CheckProximityToLandingSpots() {
		IterateLandingSpots();
		if (currentController._activeLandingSpots > 0 && CheckDistanceToLandingSpot(landingSpotControllers[lsc])) {
			landingSpotControllers[lsc].ScareAll();
		}
		Invoke("CheckProximityToLandingSpots", scareInterval);
	}

	void IterateLandingSpots() {
		ls += checkEveryNthLandingSpot;
		currentController = landingSpotControllers[lsc];
		int cc = currentController.transform.childCount;
		if (ls > cc - 1) {
			ls = ls - cc;
			if (lsc < landingSpotControllers.Length - 1)
				lsc++;
			else
				lsc = 0;
		}
	}

	bool CheckDistanceToLandingSpot(LandingSpotController lc) {		
	Transform lcT = lc.transform;
	Transform lsT = lcT.GetChild(ls);
	LandingSpot lcSpot = lsT.GetComponent<LandingSpot>();
	if(lcSpot.landingChild != null){
			float d = (lsT.position - transform.position).sqrMagnitude;
		if(d<distanceToScare*distanceToScare){
			return true;
		}
	}
	return false;	
}

void Invoker() {
	for (int i = 0; i < InvokeAmounts; i++){
			float s = (scareInterval / InvokeAmounts)*i;
		Invoke("CheckProximityToLandingSpots", scareInterval + s);
	}
}

void OnEnable() {
	CancelInvoke("CheckProximityToLandingSpots");
	if (landingSpotControllers.Length > 0)
		Invoker();
#if UNITY_EDITOR
	else
		Debug.Log("Please assign LandingSpotControllers to FlockScare");
#endif
}

void OnDisable() {
	CancelInvoke("CheckProximityToLandingSpots");
}

#if UNITY_EDITOR
void OnDrawGizmos() {
	Gizmos.color = Color.red;
	Gizmos.DrawWireSphere(transform.position, distanceToScare);
	if (InvokeAmounts <= 0) InvokeAmounts = 1;
}
#endif
}