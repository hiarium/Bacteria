using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // ボタンが押されたら呼び出される関数
    public void OnClick()
    {
        // シーンの切り替え
        SceneManager.LoadScene("Home");
    }
}
