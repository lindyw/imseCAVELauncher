using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraStream : MonoBehaviour {
    public ECNetwork connection;
    public float moveSpeed = 10;
    public float turnSpeed = 50;
    public GameObject movingObject;
    public GameObject[] cameras = new GameObject[2];
    public GameObject vrWand;
    
	// Use this for initialization
	void Start () {
        vrWand = GameObject.Find("VRWand");

        if (connection.type != ECNetwork.Type.CLIENT)
        {
            //cameras[1].SetActive(false);
            if (vrWand)
            {
                movingObject.transform.parent = vrWand.transform;
                movingObject.transform.localPosition = Vector2.zero;
            } 
        }
        else
        {
            cameras[1].GetComponent<Camera>().targetTexture = null;
            cameras[0].SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (connection.type != ECNetwork.Type.CLIENT)
        {
            if (Input.GetKey(KeyCode.UpArrow))
                movingObject.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.DownArrow))
                movingObject.transform.Translate(-Vector3.forward * moveSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.LeftArrow))
                movingObject.transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.RightArrow))
                movingObject.transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);

            if (connection.state == ConnectionState.CONNECTED)
            {
                if (connection.receivedData.Length == 1)
                {
                    System.IO.File.WriteAllBytes("ScreenCap.png", ECCommons.RenderTexTo2D(cameras[1].GetComponent<Camera>().targetTexture).EncodeToPNG());
                    
                }
                connection.DataTransfer(ECCommons.Combine(", ", 
                    movingObject.transform.position.x,
                    movingObject.transform.position.y,
                    movingObject.transform.position.z,
                    movingObject.transform.eulerAngles.x,
                    movingObject.transform.eulerAngles.y,
                    movingObject.transform.eulerAngles.z));
            }
            /*
            if (Input.GetKey(KeyCode.W))
                vrWand.transform.Translate(Vector3.up * Time.deltaTime);

            if (Input.GetKey(KeyCode.S))
                vrWand.transform.Translate(-Vector3.up * Time.deltaTime);

            if (Input.GetKey(KeyCode.A))
                vrWand.transform.Translate(Vector3.left * Time.deltaTime);

            if (Input.GetKey(KeyCode.D))
                vrWand.transform.Translate(-Vector3.left * Time.deltaTime);
            */
            if (Input.GetAxis("6th") != 0) vrWand.transform.Translate(-Vector3.left * Time.deltaTime * Input.GetAxis("6th"));
            if (Input.GetAxis("7th") != 0) vrWand.transform.Translate(Vector3.up * Time.deltaTime * Input.GetAxis("7th"));
        }
        else 
        {
            if (connection.state == ConnectionState.CONNECTED && connection.receivedData.Length == 6)
            {
                string[] data = ECCommons.Separate(", ", connection.receivedData);
                Vector3 pos = new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));
                Vector3 rot = new Vector3(float.Parse(data[3]), float.Parse(data[4]), float.Parse(data[5]));
                movingObject.transform.position = pos;
                movingObject.transform.eulerAngles = rot;
            }
            else if (connection.state != ConnectionState.CONNECTED)
            {
                connection.autoConnect = true;
                connection.Disconnect();
            }
        }
	}
    public void OnGUI()
    {
        if (connection.state == ConnectionState.CONNECTED && connection.type == ECNetwork.Type.CLIENT)
            if(GUI.Button(new Rect(0, 0, 100, 100), "CAP"))
            {
                connection.DataTransfer("CAP");
            }
    }
}
