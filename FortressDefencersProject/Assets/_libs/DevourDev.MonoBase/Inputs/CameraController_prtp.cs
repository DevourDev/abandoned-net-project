using UnityEngine;

namespace DevourDev.MonoBase.Inputs
{
    public class CameraController_prtp : MonoBehaviour
    {
        [SerializeField] private CommonSettings_prtp _settings = new();
        [SerializeField] private Vector2Buffer_prtp _mouseBuffer = new();

        private bool _cameraPositionChanged;
        private Vector3 _newCameraPosition;
        private bool _cameraRotationChanged;
        private Quaternion _newCameraRotation;
        private bool _posatronChanged;
        private Vector3 _newPosatronPosition;
        private bool _rotatronChanged;
        private Quaternion _newRotatronRotation;

        private void Start()
        {
            _settings.Rotatron.Rotate(new Vector3(_settings.Rotation.DefaultX, 0, 0));
        }


        private void Update()
        {

            HandleCamera();
        }


        public void SetPosition(Vector3 worldPos)
        {
            _settings.Posatron.position = worldPos;
        }
        /// <summary>
        /// Rotation around x axis
        /// </summary>
        /// <param name="value"></param>
        public void SetTiltAngle(float value)
        {
            value = Mathf.Clamp(value, _settings.Rotation.MinX, _settings.Rotation.MaxX);
            //float rotateXValue = value - _settings.CurrentRotation.x;
            Vector3 newRot = _settings.Rotatron.rotation.eulerAngles;
            newRot.x = value;
            _settings.Rotatron.rotation = Quaternion.Euler(newRot);
            //_settings.Rotatron.rotation.eulerAngles = (new Vector3(rotateXValue, 0, 0));
        }
        /// <summary>
        /// Rotation around y axis
        /// </summary>
        /// <param name="value"></param>
        public void SetRotationAngle(float value)
        {
            value = Mathf.Clamp(value, 0, 360);
            //float rotateYValue = value - _settings.CurrentRotation.y;
            //_settings.Rotatron.Rotate(new Vector3(0, rotateYValue, 0));
            Vector3 newRot = _settings.Rotatron.rotation.eulerAngles;
            newRot.y = value;
            _settings.Rotatron.rotation = Quaternion.Euler(newRot);
        }

        private void HandleCamera()
        {
            _mouseBuffer.SetCurrent(Input.mousePosition);

            _cameraPositionChanged = false;
            _cameraRotationChanged = false;
            _posatronChanged = false;
            _rotatronChanged = false;

            var c = _settings.Cam;
            var p = _settings.Posatron;
            var r = _settings.Rotatron;

            _newCameraPosition = c.transform.position;
            _newCameraRotation = c.transform.rotation;

            _newPosatronPosition = p.transform.position;
            _newRotatronRotation = r.transform.rotation;

            HandleZoom();
            HandleRotation();
            HandleMovement();

            //AdjustHeight_notImplemented();

            if (_cameraPositionChanged)
                c.transform.position = _newCameraPosition;
            if (_cameraRotationChanged)
                c.transform.rotation = _newCameraRotation;
            if (_posatronChanged)
                p.transform.position = _newPosatronPosition;
            if (_rotatronChanged)
                r.transform.rotation = _newRotatronRotation;

            if (_settings.LockOnLookAtTarget)
            {
                c.transform.LookAt(_settings.LookAtTarget);
            }

            _mouseBuffer.SetLast();
        }

        private void AdjustHeight_notImplemented()
        {
            throw new System.NotImplementedException();
        }

        [System.Obsolete("говна пожри, ёпта =)")]
        private Vector3 FloorPoint(Vector3 castOrigin, Vector3 through, float floorLevel)
        {
            float height = castOrigin.y - floorLevel;

            var pointA = castOrigin;
            var pointC = pointA - new Vector3(0, height, 0);
            var pointF = through;

            var a = pointC - pointA;
            float aLength = a.magnitude;

            var angleA = Vector3.Angle(a, pointF - pointA);
            var angleARads = Mathf.PI * angleA / 180f;

            float cLength = aLength / Mathf.Cos(angleARads);

            var afDir = pointF - pointA;
            var abDir = afDir.normalized * cLength;

            //var pointB = Vector3.MoveTowards(pointA, pointF, cLength);
            var pointB = pointA + abDir;

            return pointB;

            //float bLength = aLength * Mathf.Tan(angleARads);

            //Vector3 direction = (pointF - pointA);
            //direction.y = 0;

            //var b = Vector3.MoveTowards(pointC, ;

            //return b;
        }

