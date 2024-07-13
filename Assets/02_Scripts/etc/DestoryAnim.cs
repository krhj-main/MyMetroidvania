using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 해당 애니메이터의 애니메이션의 길이만큼 재생 후 삭제된다.
        Destroy(gameObject, GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
    }
}
