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
            base.printValues();
        }
        

        public void Lazer()
        {
            System.Console.WriteLine("Lazer shoting");
            this.surname = "Doe";
        }
    }
}