using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonobitEngine;
using UnityEngine.UI;

public class GameManager : MonobitEngine.MonoBehaviour
{
    public int counter=0;
    private bool first=true;
    private LineRenderer line1;
    private LineRenderer lRend;
    private GameObject line2; 
    private PolygonCollider2D collider;
    private Vector3 start_pos;
    private Vector3 before_pos;
    private List<GameObject> selectObject = new List<GameObject>();
    private string roomName = "";
    private Text score_text;
    private int enemy_score=20;

    void Start()
    {
        line1 = transform.GetComponent<LineRenderer>();
        collider = transform.GetComponent<PolygonCollider2D>();
        score_text = GameObject.Find("Text").GetComponent<Text>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //タッチ位置取得
            Vector3 cameraPosition = Input.mousePosition;
            cameraPosition.z = 10;
            start_pos = Camera.main.ScreenToWorldPoint(cameraPosition);

            // 描画されてない時
            if(line1.positionCount==0){
                line1.positionCount=1;
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
                foreach (RaycastHit2D hit2d in Physics2D.RaycastAll((Vector2)ray.origin, (Vector2)ray.direction)){
                    if(hit2d.transform.gameObject.name != "GameManager"){
                        continue;
                    }else{
                        // 円内の時
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
                }
                // 円外の時
                if(line2==null){
                    // 既存line1破壊
                    line1.positionCount = 0;
                    collider.pathCount=0;
                    line1.loop = false;
                    while(selectObject.Count!=0){
                        selectObject[0].GetComponent<Bacteria>().state = 'D';
                        selectObject.RemoveAt(0);
                    }
                    //line1の再描画
                    line1.positionCount = 1;
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
                    selectObject[0].GetComponent<Bacteria>().movepoint = lPos[1]-lPos[0];
                    selectObject[0].GetComponent<Bacteria>().state = 'M';
                    selectObject.RemoveAt(0);
                }
                // 全line削除
                Destroy(line2);
                line1.positionCount = 0;
                collider.pathCount=0;
                line1.loop = false;
            }
        }

        // ルームの人数が2人になったらゲームスタート
        if (MonobitNetwork.room.playerCount==2 && first){
            float x_renge;
            string character;
            // プレイヤーキャラクタが未登場の場合に登場させる
            if(MonobitNetwork.isHost){
                x_renge = -6.6f;
                character = "Bacteria";
            }else{
                x_renge = 5.0f;
                character = "Bacteria2";
            }
            for(int i=0;i<20;i++){
                float x = Random.Range(x_renge, x_renge+1.6f);
        		float y = Random.Range(-1.1f, 1.1f);
                MonobitNetwork.Instantiate(character,new Vector3(x,y,0), Quaternion.identity,0).tag="Player";
                counter+=1;
            }
            first=false;
        }
        
        monobitView.RPC("Score", MonobitEngine.MonobitTargets.Others, counter);
        score_text.text=enemy_score+" VS "+counter;
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

                Vector3[] collider_list = new Vector3[line1.positionCount];
                Vector2[] collider_list_2 = new Vector2[line1.positionCount];
                // lineの各頂点の座標を取得してVector2に変換
                line1.GetPositions(collider_list);
                for (int i = 0; i< line1.positionCount; i++){
                    collider_list_2[i]= transform.TransformPoint(collider_list[i]);
                }
                // lineと同じ形のコライダーを作成
                collider.SetPath(0, collider_list_2);
                yield return new WaitForSeconds(0.1f);
                // 選択対照がなかったら削除
                if(selectObject.Count==0){
                    line1.positionCount = 0;
                    collider.pathCount=0;
                    line1.loop = false;
                }
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

    void OnGUI()
    {
        if (MonobitNetwork.isConnect){
            // ボタン入力でサーバから切断＆シーンリセット
            if (GUILayout.Button("Disconnect", GUILayout.Width(150))){
                // サーバから切断する
                MonobitNetwork.DisconnectServer();
 
                // シーンをリロードする
                #if UNITY_5_3_OR_NEWER || UNITY_5_3
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                #else
                Application.LoadLevel(Application.loadedLevelName);
                #endif
            }
            if (!MonobitNetwork.inRoom){
                //ルーム名の入力
                GUILayout.BeginHorizontal();
                GUILayout.Label("RoomName : ");
                roomName = GUILayout.TextField(roomName, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                //ルームを作成して入室
                if (GUILayout.Button("Create Room", GUILayout.Width(150))){
                    MonobitNetwork.CreateRoom(roomName);
                }

                //ルーム一覧を検索
                foreach( RoomData room in MonobitNetwork.GetRoomData()){
                    // ルームパラメータの可視化
                    System.String roomParam =
                        System.String.Format(
                            "{0}({1}/{2})",
                            room.name,
                            room.playerCount,
                            ((room.maxPlayers == 0) ? "-" : room.maxPlayers.ToString())
                        );
 
                    // ルームを選択して入室する
                    if (GUILayout.Button("Enter Room : " + roomParam)){
                        MonobitNetwork.JoinRoom(room.name);
                    }
                }
            }
        }else{
            MonobitNetwork.autoJoinLobby = true;
            // MUNサーバに接続する
            if( GUILayout.Button("Connect Server", GUILayout.Width(150))){
                MonobitNetwork.ConnectServer("v1.0");
            }
        }
    }
    [MunRPC]
    void Score(int score)
    {
       enemy_score=score;
    }
}