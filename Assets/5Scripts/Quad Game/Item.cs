using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type {Coin}; // 열거형 타입. 타입일 뿐 변수가 아님.
    public Type type;
    public int value;

    void Update()
    {
        transform.Rotate(Vector3.up * 15 * Time.deltaTime);
    }
}
