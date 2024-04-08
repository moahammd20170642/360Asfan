﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace CrizGames.Vour
{
    public class VideoController : MonoBehaviour
    {
        public class EmptyEvent : UnityEvent { } // Technically not required, but it removes the inspector UI
        public class BoolEvent : UnityEvent<bool> { }
        public class FloatEvent : UnityEvent<float> { }
        public class StringEvent : UnityEvent<string> { }

        public EmptyEvent OnInit = new EmptyEvent();

        public EmptyEvent OnVideoLoaded = new EmptyEvent();
        public StringEvent OnVideoError = new StringEvent();

        public BoolEvent OnPlayStateChanged = new BoolEvent();

        public FloatEvent OnVideoProgress = new FloatEvent();
        public EmptyEvent OnVideoEnded = new EmptyEvent();

        public FloatEvent OnAudioVolumeChanged = new FloatEvent();
        public BoolEvent OnAudioMuteStateChanged = new BoolEvent();


        public bool isPlaying => videoPlayer.isPlaying;
        public bool isLooping => videoPlayer.isLooping;
        public bool hasVideoEnded { get; private set; }

        public float currentVideoTime => videoPlayer.frame / videoPlayer.frameRate;
        public float currentVideoTimePercent => videoPlayer.frame / (float)videoPlayer.frameCount;
        public float videoTimeLength => videoPlayer.frameCount / videoPlayer.frameRate;

        public bool hasAudio => videoPlayer.audioTrackCount > 0;
        public float audioVolume => hasAudio ? videoPlayer.GetDirectAudioVolume(0) : 0f;
        public bool isAudioMuted => hasAudio ? videoPlayer.GetDirectAudioMute(0) : true;

        [SerializeField] private VideoPlayer videoPlayer;

        [HideInInspector] public float audioVolumeBeforeMute = 0.1f;
        private long lastFrame = 0;


        public void Init(VideoPlayer newVideoPlayer)
        {
            // Remove events from old video player
            if (videoPlayer != null)
            {
                videoPlayer.prepareCompleted -= OnPrepareCompleted;
                videoPlayer.loopPointReached -= OnLoopPointReached;
                videoPlayer.errorReceived -= OnErrorReceived;
                videoPlayer.started -= OnVideoStarted;
            }

            videoPlayer = newVideoPlayer;
            hasVideoEnded = false;

            // Add events to new video player
            videoPlayer.prepareCompleted += OnPrepareCompleted;
            videoPlayer.loopPointReached += OnLoopPointReached;
            videoPlayer.errorReceived += OnErrorReceived;
            videoPlayer.started += OnVideoStarted;

            OnInit?.Invoke();

            // If video is already loaded
            if (videoPlayer.isPrepared)
                OnPrepareCompleted();
            else
                OnPlayStateChanged?.Invoke(false);
        }

        private void LateUpdate()
        {
            if (videoPlayer == null)
                return;

            // Update progress bar
            if (isPlaying && videoPlayer.frame != lastFrame)
            {
                OnVideoProgress?.Invoke(currentVideoTimePercent);
                lastFrame = videoPlayer.frame;
            }
        }

        public void TogglePlayingState()
        {
            SetPlayingState(!isPlaying);
        }

        public void SetPlayingState(bool playing)
        {
            OnPlayStateChanged?.Invoke(playing);
            if (playing)
            {
                // Restart video
                if (hasVideoEnded)
                {
                    videoPlayer.frame = 0;
                    hasVideoEnded = false;
                }

                videoPlayer.Play();
            }
            else
            {
                videoPlayer.Pause();
            }
        }

        public void SetVideoTime(float percent)
        {
            long frame = (long)(videoPlayer.frameCount * percent);

            // Play if skipped to a place from end of video
            if (hasVideoEnded && frame != (long)videoPlayer.frameCount)
                TogglePlayingState();

            videoPlayer.frame = frame;
            OnVideoProgress?.Invoke(percent);
        }

        public void SetLooping(bool loop)
        {
            videoPlayer.isLooping = loop;

            // Restart video if it ended and loop was enabled
            if (loop && hasVideoEnded)
                TogglePlayingState();
        }

        public void SetAudioVolume(float volume)
        {
            for (ushort i = 0; i < videoPlayer.audioTrackCount; i++)
                videoPlayer.SetDirectAudioVolume(i, volume);

            // Unmute if changing volume while muted to hear the effect
            if (volume > 0f && isAudioMuted)
                SetAudioMute(false);
            // Mute if volume is 0
            else if (volume == 0f && !isAudioMuted)
                SetAudioMute(true);

            OnAudioVolumeChanged?.Invoke(volume);
        }

        public void SetAudioMute(bool muted)
        {
            if (!hasAudio)
            {
                OnAudioMuteStateChanged?.Invoke(true);
                return;
            }

            for (ushort i = 0; i < videoPlayer.audioTrackCount; i++)
                videoPlayer.SetDirectAudioMute(i, muted);

            float volume = audioVolume;
            if (muted)
            {
                audioVolumeBeforeMute = volume > 0.05f ? volume : 0.05f;
                SetAudioVolume(0f);
            }
            else
            {
                SetAudioVolume(audioVolumeBeforeMute);
            }

            OnAudioMuteStateChanged?.Invoke(muted);
        }
        
        private void OnVideoStarted(VideoPlayer _ = null)
        {
            OnPlayStateChanged?.Invoke(true);
        }

        private void OnPrepareCompleted(VideoPlayer _ = null)
        {
            OnVideoLoaded?.Invoke();

            OnAudioMuteStateChanged.Invoke(isAudioMuted);

            if (videoPlayer.playOnAwake)
                OnPlayStateChanged?.Invoke(true);
        }

        private void OnErrorReceived(VideoPlayer _, string message)
        {
            OnVideoError?.Invoke(message);
        }

        private void OnLoopPointReached(VideoPlayer _)
        {
            if (!isLooping)
            {
                OnVideoEnded?.Invoke();
                hasVideoEnded = true;
                OnPlayStateChanged?.Invoke(false);
            }
        }
    }
}