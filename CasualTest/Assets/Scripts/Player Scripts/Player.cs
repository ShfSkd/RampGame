using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public int jumpForce;
    public int moveSpeed;
    public int cameraRotateSpeed;
    public GameObject plank;
    public GameObject holdingPlank;
    public GameObject planksParent;
    public GameObject playerRankText;
    public GameObject crown;
    public Transform plankPoint;
    public int plankCount;
    public CinemachineVirtualCamera cam;
    public Animator numberAnim;
    public Transform allPlankParent;
    public TextMeshProUGUI playerRankInText;
    public TextMeshProUGUI playerRankSuS;
    public List<GameObject> planksHolding;
    public float distanceBetweenPlanks = 0.05f;
    [SerializeField] RectTransform foreground;

    [Header("Player Step Climb")]
	[SerializeField] GameObject stepRayUpper;
	[SerializeField] GameObject stepRayLower;
	[SerializeField] float stepHeight = 0.3f;
	[SerializeField] float stepSmooth = 0.1f;

	[SerializeField] float rampInstaDistance = 3f;

    [Header("Particle effects")]
    public GameObject popParticle1;
    public GameObject popParticle2;

    [Header("Sounds")]
    public AudioClip plankPickUp;
    public AudioClip plankDown;
    public AudioClip levelFail;
    public AudioClip levelComplete;
    AudioSource audioSource;


    bool run;
    bool jump;
    bool playerController;
    bool playerMoveToPosition;
    bool rankDecided;
    bool pathCreation;
    bool hitOtherNumber;
    float yRot;
    int playerRank;
    Rigidbody rb;
    Animator animator;
    Vector3 playerReachPosition;

    Vector3 mousePosition1;
    Vector3 mousePosition2;

    RaycastHit hit;
    

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();
        rb = this.GetComponent<Rigidbody>();
        playerRankText.SetActive(false);
        crown.SetActive(false);

    }

    private void Update()
    {
        StartRunning();
        PlayersPosition();



        if (Input.GetMouseButtonDown(0) && !playerController)
        {
            run = true;
            playerController = true;
            pathCreation = true;
            playerRankText.SetActive(true);
            Run();
        }

        if(Input.GetMouseButtonDown(0))
        {            
            mousePosition1 = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, 0, 0));
        }
        
        if(Input.GetMouseButton(0))
        {
            mousePosition2 = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, 0, 0));
            Vector3 difference = mousePosition2 - mousePosition1;
            yRot += difference.x * cameraRotateSpeed;
            Quaternion qDifference = Quaternion.Euler(this.transform.rotation.x, yRot - 180, transform.rotation.z);
            this.transform.rotation = qDifference;
            mousePosition1 = mousePosition2;
        }
    }

    private void FixedUpdate()
    {
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 0.4f) && pathCreation)
        {
            if (!jump)
            {
                jump = true;
                rb.velocity = Vector3.zero;
            }
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.red, hit.distance);
        }
        else
        {
            if(jump)
            {
                if(plankCount > 0)
                {
                    Vector3 playerPos = new Vector3(transform.position.x,Mathf.Abs( transform.position.y - distanceBetweenPlanks), transform.position.z);
                    StepClimb();
                    GameObject bridgePlank = Instantiate(plank, playerPos, transform.rotation);
                   // bridgePlank.transform.Rotate(new Vector3(0, 180, 0));
                    bridgePlank.transform.parent = allPlankParent;
                    audioSource.PlayOneShot(plankDown);
                    plankCount--;
                    RemoveRampAmount();
                    planksHolding.Remove(planksParent.gameObject.transform.GetChild(plankCount).gameObject);
                    Destroy(planksParent.gameObject.transform.GetChild(plankCount).gameObject);
                    plankPoint.transform.position = new Vector3(plankPoint.transform.position.x, plankPoint.transform.position.y - 0.81f, plankPoint.transform.position.z);
                }
                else
                {
                    Jump();
                    jump = false;
                }
            }
        }

    }

	

	private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CollectablePlank"))
        {
            AddRampAmount();

            plankCount++;
            
            GameManager.instance.AddPieces(plankCount);
            other.gameObject.SetActive(false);
            other.transform.parent.GetComponent<PlankGroup>().childDeActiveCount++;

            audioSource.PlayOneShot(plankPickUp);

            GameObject addPlankToPlayerStack = Instantiate(holdingPlank, plankPoint.transform.position, plankPoint.transform.rotation);
            addPlankToPlayerStack.transform.parent = planksParent.transform;

            plankPoint.transform.position = new Vector3(plankPoint.transform.position.x, plankPoint.transform.position.y + 0.081f, plankPoint.transform.position.z);
            planksHolding.Add(addPlankToPlayerStack);
        }

        if(other.gameObject.CompareTag("PlayerRank"))
        {
            int rank = other.gameObject.GetComponent<PlayerRank>().rank;
            rank++;
            playerRankInText.text = rank.ToString();

            if(rank == 1)
            {
                playerRankSuS.text = "st";
                crown.SetActive(true);
                GameManager.instance.PlayerCrownActive();
            }
            else if(rank == 2)
            {
                playerRankSuS.text = "nd";
                crown.SetActive(false);
                GameManager.instance.PlayerCrownNotActive();
            }
            else if(rank == 3)
            {
                playerRankSuS.text = "rd";
                crown.SetActive(false);
                GameManager.instance.PlayerCrownNotActive();
            }
            else
            {
                playerRankSuS.text = "th";
                crown.SetActive(false);
                GameManager.instance.PlayerCrownNotActive();
            }

        }

        if(other.gameObject.CompareTag("RankDecider") && !rankDecided)
        {
            if (GameManager.instance.playerRank != 0)
            {
                playerController = true;
                run = false;
                int rank = GameManager.instance.playerRank;
                playerReachPosition = GameManager.instance.playerPositions[rank].transform.position;
                playerMoveToPosition = true;
                cam.Follow = null;
            }
            else
            {
                numberAnim.SetTrigger("Count");
            }
            playerRank = GameManager.instance.playerRank;
            GameManager.instance.PlayerRankIncrement();
            rankDecided = true; 
        }

        if(other.gameObject.CompareTag("15"))
        {
            pathCreation = false;
            run = false;
            jump = false;
            playerController = true;
            hitOtherNumber = false;
            playerReachPosition = other.transform.position;
            playerMoveToPosition = true;
        }

        if(other.gameObject.CompareTag("OtherNumbers"))
        {
            
                playerReachPosition = other.transform.position;
                hitOtherNumber = true;
        }

        if(other.gameObject.CompareTag("Water"))
        {
            if (rankDecided)
            {
                pathCreation = false;
                run = false;
                playerController = true;
                rb.useGravity = false;
                rb.isKinematic = true;
                this.GetComponent<Collider>().enabled = false;
                playerMoveToPosition = true;
            }
            else
            {
                pathCreation = false;
                run = false;
                cam.m_Follow = null;
                cam.LookAt = null;
                LoosePanelOn();
            }
        }
    }

	public void AddRampAmount()
	{
		switch (planksHolding.Count)
		{
            case 0:
                foreground.localScale = new Vector3(0.33f, 1, 1);
                break;

            case 1:
                foreground.localScale = new Vector3(0.66f, 1, 1);
                break;
            case 2:
                foreground.localScale= new Vector3(1f, 1, 1);
                break;
            default:
                foreground.localScale = new Vector3(0f, 1, 1);
                break;
		}
	}
    private void RemoveRampAmount()
    {
		if (plankCount > 0)
		{
            foreground.localScale = new Vector3(Mathf.Min(plankCount - 1) - 0.33f, 1, 1);
		}
		else
		{
            foreground.localScale = new Vector3(0f, 1, 1);
        }
    }

    public void StartRunning()
    {
        if (run)
        {
            MoveForward();

            if (jump)
            {
                if (plankCount == 0)
                {
                    Run();
                }
                else
                {
                    RunWithPlank();
                }
            }
        }
    }

    void PlayersPosition()
    {
        if (playerMoveToPosition)
        {
            if (Vector3.Distance(this.transform.position, playerReachPosition) > 0.1f)
            {
                Vector3 playerRankPosition;

                playerRankPosition.x = playerReachPosition.x;
                playerRankPosition.z = playerReachPosition.z;

                if(!hitOtherNumber)
                {
                    playerRankPosition.y = this.transform.position.y;
                }
                else
                {
                    playerRankPosition.y = playerReachPosition.y;
                }
                this.transform.position = Vector3.MoveTowards(this.transform.position, playerRankPosition, moveSpeed * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, playerReachPosition) <= 0.1f)
            {
                transform.rotation = Quaternion.identity;
                for (int i = 0; i < planksHolding.Count; i++)
                {
                    planksHolding[i].transform.parent = null;
                    planksHolding[i].GetComponent<Rigidbody>().isKinematic = false;
                    planksHolding[i].GetComponent<Rigidbody>().useGravity = true;
                    planksHolding[i].GetComponent<Rigidbody>().AddForce(new Vector3(0, 0f, transform.position.z * 5));
                    Destroy(planksHolding[i], 2);
                }
                cam.Follow = null;

                if(playerRank == 0)
                {
                    Win();
                }
                else
                {
                    Loose();
                    LoosePanelOn();
                }
                playerMoveToPosition = false;
            }
        }
    }

    public void MoveForward()
    {
        transform.Translate(0, 0, -1 * moveSpeed * Time.deltaTime, Space.Self);
    }   
    

    public void Run()
    {
        animator.SetInteger("Character", 1);
    }

    public void RunWithPlank()
    {
        animator.SetInteger("Character", 2);
    }

    public void Jump()
    {
        animator.SetInteger("Character", 3);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void Win()
    {
        audioSource.PlayOneShot(levelComplete);
        popParticle1.SetActive(true);
        popParticle2.SetActive(true);
        animator.SetInteger("Character", 4);
        GameManager.instance.LevelCompletePanelOn();
    }

    public void Loose()
    {
        animator.SetInteger("Character", 5);
    }

    void LoosePanelOn()
    {
        audioSource.PlayOneShot(levelFail);
        GameManager.instance.LevelFailPanelOn();
    }
	void StepClimb()
	{
		RaycastHit hitLower;

		if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
		{
			RaycastHit hitUpper;
			if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
			{
				rb.position -= new Vector3(0f, -stepSmooth, 0f);
			}
		}
	}
}



