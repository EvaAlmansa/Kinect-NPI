using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    public class MoveLeftHandtoShoulderXY
    {
        private Skeleton skeletonControl;

        const double MINERROR = 0.5;
        const double MAXERROR = 1.0;

        private double maxShoulderY;
        private double minShoulderY;

        private bool correctPos;

        private int level;

        public MoveLeftHandtoShoulderXY() 
        {
            correctPos = false;
            level = 0;
            maxShoulderY = 0.0;
            minShoulderY = 0.0;
        }

        public void resetLevel() 
        {
            level = 0;
        }

        /*Detect the progressive movement in the XY plane
         return: 0 (incorrect move), 1 (almost move), 2 (almost move), 3 (correct move)
         */
        public int detectingSkeleton(Skeleton inSk) 
        {
            bool positionX;
            bool positionY;
            bool positionZ;

            skeletonControl = inSk;
            

            //Get the initial position of the skeleton
            Joint shoulder = skeletonControl.Joints[JointType.ShoulderLeft];
            Joint elbow = skeletonControl.Joints[JointType.ElbowLeft];
            Joint wrist = skeletonControl.Joints[JointType.WristLeft];
            Joint hand = skeletonControl.Joints[JointType.HandLeft];

            switch (level)
            {
                case 0:
                    positionX = (rangeX(shoulder, elbow) && rangeX(elbow, wrist));
                    positionY = (rangeY(shoulder, elbow, MINERROR) && (rangeY(elbow, wrist, MINERROR)));
                    positionZ = (rangeZ(shoulder, elbow, MINERROR) && (rangeZ(shoulder, wrist, MINERROR)));
                    if (positionX && positionY && positionZ)
                    {
                        calculateMinMaxPosY(shoulder, MINERROR);
                        level = 1;
                        //correctPos = true;
                    }

                    break;
                case 1:
                    positionY = moveHand(shoulder, wrist);
                    if (positionY)
                    { 
                        level = 2;
                        //correctPos = true;
                    }
                    else 
                        level = 0;

                    break;
                case 2:
                    correctPos = move24(shoulder, elbow, wrist);
                    if (!correctPos)
                        level = 0;
                    else
                        level = 3;
                    break;
            }

            return (level);
        }  

        private bool rangeX(Joint point1, Joint point2)
        {
            return (point1.Position.X > point2.Position.X);
        }

        private bool rangeY(Joint point1, Joint point2, double error) 
        {
            double rateError = point1.Position.Y * error;
            return ((point1.Position.Y <= point2.Position.Y + rateError) && (point1.Position.Y >= point2.Position.Y - rateError));
        }

        private bool rangeZ(Joint point1, Joint point2, double error) 
        {
            return ((point1.Position.Z <= point2.Position.Z + error) && (point1.Position.Z >= point2.Position.Z - error));
        }

        private void calculateMinMaxPosY(Joint shoulder, double error) 
        {
            maxShoulderY = shoulder.Position.Y + shoulder.Position.Y * error;
            minShoulderY = shoulder.Position.Y + maxShoulderY * (-1.0);
        }

        private bool moveHand(Joint shoulder, Joint hand) 
        {
            bool posY = (hand.Position.Y <= maxShoulderY) && (hand.Position.Y >= minShoulderY);
            bool posX = (shoulder.Position.X > hand.Position.X);
            return (posY && posX);
        }

        private bool move24(Joint shoulder, Joint elbow, Joint wrist) 
        {
            bool posY = (wrist.Position.Y <= maxShoulderY) && (wrist.Position.Y >= minShoulderY);
            bool posXsw = (shoulder.Position.X >= wrist.Position.X);
            bool posXeh = ((elbow.Position.X <= wrist.Position.X));
            
            return (posY && posXsw && posXeh);
        }
        
        public bool fitness24(JointType point1, JointType point2) 
        { 
            bool p1 = (point1.Equals(JointType.ShoulderLeft) || (point1.Equals(JointType.ElbowLeft)) || (point1.Equals(JointType.WristLeft)) || (point1.Equals(JointType.HandLeft)));
            bool p2 = (point2.Equals(JointType.ShoulderLeft) || (point2.Equals(JointType.ElbowLeft)) || (point2.Equals(JointType.WristLeft)) || (point2.Equals(JointType.HandLeft)));

            return (p1 && p2);
        }
    }
}

