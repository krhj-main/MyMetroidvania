using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireIncendiary : Spell
{
    [SerializeField] Vector2 area;
    [SerializeField] Vector3 offSet;
    PlayerController player;
    // Start is called before the first frame update
    protected override void Start()
    {
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
    private void FixedUpdate()
    {
        int dir = PlayerController.Instance.pState.lookRight ? 1 : -1;
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z) +
                            new Vector3(offSet.x * dir, offSet.y, offSet.z);
        transform.localScale = new Vector3(transform.localScale.x * dir, transform.localScale.y, transform.localScale.z);
    }
    protected override void OnDrawGizmos()
    {
        if (!debugging) return;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(PlayerController.Instance.transform.position + offSet,area);
        //Gizmos.DrawCube(transform.position + offSet,area);
    }
    protected override void Hit(Vector2 _attackArea, float _radius, float _recoilStrength)
    {
        
    }
}