        private void HandleMovement()
        {
            if (HandleKbMovement())
                return;
            if (HandleDragMovement())
                return;
            if (HandleEdgeMovement())
                return;


            bool HandleKbMovement()
            {
                if (!_settings.KbMoving.Enabled)
                    return false;

                int horizontalInputs = 0;
                int verticalInputs = 0;
                bool movingLeft = InputChecks.CheckForAnyBindPressing(_settings.KbMoving.MoveLeftBinds);
                bool movingRight = InputChecks.CheckForAnyBindPressing(_settings.KbMoving.MoveRightBinds);
                bool movingUp = InputChecks.CheckForAnyBindPressing(_settings.KbMoving.MoveUpBinds);
                bool movingDown = InputChecks.CheckForAnyBindPressing(_settings.KbMoving.MoveDownBinds);

                if (movingLeft)
                    ++horizontalInputs;
                if (movingRight)
                    ++horizontalInputs;
                if (movingUp)
                    ++verticalInputs;
                if (movingDown)
                    ++verticalInputs;

                if (horizontalInputs + verticalInputs == 0)
                    return false;

                if (horizontalInputs > 1 || verticalInputs > 1)
                    return true;

                Vector3 direction = Vector3.zero;

                if (movingLeft)
                {
                    direction += Vector3.left;
                }
                else if (movingRight)
                {
                    direction += Vector3.right;
                }

                if (movingUp)
                {
                    direction += Vector3.forward;
                }
                else if (movingDown)
                {
                    direction += Vector3.back;
                }


                var worldDirection = GetDirection(direction);

                _posatronChanged = true;
                _newPosatronPosition += _settings.KbMoving.Speed * Time.deltaTime * worldDirection;


                return true;
            }




            bool HandleDragMovement()
            {
                if (!_settings.DragMoving.Enabled)
                    return false;

                if (!InputChecks.CheckForAnyBindPressing(_settings.DragMoving.Binds))
                    return false;



                if (_settings.DragMoving.UseRaycast)
                {
                    if (!MonoBase.MonoSimples.TryGetWorldPositionOnMouse(_settings.Cam, _mouseBuffer.LastFrame, out var startP, _settings.DragMoving.GroundLayer)
                        || !MonoBase.MonoSimples.TryGetWorldPositionOnMouse(_settings.Cam, _mouseBuffer.CurrentFrame, out var endP, _settings.DragMoving.GroundLayer))
                        return false;


                    _posatronChanged = true;
                    startP.y = 0;
                    endP.y = 0;
                    _newPosatronPosition += startP - endP;
                }
                else
                {
                    var delta = -_mouseBuffer.Delta;
                    if (delta == Vector2.zero)
                        return true;

                    var worldDirection = GetDirection(delta);

                    _posatronChanged = true;
                    _newPosatronPosition += _settings.DragMoving.NonRaycastSpeed * worldDirection;
                }


                return true;
            }

            bool HandleEdgeMovement()
            {
                if (!_settings.ScreenEdgeMoving.Enabled)
                    return false;

                var mousePos = _mouseBuffer.CurrentFrame;
                var pixelsFromEdgeToTriggerWidth = _settings.Cam.pixelWidth * _settings.ScreenEdgeMoving.EdgeDistanceToActivate;
                var pixelsFromEdgeToTriggerHeight = _settings.Cam.pixelHeight * _settings.ScreenEdgeMoving.EdgeDistanceToActivate;

                bool movingToLeft = mousePos.x <= pixelsFromEdgeToTriggerWidth;
                bool movingToRight = _settings.Cam.pixelWidth - mousePos.x <= pixelsFromEdgeToTriggerWidth;
                bool movingUp = _settings.Cam.pixelHeight - mousePos.y <= pixelsFromEdgeToTriggerHeight;
                bool movingDown = mousePos.y <= pixelsFromEdgeToTriggerHeight;

                if (!movingToLeft && !movingToRight && !movingUp && !movingDown)
                    return false;

                float horizontalStrength = 0;
                float verticalStrength = 0;

                if (movingToLeft)
                {
                    horizontalStrength -= Mathf.InverseLerp(pixelsFromEdgeToTriggerWidth, 0, mousePos.x);
                }
                else if (movingToRight)
                {
                    horizontalStrength += Mathf.InverseLerp(_settings.Cam.pixelWidth - pixelsFromEdgeToTriggerWidth, _settings.Cam.pixelWidth, mousePos.x);

                }

                if (movingUp)
                {
                    verticalStrength += Mathf.InverseLerp(_settings.Cam.pixelHeight - pixelsFromEdgeToTriggerHeight, _settings.Cam.pixelHeight, mousePos.y);
                }
                else if (movingDown)
                {
                    verticalStrength -= Mathf.InverseLerp(pixelsFromEdgeToTriggerHeight, 0, mousePos.y);
                }



                var delta = new Vector3(horizontalStrength, 0, verticalStrength);
                if (delta == Vector3.zero)
                    return true;



                _posatronChanged = true;

                var direction = GetDirection(delta, false);

                if (_settings.ScreenEdgeMoving.RelativeSpeed)
                {
                    direction *= _settings.ScreenEdgeMoving.RelativeSpeedMultiplier * Vector3.Distance(_settings.Cam.transform.position, _settings.Posatron.position);
                }
                else
                {
                    direction *= _settings.ScreenEdgeMoving.MaxSpeed;
                }

                _newPosatronPosition += Time.deltaTime * direction;


                return true;
            }


            Vector3 GetDirection(Vector3 localDirection, bool normalize = true)
            {
                if (localDirection == Vector3.zero)
                    return Vector3.zero;

                Vector3 deltaPos = _settings.Rotatron.TransformDirection(localDirection);
                deltaPos.y = 0;
                return normalize ? deltaPos.normalized : deltaPos;
            }


        }
        private void HandleRotation()
        {
            if (!_settings.Rotation.Enabled)
                return;

            int leftRightInputs = 0;
            int upDownInputs = 0;

            bool rotatingLeft = InputChecks.CheckForAnyBindPressing(_settings.Rotation.RotateLeftBinds);
            bool rotatingRight = InputChecks.CheckForAnyBindPressing(_settings.Rotation.RotateRightBinds);
            bool rotatingUp = InputChecks.CheckForAnyBindPressing(_settings.Rotation.RotateUpBinds);
            bool rotatingDown = InputChecks.CheckForAnyBindPressing(_settings.Rotation.RotateDownBinds);
            bool rotatingWithMouse = InputChecks.CheckForAllBindsPressing(_settings.Rotation.MouseRotationBinds);


            if (rotatingLeft)
                ++leftRightInputs;
            if (rotatingRight)
                ++leftRightInputs;
            if (rotatingUp)
                ++upDownInputs;
            if (rotatingDown)
                ++upDownInputs;

            if (rotatingWithMouse)
            {
                ++leftRightInputs;
                ++upDownInputs;
            }

            int totalInputs = leftRightInputs + upDownInputs;

            if (leftRightInputs > 1 || upDownInputs > 1 || !(totalInputs == 2 || totalInputs == 1))
                return;



            if (rotatingLeft)
            {
                Rotate(Vector2.up * _settings.Rotation.KbSpeed);
            }

            else if (rotatingRight)
            {
                Rotate(Vector2.down * _settings.Rotation.KbSpeed);
            }

            if (rotatingUp)
            {
                Rotate(Vector2.right * _settings.Rotation.KbSpeed);
            }
            else if (rotatingDown)
            {
                Rotate(Vector2.left * _settings.Rotation.KbSpeed);
            }

            else if (rotatingWithMouse)
            {
                if (!_mouseBuffer.Changed)
                    return;
                Vector2 delta = _mouseBuffer.Delta;

                Vector2 rotationStrength = new(-delta.y, delta.x);

                if (_settings.Rotation.InverseMouseX)
                    rotationStrength.x *= -1;
                if (_settings.Rotation.InverseMouseY)
                    rotationStrength.y *= -1;

                Rotate(rotationStrength * _settings.Rotation.MouseSpeed);

            }

            void Rotate(Vector2 strength)
            {
                Vector3 desiredRotation = (Vector3)strength * Time.deltaTime;

                float minRotX = _settings.Rotation.MinX - _newRotatronRotation.eulerAngles.x;
                float maxRotX = _settings.Rotation.MaxX - _newRotatronRotation.eulerAngles.x;
                desiredRotation.x = Mathf.Clamp(desiredRotation.x, minRotX, maxRotX);

                if (desiredRotation == Vector3.zero)
                    return;

                _rotatronChanged = true;
                _newRotatronRotation.eulerAngles += desiredRotation;
            }

        }

