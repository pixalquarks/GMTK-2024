using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace GMTK_2024
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager i;
        // public SfxClip[] sfxClips;


        [SerializeField] private AudioSource track1;
        [SerializeField] private AudioSource track2;
        [SerializeField] private AudioSource uiAudioSource;
        [SerializeField] private AudioSource sfxAudioSource;

        [SerializeField] private M_AudioClip[] musicAudioClips;
        [SerializeField] private M_AudioClip[] sfxAudioClips;

        [SerializeField] private AudioMixer masterMixer;
        [SerializeField] private float musicFadeTime = 5.0f;

        private List<M_AudioClip> _menuClips;
        private List<M_AudioClip> _levelClips;

        private List<AudioSource> _tracks;
        private int _currentTrackIndex;
        private Coroutine _currentTriggerCoroutine;
        private M_AudioClip _lastPlayedClip;
        private Scene _lastScene;

        private Dictionary<string, M_AudioClip> _nameToClipMapping;

        private void Awake()
        {
            if (i == null)
            {
                i = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
            _tracks = new List<AudioSource>() {
                track1,
                track2
            };
            _currentTrackIndex = 0;
            _nameToClipMapping = new Dictionary<string, M_AudioClip>();
            MapAudiClips();
            LoadVolumeSettings();
            SplitClips();
            PlayDefaultMusic();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }



        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene != _lastScene)
            {
                if (_currentTriggerCoroutine != null)
                {
                    StopCoroutine(_currentTriggerCoroutine);
                }
                PlayDefaultMusic();
                _lastScene = scene;
            }
        }
        private void LoadVolumeSettings()
        {
            var masterVolume = PlayerPrefs.GetFloat(VolumeManager.MasterVolumeKey, 1.0f);
            var musicVolume = PlayerPrefs.GetFloat(VolumeManager.MusicVolumeKey, 0.6f);
            var sfxVolume = PlayerPrefs.GetFloat(VolumeManager.SfxVolumeKey, 0.8f);

            masterMixer.SetFloat(VolumeManager.MasterVolumeKey, VolumeManager.ConvertToDecibel(masterVolume));
            masterMixer.SetFloat(VolumeManager.MusicVolumeKey, VolumeManager.ConvertToDecibel(musicVolume));
            masterMixer.SetFloat(VolumeManager.SfxVolumeKey, VolumeManager.ConvertToDecibel(sfxVolume));
        }



        private void PlayDefaultMusic()
        {
            const int mainMenuIndex = 0;
            var index = SceneManager.GetActiveScene().buildIndex;
            Debug.Log("Scene Count: " + SceneManager.sceneCountInBuildSettings);
            Debug.Log("Active Scene: " + index);
            if (index == mainMenuIndex)
            {
                foreach (var track in _tracks)
                {
                    track.Stop();
                }
                _tracks[_currentTrackIndex].clip = GetClipByName("MainMenu");
                _tracks[_currentTrackIndex].Play();
            }
            else if (index == SceneManager.sceneCountInBuildSettings - 1)
            {
                var clip = GetClipByName("Outro");
                FadeBetweenTrack(clip);
            }
            else
            {
                var clip = GetRandomLevelClip();
                _currentTriggerCoroutine = StartCoroutine(TriggerFadeOnMusicEnd(clip.length));
                FadeBetweenTrack(clip);
            }
        }

        private void SplitClips()
        {
            _menuClips = new List<M_AudioClip>();
            _levelClips = new List<M_AudioClip>();
            foreach (var clip in musicAudioClips)
            {
                switch (clip.type)
                {
                    case ClipType.Menu:
                        _menuClips.Add(clip);
                        break;
                    case ClipType.Level:
                        _levelClips.Add(clip);
                        break;
                    default:
                        break;
                }
            }
        }

        private void MapAudiClips()
        {
            foreach (var sfx in sfxAudioClips)
            {
                var clipName = sfx.name.Replace(" ", string.Empty).ToLower();
                _nameToClipMapping.Add(clipName, sfx);
            }

            foreach (var clip in musicAudioClips)
            {
                var clipName = clip.name.Replace(" ", string.Empty).ToLower();
                _nameToClipMapping.Add(clipName, clip);
            }
        }

        private AudioClip GetClipByName(string clipName)
        {
            var mClipName = clipName.Replace(" ", string.Empty).ToLower();
            if (!_nameToClipMapping.ContainsKey(mClipName))
            {
                Debug.LogException(new Exception($"Could not find any clip with name: {mClipName}. Please check the spelling or add the clip in AudioManager"));
                return null;
            }

            return _nameToClipMapping[mClipName].clip;
        }

        private AudioClip GetRandomLevelClip()
        {
            if (_levelClips.Count == 1)
            {
                return _levelClips[0].clip;
            }
            var index = Random.Range(0, _levelClips.Count);
            Debug.Log("Clip Count: " + _levelClips.Count + " Index: " + index);
            var temp = _levelClips[index];
            _levelClips.RemoveAt(index);
            if (_lastPlayedClip != null)
            {
                _levelClips.Add(_lastPlayedClip);
            }
            _lastPlayedClip = temp;
            return temp.clip;
        }

        private void FadeBetweenTrack(AudioClip clip)
        {
            var currentTrack = _tracks[_currentTrackIndex];
            _currentTrackIndex = (_currentTrackIndex + 1) % _tracks.Count;
            var nextTrack = _tracks[_currentTrackIndex];
            nextTrack.clip = clip;
            FadeMusic(currentTrack, nextTrack);
        }

        private void FadeMusic(AudioSource fromTrack, AudioSource toTrack)
        {
            toTrack.Play();
            toTrack.volume = 0;
            fromTrack.DOFade(0, musicFadeTime).onComplete = fromTrack.Stop;
            toTrack.DOFade(1, musicFadeTime);
        }

        private IEnumerator TriggerFadeOnMusicEnd(float delay)
        {
            Debug.Log("Starting Next Fade in: " + delay);
            yield return new WaitForSeconds(delay - musicFadeTime);
            PlayDefaultMusic();
        }

        public void PlayUIHoverSound()
        {
            const string sfxName = "UIHover";
            var clip = GetClipByName(sfxName);
            if (clip != null)
                uiAudioSource.PlayOneShot(GetClipByName(sfxName));
            else
                Debug.LogException(new Exception($"Could not find track with name: \"{sfxName}\". Please add the clip in the audio list."));
        }

        public void PlayUIClickSound()
        {
            const string sfxName = "UIClick";
            var clip = GetClipByName(sfxName);
            if (clip != null)
                uiAudioSource.PlayOneShot(GetClipByName(sfxName));
            else
                Debug.LogException(new Exception($"Could not find track with name: \"{sfxName}\". Please add the clip in the audio list."));
        }

        public void PlayAudioByName(string audioClipName)
        {
            var clip = GetClipByName(audioClipName);
            if (clip != null)
            {
                sfxAudioSource.PlayOneShot(clip);
            }
        }
    }
}
