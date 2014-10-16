using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    using SharpDX.Toolkit.Graphics;
    using System.Diagnostics;
    public class Player : ModelGameObject
    {
        public enum BoatSize { SMALL, LARGE };
        public enum HullPositions { BACK_LEFT, BACK_RIGHT, FRONT_LEFT, FRONT_RIGHT };
        private float pi = (float)Math.PI;

        private Model sailModel;
        private Matrix sailWorld;
        private float initialSailTheta, sailTheta, sailOmega;
        private float sailMaxOmega, sailAlpha;

        private BoatSize boatSize;
        private float length, width;
        private float slack;
        private float slackMin, slackMax;
        private float minAbsWindAngle;
        private float optimumUpwindAngle;
        private int deviationExp;

        private float linearBuoyancy;
        private float rotationalBuoyancy;
        private float oceanDamping;

        private float alpha;
        private float omega, maxOmega;
        private float rotationalDamping;

        private Dictionary<HullPositions, Vector3> hullPositions;

        public Player(IllegalOctopusFishingGame game, Vector3 startingPos, String boatModelName, String sailModelName, BoatSize boatSize) : base(game, startingPos, boatModelName)
        {
            hullPositions = new Dictionary<HullPositions, Vector3>();
            hullPositions.Add(HullPositions.BACK_LEFT, new Vector3());
            hullPositions.Add(HullPositions.BACK_RIGHT, new Vector3());
            hullPositions.Add(HullPositions.FRONT_LEFT, new Vector3());
            hullPositions.Add(HullPositions.FRONT_RIGHT, new Vector3());

            this.boatSize = boatSize;
            if (boatSize == BoatSize.SMALL)
            {
                length = 5f;
                width = 3f;
                mass = 400f;
                acc = 0.001f;
                maxVel = 0.08f;
            }
            else
            {
                length = 10f;
                width = 4f;
                mass = 800f;
                acc = 0.1f;
                maxVel = 0.15f;
            }

            // sail
            bool success = game.nameToModel.TryGetValue(sailModelName, out sailModel);
            if (!success)
            {
                throw new ArgumentException("failed to load sail model");
            }
            this.sailWorld = Matrix.Identity;
            this.initialSailTheta = pi/2; // sail starts off on starboard side
            this.sailTheta = initialSailTheta;
            this.sailOmega = 0f;

            this.sailMaxOmega = 0.001f;
            this.sailAlpha = 0.0005f;
            this.slack = 3*pi / 4;//initialSailTheta; // start out on full slack
            this.slackMin = pi / 2;
            this.slackMax = 3 * pi / 2;
            this.minAbsWindAngle = pi / 6; // min angle one can head into wind at, and still get thrust
            this.optimumUpwindAngle = pi / 5;
            this.deviationExp = 3;

            this.linearBuoyancy = 0.001f;
            this.rotationalBuoyancy = 0.0001f;
            this.oceanDamping = 0.008f;

            this.alpha = 0.000001f;
            this.omega = 0f;
            this.maxOmega = 0.01f;
            this.rotationalDamping = 0.04f;
        }

        internal Dictionary<HullPositions, Vector3> getHullPositions()
        {
            Vector3 left = Vector3.Cross(dir, up);
            Vector3 right = -1f * left;
            Vector3 back = -1f * dir;

            Vector3 backLeft = new Vector3(pos.X, pos.Y, pos.Z) + length*back + width*left;
            Vector3 backRight = new Vector3(pos.X, pos.Y, pos.Z) + length*back + width*right;
            Vector3 frontLeft = new Vector3(pos.X, pos.Y, pos.Z) + length*dir + width*left;
            Vector3 frontRight = new Vector3(pos.X, pos.Y, pos.Z) + length*dir + width*right;

            hullPositions[HullPositions.BACK_LEFT] = backLeft;
            hullPositions[HullPositions.BACK_RIGHT] = backRight;
            hullPositions[HullPositions.FRONT_LEFT] = frontLeft;
            hullPositions[HullPositions.FRONT_RIGHT] = frontRight;
            return hullPositions;
        }

        internal void Update(GameTime gameTime, Dictionary<HullPositions, float> hullTerrainHeights, Dictionary<HullPositions, float> hullOceanHeights, Wind wind, Gravity gravity)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            
            // TESTING
            /*
            Matrix rotation = Matrix.RotationY(0.001f * delta);
            dir = Vector3.TransformCoordinate(dir, rotation);
            vel += delta * acc * dir;
            */
            // actual code

            // wind
            Vector3 xzDir = new Vector3(dir.X, 0, dir.Z);
            xzDir.Normalize();

            // remember X cross Z == -Y
            // and X cross Y == Z. Left hand rule

            // find angle of wind (dir is angle 0, increases clockwise, around to 2*pi)
            Vector3 cross = Vector3.Cross(xzDir, wind.getDir());
            float windAngle = (float)Math.Acos(Vector3.Dot(xzDir, wind.getDir()));
            if (!MathUtil.IsZero(cross.Y) && cross.Y < 0)
            {
                windAngle = 2 * pi - windAngle;
            }

            bool isSailRight = getIsSailRight();
            float onSideLower, onSideHigher; // range of wind angles to be onside (on good side of sail)
            if (isSailRight)
            {
                onSideLower = sailTheta;
                onSideHigher = sailTheta + pi;
            }
            else
            {
                onSideLower = sailTheta - pi;
                onSideHigher = sailTheta;
            }
            bool isWindOnside = windAngle >= onSideLower && windAngle <= onSideHigher;

            // find out how much thrust wind is giving boat
            float windFactor; // 0-1
            if (getIsSailLoose())
            {
                // can't have thrust unless sail is taut
                windFactor = 0f;
            }
            else if (isWindOnside)
            {
                float absWindAngle = windAngle;
                if (absWindAngle > pi)
                {
                    absWindAngle = 2 * pi - absWindAngle;
                }

                if (absWindAngle < 0 || absWindAngle > pi) { throw new Exception("absWindAngle outside range [0-pi]"); }

                bool isWindInFront = absWindAngle < pi / 2;
                if (isWindInFront)
                {
                    // are we within the no go zone (heading into wind too directly)
                    if (absWindAngle < minAbsWindAngle)
                    {
                        windFactor = 0f;
                    }
                    else
                    {
                        // we're heading upwind, but not too steeply. calculate thrust
                        if (windAngle > (2 * pi - minAbsWindAngle) || windAngle < minAbsWindAngle) { throw new Exception("windAngle within no go zone when it shouldn't be"); }

                        float windSailAngle;
                        if (isSailRight)
                        {
                            windSailAngle = sailTheta - (windAngle - pi);
                        }
                        else
                        {
                            windSailAngle = (windAngle + pi) - sailTheta;
                        }

                        if (windSailAngle < 0 || windSailAngle > pi / 2) { throw new Exception("windSailAngle outside range [0-2pi]"); }

                        windFactor = getWindFactorFromFrontWindSailAngle(windSailAngle);
                    }
                }
                else
                {
                    // wind behind us, and onside
                    float windToSailFactor; // how much wind is perpendicular to sail
                    float sailToBoatFactor;
                    if (isSailRight)
                    {
                        if (windAngle > 3 * pi / 2 || windAngle < sailTheta) { throw new Exception("windAngle outside range [sailTheta - 3*pi/2]"); }
                        windToSailFactor = (float)Math.Sin(windAngle - sailTheta);
                        sailToBoatFactor = (float)Math.Cos(sailTheta - pi / 2);
                    }
                    else
                    {
                        if (windAngle > sailTheta || windAngle < pi / 2) { throw new Exception("windAngle outside range [pi/2 - sailTheta]"); }
                        windToSailFactor = (float)Math.Sin(sailTheta - windAngle);
                        sailToBoatFactor = (float)Math.Cos(3*pi / 2 - sailTheta);
                    }
                    windFactor = windToSailFactor * sailToBoatFactor;
                }
            }
            else
            {
                // wind is offside - no thrust
                windFactor = 0f;
            }

            if (!MathUtil.IsZero(windFactor) && !MathUtil.IsOne(windFactor) && (windFactor < 0f || windFactor > 1f)) { 
                throw new Exception("windFactor outside range [0, 1]"); 
            }

            // adjust velocity of boat based on windFactor and windSpeed
            Vector3 deltaVel = delta * acc * dir; // note this is the max possible value deltaVel can take, since windFactor,windSpeed <= 1
            deltaVel = windFactor * wind.getSpeed() * deltaVel;
            this.vel += deltaVel;

            // damping of vel
            Vector3 velUnit = vel;
            velUnit.Normalize();
            // 1 - abs value of cosine of angle between vel and dir (0 when aligned, 1 when orthogonal)
            float directionalDamping = 1 - (float)Math.Abs(Vector3.Dot(velUnit, dir));
            vel = (1 - directionalDamping) * (1 - oceanDamping) * vel;

            // damping of rotational velocity
            if (!MathUtil.IsZero(omega))
            {
                omega = (1 - rotationalDamping) * omega;
                // adjust boat's yaw
                float deltaYaw = delta * omega;
                Matrix yawRotation = Matrix.RotationAxis(up, deltaYaw);
                dir = Vector3.TransformCoordinate(dir, yawRotation);
            }


            // TODO buoyant and gravitational forces should also effect velocity
            // gravity
            /*
            Vector3 deltaGravityVel = delta * gravity.getG() * gravity.getDir();
            this.vel += deltaGravityVel;

            // want to reduce 4 hull ocean heights (due to 4 buoyant forces) to one deltaVel, one rotation
            float oceanHeightSum = 0f;
            foreach (HullPositions hullPos in hullOceanHeights.Keys)
            {
                oceanHeightSum += hullOceanHeights[hullPos] - pos.Y;
            }

            // no such thing as anti-buoyant force for being above water
            if (!MathUtil.IsZero(oceanHeightSum) && oceanHeightSum > 0f)
            {
                Vector3 deltaBuoyancyVel = delta * (oceanHeightSum / 4) * linearBuoyancy * Vector3.UnitY; // unit Y rather than up, since can be sideways underwater
                this.vel += deltaBuoyancyVel;
            }
            
            // TODO
            // calculate rotations due to buoyancy forces
            // break into pitch and roll
            float backOceanHeightSum = 0f;
            float frontOceanHeightSum = 0f;
            float leftOceanHeightSum = 0f;
            float rightOceanHeightSum = 0f;
            backOceanHeightSum = hullOceanHeights[HullPositions.BACK_LEFT] + hullOceanHeights[HullPositions.BACK_RIGHT] - 2*pos.Y;
            frontOceanHeightSum = hullOceanHeights[HullPositions.FRONT_LEFT] + hullOceanHeights[HullPositions.FRONT_RIGHT] - 2*pos.Y;
            leftOceanHeightSum = hullOceanHeights[HullPositions.BACK_LEFT] + hullOceanHeights[HullPositions.FRONT_LEFT] - 2*pos.Y;
            rightOceanHeightSum = hullOceanHeights[HullPositions.BACK_RIGHT] + hullOceanHeights[HullPositions.FRONT_RIGHT] - 2*pos.Y;

            float deltaPitch = (backOceanHeightSum - frontOceanHeightSum) / 2 * rotationalBuoyancy * delta;
            float deltaRoll = (rightOceanHeightSum - leftOceanHeightSum) / 2 * rotationalBuoyancy * delta;
            if (!MathUtil.IsZero(deltaPitch))
            {
                Vector3 right = Vector3.Cross(up, dir);
                if (!MathUtil.IsOne(right.Length())) { throw new Exception("right unit vector not one"); }

                Matrix pitchRotation = Matrix.RotationAxis(right, deltaPitch);
                dir = Vector3.TransformCoordinate(dir, pitchRotation);
                up = Vector3.TransformCoordinate(up, pitchRotation);
            }
            if (!MathUtil.IsZero(deltaRoll))
            {
                // TODO: do we even want to bother with roll?
                Matrix rollRotation = Matrix.RotationAxis(dir, deltaRoll);
                up = Vector3.TransformCoordinate(up, rollRotation);
            }
            */


            // finally adjust position of boat
            pos += delta * vel;
            // TODO: check if hull positions are underground, and if so bump them up

            // calculate change in sail's angular velocity
            float sailSpinFactor = (float)Math.Abs(Math.Sin(sailTheta - windAngle)); // how perpendicular wind is to sail
            float deltaSailOmega;
            if (MathUtil.IsZero(sailSpinFactor))
            {
                deltaSailOmega = 0f;
            }
            else if (isSailRight && isWindOnside || !isSailRight && !isWindOnside)
            {
                // wind is trying to turn sail ccw (negative change in angle)
                deltaSailOmega = -1 * delta * sailAlpha;
            }
            else
            {
                // wind is trying to turn sail cw (positive change in angle)
                deltaSailOmega = delta * sailAlpha;
            }

            // adjust sail's angular velocity
            sailOmega += deltaSailOmega;
            if (sailOmega > sailMaxOmega)
            {
                sailOmega = sailMaxOmega;
            }
            else if (sailOmega < -1 * sailMaxOmega)
            {
                sailOmega = -1 * sailMaxOmega;
            }

            // adjust sail angle
            sailTheta += delta * sailOmega;
            
            // switch slack to other side if we've changed sides
            bool updatedIsSailRight = getIsSailRight();
            if (isSailRight != updatedIsSailRight)
            {
                if (isSailRight)
                {
                    // sail was on right, now on left
                    slack = slackMax - (slack - slackMin);
                }
                else
                {
                    // sail was on left, now on right
                    slack = slackMin + (slackMax - slack);
                }
            }
            // check if sail has moved past slack allowance
            if (updatedIsSailRight && sailTheta < slack)
            {
                sailTheta = slack;
            } else if (!updatedIsSailRight && sailTheta > slack)
            {
                sailTheta = slack;
            }

            if (vel.Length() > maxVel)
            {
                vel.Normalize();
                vel = maxVel * vel;
            }
            /*
            Debug.WriteLine("sailTheta: " + sailTheta.ToString());
            Debug.WriteLine("windDir: " + wind.getDir().ToString());
            */
            Matrix modelRotation = getRotationMatrix();
            Matrix modelTranslation = getTranslationMatrix();
            Matrix sailRotation = Matrix.RotationY(sailTheta - initialSailTheta);
            Matrix sailRipple = Matrix.Identity;
            if (getIsSailLoose())
            {
                float total = (float)gameTime.TotalGameTime.TotalMilliseconds;
                float rippleTheta = (float)Math.Sin(total * 0.05f) / 8f;
                Debug.WriteLine(total * 0.001f);
                sailRipple = Matrix.RotationY(rippleTheta);
            }

            this.World = modelRotation * modelTranslation;
            this.sailWorld = sailRipple * modelRotation * sailRotation * modelTranslation;
        }

        private bool getIsSailLoose()
        {
            return !MathUtil.IsZero(sailTheta - slack);
        }

        private bool getIsSailRight() 
        {
            return sailTheta <= pi; 
        }

        public void releaseSlack(GameTime gameTime)
        {
            float delta = gameTime.ElapsedGameTime.Milliseconds;
            float deltaTheta = delta * sailMaxOmega;
            if (getIsSailRight())
            {
                deltaTheta = -1 * deltaTheta;
            }
            updateSlack(deltaTheta);
        }

        public void reduceSlack(GameTime gameTime)
        {
            float delta = gameTime.ElapsedGameTime.Milliseconds;
            float deltaTheta = delta * sailMaxOmega;
            if ( ! getIsSailRight())
            {
                deltaTheta = -1 * deltaTheta;
            }
            updateSlack(deltaTheta);
        }

        public void turnLeft(GameTime gameTime)
        {
            float delta = gameTime.ElapsedGameTime.Milliseconds;
            float deltaOmega = -1 * alpha * delta;
            omega += deltaOmega;
            if (omega > maxOmega)
            {
                omega = maxOmega;
            }
            else if (omega < -1 * maxOmega)
            {
                omega = -1 * maxOmega;
            }
            float deltaTheta = delta * omega;
        }

        public void turnRight(GameTime gameTime)
        {
            float delta = gameTime.ElapsedGameTime.Milliseconds;
            float deltaOmega = alpha * delta;
            omega += deltaOmega;
            if (omega > maxOmega)
            {
                omega = maxOmega;
            }
            else if (omega < -1 * maxOmega)
            {
                omega = -1 * maxOmega;
            }
            float deltaTheta = delta * omega;
        }

        private void updateSlack(float deltaTheta)
        {
            this.slack += deltaTheta;
            bool isSailRight = getIsSailRight();
            if (isSailRight)
            {
                if (slack > pi)
                {
                    slack = pi;
                } else if (slack < slackMin) {
                    slack = slackMin;
                }
            }
            else
            {
                if (slack < pi)
                {
                    slack = pi;
                }
                else if (slack > slackMax)
                {
                    slack = slackMax;
                }
            }
        }

        private float getWindFactorFromFrontWindSailAngle(float windSailAngle)
        {
            float deviationAngle = Math.Abs(windSailAngle - optimumUpwindAngle);
            float deviationFactor = (float)Math.Cos(deviationAngle);
            return (float)Math.Pow(deviationFactor, deviationExp);
        }

        public override void Draw(GameTime gameTime)
        {
            sailModel.Draw(game.GraphicsDevice, sailWorld, View, Projection);
            base.Draw(gameTime);
        }
    }
}
