using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    public struct vector3
    {
        public double x;
        public double y;
        public double z;
    }

    public class MoveLeftHandtoShoulderXY
    {
        private Skeleton skeletonControl;

        private bool angle180;
        private bool angle90;
        private bool angle0;
        private bool correctPos;
        private double angle;

        public MoveLeftHandtoShoulderXY() 
        {
            angle180 = false;
            angle90 = false;
            angle0 = false;
            correctPos = false;
        }

        public bool position180() 
        { 
            return (angle180);
        }

        public bool position90()
        {
            return (angle90);
        }

        public bool position0()
        {
            return (angle0);
        }

        /*Detect the progressive movement in the XY plane: 
         * 180 degrees, 90 degrees and 0 degree*/
        public bool detectingSkeleton(Skeleton inSk) 
        {
            skeletonControl = inSk;

            //Get the initial position of the skeleton
            Joint shoulder = skeletonControl.Joints[JointType.ShoulderLeft];
            Joint elbow = skeletonControl.Joints[JointType.ElbowLeft];
            Joint wrist = skeletonControl.Joints[JointType.WristLeft];
            Joint hand = skeletonControl.Joints[JointType.HandLeft];

            vector3 pointShoulderElbow = new vector3();
            vector3 pointElbowWrist = new vector3();
            pointShoulderElbow = calculateCoordinates(shoulder, elbow);
            pointElbowWrist = calculateCoordinates(elbow, wrist);

            angle = calculateAngleXY(pointShoulderElbow, pointElbowWrist);
         
            correctPos = rangeY(shoulder, elbow) && rangeZ(elbow, wrist);

            if (correctPos)
            {
                angle180 = (compAngle(angle, 180) || angle180);
                angle90 = (compAngle(angle, 90) || angle90);
                angle0 = (compAngle(angle, 0) || angle0);
            }
            else
                angle0 = angle180 = angle90 = false;
            
            return (angle0 && angle90 && angle180);
        }
        

        public vector3 calculateCoordinates(Joint point1, Joint point2) 
        {
            vector3 point;

            point.x = point1.Position.X - point2.Position.X;
            point.y = point1.Position.Y - point2.Position.Y;
            point.z = point1.Position.Z - point2.Position.Z;

            return point;
        }

        private bool rangeY(Joint point1, Joint point2) 
        {
            return ((point1.Position.Y < point2.Position.Y + 0.05) && (point1.Position.Y > point2.Position.Y - 0.05));
        }

        private bool rangeZ(Joint point1, Joint point2) 
        {
            return ((point1.Position.Z < point2.Position.Z + 0.1) && (point1.Position.Z > point2.Position.Z - 0.1));
        }

        private double calculateAngleXY(vector3 point1, vector3 point2)
        {
            double cos_sin = (point1.x * point2.x) + (point1.y * point2.y);
            double sum1 = Math.Sqrt((point1.x * point1.x) + (point1.y * point1.y));
            double sum2 = Math.Sqrt((point2.x * point2.x) + (point2.y * point2.y));
            
            cos_sin /= (sum1 * sum2);

            double angle = Math.Acos(cos_sin);

            angle *= (180 / Math.PI); //Convert to degrees

            return angle;
        }

        private bool compAngle(double alpha, double beta) 
        { 
            return ((alpha < (beta + 30))&&(alpha > (beta - 30)));
        }

        public bool fitness24(JointType point1, JointType point2) 
        { 
            bool p1 = (point1.Equals(JointType.ShoulderLeft) || (point1.Equals(JointType.ElbowLeft)) || (point1.Equals(JointType.WristLeft)) || (point1.Equals(JointType.HandLeft)));
            bool p2 = (point2.Equals(JointType.ShoulderLeft) || (point2.Equals(JointType.ElbowLeft)) || (point2.Equals(JointType.WristLeft)) || (point2.Equals(JointType.HandLeft)));

            return (p1 && p2);
        }
    }
}
