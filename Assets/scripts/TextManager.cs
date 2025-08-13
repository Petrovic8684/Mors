using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    [TextArea(3,10)]
    public string[] sentences; // Array containing sentences to type out
    private int sentenceIndex = 0; // The index of the current sentence in the [sentences] array
    public int choiceIndex; // The index of a sentence where the choice of the slide is given

    public GameObject[] choiceButtons; // Array containing buttons that lead to different game outcomes
    public GameObject leftMouseButtonText; // Reference to the text that reminds you to mouseclick to progress
    public string textTranslation; // English translation to latin text

    // Fonts
    private Font textFont; // Variable used to switch the fonts mid-slide
    public Font typewriterFont; // Reference to the typewriter font
    public Font writingFont; // Reference to the handwriting font

    // Sounds
    private AudioSource textSound; // Variable used to switch text sound mid-slide
    private AudioSource textSkipSound; // Variabla used to switch text skip sound mid-slide
    public AudioSource typeWriterSound; // Reference to the typewriter sound
    public AudioSource carriageReturnSound; // Reference to the carriage return sound
    public AudioSource penStrokeSound; // Reference to the pen stroke sound
    public AudioSource clickSound; // Reference to the sound that is heard when a button is pressed
    public AudioSource writingSound; // Reference to the writing sound
    public AudioSource specialSound1; // Reference to the first special sound
    public AudioSource specialSound2; // Reference to the second special sound

    // Logic variables
    private bool canPlayNext = false; // Variable that keeps track of when you can mouseclick to progress
    public bool isEndGame = false; // Variable that keeps track if the game is on it's last slide
    private bool canSkipSlide = false; // Variable that keeps track if you can skip the slide's typing animation
    private bool finishedTyping = false; // Variable that keeps track if the sentence is finished typing

    // Special effect variables
    public static float textSpeed = 0.15f; // Speed of the text animation
    public int sentenceToApplyHandWrittenFont; // Index of the sentence where the handwriting font should be applied in the slide

    public int sentenceToPlaySpecialSound1; // Index of the sentence where the first special sound should be played in the slide
    public int charToPlaySpecialSound1; // Index of the letter in the sentence where the first special sound should be played in the slide

    public int sentenceToPlaySpecialSound2; // Index of the sentence where the second special sound should be played in the slide
    public int charToPlaySpecialSound2; // Index of the letter in the sentence where the second special sound should be played in the slide

    public SlideManager slideManager; // Reference to the slide manager

    // Applies the fonts, sounds and line spacing of a particular sentence
    private void ApplyTextSettings(){
        if(sentenceIndex == sentenceToApplyHandWrittenFont && writingSound != null && writingFont != null && penStrokeSound != null){
            textSound = writingSound;
            textSkipSound = penStrokeSound;
            textFont = writingFont;
            gameObject.GetComponent<Text>().fontSize = 85;
            gameObject.GetComponent<Text>().lineSpacing = 0.7f;
            gameObject.GetComponent<Shadow>().enabled = false;
        } else {
            textSound = typeWriterSound;
            textSkipSound = carriageReturnSound;
            textFont = typewriterFont;
            gameObject.GetComponent<Text>().fontSize = 60;
            gameObject.GetComponent<Text>().lineSpacing = 1f;
            gameObject.GetComponent<Shadow>().enabled = true;
        }

        gameObject.GetComponent<Text>().font = textFont;
    }

    void Start(){
        // Hides the choice buttons at the start
        if(choiceButtons.Length != 0){
            foreach(GameObject button in choiceButtons){
                button.SetActive(false);
            }
        }
        // Types the first sentence of the slide
        if(sentences.Length != 0){
            StartCoroutine(TypeSentence(sentences[sentenceIndex++]));
            Invoke("DelayCanSkipSlide", 1f);
        }
    }

    // Used to prevent player from skiping slide's typing animaton right away
    private void DelayCanSkipSlide(){
        canSkipSlide = true;
    }

    private void InitiateNextSentence(){
        // Stops any special sound still playing
        if(specialSound1 != null){
            if(specialSound1.isPlaying){
                specialSound1.Stop();
            }
        }
        if(specialSound2 != null){
            if(specialSound2.isPlaying){
                specialSound2.Stop();
            }
        }

        clickSound.Play();
        Invoke("DelayCanSkipSlide", 1f);
        // If there are sentences to type, type the next one
        if(sentenceIndex < sentences.Length){
            StopAllCoroutines();
            StartCoroutine(TypeSentence(sentences[sentenceIndex++]));
        }
        // If not, load the next slide
        else{
            // If this is the last slide, load the main menu
            if(isEndGame){
                slideManager.ReloadScene();
            }else{
                slideManager.LoadNextSlide();
            }
        }
    }

    void Update(){
        // If escape is pressed, exit the game to main menu
        if(Input.GetKeyDown(KeyCode.Escape)){
            slideManager.ReloadScene();
        }
        // If no choices are given, type next sentence on mouse click
        if(canPlayNext && Input.GetMouseButtonDown(0)){
           InitiateNextSentence();
        }
        // If RMB is pressed, skip the slide's typing animation
        if(Input.GetMouseButtonDown(1) && canSkipSlide && sentences.Length != 0){
            StopAllCoroutines();
            StartCoroutine(SkipSlide());
        }
    }

    // Skips slide's typing animation
    private IEnumerator SkipSlide(){
        canSkipSlide = false;

        gameObject.GetComponent<Text>().text = sentences[sentenceIndex-1];

        textSound.Stop();
        textSkipSound.Play();

        finishedTyping = true;

        // Shows the choice buttons if there are any
        if(choiceButtons.Length != 0 && sentenceIndex == choiceIndex){
            yield return new WaitForSeconds(1.5f);
            EnableButtons();
        }
        // If not, show text that reminds player to progress on mouseclick
        else{
            yield return new WaitForSeconds(1f);
            canPlayNext = true;
            leftMouseButtonText.SetActive(true);
        }  
    }

    // Moves the text a bit upwards when choice buttons are active
    private void MoveTextOnChoice(){
        if(choiceButtons.Length != 0 && sentenceIndex == choiceIndex){
            gameObject.transform.localPosition = new Vector3(0f, 50f, 0f);
        }
        else if(choiceButtons.Length != 0 && sentenceIndex != choiceIndex){
            gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }

    // Shows the choice buttons
    private void EnableButtons(){
        foreach(GameObject button in choiceButtons){
                button.SetActive(true);
        }
    }

    // Hides the choice buttons
    private void DisableButtons(){
        if(choiceButtons.Length != 0 && sentenceIndex != choiceIndex){
            foreach(GameObject button in choiceButtons){
                button.SetActive(false);
            }
        }
    }

    private IEnumerator TypeSentence(string sentence){
        ApplyTextSettings();
        finishedTyping = false;
        leftMouseButtonText.SetActive(false);
        canPlayNext = false;
        
        MoveTextOnChoice();
        DisableButtons();

        gameObject.GetComponent<Text>().text = "";

        yield return new WaitForSeconds(textSpeed*12.5f);

        textSound.Play();

        // Types out letters one by one
        for(int i=0;i<sentence.Length;i++){
            if(!textSound.isPlaying){
                textSound.Play();
            }
            if(specialSound1 != null && sentenceIndex == sentenceToPlaySpecialSound1 && i == charToPlaySpecialSound1){
                specialSound1.Play();
            }
            if(specialSound2 != null && sentenceIndex == sentenceToPlaySpecialSound2 && i == charToPlaySpecialSound2){
                specialSound2.Play();
            }

            if(i == sentence.Length-5){
                canSkipSlide = false;
            }
            char letter = sentence[i];
            gameObject.GetComponent<Text>().text += letter;

            // Makes a longer pause if a dot i placed
            if(letter == '.' && i != sentence.Length-1){
                textSound.Stop();
                yield return new WaitForSeconds(textSpeed*6.5f);
                textSound.Play();
            }
            // Makes a longer pause if a comma is placed
            else if(letter == ','){
                textSound.Stop();
                yield return new WaitForSeconds(textSpeed*5f);
                textSound.Play();
            }else yield return new WaitForSeconds(textSpeed);
            yield return null;
        }
        canSkipSlide = false;
        finishedTyping = true;
        textSound.Stop();

        // Shows the choice buttons if there are any
        if(choiceButtons.Length != 0 && sentenceIndex == choiceIndex){
            yield return new WaitForSeconds(1.5f);
            EnableButtons();
        }
        // If not, show text that reminds player to progress on mouseclick
        else{
            yield return new WaitForSeconds(1f);
            canPlayNext = true;
            leftMouseButtonText.SetActive(true);
        }
    }

    // Changes text to english on hover enter
    public void ChangeTextOnHoverEnter(int indexToChange){
        if(sentenceIndex == indexToChange && finishedTyping){
            gameObject.GetComponent<Text>().text = textTranslation;
        }
    }

    // Changes text to latin on hover exit
    public void ChangeTextOnHoverExit(int indexToChange){
        if(sentenceIndex == indexToChange && finishedTyping){
            gameObject.GetComponent<Text>().text = sentences[sentenceIndex-1];
        }
    }

    // Choices
    public void LoadSlide(int index){
        slideManager.LoadParticularSlide(index);
    }

    public void ContinueSlide(){
        InitiateNextSentence();
    }
}
