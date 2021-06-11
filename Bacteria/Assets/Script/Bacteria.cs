using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bacteria : MonoBehaviour
{
    public float speed;
    public char state;
    public Vector2 movepoint;
    private float time=1;
    private Coroutine div;
    private Rigidbody rigidbody;
    private Vector2 direction;

    // Start is called before the first frame update
    void Start() {
        speed = 0.002f;
        rigidbody = this.GetComponent<Rigidbody>();
        state = 'D';
    }

    // Update is called once per frame
    void Update()
    {
        // 状態変数によって行動を分岐
        // 分裂(Division)
        if(state=='D'){
            if(div==null){
                div = StartCoroutine("Division");
            }
        }else if(div!=null){
            StopCoroutine(div);
            div=null;
        }
        // 選択(Select)
        if(state=='S'){
            this.GetComponent<Image>().color = GetAlphaColor();
        }else{
            if(GetComponent<Image>().color.a!=1){
                GetComponent<Image>().color =new Color(255,255,255,1);
            }
            time=1;
        }
        // 移動(Move)
        if(state=='M'){
            Vector2 bPos = transform.position;
            // 終了判定
            if(Mathf.Abs(movepoint.x-transform.position.x)<0.1f &&Mathf.Abs(movepoint.y-transform.position.y)<0.1f){
                state='D';
            }
            // 移動処理
            if(movepoint.x>transform.position.x){
                transform.position += new Vector3(speed, 0, 0);
            }else if(movepoint.x<transform.position.x){
                transform.position += new Vector3(-speed, 0, 0);
            }

            if(movepoint.y>transform.position.y){
                transform.position += new Vector3(0, speed, 0);
            }else if(movepoint.y<transform.position.y){
                transform.position += new Vector3(0,-speed, 0);
            }
        }
        // 戦闘(Fight)
        if(state=='F'){

        }
    }

    private IEnumerator Division()
    {
        Vector3 local = transform.InverseTransformPoint (transform.position);
        direction=local+new Vector3(-1.02f,1.02f,0);
        RaycastHit2D hit;
        while (state=='D')
        {
            yield return new WaitForSeconds(3.0f);
            for(int i=0;i<816;i++){
                hit = Physics2D.BoxCast(transform.TransformPoint (direction), transform.lossyScale, 0, Vector2.zero);
                if(hit.collider==null){
                    Instantiate ((GameObject)Resources.Load("Bacteria"),transform.TransformPoint (direction), Quaternion.identity,GameObject.Find("Canvas").transform);
                    direction=local+new Vector3(-1.02f,1.02f,0);
                    break;
                }
                if(i<204){
                    direction.x+=0.01f;
                    continue;
                }else if(i<408){
                    direction.y-=0.01f;
                    continue;
                }else if(i<612){
                    direction.x-=0.01f;
                    continue;
                }else{
                    direction.y+=0.01f;
                }
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; //色指定
        Gizmos.DrawCube(transform.TransformPoint (direction), transform.lossyScale); //中心点とサイズ
    }

    Color GetAlphaColor() {
        time += Time.deltaTime * 7.0f;
        Color color =new Color(255,255,255, Mathf.Sin(time) * 0.5f + 0.5f);
        return color;
    }
    /*
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag=="enemy"){
            state='F';
        }else if(collision.gameObject.tag=="stage"){
            state='D';
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(Mathf.Abs(movepoint.x-transform.position.x)<0.1f &&Mathf.Abs(movepoint.y-transform.position.y)<0.1f){
            state='M';
        }else{
            state='D';
        }
    }
    */
}