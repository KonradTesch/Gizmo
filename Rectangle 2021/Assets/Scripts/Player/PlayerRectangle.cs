using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rectangle.Player
{
    /// <summary>
    /// The player movement for the rectangle mode.
    /// </summary>
    public class PlayerRectangle : PlayerBase
    {
        private bool canDoubleJump;

        /// <summary>
        /// Let the rectangle player jump.
        /// </summary>
        public override void Jump()
        {
            if (currentCoyoteTime > 0)
            {
                grounded = false;

                //Resets the velocity on the y axis
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

                //Add the jump force
                rigidBody.AddForce(new Vector2(0f, jumpForce * 10));
                currentCoyoteTime = 0;
            }
            else if (canDoubleJump && !onRamp)
            {
                //Resets the velocity on the y axis
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

                //Add the jump force
                rigidBody.AddForce(new Vector2(0f, jumpForce * 10));
                canDoubleJump = false;
            }
        }

        protected override void PositionCheck()
        {
            base.PositionCheck();
            if(grounded)
            {
                canDoubleJump = true;
            }

            if ((Mathf.Abs(transform.rotation.eulerAngles.z) % 90 < 1 || Mathf.Abs(transform.rotation.eulerAngles.z) % 90 > 89) && Mathf.Abs(transform.rotation.eulerAngles.z) > 45)
            {
                transform.rotation = Quaternion.identity;
            }

        }
    }
}