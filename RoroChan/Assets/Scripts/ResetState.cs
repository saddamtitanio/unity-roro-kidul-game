using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetState : MonoBehaviour
{
    public Transform playerStartingPosition;
    public Transform endPosition;

    [HideInInspector]
    public Vector3[] startingPositions;

    public GameObject[] obstacles;

    public Scene scene;

    [HideInInspector]
    public int movementCounter;

    [HideInInspector]
    public Dictionary<string, int> sceneMovementCounters = new Dictionary<string, int>();


    private void Awake()
    {
        scene = SceneManager.GetActiveScene();

        if (!sceneMovementCounters.ContainsKey(scene.name))
        {
            if (scene.name == "Level1")
            {
                sceneMovementCounters.Add(scene.name, 28);
            }
            if (scene.name == "Level2")
            {
                sceneMovementCounters.Add(scene.name, 22);
            }
            if (scene.name == "Level3")
            {
                sceneMovementCounters.Add(scene.name, 20);
            }
            if (scene.name == "Level4")
            {
                sceneMovementCounters.Add(scene.name, 23);
            }
            if (scene.name == "Level5")
            {
                sceneMovementCounters.Add(scene.name, 32);
            }
        }
    }

    void Start()
    {
        scene = SceneManager.GetActiveScene();

        movementCounter = sceneMovementCounters[scene.name];

        Debug.Log(movementCounter);

        startingPositions = new Vector3[obstacles.Length];

        for (int i = 0; i < obstacles.Length; i++)
        {
            startingPositions[i] = obstacles[i].transform.position;
        }
    }

    void Update()
    {
/*        PlayerMovement playerMovementScript = GameObject.Find("Player").GetComponent<PlayerMovement>();
        movementCounter = playerMovementScript.movementCounter;

        if (movementCounter <= 0)
        {
            for (int i = 0; i < obstacles.Length; i++)
            {
                obstacles[i].transform.position = startingPositions[i];
            }
        }
*/    }
}
