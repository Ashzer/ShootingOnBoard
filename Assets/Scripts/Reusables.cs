/*
if (Input.touchCount > 0)
{
    var touch = Input.GetTouch(0);
    _mainCamTransform.RotateAround(_currentSelection.position, Vector3.up, touch.deltaPosition.x / 2);
    if (touch.phase == TouchPhase.Began)
    {
        Debug.Log($"Began : {touch.position}");
        Debug.Log($"Began : {touch.deltaPosition}");
    }

    if (touch.phase == TouchPhase.Moved)
    {
        //Debug.Log($"Moved : {touch.position}");
        //Debug.Log($"Moved : {touch.deltaPosition}");
        if (_currentSelection != null)
        {
            _mainCamTransform.RotateAround(_currentSelection.position, Vector3.up, touch.deltaPosition.x / 2);
        }
    }

    if (touch.phase == TouchPhase.Ended)
    {
        Debug.Log($"Ended : {touch.position}");
        Debug.Log($"Ended : {touch.deltaPosition}");
    }
}
*/
