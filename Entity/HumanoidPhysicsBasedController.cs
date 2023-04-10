using System;
using UnityEngine;

namespace NeonLib.Entities {
    //A capsule and physics based humanoid entity controller. Using a raycast to detect the ground, the controller floats above the ground by a 'Ride Height'.
    //The controller is moved by a vector 3 direction input, and a jump input.
    //The rigidbody is floated above the ground using a spring and damper force, which can transfer momentum to other rigidbodies.
    //The rigidbody can be pushed around by other rigidbodies, so we want to keep the capsule upright.
    //To keep the capsule upright we will use a function to apply torque to the controller.
    [RequireComponent(typeof(Rigidbody))]
    public class HumanoidPhysicsBasedController : MonoBehaviour {

        private Rigidbody rb;

        [Header("Physics Settings")]
        public bool LockInput = false;
        public bool LockRigidbody = false;
        public bool FreezePhysicsCalculations = false;
        public Vector3 Gravity = Physics.gravity;
        private Vector3 gDownDir { get { return Gravity.normalized; } }
        public Vector3 Forces { get { return totalForce; } }
        private Vector3 totalForce = Vector3.zero;
       
        [Header("Ride Settings")]
        public float RideHeight = 1f;
        public float RideSpringDamper = 100f;
        public float RideSpringStrength = 5000f;
        public float UprightJointSpringStrength = 750f;
        public float UprightJointSpringDamper = 45f;
        private Quaternion targetRotation = Quaternion.identity;
        private bool grounded = false;
        private float lastGroundedTime = 0f;
        public float GroundedTime { get { return lastGroundedTime; } }
        public bool Grounded { get { return grounded; } }
        [Header("RayCast Settings")]
        public Ray GroundRay;
        public float RayLength = 1f;
        public LayerMask RayMask;
        public RaycastHit GroundHit;
        public RaycastHit lastHitWhileGrounded;
        public float groundThreshold = 0.6f;
        private bool groundCastHit = false;

        [Header("Movement")]
        public float MaxSpeed = 10f;
        public float Acceleration = 200f;
        public AnimationCurve AccelerationFactorFromDot = AnimationCurve.Linear(-1, 2, 0, 1);
        public float MaxAccelerationForce = 150f;
        public AnimationCurve MaxAccelerationForceFactorFromDot = AnimationCurve.Linear(-1, 2, 0, 1);
        public Vector3 MovementForceScale = new Vector3(1, 0, 1);
        private Vector3 inputDirection = Vector3.zero;
        private Vector3 movementGoalVelocity;
        private float speedFactor = 1f;
        private float maxAccelForceFactor = 1f;
        private Vector3 groundVelocity = Vector3.zero;


        [Header("Slope")]
        public float SlopeLimit = 45f;
        public float SlopeForceFactor = 1f;
        public AnimationCurve UnderSlopeLimitSlopeForceFromDot = AnimationCurve.EaseInOut(-1, 1, 1, 1);
        public AnimationCurve OverSlopeLimitSlopeForceFromDot = AnimationCurve.EaseInOut(-1, 1, 1, 1);
        private Vector3 slopeGoalVelocity;
        private float slopeAngle = 0f;
        
        [Header("Jump")]
        public float JumpForce = 650f;
        public AnimationCurve JumpFactorFromCurrentVelocity = AnimationCurve.Linear(0, 1, 1, 0);
        public AnimationCurve SlopeJumpFactorFromCurrentVelocity = AnimationCurve.Linear(0, 1, 1, 0);
        public float SlopeJumpFactor = 1f;
        public float MaximumJumpTime = 0.2f;
        public float MaximumCoyoteTime = 0.15f;
        public float MaximumJumpBufferTime = 0.2f;
        private bool isJumping = false;
        private bool jumpWasPressed = false;
        private float jumpTime;
        private float coyoteTime;
        private float jumpBufferTime;
        private Vector3 jumpDirection;
        
        void Start() {
            rb = GetComponent<Rigidbody>();
            jumpDirection = -gDownDir;
        }
        
