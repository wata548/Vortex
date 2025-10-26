using System;
using System.Linq;
using Extension;
using UnityEngine;

namespace MapGenerator.Player {
    [RequireComponent(typeof(Rigidbody))]
    public class Movement: MonoEasyBehaviour {

        //peekTime = 0.2, peekHeight = 1.7 => gravityScale = -75, jumpScale = 15
        private const float JUMP_SCALE = 11.333f;

        private int _contactCount = 0;
        private bool _isGround;

        private void Update() {

            var direction = InputSetting.GetDirection();
            var delta = 6 * direction;
            var velocity = Component<Rigidbody>().velocity;
            
            var pos = transform.position + 0.5f * direction.Multiple(transform.localScale);
            var scale = transform.localScale * 0.49f;
            var rotation = Quaternion.Euler(0, Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg, 0);
            scale.z = 0.05f;
            if (Physics.OverlapBox(pos, scale, rotation, LayerMask.GetMask("Ground")).Length > 0) {
                delta = Vector3.zero;
            }
            
            (velocity.x, velocity.z) = (delta.x, delta.z);
            
            if (_isGround && Input.GetKeyDown(InputSetting.Jump))
                velocity.y += JUMP_SCALE;

            Component<Rigidbody>().velocity = velocity;
            pos = transform.position;
            pos.y -= transform.localScale.y / 2;
            scale = transform.localScale * 0.49f;
            scale.y = 0.05f;
            var groundCount = Physics.OverlapBox(pos, scale, Quaternion.identity, LayerMask.GetMask("Ground")).Length;
            _isGround = groundCount != 0;

        }

        private void OnCollisionEnter(Collision other) {
            _contactCount++;
        }

        private void OnCollisionExit(Collision other) {
            _contactCount--;
        }

        private void OnDrawGizmos() {
            
            var pos = transform.position;
            pos.y -= transform.localScale.y / 2 + 0.06f;
            var scale = transform.localScale * 0.49f;
            scale.y = 0.05f;
            scale *= 2;
            Gizmos.color = Color.red;
            Gizmos.DrawCube(pos, scale);
            
            
            
        }
    }
}