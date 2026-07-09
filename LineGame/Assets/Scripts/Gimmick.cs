using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gimmick : MonoBehaviour
{ 
    [Header("弾発射ギミック")]
    public GameObject Gummick;
    public Transform spawnPoint;


    [Header("床ギミック")]
    public Transform leftDoor;
    public Transform rightDoor;
    public float openAngle = 90f;
    public float speed = 3f;
    public bool isOpen = false;
    Quaternion leftClose;
    Quaternion rightClose;

    [SerializeField] float openTime = 1f;
    private float openTimer = 0f;

    [Header("プレスギミック")]
    public bool isPressing = true;
    float press_speed = 0;
    float origine_pos;
    [SerializeField] private float press_limmit = 0;

    [Header("ボタンギミック")]
    bool isPushing = false;
    bool GimmickActive = false;
    [SerializeField] GameObject ButtonGimmick;

    [Header("ギミック作動時間")]
    public float gimmick_shoot_time = 0;
    float gimmick_count_time = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameObject.CompareTag("Hole"))
        {
            leftClose = leftDoor.localRotation;
            rightClose = rightDoor.localRotation;
        }

        press_limmit += transform.position.y;
        origine_pos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.CompareTag("Arrow"))
        {
            Arrow();
        }
        else if (gameObject.CompareTag("Press"))
        {
            Press();
        }
        else if (gameObject.CompareTag("Hole"))
        {
            Hole();
        }
        else if (gameObject.CompareTag("Button"))
        {
            Button();
        }
    }

    void Arrow()
    { 
        gimmick_count_time += Time.deltaTime;
        if (gimmick_count_time > gimmick_shoot_time)
        {
            Instantiate(Gummick, spawnPoint.position,spawnPoint.rotation);
            gimmick_count_time = 0;
        }
    }

    void Press()
    {
        gimmick_count_time += Time.deltaTime;
        
        if (gimmick_count_time > gimmick_shoot_time)
        {
            //�v���X�O
            if (isPressing)
            {
                press_speed = 5.0f;
                transform.position -= transform.up * press_speed * Time.deltaTime;
                if (transform.position.y < press_limmit)
                {
                    isPressing = false;
                }
            }
            //�v���X��
            if (!isPressing)
            {
                press_speed = 1.0f;
                transform.position += transform.up * press_speed * Time.deltaTime;
                
                if(transform.position.y > origine_pos)
                { 
                    isPressing = true;
                    gimmick_count_time = 0;
                }
            }
        }
    }

    void Hole()
    {
        Quaternion leftTarget;
        Quaternion rightTarget;

        if (isOpen)
        {
            leftTarget = Quaternion.Euler(0, 0, openAngle);
            rightTarget = Quaternion.Euler(0, 0, -openAngle);
        }
        else
        {
            leftTarget = leftClose;
            rightTarget = rightClose;
        }

        leftDoor.localRotation =
            Quaternion.Lerp(
                leftDoor.localRotation,
                leftTarget,
                Time.deltaTime * speed
            );

        rightDoor.localRotation =
            Quaternion.Lerp(
                rightDoor.localRotation,
                rightTarget,
                Time.deltaTime * speed
            );

        if (isOpen)
        {
            openTimer += Time.deltaTime;

            if (openTimer >= openTime)
            {
                isOpen = false;
                openTimer = 0f;
            }
        }

    }

    private void Button()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Hole"))
        {
            isOpen = true;
        }
    }
}