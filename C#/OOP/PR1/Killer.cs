using System;
using System.Xml;

namespace project
{
    
    enum Type { Enemy, Hero, Traitor}

    class Killer : Robot
    {
        
        public int Health
        {
            get;
            set;
        }

        public Type type;


        public Killer() {}

        public Killer(string name, int weight, byte[] coordinates, int health, Type type) : base(name, weight, coordinates)
        {
            this.Health = health;
            this.type = type;
            
        }

        public override void printValues()
        {
            System.Console.WriteLine(this.Name + " weight " + this.Weight + ". Coordinates: ");
            foreach(byte el in this.Coordinates) System.Console.WriteLine(el + " ");
            System.Console.WriteLine("Health: " + this.Health);

            if(this.type == Type.Hero)
            {
                System.Console.WriteLine("His hero!");
            }
        }
        

        public void Lazer()
        {
            System.Console.WriteLine("Lazer shoting");
            this.surname = "Doe";
        }
    }
}