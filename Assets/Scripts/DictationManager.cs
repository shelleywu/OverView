using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class dictationResponse {
    public string text;
    public AudioClip clip;
}


public class DictationManager : MonoBehaviour
{
    [SerializeField]
    private String m_Hypotheses = "";

    [SerializeField]
    private String m_Recognitions = "";

    private DictationRecognizer m_DictationRecognizer;

    private AudioClip tempClip;

    void StartRecognizer()
    {
        m_DictationRecognizer = new DictationRecognizer();

        m_DictationRecognizer.DictationResult += (string text, ConfidenceLevel confidence) =>
        {
            Debug.LogFormat("Dictation result: {0}", text);
            m_Recognitions += text + "\n";
        };

        m_DictationRecognizer.DictationHypothesis += (string text) =>
        {
            Debug.LogFormat("Dictation hypothesis: {0}", text);
            m_Hypotheses += text;
        };

        m_DictationRecognizer.DictationComplete += (DictationCompletionCause completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
        };

        m_DictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };

        m_DictationRecognizer.Start();
    }

    public void startDictation() {
        Debug.Log("Starting Dictation");
        m_Hypotheses = "";
        m_Recognitions = "";
        StartRecognizer();
        tempClip = Microphone.Start(null, false, 30, 44100);
    }

    public dictationResponse endDictation() {
        Debug.Log("End Dictation");
        m_DictationRecognizer.Stop();
        Microphone.End(null);

        dictationResponse newResponse = new dictationResponse();
        newResponse.text = m_Recognitions;
        if(m_Recognitions.Length > 0) {
            newResponse.clip = tempClip;
        }
        return newResponse;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)) {
            startDictation();
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            dictationResponse thisResponse = endDictation();
            Debug.Log(thisResponse.text);
            AudioSource thisSource = transform.gameObject.AddComponent<AudioSource>();
            thisSource.clip = thisResponse.clip;
            thisSource.Play();
        }
    }
}