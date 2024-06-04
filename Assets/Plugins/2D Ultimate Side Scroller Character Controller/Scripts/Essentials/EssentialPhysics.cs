using System.Collections.Generic;
using System.Linq;
using UltimateCC;
using UnityEngine;
using static UltimateCC.PlayerMain;

namespace Plugins._2D_Ultimate_Side_Scroller_Character_Controller.Scripts.Essentials
{
    public static class EssentialPhysics
    {
        // Updates player's facing direction variable at PlayerData.Physics
        public static void SetPlayerFacingDirection(PlayerInputManager inputManager, PlayerMain player,
            PlayerData playerData)
        {
            if (player.CurrentState == AnimName.WallSlide)
            {
                playerData.Physics.FacingDirection = playerData.Physics.WallDirection *
                                                     playerData.Walls.WallSlide.FacingDirectionWhenSliding;
            }
            else if (player.CurrentState == AnimName.WallGrab || player.CurrentState == AnimName.WallClimb)
            {
                playerData.Physics.FacingDirection = playerData.Physics.WallDirection;
            }
            else if (player.CurrentState == AnimName.WallJump)
            {
                playerData.Physics.FacingDirection = playerData.Physics.IsNextToWall
                    ? -playerData.Physics.WallDirection
                    : (int)Mathf.Sign(player.Rigidbody2D.velocity.x);
            }
            else if (inputManager.Input_Walk != 0 &&
                     Mathf.Sign(inputManager.Input_Walk) != Mathf.Sign(playerData.Physics.FacingDirection))
            {
                playerData.Physics.FacingDirection *= -1;
            }
            else if (playerData.Physics.FacingDirection == 0)
            {
                playerData.Physics.FacingDirection = 1;
            }

            SetLocalScale(player, playerData);
        }

        // Updates localScale of player based on facing direction
        public static void SetLocalScale(PlayerMain player, PlayerData playerData)
        {
            player.transform.right = Vector3.right * playerData.Physics.FacingDirection;
        }

        // This function returns the time value on an AnimationCurve where a given value is first reached at that specified time.
        // The 'greater' parameter is used to specify whether the function should search for the first time the curve value is greater than or equal to the given value.
        public static float SetCurveTimeByValue(AnimationCurve curve, float value, float maxTime,
            bool greaterValues = true)
        {
            float _curveTime = 0f;
            while ((greaterValues && curve.Evaluate(_curveTime) <= value) ||
                   (!greaterValues && curve.Evaluate(_curveTime) >= value))
            {
                _curveTime += Time.fixedDeltaTime;
                if (_curveTime >= maxTime)
                {
                    break;
                }
            }

            return _curveTime;
        }

        public static void GroundCheck(PlayerMain player, PlayerData playerData)
        {
            playerData.Physics.GroundCheckPosition = SetGroundCheckPosition(player, playerData);
            float _offset = -0.01f;
            List<RaycastHit2D> results = new();
            int count = Physics2D.CircleCast(playerData.Physics.GroundCheckPosition,
                player.CapsuleCollider2D.size.x / 1.99f * Mathf.Abs(player.transform.localScale.x) + _offset,
                -player.transform.up, playerData.Physics.GroundContactFilter, results, 0.1f);

            RaycastHit2D hittedPlayer = results.FirstOrDefault(hit => hit.collider == player.CapsuleCollider2D);
            
            if (hittedPlayer) 
                results.Remove(hittedPlayer);

            // Debug.DrawRay(_hit.point, _hit.normal, Color.red);
            if (results.Count > 0)
            {
                foreach (var hit in results)
                {
                    playerData.Physics.IsGrounded = true;
                    playerData.Physics.WalkSpeedDirection = Vector2.Perpendicular(hit.normal).normalized;
                    playerData.Physics.ContactPosition = hit.point;
                    playerData.Physics.Slope.CurrentSlopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    playerData.Physics.IsOnNotWalkableSlope = playerData.Physics.Slope.CurrentSlopeAngle >
                                                              playerData.Physics.Slope.MaxSlopeAngle;
                    SlopeAngleChangeCheck(hit, player, playerData);
                    if (hit.collider.gameObject.layer == 12 && hit.rigidbody)
                    {
                        playerData.Physics.CollidedMovingRigidbody = hit.rigidbody;
                    }
                }
            }
            else
            {
                playerData.Physics.IsGrounded = false;
                playerData.Physics.WalkSpeedDirection = new Vector2(-1, 0);
                playerData.Physics.ContactPosition = Vector2.zero;
                playerData.Physics.Slope.CurrentSlopeAngle = 0f;
                playerData.Physics.IsOnNotWalkableSlope = false;
                playerData.Physics.Slope.CurrentSlopeAngleChange = 0f;
                playerData.Physics.IsOnCorner = false;
                playerData.Physics.CollidedMovingRigidbody = null;
            }

            playerData.Physics.IsMultipleContactWithWalkableSlope = MultipleGroundContactCheck(player, playerData);
        }

