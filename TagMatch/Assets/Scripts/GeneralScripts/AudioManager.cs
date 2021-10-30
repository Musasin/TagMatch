using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    public List<AudioClip> BGMList;
    public List<AudioClip> SEList;
    public List<AudioClip> exVoiceList;

    static private CriWareInitializer criWareInitializer;
    static private CriAtom criAtom;
    static private CriAtomExPlayer bgm;
    static private CriAtomExPlayer se;
    static private CriAtomExPlayer voice;
    static private CriAtomExAcb currentAcb;
    static private CriAtomExAcb seAcb;

    private const string InGameDirectory = "InGame/";
    private string currentLoadedACB = "";
    static private string _lastPlayedBGM = "";
    public string lastPlayedBGM { get { return _lastPlayedBGM; } }

    public void Awake()
    {
        if (criWareInitializer == null)
        {
            criWareInitializer = gameObject.GetComponentInChildren<CriWareInitializer>();
            criWareInitializer.atomConfig.standardVoicePoolConfig.memoryVoices = 32;

            criWareInitializer.atomConfig.usesInGamePreview = false;
            criWareInitializer.Initialize();
            criAtom = gameObject.AddComponent<CriAtom>();

            if (criWareInitializer.atomConfig.usesInGamePreview == true)
            {
                criAtom.acfFile = InGameDirectory + "AtomCraftProject.acf";
            }
            else
            {
                criAtom.acfFile = "AtomCraftProject.acf";
            }
            criAtom.dspBusSetting = "DspBusSetting_0";
            criAtom.Setup();

            bgm = new CriAtomExPlayer();
            se = new CriAtomExPlayer();
            voice = new CriAtomExPlayer();

            Coroutine coroutine = StartCoroutine("LoadACB");
            GameObject.DontDestroyOnLoad(this);
        }
    }

    IEnumerator LoadACB()
    {
        if (seAcb != null) { yield break; }

        if (criWareInitializer.atomConfig.usesInGamePreview == true)
        {
            CriAtom.AddCueSheet("Common", InGameDirectory + "Common.acb", "");
        }
        else
        {
            CriAtom.AddCueSheet("Common", "Common.acb", "");
        }

        while (CriAtom.CueSheetsAreLoading)
        {
            yield return null;
        }

        seAcb = CriAtom.GetAcb("Common");
    }

    public void LoadACB(string acbName, string acbPath, string awbPath = "")
    {
        CriAtom.RemoveCueSheet(currentLoadedACB);
        if (criWareInitializer.atomConfig.usesInGamePreview == true)
        {
            Coroutine coroutine = StartCoroutine(LoadACBInternal(acbName, InGameDirectory + acbPath, awbPath));
        }
        else
        {
            Coroutine coroutine = StartCoroutine(LoadACBInternal(acbName, acbPath, awbPath));
        }
    }

    IEnumerator LoadACBInternal(string acbName, string acbPath, string awbPath)
    {
        CriAtom.AddCueSheet(acbName, acbPath, awbPath);

        while (CriAtom.CueSheetsAreLoading)
        {
            yield return null;
        }

        currentAcb = CriAtom.GetAcb(acbName);
        currentLoadedACB = acbName;
    }

    public void OnDestroy()
    {
    }

    public void Update()
    {
    }

    public void ChangeMasterVolume(float volume)
    {
        CriAtomExAsr.SetBusVolume("MasterOut", volume);
    }

    public void ChangeBGMVolume(float volume)
    {
        CriAtom.SetCategoryVolume("BGM", volume);
    }
    public void ChangeSEVolume(float volume)
    {
        CriAtom.SetCategoryVolume("SE", volume);
    }
    public void ChangeVoiceVolume(float volume)
    {
        CriAtom.SetCategoryVolume("VOICE", volume);
        CriAtom.SetCategoryVolume("BATTLE_VOICE", volume);
    }

    public void ChangeDSPSnapshot(string snapShotName)
    {
        CriAtomEx.ApplyDspBusSnapshot(snapShotName, 100);
    }

    public CriAtomExPlayback PlaySE(string seName)
    {
        se.SetCue(seAcb, seName);
        return se.Start();
    }

    public void StopSE()
    {
        se.Stop();
    }

    public void StopSE(ref CriAtomExPlayback playback)
    {
        playback.Stop(false);
    }

    public void PlayExVoice(string exVoiceName, bool isUseCommonAcb = false)
    {
        if (isUseCommonAcb)
        {
            voice.SetCue(seAcb, exVoiceName);
        } 
        else
        {
            voice.SetCue(currentAcb, exVoiceName);
        }
        voice.Start();
    }

    public bool IsPlayingExVoice()
    {
        return voice.GetStatus() == CriAtomExPlayer.Status.Playing;
    }

    public CriAtomExPlayback PlayBGM(string bgmName)
    {
        bgm.SetCue(currentAcb, bgmName);
        _lastPlayedBGM = bgmName;
        return bgm.Start();
    }

    public void SetSEAISACControlValue(CriAtomExPlayback playbackSource, string aisacControlName, float value)
    {
        se.SetAisacControl(aisacControlName, value);
        se.Update(playbackSource);
    }
    public void SetBGMAISACControlValue(CriAtomExPlayback playbackSource, string aisacControlName, float value)
    {
        bgm.SetAisacControl(aisacControlName, value);
        bgm.Update(playbackSource);
    }
    public void FadeInBGM(string bgmName, int fadeInTimeMs = 1000)
    {
        PlayBGM(bgmName + "_withFade");
    }

    public void StopBGM()
    {
        bgm.Stop();
        _lastPlayedBGM = "";
    }

    public void StopBGMWithoutRelease()
    {
        bgm.StopWithoutReleaseTime();
        _lastPlayedBGM = "";
    }

    public void SetNextBlock(ref CriAtomExPlayback playback, int blockIndex)
    {
        playback.SetNextBlockIndex(blockIndex);
    }
}
