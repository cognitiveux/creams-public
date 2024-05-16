#define PAPER_IS_RUNNING

using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

using UnityEngine.UI;
public struct InformationAboutArtWork
{

    public List<String> description;
    public List<float> videoDimensions;
    public List<String> videosUrl;
    public List<String> audio;
    public int id;
    public List<Sprite> releatedArtworks;
    public Sprite sprite;

    public string name;
    public int page;

    public InformationAboutArtWork(int page)
    {
        description = new List<String>();
        videoDimensions = new List<float>();
        videosUrl = new List<string>();
        releatedArtworks = new List<Sprite>();
        sprite = null;
        name = null;
        audio = new List<String>();
        id = -1;
        this.page = page;
    }
    public void Destroy()
    {
        description.Clear();
        videoDimensions.Clear();
        videosUrl.Clear();
        releatedArtworks.Clear();
        sprite = null;
        name = null;
        audio.Clear();
    }
    public void AddId(int id_par)
    {
        this.id = id_par;
    }
    public void AddAudio(string url)
    {
        audio.Add(url);
    }

    public void SetName(string str)
    {
        this.name = str;
    }

    public void AddVideoUrl(string url)
    {
        videosUrl.Add(url);
    }

    public void AddDimensions(float dim)
    {

        videoDimensions.Add(dim);
    }
    public void AddDescription(string des)
    {
        this.description.Add(des);
    }
    public void AddReleatedArtworks(byte[] fileData)
    {
        Texture2D text = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        text.LoadImage(fileData);
        Sprite sprite = Sprite.Create(text, new Rect(0, 0, text.width, text.height), new Vector2(.5f, .5f));
        releatedArtworks.Add(sprite);
    }

    public void addSprite(byte[] fileData)
    {
        Texture2D text = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        text.LoadImage(fileData);
        sprite = Sprite.Create(text, new Rect(0, 0, text.width, text.height), new Vector2(.5f, .5f));
    }

}
public class PhotoManagerCreator : MonoBehaviour
{
    int sumOfArtworks = 0;

    const int _airtapSumForChangeToTraditional=5;
    private int _airTapCounter = 0;
    private DateTime _lastClickTime;

    public GameObject LLM;
     List<PhotoManager> LLMManangers;
    List<PhotoManager> TraditionalManangers;

