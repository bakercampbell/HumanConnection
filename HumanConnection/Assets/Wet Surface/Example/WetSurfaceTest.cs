using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WetSurfaceTest : MonoBehaviour 
{
	public bool LinearLerp = false;
	public float LerpTime = 20;
	MeshRenderer _renderer;
	//Non linear lerp
	private float lerpAccuracity = 0.05f;
	private bool WetnessUp;
	void Start()
	{
		_renderer = GetComponent<MeshRenderer> ();
	}
	void FixedUpdate () 
	{
		if (LinearLerp) {
			float wetness = (Time.time - ((int)(Time.time / LerpTime) * LerpTime)) / LerpTime;
			if ((int)(Time.time / LerpTime) % 2 == 0) {
				_renderer.materials [0].SetFloat ("_Wetness", wetness);
			} else {
				_renderer.materials [0].SetFloat ("_Wetness", 1f - wetness);
			}
		} else {
			float wetness = _renderer.materials [0].GetFloat ("_Wetness");
			if (WetnessUp) {
				wetness = Mathf.Lerp (wetness, 1, Time.fixedDeltaTime / LerpTime);
				if (wetness >= 1 - lerpAccuracity) {
					WetnessUp = false;
				}
			} else {
				wetness = Mathf.Lerp (wetness, 0, Time.fixedDeltaTime / LerpTime);
				if (wetness <= lerpAccuracity) {
					WetnessUp = true;
				}
			}
			_renderer.materials [0].SetFloat ("_Wetness", wetness);
		}
	}
}
