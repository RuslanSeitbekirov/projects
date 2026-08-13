using System;
using System.Dynamic;

namespace project
{
    class Robot
    {
        private static int count;
        private string name = "";
        private int weight;
        private byte[] coordinates = Array.Empty<byte>();

        protected string surname;

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

        public void printValues()
        {
            System.Console.WriteLine(this.name + " weight " + this.weight + ". Coordinates: ");
            foreach(byte el in coordinates) System.Console.WriteLine(el + " ");
        }

        public static void Print()
        {
            System.Console.WriteLine("Count: {0}", count);
        }

    }
}