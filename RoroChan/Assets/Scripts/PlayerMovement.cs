using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    private bool isMoving, isSliding;
    private Vector3 origPos, targetPos;
    private float timeToMove = 0.15f;
    private float slidingTime = 0.3f;

    private SpriteRenderer spriteRenderer;

    [HideInInspector]
    public int movementCounter;

    [SerializeField]
    private Transform startingPoint;

    private GameObject[] ObjToPush;

    private Vector3 moveToPosition;

    public Tilemap[] obstacles;
    public Tilemap waterTilemap;
    public Tilemap trapTilemap;
    public Tilemap destinationTilemap;

    public Animator anim;

    Vector3 moveDirection;

    private bool isTrapped = false;



    // Start is called before the first frame update
    void Start()
    {
        ResetState initialCounter = FindObjectOfType<ResetState>();
        Scene scene = initialCounter.scene;

        movementCounter = initialCounter.sceneMovementCounters[scene.name];

        spriteRenderer = GetComponent<SpriteRenderer>();

        transform.position = startingPoint.transform.position;

        ObjToPush = GameObject.FindGameObjectsWithTag("ObjToPush");

    }

    // Update is called once per frame
    void Update()
    {
        if (isTrapped)
        {
            if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) || (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) ||
                (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) || (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
            {
                isTrapped = false;
                isSliding = false;

                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                {
                    moveDirection = Vector3.up;
                }
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    moveDirection = Vector3.down;
                }
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    spriteRenderer.flipX = false;
                    moveDirection = Vector3.right;
                }
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    spriteRenderer.flipX = true;
                    moveDirection = Vector3.left;
                }

                movementCounter--;
                anim.SetBool("isMoving", true);
                StartCoroutine(StopMovingAnimation());

                StartCoroutine(SlidePlayer(moveDirection));
            }
        }

        if (isInWater() && !isSliding && !isTrapped)
        {
            if (!isTrap(moveDirection))
            {
                if (checkImmovableObj(transform.position, moveDirection))
                {
                    StartCoroutine(SlidePlayer(moveDirection));
                }
            }

        }

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && !isMoving && !isSliding)
        {
            moveDirection = Vector3.up;

            if (checkImmovableObj(transform.position, moveDirection))
            {
                StartCoroutine(MovePlayer(moveDirection));
                movementCounter--;
            }
        }

        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && !isMoving && !isSliding)
        {
            moveDirection = Vector3.down;

            if (checkImmovableObj(transform.position, Vector3.down))
            {
                StartCoroutine(MovePlayer(Vector3.down));
                movementCounter--;
            }
        }

        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !isMoving && !isSliding)
        {
            moveDirection = Vector3.left;

            if (checkImmovableObj(transform.position, Vector3.left))
            {
                spriteRenderer.flipX = true;
                StartCoroutine(MovePlayer(Vector3.left));
                movementCounter--;
            }
        }

        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !isMoving && !isSliding)
        {
            moveDirection = Vector3.right;

            if (checkImmovableObj(transform.position, Vector3.right))
            {
                spriteRenderer.flipX = false;
                StartCoroutine(MovePlayer(Vector3.right));
                movementCounter--;
            }
        }

        isPlayerDead();
        isDestinationReached();
    }

    private IEnumerator StopMovingAnimation()
    {
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("isMoving", false);
    }

    private IEnumerator SlidePlayer(Vector3 direction)
    {
        isSliding = true;

        float elapsedTime = 0f;

        checkTraps(direction);

        Vector3 origPos = transform.position;
        Vector3 targetPos = origPos + direction;

        while (elapsedTime < slidingTime)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / slidingTime));
            elapsedTime += Time.deltaTime;

            if (isTrap(direction))
            {
                transform.position = targetPos + direction;

                isTrapped = true;

                isPlayerDead();

                yield break;
            }

            yield return null;

        }

        transform.position = targetPos;

        if (movementCounter < 0)
        {
            StopCoroutine(SlidePlayer(direction));
        }
        isPlayerDead();

        isSliding = false;
    }



    private IEnumerator MovePlayer(Vector3 direction)
    {
        isMoving = true;

        float elapsedTime = 0;

        origPos = transform.position;
        targetPos = origPos + direction;

        GameObject objectDetected = pushableObject(targetPos, direction);

        if (objectDetected != null)
        {
            anim.SetBool("isPush", true);

            if (!checkAdjacentObj(objectDetected, direction) && checkImmovableObj(objectDetected.transform.position, direction))
            {
                Vector3 obstacleOrigPos = objectDetected.transform.position;
                Vector3 targetObstaclePos = obstacleOrigPos + direction;

                while (elapsedTime < timeToMove)
                {
                    objectDetected.transform.position = Vector3.Lerp(obstacleOrigPos, targetObstaclePos, (elapsedTime / 0.2f));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                objectDetected.transform.position = targetObstaclePos;
            }
            isPlayerDead();
            checkTraps(Vector3.zero);
        }
        else
        {
            checkTraps(direction);

            anim.SetBool("isMoving", true);

            while (elapsedTime < timeToMove)
            {
                transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / timeToMove));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPos;
        }

        if (movementCounter < 0)
        {
            StopCoroutine(MovePlayer(direction));
        }

        isPlayerDead();

        yield return new WaitForSeconds(0.15f);

        anim.SetBool("isPush", false);
        anim.SetBool("isMoving", false);

        isMoving = false;
    }

    void isPlayerDead()
    {
        ResetState initialCounter = FindObjectOfType<ResetState>();
        Scene scene = initialCounter.scene;

        if (movementCounter < 0)
        {
            movementCounter = initialCounter.sceneMovementCounters[scene.name];
            anim.SetBool("isMoving", false);
            transform.position = startingPoint.transform.position;
            spriteRenderer.flipX = true;
        }
    }

    private GameObject pushableObject(Vector3 targetPos, Vector3 direction)
    {
        foreach (var obj in ObjToPush)
        {
            if (Vector2.Distance(obj.transform.position, targetPos) < 0.5f)
            {
                isPlayerDead();
                return obj;
            }
        }
        return null;
    }

    bool checkAdjacentObj(GameObject pushedObject, Vector3 direction)
    {
        foreach (var obj in ObjToPush)
        {
            if (obj != pushedObject)
            {
                if (Vector2.Distance(pushedObject.transform.position + direction, obj.transform.position) < 0.5f)
                {
                    isPlayerDead();
                    return true;
                }
            }
        }
        return false;
    }


    bool checkImmovableObj(Vector3 position, Vector3 direction)
    {
        moveToPosition = position + direction;

        foreach (Tilemap obstacle in obstacles)
        {
            Vector3Int obstacleMap = obstacle.WorldToCell(moveToPosition - new Vector3(0, 0.5f, 0));

            if (obstacle.GetTile(obstacleMap) != null)
            {
                isPlayerDead();
                return false;
            }
        }
        return true;
    }

    private bool isInWater()
    {
        Vector3Int playerCellPosition = trapTilemap.WorldToCell(transform.position);

        if (waterTilemap.HasTile(playerCellPosition))
        {
            isPlayerDead();
            return true;
        }
        return false;

    }

    private bool isTrap(Vector3 direction)
    {
        Vector3Int playerCellPosition = trapTilemap.WorldToCell(transform.position);

        if (trapTilemap.HasTile(playerCellPosition))
        {
            isPlayerDead();
            movementCounter--;
            return true;
        }
        return false;
    }

    private void checkTraps(Vector3 direction)
    {
        Vector3Int playerCellPosition = trapTilemap.WorldToCell(transform.position + direction);

        if (trapTilemap.HasTile(playerCellPosition))
        {
            isPlayerDead();
            DamageEffect damage = GetComponent<DamageEffect>();
            damage.Flash();
            movementCounter--;
        }
    }

    private bool isDestinationReached()
    {
        Vector3Int playerCellPosition = destinationTilemap.WorldToCell(transform.position);

        if (destinationTilemap.HasTile(playerCellPosition))
        {
            return true;
        }
        return false;
    }
}
