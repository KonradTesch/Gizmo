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
            if (grounded)
            {
                grounded = false;
                rigidBody.AddForce(new Vector2(0f, jumpForce * 10));

                canDoubleJump = true;
            }
            else if (canDoubleJump)
            {
                //Resets the velocity on the y axis
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

                //Add the jump force
                rigidBody.AddForce(new Vector2(0f, jumpForce * 10));
                canDoubleJump = false;
            }
        }
    }
}