using System;

namespace pr_1{
    
    class DataStruct{
        
        static void Main() {
            Book idiot = new Book();
            idiot.setValues("Идиот", "Лох позорный");
            idiot.printValues();
        }
    }
}