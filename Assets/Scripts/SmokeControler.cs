using UnityEngine;

public class SmokeControler : MonoBehaviour {

	public GameControler control;
	public ScalabilityControler scalingController;

	private ParticleSystem smokeProduction;
	private bool isProducingSmoke;
	private bool shouldProduceSmoke;
	private int nextPuff = 500;

	// Use this for initialization
	void Start () {
		smokeProduction = GetComponent<ParticleSystem> ();
		shouldProduceSmoke = true;
		isProducingSmoke = shouldProduceSmoke;
	}
	
	// Update is called once per frame
	void Update () {
		//Can we run smoke on this hardware?
		if (scalingController.shouldProduceSmoke ()) {
			//Direct control, not sure I want to keep this.
			if (control.GAME_STATE == 0) {
				shouldProduceSmoke = true;
			} else {
				shouldProduceSmoke = false;
			}

			if (isProducingSmoke && !shouldProduceSmoke) {
				smokeProduction.Stop ();
				isProducingSmoke = false;
			} else if (!isProducingSmoke && shouldProduceSmoke) {
				//smokeProduction.Emit (100);
				smokeProduction.Play ();
				isProducingSmoke = true;
			}

			nextPuff--;
			if (nextPuff == 0) {
				smokeProduction.Emit (50);
				nextPuff = Random.Range (250, 1250);
			}
		} else {
			if (isProducingSmoke) {
				isProducingSmoke = false;
				shouldProduceSmoke = false;
				smokeProduction.Stop ();
			}
		}
	}
}
