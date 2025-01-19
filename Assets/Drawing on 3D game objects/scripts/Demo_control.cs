using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace cube_game.Drawing_on_3D_game_objects
{
    public class Demo_control : MonoBehaviour
    {
        [Header("game_objects")]
        public GameObject[] game_objects;

        [Header("draw_tool")]
        public GameObject[] draw_tools;


        private int index = 0;

        private int fingerID = -1;


        public void on_previous_btn_click()
        {
            this.index--;
            if (index < 0)
            {
                this.index = this.game_objects.Length - 1;
            }

            for (int i = 0; i < this.game_objects.Length; i++)
            {
                if (this.index == i)
                {
                    this.game_objects[i].SetActive(true);
                    this.draw_tools[i].SetActive(true);
                }
                else
                {
                    this.game_objects[i].SetActive(false);
                    this.draw_tools[i].SetActive(false);
                }
            }

            Audio_manager.instance.play_btn_audio();
        }


        public void on_next_btn_click()
        {
            this.index++;
            if (index > (this.game_objects.Length - 1))
            {
                this.index = 0;
            }

            for (int i = 0; i < this.game_objects.Length; i++)
            {
                if (this.index == i)
                {
                    this.game_objects[i].SetActive(true);
                    this.draw_tools[i].SetActive(true);
                }
                else
                {
                    this.game_objects[i].SetActive(false);
                    this.draw_tools[i].SetActive(false);
                }
            }

            Audio_manager.instance.play_btn_audio();
        }
    }
}
