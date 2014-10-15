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
            this.sailAlpha = 0.005f;
            this.slack = initialSailTheta; // start out on full slack
            this.slackMin = pi / 2;
            this.slackMax = 3 * pi / 2;
            this.minAbsWindAngle = pi / 6; // min angle one can head into wind at, and still get thrust
            this.optimumUpwindAngle = pi / 5;
            this.deviationExp = 3;
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
            float delta = gameTime.ElapsedGameTime.Milliseconds;
            
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

            bool isSailRight = sailTheta <= pi;
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
            Debug.WriteLine(isWindOnside);

            // find out how much thrust wind is giving boat
            float windFactor; // 0-1
            if ( ! MathUtil.IsZero(sailTheta - slack))
            {
                // can't have thrust unless sail is fully extended
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

            if (windFactor < 0f || windFactor > 1f) { throw new Exception("windFactor outside range [0, 1]"); }

            // adjust velocity of boat based on windFactor and windSpeed
            Vector3 deltaVel = delta * acc * dir; // note this is the max possible value deltaVel can take, since windFactor,windSpeed <= 1
            deltaVel = windFactor * wind.getSpeed() * deltaVel;
            this.vel += deltaVel;

            // TODO buoyant and gravitational forces should also effect velocity

            // finally adjust position of boat
            pos += delta * vel;

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
            if (isSailRight && sailTheta < slack)
            {
                sailTheta = slack;
            }
            if (!isSailRight && sailTheta > slack)
            {
                sailTheta = slack;
            }

            if (vel.Length() > maxVel)
            {
                vel.Normalize();
                vel = maxVel * vel;
            }

            Debug.WriteLine(sailTheta);
            Matrix rotation = getRotationMatrix();
            Matrix translation = getTranslationMatrix();
            Matrix sailRotation = Matrix.RotationY(sailTheta - initialSailTheta);
            this.World = rotation * translation;
            this.sailWorld = rotation * sailRotation * translation;
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
