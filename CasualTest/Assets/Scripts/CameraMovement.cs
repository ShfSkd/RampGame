using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	public float cameraSpeed = 7f;
	public Vector3 cameraVelocity;

	[SerializeField] Transform player;
	public Vector3 offset;

	private void Update()
	{
		//if (FindObjectOfType<PlayerController>()._dead) return;

	/*	if (FindObjectOfType<PlayerController>().canMove)
			transform.position += (Vector3.forward * cameraSpeed) * Time.deltaTime;*/

		cameraVelocity = (Vector3.forward * cameraSpeed) * Time.deltaTime * 100f;

		transform.position = player.transform.position + offset;
	}
}
