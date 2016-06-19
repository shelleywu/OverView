using UnityEngine;
using System.Collections;

public class ProxyUpdater: MonoBehaviour{
	private const float realX =	0.42f;
	private const float realY =	-0.375f;
	private const	float realZ =	1.29f;
	private const	float miniX = 0.00556f;
	private const	float miniY = -0.00356f;
	private const	float miniZ = -0.00695f;
	private const	float ratioX = ( miniX/realX )*10;
	private const	float ratioY = ( miniY/realY )*10;
	private const	float ratioZ = ( miniZ/realZ )*10;

	public static Vector3 MiniaturizePosition( Vector3 realWorldPos ){
		return new Vector3( realWorldPos.x * ratioX, realWorldPos.y * ratioY, realWorldPos.z * ratioZ );
	}
}