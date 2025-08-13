using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpscareManager : MonoBehaviour
{   
    public SlideManager slideManager; // Reference to the slide manager

    void Start(){
        StartCoroutine(ChangeSlideAfterJumpscare());
    }

    // Used for the zooming effect of the jumpscare
    void Update() {
        gameObject.transform.localScale = new Vector2(1+Mathf.PingPong(Time.time*3f, 1)*0.65f, 1+Mathf.PingPong(Time.time*3f, 1)*0.65f);
    }

    // Changes the slide after 5 seconds
    private IEnumerator ChangeSlideAfterJumpscare(){
        yield return new WaitForSeconds(5f);
        slideManager.LoadNextSlide();
    }
}
