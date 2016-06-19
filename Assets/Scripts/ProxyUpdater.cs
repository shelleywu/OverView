using UnityEngine;
using System.Collections;

public class ProxyUpdater: MonoBehaviour{
	private const float scaleRatio = 0.0554648f;

	//private const float offsetX = -0.49781264f;
	//private const float offsetY = -0.3559896f;
	//private const float offsetZ = 0.737765f;

	private const float offsetX = -0.1011222f;
	private const float offsetY = -0.7102006f;
	private const float offsetZ = 1.956237f;



	public static Vector3 MiniaturizePosition( Vector3 realWorldPos ){
		return new Vector3( ( realWorldPos.x*scaleRatio )+offsetX, ( realWorldPos.y*scaleRatio )+offsetY, ( realWorldPos.z*scaleRatio )+offsetZ );
	}
}