using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager: MonoBehaviour{
	public RoomCommands commandHandlerXRay;
	public RoomCommands commandHandlerOpaque;
	private Renderer xrayRenderer;
	private Renderer opaqueRenderer;
	public GameObject roomManager;

	private KeywordRecognizer keywordRecognizer = null;
	private Dictionary< string, System.Action > keywords = new Dictionary< string, System.Action >();

	public void OnGUI(){
		GUI.color = Color.white;
		if( GUI.Button( new Rect( Screen.width/2, 10, 240, 20 ), "Shrink world" ) ){
			commandHandlerXRay.SendMessage( "OnShrink" );
			commandHandlerOpaque.SendMessage( "OnShrink" );
		}

		if( GUI.Button( new Rect( Screen.width/2, 30, 240, 20 ), "Reset world" ) ){
			commandHandlerXRay.SendMessage( "OnReset" );
			commandHandlerOpaque.SendMessage( "OnReset" );
		}

		if( GUI.Button( new Rect( Screen.width/2, 50, 240, 20 ), "Toggle X-Ray" ) ){
			xrayRenderer.enabled = !xrayRenderer.enabled;
			opaqueRenderer.enabled = !opaqueRenderer.enabled;
		}

		if( GUI.Button( new Rect( Screen.width/2, 70, 240, 20 ), "Toggle Visibility" ) )
			roomManager.SetActive( !roomManager.activeInHierarchy );
	}

	void Start(){
		xrayRenderer = commandHandlerXRay.GetComponent< Renderer >();
		opaqueRenderer = commandHandlerOpaque.GetComponent< Renderer >();

		keywords.Add(
			"Shrink world", () => { // Call the OnReset method on every descendant object.
				commandHandlerXRay.SendMessage( "OnShrink" );
				commandHandlerOpaque.SendMessage( "OnShrink" );
			}
		);

		keywords.Add(
			"Reset world", () => { // Call the OnReset method on every descendant object.
				commandHandlerXRay.SendMessage( "OnReset" );
				commandHandlerOpaque.SendMessage( "OnReset" );
			}
		);

		keywords.Add(
			"Toggle X-Ray", () => {
				xrayRenderer.enabled = !xrayRenderer.enabled;
				opaqueRenderer.enabled = !opaqueRenderer.enabled;
			}
		);

		keywords.Add(
			"Toggle Visibility", () => {
				roomManager.SetActive( !roomManager.activeInHierarchy );
			}
		);

		// Tell the KeywordRecognizer about our keywords.
		keywordRecognizer = new KeywordRecognizer( keywords.Keys.ToArray() );

		// Register a callback for the KeywordRecognizer and start recognizing!
		keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
		keywordRecognizer.Start();
	}

	private void KeywordRecognizer_OnPhraseRecognized( PhraseRecognizedEventArgs args ){
		System.Action keywordAction;
		if( keywords.TryGetValue( args.text, out keywordAction ) )
			keywordAction.Invoke();
	}
}