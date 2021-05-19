using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int fun_counter = 1;
    private LineRenderer renderer;
    private PolygonCollider2D collider;
    private Vector3 start_pos;
    private Vector3 before_pos;
    // Start is called before the first frame update
    void Start()
    {
        renderer = gameObject.GetComponent<LineRenderer>();
        collider = gameObject.GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            renderer.positionCount = 1;
            //タッチ位置取得
            Vector3 cameraPosition = Input.mousePosition;
            cameraPosition.z = 10;
            start_pos = Camera.main.ScreenToWorldPoint(cameraPosition);
            //ラインの起点設定
            renderer.SetPosition(0, start_pos);
            before_pos = start_pos;
            //コルーチンスタート
            StartCoroutine("Line");
        }
    }

    private IEnumerator Line()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            if (Input.GetMouseButton(0))
            {
                if (renderer.positionCount < 50)
                {
                    //タッチ位置取得
                    Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    cameraPosition.z = 0;
                    if (before_pos != cameraPosition)
                    {
                        renderer.positionCount += 1;
                        renderer.SetPosition(renderer.positionCount - 1, cameraPosition);
                        before_pos = cameraPosition;
                    }
                }
            }
            else
            {
                if (renderer.positionCount < 3)
                {
                    renderer.positionCount = 0;
                    break;
                }
                renderer.positionCount +=1;
                renderer.SetPosition(renderer.positionCount-1, start_pos);
                Vector3[] collider_list = new Vector3[renderer.positionCount];
                Vector2[] collider_list_2 = new Vector2[renderer.positionCount];
                renderer.GetPositions(collider_list);
                for (int i = 0; i< renderer.positionCount; i++){
                    collider_list_2[i]= transform.TransformPoint(collider_list[i]);
                }
                collider.SetPath(0, collider_list_2);
                yield return new WaitForSeconds(0.01f);
                collider.pathCount = 0;
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        /// 2D同士が接触した瞬間の１回のみ呼び出される処理
        Destroy(collider.gameObject);
        fun_counter -= 1;
    }
}