using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MonobitEngine;

public class Bacteria : MonobitEngine.MonoBehaviour
{
    public float speed;
    public char state;
    public Vector2 movepoint;
    private float time=1;
    private Coroutine div;
    private Rigidbody2D rb;
    private Vector2 direction;
    private Vector2 bpos;
    MonobitEngine.MonobitView m_MonobitView = null;
    
    void Awake()
    {
        // すべての親オブジェクトに対して MonobitView コンポーネントを検索する
        if (GetComponentInParent<MonobitEngine.MonobitView>() != null)
        {
            m_MonobitView = GetComponentInParent<MonobitEngine.MonobitView>();
        }
        // 親オブジェクトに存在しない場合、すべての子オブジェクトに対して MonobitView コンポーネントを検索する
        else if (GetComponentInChildren<MonobitEngine.MonobitView>() != null)
        {
            m_MonobitView = GetComponentInChildren<MonobitEngine.MonobitView>();
        }
        // 親子オブジェクトに存在しない場合、自身のオブジェクトに対して MonobitView コンポーネントを検索して設定する
        else
        {
            m_MonobitView = GetComponent<MonobitEngine.MonobitView>();
        }
    }

    // Start is called before the first frame update
    void Start() {
        speed = 0.8f;
        rb = this.GetComponent<Rigidbody2D>();
        state = 'D';
        bpos=transform.position;
        transform.parent =GameObject.Find("Canvas").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if( !m_MonobitView.isMine )
        {
            return;
        }
        // 状態変数によって行動を分岐
        // 分裂(Division)
        /*
        if(state=='D'){
            if(div==null){
                div = StartCoroutine("Division");
            }
        }else if(div!=null){
            StopCoroutine(div);
            div=null;
        }
        */
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
            // 移動処理
            if(movepoint.x>transform.position.x){
                rb.velocity = new Vector2(speed,rb.velocity.y);
            }else if(movepoint.x<transform.position.x){
                rb.velocity = new Vector2(-speed,rb.velocity.y);
            }
            if(movepoint.y>transform.position.y){
                rb.velocity = new Vector2(rb.velocity.x,speed);
            }else if(movepoint.y<transform.position.y){
                rb.velocity = new Vector2(rb.velocity.x,-speed);
            }
            // 終了判定
            if(Mathf.Abs(movepoint.x-transform.position.x)<0.01f &&Mathf.Abs(movepoint.y-transform.position.y)<0.01f||(Mathf.Abs(bpos.x-transform.position.x)<0.01f &&Mathf.Abs(bpos.y-transform.position.y)<0.01f&&bpos.x-transform.position.x!=0)){
                state='D';
            }
            bpos=transform.position;
        }else{
            rb.velocity = new Vector2(0, 0);
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