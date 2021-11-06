using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MonobitEngine;

public class Bacteria : MonobitEngine.MonoBehaviour
{
    public char state;
    public Vector2 movepoint;
    public int hp;
    private float time=1;
    private Coroutine div;
    private Coroutine fig;
    private Vector2 direction;
    private Bacteria enemy;
    private GameManager game_maneger;
    private RaycastHit2D hit;
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
        transform.parent =GameObject.Find("Canvas").transform;
        game_maneger = GameObject.Find("GameManager").GetComponent<GameManager>();
        state = 'D';
        hp=100;
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
        if(state=='D'){
            if(div==null){
                div = StartCoroutine("Division");
            }
        }else if(div!=null){
            //コルーチンスタート済みでD以外になった時
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
            Vector2 direction=new Vector2(0,0);
            float speed=1.0f * Time.deltaTime;
            // 移動方向判断
            if(movepoint.x>0.1f){
                direction.x=0.27f;;
            }else if(movepoint.x<-0.1f){
                direction.x=-0.27f;;
            }else{
                direction.x=0;
            }
            if(movepoint.y>0.1f){
                direction.y=0.27f;;
            }else if(movepoint.y<-0.1f){
                direction.y=-0.27f;;
            }else{
                direction.y=0;
            }
            // 移動方向にオブジェクトがあったら移動しない
            hit = Physics2D.Raycast((Vector2)transform.InverseTransformPoint(transform.position),direction);
            Debug.DrawRay ((Vector2)transform.InverseTransformPoint(transform.position),direction, Color.red, 0.01f, false);
            try{
                if(hit.collider.gameObject.tag=="Player"){
                    speed=0.0f;
                }
            }catch{}
            // 移動処理
            if(movepoint.x>0.1f){
                transform.position+=new Vector3(speed,0,0);
                movepoint.x-=speed;
            }else if(movepoint.x<-0.1f){
                transform.position-=new Vector3(speed,0,0);
                movepoint.x+=speed;
            }
            if(movepoint.y>0.1f){
                transform.position+=new Vector3(0,speed,0);
                movepoint.y-=speed;
            }else if(movepoint.y<-0.1f){
                transform.position-=new Vector3(0,speed,0);
                movepoint.y+=speed;
            }

            if(Mathf.Abs(movepoint.x)<0.1f &&Mathf.Abs(movepoint.y)<0.1f){
                state='D';
            }
        }
        // 戦闘(Fight)
        if(state=='F'){
            if(fig==null){
                fig = StartCoroutine("Fight");
            }
        }else if(fig!=null){
            //コルーチンスタート済みでF以外になった時
            StopCoroutine(fig);
            fig=null;
        }
        if(hp<=0){
            MonobitNetwork.Destroy(m_MonobitView);
            game_maneger.counter-=1;
        }
    }

    private IEnumerator Division()
    {
        Vector3 local = transform.InverseTransformPoint (transform.position);
        direction=local+new Vector3(-1.02f,1.02f,0);
        while (state=='D')
        {
            yield return new WaitForSeconds(3.0f);
            if(game_maneger.counter<50){
                for(int i=0;i<816;i++){
                    hit = Physics2D.BoxCast(transform.TransformPoint (direction), transform.lossyScale, 0, Vector2.zero);
                    if(hit.collider==null){
                        if(MonobitNetwork.isHost){
                            MonobitNetwork.Instantiate ("Bacteria",transform.TransformPoint (direction), Quaternion.identity,0).tag = "Player";
                        }else{
                            MonobitNetwork.Instantiate ("Bacteria2",transform.TransformPoint (direction), Quaternion.identity,0).tag = "Player";
                        }
                        direction=local+new Vector3(-1.02f,1.02f,0);
                        game_maneger.counter+=1;
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
            }else{
                continue;
            }
        }
    }

    private IEnumerator Fight()
    {
        while(state=='F'){
            yield return new WaitForSeconds(2.0f);
            //ダメージを送信
            monobitView.RPC("Damage", MonobitEngine.MonobitTargets.Others, 25);
        }
    }
    
    // ダメージ受信メソッド
    [MunRPC]
    void Damage(int damage)
    {
       enemy.hp-=damage;
    }

    Color GetAlphaColor() {
        time += Time.deltaTime * 7.0f;
        Color color =new Color(255,255,255, Mathf.Sin(time) * 0.5f + 0.5f);
        return color;
    }
    
    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag!=transform.tag){
            enemy=coll.gameObject.GetComponent<Bacteria>();
            state='F';
        }
        if(coll.gameObject.tag=="stage"){
            state='D';
        }
    }
    void OnCollisionExit2D(Collision2D coll)
    {
        if(Mathf.Abs(movepoint.x)>0.1f ||Mathf.Abs(movepoint.y)>0.1f){
            state='M';
        }else{
            state='D';
        }
    }
}