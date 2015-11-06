using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class limitCameraRotation
{
	public float xMin = -80, xMax = 80;
}

[Serializable]
public class CameraController {
	
	Transform cameraTransform;
	Transform playerTransform;
	Quaternion characterTargetRot;
	Quaternion cameraTargetRot;
	
	[Range(0f, 5f)] public float verticalSensitivity = 5;
	[Range(0f, 5f)] public float horizontalSensitivity = 5;
	public limitCameraRotation cameraRotation;
	
	public void Setup(Transform camera, Transform player)
	{
		cameraTransform = camera;

		playerTransform = player;

		characterTargetRot = playerTransform.localRotation;
		cameraTargetRot = cameraTransform.localRotation;
	}

	public void rotate ()
	{
		float xRot = horizontalSensitivity * Input.GetAxis ("Mouse X");
		float yRot = verticalSensitivity * Input.GetAxis ("Mouse Y");

		characterTargetRot *= Quaternion.Euler (0f, 0f, xRot);
		cameraTargetRot *= Quaternion.Euler (-yRot, 0f, 0f);

		cameraTargetRot = ClampRotationAroundXAxis (cameraTargetRot);

		playerTransform.localRotation = characterTargetRot;
		cameraTransform.localRotation = cameraTargetRot;
	}
	
	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;
		
		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
		
		angleX = Mathf.Clamp (angleX, cameraRotation.xMin, cameraRotation.xMax);
		
		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
		
		return q;
	}

}
