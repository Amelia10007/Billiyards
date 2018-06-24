using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private static readonly float threholdVelocity = 0.05f;
    public static IReadOnlyList<bool> IsMoving
    {
        get
        {
            return isMoving;
        }
    }
    private static List<bool> isMoving = new List<bool>();
    private Rigidbody rb;
    private AudioSource ballCollisionAudioSource;
    private AudioSource ballPocketAudioSource;
    private bool hasPocketed;
    // Use this for initialization
    void Start()
    {
        this.rb = this.GetComponent<Rigidbody>();
        var audioSources = this.GetComponents<AudioSource>();
        this.ballCollisionAudioSource = audioSources[0];
        this.ballPocketAudioSource = audioSources[1];
        this.hasPocketed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving.Count >= 15) isMoving.Clear();
        if (this.rb.velocity.magnitude <= threholdVelocity) this.rb.velocity = Vector3.zero;
        var moving = !this.hasPocketed && this.rb.velocity.magnitude > threholdVelocity;
        isMoving.Add(moving);
        if (this.rb.position.y < -30)
        {
            PlayerBarController.DroppedNum++;
            Destroy(this.gameObject);
        }
        if (this.rb.position.y < -0.05f && !this.hasPocketed)
        {
            this.hasPocketed = true;
            this.ballPocketAudioSource.PlayOneShot(this.ballPocketAudioSource.clip);
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ball" && this.rb.velocity.magnitude > 1)
        {
            this.ballCollisionAudioSource.PlayOneShot(this.ballCollisionAudioSource.clip);
        }
    }
}
