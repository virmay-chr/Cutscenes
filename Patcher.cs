using HarmonyLib;
using UnityEngine;
using TMPro;
using System.Text;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;

namespace Cutscenes
{
    public class Patcher
    {
        //ON CREATING LEVEL SCENE ================================================================================
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.Start))]
        static void Create()
        {
            Plugin.RewindIcon = new GameObject("RewindIcon").AddComponent<TextMeshProUGUI>();
            Plugin.RewindIcon.text = "<cspace=-15>▶▶";
            Plugin.RewindIcon.alignment = TextAlignmentOptions.Right;
            Plugin.RewindIcon.fontSize = 58;
            Plugin.RewindIcon.raycastTarget = false;
            Plugin.RewindIcon.rectTransform.SetParent(GameManager.inst.Timeline.parent, false);
            Plugin.RewindIcon.rectTransform.anchoredPosition = new(555, 494);
            Plugin.RewindIcon.rectTransform.sizeDelta = new(700, 50);
            Plugin.RewindIcon.alpha = 0;

            Plugin.SkipLabel = new GameObject("SkipLabel").AddComponent<TextMeshProUGUI>();
            Plugin.SkipLabel.fontStyle = FontStyles.Bold;
            Plugin.SkipLabel.alignment = TextAlignmentOptions.Right;
            Plugin.SkipLabel.fontSize = 32;
            Plugin.SkipLabel.raycastTarget = false;
            Plugin.SkipLabel.rectTransform.SetParent(GameManager.inst.Timeline.parent, false);
            Plugin.SkipLabel.rectTransform.anchoredPosition = new(580, 495);
            Plugin.SkipLabel.rectTransform.sizeDelta = new(700, 50);
            Plugin.SkipLabel.enabled = false;
        }

        //ON EXITING LEVEL SCENE ================================================================================
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.OnDestroy))]
        static void Destroy()
        {
            Object.Destroy(Plugin.SkipLabel.gameObject);
            Object.Destroy(Plugin.RewindIcon.gameObject);
        }


        //ON LEVEL ================================================================================
        static readonly string text = $"[{Plugin.key.Value.MainKey}] - Skip";
        static float glitch = 1;
        static float et = 0;
        static readonly float dur = 2.5f;
        static StringBuilder sb;
        static bool isCutsceneFlag = false, bypassedFlag = false;
        static float pitchState;
        static float hitSkipTime = float.MaxValue, destTime = float.MaxValue;
        static readonly LSEffectsManager.GlitchOverrideProfile profile = new()
        {
            intensity = Plugin.glitchIntensity.Value,
            speed = 2f,
            width = 0.99f
        };
        static bool IsEditor => Systems.SceneManagement.SceneLoader.Inst.manager.ActiveSceneGroup.GroupName == "Editor";
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static void TweenText() => et = 0.001f;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static void BreakTweenText() => et = dur / 10 * 9;


        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.UpdateTimeline))]
        static void Update()
        {
            //return if level is not loaded or it is in editing mode
            if (GameManager.inst.CurGameState == GameManager.GameState.Loading
                || DataManager.inst.gameData.beatmapData.checkpoints == null) return;


            //get current checkpoint
            var idx = GameManager.inst.GetClosestCheckpointIndex(DataManager.inst.gameData.beatmapData.checkpoints, GameManager.Inst.CurrentSongTimeSmoothed);
            //get next checkpoint
            int idx2 = -1;
            if (!IsEditor) idx2 = idx + 1;
            else
            {
                float min = GameManager.Inst.CurrentSongLength;
                for (int i = 0; i < DataManager.inst.gameData.beatmapData.checkpoints.Count; i++)
                    if (GameManager.Inst.CurrentSongTimeSmoothed < DataManager.inst.gameData.beatmapData.checkpoints[i].time && DataManager.inst.gameData.beatmapData.checkpoints[i].time < min)
                    {
                        min = DataManager.inst.gameData.beatmapData.checkpoints[i].time;
                        idx2 = i;
                    }
            }
            bool outOfRange = !IsEditor ? idx == DataManager.inst.gameData.beatmapData.checkpoints.Count - 1 : idx2 == -1;

            //if the current checkpoint is CUTSCENE type
            if (DataManager.inst.gameData.beatmapData.checkpoints[idx].name == "!CUTSCENE")
            {
                ///Do once
                if (!isCutsceneFlag)
                {
                    //get destiination time of rewinding
                    destTime = (!outOfRange ?
                        DataManager.inst.gameData.beatmapData.checkpoints[idx2].time :
                        GameManager.inst.CurrentSongLength) - 0.2f;
                    //Hide progress bar
                    GameManager.inst.Timeline.localScale = Vector3.zero;
                    //tween text
                    Plugin.SkipLabel.enabled = true;
                    TweenText();
                    isCutsceneFlag = true;
                    bypassedFlag = false;
                }

                //glitch text
                if (0 < et && et < dur)
                {
                    //tween glitch amount
                    et += Time.deltaTime;
                    glitch = Mathf.Lerp(0, 1, Mathf.Clamp01(Mathf.Pow(et / dur * 2 - 1, 40) * 1.01f - 0.01f));
                    //generate glitches
                    sb = new(text);
                    for (int i = 0; i < glitch * sb.Length; i++)
                        sb[Random.Range(0, sb.Length)] = "░▒▓█"[Random.Range(0, 3)];
                    Plugin.SkipLabel.text = sb.ToString();
                    sb.Clear();
                }
                Plugin.SkipLabel.enabled = glitch < 1;

                //Rewind to the next checkpoint
                if ((!IsEditor ? GameManager.inst.CurGameState == GameManager.GameState.Playing : AudioManager.Inst.IsPlaying) && VyInput.GetKeyDown() && !bypassedFlag)
                {
                    //start rewind
                    hitSkipTime = GameManager.inst.CurrentSongTimeSmoothed;
                    LSEffectsManager.Inst.activeGlitchProfile = profile;
                    //Break label tween
                    if (Plugin.SkipLabel.enabled) BreakTweenText();
                    bypassedFlag = true;
                }
                if(!bypassedFlag) pitchState = AudioManager.inst.AudioPlaybackSpeed;

                //rewind
                if (hitSkipTime <= GameManager.inst.CurrentSongTimeSmoothed && GameManager.inst.CurrentSongTimeSmoothed <= destTime + Time.deltaTime)
                {
                    float prg = -Mathf.Pow((GameManager.inst.CurrentSongTimeSmoothed - hitSkipTime) / (destTime - hitSkipTime) * 2 - 1, 6) + 1;
                    LSEffectsManager.inst.glitchOverrideBlend = Mathf.Lerp(0, 1, prg);
                    LSEffectsManager.inst.ResolveGlitchValues();
                    AudioManager.inst.AudioPlaybackSpeed = Mathf.Lerp(pitchState, pitchState + (destTime - hitSkipTime) / 2.2f, prg);
                    Plugin.RewindIcon.alpha = Mathf.Round((GameManager.inst.CurrentSongTimeSmoothed - hitSkipTime) * 0.2f % 1);
                }
            }

            //else exit cutscene mode, do once
            else
            {
                if (isCutsceneFlag)
                {
                    GameManager.inst.Timeline.localScale = Vector3.one;
                    Plugin.RewindIcon.alpha = 0;
                    Plugin.SkipLabel.enabled = false;
                    hitSkipTime = destTime = float.MaxValue;
                    AudioManager.inst.AudioPlaybackSpeed = pitchState;
                    LSEffectsManager.inst.glitchOverrideBlend = 0;
                    isCutsceneFlag = bypassedFlag = false;
                }
            }
        }
    }

    public class VyInput
    {
        public static bool IsTyping
        {
            get
            {
                try
                {
                    if (EventSystem.current.currentSelectedGameObject != null)
                        if (EventSystem.current.currentSelectedGameObject.TryGetComponent(out TMP_InputField _))
                            return true;
                        else return false;
                    else return false;
                }
                catch { return false; }

            }
        }
        public static bool GetKeyDown() => !IsTyping && Plugin.key.Value.IsDown();
    }
}