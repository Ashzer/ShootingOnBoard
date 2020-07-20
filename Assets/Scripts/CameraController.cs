using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _fireCamera;
    public static CameraController Instance { get; set; }
    private float _cameraRotateSpeed;
    private Camera _currentCamera;
    private Vector2?[] _prevTouchPosition = { null, null };
    private Vector2 _prevTouchVector;
    private float _prevTouchDist;
    private Vector3 _mainCamOriginPosition;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _mainCamOriginPosition = _mainCamera.transform.position;
        MainCameraOn();
        _cameraRotateSpeed = 4f;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    public void MainCameraOn()
    {
        _mainCamera.enabled = true;
        _mainCamera.gameObject.SetActive(true);
        _fireCamera.enabled = false;
        _fireCamera.gameObject.SetActive(false);
        Screen.orientation = ScreenOrientation.Portrait;
        _currentCamera = _mainCamera;
        _mainCamera.transform.position = _mainCamOriginPosition;
        _mainCamera.transform.LookAt(new Vector3(0,0,0));
    }

    public void FireCameraOn()
    {
        _fireCamera.enabled = true;
        _fireCamera.gameObject.SetActive(true);
        _mainCamera.enabled = false;
        _mainCamera.gameObject.SetActive(false);
        _currentCamera = _fireCamera;
    }

    public void FireCameraHandling(Transform rotationAxis)
    {
        if (_currentCamera != _fireCamera) return;
        var touch = Input.GetTouch(0);
        var scale = touch.deltaPosition.x * Math.Abs(touch.deltaPosition.x) * _cameraRotateSpeed;
        _currentCamera.transform.RotateAround(rotationAxis.position, Vector3.up, scale / _currentCamera.pixelWidth);
    }

    public Camera GetCurrentCamera()
    {
        return _currentCamera;
    }

    public Camera GetMainCamera()
    {
        return _mainCamera;
    }

    public Camera GetFireCamera()
    {
        return _fireCamera;
    }

    public void MainCameraHandling()
    {
        if (_currentCamera != _mainCamera) return;

        //No touch
        if (Input.touchCount == 0)
        {
            _prevTouchPosition[0] = null;
            _prevTouchPosition[1] = null;
        }//Single touch
        else if (Input.touchCount == 1)
        {
            if (_prevTouchPosition[0] == null || _prevTouchPosition[1] != null)
            {
                _prevTouchPosition[0] = Input.GetTouch(0).position;
                _prevTouchPosition[1] = null;
            }
            else
            {
                Vector2 newTouchPosition = Input.GetTouch(0).position;

                /*_currentCamera.transform.position += _currentCamera.transform.TransformDirection(
                    (Vector3)(_prevTouchPosition[0] - newTouchPosition) * _currentCamera.orthographicSize /
                    _currentCamera.pixelHeight);*/

                _currentCamera.transform.Translate(((_prevTouchPosition[0].Value.x)- newTouchPosition.x) /_currentCamera.pixelWidth*20f , ((_prevTouchPosition[0].Value.y)- newTouchPosition.y) /_currentCamera.pixelWidth*20f, 0);
                _currentCamera.transform.position = GetCameraArea(_currentCamera.transform.position);
                _prevTouchPosition[0] = newTouchPosition;
                //Debug.Log(_currentCamera.transform.position);
                
                

            }
        }//Double touch
        else if (Input.touchCount == 2)
        {
            Debug.Log("Double touched");
            if (_prevTouchPosition[1] == null)
            {
                _prevTouchPosition[0] = Input.GetTouch(0).position;
                _prevTouchPosition[1] = Input.GetTouch(1).position;
                _prevTouchVector = (Vector2) (_prevTouchPosition[0] - _prevTouchPosition[1]);
                _prevTouchDist = _prevTouchVector.magnitude;
            }
            else
            {
                Vector2 screen = new Vector2(_currentCamera.pixelWidth, _currentCamera.pixelHeight);

                Vector2[] newTouchPosition = {Input.GetTouch(0).position, Input.GetTouch(1).position};
                Vector2 newTouchVector = newTouchPosition[0] - newTouchPosition[1];
                float newTouchDist = newTouchVector.magnitude;

                var touchData = newTouchDist - _prevTouchDist;
                //Debug.Log(_currentCamera.transform.position);
                _currentCamera.transform.Translate(0, 0, touchData * Time.deltaTime * 2f);
                _currentCamera.transform.position = GetCameraArea(_currentCamera.transform.position);

                _prevTouchPosition[0] = newTouchPosition[0];
                _prevTouchPosition[1] = newTouchPosition[1];
                _prevTouchVector = newTouchVector;
                _prevTouchDist = newTouchDist;
            }
        }
    }

    private Vector3 GetCameraArea(Vector3 cameraPosition)
    {
        Debug.Log($"{cameraPosition} = {cameraPosition.magnitude}");

        var x = Mathf.Clamp(cameraPosition.x, -10, 10);
        var y = Mathf.Clamp(cameraPosition.y, 10, 30);
        var z = Mathf.Clamp(cameraPosition.z, -30, 20);
        
        return new Vector3(x,y,z);
    }
}
