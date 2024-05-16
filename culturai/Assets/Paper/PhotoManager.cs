using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
/// <summary>
/// Class <c>PhotoManager</c> Downloads the images for the quiz and some description.
/// </summary>
public class PhotoManager : MonoBehaviour
{
    List<String> conversationHistory=new List<String>();
    private GameObject _image;
    public GameObject ScrolledGameObject;
    private Text Description;
    string name;
    private Text Messenger;
    const int fontMinSize = 14;
    const int fontMaxSize = 25;
    private Text LLMConversation;
    bool ai=false;
    public const string recordButtonName = "RecordV";
    const string resetButtonName = "Reset";
    public const string stopRecordButtonName = "StopRecord";
    const int maxSecondsThatCouldBeRecorded = 20;
    static bool isBeingRecorded = false;
    string artWorkNameThatIsBeingDisplayed = null;
    int id;
    public const string imageCanvasName = "Image";
    public const string textCanvasName = "Conversation";
    const string buttonForBiggerTextFontName = "BiggerTextSize";
    const string buttonForSmallerTextFontName = "SmallerTextSize";
    public const string messageExhangeCanvasName = "MessageExchange";
    const string questionColorFont = "<color=#FFFFFF>";
    const string answerColorFont = "<color=#FFFFFF>";


    public void SetContent(GameObject content)
    {
        _image = content;

    }


    public void SetPhoto(Sprite parameter)
    {

        if (_image == null)
            throw new Exception("_CONTENT NOT FOUND");
        Image img=null;
        Image[] images=_image.GetComponentsInChildren<Image>();
        foreach (Image tmp in images)
            if (tmp.name == "Image")
            {
                img = tmp;break;
            }

        if (img == null)
            throw new Exception("Image not found ");
        if (img.transform.name != "Image")
            throw new Exception("Wrong image");

        img.sprite = parameter;
    }
    bool fontSizeCanDecrease(Text text)
    {
        return !(text.fontSize == fontMinSize);

    }
    bool fontSizeCanIncrease(Text text)
    {
        return !(text.fontSize == fontMaxSize);

    }
    public void IncreaseTextSizeInDescription()
    {
        if (Description == null)
            throw new Exception("Description component not found");
        if (!fontSizeCanIncrease(Description))
            return;
        Description.fontSize++;
    }
    public void DecreaseTextSizeInDescription()
    {
        if (Description == null)
            throw new Exception("Description component not found");
        if (!fontSizeCanDecrease(Description))
            return;
        Description.fontSize--;
    }

    public void IncreaseTextSizeInMessageExchange()
    {
        if (Messenger == null)
            throw new Exception("Description component not found");
        if (!fontSizeCanIncrease(Messenger))
        return;
        Messenger.fontSize++;
    }
    public void DecreaseTextSizeInMessageExchange()
    {
        if (Messenger == null)
            throw new Exception("Description component not found");
        if (!fontSizeCanDecrease(Messenger))
            return;
        Messenger.fontSize--;
    }

    public void SetDescription(string description)
    {
        Description.color = Color.white;
        Description.text = description;
        ScrolledGameObject = Description.transform.parent.gameObject;
    }
    public void SetQuestionAndAnswer(string question,string answer,bool save)
    {
        if (save)
        {
            conversationHistory.Add(question);
            conversationHistory.Add(answer);
        }
        if (!save)
        {
            Messenger.text = "" + questionColorFont + question + "</color>\n" + questionColorFont + answer + "</color>" + "\n";
        }
        else
        {
            Messenger.text = "";
            if (conversationHistory.Count % 2 != 0)
                throw new Exception("conversation history issue");
            for (int i = 0; i < conversationHistory.Count; i += 2)
            {
                string tmpQuestion = conversationHistory[i];
                string tmpAnswer = conversationHistory[i + 1];
                Messenger.text = "" + questionColorFont + tmpQuestion + "</color>\n" + questionColorFont + tmpAnswer + "</color>" + "\n";

            }
        }
    }


    private void _antiScroll()
    {
        if (_getYOfScroll() < 0)
            _setYOfScroll(0);

    }

