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

    // Start is called before the first frame update
    void Start()
    {
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
        }else{
            if(div!=null){
                StopCoroutine(div);
                div = null;
            }
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
            while(movepoint!=new Vector2(0,0)){
                
            }
        }else{
            
        }
        // 戦闘(Fight)
        if(state=='F'){

        }else{
            
        }
    }

    private IEnumerator Division()
    {
        Vector3 local = transform.InverseTransformPoint (transform.position);
        Vector3 direction=transform.TransformPoint (local+new Vector3(1.02f,0,0));
        RaycastHit2D hit;
        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            local = transform.InverseTransformPoint (transform.position);
            Debug.Log(transform.position);
            Debug.Log(local);
            direction=transform.TransformPoint (local+new Vector3(1.02f,0,0));
            hit = Physics2D.BoxCast(direction, transform.lossyScale, 0, Vector2.zero);
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

    Color GetAlphaColor() {
        time += Time.deltaTime * 7.0f;
        Color color =new Color(255,255,255, Mathf.Sin(time) * 0.5f + 0.5f);
        return color;
    }
}