        private void FixedUpdate() {
            if (LockRigidbody) {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                return;
            }
            if (FreezePhysicsCalculations) {
                return;
            }
            UpdateGrounded();
            Vector3 ridingForce = UpdateRidingForce(); 
            rb.AddForce(ridingForce);
            Vector3 movementForce = UpdateMovementForce();
            Vector3 slopeForce = UpdateSlopeForce(movementForce, ridingForce);
            rb.AddForce(slopeForce);
            Vector3 jumpForce = UpdateJumpForce();
            if (Mathf.Abs(jumpForce.magnitude) > 0) {
                rb.AddForce(jumpForce, ForceMode.VelocityChange);
            }
            totalForce = jumpForce + slopeForce + ridingForce;

            UpdateUprightTorque();
            UpdateTargetRotation();
        }

        private void UpdateGrounded() {
            float groundDistanceMagnitude = GroundHit.distance - RideHeight;
            GroundRay = new Ray(transform.position, gDownDir);
            groundCastHit = Physics.Raycast(GroundRay, out GroundHit, RayLength, RayMask);

            //Detect if we are grounded with the ground raycast and ground distance threshold
            if (groundDistanceMagnitude < groundThreshold && groundCastHit && (jumpTime <= 0 || jumpTime == MaximumJumpTime)) {
                grounded = true;
                lastGroundedTime = Time.time;
                lastHitWhileGrounded = GroundHit;
            }
            else {
                grounded = false;
            }
        }

        private void UpdateTargetRotation() {
            var dir = Vector3.ProjectOnPlane(rb.velocity, -gDownDir.normalized).normalized;
            if (LockInput || dir == Vector3.zero) {
                return;
            }
            Quaternion goalRotation = Quaternion.LookRotation(-dir, Vector3.up);
            targetRotation = Quaternion.Slerp(targetRotation, goalRotation, Time.fixedDeltaTime * MaxAccelerationForce);
        }
        private Vector3 UpdateMovementForce() {
            Vector3 returnForce = Vector3.zero;
            float acceleration = Acceleration;

            Vector3 goalVel = inputDirection * MaxSpeed * speedFactor;
            movementGoalVelocity = Vector3.MoveTowards(movementGoalVelocity, (goalVel) + (groundVelocity), acceleration * Time.fixedDeltaTime);
            Vector3 neededAccel = (movementGoalVelocity - rb.velocity) / Time.fixedDeltaTime;

            float maxAccel = MaxAccelerationForce * maxAccelForceFactor;
            neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
            returnForce = Vector3.Scale(neededAccel * rb.mass, MovementForceScale);
            return returnForce;
            //Vector3 returnForce = Vector3.zero;
            //Vector3 unitVel = movementGoalVelocity.normalized;
            //float velDot = Vector3.Dot(inputDirection, unitVel);
            //float acceleration = Acceleration * AccelerationFactorFromDot.Evaluate(velDot);

            //Vector3 goalVel = Vector3.zero;
            //goalVel = inputDirection * MaxSpeed * speedFactor;


            //movementGoalVelocity = Vector3.MoveTowards(movementGoalVelocity, (goalVel) + (groundVelocity), acceleration * Time.fixedDeltaTime);
            //Vector3 neededAccel = (movementGoalVelocity - rb.velocity) / Time.fixedDeltaTime;

            //float maxAccel = MaxAccelerationForce * MaxAccelerationForceFactorFromDot.Evaluate(velDot) * maxAccelForceFactor;
            //neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
            //returnForce = Vector3.Scale(neededAccel * rb.mass, MovementForceScale);
            //return returnForce;
        }


