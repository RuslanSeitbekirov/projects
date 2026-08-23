using System;
using System.Dynamic;

namespace project
{
    abstract class Robot : IRun, IJump
    {
        private static int count;
        private string name = "";
        private int weight;
        private byte[] coordinates = Array.Empty<byte>();

        protected string surname = "";

        public string Name { get{ return name; } private set{} }
        public byte[] Coordinates { get{ return coordinates; } private set{} }

        public int Weight
        {
            get
            {
                System.Console.WriteLine("Результат: ");
                return this.weight;
            }
            set
            {
                if (value < 1)
                {
                    this.weight = 1;    
                }
                else if (value > 5000) {
                    this.weight = 5000;
                } else
                {
                    this.weight = value;
                }

            }
        }

        public int Width{ get;  set;}
        public float speed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float y { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Robot(string name, int weight, byte[] coordinates)
        {
            System.Console.WriteLine("Object has been created");
            this.setValues(name, weight, coordinates);
            count++;
        }

        public Robot(string name)
        {
            System.Console.WriteLine("Object has been created");
            this.name = name;
            count++;
        }

        public Robot()
        {
            count++;
        }

        public void setValues(string name, int weight, byte[] coordinates)
        {
            this.name = name;
            this.weight = weight;
            this.coordinates = coordinates;
        }

        public abstract void printValues();


        public static void Print()
        {
            System.Console.WriteLine("Count is: {0}", count);
        }

        public void RobotRun()
        {
            System.Console.WriteLine("Robot is run");
        }

        public void Jump()
        {
             System.Console.WriteLine("Robot is jump");
        }
    }
}