using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public class AudioMgr : MonoBehaviour
    {
        //音乐文件
        public AudioSource music;
        public AudioSource sound;

        //音乐开关记录
        private bool _musicEnable;

        //音效开关记录
        private bool _soundEnable;

        private const string MUSIC_KEY = "GAME_MUSIC";
        private const string SOUND_KEY = "GAME_SOUND";

        private string _musicName = "";

        static AudioMgr _ins;
        public static AudioMgr getInstance() { return _ins; }
        public static AudioMgr Ins { get => _ins; }

        void Awake()
        {
            _ins = this;
        }

        void Start()
        {
            int musicVal = PlayerPrefs.GetInt(MUSIC_KEY, 1);
            int soundVal = PlayerPrefs.GetInt(SOUND_KEY, 1);
            _musicEnable = musicVal == 1;
            _soundEnable = soundVal == 1;
        }

        public static void SplayMusic(string musicName)
        {
            if (_ins == null)
            {
                return;
            }

            _ins.PlayMusic(musicName);
        }


        public static void SplaySound(string soundName, bool loop = false)
        {
            if (_ins == null)
            {
                return;
            }

            _ins.PlaySound(soundName, loop);
        }

        //播放背景音乐
        public void PlayMusic(string musicName)
        {
            _musicName = musicName;
            if (!_musicEnable)
            {
                return;
            }

            AudioClip clip = Resources.Load("audio/" + musicName) as AudioClip;
            if (clip == null)
            {
                _musicName = "";
                return;
            }

            Debug.Log("[*] PlayMusic:" + musicName);

            PlayMusic(clip);
        }

        public void PlayMusic(AudioClip clip)
        {
            if (!clip) return;
            music.clip = clip;
            music.loop = true;
            music.Play();
        }

        //播放音效
        public void PlaySound(string soundName, bool loop = false)
        {
            if (!_soundEnable)
            {
                return;
            }
            //Debug.Log("[*] PlaySound:" + soundName);
            AudioClip clip = Resources.Load("audio/" + soundName) as AudioClip;

            if (clip)
            {
                if (loop)
                {
                    sound.clip = clip;
                    sound.loop = true;
                    sound.Play();
                }
                else
                {
                    sound.clip = null;
                    sound.loop = false;
                    sound.PlayOneShot(clip);
                }
            }
        }

        public void PlaySound(AudioClip clip)
        {
            if (!clip) return;
            sound.clip = clip;
            sound.loop = false;
            sound.Play();
        }

        public bool MusicEnable
        {
            set
            {
                //music.volume = value ? 1 : 0;
                _musicEnable = value;
                PlayerPrefs.SetInt(MUSIC_KEY, value ? 1 : 0);
                if (_musicEnable)
                {
                    PlayMusic(_musicName);
                }
                else
                {
                    music.Stop();
                }
            }
            get { return _musicEnable; }
        }

        //音效开关设置
        public bool SoundEnable
        {
            set
            {
                //sound.volume = value ? 1 : 0;
                _soundEnable = value;
                PlayerPrefs.SetInt(SOUND_KEY, value ? 1 : 0);
            }
            get { return _soundEnable; }
        }

    }
}