        private Vector3 UpdateSlopeForce(Vector3 MovementForce, Vector3 RidingForce) {
            Vector3 calculatedForce = MovementForce;
            if (Grounded) {
                //Get the slope from the ground hit normal.
                Vector3 slope = GroundHit.normal.normalized;
                slopeAngle = Vector3.Angle(slope, -gDownDir.normalized);

                if (slopeAngle != 0) {
                    //Project the movement force onto the slope.
                    Vector3 projectedForce = Vector3.ProjectOnPlane(MovementForce, slope);
                    Vector3 projectedInputDirection = Vector3.ProjectOnPlane(inputDirection, slope);
                    calculatedForce = projectedForce;

                    //Get the rb predicted movement before slope
                    Vector3 totalVelocityThisFrame = rb.velocity + ((calculatedForce / rb.mass) * Time.fixedDeltaTime);
                    Vector3 movementDirection = totalVelocityThisFrame.normalized;

                    //Get the direction going uphill on the slope.
                    Vector3 right = Vector3.Cross(gDownDir.normalized, slope).normalized;
                    Vector3 upSlope = Vector3.Cross(-slope, right).normalized;
                    Vector3 downSlope = Vector3.Cross(slope, right).normalized;

                    Vector3 velDir = (downSlope + gDownDir).normalized;
                    
                    Vector3 unitVel = slopeGoalVelocity.normalized;
                    float velDot = Vector3.Dot(projectedInputDirection, unitVel);
                    float acceleration = Acceleration * AccelerationFactorFromDot.Evaluate(velDot);

                    //Get how similar the movement direction is to the slope up direction.
                    float slopeDot = Vector3.Dot(movementDirection, upSlope);
                    //This allows for more control over the slope force, for example even though we have a natrual speed boost when going downhill
                    //We can remove this by lowering the animation curve in the negative values, or we can move even faster downslope by increasing the negative values.
                    float slopeForceFactor = SlopeForceFactor * UnderSlopeLimitSlopeForceFromDot.Evaluate(slopeDot);
                    float limitSlopeForceFactor = SlopeForceFactor * OverSlopeLimitSlopeForceFromDot.Evaluate(slopeDot);

                    Vector3 goalVel = Vector3.zero;
                    //Get the slope angle force factor
                    //The closer the slope is to the slope angle limit the more force is applied.
                    //going over the slope angle limit the desired goal velocity just barely overcomes the projectedForce when going uphill, and boosts the force when going downhill
                    float slopeAngleFactor = 1f - (SlopeLimit - slopeAngle) / SlopeLimit;


                    //If the slope is too steep, we want to go down.
                    if (slopeAngle > SlopeLimit) {
                        //Sometimes the player force can overcome the slope limit
                        //To prevent this we swap out our slope force animation curve, with one where the positive value is at least 2.1f
                        //This makes it so the slope force is increased when the player is trying to go uphill, but preserved when going downhill.
                        //This makes the natrual speed boost when going downhill not compound ontop of the slope force, preserving a sense of gravity.
                        goalVel = (velDir * (Gravity).magnitude * slopeAngleFactor * limitSlopeForceFactor);

                        float goalVelDot = Vector3.Dot(goalVel, -gDownDir);
                        float calcForceDot = Vector3.Dot(calculatedForce, -gDownDir);
                        //Already moving downhill, so we want to keep the force going.
                        if (goalVelDot > calcForceDot) {
                            goalVel = Vector3.zero;
                        }

                        if (projectedInputDirection != Vector3.zero) {
                            //Since we are within the slope limit we don't need as much force to slow down the player going up hill
                            //So we use a different animation curve than the slope limit curve, as it can let us go faster down hill, or the same speed
                            goalVel = (velDir * (Gravity).magnitude * slopeForceFactor * limitSlopeForceFactor);
                        }

                    }//Otherwise only apply the force if the input is moving 
                    else if (projectedInputDirection != Vector3.zero) {
                        //Since we are within the slope limit we don't need as much force to slow down the player going up hill
                        //So we use a different animation curve than the slope limit curve, as it can let us go faster down hill, or the same speed
                        goalVel =  (velDir * slopeForceFactor * slopeAngleFactor);
                    }

                    slopeGoalVelocity = Vector3.MoveTowards(slopeGoalVelocity, goalVel, acceleration * Time.fixedDeltaTime);
                    Vector3 neededAccel = slopeGoalVelocity / Time.fixedDeltaTime;

                    float maxAccel = MaxAccelerationForce * MaxAccelerationForceFactorFromDot.Evaluate(velDot) * maxAccelForceFactor;
                    neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

                    Vector3 slopeForce = neededAccel * rb.mass;
                    Debug.DrawLine(transform.position, transform.position + rb.velocity, Color.magenta);

                    calculatedForce += slopeForce;
                }
            }
            return calculatedForce;
        }
        private Vector3 UpdateRidingForce() {
            Vector3 returnForce = Vector3.zero;


            if (groundCastHit) {
                Vector3 currVel = rb.velocity;
                Vector3 rayDirection = transform.TransformDirection(gDownDir);

                Vector3 otherVel = Vector3.zero;
                Rigidbody otherRB = GroundHit.rigidbody;

                if (otherRB != null) {
                    otherVel = otherRB.velocity;
                    groundVelocity = otherVel;
                }
                else {
                    groundVelocity = Vector3.zero;
                }

                float rayDirectionVelocity = Vector3.Dot(rayDirection, currVel);
                float otherDirectionVelocity = Vector3.Dot(rayDirection, otherVel);

                float relativeVelocity = rayDirectionVelocity - otherDirectionVelocity;

                float groundDistanceMagnitude = GroundHit.distance - RideHeight;

                float springForce = (groundDistanceMagnitude * RideSpringStrength) - (relativeVelocity * RideSpringDamper);
                if (!IsSurfaceWithinSlopeLimit(GroundHit.normal)) {
                    springForce = (groundDistanceMagnitude * RideSpringStrength) - (relativeVelocity * RideSpringDamper);
                }
                
                if (grounded) {
                    returnForce = rayDirection * springForce;
                }
                if (otherRB != null) {
                    otherRB.AddForceAtPosition(rayDirection * -springForce, GroundHit.point);
                }

            }
            return returnForce;
        }

