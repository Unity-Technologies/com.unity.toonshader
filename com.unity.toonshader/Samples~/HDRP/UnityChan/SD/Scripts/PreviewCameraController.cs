//Based on Scene view style camera controller
//https://discussions.unity.com/t/scene-view-style-camera-controller/561953/7
using UnityEngine;

namespace UnityChan {
public class PreviewCameraController : MonoBehaviour {

    void Start() {
        Init();
    }

    void Init() {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target) {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        m_curCamState.currentDistance = distance;
        m_curCamState.desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        m_curCamState.position = transform.position;
        m_curCamState.rotation = transform.rotation;
        m_curCamState.currentRotation = transform.rotation;
        m_curCamState.desiredRotation = transform.rotation;

        m_curCamState.xDeg = Vector3.Angle(Vector3.right, transform.right);
        m_curCamState.yDeg = Vector3.Angle(Vector3.up, transform.up);

        m_initialCamState = m_curCamState;
    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled.
     */
    void LateUpdate() {
        bool leftMousePressed = m_inputReader.ReadButton((int)SDUnityChanButtonInput.Attack);

        bool leftAltPressed = m_inputReader.ReadButton((int)SDUnityChanButtonInput.LeftAlt);
        
        Vector2 mouseDelta = m_inputReader.Read2DAxis((int) SDUnityChan2DAxisInput.Look);
        // ZOOM when Control and Alt and Left button
        if (leftMousePressed && leftAltPressed && m_inputReader.ReadButton((int) SDUnityChanButtonInput.LeftCtrl)) {
            m_curCamState.desiredDistance -= mouseDelta.y * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs(m_curCamState.desiredDistance);
        } else if (leftMousePressed && leftAltPressed) {
            m_curCamState.xDeg += mouseDelta.x * xSpeed * 0.02f;
            m_curCamState.yDeg -= mouseDelta.y * ySpeed * 0.02f;

            ////////OrbitAngle

            //Clamp the vertical axis for the orbit
            m_curCamState.yDeg = ClampAngle(m_curCamState.yDeg, yMinLimit, yMaxLimit);
            // set camera rotation
            m_curCamState.desiredRotation = Quaternion.Euler(m_curCamState.yDeg, m_curCamState.xDeg, 0);
            m_curCamState.currentRotation = transform.rotation;

            m_curCamState.rotation = Quaternion.Lerp(m_curCamState.currentRotation, m_curCamState.desiredRotation, Time.deltaTime * zoomDampening);
            transform.rotation = m_curCamState.rotation;
        }

        ////////Orbit Position

        // affect the desired Zoom distance if we roll the scrollwheel
        float scrollY = m_inputReader.Read1DAxis((int)SDUnityChan1DAxisInput.MouseScroll);
        m_curCamState.desiredDistance -= scrollY * Time.deltaTime * zoomRate * Mathf.Abs(m_curCamState.desiredDistance);
        //clamp the zoom min/max
        m_curCamState.desiredDistance = Mathf.Clamp(m_curCamState.desiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance
        m_curCamState.currentDistance = Mathf.Lerp(m_curCamState.currentDistance, m_curCamState.desiredDistance, Time.deltaTime * zoomDampening);

        // calculate position based on the new currentDistance
        m_curCamState.position = target.position - (m_curCamState.rotation * Vector3.forward * m_curCamState.currentDistance + targetOffset);
        transform.position = m_curCamState.position;
        
        if (m_inputReader.ReadButtonDown((int) SDUnityChanButtonInput.Reset)) {
            m_curCamState = m_initialCamState;
            transform.rotation = m_curCamState.rotation;
            transform.position = m_curCamState.position;
        }        
    }

    private static float ClampAngle(float angle, float min, float max) {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

//----------------------------------------------------------------------------------------------------------------------
    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 40;
    public float zoomDampening = 5.0f;
    
    
    [SerializeField] private SDUnityChanInputReader m_inputReader;


    private CameraState m_curCamState;
    private CameraState m_initialCamState;
}

struct CameraState {
    public float xDeg;
    public float yDeg;
    public float currentDistance;
    public float desiredDistance;
    public Quaternion currentRotation;
    public Quaternion desiredRotation;
    public Quaternion rotation;
    public Vector3 position;
    
}
} //end namespace UnityChan