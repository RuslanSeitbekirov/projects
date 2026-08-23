using System;

namespace project
{
    

    class Bot : Robot, IRun
    {
        

        public Bot() {}

        public Bot(string name, int weight, byte[] coordinates) : base(name, weight, coordinates)
        {

            
        }

        public override void printValues()
        {
            System.Console.WriteLine(this.Name + " weight " + this.Weight + ". Coordinates: ");
            foreach(byte el in this.Coordinates) System.Console.WriteLine(el + " ");
        }
    }
}