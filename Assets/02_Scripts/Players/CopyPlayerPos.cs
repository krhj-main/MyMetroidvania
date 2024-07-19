using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPlayerPos : MonoBehaviour
{
    Transform player;
    Vector3 pos;
    public RectTransform rect, rect2;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance.transform;
        pos = GameObject.Find("Cave_1").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = (Camera.main.WorldToScreenPoint(player.transform.position) + pos/2);
        //gameObject.transform.SetParent(GameObject.Find("Cave_1").transform);
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,player.transform.position,null,out Vector2 _pos);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rect2, player.transform.position, null, out Vector3 _pos);
        transform.position = _pos + rect.transform.position;
    }
}
