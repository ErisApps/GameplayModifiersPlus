﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System.Reflection;
using System.IO;
namespace GamePlayModifiersPlus
{
    internal class EndlessBehavior : MonoBehaviour
    {
        private CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        private SongProgressUIController progessController;
        private AudioClip nextSong;
        private BeatmapData nextBeatmap;
        private CustomPreviewBeatmapLevel nextSongInfo;
        private StandardLevelInfoSaveData.DifficultyBeatmap nextMapDiffInfo;
        private PauseMenuManager pauseManager;
        private float switchTime = float.MaxValue;

        private BeatmapObjectCallbackController callbackController;
        private BeatmapObjectSpawnMovementData originalSpawnMovementData;
        private NoteCutSoundEffectManager seManager;
        private BeatmapDataLoader dataLoader = new BeatmapDataLoader();

        void Awake()
        {
            StartCoroutine(Setup());
        }
        private IEnumerator Setup()
        {
            yield return new WaitForSeconds(0.1f);
            callbackController = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>().First();
            originalSpawnMovementData = Plugin.spawnController.GetField<BeatmapObjectSpawnMovementData>("_beatmapObjectSpawnMovementData");
            seManager = Resources.FindObjectsOfTypeAll<NoteCutSoundEffectManager>().First();
            progessController = Resources.FindObjectsOfTypeAll<SongProgressUIController>().First();
            pauseManager = Resources.FindObjectsOfTypeAll<PauseMenuManager>().First();
            // switchTime = 20f;
            switchTime = Plugin.songAudio.clip.length - 1f;
            IPA.Utilities.Async.UnityMainThreadTaskScheduler.Factory.StartNew(PrepareNextSong);
        }
        void Update()
        {
            if (Plugin.songAudio.time >= switchTime && nextSong != null)
            {
                //   switchTime = 20f;
                switchTime = nextSong.length - 1f;
                SwitchToNextMap();
            }
        }

        private void SwitchToNextMap()
        {
            if (BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.playerSpecificSettings.staticLights)
                nextBeatmap.SetProperty<BeatmapData>("beatmapEventData", new BeatmapEventData[0]);

            AudioClip oldClip = Plugin.songAudio.clip;
            TwitchPowers.ResetTimeSync(nextSong, 0f, nextSongInfo.songTimeOffset, 1f);
            TwitchPowers.ManuallySetNJSOffset(Plugin.spawnController, nextMapDiffInfo.noteJumpMovementSpeed,
                nextMapDiffInfo.noteJumpStartBeatOffset, nextSongInfo.beatsPerMinute);
            //    TwitchPowers.ClearCallbackItemDataList(callBackDataList);
            // DestroyNotes();
            TwitchPowers.DestroyObjectsRaw();
            TwitchPowers.ResetNoteCutSoundEffects(seManager);
            callbackController.SetField("_spawningStartTime", 0f);
            callbackController.SetNewBeatmapData(nextBeatmap);
            ResetProgressUI();
            UpdatePauseMenu();
            ClearSoundEffects();
            CheckIntroSkip();
            //Destroying audio clip is actually bad idea
            //   IPA.Utilities.Async.UnityMainThreadTaskScheduler.Factory.StartNew(() => { oldClip.UnloadAudioData(); AudioClip.Destroy(oldClip); });
            IPA.Utilities.Async.UnityMainThreadTaskScheduler.Factory.StartNew(PrepareNextSong);
        }

        private void ClearSoundEffects()
        {
            seManager.GetField<NoteCutSoundEffect.Pool>("_noteCutSoundEffectPool").Clear();
        }
        private void ResetProgressUI()
        {
            progessController.Start();
            var cPlusCounter = GameObject.Find("Counters+ | Progress Counter");
            if (cPlusCounter != null)
            {
                ResetCountersPlusCounter(cPlusCounter);
            }

        }
        private void UpdatePauseMenu()
        {
            var currInitData = pauseManager.GetField<PauseMenuManager.InitData>("_initData");
            PauseMenuManager.InitData newData = new PauseMenuManager.InitData(currInitData.backButtonText, nextSongInfo.songName, nextSongInfo.songSubName, nextMapDiffInfo.difficulty);
            pauseManager.SetField("_initData", newData);
            pauseManager.Start();
        }
        private void ResetCountersPlusCounter(GameObject counter)
        {
            counter.GetComponent<CountersPlus.Counters.ProgressCounter>().SetField("length", nextSong.length);
        }

        private void CheckIntroSkip()
        {
            var skip = GameObject.Find("IntroSkip Behavior");
            if (skip != null)
                ResetIntroSkip(skip);
        }

        private void ResetIntroSkip(GameObject skip)
        {
            bool practice = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.practiceSettings != null;
            if (practice || BS_Utils.Gameplay.Gamemode.IsIsolatedLevel) return;

            var skipBehavior = skip.GetComponent<IntroSkip.SkipBehavior>();
            skipBehavior.StartCoroutine(skipBehavior.ReadMap());
        }
        private async Task PrepareNextSong()
        {
            try
            {
                bool validSong = false;

                while (!validSong)
                {
                    await Task.Yield();
                    int nextSongIndex = UnityEngine.Random.Range(0, SongCore.Loader.CustomLevels.Count);
                    nextSongInfo = SongCore.Loader.CustomLevels.ElementAt(nextSongIndex).Value;
                    validSong = IsValid(nextSongInfo);
                }

                nextMapDiffInfo = nextSongInfo.standardLevelInfoSaveData.difficultyBeatmapSets[0].difficultyBeatmaps.Last();
                nextSong = await nextSongInfo.GetPreviewAudioClipAsync(CancellationTokenSource.Token);
                //   bool loaded;
                //  await Task.Run(() => loaded = nextSong.LoadAudioData());

                string path = Path.Combine(nextSongInfo.customLevelPath, nextMapDiffInfo.beatmapFilename);
                string json = File.ReadAllText(path);
                nextBeatmap = dataLoader.GetBeatmapDataFromJson(json, nextSongInfo.beatsPerMinute, nextSongInfo.shuffle, nextSongInfo.shufflePeriod);
                Plugin.Log($"Next Song: {nextSongInfo.songName} - Mapped by {nextSongInfo.levelAuthorName}, is Ready");
            }
            catch (Exception ex)
            {
                Plugin.Log(ex.ToString());
            }
        }

        private bool IsValid(CustomPreviewBeatmapLevel level)
        {
            var extraData = SongCore.Collections.RetrieveExtraSongData(level.levelID, level.customLevelPath);
            if (extraData == null)
            {
                Plugin.Log("Null Extra Data");
                return false;
            }

            List<string> requirements = new List<string>();

            foreach (var diff in extraData._difficulties)
                requirements.AddRange(diff.additionalDifficultyData._requirements);

            if (requirements.Count > 0) //(x => !SongCore.Collections.capabilities.Contains(x)))
            {
                Plugin.Log("Req Present");
                return false;
            }

            if (level.previewDifficultyBeatmapSets.Any(x => x.beatmapCharacteristic.containsRotationEvents))
            {
                Plugin.Log("360 map");
                return false;
            }


            return true;
        }


    }
}
