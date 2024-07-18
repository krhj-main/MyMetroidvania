using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] Transform upFloor, downFloor;
    [SerializeField] float elevatorSpeed;
    PlayerController player;

    bool activating=false;
    float dist;
    private void Awake()
    {
        player = PlayerController.Instance;
    }


    private void Update()
    {
        if (activating)
        {
            if(dist > 0.5f)
            StartCoroutine(ElevatorMove(upFloor));

            else
            StartCoroutine(ElevatorMove(downFloor));
        }
    }


    void UpdateElevator()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            activating = true;
            dist = Vector2.Distance(transform.position, upFloor.position);
            player.gameObject.transform.SetParent(transform);
        }
    }
    IEnumerator ElevatorMove(Transform _movePos)
    {
        Vector2 _verticalDir = (transform.position - _movePos.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, _movePos.position, elevatorSpeed * Time.deltaTime);
        if (Mathf.Abs(_verticalDir.y) < 0.1f)
        {
            activating = false;
            player.gameObject.transform.SetParent(null);
            DontDestroyOnLoad(player.gameObject);
        }
        yield return null;
    }

    private void OnTriggerStay2D(Collider2D _col)
    {
        if (_col.tag.Contains("Player"))
        {
            UpdateElevator();
        }
    }
}