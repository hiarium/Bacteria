using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + new Vector2(0.6f,0.6f),new Vector2(1,1),1.0f);
        Debug.DrawRay((Vector2)transform.position + new Vector2(0.6f,0.6f),new Vector2(1,1), Color.red);
        if(hit){
            print(hit.collider.gameObject.name);
        }
    }
}