        private Vector3 UpdateJumpForce() {
            float calculatedJumpForce = 0;
            float jumpSlopeForceFactor = 1f;
            float jumpForceFactor = 1f;
            float VelocityInJumpDirection = Vector3.Dot(rb.velocity, jumpDirection);
            float currentForce = (VelocityInJumpDirection / rb.mass) * Time.fixedDeltaTime;
            float animCurveVal = currentForce / JumpForce;
            if (!IsSurfaceWithinSlopeLimit(lastHitWhileGrounded.normal)) {
                Vector3 slope = lastHitWhileGrounded.normal.normalized;
                Vector3 right = Vector3.Cross(-gDownDir.normalized, slope).normalized;
                Vector3 backSlope = Vector3.Cross(gDownDir.normalized, right).normalized;
                //Vector3 downSlope = Vector3.Cross(slope, right).normalized;
                jumpDirection = backSlope + (slope * 0.5f);
                jumpDirection = jumpDirection.normalized;
                if (inputDirection != Vector3.zero) {
                    Vector3 projectedInput = Vector3.ProjectOnPlane(inputDirection, -gDownDir.normalized);
                    float projectedInputForceAwayFromSlope = Vector3.Dot(projectedInput, backSlope);
                    projectedInputForceAwayFromSlope = Mathf.Clamp01(projectedInputForceAwayFromSlope);
                    jumpSlopeForceFactor = SlopeJumpFactor * projectedInputForceAwayFromSlope;
                }
                else {
                    jumpSlopeForceFactor = SlopeJumpFactor;
                    if (isJumping) {
                        inputDirection = backSlope;
                    }
                }
                jumpForceFactor = SlopeJumpFactorFromCurrentVelocity.Evaluate(animCurveVal);
            } else {
                jumpForceFactor = JumpFactorFromCurrentVelocity.Evaluate(animCurveVal);
                jumpDirection = -gDownDir.normalized;
                jumpSlopeForceFactor = 1f;
            }

            if (jumpBufferTime > 0 && !isJumping && coyoteTime > 0) {
                calculatedJumpForce = JumpForce * jumpForceFactor * JumpFactorFromCurrentVelocity.Evaluate(animCurveVal);
            }
            else if (jumpWasPressed && isJumping && jumpTime > 0) {
                calculatedJumpForce = JumpForce * jumpForceFactor * JumpFactorFromCurrentVelocity.Evaluate(animCurveVal);
            } else if (isJumping && grounded || jumpTime <= 0) {
                isJumping = false;
            }

            if (Mathf.Abs(calculatedJumpForce) > 0 && Mathf.Abs(jumpSlopeForceFactor) > 0) {
                var vel = rb.velocity;
                float difference = Vector3.Dot(vel, -gDownDir.normalized);
                if (difference > 0f) {
                    vel -= -gDownDir.normalized * difference;
                }
                rb.velocity = vel;
                if (jumpBufferTime > 0 && !isJumping && coyoteTime > 0) {
                    isJumping = true;
                    grounded = false;
                    jumpBufferTime = 0;
                    coyoteTime = 0;
                }
            }
            
            SetJumpTime();
            SetCoyoteTime();
            SetJumpBufferTime();
            jumpWasPressed = false;
            return jumpDirection * calculatedJumpForce * jumpSlopeForceFactor;
        }
        
