using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer mixer; // A reference to the game's audio mixer
    public Slider audioSlider; // A reference to the audio slider ( UI element)
    public Slider textSpeedSlider; // A reference to the text speed slider ( UI element)

    // Sets the initial slider positions on menu load
    void Start(){
        float soundValue = PlayerPrefs.GetFloat("Volume", 0.9783473f);
        soundValue = Mathf.Exp(soundValue/20);
        audioSlider.value = soundValue;
        textSpeedSlider.value = PlayerPrefs.GetFloat("TextSpeed", 0.15f);
    }

    // Updates the volume on slider drag
    public void SetVolume(float volume){
        float value = Mathf.Log(volume) * 20;
        PlayerPrefs.SetFloat("Volume", value);
        mixer.SetFloat("musicVol", value);
    }

    // Updates the text speed on slider drag
    public void SetTextSpeed(float speed){
        PlayerPrefs.SetFloat("TextSpeed", speed);
        TextManager.textSpeed = speed;
    }
}
