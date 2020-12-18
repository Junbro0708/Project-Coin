using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float hAxis;
    float vAxis;
    public float moveSpeed = 0.5f;
    public float jumpPower = 1f;

    public int coin = 0;
    int maxCoin = 99999;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Animator anim;
    Rigidbody rigid;
    AudioSource audio;
    GameObject nearObject;
    GameManagerLogic manager;

    bool wDown;
    bool jDown;
    bool isJump = false;
    bool isDodge;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput(); // 제일 위에 있어야 함
        Move();
        Turn();
        Jump();
        Dodge();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
            anim.SetBool("isJump", false);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch(item.type)
            {
                    case Item.Type.Coin:
                    coin += item.value;
                    audio.Play();
                    if (coin > maxCoin)
                        coin = maxCoin;
                    
                    break;
            }
            Destroy(other.gameObject);
        }
    }
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
    } // 입력 함수
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        transform.position += moveVec * moveSpeed * (wDown ? 0.3f : 1f) * Time.deltaTime; // 삼항 연산자

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);

    } // 캐릭터 이동 함수
    void Turn()
    {
        transform.LookAt(transform.position + moveVec); // 우리가 나아가는 방향으로 바라본다.
    } // 캐릭터 회전 함수
    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }
    void Dodge()
    {
        if(jDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            dodgeVec = moveVec;
            moveSpeed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.6f);
        }
    }
    void DodgeOut()
    {
        moveSpeed *= 0.5f;
        isDodge = false;
    }
}
