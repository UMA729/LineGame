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
    Vector3 origine_pos;
    [SerializeField] private float press_limmit = 0;

    [Header("ボタンギミック")]
    bool isPushing = false;
    bool GimmickActive = false;
    [SerializeField] GameObject ButtonGimmick;
    [SerializeField] private float open_limmit; 
    Vector3 startPos;

    [Header("ギミック作動時間")]
    public float gimmick_shoot_time = 0;
    float gimmick_count_time = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ButtonGimmick != null)
        {
            if(ButtonGimmick.gameObject.CompareTag("UpDoor"))
            {
                startPos = ButtonGimmick.transform.position;
            }
        }
        if (gameObject.CompareTag("Hole"))
        {
            leftClose = leftDoor.localRotation;
            rightClose = rightDoor.localRotation;
        }

        press_limmit += transform.position.y;
        origine_pos = transform.position;
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
            if (isPressing)
            {
                float move =
                    Vector3.Dot(transform.position - origine_pos, transform.up);

                press_speed = 5.0f;

                transform.position -= transform.up * press_speed * Time.deltaTime;

                if (move < -press_limmit)
                {
                    isPressing = false;
                }
            }


            if (!isPressing)
            {
                press_speed = 1.0f;

                transform.position += transform.up * press_speed * Time.deltaTime;

                float move =
                    Vector3.Dot(transform.position - origine_pos, transform.up);

                if (move >= 0)
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
        if(isPushing)
        {
            Debug.Log("上昇中");
            if (ButtonGimmick.gameObject.layer == LayerMask.NameToLayer("UpDoor"))
            {
                float moved =
                Vector3.Dot(
                    ButtonGimmick.transform.position - startPos,
                    ButtonGimmick.transform.up);

                if (moved < open_limmit)
                ButtonGimmick.transform.position += ButtonGimmick.transform.up * speed * Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Hole"))
        {
            isOpen = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Weight") && gameObject.CompareTag("Button") ||
            collision.gameObject.CompareTag("Player")&&gameObject.CompareTag("Button"))
        {
            isPushing = true;
        }
    }
}