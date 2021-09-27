using System;
using UnityEngine;

namespace Platform.Scripts.Character {
    public class CharacterCameraController : MonoBehaviour {

        private Transform _transform;

        private RectTransform _target;
        
        private Camera _camera;
        
        private void Start() {
            
            _transform = transform;
            
            if (_target == null) {
                _target = GameObject.FindWithTag("CharacterAnchor").GetComponent<RectTransform>();
            }

            if (_camera == null) {
                _camera = GetComponent<Camera>();
            }

            var newPos = _target.position;
            
            Debug.Log(newPos);
            
            var halfOrthoSize = _camera.orthographicSize * 0.5f;
            
            _transform.position = new Vector3(newPos.x, -newPos.y, _transform.position.z);
        }
    }
}