        private void SetJumpTime() {
            if (isJumping && !grounded) {
                jumpTime -= Time.fixedDeltaTime;
            }
            else {
                jumpTime = MaximumJumpTime;
            }
        }
        
        private void SetCoyoteTime() {
            if (grounded) {
                coyoteTime = MaximumCoyoteTime;
            }
            else {
                coyoteTime -= Time.fixedDeltaTime;
            }
        }
        private void SetJumpBufferTime() {
            if (jumpWasPressed) {
                jumpBufferTime = MaximumJumpBufferTime;
            }
            else if(jumpBufferTime > 0){
                jumpBufferTime -= Time.fixedDeltaTime;
            }
        }
        private bool IsSurfaceWithinSlopeLimit(Vector3 normal) {
            return Vector3.Angle(normal, Vector3.up) <= SlopeLimit;
        }

        

        private void UpdateUprightTorque() {
            Quaternion current = transform.rotation;
            Quaternion goal = ShortestRotation(targetRotation, current);

            Vector3 rotationAxis;
            float rotationDegrees;

            goal.ToAngleAxis(out rotationDegrees, out rotationAxis);
            rotationAxis.Normalize();

            float rotationInRadians = rotationDegrees * Mathf.Deg2Rad;

            Vector3 torque = (rotationAxis * (rotationInRadians * UprightJointSpringStrength)) - (rb.angularVelocity * UprightJointSpringDamper);

            rb.AddTorque(torque);
        }

        public void Jump() {
            if (LockInput) {
                return;
            }
            jumpWasPressed = true;
        }

        public void Move(Vector3 direction) {
            if (LockInput) {
                inputDirection = Vector3.zero;
                return;
            }
            inputDirection = direction;
            if (inputDirection.magnitude > 1.0f) {
                inputDirection.Normalize();
            }
        }

        private Quaternion ShortestRotation(Quaternion a, Quaternion b) {
            if (Quaternion.Dot(a, b) < 0) {

                return a * Quaternion.Inverse(Multiply(b, -1));

            }
            else return a * Quaternion.Inverse(b);
        }



        private static Quaternion Multiply(Quaternion input, float scalar) {
            return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
        }
    }


    //public class HumanoidPhysicsBasedController : MonoBehaviour {

    //    private Rigidbody rb;

    //    public bool LockInput = false;
    //    public bool LockRigidbody = false;
    //    public bool FreezePhysicsCalculations = false;

    //    public float SlopeLimit = 45f;
        
    //    public float MaxSpeed = 10f;
    //    public float Acceleration = 200f;
    //    public AnimationCurve AccelerationFactorFromDot = AnimationCurve.Linear(-1, 2, 0, 1);
    //    public float MaxAccelerationForce = 150f;
    //    public AnimationCurve MaxAccelerationForceFactorFromDot = AnimationCurve.Linear(-1, 2, 0, 1);
    //    public Vector3 MovementForceScale = new Vector3(1, 0, 1);
    //    public float GravityScaleDrop = 10f;

    //    private bool tryJump = false;
    //    private float lastTimeJumpWasPressed = -1f;
    //    public float JumpForce = 7.5f;
    //    public AnimationCurve JumpForceFactorFromCurrentVelocity = AnimationCurve.Linear(0, 1, 1, 0);
    //    public AnimationCurve AnalogJumpForce = AnimationCurve.Linear(0, 1, 1, 0);
    //    public float JumpTerminalVelocity = 22.5f;
    //    public float JumpDuration = 1f / 3f;
    //    public float CoyoteTime = 0.1f;
    //    float jumpTime = 0f;
    //    bool canJump { get { return jumpTime <= JumpDuration; } }

    //    public float RideHeight = 1f;
    //    public float RideSpringDamper = 1000f;
    //    public float RideSpringStrength = 2000f;
    //    public float UprightJointSpringStrength = 10f;
    //    public float UprightJointSpringDamper = 0.5f;

    //    private Quaternion targetRotation = Quaternion.identity;

    //    public Ray GroundRay;
    //    public float RayLength = 1f;
    //    public LayerMask RayMask;
    //    public RaycastHit GroundHit;

    //    public float groundThreshold = 0.6f;
        
    //    private Vector3 gDownDir => Vector3.Normalize(gravity);
    //    private Vector3 gravity => Physics.gravity;