        private void HandleZoom()
        {
            if (!_settings.Zoom.Enabled)
                return;

            int inputsAtOneTime = 0;

            bool zoomingIn = InputChecks.CheckForAnyBindPressing(_settings.Zoom.ZoomInBinds);
            bool zoomingOut = InputChecks.CheckForAnyBindPressing(_settings.Zoom.ZoomOutBinds);

            float scrollDelta = Input.mouseScrollDelta.y;
            bool scrollerZooming = _settings.Zoom.UseMouseScroller && scrollDelta != 0;

            if (zoomingIn)
                ++inputsAtOneTime;
            if (zoomingOut)
                ++inputsAtOneTime;
            if (scrollerZooming)
                ++inputsAtOneTime;

            if (inputsAtOneTime != 1)
                return;

            if (zoomingIn)
            {
                Zoom(_settings.Zoom.ZoomSpeed);
            }

            else if (zoomingOut)
            {
                Zoom(-_settings.Zoom.ZoomSpeed);
            }

            else if (scrollerZooming)
            {
                ScrollerZoom();
            }

            void ScrollerZoom()
            {
                if (_settings.Zoom.InverseMouseScroller)
                    Zoom(scrollDelta * _settings.Zoom.ScrollZoomSpeed);
                else
                    Zoom(-scrollDelta * _settings.Zoom.ScrollZoomSpeed);
            }

            void Zoom(float direction)
            {
                if (direction == 0)
                    return;

                _cameraPositionChanged = true;
                //var zoomAmount = strength * Time.deltaTime;
                var zoomAmount = CalculateZoomAmount(direction);

                _newCameraPosition = Vector3.MoveTowards(_newCameraPosition, _newPosatronPosition, zoomAmount);
            }

            float CalculateZoomAmount(float direction)
            {
                float currentDistance = Vector3.Distance(_newCameraPosition, _newPosatronPosition);


                float desiredValue = direction * Time.deltaTime;

                if (desiredValue > 0)
                {
                    return Mathf.Clamp(desiredValue, 0, currentDistance - _settings.Zoom.ClosestDistance);
                }
                else
                {
                    return Mathf.Clamp(desiredValue, -(_settings.Zoom.FarestDistance - currentDistance), 0);
                }
            }
        }

