using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int fun_counter = 1;
    private LineRenderer line1;
    private LineRenderer lRend;
    private GameObject line2; 
    private PolygonCollider2D collider;
    private Vector3 start_pos;
    private Vector3 before_pos;
    private bool circle = false;
    private List<GameObject> selectObject = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        line1 = gameObject.GetComponent<LineRenderer>();
        collider = gameObject.GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //タッチ位置取得
            Vector3 cameraPosition = Input.mousePosition;
            cameraPosition.z = 10;
            start_pos = Camera.main.ScreenToWorldPoint(cameraPosition);

            // 描画されてない時
            if(!circle){
                line1.positionCount = 1;
                //line1の起点設定
                line1.SetPosition(0, start_pos);
                before_pos = start_pos;
                //コルーチンスタート
                StartCoroutine("Line");
            }
            // 描画済の時
            else{
                // 円内のクリックか判定
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
                //円内の時
                if (hit2d.collider&&hit2d.transform.gameObject.name == "GameManager") {
                    // line2生成
                    line2 = new GameObject ("Line");
                    line2.transform.parent = GameObject.Find("Canvas").transform;
                    lRend = line2.AddComponent<LineRenderer> ();
                    lRend.SetVertexCount(2);
                    lRend.SetColors(new Color(0,37,255),new Color(0,0.13f,1));
                    lRend.material = new Material(Shader.Find("Sprites/Default"));
                    lRend.SetWidth (0.1f, 0.1f);
                    lRend.SetPosition (0, start_pos);
                    lRend.SetPosition (1, start_pos);
                }
                // 円外の時
                else{
                    // 既存line1破壊
                    line1.positionCount = 1;
                    collider.pathCount=0;
                    circle = false;
                    line1.loop = false;
                    while(selectObject.Count!=0){
                        selectObject[0].GetComponent<Bacteria>().state = 'D';
                        selectObject.RemoveAt(0);
                    }
                    //line1の再描画
                    line1.SetPosition(0, start_pos);
                    before_pos = start_pos;
                    //コルーチンスタート
                    StartCoroutine("Line");
                }
            }
        }
        if (Input.GetMouseButton(0)){
            if(line2 != null){
                Vector3 cameraPosition = Input.mousePosition;
                cameraPosition.z = 10;
                lRend.SetPosition (1, Camera.main.ScreenToWorldPoint(cameraPosition));
            }
        }
        // line2描画済みの時、全line削除と選択中の菌を移動する
        if (Input.GetMouseButtonUp(0)){
            if(line2 != null){
                // 選択中の菌に移動先指定
                Vector3[] lPos = new Vector3[2];
                lRend.GetPositions(lPos);
                while(selectObject.Count!=0){
                    selectObject[0].GetComponent<Bacteria>().state = 'M';
                    selectObject[0].GetComponent<Bacteria>().movepoint = lPos[1];
                    selectObject.RemoveAt(0);
                }
                // 全line削除
                Destroy(line2);
                line1.positionCount = 1;
                collider.pathCount=0;
                circle = false;
                line1.loop = false;
            }
        }
    }

    private IEnumerator Line()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            // クリック中line描画処理
            if (Input.GetMouseButton(0))
            {
                if (line1.positionCount < 50)
                {
                    //タッチ位置取得
                    Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    cameraPosition.z = 0;
                    if (before_pos != cameraPosition)
                    {
                        line1.positionCount += 1;
                        line1.SetPosition(line1.positionCount - 1, cameraPosition);
                        before_pos = cameraPosition;
                    }
                }
            }
            else　// クリックをやめたときの処理
            {
                // line頂点の数が3未満ならなかったことにする
                if (line1.positionCount < 3)
                {
                    line1.positionCount = 0;
                    yield break;
                }
                //　lineの終点と起点をそろえる
                line1.loop = true;
                circle = true;

                Vector3[] collider_list = new Vector3[line1.positionCount];
                Vector2[] collider_list_2 = new Vector2[line1.positionCount];
                // lineの各頂点の座標を取得してVector2に変換
                line1.GetPositions(collider_list);
                for (int i = 0; i< line1.positionCount; i++){
                    collider_list_2[i]= transform.TransformPoint(collider_list[i]);
                }
                // lineと同じ形のコライダーを一瞬だけ作成
                collider.SetPath(0, collider_list_2);
                yield break;
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        /// 2D同士が接触した瞬間の１回のみ呼び出される処理
        if(collider.gameObject.tag == "Player"){
            selectObject.Add(collider.gameObject);
            collider.gameObject.GetComponent<Bacteria>().state = 'S';
        }
    }
}