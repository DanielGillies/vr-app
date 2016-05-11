/*
* MPMP
* Copyright © 2016 Stefan Schlupek
* All rights reserved
* info@monoflow.org
*/

//This define is only for iOS/OSX
//https://developer.apple.com/library/mac/documentation/AVFoundation/Reference/AVPlayer_Class/index.html#//apple_ref/occ/instm/AVPlayer/seekToTime:toleranceBefore:toleranceAfter:
//#define SEEK_TOLERANCE


using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.IO;


namespace monoflow
{
    public partial class MPMP : MonoBehaviour
    {
        /// <summary>
        /// Internal types of events from the native site 
        /// </summary>
        public enum Events { LOAD, LOADED, PLAY, PAUSE, DESTROY, ERROR, PLAYBACKCOMPLETED,AVF_PIXELBUFFER_ERROR, TEXTURE_CHANGED };

        /// <summary>
        /// Event that is called after the native part is initialized
        /// </summary>
        public Action<MPMP> OnInit;
        /// <summary>
        /// Event that is called when a loading is triggert
        /// </summary>
        public Action<MPMP> OnLoad;

        /// <summary>
        /// Event  that is called when the loading is finished.
        /// This is the best time to do some setup with regard to the actual video data like size, duration...
        /// </summary>
        public Action<MPMP> OnLoaded;

        /// <summary>
        /// Event is called when MPMP is pausing
        /// </summary>
        public Action<MPMP> OnPause;

        /// <summary>
        /// Event is called when MPMP starts to play
        /// </summary>
        public Action<MPMP> OnPlay;

        /// <summary>
        /// Event is called when the MPMP instance is destroyed
        /// </summary>
        public Action<MPMP> OnDestroyed;

        /// <summary>
        /// Event is called when there arise an error on the native site
        /// </summary>
        public Action<MPMP> OnError;

        /// <summary>
        /// Event is called when the video has reached the end. When you are in Loop mode this event is not called!
        /// </summary>
        public Action<MPMP> OnPlaybackCompleted;

        /// <summary>
        /// On OSX 10.11 El Capitan there is an issue with AVFoundation. To circumvent video refreshing errors you can catch this error.You should use this event to trigger a new Load.
        /// </summary>
        public Action<MPMP> OnPixelBufferError;

        /// <summary>
        /// Event is called when the dimension of the video texture has changed
        /// </summary>
        public Action<MPMP> OnTextureChanged;


        // public Action<MPMP> OnDimensionChanged;


        /// <summary>
        /// Triggers a loading with the current videoPath 
        /// <para/> <seealso cref="monoflow.MPMP.videoPath"/>
        /// </summary>
        public void Load()
        {
            string filePath = GetFilePath(videoPath);
            if (String.IsNullOrEmpty(filePath)){
                Debug.LogWarning("Please check your video path:" + filePath);
            } else {

            

                if (isNVIDIA) { 
                    
                    if (_videoTexture != null)
                    {
                        /*
                        Debug.Log(String.Format("width:{0}  height:{1}", _videoTexture.width, _videoTexture.height));
                        if(texCopy != null) DestroyImmediate(texCopy);
                        //http://forum.unity3d.com/threads/external-textures.378940/
                        texCopy = new Texture2D(_videoTexture.width/4, _videoTexture.height/4, _videoTexture.format, true,false);                                                                                                                         
                        nativeTexturePtr = GetNativeTexture(_id);
                        //We need the texture data on the CPU! So this doesn't work with the nativeTexturePtr
                         texCopy.LoadRawTextureData(nativeTexturePtr,(_videoTexture.width* _videoTexture.height*4));
                        texCopy.Apply();
                        */

                        _RestoreMaterialState();

                        DestroyImmediate(_videoTexture);
                        _videoTexture = null;
                                
                     }
                   // _loadLock = true;
                }//NVIDIA

                Pause();
               // _UpdateIsPlaying();
               // _UpdateIsPaused();

                _loadLock = true;
                Load(_id, filePath);
                _UpdateIsLoading();
            }
           
        }

        Texture2D texCopy;

