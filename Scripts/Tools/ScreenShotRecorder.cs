using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
Usage:
1. Attach this script to your chosen camera's game object.
2. Set that camera's Clear Flags field to Solid Color.

4. Choose your desired resolution in Unity's Game window (must be less than or equal to your screen resolution)
5. Turn on "Maximise on Play"
6. Play your scene. Screenshots will be saved to YourUnityProject/Screenshots by default.
*/

namespace Assets._TwoFatCatsAssets.Scripts.Tools
{
    public class ScreenShotRecorder : MonoBehaviour
    {
        #region public fields
        [Tooltip("A folder will be created with this base name in your project root")]
        public string FolderName = "Screenshots";
        #endregion
        #region private fields
        private string _folderName = "";
        private GameObject _whiteCamGameObject;
        private GameObject _blackCamGameObject;
        private Camera _whiteCam;
        private Camera _blackCam;
        private Camera _mainCam;
        private bool _done = false;
        private int _screenWidth;
        private int _screenHeight;
        private Texture2D _textureBlack;
        private Texture2D _textureWhite;
        private Texture2D _textureTransparentBackground;

        public List<GameObject> ObjectsToScreenShot = new List<GameObject>();
        #endregion

        void Awake()
        {
            _mainCam = gameObject.GetComponent<Camera>();
            CreateBlackAndWhiteCameras();
            CreateNewFolderForScreenshots();
            CacheAndInitialiseFields();

            foreach (var go in ObjectsToScreenShot)
            {
                go.SetActive(false);
            }
        }

        void LateUpdate()
        {
            if (!_done)
            {
                StartCoroutine(CaptureFrame());
            }
            else
            {
                Debug.Log("Complete!");
                Debug.Break();
            }
        }

        IEnumerator CaptureFrame()
        {
            yield return new WaitForEndOfFrame();
            if (_done) StopCoroutine("CaptureFrame");

            for (int i = 0; i < ObjectsToScreenShot.Count; i++)
            {
                ObjectsToScreenShot[i].SetActive(true);
                RenderCamToTexture(_blackCam, _textureBlack);
                RenderCamToTexture(_whiteCam, _textureWhite);
                CalculateOutputTexture();
                SavePng(ObjectsToScreenShot[i].name);
                ObjectsToScreenShot[i].SetActive(false);
                if (i == ObjectsToScreenShot.Count - 1) _done = true;
            }
        }

        void RenderCamToTexture(Camera cam, Texture2D tex)
        {
            cam.enabled = true;
            cam.Render();
            WriteScreenImageToTexture(tex);
            cam.enabled = false;
        }

        void CreateBlackAndWhiteCameras()
        {
            _whiteCamGameObject = (GameObject)new GameObject();
            _whiteCamGameObject.name = "White Background Camera";
            _whiteCam = _whiteCamGameObject.AddComponent<Camera>();
            _whiteCam.CopyFrom(_mainCam);
            _whiteCam.backgroundColor = Color.white;
            _whiteCamGameObject.transform.SetParent(gameObject.transform, true);

            _blackCamGameObject = (GameObject)new GameObject();
            _blackCamGameObject.name = "Black Background Camera";
            _blackCam = _blackCamGameObject.AddComponent<Camera>();
            _blackCam.CopyFrom(_mainCam);
            _blackCam.backgroundColor = Color.black;
            _blackCamGameObject.transform.SetParent(gameObject.transform, true);
        }

        void CreateNewFolderForScreenshots()
        {
            // Find a folder name that doesn't exist yet. Append number if necessary.
            _folderName = FolderName;
            int count = 1;
            while (Directory.Exists(_folderName))
            {
                _folderName = FolderName + count;
                count++;
            }
            Directory.CreateDirectory(_folderName); // Create the folder
        }

        void WriteScreenImageToTexture(Texture2D tex)
        {
            tex.ReadPixels(new Rect(0, 0, _screenWidth, _screenHeight), 0, 0);
            tex.Apply();
        }

        void CalculateOutputTexture()
        {
            Color color;
            for (int y = 0; y < _textureTransparentBackground.height; ++y)
            {
                // row
                for (int x = 0; x < _textureTransparentBackground.width; ++x)
                {
                    // column
                    float alpha = _textureWhite.GetPixel(x, y).r - _textureBlack.GetPixel(x, y).r;
                    alpha = 1.0f - alpha;
                    if (Math.Abs(alpha) < 0.05f)
                    {
                        color = Color.clear;
                    }
                    else
                    {
                        color = _textureBlack.GetPixel(x, y) / alpha;
                    }
                    color.a = alpha;
                    _textureTransparentBackground.SetPixel(x, y, color);
                }
            }
        }

        void SavePng(string itemName)
        {
            string fileName = string.Format("{0}/{1:D04}.png", _folderName, itemName);
            var pngShot = _textureTransparentBackground.EncodeToPNG();
            File.WriteAllBytes(fileName, pngShot);
        }

        void CacheAndInitialiseFields()
        {
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
            _textureBlack = new Texture2D(_screenWidth, _screenHeight, TextureFormat.RGB24, false);
            _textureWhite = new Texture2D(_screenWidth, _screenHeight, TextureFormat.RGB24, false);
            _textureTransparentBackground = new Texture2D(_screenWidth, _screenHeight, TextureFormat.ARGB32, false);
        }
    }
}