using System;

namespace pr_1{
    

    struct Book
    {
        private string title, avtor, intro;

        private short pages;

        public void setValues(string title, string avtor)
        {
            this.title = title;
            this.avtor = avtor;
        }


        public void printValues()
        {
            System.Console.WriteLine(this.avtor + " написал книгу: «" + this.title + "»");
        }
    }
}