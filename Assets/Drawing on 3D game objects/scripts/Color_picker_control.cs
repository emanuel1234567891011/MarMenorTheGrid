using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace cube_game.Drawing_on_3D_game_objects
{
    public class Color_picker_control : MonoBehaviour
    {
        [Header("slider-------------------")]
        public Slider silder_R;
        public Slider slider_G;
        public Slider slider_B;

        [Header("InputField-------------------")]
        public InputField input_field_R;
        public InputField input_field_G;
        public InputField input_field_B;

        [Header("image preview-------------------")]
        public Image image_prefview;

        [Header("color")]
        public Color color;


        [Header("Drawing control")]
        public Drawing_control drawing_control;

        void Start()
        {
            //event
            this.silder_R.onValueChanged.AddListener(this.update_color_slider);
            this.slider_G.onValueChanged.AddListener(this.update_color_slider);
            this.slider_B.onValueChanged.AddListener(this.update_color_slider);

            this.input_field_R.onEndEdit.AddListener(this.update_color_input_field);
            this.input_field_G.onEndEdit.AddListener(this.update_color_input_field);
            this.input_field_B.onEndEdit.AddListener(this.update_color_input_field);

            this.update_color_slider(0);
        }


        public void update_color_slider(float value)
        {
            float red = this.silder_R.value;
            float green = this.slider_G.value;
            float blue = this.slider_B.value;

            this.input_field_R.text = red + "";
            this.input_field_G.text = green + "";
            this.input_field_B.text = blue + "";


            this.color = new Color(red, green, blue);


            if (this.image_prefview != null)
            {
                this.image_prefview.color = this.color;
            }

            if (this.drawing_control != null)
            {
                this.drawing_control.set_color(this.color);
            }
        }

        public void update_color_input_field(string value)
        {
            float red = float.Parse(this.input_field_R.text);
            float green = float.Parse(this.input_field_G.text);
            float blue = float.Parse(this.input_field_B.text);

            this.silder_R.value = red;
            this.slider_G.value = green;
            this.slider_B.value = blue;


            this.color = new Color(red, green, blue);

            if (this.image_prefview != null)
            {
                this.image_prefview.color = this.color;
            }

            if (this.drawing_control != null)
            {
                this.drawing_control.set_color(this.color);
            }
        }
    }
}
