// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

/// <summary>
/// Broadcasts the head transform of the local user to other users in the session,
/// and adds and updates the head transforms of remote users.
/// Head transforms are sent and received in the local coordinate space of the GameObject
/// this component is on.
/// </summary>
public class RemoteHeadManager: Singleton<RemoteHeadManager> {
	public class RemoteHeadInfo {
		public long UserID;
		public GameObject HeadObject;
	}

	/// <summary>
	/// Keep a list of the remote heads, indexed by XTools userID
	/// </summary>
	Dictionary<long, HeadInfo> remoteHeadInfos = new Dictionary<long, HeadInfo>();

	public class HeadInfo{
		public RemoteHeadInfo headInfo;
		public RemoteHeadInfo headProxyInfo;

		public HeadInfo( RemoteHeadInfo headInfo, RemoteHeadInfo headProxyInfo ){
			this.headInfo = headInfo;
			this.headProxyInfo = headProxyInfo;
		}
	}

	void Start() {
		//TODO: copy this for your messages.
		//TODO: Step 3 Register Message handler for receiving messages of new MessageID Types.
		CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.HeadTransform] = this.UpdateHeadTransform;



		SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;
		SharingSessionTracker.Instance.SessionLeft += Instance_SessionLeft;
	}

	void Update() {
		// Grab the current head transform and broadcast it to all the other users in the session
		Transform headTransform = Camera.main.transform;

		// Transform the head position and rotation from world space into local space
		Vector3 headPosition = this.transform.InverseTransformPoint( headTransform.position );
		Quaternion headRotation = Quaternion.Inverse( this.transform.rotation ) * headTransform.rotation;

		//TODO: Step 5 Call network broadcaster with current game state.
		CustomMessages.Instance.SendHeadTransform( headPosition, headRotation );

	}

	/// <summary>
	/// Called when a new user is leaving.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Instance_SessionLeft( object sender, SharingSessionTracker.SessionLeftEventArgs e ) {
		HeadInfo headInfo = remoteHeadInfos[ e.exitingUserId ];
		DestroyImmediate( headInfo.headInfo.HeadObject );
		DestroyImmediate( headInfo.headProxyInfo.HeadObject );
		remoteHeadInfos.Remove( e.exitingUserId );
	}

	/// <summary>
	/// Called when a user is joining.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Instance_SessionJoined( object sender, SharingSessionTracker.SessionJoinedEventArgs e ) {
		GetRemoteHeadInfo( e.joiningUser.GetID() );
	}

	/// <summary>
	/// Gets the data structure for the remote users' head position.
	/// </summary>
	/// <param name="userID"></param>
	/// <returns></returns>
	public HeadInfo GetRemoteHeadInfo( long userID ) {
		HeadInfo headInfo;

		// Get the head info if its already in the list, otherwise add it
		if( !remoteHeadInfos.TryGetValue( userID, out headInfo ) ){
			headInfo = new HeadInfo( new RemoteHeadInfo(), new RemoteHeadInfo() );

			headInfo.headInfo.UserID = userID;
			headInfo.headProxyInfo.UserID = userID;

			headInfo.headInfo.HeadObject = CreateRemoteHead();
			headInfo.headInfo.HeadObject.name = "Remote User "+userID;
			headInfo.headProxyInfo.HeadObject = CreateRemoteHead();
			headInfo.headProxyInfo.HeadObject.name = "Remote User "+userID+" proxy";

			headInfo.headProxyInfo.HeadObject.transform.localScale = Vector3.one * 0.02f;

			remoteHeadInfos.Add( userID, headInfo );
		}

		return headInfo;
	}

	//TODO: Step 4 create Handler to receive network message and update current application state.
	/// <summary>
	/// Called when a remote user sends a head transform.
	/// </summary>
	/// <param name="msg"></param>
	void UpdateHeadTransform( NetworkInMessage msg ) {
		// Parse the message
		long userID = msg.ReadInt64();

		Vector3 headPos = CustomMessages.Instance.ReadVector3( msg );

		Quaternion headRot = CustomMessages.Instance.ReadQuaternion( msg );

		HeadInfo headInfo = GetRemoteHeadInfo( userID );

		headInfo.headInfo.HeadObject.transform.localPosition = headPos;
		headInfo.headInfo.HeadObject.transform.localRotation = headRot;

		headInfo.headProxyInfo.HeadObject.transform.localPosition = ProxyUpdater.MiniaturizePosition( headPos );
		headInfo.headProxyInfo.HeadObject.transform.localRotation = headRot;
	}



	//other user will see visility if it is recent voice command




	/// <summary>
	/// Creates a new game object to represent the user's head.
	/// </summary>
	/// <returns></returns>
	GameObject CreateRemoteHead() {
		GameObject newHeadObj = GameObject.CreatePrimitive( PrimitiveType.Cube );
		newHeadObj.transform.parent = this.gameObject.transform;
		newHeadObj.transform.localScale = Vector3.one * 0.2f;

		return newHeadObj;
	}
	
}