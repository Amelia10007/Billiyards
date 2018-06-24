using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBarController : MonoBehaviour
{
    private static readonly float shotVelocityMax = 70;
    public static int DroppedNum;
    public static int ShotCount;
    private float pushVelocity;
    public Text ScoreText;
    public Text PushText;
    public Text WinText;
    public float PushTimeSpan;
    public float MoveCoefficient;
    public float RotationCoefficient;
    private Rigidbody rb;
    private AudioSource audioSource;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float pushingTimeSpan;
    private bool HasPushed;
    private Vector3 beforeMousePosition;
    private bool IsPushing
    {
        get { return this.pushingTimeSpan > 0; }
        set { this.pushingTimeSpan = value ? 0.001f : 0f; }
    }
    // Use this for initialization
    void Start()
    {
        DroppedNum = 0;
        ShotCount = 0;
        this.pushVelocity = shotVelocityMax;
        this.rb = this.GetComponent<Rigidbody>();
        this.audioSource = this.GetComponent<AudioSource>();
        this.initialPosition = this.rb.position;
        this.initialRotation = this.rb.rotation;
        this.IsPushing = false;
        this.beforeMousePosition = Input.mousePosition;
        this.gameObject.layer = LayerMask.NameToLayer("NeverCollision");
    }

    // Update is called once per frame
    void Update()
    {
        this.ScoreText.text = $"Shot count: {ShotCount}  Drop:{DroppedNum}";
        this.PushText.text = $"Shot speed: {this.pushVelocity}";
        this.ScoreText.color = (this.IsPushing || this.HasPushed) ? Color.red : Color.white;
        this.PushText.color = (this.IsPushing || this.HasPushed) ? Color.red : Color.white;
        if (DroppedNum >= 15) this.WinText.text = "You Win!!";
        //ショットを打ってから一定時間たったら初期位置に戻す
        if (this.IsPushing)
        {
            this.pushingTimeSpan += Time.deltaTime;
            if (this.pushingTimeSpan >= this.PushTimeSpan)
            {
                this.UnenablePush();
            }
        }
        //キー操作でショット時の速度変更
        if (Input.GetKey(KeyCode.Z)) this.pushVelocity += 0.4f;
        else if (Input.GetKey(KeyCode.X)) this.pushVelocity -= 0.4f;
        if (this.pushVelocity > shotVelocityMax) this.pushVelocity = shotVelocityMax;
        else if (this.pushVelocity < 1) this.pushVelocity = 1;
        //全てのボールが止まったら再びショットを打てるようにする
        if (this.HasPushed && !PlayerBallController.IsMoving && !BallController.IsMoving.Any(x => x))
        {
            this.HasPushed = false;
            this.beforeMousePosition = Input.mousePosition;
        }
        //
        if (this.IsPushing || this.HasPushed) return;
        //クリックでショット
        if (Input.GetMouseButtonDown(0))
        {
            this.IsPushing = true;
            this.HasPushed = true;
            this.rb.AddRelativeForce(0, this.pushVelocity, 0, ForceMode.VelocityChange);
            this.gameObject.layer = LayerMask.NameToLayer("PlayerBar");
            //高速ショットなら効果音を鳴らす
            if (this.pushVelocity >= 20) this.audioSource.PlayOneShot(this.audioSource.clip);
        }
        //右クリックで初期位置に戻る
        else if (Input.GetMouseButton(1))
        {
            this.SetInitialState();
        }
        //マウスの移動・スクロールでバーの姿勢変更
        var mouseDelta = Input.mousePosition - this.beforeMousePosition;
        this.rb.position += new Vector3(mouseDelta.x, 0, mouseDelta.y) * this.MoveCoefficient;
        this.rb.rotation = Quaternion.Euler(90, this.rb.rotation.eulerAngles.y + Input.mouseScrollDelta.y * this.RotationCoefficient, 0);
        this.beforeMousePosition = Input.mousePosition;
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "PlayerBall")
        {
            ShotCount++;
            this.UnenablePush();
        }
    }
    void UnenablePush()
    {
        this.IsPushing = false;
        this.HasPushed = true;
        this.gameObject.layer = LayerMask.NameToLayer("NeverCollision");
        this.SetInitialState();
    }
    void SetInitialState()
    {
        this.rb.position = this.initialPosition;
        this.rb.rotation = this.initialRotation;
        this.rb.velocity = Vector3.zero;
        this.rb.angularVelocity = Vector3.zero;
    }
}