        public static Vector2 SetGroundCheckPosition(PlayerMain player, PlayerData playerData)
        {
            Vector2 _groundCheckPosition = player.transform.position;
            Vector2 _size = new(player.CapsuleCollider2D.size.x * Mathf.Abs(player.transform.localScale.x),
                player.CapsuleCollider2D.size.y * player.transform.localScale.y);
            Vector2 _offset = new(player.CapsuleCollider2D.offset.x * Mathf.Abs(player.transform.localScale.x),
                player.CapsuleCollider2D.offset.y * player.transform.localScale.y);
            _groundCheckPosition.y -= Mathf.Abs(_size.x - _size.y) / 2;
            _groundCheckPosition += new Vector2(_offset.x * playerData.Physics.FacingDirection, _offset.y);
            _groundCheckPosition = _groundCheckPosition.RotateAround(
                (Vector2)player.transform.position + player.CapsuleCollider2D.offset, player.Rigidbody2D.rotation);
            return _groundCheckPosition;
        }

        public static Vector2 SetHeadCheckPosition(PlayerMain player, PlayerData playerData)
        {
            Vector2 _headCheckPosition = player.transform.position;
            Vector2 _size = new(player.CapsuleCollider2D.size.x * Mathf.Abs(player.transform.localScale.x),
                player.CapsuleCollider2D.size.y * player.transform.localScale.y);
            Vector2 _offset = new(player.CapsuleCollider2D.offset.x * Mathf.Abs(player.transform.localScale.x),
                player.CapsuleCollider2D.offset.y * player.transform.localScale.y);
            _headCheckPosition.y += Mathf.Abs(_size.x - _size.y) / 2;
            _headCheckPosition += new Vector2(_offset.x * playerData.Physics.FacingDirection, _offset.y);
            _headCheckPosition = _headCheckPosition.RotateAround(
                (Vector2)player.transform.position + player.CapsuleCollider2D.offset, player.Rigidbody2D.rotation);
            return _headCheckPosition;
        }

        public static void SlopeAngleChangeCheck(RaycastHit2D hit, PlayerMain player, PlayerData playerData)
        {
            RaycastHit2D hitFront =
                Physics2D.Raycast(hit.point + new Vector2(0.05f * playerData.Physics.FacingDirection, 0.2f),
                    Vector2.down, 1f, playerData.Physics.GroundLayerMask);
            RaycastHit2D hitBack =
                Physics2D.Raycast(hit.point + new Vector2(-0.05f * playerData.Physics.FacingDirection, 0.2f),
                    Vector2.down, 1f, playerData.Physics.GroundLayerMask);
            Debug.DrawRay(hitFront.point, hitFront.normal, Color.magenta);
            playerData.Physics.Slope.CurrentSlopeAngleChange = Vector2.Angle(hitFront.normal, hit.normal);
            if (playerData.Physics.Slope.CurrentSlopeAngleChange > 0 && hitFront
                                                                     && Vector2.Angle(hitFront.normal, Vector2.up) <=
                                                                     playerData.Physics.Slope.MaxSlopeAngle
                                                                     && playerData.Physics.Slope
                                                                         .CurrentSlopeAngleChange <=
                                                                     playerData.Physics.Slope.MaxEasedSlopeAngleChange)
            {
                playerData.Physics.WalkSpeedDirection =
                    (Vector2)(Quaternion.Euler(0, 0, 20 * -playerData.Physics.FacingDirection) *
                              (Vector3)playerData.Physics.WalkSpeedDirection);
                playerData.Physics.IsOnCorner = !hitBack;
            }
            else if (!hitFront)
            {
                playerData.Physics.WalkSpeedDirection = new(-1, 0);
                playerData.Physics.Slope.CurrentSlopeAngleChange = 0f;
                playerData.Physics.IsOnCorner = true;
            }
            else if (!hitBack)
            {
                playerData.Physics.IsOnCorner = true;
            }
            else
            {
                playerData.Physics.IsOnCorner = false;
            }
        }