        [System.Serializable]
        public class CommonSettings_prtp
        {
            public Camera Cam;
            public Transform Posatron;
            public Transform Rotatron;
            public Transform LookAtTarget;
            public bool LockOnLookAtTarget;
            public CameraRotationSettings_prtp Rotation;
            public CameraMouseDragPaningSettings_prtp DragMoving;
            public CameraKBMovingSettings_prtp KbMoving;
            public ScreenEdgeCameraMovingSettings_prtp ScreenEdgeMoving;
            public CameraZoomSettings_prtp Zoom;


            public Vector3 CurrentRotation => Rotatron.rotation.eulerAngles;

            [System.Serializable]
            public class CameraRotationSettings_prtp
            {
                public bool Enabled = true;
                public float KbSpeed = 25.0f;
                public float MouseSpeed = 5.0f;
                public bool InverseMouseX = false;
                public bool InverseMouseY = false;
                public float MinX = 5;
                public float DefaultX = 40;
                public float MaxX = 90;
                public KeyBind[] RotateLeftBinds = { new(KeyCode.LeftShift, KeyCode.A), new(KeyCode.LeftShift, KeyCode.LeftArrow) };
                public KeyBind[] RotateRightBinds = { new(KeyCode.LeftShift, KeyCode.D), new(KeyCode.LeftShift, KeyCode.RightArrow) };
                public KeyBind[] RotateUpBinds = { new(KeyCode.LeftShift, KeyCode.W), new(KeyCode.LeftShift, KeyCode.UpArrow) };
                public KeyBind[] RotateDownBinds = { new(KeyCode.LeftShift, KeyCode.S), new(KeyCode.LeftShift, KeyCode.DownArrow) };
                public KeyBind[] MouseRotationBinds = { new KeyBind(KeyCode.LeftAlt, KeyCode.Mouse0) };
            }

