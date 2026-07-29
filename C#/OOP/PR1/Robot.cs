using System;

namespace project
{
    class Robot
    {
        private string name = "";
        private int weight;
        private byte[] coordinates = Array.Empty<byte>();

        public void setValues(string _name, int _weight, byte[] _coordinates)
        {
            name = _name;
            weight = _weight;
            coordinates = _coordinates;
        }

        public void printValues()
        {
            System.Console.Write(name + " weight " + weight + ". Coordinates: ");
            foreach(byte el in coordinates) System.Console.Write(el + " ");
        }

    }
}