        private void _RestoreMaterialState()
        {
           // return;
           // Debug.Log("_RestoreMaterialState:"+ _videoDefaultTexture.name);
            if (_videoMaterial == null) return;
            if (!_hasTextureProperty) return;            
            _videoMaterial.SetTexture(texturePropertyName, _videoDefaultTexture);                
        }
        private void _StoreMaterialState()
        {
           // return;
            _videoDefaultTexture = null;
            if (_videoMaterial == null) return;
            if (!_hasTextureProperty) return;
            _videoDefaultTexture = _videoMaterial.GetTexture(texturePropertyName);        
        }

        /// <summary>
        /// Sets the current videoPath and triggers a loading
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path) {
            videoPath = path;
           // Pause();
            Load();           
        }

        /// <summary>
        /// Start playing the media when loaded   
        /// </summary>
        public void Play()
        {
            if (_loadLock) return;
            Play(_id);
            _UpdateIsPlaying();
            _UpdateIsPaused();
        }

        /// <summary>
        /// Pause the media
        /// </summary>
        public void Pause()
        {
            if (_loadLock) return;
            Pause(_id);
            _UpdateIsPaused();
            _UpdateIsPlaying();
           
        }


        /// <summary>
        /// Media starts playing automatically when loaded
        /// </summary>
        public bool autoPlay
        {
            get { return _autoPlay; }
            set
            {
                if (_loadLock) return;
                bool isChanged = false;
                isChanged = _autoPlay != value;
                if (isChanged)
                {
                    _autoPlay = value;
                    //Debug.Log("SetAutoPlay:"+_autoPlay);
                    SetAutoPlay(_id, _autoPlay);
                }
            }
        }


        /// <summary>
        /// Retrieve the current seek position
        /// <para/> The values are normalized (0 - 1)  
        /// <seealso cref="monoflow.MPMP.GetCurrentPosition(bool normalized)"/>       
        /// </summary>
        public float GetSeek(bool normalized)
        {
            // _seek = (float)GetCurrentPosition(_id);
            return (float)GetCurrentPosition(normalized);                       
           
        }


        /// <summary>
        /// Seek to a point in time in sec
        /// <para/>On iOS/OSX you can change the seek behaviour to a more precise version.(But could cause some decoding delay)
        /// <para/>To set SEEK_TOLERANCE script define you can go in the Unity editor to 'Edit/Preferences.../MPMP'
        /// or enable the SEEK_TOLERANCE define at the top of the MPMP_API.cs file manually
        ///<param name="t"></param>
        /// </summary>
        public void SeekTo(float t)
        {
            SeekTo(t, false);
        }


        /// <summary>
        /// Seek to a point in time
        /// <para/>Depending on the normalized paramter the values are normalized (0 - 1)  or in sec 
        /// <para/>On iOS/OSX you can change the seek behaviour to a more precise version.(But could cause some decoding delay)
        /// <para/>To set SEEK_TOLERANCE script define you can go in the Unity editor to 'Edit/Preferences.../MPMP'
        /// or enable the SEEK_TOLERANCE define at the top of the MPMP_API.cs file manually
        ///  <param name="t">time</param>
        ///  <param name="normalized">normalized flag</param>
        /// </summary>
        public void SeekTo(float t, bool normalized)
        {
            // seek = t;   
            if (_loadLock) return;
            float tValue = t;
            if(normalized) tValue = Mathf.Clamp(tValue, 0.0f, 1.0f);
            bool isChanged = false;
           
            isChanged = GetSeek(normalized) != tValue;
                        
            if (isChanged)
            {
               // Debug.Log(String.Format("SeekTo:{0}, {1}" , cv, tValue));
                _seek = tValue;
                //Debug.Log("SeekTo:" + _seek);            
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IPHONE
#if SEEK_TOLERANCE
							SeekToWithTolerance(_id,_seek,0,normalized);
#else
							SeekTo(_id, _seek,normalized);
#endif
#else
                SeekTo(_id, _seek, normalized);
#endif
            }
        }

        /// <summary>
        /// The  seeking property should be set when seeking with a gui element on Android otherwise the video will not update while you seek
        /// </summary>
        /// <param name="status"></param>
        public void SetSeeking(bool status)
        {
           // Debug.Log("SetSeeking:" + status);
            _seeking = status;
        }

  