    //    private Vector3 inputDirection = Vector3.zero;
    //    private Vector3 goalVelocity;
    //    private float speedFactor = 1f;
    //    private float maxAccelForceFactor = 1f;

    //    private Vector3 groundVelocity = Vector3.zero;

    //    private bool grounded = false;
    //    private float lastGroundedTime = 0f;
        
    //    public float GroundedTime { get { return lastGroundedTime; } }
    //    public bool Grounded { get { return grounded; } }


    //    // Start is called before the first frame update
    //    void Start() {
    //        rb = GetComponent<Rigidbody>();
    //    }

    //    // Update is called once per frame
    //    void Update() {

    //    }

    //    private void FixedUpdate() {
    //        if (LockRigidbody) {
    //            rb.velocity = Vector3.zero;
    //            rb.angularVelocity = Vector3.zero;
    //            return;
    //        }
    //        if (FreezePhysicsCalculations) {
    //            return;
    //        }
    //        UpdateMovementForce();
    //        UpdateRidingForce();
    //        UpdateUprightTorque();
    //        UpdateTargetRotationFromInputDirection();
    //        UpdateJumpForce();
    //    }

    //    private void UpdateTargetRotationFromInputDirection() {
    //        if (LockInput || inputDirection == Vector3.zero) {
    //            return;
    //        }
    //        Quaternion goalRotation = Quaternion.LookRotation(-inputDirection, Vector3.up);
    //        targetRotation = Quaternion.Slerp(targetRotation, goalRotation, Time.fixedDeltaTime * MaxAccelerationForce);
    //    }

    //    private void UpdateJumpForce() {
    //        jumpTime = lastTimeJumpWasPressed - (lastGroundedTime + CoyoteTime);
    //        if (tryJump) {
    //            if (!canJump) {
    //                tryJump = false;
    //                return;
    //            } else {
    //                grounded = false;
    //            }
    //        }
    //        if (tryJump && canJump) {
    //            //we apply the jump force every fixed update, so we need to cancel out the velocity from the last frame
    //            //we do this by subtracting the difference between the velocity and the jump direction, multiplied by the jump direction.
    //            Vector3 jumpDirection = -gDownDir;
    //            if (!IsSurfaceWithinSlopeLimit(GroundHit.normal)) {
    //                Vector3 forward = Vector3.Cross(GroundHit.normal, Vector3.up);
    //                jumpDirection = forward;
    //            }
    //            Vector3 vel = rb.velocity;
    //            float difference = Vector3.Dot(vel, jumpDirection);
    //            if (difference >0f) {
    //                vel -= jumpDirection * difference;
    //            }
    //            rb.velocity = vel;
                
    //            float jumpForce = JumpForce;
    //            if (IsSurfaceWithinSlopeLimit(GroundHit.normal))
    //                jumpForce *= JumpForceFactorFromCurrentVelocity.Evaluate(Vector3.Scale(vel, jumpDirection).magnitude);
    //            else
    //                jumpForce *= GravityScaleDrop;
                
    //            jumpForce *= AnalogJumpForce.Evaluate(jumpTime);
    //            rb.AddForce(jumpDirection * jumpForce, ForceMode.Force);
    //            if (GroundHit.rigidbody != null && grounded) {
    //                GroundHit.rigidbody.AddForceAtPosition(-jumpDirection * jumpForce, GroundHit.point);
    //            }
    //            tryJump = false;
    //        } else {
    //            tryJump = false;
    //        }
    //    }
        
    //    private void UpdateMovementForce() {
    //        Vector3 unitVel = goalVelocity.normalized;
    //        float velDot = Vector3.Dot(inputDirection, unitVel);
    //        float acceleration = Acceleration * AccelerationFactorFromDot.Evaluate(velDot);

    //        Vector3 goalVel = Vector3.zero;
    //        goalVel = inputDirection * MaxSpeed * speedFactor;
    //        goalVel = Quaternion.FromToRotation(transform.up, rb.transform.InverseTransformDirection(GroundHit.normal)) * goalVel;
    //        goalVel += goalVel * ((Vector3.Angle(goalVel, transform.up) - 90.0f) / SlopeLimit);