        public static bool MultipleGroundContactCheck(PlayerMain player, PlayerData playerData)
        {
            //WalkSpeedDirection
            //StayStill
            //IsMultipleContactWithNonWalkableSlope
            //return IsMultipleContactWithWalkableSlope
            float _offset = -0.01f;
            RaycastHit2D _frontHit = Physics2D.CircleCast(playerData.Physics.GroundCheckPosition,
                player.CapsuleCollider2D.size.x / 2 * Mathf.Abs(player.transform.localScale.x) + _offset,
                playerData.Physics.FacingDirection * Vector2.right, 0.1f, playerData.Physics.GroundLayerMask);
            if (playerData.Physics.IsGrounded && !playerData.Physics.IsOnNotWalkableSlope &&
                !playerData.Physics.IsOnCorner && _frontHit &&
                (_frontHit.point - playerData.Physics.ContactPosition).magnitude > 0.1f)
            {
                if (Vector2.Angle(playerData.Physics.GroundCheckPosition - _frontHit.point, Vector2.up) >
                    playerData.Physics.Slope.MaxSlopeAngle)
                {
                    playerData.Physics.IsMultipleContactWithNonWalkableSlope = true;
                    playerData.Physics.Slope.StayStill =
                        Mathf.Sign(_frontHit.point.x - playerData.Physics.GroundCheckPosition.x) ==
                        Mathf.Sign(playerData.Physics.FacingDirection);
                    return false;
                }
                else
                {
                    playerData.Physics.WalkSpeedDirection = new Vector2(-1, 0);
                    playerData.Physics.Slope.StayStill = false;
                    playerData.Physics.IsMultipleContactWithNonWalkableSlope = false;
                    return true;
                }
            }
            else
            {
                playerData.Physics.Slope.StayStill = false;
                playerData.Physics.IsMultipleContactWithNonWalkableSlope = false;
            }

            return false;
        }

        public static void WallCheck(PlayerMain player, PlayerData playerData)
        {
            playerData.Physics.HeadCheckPosition = SetHeadCheckPosition(player, playerData);
            float _offset = 0.1f;
            RaycastHit2D _rightHit = Physics2D.Raycast(playerData.Physics.HeadCheckPosition, Vector2.right,
                player.CapsuleCollider2D.size.x / 2 * Mathf.Abs(player.transform.localScale.x) + _offset,
                playerData.Physics.WallLayerMask);
            RaycastHit2D _leftHit = Physics2D.Raycast(playerData.Physics.HeadCheckPosition, Vector2.left,
                player.CapsuleCollider2D.size.x / 2 * Mathf.Abs(player.transform.localScale.x) + _offset,
                playerData.Physics.WallLayerMask);
            
            if(_rightHit.collider == player.CapsuleCollider2D || _leftHit.collider == player.CapsuleCollider2D)
                return;
            
            if (_rightHit || _leftHit)
            {
                playerData.Physics.WallDirection = _rightHit ? 1 : -1;
                RaycastHit2D _hit = _rightHit ? _rightHit : _leftHit;
                playerData.Physics.CollidedMovingRigidbody = _hit.rigidbody;
                playerData.Physics.IsNextToWall = true;
            }
            else
            {
                playerData.Physics.WallDirection = 0;
                if (playerData.Physics.CollidedMovingRigidbody != null &&
                    playerData.Physics.CollidedMovingRigidbody.gameObject.layer == 13)
                {
                    playerData.Physics.CollidedMovingRigidbody = null;
                }

                playerData.Physics.IsNextToWall = false;
            }
        }

        public static void HeadBumpCheck(PlayerMain player, PlayerData playerData)
        {
            playerData.Physics.HeadCheckPosition = SetHeadCheckPosition(player, playerData);
            RaycastHit2D _hit;
            float _offset = 0.01f;
            _hit = Physics2D.CircleCast(playerData.Physics.HeadCheckPosition,
                player.CapsuleCollider2D.size.x / 2 * Mathf.Abs(player.transform.localScale.x) + _offset,
                player.transform.up, 0.2f, playerData.Physics.HeadBumpLayerMask);
            if (_hit)
            {
                playerData.Physics.IsOnHeadBump = true;
                playerData.Physics.CanBumpHead =
                    Vector2.Angle(_hit.normal, Vector2.down) < playerData.Physics.HeadBumpMinAngle;
            }
            else
            {
                playerData.Physics.IsOnHeadBump = false;
                playerData.Physics.CanBumpHead = false;
            }
        }