        /// <summary>
        /// get or set the current volume of the media
        /// <para/>values are normalized (0 - 1)
        /// </summary>
        public float volume
        {
            get {
               // _volume = (float)GetCurrentVolume(_id);
                return _volume;
            }
            set
            {
                if (_loadLock) return;
                float tValue = Mathf.Clamp(value,0.0f,1.0f);
                bool isChanged = false;
                isChanged = _volume != tValue;
                if (isChanged)
                {
                    _volume = tValue;
                    //Debug.Log("SetVolume:"+ _volume);
                    SetVolume(_id, _volume);
                }
            }
        }
        private void _UpdateVolume()
        {
            _volume = (float)GetCurrentVolume(_id);
        }


        /// <summary>
        /// Set or get the current audio output balance
        /// <para/> normalized values: 
        /// <para/>-1 : just left channel 
        /// <para/>0 : both channels
        /// <para/>1 just right channel
        /// <para/>(At the moment this is unsupported on OSX/iOS)
        /// </summary>
        public float balance
        {
            get { return _balance; }
            set
            {
                if (_loadLock) return;
                float tValue = Mathf.Clamp(value, -1.0f, 1.0f);
                bool isChanged = false;
                isChanged = _balance != tValue;
                if (isChanged)
                {
                    _balance = tValue;
                    //Debug.Log("SetBalance:" + _balance);
                    SetBalance(_id, _balance);
                }
            }
        }
       

        /// <summary>
        /// Get or set the looping of the media
        /// </summary>
        public bool looping
        {
            get { return _looping; }
            set
            {
                if (_loadLock) return;
                bool isChanged = false;
                isChanged = _looping != value;
                if (isChanged)
                {
                    _looping = value;
                    //Debug.Log("SetLooping:"+_looping);
                    SetLooping(_id, _looping);
                }
            }
        }



        /// <summary>
        /// Get or set the current playback rate of the media
        /// <para/>negative values are reverse playback
        /// <para/>(On Android this is only supported at API level 23+ (Android 6+) )
        /// </summary>
        public float rate
        {
            get
            {
               // _rate = GetPlaybackRate(_id);
                //Debug.Log("GetPlaybackRate:" + _rate);
                return _rate;
            }
            set
            {
                if (_loadLock) return;
                float tValue = value;// Mathf.Clamp(value, 0.0f, 1.0f);
                bool isChanged = false;
                isChanged = _rate != tValue;
                if (isChanged)
                {
                    _rate = tValue;
                    //Debug.Log("SetPlaybackRate:"+ _rate);
                    SetPlaybackRate(_id, _rate);
                }
            }
        }
        public void _UpdatePlaybackRate()
        {
            _rate = GetPlaybackRate(_id);
        }

        /// <summary>
        ///  Get the current position of the media that is playing in sec
        /// </summary>
        /// <returns></returns>
        public double GetCurrentPosition()
        {
            return GetCurrentPosition(false);
        }
        /// <summary>
        /// Get the current position of the media that is playing
        /// <para/>depending on the normalized parameter the values are normalized (0 -1 ) or in sec
        /// </summary>
        /// <returns></returns>
        public double GetCurrentPosition(bool normalized)
        {
            if (normalized) {
                return  _currentPositionNormalized;
            } else {
                return _currentPosition;
            }
           // return _currentPosition;
        }

        private double _currentPosition;
        private double _currentPositionNormalized;

        private void _UpdateCurrentPosition()
        {
            //Internal call to plugin
            _currentPosition = GetCurrentPosition(_id);
            _currentPosition = (_currentPosition == double.NaN) ? 0 : _currentPosition;
            _currentPositionNormalized = _duration > 0 ? (_currentPosition / _duration) : 0;
        }



        /// <summary>
        /// Get the duration of the media file in seconds
        /// </summary>
        /// <returns></returns>
        public double GetDuration()
        {
            return _duration;
        }

        private double _duration;
        private void _UpdateDuration()
        {
            //Internal call to plugin
            _duration =  GetDuration(_id);
        }

