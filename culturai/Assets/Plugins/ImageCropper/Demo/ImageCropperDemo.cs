using System.Collections;
using UnityEngine;
using UnityEngine.UI;


	public class ImageCropperDemo : MonoBehaviour
	{
		public  static RawImage croppedImageHolder;
		public static Text croppedImageSize;
	public Text debugText = null;

	public Toggle ovalSelectionInput, autoZoomInput;
		public InputField minAspectRatioInput, maxAspectRatioInput;
	public  void test()
    {
		debugText.text += "test function ran\n";
		Debug.Log("palm up");
    }
	void Start()
    {
		//	croppedImageHolder = 
		//croppedImageSize = gameObject.AddComponent<Text>();

	}
	void Update()
    {
        
    }
   public void Crop()
		{
		ImageCropper.debugText = debugText;

		// If image cropper is already open, do nothing
		if (ImageCropper.Instance.IsOpen)
			{
				Debug.Log("returned image cropper");
				return;
			}
		Debug.Log("Starting coroutine crop");
		debugText.text += "Calling coroutine for cropper\n";
		
			 StartCoroutine(TakeScreenshotAndCrop()) ;
		}

		IEnumerator TakeScreenshotAndCrop()
		{

		debugText.text += "Inside coroutine for cropper\n";

		//yield return new WaitForSeconds(20);
		debugText.text += "woken in coroutine for cropper\n";
		Debug.Log("Inside corouting image cropping");

		yield return new WaitForEndOfFrame();

			Debug.Log("Continuing");

			bool ovalSelection = false;//ovalSelectionInput.isOn;
			bool autoZoom = true;// autoZoomInput.isOn;

			float minAspectRatio, maxAspectRatio;
		//	if( !float.TryParse( minAspectRatioInput.text, out minAspectRatio ) )
				minAspectRatio = 0f;
		//	if( !float.TryParse( maxAspectRatioInput.text, out maxAspectRatio ) )
				maxAspectRatio = 0f;

			Texture2D screenshot = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
			screenshot.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0 );
			screenshot.Apply();
			Debug.Log("continuing...");
		debugText.text += "Calling function\n";

		ImageCropper.Instance.Show( screenshot, ( bool result, Texture originalImage, Texture2D croppedImage ) =>
			{ 
				debugText.text += "Called function\n";

				// Destroy previously cropped texture (if any) to free memory
				Destroy( croppedImageHolder.texture, 5f );

				// If screenshot was cropped successfully
				if( result )
				{
					Debug.Log("cropped");

					// Assign cropped texture to the RawImage
					croppedImageHolder.enabled = true;
					croppedImageHolder.texture = croppedImage;

					Vector2 size = croppedImageHolder.rectTransform.sizeDelta;
					if( croppedImage.height <= croppedImage.width )
						size = new Vector2( 400f, 400f * ( croppedImage.height / (float) croppedImage.width ) );
					else
						size = new Vector2( 400f * ( croppedImage.width / (float) croppedImage.height ), 400f );
					croppedImageHolder.rectTransform.sizeDelta = size;

					//croppedImageSize.enabled = true;
				//	croppedImageSize.text = "Image size: " + croppedImage.width + ", " + croppedImage.height;
				}
				else
				{
					croppedImageHolder.enabled = false;
				//	croppedImageSize.enabled = false;
				}

				// Destroy the screenshot as we no longer need it in this case
				Destroy( screenshot );
			},
			settings: new ImageCropper.Settings()
			{
				ovalSelection = ovalSelection,
				autoZoomEnabled = autoZoom,
				imageBackground = Color.clear, // transparent background
				selectionMinAspectRatio = minAspectRatio,
				selectionMaxAspectRatio = maxAspectRatio

			},
			croppedImageResizePolicy: ( ref int width, ref int height ) =>
			{
				// uncomment lines below to save cropped image at half resolution
				//width /= 2;
				//height /= 2;
			} );
		debugText.text += "Done\n";

	}
}
