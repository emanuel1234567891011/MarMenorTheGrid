using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace cube_game.Drawing_on_3D_game_objects
{
    public class Audio_manager : MonoBehaviour
    {
        public static Audio_manager instance;

        public AudioSource audio_source;
        public AudioClip audio_clip;

        void Awake()
        {
            Audio_manager.instance = this;
        }

        public void play_btn_audio()
        {
            if (this.audio_source != null && this.audio_clip != null)
                this.audio_source.PlayOneShot(this.audio_clip, 0.1f);
        }
    }
}