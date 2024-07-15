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
        spellOption.spell_XTransform = GameObject.Find("XSpell").transform;
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
    private void FixedUpdate()
    {
        transform.position = spellOption.spell_XTransform.position;
        if (PlayerController.Instance.pState.lookRight)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else //if(!PlayerController.Instance.pState.lookRight)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }
    protected override void OnDrawGizmos()
    {
        if (!debugging) return;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(spellOption.spell_XTransform.position,area);
        //Gizmos.DrawCube(transform.position + offSet,area);
    }
    protected override void Hit(Vector2 _attackArea, float _radius, float _recoilStrength)
    {
        
    }
}
