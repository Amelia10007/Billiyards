using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBallController : MonoBehaviour
{
    private static readonly float threholdVelocity = 0.05f;
    public static bool IsMoving;
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
        IsMoving = this.rb.velocity.magnitude > threholdVelocity;
        if (!IsMoving) this.rb.velocity = Vector3.zero;
        if (rb.position.y < -30)
        {
            PlayerBarController.ShotCount++;
            this.rb.position = new Vector3(0, 0.25f, 0);
            this.rb.velocity = Vector3.zero;
            this.rb.angularVelocity = Vector3.zero;
        }
        else if (this.rb.position.y < -0.05f && !this.hasPocketed)
        {
            this.hasPocketed = true;
            this.ballPocketAudioSource.PlayOneShot(this.ballPocketAudioSource.clip);
        }
        else if (rb.position.y > 0.25)
        {
            this.rb.position = new Vector3(this.rb.position.x, 0.25f, this.rb.position.z);
        }
        else if (rb.position.y > 0)
        {
            this.hasPocketed = false;
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
