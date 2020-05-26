using UnityEngine;

public class ScalabilityControler : MonoBehaviour {

	//3 is all graphics active, 2 is no smoke 60 fps mode, 1 is 30 fps smoke mode, 0 is 30 fps no smoke mode.
	public int scaleSetting = 3;

	public int targetFSP = 60;
	public int inferiorFPS = 45;
	private int superiorFPS = 62;

	// Use this for initialization
	void Start () {
		StartFPS ();
		superiorFPS = inferiorFPS + 5;
	}
	
	// Update is called once per frame
	void Update () {
		//Get Current FPS
		UpdateFPS();
		superiorFPS = inferiorFPS + 10;

		if (m_CurrentFps <= inferiorFPS) {
			if (scaleSetting >= 1) {
				scaleSetting -= 1;
			}
			if (scaleSetting <= 1) {
				if (targetFSP == 60) {
					targetFSP = 30;
					inferiorFPS = 20;
				}
			}
		} else if (m_CurrentFps >= superiorFPS) {
			if (scaleSetting <= 3) {
				scaleSetting += 1;
			}
			if (scaleSetting >= 2 && targetFSP == 30) {
				targetFSP = 60;
				inferiorFPS = 45;
			}
		}
	}

	const float fpsMeasurePeriod = 0.5f;
	private int m_FpsAccumulator = 0;
	private float m_FpsNextPeriod = 0;
	private int m_CurrentFps;


	private void StartFPS()
	{
		m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
	}


	private void UpdateFPS()
	{
		// measure average frames per second
		m_FpsAccumulator++;
		if (Time.realtimeSinceStartup > m_FpsNextPeriod)
		{
			m_CurrentFps = (int) (m_FpsAccumulator/fpsMeasurePeriod);
			m_FpsAccumulator = 0;
			m_FpsNextPeriod += fpsMeasurePeriod;
		}
	}

	public bool shouldProduceSmoke(){
		if (scaleSetting == 2 || scaleSetting == 0) {
			return false;
		} else {
			return true;
		}
	}

}