    private float _getYOfScroll()
    {
        if (ScrolledGameObject == null) throw new Exception("set ScrolledGameObject");
        return ScrolledGameObject.transform.localPosition.y;
    }
    private void _setYOfScroll(float y)
    {
        if (ScrolledGameObject == null) throw new Exception("set ScrolledGameObject");
         ScrolledGameObject.transform.localPosition=new Vector3(ScrolledGameObject.transform.localPosition.x,y, ScrolledGameObject.transform.localPosition.z);

    }
    public void ResetScroll()
    {
        _setYOfScroll(0);
    }
    void ResetScrollMessageExchange()
    {

        Messenger.transform.parent.localPosition = new Vector3(Messenger.transform.parent.localPosition.x, 0, Messenger.transform.parent.localPosition.z);

    }
    void ResetScrollDescription()
    {
        Description.transform.parent.localPosition = new Vector3(Description.transform.parent.localPosition.x,0, Description.transform.parent.localPosition.z);
    }
    public void InitializeAI(GameObject content,Text descriptionText,Text messegeExchangeText,string name)
    {
        ai = true;
        this.name = name;
        this.Description = descriptionText;
        this.Messenger = messegeExchangeText;
        Messenger.text = "";
        SetContent(content);
        AddListenersOnButton();
        PlaceButtons();

    }
    public int getId()
    {
        return this.id;
    }
    public string GetName()
    {
        return this.name;
    }
    public void InitializeTraditional(GameObject content, Text text,int id,string name)
    {
        this.name = name;
        this.id = id;
        this.Description = text;
        SetContent(content);
        PlaceButtons();
        AddListenersOnButton();

    }
    public void AddListenersOnButton()
    {

        if (ai)
        {
            Interactable recordButton = _findButtonInteractableInChildremByName(recordButtonName);
            if (recordButton == null)
                throw new Exception("nextButton not found");
            recordButton.OnClick.AddListener(Record);

            Interactable stopRecordButton = _findButtonInteractableInChildremByName(stopRecordButtonName);
            if (stopRecordButton == null)
                throw new Exception("nextButton not found");
            stopRecordButton.OnClick.AddListener(Stop);
        }
        Canvas descriptionCanvas = returnCanvasWithName(textCanvasName);
        Canvas conversationCanvas = returnCanvasWithName(messageExhangeCanvasName);
        if (descriptionCanvas == null || (conversationCanvas == null && ai))
            throw new Exception("descriptionCanvas == null || conversationCanvas==null");

            Interactable biggerFontSizeFromDescription = _findButtonInteractableInChildremByName(descriptionCanvas,buttonForBiggerTextFontName);
        Interactable smallerFontSizeFromDescription = _findButtonInteractableInChildremByName(descriptionCanvas, buttonForSmallerTextFontName);
        if (ai)
        {
            Interactable biggerFontSizeFromConversation = _findButtonInteractableInChildremByName(conversationCanvas, buttonForBiggerTextFontName);
            Interactable smallerFontSizeFromConversation = _findButtonInteractableInChildremByName(conversationCanvas, buttonForSmallerTextFontName);
            if( biggerFontSizeFromConversation == null || smallerFontSizeFromConversation == null) throw new Exception("resetFromConversation button(s) were not found");
            biggerFontSizeFromConversation.OnClick.AddListener(IncreaseTextSizeInMessageExchange);
            smallerFontSizeFromConversation.OnClick.AddListener(DecreaseTextSizeInMessageExchange);
            Interactable resetFromConversation = _findButtonInteractableInChildremByName(conversationCanvas, resetButtonName);
            if (resetFromConversation == null )
            {
                throw new Exception("resetFromConversation button(s) were not found");
            }
            resetFromConversation.OnClick.AddListener(ResetScrollMessageExchange);


        }

        if (biggerFontSizeFromDescription == null || smallerFontSizeFromDescription == null )
            throw new Exception("font size button(s) were not found");

        biggerFontSizeFromDescription.OnClick.AddListener(IncreaseTextSizeInDescription);
        smallerFontSizeFromDescription.OnClick.AddListener(DecreaseTextSizeInDescription);
        Interactable resetFromDescription = _findButtonInteractableInChildremByName(descriptionCanvas, resetButtonName);
        if (resetFromDescription == null)
            throw new Exception("resetFromConversation button(s) were not found");

        resetFromDescription.OnClick.AddListener(ResetScrollDescription);

    }
    void AddManipulationEvents()
    {
        ManipulationHandler handler= transform.parent.GetComponentInChildren<ManipulationHandler>();
        if(handler==null)
        {
            throw new Exception("handler not found");
        }

        handler.OnManipulationEnded.AddListener(ManipulationEndedEvent);

    }
    public void ManipulationEndedEvent(ManipulationEventData arg)
    {
        ManipulationHandler handler = transform.parent.GetComponentInChildren<ManipulationHandler>();
        if (handler == null)
        {
            throw new Exception("handler not found");
        }
        Transform handlerParent = handler.transform.parent;
        foreach(Canvas canvas in handlerParent.gameObject.GetComponentsInChildren<Canvas>())
        {
            if(canvas.gameObject!= handler.gameObject)
            {
                canvas.transform.localRotation = handler.transform.localRotation;
            }
        }
    }
    private Interactable _findButtonInteractableInChildremByName(string name)
    {
        Interactable[] buttons = gameObject.GetComponentsInChildren<Interactable>(true);
        foreach (Interactable button in buttons)
        {
            if (button.name == name)
            {
                return button;

            }
        }
        return null;
    }

