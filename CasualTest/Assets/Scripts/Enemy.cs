using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int jumpForce;
    public int moveSpeed;
    public GameObject walkingPlank;
    public GameObject holdingPlank;
    public GameObject planksParent;
    public GameObject crown;
    public Transform plankPoint;
    public Transform rankDecider;
    public Transform allPlankParent;
    public List<Transform> wayPoints;
    public List<GameObject> planksHolding;

    Animator animator;
    Rigidbody rb;
    RaycastHit hit;

    bool run;
    bool jump;
    bool enemyMoveToPosition;
    bool rankDecided;
    int wayPointCounter;
    int plankCount;
    Vector3 enemyReachPosition;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
        crown.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            run = true;
            Run();
        }

        EnemyStartRunning();
        EnemyFinalPosition();
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.down), out hit, 0.35f))
        {
            if (!jump)
            {
                jump = true;
            }
            Debug.DrawRay(this.transform.position, transform.TransformDirection(Vector3.down), Color.red, hit.distance);
        }
        else
        {
            if (jump)
            {
                if (plankCount > 0)
                {
                    Vector3 playerPos = new Vector3(this.transform.position.x, this.transform.position.y - 0.052f, this.transform.position.z);
                    GameObject bridgePlank = Instantiate(walkingPlank, playerPos, this.transform.rotation);
                    bridgePlank.transform.parent = allPlankParent;
                    plankCount--;
                    if(planksHolding[plankCount])
                    {
                        planksHolding.Remove(planksParent.gameObject.transform.GetChild(plankCount).gameObject);
                        Destroy(planksParent.gameObject.transform.GetChild(plankCount).gameObject);
                        plankPoint.transform.position = new Vector3(plankPoint.transform.position.x, plankPoint.transform.position.y - 0.081f, plankPoint.transform.position.z);
                    }
                    
                }
                if(plankCount < 2 )
                {
                    for (int i = 0; i < 10; i++)
                    {
                        plankCount++;

                        GameObject addPlankToPlayerStack = Instantiate(holdingPlank, plankPoint.transform.position, plankPoint.transform.rotation);
                        addPlankToPlayerStack.transform.parent = planksParent.transform;

                        plankPoint.transform.position = new Vector3(plankPoint.transform.position.x, plankPoint.transform.position.y + 0.081f, plankPoint.transform.position.z);
                        planksHolding.Add(addPlankToPlayerStack);

                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("WayPoints"))
        {
            int rand = UnityEngine.Random.Range(0, 10);
            if(rand < 4)
            {
                wayPointCounter += other.gameObject.GetComponent<WayPoints>().plusPoints;
            }
            else
            {
                wayPointCounter += other.gameObject.GetComponent<WayPoints>().forBridge;
            }
        }

        if(other.gameObject.CompareTag("PlayerRank"))
        {
            int rank = other.gameObject.GetComponent<PlayerRank>().rank;
            if(rank == 0 && !GameManager.instance.playerCrownActivity)
            {
                crown.SetActive(true);
            }
            else
            {
                crown.SetActive(false);
            }
            other.gameObject.GetComponent<PlayerRank>().rank++;
        }
            

        if (other.gameObject.CompareTag("CollectablePlank"))
        {
            plankCount++;
            other.gameObject.SetActive(false);
            other.transform.parent.GetComponent<PlankGroup>().childDeActiveCount++;

            GameObject addPlankToPlayerStack = Instantiate(holdingPlank, plankPoint.transform.position, plankPoint.transform.rotation);
            addPlankToPlayerStack.transform.parent = planksParent.transform;

            plankPoint.transform.position = new Vector3(plankPoint.transform.position.x, plankPoint.transform.position.y + 0.07f, plankPoint.transform.position.z);
            planksHolding.Add(addPlankToPlayerStack);
        }

        if (other.gameObject.CompareTag("RankDecider") && !rankDecided)
        {
            run = false;
            rankDecided = true;
            if (GameManager.instance.playerRank != 0)
            {
                int rank = GameManager.instance.playerRank;
                enemyReachPosition = GameManager.instance.playerPositions[rank].transform.position;
                enemyMoveToPosition = true;
            }
            else
            {
                for (int i = 0; i < planksHolding.Count; i++)
                {
                    planksHolding[i].transform.parent = null;
                    planksHolding[i].GetComponent<Rigidbody>().isKinematic = false;
                    planksHolding[i].GetComponent<Rigidbody>().useGravity = true;
                    planksHolding[i].GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, this.transform.position.z * 5));
                    Destroy(planksHolding[i], 2);
                }

                this.transform.rotation = Quaternion.Euler(0, 180, 0);
                Win();
            }
            GameManager.instance.PlayerRankIncrement();
        }
    }
    private void EnemyStartRunning()
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

    public void MoveForward()
    {
        Vector3 playerPosition = this.transform.position;
        Vector3 wayPointPosition;        

        if(wayPointCounter < wayPoints.Count)
        {
            wayPointPosition.x = wayPoints[wayPointCounter].position.x;
            wayPointPosition.y = this.transform.position.y;
            wayPointPosition.z = wayPoints[wayPointCounter].position.z;

            this.transform.position = Vector3.MoveTowards(playerPosition, wayPointPosition, moveSpeed * Time.deltaTime);
            this.transform.LookAt(wayPointPosition);
        }
        else
        {
            wayPointPosition.x = rankDecider.position.x;
            wayPointPosition.y = this.transform.position.y;
            wayPointPosition.z = rankDecider.position.z;

            if(Vector3.Distance(this.transform.position, wayPointPosition) > 0.1f)
            {
                this.transform.position = Vector3.MoveTowards(playerPosition, wayPointPosition, moveSpeed * Time.deltaTime);
                this.transform.LookAt(rankDecider);
            }            
        }
    }

    void EnemyFinalPosition()
    {
        if (enemyMoveToPosition)
        {
            Vector3 playerPosition = this.transform.position;
            Vector3 wayPointPosition;

            wayPointPosition.x = enemyReachPosition.x;
            wayPointPosition.y = this.transform.position.y;
            wayPointPosition.z = enemyReachPosition.z;

            if (Vector3.Distance(playerPosition, enemyReachPosition) > 0.2f)
            {
                this.transform.position = Vector3.MoveTowards(playerPosition, enemyReachPosition, moveSpeed * Time.deltaTime);
                this.transform.LookAt(enemyReachPosition);
            }
            else
            {
                //ThroughPlanks();

                for (int i = 0; i < planksHolding.Count; i++)
                {
                    if (planksHolding[i])
                    {
                        planksHolding[i].transform.parent = null;
                        planksHolding[i].GetComponent<Rigidbody>().isKinematic = false;
                        planksHolding[i].GetComponent<Rigidbody>().useGravity = true;
                        planksHolding[i].GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, this.transform.position.z * 5));
                        Destroy(planksHolding[i], 2);
                    }
                    
                }

                this.transform.rotation = Quaternion.Euler(0, 180, 0);
                Loose();
                enemyMoveToPosition = false;
            }
        }
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
        animator.SetInteger("Character", 4);
    }

    public void Loose()
    {
        animator.SetInteger("Character", 5);
    }
}
