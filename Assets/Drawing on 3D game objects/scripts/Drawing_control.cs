using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace cube_game.Drawing_on_3D_game_objects
{
    public class Drawing_control : MonoBehaviour
    {
        public MeshRenderer _mesh;

        [Header("Game objects that need to be drawn must contain a mesh renderer")]
        public GameObject drawn_gameobject;

        //The material of the game object that needs to be drawn
        private Material drawn_gameobject_material;

        //Render texture, Used to be drawn
        private RenderTexture RT_draw;
        private RenderTexture RT_erase;

        [Header("Texture Brush�� Texture earse")]
        public Texture texture_brush;
        public Texture texture_erase;

        //Brush thickness,
        private float brush_thickness = 0.1f;

        //color
        private Color color;

        [Header("Paint Statu")]
        public Drawing_statu drawing_statu = Drawing_statu.draw;

        [Header("cursor")]
        public Texture2D texture_cursor_draw;
        public Texture2D texture_cursor_erase;
        private Texture2D texture_cursor;

        [Header("view control")]
        private View_control view_control;

        [Header("UI------------")]
        public Button btn_camera_reset;
        public Button btn_save_texture;
        public Button btn_redraw;
        public Slider slider_brush_thickness;

        public bool CanDraw = false;

        void Start()
        {
            #region 
            if (this.btn_camera_reset != null)
                this.btn_camera_reset.onClick.AddListener(on_btn_camera_reset);
            if (this.btn_save_texture != null)
                this.btn_save_texture.onClick.AddListener(on_btn_save_texture);
            if (this.btn_redraw != null)
                this.btn_redraw.onClick.AddListener(on_btn_redraw);
            if (this.slider_brush_thickness != null)
                this.slider_brush_thickness.onValueChanged.AddListener(on_slider_brush_thickness);
            #endregion

            if (this.GetComponent<View_control>() != null)
            {
                this.view_control = this.GetComponent<View_control>();
            }

            //Code to create a render texture
            this.RT_draw = new RenderTexture(1024, 2048, 24, RenderTextureFormat.ARGB32);
            this.RT_erase = new RenderTexture(1024, 2048, 24, RenderTextureFormat.ARGB32);


            //The code gets the material and sets the texture
            if (this.drawn_gameobject.GetComponent<Renderer>() != null)
            {
                this.drawn_gameobject_material = this.drawn_gameobject.GetComponent<Renderer>().material;
                this.drawn_gameobject_material.SetTexture("_render_texture_draw", this.RT_draw);
                this.drawn_gameobject_material.SetTexture("_render_texture_erase", this.RT_erase);
            }

            brush_thickness = .75f;
        }

        public void SetCanDraw(bool state)
        {
            CanDraw = state;
        }

        public void Init()
        {
            draw(0, 0, this.RT_draw, this.RT_erase, this.texture_brush, 0);
        }

        void Update()
        {
            if (CanDraw == false)
                return;

            //draw
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == this.drawn_gameobject)
                    {
                        this.drawing_statu = Drawing_statu.draw;
                        this.set_cursor();

                        int x = (int)(hit.textureCoord.x * this.RT_draw.width);
                        int y = (int)(this.RT_draw.height - hit.textureCoord.y * this.RT_draw.height);

                        // Texture2D tex = (Texture2D)drawn_gameobject_material.GetTexture("Texture2D_3f3542f4b23c413d97123944acaa3ef7");
                        // if (tex.GetPixel(x, y) == Color.blue)

                        this.draw(x, y, this.RT_draw, this.RT_erase, this.texture_brush, this.brush_thickness);
                    }
                }
            }

            //erase
            else if (Input.GetMouseButton(1) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == this.drawn_gameobject)
                    {
                        this.drawing_statu = Drawing_statu.erase;
                        this.set_cursor();

                        //todo Get all pixels in radius from point and add them to a list which we will add to the map data.

                        int x = (int)(hit.textureCoord.x * this.RT_draw.width);
                        int y = (int)(this.RT_draw.height - hit.textureCoord.y * this.RT_draw.height);

                        this.erase(x, y, this.RT_erase, this.texture_erase, this.brush_thickness);
                    }
                }
            }
            else
            {
                this.drawing_statu = Drawing_statu.none;
                this.set_cursor();
            }
        }

        //clear the rendertexture
        public void clear(RenderTexture render_texture_erase, RenderTexture render_texture_paint)
        {
            RenderTexture.active = render_texture_erase;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;

            RenderTexture.active = render_texture_paint;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;
        }

        //erase
        public void erase(int x, int y, RenderTexture render_texture_erase, Texture texture, float brush_thickness)
        {
            RenderTexture.active = render_texture_erase;

            GL.PushMatrix();
            GL.LoadPixelMatrix(0, render_texture_erase.width, render_texture_erase.height, 0);

            x -= (int)(texture.width * 0.5f * this.brush_thickness);
            y -= (int)(texture.height * 0.5f * this.brush_thickness);
            Rect rect = new Rect(x, y, texture.width * brush_thickness * 1.25f, texture.height * brush_thickness * 1.25f);
            //Graphics.DrawTexture(rect, texture_cirlce_erase, mat_brush_erase, 0);
            Graphics.DrawTexture(rect, texture, new Rect(0, 0, 1, 1), 0, 0, 0, 0, Color.white);

            GL.PopMatrix();
            RenderTexture.active = null;
        }

        //paint
        private void draw(int x, int y, RenderTexture render_texture_paint, RenderTexture render_texture_erase, Texture texture, float brush_thickness)
        {
            RenderTexture.active = render_texture_paint;
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, render_texture_paint.width, render_texture_paint.height, 0);

            //set position
            x -= (int)(texture.width * 0.5f * brush_thickness);
            y -= (int)(texture.height * 0.5f * brush_thickness);



            //todo Get all pixels in radius of point.

            //paint on render_texture_paint
            Rect rect = new Rect(x, y, texture.width * brush_thickness, texture.height * brush_thickness);
            //Graphics.DrawTexture(rect, texture_brush, mat_brush_paint, 0,Color.red);
            Graphics.DrawTexture(rect, texture, new Rect(0, 0, 1, 1), 0, 0, 0, 0, this.color);
            GL.PopMatrix();


            //paint on render_texture_erase---------------------------------------------------------------------------------------------------------------------------
            RenderTexture.active = render_texture_erase;
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, render_texture_erase.width, render_texture_erase.height, 0);
            //Graphics.DrawTexture(rect, texture_brush, mat_brush_paint_1, 0);
            Graphics.DrawTexture(rect, texture, new Rect(0, 0, 1, 1), 0, 0, 0, 0, Color.black);
            GL.PopMatrix();
            RenderTexture.active = null;
        }

        public void SaveDrawingToMap()
        {
            RenderTexture.active = RT_draw;
            Texture2D tex = new Texture2D(RT_draw.width, RT_draw.height);
            tex.ReadPixels(new Rect(0, 0, RT_draw.width, RT_draw.height), 0, 0);
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            _mesh.material.mainTexture = tex;

            List<Vector2Int> _swimmingAreaPos = new List<Vector2Int>();

            for (int x = 0; x < tex.width; x++)
                for (int y = 0; y < tex.height; y++)
                {
                    Color c = tex.GetPixel(x, y);
                    if (c == Color.black)
                        _swimmingAreaPos.Add(new Vector2Int(x, y));
                }

            FindAnyObjectByType<GridManager>().SetSwimmingArea(_swimmingAreaPos);

            //todo try to figure out how to get the black pixels, if that doesn't work then try to get the radius from the brush point and add those grid locations to border grid list.

            // GridManager gm = FindAnyObjectByType<GridManager>();
            // RenderTexture render_texture = this.RT_draw;
            // RenderTexture.active = render_texture;
            // Texture2D texture2D = new Texture2D(render_texture.width, render_texture.height);
            // texture2D.ReadPixels(new Rect(0, 0, render_texture.width, render_texture.height), 0, 0);
            // texture2D.Apply();
            // gm.mapData.modifiedBitmap = texture2D;
            // Debug.Log(texture2D.width + " / " + texture2D.height);
            // RenderTexture.active = null;
        }

        public Texture2D save_texture(RenderTexture render_texture, string filePath)
        {
            //Create a new Texture2D object with the same dimensions as the RenderTexture
            RenderTexture.active = render_texture;
            Texture2D texture2D = new Texture2D(render_texture.width, render_texture.height);

            //Read pixels from RenderTexture into Texture2D
            texture2D.ReadPixels(new Rect(0, 0, render_texture.width, render_texture.height), 0, 0);
            texture2D.Apply();

            //Save Texture2D as PNG file
            byte[] bytes = texture2D.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes); //Application.dataPath + "/SavedRenderTexture.png"

            //Clear the active RenderTexture
            RenderTexture.active = null;

            Debug.Log("RenderTexture saved as PNG at: " + filePath);

            return texture2D;
        }

        public void set_color(Color color)
        {
            this.color = color;
            //Debug.Log("Color setting successful");
        }

        public void set_brush(Texture texture)
        {
            this.texture_brush = texture;
        }

        //set cursor------------------
        private void set_cursor()
        {
            switch (this.drawing_statu)
            {
                case Drawing_statu.draw:
                    if (this.texture_cursor != this.texture_cursor_draw)
                    {
                        Cursor.SetCursor(this.texture_cursor_draw, new Vector2(0, this.texture_cursor_draw.height), CursorMode.Auto);
                        this.texture_cursor = this.texture_cursor_draw;
                    }
                    break;

                case Drawing_statu.erase:
                    if (this.texture_cursor != this.texture_cursor_erase)
                    {
                        Cursor.SetCursor(this.texture_cursor_erase, new Vector2(this.texture_cursor_draw.width / 3f, this.texture_cursor_draw.height / 2f), CursorMode.Auto);
                        this.texture_cursor = this.texture_cursor_erase;
                    }
                    break;

                case Drawing_statu.none:
                    if (this.texture_cursor != null)
                    {
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                        this.texture_cursor = null;
                    }
                    break;

                default:
                    break;
            }
        }

        //ui event
        #region 
        public void on_btn_camera_reset()
        {
            if (this.view_control != null)
            {
                this.view_control.reset_camera_transform();
                Audio_manager.instance.play_btn_audio();
                Debug.Log("The camera position has been reset");
            }
        }

        public void on_btn_save_texture()
        {
            this.save_texture(this.RT_draw, Application.dataPath + "/Drawing on 3D game objects/textures_drawing/" + this.drawn_gameobject.name + Time.time + "_draw.png");
            this.save_texture(this.RT_erase, Application.dataPath + "/Drawing on 3D game objects/textures_drawing/" + this.drawn_gameobject.name + Time.time + "_erase.png");
            Audio_manager.instance.play_btn_audio();
        }

        public void on_btn_redraw()
        {
            this.clear(this.RT_erase, this.RT_draw);
            Audio_manager.instance.play_btn_audio();
            Debug.Log("Redraw");
        }

        private void on_slider_brush_thickness(float value)
        {
            this.brush_thickness = value;
            Debug.Log("Change brush thickness to: " + value);
        }


        #endregion
    }

    public enum Drawing_statu
    {
        draw,
        erase,
        none
    }
}