    private Interactable _findButtonInteractableInChildremByName(Canvas canvas,string name)
    {
        Interactable[] buttons = canvas.gameObject.GetComponentsInChildren<Interactable>(true);
        foreach (Interactable button in buttons)
        {
            if (button.name == name)
            {
                return button;

            }
        }
        return null;
    }
    public void Record()
    {
        if (isBeingRecorded)
            return;
        isBeingRecorded = true;
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, maxSecondsThatCouldBeRecorded, 44100);
        _findButtonInteractableInChildremByName(recordButtonName).gameObject.SetActive(false);
        _findButtonInteractableInChildremByName(stopRecordButtonName).gameObject.SetActive(true);

        if (gameObject.GetComponent<AiChatManager>() == null)
        {
            gameObject.AddComponent<AiChatManager>();
           gameObject.GetComponent<AiChatManager>().SetMananger(this);
        }
        gameObject.GetComponent<AiChatManager>().SetIsBeingRecordedText();

    }
    public void Stop()
    {
        Microphone.End(null);
        AudioSource audioSource = GetComponent<AudioSource>();

        _findButtonInteractableInChildremByName(recordButtonName).gameObject.SetActive(true);
        _findButtonInteractableInChildremByName(stopRecordButtonName).gameObject.SetActive(false);
        isBeingRecorded = false;

        StartCoroutine(GetComponent<AiChatManager>().SendAudio(SavWav.Save(audioSource.clip)));
    }
    Canvas returnCanvasWithName(string name)
    {
        Canvas[] canvases = gameObject.GetComponentsInChildren<Canvas>(true);
        foreach (Canvas canvas in canvases)
        {
            if (canvas.name == name)
            {
                return canvas;

            }
        }
        return null;
    }
    public void PlaceButtons()
    {
        Canvas canvasOfImage = returnCanvasWithName(imageCanvasName);
        if (canvasOfImage == null)
            throw new Exception("image canvas is null");
        Canvas textCanvas= returnCanvasWithName(textCanvasName);
        if (textCanvas == null)
            throw new Exception("textCanvas canvas is null");
        if (ai)
        {
            Canvas messageExhangeCanvas = returnCanvasWithName(messageExhangeCanvasName);
            if (messageExhangeCanvas == null)
                throw new Exception("messageExhangeCanvas canvas is null");
            SetButtonPosition(messageExhangeCanvas, new string[] { }, new string[] { buttonForBiggerTextFontName, buttonForSmallerTextFontName,resetButtonName }, null);
        }

        SetButtonPosition(textCanvas, new string[] {}, new string[] {buttonForBiggerTextFontName,buttonForSmallerTextFontName,resetButtonName}, null);
    }
    /// <summary>
    /// Method <c>SetButtonPosition</c> sets dynamically buttons positions on the canvas .
    /// </summary>
    public static void SetButtonPosition(Canvas canvasParameter, string[] includeRight, string[] includeTop, string[] includeBottomRightToLeft)
    {
        RectTransform transform = canvasParameter.GetComponent<RectTransform>();

        if (canvasParameter == null)
            throw new System.Exception("GridElement:SetButtonPosition:canvas parameter is null");
        float offsetXTop = 0;
        float offsetYTop = 0;
        float offsetXBottomRight = transform.rect.xMax;
        float offsetYBottomRight = transform.rect.yMin;
        offsetXTop = transform.rect.xMin;
        offsetYTop = -transform.rect.yMin;
        float offsetXRight = transform.rect.xMax;
        float offsetYRight = transform.rect.yMax;

        PhysicalPressEventRouter[] buttonsPhysicalPressEventRouterComponent = canvasParameter.gameObject.GetComponentsInChildren<PhysicalPressEventRouter>(true);

        bool placedRight = false;
        bool placedXAxis = false;
        bool placedBottomRight = false;
        foreach (PhysicalPressEventRouter element in buttonsPhysicalPressEventRouterComponent)
        {
            Debug.Log(element.name);
            if (element.name == PhotoManager.recordButtonName || element.name == PhotoManager.stopRecordButtonName)
                continue;
            GameObject elementGameObject = element.gameObject;
            bool shouldBeIncludedRight = false;
            bool shouldBeIncludedTop = false;
            bool shouldBeIncludedBottmRight = false;
            if (includeBottomRightToLeft != null)
                foreach (string str in includeBottomRightToLeft)
                {
                    if (str == elementGameObject.name)
                    {
                        shouldBeIncludedBottmRight = true;
                        break;
                    }
                }
            if (includeRight != null)
                foreach (string str in includeRight)
                {
                    if (str == elementGameObject.name)
                    {
                        shouldBeIncludedRight = true;
                        break;
                    }

                }
            if (includeTop != null)
                foreach (string str in includeTop)
                {
                    if (str == elementGameObject.name)
                    {
                        shouldBeIncludedTop = true;
                        break;
                    }
                }
            if (!shouldBeIncludedRight && !shouldBeIncludedTop && !shouldBeIncludedBottmRight)
            {
                elementGameObject.SetActive(false);
                continue;
            }

            elementGameObject.SetActive(true);
            elementGameObject.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);

            if (shouldBeIncludedTop)
            {
                elementGameObject.GetComponent<Transform>().localPosition = new Vector3(offsetXTop + elementGameObject.GetComponent<BoxCollider>().size.x / 2 * elementGameObject.GetComponent<Transform>().localScale.x, offsetYTop + elementGameObject.GetComponent<BoxCollider>().size.y / 2 * elementGameObject.GetComponent<Transform>().localScale.y, 0);


                offsetXTop += elementGameObject.GetComponent<BoxCollider>().size.x * elementGameObject.GetComponent<Transform>().localScale.x;

            }
            else if (shouldBeIncludedRight)
            {
                if (!placedRight)
                {
                    offsetYRight -= elementGameObject.GetComponent<BoxCollider>().size.y * elementGameObject.GetComponent<Transform>().localScale.y;
                    placedRight = true;
                }
                elementGameObject.GetComponent<Transform>().localPosition = new Vector3(offsetXRight + elementGameObject.GetComponent<BoxCollider>().size.x / 2 * elementGameObject.GetComponent<Transform>().localScale.x, offsetYRight + elementGameObject.GetComponent<BoxCollider>().size.y / 2 * elementGameObject.GetComponent<Transform>().localScale.y, 0);
                offsetYRight -= elementGameObject.GetComponent<BoxCollider>().size.y * elementGameObject.GetComponent<Transform>().localScale.y;
            }
            else if (shouldBeIncludedBottmRight)
            {
                if (placedBottomRight == false)
                {
                    placedBottomRight = true;
                    offsetXBottomRight -= elementGameObject.GetComponent<BoxCollider>().size.x * elementGameObject.GetComponent<Transform>().localScale.x;
                    offsetYBottomRight -= elementGameObject.GetComponent<BoxCollider>().size.y * elementGameObject.GetComponent<Transform>().localScale.y;
                }
                elementGameObject.GetComponent<Transform>().localPosition = new Vector3(offsetXBottomRight + elementGameObject.GetComponent<BoxCollider>().size.x / 2 * elementGameObject.GetComponent<Transform>().localScale.x, offsetYBottomRight + elementGameObject.GetComponent<BoxCollider>().size.y / 2 * elementGameObject.GetComponent<Transform>().localScale.y, 0);
                offsetXBottomRight -= elementGameObject.GetComponent<BoxCollider>().size.x * elementGameObject.GetComponent<Transform>().localScale.x;
            }
        }

    }
    public void PlaceButtonsRegular()
    {

    }

    void Start()
    {
        AddManipulationEvents();
    }

    void Update()
    {
        if (ScrolledGameObject!=null && _getYOfScroll() < 0)
            _setYOfScroll(0);
    }
}
