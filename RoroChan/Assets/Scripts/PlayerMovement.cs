using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private bool isMoving;
    private Vector3 origPos, targetPos;
    private float timeToMove = 0.15f;

    private SpriteRenderer spriteRenderer;

    public TMP_Text movementCounterText;

    [HideInInspector]
    public int movementCounter;

    [HideInInspector]
    public bool isDead;

    [SerializeField]
    private Transform startingPoint;

    private GameObject[] ObjToPush;
    private GameObject[] traps;

    bool isBlock = false;


    // Start is called before the first frame update
    void Start()
    {
        movementCounter = int.Parse(movementCounterText.text);
        spriteRenderer = GetComponent<SpriteRenderer>();

        transform.position = startingPoint.transform.position;

        ObjToPush = GameObject.FindGameObjectsWithTag("ObjToPush");
        traps = GameObject.FindGameObjectsWithTag("Trap");
    }

    // Update is called once per frame
    void Update()
    {

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.up));
            movementCounter--;
        }

        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.down));
            movementCounter--;
        }

        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !isMoving)
        {
            spriteRenderer.flipX = true;
            StartCoroutine(MovePlayer(Vector3.left));
            movementCounter--;
        }

        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !isMoving)
        {
            spriteRenderer.flipX = false;
            StartCoroutine(MovePlayer(Vector3.right));
            movementCounter--;
        }

/*        if (movementCounter < 0)
        {
            transform.position = startingPoint.transform.position;
            spriteRenderer.flipX = true;
            movementCounter = 23;
        }*/
    }

    private IEnumerator MovePlayer(Vector3 direction)
    {
        isMoving = true;

        float elapsedTime = 0;

        origPos = transform.position;
        targetPos = origPos + direction;

        GameObject objectDetected = checkObjCanMove(targetPos);

        isBlock = false;

        Debug.Log(isBlock);

        if (objectDetected != null)
        {
            Vector3 obstacleOrigPos = objectDetected.transform.position;
            Vector3 targetObstaclePos = obstacleOrigPos + direction;

            while (elapsedTime < timeToMove)
            {
                objectDetected.transform.position = Vector3.Lerp(obstacleOrigPos, targetObstaclePos, (elapsedTime / 0.1f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            objectDetected.transform.position = targetObstaclePos;
        }
        else
        {
            checkTraps(targetPos);
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
            movementCounter = 23;
            transform.position = startingPoint.transform.position;
            spriteRenderer.flipX = true;
        }

        isMoving = false;
    }

    public bool isBlocked(Vector3 targetPos)
    {
        foreach (var obj in ObjToPush)
        {
            if (obj.transform.position == targetPos)
            {
                isBlock = true;
                return true;
            }
        }

        isBlock = false;
        return false;
    }

    public GameObject checkObjCanMove(Vector3 targetPos)
    {
        foreach (var obj in ObjToPush)
        {
            if (obj.transform.position == targetPos)
            {
                return obj;
            }
        }
        return null;
    }

    public void checkTraps(Vector3 targetPos)
    {
        foreach (var trap in traps)
        {
            if (trap.transform.position == targetPos)
            {
                movementCounter--;
                break;
            }
        }
    }

    public bool Blocked(Vector3 position, Vector2 direction)
    {
        Vector2 newpos = new Vector2(position.x, position.y) + direction;

        foreach (var objToPush in ObjToPush)
        {
            if (objToPush.transform.position.x == newpos.x && objToPush.transform.position.y == newpos.y)
            {
                Push objPush = objToPush.GetComponent<Push>();

                if (objToPush && objPush.Move(direction))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }
}
