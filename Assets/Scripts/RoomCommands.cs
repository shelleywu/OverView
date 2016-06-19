using UnityEngine;
using System.Collections;

public class RoomCommands: MonoBehaviour{
	private Vector3 fullScale;
	private Vector3 smallScale;
	private Vector3 currentScale;

	private Vector3 fullPosition;
	private Vector3 smallPosition;
	private Vector3 currentPosition;

	private float smooth = 1.0f;
	private Renderer myRenderer;

	void Start(){
		myRenderer = GetComponent< Renderer >();

		fullScale = transform.localScale;
		smallScale = new Vector3( 0.0554647f, 0.0554647f, 0.0554647f );
		currentScale = fullScale;

		fullPosition = transform.localPosition;
		smallPosition = new Vector3( 0.00556f, -0.00356f, -0.00695f );
		currentPosition = fullPosition;
	}

	void Update(){
		float time = Time.deltaTime*smooth;

		transform.localScale = Vector3.Lerp( transform.localScale, currentScale, time );
		transform.localPosition = Vector3.Lerp( transform.localPosition, currentPosition, time );
	}

	public void OnShrink(){
		Debug.Log( "Shrinking world" );
		currentScale = smallScale;
		currentPosition = smallPosition;
	}

	public void OnReset(){
		Debug.Log( "Resetting world" );
		currentScale = fullScale;
		currentPosition = fullPosition;
	}

	public void OnToggleXRay(){
		Debug.Log( "Toggling X-Ray" );
		myRenderer.enabled = !myRenderer.enabled;
	}
}