    public string LLMJsonUrl;
    public GameObject Traditional;
    List<InformationAboutArtWork> artworks = new List<InformationAboutArtWork>();
    PhotoManager TraditionalMananger;

#if !NOT_STRING_DEBUGGING
    string json_ans = "{\n  \"message\": \"Success\",\n  \"resource_obj\": {\n    \"artworks\": [\n      {\n        \"id\": 1,\n        \"name\": \"Mona Lisa by Leonardo da Vinci\",\n        \"src\": \"study_images/Mona Lisa by Leonardo da Vinci.jpg\",\n        \"description\": \"The Mona Lisa, painted by Leonardo da Vinci in the early 16th century, is arguably the most famous artwork in the world. Housed in the Louvre Museum in Paris, it is celebrated for its extraordinary realism, the mastery of its execution, and the enigmatic expression of its subject.\nThe portrait depicts Lisa Gherardini, wife of Florentine merchant Francesco del Giocondo, seated against a vast, imaginary landscape. The subtle gradations of light and shadow, known as sfumato, give the face a lifelike presence and enigmatic appeal. Her smile, often described as elusive or mysterious, has intrigued viewers for centuries. The background, with its winding paths and distant mountains, adds to the painting\'s sense of depth. \nThe Mona Lisa\'s fame is amplified by its rich history of theft and recovery, and the countless stories and hypotheses about the identity and expression of its subject. Da Vinci\'s meticulous attention to detail and innovative techniques have made the Mona Lisa not just a masterpiece of the Renaissance, but a timeless icon of art.\"\n      },\n      {\n        \"id\": 2,\n        \"name\": \"The Starry Night by Vincent van Gogh\",\n        \"src\": \"study_images/The Starry Night by Vincent van Gogh.jpg\",\n        \"description\": \"The Starry Night, created by Vincent van Gogh in 1889, is one of the most recognized pieces in the history of Western art. This masterpiece, currently housed in the Museum of Modern Art in New York, was painted while van Gogh was in an asylum in Saint-Rémy-de-Provence, France.\n The painting depicts a tumultuous sky filled with swirling stars above a quiet, sleeping village, with a large cypress tree in the foreground.\n Van Gogh\'s use of thick, impasto brushstrokes and vibrant colors conveys intense emotion and energy, a hallmark of his style.\n The Starry Night is often interpreted as reflecting van Gogh\'s state of mind during his stay at the asylum, showcasing his struggle with despair while also conveying a sense of hope through the bright stars. The painting\'s dynamic, flowing lines and the contrast between the calm village and the dramatic sky have captivated viewers and artists alike, making it a symbol of the power of imagination and creativity. Van Gogh\'s innovative technique and the emotional depth of The Starry Night have contributed significantly to the foundations of modern art.\"\n      },\n      {\n        \"id\": 3,\n        \"name\": \"The Last Supper by Leonardo da Vinci\",\n        \"src\": \"study_images/The Last Supper by Leonardo da Vinci.jpg\",\n        \"description\": \"\\\"The Last Supper,\\\" completed by Leonardo da Vinci in the late 15th century, is a monumental mural painting that covers the back wall of the dining hall at the convent of Santa Maria delle Grazie in Milan, Italy.\n This masterpiece depicts the dramatic moment from the New Testament when Jesus announces that one of his Twelve Apostles would betray him. Da Vinci\'s composition masterfully captures the varied emotional reactions of each apostle through their expressions and gestures, organized into groups of three on either side of Christ.\n Unlike traditional depictions, da Vinci chose to portray this significant biblical scene as a dynamic, unfolding event, emphasizing the psychological intensity and human drama. The use of perspective leads the viewer\'s attention directly to the central figure of Jesus, whose calm demeanor contrasts with the turmoil among the apostles.\n Over the years, \\\"The Last Supper\\\" has suffered from deterioration, yet remains one of the most studied and revered paintings in the world, showcasing da Vinci\'s genius in composition, emotional depth, and use of space.\"\n      },\n      {\n        \"id\": 4,\n        \"name\": \"Guernica by Pablo Picasso\",\n        \"src\": \"study_images/Guernica by Pablo Picasso.jpg\",\n        \"description\": \"\\\"Guernica,\\\" painted by Pablo Picasso in 1937, is a powerful political statement and one of the most moving anti-war artworks in history. Created in response to the Nazi German bombing of Guernica, a Basque Country village in northern Spain, during the Spanish Civil War, the mural-sized oil painting is a complex tableau of suffering, depicting the tragedies of war and the innocent lives it impacts.\n The monochromatic palette of black, white, and grays serves to emphasize the starkness of the scene and the universal nature of its message. Key elements within the painting, such as the grieving woman holding her dead child, the bull, and the dismembered soldier, convey a sense of chaos and agony. Picasso\'s use of Cubist techniques and disjointed space adds to the painting\'s emotional intensity, making it a timeless reminder of the horrors of war.\n \\\"Guernica\\\" has not only become an iconic symbol of peace and a protest against war but also a pivotal work in Picasso\'s oeuvre, showcasing his ability to merge form, content, and politics.\"\n      },\n      {\n        \"id\": 5,\n        \"name\": \"The Scream by Edvard Munch\",\n        \"src\": \"study_images/The Scream by Edvard Munch.jpg\",\n        \"description\": \"\\\"The Scream\\\" is one of the most iconic and recognizable paintings in the art world, created by Norwegian artist Edvard Munch in 1893. \n This expressionist masterpiece conveys profound emotional content, capturing a figure standing against a backdrop of a blood-red sky, holding its head in despair while screaming. The swirling sky and the curvilinear lines of the landscape contribute to the painting\'s intense emotional effect, creating a sense of movement and turmoil. Munch\'s work is often seen as a representation of the anxiety and existential dread of the modern human condition. The figure\'s distorted facial expression and the vibrant colors amplify the feeling of anguish and disorientation.\n \\\"The Scream\\\" has been interpreted in various ways, including as a symbol of the individual\'s struggle with identity, the pressures of society, and the artist\'s personal inner turmoil.\n It exists in several versions, including painted and pastel. This piece has left a lasting impact on the art world, influencing the development of expressionism in Europe and serving as an enduring symbol of human emotion and psychological depth.\"\n      },\n      {\n        \"id\": 6,\n        \"name\": \"Girl with a Pearl Earring by Johannes Vermeer\",\n        \"src\": \"study_images/Girl with a Pearl Earring by Johannes Vermeer.jpg\",\n        \"description\": \"\\\"Girl with a Pearl Earring,\\\" also known as the \\\"Dutch Mona Lisa,\\\" is a renowned 17th-century painting by Johannes Vermeer.\n This exquisite piece is celebrated for its simplicity and the mystery surrounding the identity of the subject. The painting portrays a young girl, turning over her shoulder to look at the viewer, with her lips slightly parted as if caught in a moment of silent communication. The most striking feature is the luminous pearl earring, which, alongside the girl\'s soft gaze, draws the viewer\'s attention. \n Vermeer\'s mastery of light and shadow is evident in the delicate rendering of the girl\'s face and the pearl\'s soft glow, creating a sense of depth and realism. The dark background enhances the subject\'s luminescence, focusing all attention on her.\n The painting\'s allure is amplified by its enigmatic qualities, including the girl\'s identity and her expression\'s subtle complexity. Housed in the Mauritshuis museum in The Hague, this masterpiece is a prime example of Dutch Golden Age painting, showcasing Vermeer\'s skill in capturing the beauty of ordinary life and elevating it to the extraordinary.\"\n      },\n      {\n        \"id\": 7,\n        \"name\": \"The Birth of Venus by Sandro Botticelli\",\n        \"src\": \"study_images/The Birth of Venus by Sandro Botticelli.jpg\",\n        \"description\": \"\\\"The Birth of Venus\\\" is a seminal work of the Italian Renaissance, painted by Sandro Botticelli in the mid-1480s.\n This iconic painting depicts the mythological story of Venus, the Roman goddess of love and beauty, emerging from the sea as a fully grown woman. According to mythology, Venus was born from the sea foam and transported to the shore on a giant scallop shell, symbolizing divine beauty and purity. Botticelli\'s masterpiece is renowned for its extraordinary grace and the fluidity of its figures. The goddess stands in the center, her hair flowing in the wind, modestly covering herself with her hands, while the figures of Zephyrus, the god of the west wind, and Aura, the personification of a lighter breeze, gently blow her towards the shore. On the right, one of the Horae, goddesses of the seasons, is ready to clothe her with a richly embroidered mantle. The painting\'s use of line and form, along with its ethereal quality and the idealized beauty of Venus, exemplifies the Renaissance fascination with classical antiquity.\n Housed in the Uffizi Gallery in Florence, \\\"The Birth of Venus\\\" remains a timeless celebration of beauty, love, and the arts.\"\n      },\n      {\n        \"id\": 8,\n        \"name\": \"The Night Watch by Rembrandt van Rijn\",\n        \"src\": \"study_images/The Night Watch by Rembrandt van Rijn.jpg\",\n        \"description\": \"\\\"The Night Watch,\\\" completed in 1642 by Rembrandt van Rijn, is one of the most famous works of the Dutch Golden Age. This large painting is officially titled \\\"Militia Company of District II under the Command of Captain Frans Banninck Cocq,\\\" but it is popularly known as \\\"The Night Watch\\\" due to its perceived nighttime setting, a misconception corrected by later cleanings which revealed a daytime scene. The painting is celebrated for its dramatic use of light and shadow (chiaroscuro) and the perception of motion, setting it apart from the static formality typical of militia group portraits of the time.\n Rembrandt masterfully depicts a dynamic moment as the company prepares to march out, with Captain Cocq and his lieutenant illuminated in the foreground, leading the viewer\'s eye into the scene\'s depth. The intricate composition and the lively expressions of the figures showcase Rembrandt\'s skill in capturing individual characters and the collective energy of the group. \nHoused in the Rijksmuseum in Amsterdam, \\\"The Night Watch\\\" remains a masterpiece of baroque art, reflecting the civic pride and military spirit of Amsterdam\'s citizens during the 17th century.\"\n      },\n      {\n        \"id\": 9,\n        \"name\": \"Water Lilies by Claude Monet\",\n        \"src\": \"study_images/Water Lilies by Claude Monet.jpg\",\n        \"description\": \"\\\"Water Lilies\\\" is a series of approximately 250 oil paintings by French Impressionist artist Claude Monet. The series, created during the last 30 years of Monet\'s life, focuses on the water lily pond in his garden at Giverny.\nThese paintings are celebrated for their exploration of light, reflection, and the natural beauty of the pond environment. Monet\'s fascination with the subject led him to capture the pond\'s surface in various conditions, from dawn to dusk, highlighting the changing light and its effects on color. The \\\"Water Lilies\\\" series is a testament to Monet\'s mastery of color theory and his innovative approach to capturing the transient moments of nature.\nThe paintings vary in size, some spanning over six meters in width, designed to envelop the viewer in the scene. This immersive experience was further realized in Monet\'s design of the Orangerie Museum in Paris, where panels of Water Lilies are displayed in two oval rooms, creating a continuous panorama. The series not only epitomizes Impressionist techniques of outdoor painting and the play of light but also marks a transition towards the abstract, as Monet emphasized the harmonious interplay of light, water, and the lily pads over realistic representation.\"\n      },\n      {\n        \"id\": 10,\n        \"name\": \"The Persistence of Memory by Salvador Dali\",\n        \"src\": \"study_images/The Persistence of Memory by Salvador Dali.jpg\",\n        \"description\": \"\\\"The Persistence of Memory,\\\" painted by Salvador Dalí in 1931, is one of the most famous and recognizable works of Surrealism. This small canvas, measuring just 24 x 33 cm, depicts a dream-like landscape where time appears to melt away, represented by several soft, melting pocket watches. The setting is a barren landscape, reminiscent of Dalí\'s home in Catalonia, under a calm sky. One of the watches drapes over the edge of a table, another hangs from a dead tree, and a third covers a strange, amorphous face that may be a self-portrait of Dalí himself, suggesting the fluidity and distortion of time in the dream state. The precise rendering and the use of familiar objects in an unfamiliar context are hallmarks of Dalí\'s work, inviting viewers to explore the subconscious and the irrational world of dreams. \\\"The Persistence of Memory\\\" challenges the viewer\'s conventional perceptions of reality and time, embodying the essence of Surrealism by blending the bizarre with the meticulous. Housed in the Museum of Modern Art (MoMA) in New York, this masterpiece continues to fascinate and perplex audiences with its enigmatic themes and technical brilliance.\"\n      }\n    ]\n  }\n}";
#endif
    public static string media_base_url = "https://creams-poc2.cognitiveux.net/media/";
    public static string media_base_url2 = "https://creams-poc2.cognitiveux.net";
    public static string associatedMediaUrl = "https://creams-poc2.cognitiveux.net/web_app/artworks/associated_media?artwork_id=";
    private int photoIndex = 0;
    public GameObject[] AiChildren;

