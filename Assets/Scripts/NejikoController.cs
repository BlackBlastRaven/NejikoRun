using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NejikoController : MonoBehaviour
{
    private const int MinLane = -2;
    private const int MaxLane = 2;
    private const float LaneWidth = 1.0f;
    private const int DefaultLife = 3;
    private const float StunDuration = 0.5f;
    private CharacterController controller;

    private Animator animator;
    Vector3 moveDirection = Vector3.zero;
    private int targetLane;
    private int life = DefaultLife;
    private float recoverTime = 0.0f;

    public float gravity;

    public float speedZ;
    public float speedX;

    public float speedJump;
    public float accelerationZ;
    
    //HPを取得するための関数
    public int Lite()
    {
        return life;
    }

    //スタン判定
    bool IsStun()
    {
        return recoverTime > 0.0f || life <= 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        //必要なコンポーネントを自動取得する
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //PCでのデバッグ用
        if (Input.GetKeyDown("left"))
        {
            MoveToLeft();
        }

        if (Input.GetKeyDown("right"))
        {
            MoveToRight();
        }

        if (Input.GetKeyDown("space"))
        {
            Jump();
        }
        
        //スタン時の行動
        if (IsStun())
        {
            //動きを止め、スタン状態解除の復帰カウントを進める
            moveDirection.x = 0.0f;
            moveDirection.z = 0.0f;
            recoverTime -= Time.deltaTime;
        }
        else
        {
            //ゲーム開始後、自動でz方向に前進させる。徐々に加速する。
            float acceleratedZ = moveDirection.z + (accelerationZ * Time.deltaTime);
            moveDirection.z = Mathf.Clamp(acceleratedZ, 0, speedZ);
        
            //X方向は目標のポジションまでの差分の割合で速度を計算
            float ratioX = (targetLane * LaneWidth - transform.position.x) / LaneWidth;
            moveDirection.x = ratioX * speedX;
        }

        /*キーボード操作用の古いコード
        if (controller.isGrounded)
        {
            if (Input.GetAxis("Vertical") >0.0f)
            {
                moveDirection.z = Input.GetAxis("Vertical") * speedZ;
            }
            else
            {
                moveDirection.z = 0;
            }
        
            transform.Rotate(0,Input.GetAxis("Horizontal") * 3,0);

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = speedJump;
                animator.SetTrigger("jump");
            }
        }
        */
        //重力分の力を毎フレーム追加する
        moveDirection.y -= gravity * Time.deltaTime;
        
        //移動実行
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        controller.Move(globalDirection * Time.deltaTime);
        
        //移動後接地していたらY方向の速度はリセットする(地面を貫通して落下しないようにする)
        if (controller.isGrounded)
        {
            moveDirection.y = 0;
        }
        
        //速度が0以上なら走っているアニメーションフラグをオンにする
        animator.SetBool("run",moveDirection.z > 0.0f);
    }
    
    //左のレーンに移動する
    public void MoveToLeft()
    {
        //スタン中は入力を無効にする
        if (IsStun())
        {
            return;
        }
        
        if (controller.isGrounded && targetLane > MinLane)
        {
            targetLane--;
        }
    }
    
    //右のレーンに移動する
    public void MoveToRight()
    {
        //スタン中は入力を無効にする
        if (IsStun())
        {
            return;
        }
        
        if (controller.isGrounded && targetLane < MaxLane)
        {
            targetLane++;
        }
    }

    public void Jump()
    {
        //スタン中は入力を無効にする
        if (IsStun())
        {
            return;
        }
        
        if (controller.isGrounded)
        {
            moveDirection.y = speedJump;
            
            //ジャンプトリガーを設定
            animator.SetTrigger("jump");
        }
    }
    
    //CharacterControllerに衝突判定があったときの処理
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (IsStun())
        {
            return;
        }

        if (hit.gameObject.tag == "Robo")
        {
            //HPを減少させてスタン状態に移行
            life--;
            recoverTime = StunDuration;
            
            //ダメージトリガーを設定
            animator.SetTrigger("damage");
            
            //ヒットしたオブジェクトは削除
            Destroy(hit.gameObject);
        }
    }
}
