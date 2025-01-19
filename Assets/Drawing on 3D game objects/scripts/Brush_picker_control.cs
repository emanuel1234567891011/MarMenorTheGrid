using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace cube_game.Drawing_on_3D_game_objects
{
    public class Brush_picker_control : MonoBehaviour
    {
        [Header("btns")]
        public Button[] btns_brush_choose;

        [Header("texture")]
        public Texture[] textures_brush;

        [Header("Drawing control")]
        public Drawing_control drawing_control;

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < this.btns_brush_choose.Length; i++)
            {
                int index = i; 


                if (this.btns_brush_choose[i] == null)
                {
                    Debug.LogError("Button at index " + i + " is null!");
                }
                else
                {
                    this.btns_brush_choose[i].onClick.AddListener(() => this.on_brush_choose_btns(index));
                }

            }

            this.drawing_control.set_brush(this.textures_brush[0]);
        }


        public void on_brush_choose_btns(int index)
        {
            for (int i = 0; i < this.btns_brush_choose.Length; i++)
            {
                if (i == index)
                {
                    this.btns_brush_choose[i].transform.Find("Image").gameObject.SetActive(true);
                }
                else
                {
                    this.btns_brush_choose[i].transform.Find("Image").gameObject.SetActive(false);
                }
            }
            this.drawing_control.set_brush(this.textures_brush[index]);

            Audio_manager.instance.play_btn_audio();
        }
    }
}