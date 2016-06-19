using UnityEngine;
using System.Collections;

public class ProxyUpdater: MonoBehaviour{
	public void UpdatePosition( Vector3 parentPos ){
//		Vector3 proxyPos = new Vector3( parentPos );

		gameObject.transform.position = parentPos;
	}
}