    //        goalVelocity = Vector3.MoveTowards(goalVelocity, (goalVel) + (groundVelocity), acceleration * Time.fixedDeltaTime);
    //        Vector3 neededAccel = (goalVelocity - rb.velocity) / Time.fixedDeltaTime;

    //        float maxAccel = MaxAccelerationForce * MaxAccelerationForceFactorFromDot.Evaluate(velDot) * maxAccelForceFactor;
    //        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
    //        rb.AddForce(Vector3.Scale(neededAccel * rb.mass, MovementForceScale), ForceMode.Force);
    //    }

    //    private bool IsSurfaceWithinSlopeLimit(Vector3 normal) {
    //        return Vector3.Angle(normal, Vector3.up) <= SlopeLimit;
    //    }

    //    private void UpdateRidingForce() {
    //        GroundRay = new Ray(transform.position, gDownDir);
    //        bool hit = false;

    //        hit = Physics.Raycast(GroundRay, out GroundHit, RayLength, RayMask);

    //        if (hit) {
    //            Vector3 currVel = rb.velocity;
    //            Vector3 rayDirection = transform.TransformDirection(gDownDir);

    //            Vector3 otherVel = Vector3.zero;
    //            Rigidbody otherRB = GroundHit.rigidbody;

    //            if (otherRB != null) {
    //                otherVel = otherRB.velocity;
    //                groundVelocity = otherVel;
    //            } else {
    //                if (IsSurfaceWithinSlopeLimit(GroundHit.normal)) {
    //                    groundVelocity = Vector3.zero;
    //                }
    //                else if(grounded) {
    //                    Vector3 gravityForce = Vector3.Project(-gravity, GroundHit.normal);
    //                    groundVelocity = gravityForce;
    //                } else {
    //                    groundVelocity = Vector3.zero;
    //                }
    //            }

    //            float rayDirectionVelocity = Vector3.Dot(rayDirection, currVel);
    //            float otherDirectionVelocity = Vector3.Dot(rayDirection, otherVel);

    //            float relativeVelocity = rayDirectionVelocity - otherDirectionVelocity;

    //            float groundDistanceMagnitude = GroundHit.distance - RideHeight;

    //            float springForce = (groundDistanceMagnitude * RideSpringStrength) - (relativeVelocity * RideSpringDamper);
    //            //Detect if we are grounded with the ground raycast and ground distance threshold

    //            if (grounded) {
    //                rb.AddForce(rayDirection * springForce);
    //            }
    //            if (otherRB != null) {
    //                otherRB.AddForceAtPosition(rayDirection * -springForce, GroundHit.point);
    //            }
    //            if (groundDistanceMagnitude < groundThreshold) {
    //                grounded = true;
    //                //rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    //                lastGroundedTime = Time.time;
    //            }
    //            else {
    //                grounded = false;
    //            }
    //        }
    //    }

    //    private void UpdateUprightTorque() {
    //        Quaternion current = transform.rotation;
    //        Quaternion goal = ShortestRotation(targetRotation, current);

    //        Vector3 rotationAxis;
    //        float rotationDegrees;

    //        goal.ToAngleAxis(out rotationDegrees, out rotationAxis);
    //        rotationAxis.Normalize();

    //        float rotationInRadians = rotationDegrees * Mathf.Deg2Rad;

    //        Vector3 torque = (rotationAxis * (rotationInRadians * UprightJointSpringStrength)) - (rb.angularVelocity * UprightJointSpringDamper);

    //        rb.AddTorque(torque);
    //    }

    //    public void Jump() {
    //        if (LockInput) {
    //            return;
    //        }
    //        tryJump = true;
    //        lastTimeJumpWasPressed = Time.time;
    //    }

    //    public void Move(Vector3 direction) {
    //        if (LockInput) {
    //            inputDirection = Vector3.zero;
    //            return;
    //        }
    //        inputDirection = direction;
    //        if (inputDirection.magnitude > 1.0f) {
    //            inputDirection.Normalize();
    //        }
    //    }

    //    private Quaternion ShortestRotation(Quaternion a, Quaternion b) {
    //        if (Quaternion.Dot(a, b) < 0) {

    //            return a * Quaternion.Inverse(Multiply(b, -1));

    //        }
    //        else return a * Quaternion.Inverse(b);
    //    }



    //    private static Quaternion Multiply(Quaternion input, float scalar) {
    //        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    //    }
    //}
}