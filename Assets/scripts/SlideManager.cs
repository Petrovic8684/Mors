using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlideManager : MonoBehaviour
{
    public GameObject[] slides; // Array containing the slides
    public AudioSource clickSound; // Reference to the button click sound
    private GameObject transitionCanvas; // Reference to the black canvas used for the transition effect between slides
    private Animator transitionAnimator; //Reference to the animator component of the black canvas

    void Awake(){
        transitionCanvas = GameObject.Find("TransitionCanvas/TransitionPanel"); // Finds the [transitionCanvas] object in the hierarchy
        transitionAnimator = transitionCanvas.GetComponent<Animator>(); // Gets it's animator component
    }

    void Start(){
        transitionAnimator.SetTrigger("Fade"); // Plays the fade animation on menu load
    }

    // Reloads the scene
    public void ReloadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Returns the index of the slide that is currently shown
    private int ReturnActiveSlideIndex(){
        for(int i=0;i<slides.Length;i++){
            if(slides[i].activeSelf){
                return i;
            }
        }

        return -1;
    }

    // Loads the next slide
    public void LoadNextSlide(){
        transitionAnimator.SetTrigger("Fade");
        int activeSlideIndex = ReturnActiveSlideIndex();
        slides[activeSlideIndex].SetActive(false);
        slides[activeSlideIndex+1].SetActive(true);
    }

    // Used for loading the jumpscare slide, since it doesn't need a fade effect
    public void LoadParticularSlideWithoutFade(int index){
        int activeSlideIndex = ReturnActiveSlideIndex();
        foreach(GameObject slide in slides){
            slide.SetActive(false);
        }

        slides[index].SetActive(true);
    }

    // Loads a particular slide
    public void LoadParticularSlide(int index){
        if(AnimatorIsPlaying()){
            transitionAnimator.Rebind();
            transitionAnimator.ResetTrigger("Fade");
        }
        transitionAnimator.SetTrigger("Fade");
        LoadParticularSlideWithoutFade(index);
    }

    // Main menu exit button
    public void ExitGame(){
        clickSound.Play();
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    // Returns true if the animator is playing any animation, false if not
    bool AnimatorIsPlaying(){
        return transitionAnimator.GetCurrentAnimatorStateInfo(0).length > transitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
