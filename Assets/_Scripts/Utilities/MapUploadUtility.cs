using SimpleFileBrowser;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;
using B83.Image.BMP;

public class MapUploadUtility : MonoBehaviour
{
    public TMP_InputField _widthText, _heightText, _mapName;
    Texture2D _satelliteTex;
    Texture2D _bitmapImageTex;
    public Image _satelliteImage;
    public Image _bitmapImage;

    public MapData _testMapData; //! debug, remove this in favor of a drop down of all map datas.


    void Start()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png", ".jpeg", ".bmp"), new FileBrowser.Filter("Text Files", ".txt", ".pdf"));
        FileBrowser.SetDefaultFilter(".jpg");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        // Example 1: Show a save file dialog using callback approach
        // onSuccess event: not registered (which means this dialog is pretty useless)
        // onCancel event: not registered
        // Save file/folder: file, Allow multiple selection: false
        // Initial path: "C:\", Initial filename: "Screenshot.png"
        // Title: "Save As", Submit button text: "Save"
        // FileBrowser.ShowSaveDialog( null, null, FileBrowser.PickMode.Files, false, "C:\\", "Screenshot.png", "Save As", "Save" );

        // Example 2: Show a select folder dialog using callback approach
        // onSuccess event: print the selected folder's path
        // onCancel event: print "Canceled"
        // Load file/folder: folder, Allow multiple selection: false
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Select Folder", Submit button text: "Select"
        // FileBrowser.ShowLoadDialog( ( paths ) => { Debug.Log( "Selected: " + paths[0] ); },
        //						   () => { Debug.Log( "Canceled" ); },
        //						   FileBrowser.PickMode.Folders, false, null, null, "Select Folder", "Select" );

        // Example 3: Show a select file dialog using coroutine approach
        // StartCoroutine( ShowLoadDialogCoroutine() );
    }

    public void LoadSatelliteImage()
    {
        StartCoroutine(ShowSatelliteLoadDialogCoroutine());
    }

    IEnumerator ShowSatelliteLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Allow multiple selection: true
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Load File", Submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, true, null, null, "Select Files", "Load");

        // Dialog is closed
        // Print whether the user has selected some files or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
            OnSatelliteFileSelected(FileBrowser.Result); // FileBrowser.Result is null, if FileBrowser.Success is false
    }

    void OnSatelliteFileSelected(string[] filePaths)
    {
        // Print paths of the selected files
        for (int i = 0; i < filePaths.Length; i++)
            Debug.Log(filePaths[i]);

        // Get the file path of the first selected file
        string filePath = filePaths[0];

        // Read the bytes of the first file via FileBrowserHelpers
        // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
        byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(filePath);

        _satelliteTex = new Texture2D(1, 1);

        if (ImageConversion.LoadImage(_satelliteTex, bytes))
        {
            var sprite = Sprite.Create(_satelliteTex, new Rect(0, 0, _satelliteTex.width, _satelliteTex.height), new Vector2(_satelliteTex.width / 2, _satelliteTex.height / 2));
            _satelliteImage.sprite = sprite;
        }
        else
            Debug.Log("image conversion failed");

        _satelliteImage.preserveAspect = true;
        _satelliteImage.rectTransform.sizeDelta = new Vector2(250, 250);


        // Or, copy the first file to persistentDataPath
        string destinationPath = Path.Combine(Application.persistentDataPath, FileBrowserHelpers.GetFilename(filePath));
        FileBrowserHelpers.CopyFile(filePath, destinationPath);
    }

    //--

    public void LoadBitmapImage()
    {
        StartCoroutine(ShowBitmapLoadDialogCoroutine());
    }

    IEnumerator ShowBitmapLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Allow multiple selection: true
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Load File", Submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, true, null, null, "Select Files", "Load");

        // Dialog is closed
        // Print whether the user has selected some files or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
            OnBitmapFileSelected(FileBrowser.Result); // FileBrowser.Result is null, if FileBrowser.Success is false
    }

    void OnBitmapFileSelected(string[] filePaths)
    {
        // Print paths of the selected files
        for (int i = 0; i < filePaths.Length; i++)
            Debug.Log(filePaths[i]);

        // Get the file path of the first selected file
        string filePath = filePaths[0];

        // Read the bytes of the first file via FileBrowserHelpers
        // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
        byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(filePath);

        BMPLoader bmpLoader = new BMPLoader();
        //bmpLoader.ForceAlphaReadWhenPossible = true; //Uncomment to read alpha too

        //Load the BMP data
        BMPImage bmpImg = bmpLoader.LoadBMP(bytes);

        //Convert the Color32 array into a Texture2D
        _bitmapImageTex = bmpImg.ToTexture2D();

        var sprite = Sprite.Create(_bitmapImageTex, new Rect(0, 0, _bitmapImageTex.width, _bitmapImageTex.height), new Vector2(_bitmapImageTex.width / 2, _bitmapImageTex.height / 2));
        _bitmapImage.sprite = sprite;
        _bitmapImage.preserveAspect = true;
        _bitmapImage.rectTransform.sizeDelta = new Vector2(250, 250);

        // string destinationPath = Path.Combine(Application.persistentDataPath, FileBrowserHelpers.GetFilename(filePath));
        // FileBrowserHelpers.CopyFile(filePath, destinationPath);
    }

    public void CreateScriptableObjectButton()
    {
        if (_widthText.text == "" || _heightText.text == "" || _mapName.text == "")
            return;
        CreateScriptableObject(int.Parse(_widthText.text), int.Parse(_heightText.text), _mapName.text);
    }

    public void CreateScriptableObject(int width, int height, string name)
    {
        Debug.Log($"Create SO - name: {name}, width: {width}, height: {height}");

        MapData mapData = new MapData();
        mapData.mapWidth = width;
        mapData.mapHeight = height;
        mapData.mapName = name;
        mapData.overlayMap = _satelliteTex;
        mapData.bitmap = _bitmapImageTex;

        _testMapData = mapData;

        //todo Create the scriptble object and save it to the folder.
    }
}