        public static bool CornerSlideCheck(List<ContactPoint2D> contacts, PlayerMain player, PlayerData playerData)
        {
            contacts = contacts.Where(x => x.point.y <= playerData.Physics.GroundCheckPosition.y).GroupBy(x => x.point)
                .Select(x => x.First()).ToList();
            foreach (ContactPoint2D contact in contacts)
            {
                if (Vector2.Angle(playerData.Physics.GroundCheckPosition - contact.point, Vector2.up) >=
                    playerData.Physics.SlideOnCornerMinAngle)
                {
                    return playerData.Physics.IsOnCorner;
                }
            }

            return false;
        }

        public static void ApplyRotationOnSlope(PlayerMain player, PlayerData playerData)
        {
            float _currentSlopeAngle, _rotationDirection, _finalRotation, _rotationDifference;
            _currentSlopeAngle = playerData.Physics.Slope.CurrentSlopeAngle;
            _rotationDirection = playerData.Physics.ContactPosition.x > playerData.Physics.GroundCheckPosition.x
                ? 1
                : -1;
            _finalRotation = 0f;
            if (playerData.Physics.IsGrounded && !(playerData.Physics.IsOnNotWalkableSlope ||
                                                   playerData.Physics.IsMultipleContactWithNonWalkableSlope ||
                                                   playerData.Physics.IsMultipleContactWithWalkableSlope))
            {
                _finalRotation = _currentSlopeAngle * _rotationDirection *
                                 playerData.Physics.Slope.RotationMultiplierOnSlope;
            }

            _rotationDifference = _finalRotation - player.Rigidbody2D.rotation;
            if (Mathf.Abs(_rotationDifference) > 2f)
            {
                Vector2 _pivot = playerData.Physics.GroundCheckPosition;
                Vector2 _offset = player.Rigidbody2D.position - _pivot;
                float angleInRadians = playerData.Physics.Slope.RotationSpeed * _rotationDifference *
                                       Time.fixedDeltaTime * Mathf.Deg2Rad;
                float cosAngle = Mathf.Cos(angleInRadians);
                float sinAngle = Mathf.Sin(angleInRadians);
                Vector2 rotatedOffset = new Vector2(
                    _offset.x * cosAngle - _offset.y * sinAngle,
                    _offset.x * sinAngle + _offset.y * cosAngle
                );
                player.Rigidbody2D.position += _pivot + rotatedOffset - player.Rigidbody2D.position;
                player.Rigidbody2D.angularVelocity = playerData.Physics.Slope.RotationSpeed * _rotationDifference;
            }
            else
            {
                player.Rigidbody2D.angularVelocity = 0f;
                player.Rigidbody2D.SetRotation(_finalRotation);
            }
        }

        public static void GetPlatformVelocity(Rigidbody2D platformRigidbody, PlayerData playerData)
        {
            if (platformRigidbody)
            {
                Vector2 _platformCenterToContact =
                    playerData.Physics.ContactPosition - (Vector2)platformRigidbody.position;
                float _angularVelocity = platformRigidbody.angularVelocity * Mathf.Deg2Rad;
                Vector2 _rotationalLinearVelocity = new(-_platformCenterToContact.y * _angularVelocity,
                    _platformCenterToContact.x * _angularVelocity);
                Vector2 _movingLinearVelocity = platformRigidbody.velocity;
                Vector2 _linearVelocity = _rotationalLinearVelocity + _movingLinearVelocity;

                playerData.Physics.Platform.MaxPlatformVelocity = _linearVelocity;
                playerData.Physics.Platform.DampedVelocity = _linearVelocity;
            }
            else
            {
                float _slowDownTime = SetCurveTimeByValue(playerData.Physics.Platform.DampingCurve,
                    playerData.Physics.Platform.DampedVelocity.magnitude /
                    playerData.Physics.Platform.MaxPlatformVelocity.magnitude, 1, false);
                _slowDownTime += Time.fixedDeltaTime;
                playerData.Physics.Platform.DampedVelocity =
                    playerData.Physics.Platform.DampingCurve.Evaluate(_slowDownTime /
                                                                      playerData.Physics.Platform.DampingTime) *
                    playerData.Physics.Platform.MaxPlatformVelocity;
            }
        }
    }
}