using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 moveDirection;

    [SerializeField]
    private float moveSpeed = 5f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
    }

    /// <summary>
    /// 玩家移动函数
    /// 每一帧都会被 Update() 调用
    /// </summary>
    private void Move()
    {
        // 获取左右输入（A=-1，D=1）
        float horizontal = Input.GetAxisRaw("Horizontal");

        // 获取前后输入（S=-1，W=1）
        float vertical = Input.GetAxisRaw("Vertical");

        // 把两个输入组合成一个三维方向
        // x = 左右
        // y = 不移动（跳跃以后才会改变）
        // z = 前后
        Vector3 moveDirection = new Vector3(horizontal, vertical, 0f);

        // 如果方向长度大于1（例如同时按 W+D）
        // 就把它缩放成单位向量，避免斜着跑更快
        if (moveDirection.magnitude > 1)
        {
            moveDirection.Normalize();
        }

        // CharacterController.Move() 接收的是“位移”
        // 所以：
        // 方向 × 速度 × 每帧时间
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

}