            [System.Serializable]
            public class CameraMouseDragPaningSettings_prtp
            {
                public bool Enabled = true;
                public bool UseRaycast = true;
                public float NonRaycastSpeed = 0.3f;
                public LayerMask GroundLayer;
                /// <summary>
                /// (нажатие колёсика в Доте)
                /// </summary>
                public KeyBind[] Binds = { new(KeyCode.Mouse2) };
            }
            [System.Serializable]
            public class CameraKBMovingSettings_prtp
            {
                public bool Enabled = true;
                public float Speed = 50.0f;
                public KeyBind[] MoveLeftBinds = { new(KeyCode.A), new(KeyCode.LeftArrow) };
                public KeyBind[] MoveRightBinds = { new(KeyCode.D), new(KeyCode.RightArrow) };
                public KeyBind[] MoveUpBinds = { new(KeyCode.W), new(KeyCode.UpArrow) };
                public KeyBind[] MoveDownBinds = { new(KeyCode.S), new(KeyCode.DownArrow) };
            }

            [System.Serializable]
            public class ScreenEdgeCameraMovingSettings_prtp
            {
                public bool Enabled = true;
                /// <summary>
                /// (от разшерения экрана)
                /// </summary>
                public float EdgeDistanceToActivate = 0.05f;
                public float MaxSpeed = 50.0f;
                public bool RelativeSpeed = true;
                public float RelativeSpeedMultiplier = 0.4f;
            }
            [System.Serializable]
            public class CameraZoomSettings_prtp
            {
                [Range(1f, 500f)]
                public float ClosestDistance = 1;
                [Range(1f, 500f)]
                public float FarestDistance = 500;
                [Range(1f, 500f)]
                public float DefaultZoom = 100;
                public bool Enabled = true;
                public bool UseMouseScroller = true;
                public bool InverseMouseScroller = false;
                public KeyBind[] ZoomInBinds = { new(KeyCode.Equals) };
                public KeyBind[] ZoomOutBinds = { new(KeyCode.Minus) };
                public float ScrollZoomSpeed = 500.0f;
                public float ZoomSpeed = 100.0f;
            }
        }

        [System.Serializable]
        public class Vector2Buffer_prtp
        {
            private Vector2 _currentFrame;
            private Vector2 _lastFrame;


            public bool Changed => CurrentFrame != LastFrame;
            public Vector2 Delta => CurrentFrame - LastFrame;

            public Vector2 CurrentFrame => _currentFrame;
            public Vector2 LastFrame => _lastFrame;

            public void SetCurrent(Vector2 value)
            {
                _currentFrame = value;
            }
            public void SetLast()
            {
                _lastFrame = CurrentFrame;
            }
        }

        [System.Serializable]
        public class Vector3Buffer_prtp
        {
            private Vector3 _currentFrame;
            private Vector3 _lastFrame;


            public bool Changed => _currentFrame != _lastFrame;
            public Vector3 Delta => _currentFrame - _lastFrame;


            public Vector3 CurrentFrame => _currentFrame;
            public Vector3 LastFrame => _lastFrame;

            public void SetCurrent(Vector3 value)
            {
                _currentFrame = value;
            }
            public void SetLast()
            {
                _lastFrame = _currentFrame;
            }
        }
    }
}