    public string TraditionalJsonUrl= "https://creams-poc2.cognitiveux.net/web_app/artworks/all?format=json";

    public void AirTapDetected()
    {
        TimeSpan timeDifference = DateTime.Now - _lastClickTime;

        if(timeDifference.TotalSeconds>2)
        {
            _airTapCounter = 0;
        }
        _airTapCounter++;
        if (_airTapCounter == _airtapSumForChangeToTraditional)
            InitializeTraditional();
        _lastClickTime = DateTime.Now;
    }
    public static Interactable ReturnButtonByName(string str)
    {
        Interactable[] buttons=Resources.FindObjectsOfTypeAll<Interactable>();
        foreach (Interactable button in buttons)
            if (button.name == str)
                return button;
        return null;
    }
    public IEnumerator GetAssetsFromServer(string json_url)
    {
        JObject jsonToArray;
        int counter = 0;
        int downloaded = 0;
#if NOT_STRING_DEBUGGING
        UnityWebRequest request_json = UnityWebRequest.Get(json_url);
        yield return request_json.SendWebRequest();

        if (request_json.isNetworkError)
        {
            throw new Exception("net error");
        }
        else
        {
            string json = request_json.downloadHandler.text;
            jsonToArray = JObject.Parse(json);
            Debug.Log(request_json.downloadHandler.text);

        }
#else
        jsonToArray = JObject.Parse(json_ans);
#endif
        if (jsonToArray == null)
            Debug.Log("Null file");

        int jsonCount = 0;
        if (jsonToArray == null)
        {
            throw new Exception("The requested json was converted  to array (Jobject) and is null");
        }

        foreach (JObject item in jsonToArray["resource_obj"]["artworks"])
        {
            jsonCount++;
        }


        if (jsonToArray == null)
        {
            throw new Exception("The requested json was converted  to array (Jobject) and is null");
        }

        foreach (JObject item in jsonToArray["resource_obj"]["artworks"])
        {
            counter++;
            downloaded++;

            string requesturl = media_base_url + item["src"];
            Debug.Log(requesturl);
            int id = ((int)item["id"]);
            string filename = ((string)(item["src"])).Split('/')[1];
            Debug.Log("Downloading:" + filename);

            InformationAboutArtWork tmp = new InformationAboutArtWork(0);
            tmp.SetName(filename);


            UnityWebRequest request = UnityWebRequest.Get(requesturl);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(requesturl);
                Debug.LogError("Error downloading file: " + request.error);
                continue;
            }
            else
            {
                byte[] fileData = request.downloadHandler.data;
                tmp.addSprite(fileData);
            }

            tmp.AddDescription(item["description"].ToString());
            tmp.AddId((int)item["id"]);
            artworks.Add(tmp);
            sumOfArtworks++;
        }
        InitializeAI();

    }
    private Text _findTextByNameAndCanvas(GameObject parent,string canvasName, string textName)
    {
        Canvas[] canvasses= parent.GetComponentsInChildren<Canvas>();
        Canvas target=null;
        foreach (Canvas canvas in canvasses)
            if (canvas.name == canvasName)
            {
                target = canvas;
                break;
            }
        if (target == null)
            throw new Exception("target canvas was not found in decestors");
        Text[] texts = target.gameObject.GetComponentsInChildren<Text>(true);

        foreach (Text text in texts)
        {
            if (text.name == textName)
            {
                return text;

            }
        }
        throw new Exception("text not found");
        return null;
    }
    public void InitializeAI()
    {

        if (LLM == null)
            throw new System.Exception("LLM not found");
        LLM.SetActive(true);
        Traditional.SetActive(false);

        LLMManangers = new List<PhotoManager>();

        for(int i=0;i<LLM.transform.childCount;i++)
        {
            Transform child = LLM.transform.GetChild(i);
            Debug.Log("Initializing " + child.name);
            LLMManangers.Add(child.gameObject.AddComponent<PhotoManager>());
            Text descriptionText=_findTextByNameAndCanvas(child.gameObject, PhotoManager.textCanvasName, "Text");
            Text messenger = _findTextByNameAndCanvas(child.gameObject, PhotoManager.messageExhangeCanvasName, "Text");
            LLMManangers[i].InitializeAI(child.gameObject, descriptionText,messenger,artworks[i].name);
            LLMManangers[i].SetPhoto(artworks[i].sprite);
            string description = "";
            if (artworks[i].description.Count > 0)
                description = artworks[i].description[0];
            LLMManangers[i].SetDescription(description);

        }
    }

    public void InitializeTraditional()
    {

        if (Traditional == null)
            throw new System.Exception("LLM not found");
        LLM.SetActive(false);
        Traditional.SetActive(true);
        TraditionalManangers = new List<PhotoManager>();
        int offsetInList = 5;
        for (int i = 0; i < Traditional.transform.childCount; i++)
        {
            Transform child = Traditional.transform.GetChild(i);
            Debug.Log("Initializing " + child.name);
            TraditionalManangers.Add(child.gameObject.AddComponent<PhotoManager>());
            Text text = child.GetComponentInChildren<Text>();
            if (text == null)
                throw new Exception("text was not found");
            TraditionalManangers[i].InitializeTraditional(child.gameObject, text, artworks[i + offsetInList].id,artworks[i+offsetInList].name);
            TraditionalManangers[i].SetPhoto(artworks[i+ offsetInList].sprite);
            string description = "";
            if (artworks[i + offsetInList].description.Count > 0)
                description = artworks[i + offsetInList].description[0];
            TraditionalManangers[i].SetDescription(description);

        }
    }


    void Start()
    {
        if (LLM == null)
            throw new System.Exception("LLM not found");
        StartCoroutine(GetAssetsFromServer(TraditionalJsonUrl));
    }

    void Update()
    {

    }
}
