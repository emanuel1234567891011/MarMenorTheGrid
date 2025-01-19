using UnityEngine;
using UnityEngine.EventSystems;

namespace cube_game.Drawing_on_3D_game_objects
{
    [RequireComponent(typeof(Drawing_control))]
    public class View_control : MonoBehaviour
    {
        [Header("Camera movement speed")]
        public float move_speed = 10f; 

        [Header("Camera rotation speed")]
        public float rotate_speed = 10.0f; 
        private Vector3 last_mouse_position;

        [Header("Camera zoom speed, as well as maximum and minimum zoom distances")]
        public float zoom_speed = 1.0f; 
        public float min_distance = 0f; 
        public float max_distance = 50.0f;

        [Header("cursor texture-----------")]
        public Texture2D texture_cursor_move;
        public Texture2D texture_cursor_rotate;
        public Texture2D texture_cursor_zoom;
        private Texture2D texture_cursor;

        //Drawing_control and target
        private Drawing_control drawing_control;
        private Transform target;

        [Header("View_control_statu")]
        public View_control_statu view_control_statu = View_control_statu.none;

        //camera transform
        private Vector3 v3_init_postion;
        private Quaternion qua_init_rotation;

        void Start()
        {
            //Store the initial position information of the camera
            this.v3_init_postion = Camera.main.transform.position;
            this.qua_init_rotation = Camera.main.transform.rotation;


            this.drawing_control = this.GetComponent<Drawing_control>();
            this.target = this.drawing_control.drawn_gameobject.transform;
        }

        void Update()
        {
            //If it is in the drawing state, it will not be processed
            if (this.drawing_control.drawing_statu != Drawing_statu.none)
                return;

            // move
            #region 
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) //control key
            {
                this.view_control_statu = View_control_statu.move;

                //Left mouse button
                if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) 
                {
                    this.set_cursor();

                    float moveX = -Input.GetAxis("Mouse X") * move_speed * Time.deltaTime;
                    float moveY = -Input.GetAxis("Mouse Y") * move_speed * Time.deltaTime;

                    //Move the camera based on the amount of mouse drag
                    Camera.main.transform.Translate(new Vector3(moveX, moveY, 0));

                    return;
                }

            }
            else
            {
                this.view_control_statu = View_control_statu.none;
                this.set_cursor();
            }

            #endregion

            // rotate
            #region 
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                this.view_control_statu = View_control_statu.roate;

                if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    this.set_cursor();
                    this.rotate_camera();
                    last_mouse_position = Input.mousePosition;
                    return;
                }
            }
            else
            {
                this.view_control_statu = View_control_statu.none;
                this.set_cursor();
            }
            last_mouse_position = Input.mousePosition;
            #endregion

            // zoom
            #region 
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0.0f)
            {
                this.view_control_statu = View_control_statu.zoom;
                this.set_cursor();
                this.Zoom_camera(scroll);
                return;
            }
            #endregion

        }

        //set cursor
        private void set_cursor()
        {
            switch (this.view_control_statu)
            {
                case View_control_statu.move:
                    if (this.texture_cursor != this.texture_cursor_move)
                    {
                        Cursor.SetCursor(this.texture_cursor_move, new Vector2(this.texture_cursor_move.width / 2f, this.texture_cursor_move.height / 2f), CursorMode.Auto);
                        this.texture_cursor = this.texture_cursor_move;
                    }
                    break;

                case View_control_statu.roate:
                    if (this.texture_cursor != this.texture_cursor_rotate)
                    {
                        Cursor.SetCursor(this.texture_cursor_rotate, new Vector2(this.texture_cursor_move.width / 2f, this.texture_cursor_move.height / 2f), CursorMode.Auto);
                        this.texture_cursor = this.texture_cursor_rotate;
                    }
                    break;


                case View_control_statu.zoom:
                    if (this.texture_cursor != this.texture_cursor_zoom)
                    {
                        Cursor.SetCursor(this.texture_cursor_zoom, new Vector2(this.texture_cursor_move.width / 2f, this.texture_cursor_move.height / 2f), CursorMode.Auto);
                        this.texture_cursor = this.texture_cursor_zoom;
                    }
                    break;

                case View_control_statu.none:
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

        //Realize the rotation of the camera around the target
        void rotate_camera()
        {
            //Get the mouse displacement
            Vector3 delta = Input.mousePosition - last_mouse_position;


            float rotationX = delta.x * rotate_speed * Time.deltaTime;
            float rotationY = -delta.y * rotate_speed * Time.deltaTime;

            //if (Mathf.Abs(rotationX) > Mathf.Abs(rotationY))
            //{
                //Horizontal rotation around the Y axis
                Camera.main.transform.RotateAround(target.position, Vector3.up, rotationX);
            //}
            //else
            //{
            //    //Vertical rotation around the X axis
            //    Vector3 right = transform.right; //Get the right vector of the camera
            //    Camera.main.transform.RotateAround(target.position, right, rotationY);
            //}
        }

        void Zoom_camera(float scroll)
        {
            //Zoom the camera based on scroll wheel input
            Vector3 direction = Camera.main.transform.forward;
            Vector3 position = Camera.main.transform.position;

            //Calculate the target position after scaling
            Vector3 targetPosition = position + direction * scroll * zoom_speed;

            //Limit the minimum and maximum distances for camera zoom
            float distance = Vector3.Distance(targetPosition, Vector3.zero);
            if (distance >= min_distance && distance <= max_distance)
            {
                Camera.main.transform.position = targetPosition;
            }
        }

        //reset camera transform
        public void reset_camera_transform()
        {
            Camera.main.transform.position = this.v3_init_postion;
            Camera.main.transform.rotation = this.qua_init_rotation;
        }
    }

    public enum View_control_statu
    {
        move,
        roate,
        zoom,
        none
    }
}