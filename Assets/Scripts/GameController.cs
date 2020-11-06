using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public NejikoController nejiko;

    public Text scoreText;

    public LifePanel lifePanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //スコアを更新
        int score = CalcScore();
        scoreText.text = "Score:" + score + "m";
        
        //ライフパネルを更新
        lifePanel.UpdateLite(nejiko.Lite());
    }

    int CalcScore()
    {
        //ねじ子の走行距離をスコアにする
        return (int) nejiko.transform.position.z;
    }
}
