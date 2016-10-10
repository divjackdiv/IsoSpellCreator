using UnityEngine;
using System.Collections;

public class camera : MonoBehaviour {
 	public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;

  	private float currentZoom;
    public float maxZoom;
    public float minZoom;

    void Awake(){
    	if (target)
        {
            transform.position = new Vector3 (target.position.x, target.position.y, transform.position.z);
        }
    }
    void Start(){
    	currentZoom = Camera.main.orthographicSize;
    }

	void Update () {
		if (target)
        {
             Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);
             Vector3 delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); 
             Vector3 destination = transform.position + delta;
             transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }

		if(Input.GetAxis("Mouse ScrollWheel") != 0)
		{

			if (Input.GetAxis("Mouse ScrollWheel") > 0)
			{
				if(currentZoom > minZoom){
					zoom(-0.2f);
				}
			}
			else if(Input.GetAxis("Mouse ScrollWheel") < 0)
			{	
				if(currentZoom < maxZoom){
					zoom(0.2f);
				}
			}  
		}
	}


	void zoom(float direction){
		Camera.main.orthographicSize += direction;
		currentZoom += direction;
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);			
	}
}