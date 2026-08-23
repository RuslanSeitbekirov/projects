using System;

namespace project
{
    

    class Killer : Robot
    {
        
        public int Health
        {
            get;
            set;
        }

        public Killer() {}

        public Killer(string name, int weight, byte[] coordinates, int health) : base(name, weight, coordinates)
        {
            this.Health = health;
            
        }

        public override void printValues()
        {
            System.Console.WriteLine(this.Name + " weight " + this.Weight + ". Coordinates: ");
            foreach(byte el in this.Coordinates) System.Console.WriteLine(el + " ");
            System.Console.WriteLine("Health: " + this.Health);
        }
        

        public void Lazer()
        {
            System.Console.WriteLine("Lazer shoting");
            this.surname = "Doe";
        }
    }
}