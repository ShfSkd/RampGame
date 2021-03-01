using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	[SerializeField] float speed;
	[SerializeField] float jumpForce;
	[SerializeField] float verticalVelocity;
	[SerializeField] float clampDelta;
	[SerializeField] LayerMask layerMask;
	[SerializeField] float distanceBetwwenRamps = 0.05f;

	public float sensitivity;
	public int rampCount;
	public GameObject ramp;
	public GameObject rampParent;
	public GameObject holdingRamp;
	public Transform allRampParent;
	public Transform rampPoint;
	public List<GameObject> rampsHolding = new List<GameObject>();

	[HideInInspector]
	public bool canMove, gameOver, finish, jump, playerMoveToPosition;

	public float bounds = 5f;

	private Rigidbody rb;
	private Vector3 lastMousePosition;
	private Vector3 playerReachPosition;
	private CapsuleCollider col;
	private Animator anim;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		col = GetComponent<CapsuleCollider>();
		anim = GetComponentInChildren<Animator>();
	}

	private void Update()
	{
		if (!PlayerManager.isGameStarted) return;

		PlayerPosition();

		transform.position = new Vector3(Mathf.Clamp(transform.position.x, -bounds, bounds), transform.position.y, transform.position.z);

		if (canMove)
			transform.position += FindObjectOfType<CameraMovement>().cameraVelocity;

		if (!canMove && gameOver)
		{
			if (Input.GetMouseButtonDown(0))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				Time.timeScale = 1f;
				Time.fixedDeltaTime = Time.timeScale * 0.02f;

			}
		}
		else if (!canMove && !finish)
		{
			if (Input.GetMouseButtonDown(0))
				canMove = true;
		}
		Movement();
	}



	private void FixedUpdate()
	{
		if (Input.GetMouseButtonDown(0))
			lastMousePosition = Input.mousePosition;

		RaycastHit hit;

		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 0.4f))
		{
			if (!jump)
			{
				jump = false;
				rb.velocity = Vector3.zero;
			}
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.red, hit.distance);
		}
		else
		{
			if (jump)
			{
				if (rampCount > 0)
				{
					Vector3 playerPos = new Vector3(transform.position.x, transform.position.y - distanceBetwwenRamps, transform.position.z);
					GameObject bridgeRamp = Instantiate(ramp, playerPos, transform.rotation);
					bridgeRamp.transform.parent = allRampParent;
					// sound
					rampCount--;
					rampsHolding.Remove(rampParent.gameObject.transform.GetChild(rampCount).gameObject);
					Destroy(rampParent.gameObject.transform.GetChild(rampCount).gameObject);
					rampPoint.transform.position = new Vector3(rampPoint.transform.position.x, rampPoint.transform.position.y - 0.81f, rampPoint.transform.position.z);
				}
				else
				{
					Jump();
					jump = false;
				}
			}
		}
	}

	private void Jump()
	{
		anim.SetTrigger("Jump");
		rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("CollectableRamp"))
		{
			rampCount++;
			// GameManger

			other.gameObject.SetActive(false);
			other.transform.parent.GetComponent<RampGroup>().childDeactivate++;

			// Sound

			GameObject addRampToPlayerStack = Instantiate(holdingRamp, rampPoint.transform.position, rampPoint.transform.rotation);
			addRampToPlayerStack.transform.parent = rampParent.transform;

			rampPoint.transform.position = new Vector3(rampPoint.position.x, rampPoint.transform.position.y + 0.081f, rampPoint.transform.position.z);
			rampsHolding.Add(addRampToPlayerStack);
		}
	}
	private void Movement()
	{

		if (canMove)
		{
			if (Input.GetMouseButton(0))
			{
				Vector3 pos = lastMousePosition - Input.mousePosition;
				pos = new Vector3(pos.x, 0f, pos.y);

				lastMousePosition = Input.mousePosition;

				anim.SetBool("Grounded", true);
				Vector3 moveForce = Vector3.ClampMagnitude(pos, clampDelta);

				moveForce.z = Mathf.Clamp(pos.z, 0, 0);
				rb.AddForce(-moveForce * sensitivity - rb.velocity / speed, ForceMode.VelocityChange);
			}
		}
		else if (!IsGrounded())
		{
			anim.SetBool("Grounded", false);

		}
		rb.velocity.Normalize();
	}
	private bool IsGrounded()
	{
		RaycastHit hit;
		Physics.Raycast(col.bounds.center, Vector3.down, out hit, Mathf.Infinity + 5f, layerMask);
		Color rayColor;

		if (hit.collider != null)
		{
			rayColor = Color.green;
		}
		else
		{
			rayColor = Color.red;
		}

		Debug.DrawRay(col.bounds.center, Vector3.down * (Mathf.Infinity + 5f), rayColor);
		//Debug.Log(hit.collider);
		return hit.collider != null;

	}
	private void PlayerPosition()
	{
		if (playerMoveToPosition)
		{
			if (Vector3.Distance(transform.position, playerReachPosition) <= 0.1f)
			{
				transform.rotation = Quaternion.identity;
				for (int i = 0; i < rampsHolding.Count; i++)
				{
					rampsHolding[i].transform.parent = null;
					rampsHolding[i].GetComponent<Rigidbody>().isKinematic = false;
					rampsHolding[i].GetComponent<Rigidbody>().useGravity = true;
					rampsHolding[i].GetComponent<Rigidbody>().AddForce(new Vector3(0, 0f, transform.position.z * 5));
					Destroy(rampsHolding[i], 2);
				}
			}
		}
	}
}