/*using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public int jumpForce;
    public int moveSpeed;
    public int cameraRotateSpeed;
    public GameObject plank;
    public GameObject holdingPlank;
    public GameObject planksParent;
    public GameObject playerRankText;
    public GameObject crown;
    public Transform plankPoint;
    public int plankCount;
    public CinemachineVirtualCamera cam;
    public Animator numberAnim;
    public Transform allPlankParent;
    public TextMeshProUGUI playerRankInText;
    public TextMeshProUGUI playerRankSuS;
    public List<GameObject> planksHolding;

    [Header("Particle effects")]
    public GameObject popParticle1;
    public GameObject popParticle2;

    [Header("Sounds")]
    public AudioClip plankPickUp;
    public AudioClip plankDown;
    public AudioClip levelFail;
    public AudioClip levelComplete;
    AudioSource audioSource;


    bool run;
    bool jump;
    bool playerController;
    bool playerMoveToPosition;
    bool rankDecided;
    bool pathCreation;
    bool hitOtherNumber;
    float yRot;
    int playerRank;
    Rigidbody rb;
    Animator animator;
    Vector3 playerReachPosition;

    Vector3 mousePosition1;
    Vector3 mousePosition2;

    RaycastHit hit;


    private void Start()
    {
        animator = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();
        rb = this.GetComponent<Rigidbody>();
        playerRankText.SetActive(false);
        crown.SetActive(false);
    }

    private void Update()
    {
        StartRunning();
        PlayersPosition();



        if (Input.GetMouseButtonDown(0) && !playerController)
        {
            run = true;
            playerController = true;
            pathCreation = true;
            playerRankText.SetActive(true);
            Run();
        }

        if (Input.GetMouseButtonDown(0))
        {
            mousePosition1 = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, 0, 0));
        }

        if (Input.GetMouseButton(0))
        {
            mousePosition2 = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, 0, 0));
            Vector3 difference = mousePosition2 - mousePosition1;
            yRot += difference.x * cameraRotateSpeed;
            Quaternion qDifference = Quaternion.Euler(this.transform.rotation.x, yRot - 180, this.transform.rotation.z);
            this.transform.rotation = qDifference;
            mousePosition1 = mousePosition2;
        }
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.down), out hit, 0.4f) && pathCreation)
        {
            if (!jump)
            {
                jump = true;
                rb.velocity = Vector3.zero;
            }
            Debug.DrawRay(this.transform.position, transform.TransformDirection(Vector3.down), Color.red, hit.distance);
        }
        else
        {
            if (jump)
            {
                if (plankCount > 0)
                {
                    Vector3 playerPos = new Vector3(this.transform.position.x, this.transform.position.y - 0.05f, this.transform.position.z);
                    GameObject bridgePlank = Instantiate(plank, playerPos, this.transform.rotation);
                    bridgePlank.transform.parent = allPlankParent;
                    audioSource.PlayOneShot(plankDown);
                    plankCount--;
                    planksHolding.Remove(planksParent.gameObject.transform.GetChild(plankCount).gameObject);
                    Destroy(planksParent.gameObject.transform.GetChild(plankCount).gameObject);
                    plankPoint.transform.position = new Vector3(plankPoint.transform.position.x, plankPoint.transform.position.y - 0.081f, plankPoint.transform.position.z);
                }
                else
                {
                    Jump();
                    jump = false;
                }
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CollectablePlank"))
        {
            plankCount++;
            other.gameObject.SetActive(false);
            other.transform.parent.GetComponent<PlankGroup>().childDeActiveCount++;

            audioSource.PlayOneShot(plankPickUp);

            GameObject addPlankToPlayerStack = Instantiate(holdingPlank, plankPoint.transform.position, plankPoint.transform.rotation);
            addPlankToPlayerStack.transform.parent = planksParent.transform;

            plankPoint.transform.position = new Vector3(plankPoint.transform.position.x, plankPoint.transform.position.y + 0.081f, plankPoint.transform.position.z);
            planksHolding.Add(addPlankToPlayerStack);
        }

        if (other.gameObject.CompareTag("PlayerRank"))
        {
            int rank = other.gameObject.GetComponent<PlayerRank>().rank;
            rank++;
            playerRankInText.text = rank.ToString();

            if (rank == 1)
            {
                playerRankSuS.text = "st";
                crown.SetActive(true);
                GameManager.instance.PlayerCrownActive();
            }
            else if (rank == 2)
            {
                playerRankSuS.text = "nd";
                crown.SetActive(false);
                GameManager.instance.PlayerCrownNotActive();
            }
            else if (rank == 3)
            {
                playerRankSuS.text = "rd";
                crown.SetActive(false);
                GameManager.instance.PlayerCrownNotActive();
            }
            else
            {
                playerRankSuS.text = "th";
                crown.SetActive(false);
                GameManager.instance.PlayerCrownNotActive();
            }

        }

        if (other.gameObject.CompareTag("RankDecider") && !rankDecided)
        {
            if (GameManager.instance.playerRank != 0)
            {
                playerController = true;
                run = false;
                int rank = GameManager.instance.playerRank;
                playerReachPosition = GameManager.instance.playerPositions[rank].transform.position;
                playerMoveToPosition = true;
                cam.Follow = null;
            }
            else
            {
                numberAnim.SetTrigger("Count");
            }
            playerRank = GameManager.instance.playerRank;
            GameManager.instance.PlayerRankIncrement();
            rankDecided = true;
        }

        if (other.gameObject.CompareTag("15"))
        {
            pathCreation = false;
            run = false;
            jump = false;
            playerController = true;
            hitOtherNumber = false;
            playerReachPosition = other.transform.position;
            playerMoveToPosition = true;
        }

        if (other.gameObject.CompareTag("OtherNumbers"))
        {

            playerReachPosition = other.transform.position;
            hitOtherNumber = true;
        }

        if (other.gameObject.CompareTag("Water"))
        {
            if (rankDecided)
            {
                pathCreation = false;
                run = false;
                playerController = true;
                rb.useGravity = false;
                rb.isKinematic = true;
                this.GetComponent<Collider>().enabled = false;
                playerMoveToPosition = true;
            }
            else
            {
                pathCreation = false;
                run = false;
                cam.m_Follow = null;
                cam.LookAt = null;
                LoosePanelOn();
            }
        }
    }

    public void StartRunning()
    {
        if (run)
        {
            MoveForward();

            if (jump)
            {
                if (plankCount == 0)
                {
                    Run();
                }
                else
                {
                    RunWithPlank();
                }
            }
        }
    }

    void PlayersPosition()
    {
        if (playerMoveToPosition)
        {
            if (Vector3.Distance(this.transform.position, playerReachPosition) > 0.1f)
            {
                Vector3 playerRankPosition;

                playerRankPosition.x = playerReachPosition.x;
                playerRankPosition.z = playerReachPosition.z;

                if (!hitOtherNumber)
                {
                    playerRankPosition.y = this.transform.position.y;
                }
                else
                {
                    playerRankPosition.y = playerReachPosition.y;
                }
                this.transform.position = Vector3.MoveTowards(this.transform.position, playerRankPosition, moveSpeed * Time.deltaTime);
            }

            if (Vector3.Distance(this.transform.position, playerReachPosition) <= 0.1f)
            {
                this.transform.rotation = Quaternion.identity;
                for (int i = 0; i < planksHolding.Count; i++)
                {
                    planksHolding[i].transform.parent = null;
                    planksHolding[i].GetComponent<Rigidbody>().isKinematic = false;
                    planksHolding[i].GetComponent<Rigidbody>().useGravity = true;
                    planksHolding[i].GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, this.transform.position.z * 5));
                    Destroy(planksHolding[i], 2);
                }
                cam.Follow = null;

                if (playerRank == 0)
                {
                    Win();
                }
                else
                {
                    Loose();
                    LoosePanelOn();
                }
                playerMoveToPosition = false;
            }
        }
    }

    public void MoveForward()
    {
        this.transform.Translate(0, 0, -1 * moveSpeed * Time.deltaTime, Space.Self);
    }


    public void Run()
    {
        animator.SetInteger("Character", 1);
    }

    public void RunWithPlank()
    {
        animator.SetInteger("Character", 2);
    }

    public void Jump()
    {
        animator.SetInteger("Character", 3);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void Win()
    {
        audioSource.PlayOneShot(levelComplete);
        popParticle1.SetActive(true);
        popParticle2.SetActive(true);
        animator.SetInteger("Character", 4);
        GameManager.instance.LevelCompletePanelOn();
    }

    public void Loose()
    {
        animator.SetInteger("Character", 5);
    }

    void LoosePanelOn()
    {
        audioSource.PlayOneShot(levelFail);
        GameManager.instance.LevelFailPanelOn();
    }
}
*/