        /// <summary>
        /// Check if the media is playing
        /// </summary>
        /// <returns></returns>
        public bool IsPlaying() {
            // return IsPlaying(_id);
            return _isPlaying;
        }
        private bool _isPlaying;
        private void _UpdateIsPlaying()
        {
            //Internal call to plugin
            _isPlaying = IsPlaying(_id);
        }

        /// <summary>
        /// Check if the media is paused
        /// </summary>
        /// <returns></returns>
        public bool IsPaused() {
            // return IsPaused(_id);
            return _isPaused;
        }
        private bool _isPaused;
        private void _UpdateIsPaused()
        {
            //Internal call to plugin
            _isPaused = IsPaused(_id);
        }

        /// <summary>
        /// Check if the media is loading
        /// </summary>
        /// <returns></returns>
        public bool IsLoading()
        {
            //return IsLoading(_id);
            return _isLoading;
        }
        private bool _isLoading;
        private void _UpdateIsLoading()
        {
            //Internal call to plugin
            _isLoading = IsLoading(_id);
        }
       

        /// <summary>
        /// Get the raw video texture as Texture2D
        /// </summary>
        /// <returns></returns>
        public Texture2D GetVideoTexture()
        {
          
           // lock(this)
            {
                return _videoTexture == null ? null : _videoTexture;
            }
              
            //return _videoTexture;
        }

        /// <summary>
        /// Get the video size from the native plugin as Vector2
        /// <para/> x: width
        /// <para/> y: height
        /// </summary>
        /// <returns></returns>
        public Vector2 GetNativeVideoSize()
        {         
            return _currentNativeVideoSize;
        }
       
        private void _UpdateNativeVideoSize()
        {
            //Internal call to plugin       
            GetNativeVideoSize(_id, ref _currentNativeVideoSize);
        }
      

        /// <summary>
        /// Set the video material.       
        /// </summary>
        /// <param name="mat"></param>
        public void SetVideoMaterial(Material mat)
        {

            lock (this)
            {
                if (_videoMaterial != null)
                {
                    _RestoreMaterialState();
                }
     

                if (mat != null)
                {
                    _hasTextureProperty = mat.HasProperty(texturePropertyName);
                }
                else
                {
                    _hasTextureProperty = false;
                }

                _videoMaterial = mat;

                _StoreMaterialState();

            }//lock

            if (!_hasTextureProperty) Debug.LogWarning(String.Format("The shader of the Material needs a '{0}' property!", texturePropertyName));

          
        }

        /// <summary>
        /// Get the video material.
        /// </summary>
        /// <returns></returns>
        public Material GetVideoMaterial()
        {
            return _videoMaterial;            
        }

        /// <summary>
        /// Texture filtering modes for Windows
        /// </summary>
        public enum FilterModeMPMP { Point, Bilinear }

        /// <summary>
        /// On Windows you can choose between Point and Bilinear texture filtering
        /// <para>If you set this property on non Windows platforms it has no effect on the video texture</para>
        /// </summary>
        public FilterModeMPMP filtermode
 
        {
            get { return _filtermode; }
            set
            {
                if (!Application.platform.ToString().StartsWith("Windows"))
                {
                    Debug.Log("<color='red'>MPMP</ color >:Texture filtering just works under Windows!");
                }

                bool isChanged = false;
                isChanged = _filtermode != value;
                if (isChanged)
                {                  
                    _filtermode = value;
                    //Debug.Log("SetBalance:" + _balance);              
                }
            }
        }

        /// <summary>
        /// Set the frequency how often per second the native plugin should update
        /// <para>Depending on your system you normaly leave this at 60</para>
        /// <para>Keep in mind that the real maximal frequency depends on the framerate of your app</para>
        /// </summary>
        /// <param name="interval"></param>
        public void SetUpdateFrequency(float interval)
        {        
            _updateFrequency = interval;
            _updateFrequencyInverse = 1.0f / _updateFrequency;
        }

        /// <summary>
        /// Get the frequency how often per second the native plugin should update
        /// </summary>
        /// <returns></returns>
        public float GetUpdateFrequency( )
        {
            return _updateFrequency;
        }


        public bool HasHadPixelBufferError()
        {
            return _hasHadPixelBufferError;
        }

    }//class
}//namespace
