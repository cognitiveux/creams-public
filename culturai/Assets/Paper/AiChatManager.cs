//#define NOT_API
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class AiChatManager : MonoBehaviour
{
    private static string[] _messages = { "Your are being recorded.", "Your are being recorded..", "Your are being recorded...", "Your message is sent.Please wait. ", "Your message is sent.Please wait..", "Your message is sent.Please wait..","You have been ","Please confirm that your speech was recognized correctly.", "Please wait your answer is being generated.", "Please wait your answer is being generated..", "Please wait your answer is being generated..." };
    string SpeechToText;
    public PhotoManager mananger;
    int isBeingRecorded=0;
    int messageIsBeingSent=0;
    private string _question;
    private float _lastTimeMessageChanged = 0.0f;


    public void SendRequestForAnswer()
    {

    }

    public void SendRequestForConvertionOfSpeechToText()
    {

    }
    public void RecordStream()
    {

    }
    public void SetIsBeingRecordedText()
    {

        if (mananger == null) throw new System.Exception("photo mananger is null");
        _lastTimeMessageChanged = 0;
        if (isBeingRecorded == 0)
            isBeingRecorded = 1;
        mananger.SetQuestionAndAnswer(_messages[isBeingRecorded-1], "",false);
        if (isBeingRecorded<3)
        {
            isBeingRecorded++;
        }

        else
        {
            isBeingRecorded = 1;
        }
    }
    public void StopRecording()
    {

    }
    public void SetTextToQuestionIsSent()
    {
        if (mananger == null) throw new System.Exception("photo mananger is null");
        int messageIndexOfQuestionIsBeingSent = 3;
        _lastTimeMessageChanged = 0;
        if (messageIsBeingSent == 0)
            messageIsBeingSent = 1;

        mananger.SetQuestionAndAnswer(_question, _messages[messageIndexOfQuestionIsBeingSent + messageIsBeingSent-1], false);
        if (messageIsBeingSent % 8 < 3)
        {
            messageIsBeingSent++;
        }
        else
        {
            messageIsBeingSent = 1;
        }
    }
    public void SetStream()
    {

    }

    public IEnumerator SendAudio(byte[] audioBytes)
    {
#if NOT_API
        mananger.SetQuestionAndAnswer("voice question", "answer", true);
        yield return null;
#endif
#if !NOT_API
        isBeingRecorded = 0;
        messageIsBeingSent = 1;

        Debug.Log("sent");
        UnityWebRequest www = UnityWebRequest.Put("https://creams-poc2.cognitiveux.net/web_app/query_gpt/", audioBytes);

        www.SetRequestHeader("Content-Type", "audio/wav");
        string artworkName = mananger.GetName().Replace(" ", "_");
        www.SetRequestHeader("Name-Of-Artwork", artworkName);

        yield return www.SendWebRequest();
        if (www.isNetworkError)
            Debug.Log("network error:"+ www.error);
        messageIsBeingSent = 0;
        string response = www.downloadHandler.text;
        JObject jsonFromAns = JObject.Parse(response);
        string question = jsonFromAns["question"].ToString();
        string answer = jsonFromAns["answer"].ToString();

        Debug.Log("whole response"+response);
        Debug.Log("question:" + question + "answer" + answer);
        mananger.SetQuestionAndAnswer("-"+question, "-"+answer, true);
#endif

    }

    public void AnswerReceived()
    {

    }
    public void ChangeIconOfRecord()
    {

    }
    public void ManipulateText(string text)
    {

    }
    public void SetMananger(PhotoManager parameter)
    {
        this.mananger = parameter;
    }

    void Start()
    {

    }


    void Update()
    {
#if !NOT_API
         _lastTimeMessageChanged += Time.deltaTime;
        if (isBeingRecorded > 0 && _lastTimeMessageChanged>=1)
            SetIsBeingRecordedText();
        if(messageIsBeingSent>0 && _lastTimeMessageChanged >= 1)
        {
            SetTextToQuestionIsSent();
        }
#endif

    }
}
