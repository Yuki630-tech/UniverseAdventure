using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Audio/AudioData")]
public class AudioClipData : ScriptableObject
{
    public enum AudioClipName
    {
        /// <summary>
        /// ‰½‚à—¬‚³‚È‚¢
        /// </summary>
        None,
        StageBGM1,
        StageBGM2,
        RhythmicalBGM,
        ConversationBGM,
        DieBGM,
        GameOverBGM,
        TitleBGM
        
    }

    public enum SeAudioClipName
    {
        None,
        GetItem,
        AppearStarring,
        Die
    }

    [Serializable]
    public struct AudioData
    {
        public AudioClipName Name;
        public AudioClip AudioClip;
    }

    [Serializable]
    public struct SeAudioData
    {
        public SeAudioClipName Name;
        public AudioClip AudioClip;
    }

    [SerializeField] List<AudioData> bgmDatas;
    [SerializeField] List<SeAudioData> seDatas;

    public List<AudioData> BgmDatas { get { return bgmDatas; } }

    public List<SeAudioData> SeDatas { get => seDatas; }
}
