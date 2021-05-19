using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bacteria : MonoBehaviour
{
    public float xspeed;
    public float yspeed;
    private Rigidbody2D rb;
    private GameObject gm;
    private GameObject copy;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gm = GameObject.Find("GameManager");
        copy = (GameObject)Resources.Load("Bacteria");
        StartCoroutine("Division");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            xspeed = -1;
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            xspeed = 1;
        }
        else
        {
            xspeed = 0;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            yspeed = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            yspeed = -1;
        }
        else
        {
            yspeed = 0;
        }
        
        rb.velocity = new Vector2(xspeed, yspeed);
        
    }

    private IEnumerator Division()
    {
        Vector3 local = transform.InverseTransformPoint (transform.position);
        Vector3 direction=transform.TransformPoint (local+new Vector3(1.02f,0,0));
        RaycastHit2D hit;
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            local = transform.InverseTransformPoint (transform.position);
            direction=transform.TransformPoint (local+new Vector3(1.02f,0,0));
            hit = Physics2D.BoxCast(direction, transform.lossyScale, 0, Vector2.zero);
            Debug.Log(hit.collider);
            if(hit.collider==null){
                Instantiate ((GameObject)Resources.Load("Bacteria"), direction, Quaternion.identity,GameObject.Find("Canvas").transform);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Vector3 local = transform.InverseTransformPoint (transform.position);
        Gizmos.color = Color.red; //色指定
        Gizmos.DrawCube(transform.TransformPoint (local+new Vector3(1.02f,0,0)), transform.lossyScale); //中心点とサイズ
    }
    
}
