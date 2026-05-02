// #include <iostream>
// using namespace std;

// // Антиимпликация: x и не y (x ↛ y)
// bool antiImpl(bool x, bool y) {
//     return x && !y;
// }

// // Антирепликация: не x и y (обратная антиимпликация, y ↛ x)
// bool antiRepl(bool x, bool y) {
//     return !x && y;
// }

// // Штрих Шефера | (NAND): не (x и y)
// bool nand(bool x, bool y) {
//     return !(x && y);
// }

// // Стрелка Пирса ↓ (NOR): не (x или y)
// bool nor(bool x, bool y) {
//     return !(x || y);
// }

// int over(int a, int b) {
//     if ((a + b) < 2)  {
//         return a + b;
//     } else {
//         return 0;
//     }
// }

// void print(int a, int b){
//     a = 3;
//     cout << "| x | y | z | 1 | 2 | 3 | 4 | 5 | 6 | 7 |" << endl;
//     cout << "|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|" << endl;
//     for (int x = 0; x < 2; ++x) {
//         for (int y = 0; y < 2; ++y) {
//             for (int z = 0; z < 2; ++z) {
//                 cout << "| " << x << " | " << y << " | " << z << " | "
//                     << !(y) << " | "
//                     << nand(x, !(y)) << " | "
//                     << (y || z) << " | "                
//                     << over(x, z) << " | "
//                     << (over(x, z) >= (y || z)) << " | "   
//                     << nand(y, (over(x, z) >= (y || z))) << " | "
//                     << (!(x && !y) <= !(y || (over(x, z) >= (y || z)))) << " | " << endl;
//             }
//         }
//     }

// }

// int main()
// {
//     int a = 1;
//     int b = 1;
//     print(a);
// }


// // Ф3
// #include <iostream>
// using namespace std;

// // Антиимпликация: x и не y (x ↛ y)
// bool antiImpl(bool x, bool y) {
//     return x && !y;
// }

// // Антирепликация: не x и y (обратная антиимпликация, y ↛ x)
// bool antiRepl(bool x, bool y) {
//     return !x && y;
// }

// // Штрих Шефера | (NAND): не (x и y)
// bool nand(bool x, bool y) {
//     return !(x && y);
// }

// // Стрелка Пирса ↓ (NOR): не (x или y)
// bool nor(bool x, bool y) {
//     return !(x || y);
// }

// int over(int a, int b) {
//     if ((a + b) < 2)  {
//         return a + b;
//     } else {
//         return 0;
//     }
// }

// void print(int a, int b){
//     a = 3;
//     cout << "| x | y | z | 1 | 2 | 3 | 4 | 5 | " << endl;
//     cout << "|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|" << endl;
//     for (int x = 0; x < 2; ++x) {
//         for (int y = 0; y < 2; ++y) {
//             for (int z = 0; z < 2; ++z) {
//                 cout << "| " << x << " | " << y << " | " << z << " | "
//                     << (over(!(y), (z && x))) << " | " // 1
//                     << (z || y) << " | " // 2
//                     << !((z || y) == x) << " | " //3                 
//                     // << !(z || y) << " | "                  
//                     << antiImpl( !((z || y) == x), y) << " | " 
//                     // << antiRepl(!(x), y) << " | "
//                     // << (!(z || y) != (antiRepl(!(x), y))) << " | "
//                     << nand(antiImpl( (!((z || y) == x)), y), (over(!(y), (z && x)))) << " | " << endl;
//                     // << nand( (!(z || y) != (antiRepl(!(x), y))), (over(!(y), (z && x)) )) << " | " << endl;
//             }
//         }
//     }

// }

// int main()
// {
//     int a = 1;
//     int b = 1;
//     print(a, b);
// }

// Ф3
// #include <iostream>
// using namespace std;

// // Антиимпликация: x и не y (x ↛ y)
// bool antiImpl(bool x, bool y) {
//     return x && !y;
// }

// // Антирепликация: не x и y (обратная антиимпликация, y ↛ x)
// bool antiRepl(bool x, bool y) {
//     return !x && y;
// }

// // Штрих Шефера | (NAND): не (x и y)
// bool nand(bool x, bool y) {
//     return !(x && y);
// }

// // Стрелка Пирса ↓ (NOR): не (x или y)
// bool nor(bool x, bool y) {
//     return !(x || y);
// }

// int over(int a, int b) {
//     if ((a + b) < 2)  {
//         return a + b;
//     } else {
//         return 0;
//     }
// }

// void print(int a, int b){
//     a = 3;
//     cout << "| x | y | z | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | " << endl;
//     cout << "|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|" << endl;
//     for (int x = 0; x < 2; ++x) {
//         for (int y = 0; y < 2; ++y) {
//             for (int z = 0; z < 2; ++z) {
//                 cout << "| " << x << " | " << y << " | " << z << " | "
//                     << (z || y) << " | " // 1
//                     << ((z || y) == x) << " | " // 2
//                     << !((z || y) == x) << " | " // 3                                 
//                     << antiImpl(!((z || y) == x), y) << " | " // 4
//                     << !(y) << " | " // 5
//                     << (z && x) << " | " // 6
//                     << over(!(y), (z && x)) << " | " // 7
//                     << nand(antiImpl(!((z || y) == x), y), over(!(y), (z && x))) << " | " << endl; // 8
//             }
//         }
//     }

// }

// int main()
// {
//     int a = 1;
//     int b = 1;
//     print(a, b);
// }


// //Ф4
// #include <iostream>
// using namespace std;

// // Антиимпликация: x и не y (x ↛ y)
// bool antiImpl(bool x, bool y) {
//     return x && !y;
// }

// // Антирепликация: не x и y (обратная антиимпликация, y ↛ x)
// bool antiRepl(bool x, bool y) {
//     return !x && y;
// }

// // Штрих Шефера | (NAND): не (x и y)
// bool nand(bool x, bool y) {
//     return !(x && y);
// }

// // Стрелка Пирса ↓ (NOR): не (x или y)
// bool nor(bool x, bool y) {
//     return !(x || y);
// }

// int over(int a, int b) {
//     if ((a + b) < 2)  {
//         return a + b;
//     } else {
//         return 0;
//     }
// }

// void print(int a, int b){
//     a = 3;
//     cout << "| x | y | z | 1 | 2 | 3 | 4 | 5 | 6 |" << endl;
//     cout << "|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|" << endl;
//     for (int x = 0; x < 2; ++x) {
//         for (int y = 0; y < 2; ++y) {
//             for (int z = 0; z < 2; ++z) {
//                 cout << "| " << x << " | " << y << " | " << z << " | "
//                     // <<  << " | "
//                     << antiRepl(x, y) << " | " // 1
//                     <<  !(z) << " | "          // 2  
//                     << (y == !(z)) << " | "    // 3
//                     << nor(antiImpl(x, y), (y == !(z))) << " | "  // 4 
//                     << (x <= (y && z)) << " | " // 5
//                     << (nor(antiRepl(x, y), (y == !(z))) && (x <= (y && z))) << " | " << endl; // 6
//             }
//         }
//     }
// }

// int main()
// {
//     int a = 1;
//     int b = 1;
//     print(a, b);
// }


// #include <iostream>
// using namespace std;

// // Антиимпликация: x и не y (x ↛ y)
// bool antiImpl(bool x, bool y) {
//     return x && !y;
// }

// // Антирепликация: не x и y (обратная антиимпликация, y ↛ x)
// bool antiRepl(bool x, bool y) {
//     return !x && y;
// }

// // Штрих Шефера | (NAND): не (x и y)
// bool nand(bool x, bool y) {
//     return !(x && y);
// }

// // Стрелка Пирса ↓ (NOR): не (x или y)
// bool nor(bool x, bool y) {
//     return !(x || y);
// }

// int over(int a, int b) {
//     if ((a + b) < 2)  {
//         return a + b;
//     } else {
//         return 0;
//     }
// }

// void print(int a, int b){
//     a = 3;
//     cout << "| x | y | z | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10| 11| 12|" << endl;
//     cout << "|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|" << endl;
//     for (int x = 0; x < 2; ++x) {
//         for (int y = 0; y < 2; ++y) {
//             for (int z = 0; z < 2; ++z) {
//                 cout << "| " << x << " | " << y << " | " << z << " | "
//                     // <<  << " | "
//                     << !(z) << " | " // 1 
//                     << nor(!(z), z) << " | " // 2  p
//                     << antiRepl(z, x) << " | " // 3 g
//                     << ((antiRepl(z, x)) == (nor(!(z), z))) << " | " // 4 e
//                     << (x >= y) << " | " // 5 L
//                     << (x <= (x >= y)) << " | " // 6 r
//                     << !(x <= (x >= y)) << " | " // 7 r + n
//                     << over(z, y) << " | " // 8 m 
//                     << antiImpl(!(x <= (x >= y)), over(z, y)) << " | " // 9 f
//                     << nand(x, y) << " | " // 10 s
//                     << (antiImpl(!(x <= (x >= y)), over(z, y)) && nand(x, y)) << " | " // 11 c
//                     << ((antiImpl(!(x <= (x >= y)), over(z, y)) && nand(x, y)) || ((antiRepl(z, x)) == (nor(!(z), z)))) << " | " << endl; // 12 d
//             }
//         }
//     }

// }

// int main()
// {
//     int a = 1;
//     int b = 1;
//     print(a, b);
// }

//№5
// #include <iostream>
// using namespace std;

// // Антиимпликация: x и не y (x ↛ y)
// bool antiImpl(bool x, bool y) {
//     return x && !y;
// }

// // Антирепликация: не x и y (обратная антиимпликация, y ↛ x)
// bool antiRepl(bool x, bool y) {
//     return !x && y;
// }

// // Штрих Шефера | (NAND): не (x и y)
// bool nand(bool x, bool y) {
//     return !(x && y);
// }

// // Стрелка Пирса ↓ (NOR): не (x или y)
// bool nor(bool x, bool y) {
//     return !(x || y);
// }

// int over(int a, int b) {
//     if ((a + b) < 2)  {
//         return a + b;
//     } else {
//         return 0;
//     }
// }

// void print(int a, int b){
//     a = 3;
//     cout << "| x | y | z | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10| 11|" << endl;
//     cout << "|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|" << endl;
//     for (int x = 0; x < 2; ++x) {
//         for (int y = 0; y < 2; ++y) {
//             for (int z = 0; z < 2; ++z) {
//                 cout << "| " << x << " | " << y << " | " << z << " | "
//                     << (x || z) << " | " // 1 d
//                     << (x >= y) << " | " // 2 I
//                     << nand(z, (x >= y)) << " | " // 3 s
//                     << !(antiImpl(y, x)) << " | " // 4 f + n
//                     << (z <= !(antiImpl(y, x))) << " | " // 5 r
//                     << (z == x) << " | " // 6 e
//                     << over((z <= !(antiImpl(y, x))), (z == x)) << " | " // 7 m
//                     << (x && over((z <= !(antiImpl(y, x))), (z == x))) << " | " // 8 c
//                     << !(x && over((z <= !(antiImpl(y, x))), (z == x))) << " | " // 9 n + c
//                     << antiRepl(nand(z, (x >= y)), !(x && over((z <= !(antiImpl(y, x))), (z == x)))) << " | " // 10 g
//                     << nor((x || z), antiRepl(nand(z, (x >= y)), !(x && over((z <= !(antiImpl(y, x))), (z == x))))) << " | " << endl; // 11 p
//             }
//         }
//     }
// }

// int main()
// {
//     int a = 1;
//     int b = 1;
//     print(a, b);
// }

//Лабораторная 2
//№1
// #include <iostream>
// using namespace std;

// // Антиимпликация: x и не y (x ↛ y)
// bool antiImpl(bool x, bool y) {
//     return x && !y;
// }

// // Антирепликация: не x и y (обратная антиимпликация, y ↛ x)
// bool antiRepl(bool x, bool y) {
//     return !x && y;
// }

// // Штрих Шефера | (NAND): не (x и y)
// bool nand(bool x, bool y) {
//     return !(x && y);
// }

// // Стрелка Пирса ↓ (NOR): не (x или y)
// bool nor(bool x, bool y) {
//     return !(x || y);
// }

// int over(int a, int b) {
//     if ((a + b) < 2)  {
//         return a + b;
//     } else {
//         return 0;
//     }
// }

// void print(int a, int b){
//     a = 3;
//     cout << "| x1 | x2 | x3 | 1 | 2 | 3 | 4 | 5 |" << endl;
//     cout << "|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|" << endl;
//     for (int x = 0; x < 2; ++x) {
//         for (int y = 0; y < 2; ++y) {
//             for (int z = 0; z < 2; ++z) {
//                 cout << "| " << x << " | " << y << " | " << z << " | "
//                     << over(z, !(y)) << " | " // 1
//                     << (x <= over(z, !(y))) << " | " // 2
//                     << nand(!(x), z) << " | " // 3                                 
//                     << nor(y, nand(!(x), z)) << " | " // 4
//                     << ((x <= over(z, !(y))) == nor(y, nand(!(x), z))) << " | " << endl; // 5
//             }
//         }
//     }

// }

// int main()
// {
//     int a = 1;
//     int b = 1;
//     print(a, b);
// }


//№2

// #include <iostream>
// using namespace std;

// // Антиимпликация: x и не y (x ↛ y)
// bool antiImpl(bool x, bool y) {
//     return x && !y;
// }

// // Антирепликация: не x и y (обратная антиимпликация, y ↛ x)
// bool antiRepl(bool x, bool y) {
//     return !x && y;
// }

// // Штрих Шефера | (NAND): не (x и y)
// bool nand(bool x, bool y) {
//     return !(x && y);
// }

// // Стрелка Пирса ↓ (NOR): не (x или y)
// bool nor(bool x, bool y) {
//     return !(x || y);
// }

// int over(int a, int b) {
//     if ((a + b) < 2)  {
//         return a + b;
//     } else {
//         return 0;
//     }
// }

// void print(int a, int b){
//     a = 3;
//     cout << "| x1 | x2 | x3 | 1 | 2 | 3 | 4 |" << endl;
//     cout << "|:-:|:-:|:-:|:-:|:-:|:-:|:-:|" << endl;
//     for (int x = 0; x < 2; ++x) {
//         for (int y = 0; y < 2; ++y) {
//             for (int z = 0; z < 2; ++z) {
//                 cout << "| " << x << " | " << y << " | " << z << " | "
//                     << over(x, y) << " | " // 1
//                     << antiImpl(x, z) << " | " // 2
//                     << over(y, antiImpl(x, z)) << " | " // 3                                 
//                     << nand(over(x, y), over(y, antiImpl(x, z))) << " | " << endl; // 4
//             }
//         }
//     }

// }

// int main()
// {
//     int a = 1;
//     int b = 1;
//     print(a, b);
// }


// //№3
// #include <iostream>
// using namespace std;

// // Антиимпликация: x и не y (x ↛ y)
// bool antiImpl(bool x, bool y) {
//     return x && !y;
// }

// // Антирепликация: не x и y (обратная антиимпликация, y ↛ x)
// bool antiRepl(bool x, bool y) {
//     return !x && y;
// }

// // Штрих Шефера | (NAND): не (x и y)
// bool nand(bool x, bool y) {
//     return !(x && y);
// }

// // Стрелка Пирса ↓ (NOR): не (x или y)
// bool nor(bool x, bool y) {
//     return !(x || y);
// }

// int over(int a, int b) {
//     if ((a + b) < 2)  {
//         return a + b;
//     } else {
//         return 0;
//     }
// }

// void print(int a, int b){
//     a = 3;
//     cout << "| x1 | x2 | x3 | 1 | 2 | 3 | 4 |" << endl;
//     cout << "|:-:|:-:|:-:|:-:|:-:|:-:|:-:|" << endl;
//     for (int x = 0; x < 2; ++x) {
//         for (int y = 0; y < 2; ++y) {
//             for (int z = 0; z < 2; ++z) {
//                 cout << "| " << x << " | " << y << " | " << z << " | "
//                     << (z == x) << " | " // 1
//                     << (y <= (z == x)) << " | " // 2
//                     << ((y <= (z == x)) && x) << " | " // 3                                 
//                     << !((y <= (z == x)) && x) << " | " << endl; // 4
//             }
//         }
//     }

// }

// int main()
// {
//     int a = 1;
//     int b = 1;
//     print(a, b);
// }


// #include <iostream>
// using namespace std;

// // Антиимпликация: x и не y (x ↛ y)
// bool antiImpl(bool x, bool y) {
//     return x && !y;
// }

// // Антирепликация: не x и y (обратная антиимпликация, y ↛ x)
// bool antiRepl(bool x, bool y) {
//     return !x && y;
// }

// // Штрих Шефера | (NAND): не (x и y)
// bool nand(bool x, bool y) {
//     return !(x && y);
// }

// // Стрелка Пирса ↓ (NOR): не (x или y)
// bool nor(bool x, bool y) {
//     return !(x || y);
// }

// int over(int a, int b) {
//     if ((a + b) < 2)  {
//         return a + b;
//     } else {
//         return 0;
//     }
// }

// void print(int a, int b){
//     a = 3;
//     cout << "| x1 | x2 | x3 | 1 | 2 | 3 | 4 |" << endl;
//     cout << "|:-:|:-:|:-:|:-:|:-:|:-:|:-:|" << endl;
//     for (int x = 0; x < 2; ++x) {
//         for (int y = 0; y < 2; ++y) {
//             for (int z = 0; z < 2; ++z) {
//                 cout << "| " << x << " | " << y << " | " << z << " | "
//                     << (x || !(y) || z) << " | " // 1
//                     << (x || !(y) || !(z)) << " | " // 2
//                     << (!(x) || y || z) << " | " // 3                                 
//                     << over(nand(nor(z, y),x),(x && (y >= z))) << " | " << endl; // 4
//             }
//         }
//     }

// }

// int main()
// {
//     int a = 1;
//     int b = 1;
//     print(a, b);
// }


// #include <iostream>
// using namespace std;

// // Антиимпликация: x и не y (x ↛ y)
// bool antiImpl(bool x, bool y) {
//     return x && !y;
// }

// // Антирепликация: не x и y (обратная антиимпликация, y ↛ x)
// bool antiRepl(bool x, bool y) {
//     return !x && y;
// }

// // Штрих Шефера | (NAND): не (x и y)
// bool nand(bool x, bool y) {
//     return !(x && y);
// }

// // Стрелка Пирса ↓ (NOR): не (x или y)
// bool nor(bool x, bool y) {
//     return !(x || y);
// }

// int over(int a, int b) {
//     if ((a + b) < 2)  {
//         return a + b;
//     } else {
//         return 0;
//     }
// }

// void print(int a, int b){
//     a = 3;
//     cout << "| x1 | x2 | x3 | 1 | 2 | 3 | 4 |" << endl;
//     cout << "|:-:|:-:|:-:|:-:|:-:|:-:|:-:|" << endl;
//     for (int x = 0; x < 2; ++x) {
//         for (int y = 0; y < 2; ++y) {
//             for (int z = 0; z < 2; ++z) {
//                 cout << "| " << x << " | " << y << " | " << z << " | "
//                     << (!(x)&& y) << " | " // 1
//                     << (!(x)&& z) << " | " // 2
//                     << (y && z) << " | " // 3                                 
//                     << ((!(x)&& y) || (!(x)&& z) || (y && z)) << " | " << endl; // 4
//             }
//         }
//     }

// }

// int main()
// {
//     int a = 1;
//     int b = 1;
//     print(a, b);
// }




// #include <iostream>
// using namespace std;

// // Антиимпликация: x и не y (x ↛ y)
// bool antiImpl(bool x, bool y) {
//     return x && !y;
// }

// // Антирепликация: не x и y (обратная антиимпликация, y ↛ x)
// bool antiRepl(bool x, bool y) {
//     return !x && y;
// }

// // Штрих Шефера | (NAND): не (x и y)
// bool nand(bool x, bool y) {
//     return !(x && y);
// }

// // Стрелка Пирса ↓ (NOR): не (x или y)
// bool nor(bool x, bool y) {
//     return !(x || y);
// }

// int over(int a, int b) {
//     if ((a + b) < 2)  {
//         return a + b;
//     } else {
//         return 0;
//     }
// }

// void print(int a, int b){
//     a = 3;
//     cout << "| x1 | x2 | x3 | 1 | 2 | 3 | 4 | 5 |" << endl;
//     cout << "|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|" << endl;
//     for (int x = 0; x < 2; ++x) {
//         for (int y = 0; y < 2; ++y) {
//             for (int z = 0; z < 2; ++z) {
//                 bool T1 = antiImpl(antiImpl(1, x), antiImpl(1, y));
//                 bool T2 = antiImpl(antiImpl(1, x), antiImpl(1, z));
//                 bool T3 = antiImpl(y, antiImpl(1, z));
//                 cout << "| " << x << " | " << y << " | " << z << " | "
//                     << antiImpl(1, T1) << " | " // 1
//                     << antiImpl(antiImpl(1, T1), T2) << " | " // 2
//                     << antiImpl(1, antiImpl(antiImpl(1, T1), T2)) << " | " // 3                                 
//                     << antiImpl(1, antiImpl(1, antiImpl(antiImpl(1, T1), T2))) << " | "  // 4
//                     << antiImpl(antiImpl(1, antiImpl(1, antiImpl(antiImpl(1, T1), T2))), T3) << " | "  // 5
//                     << antiImpl(1, antiImpl(antiImpl(1, antiImpl(1, antiImpl(antiImpl(1, T1), T2))), T3)) << " | " << endl; // 6
//             }
//         }
//     }

// }

// int main()
// {
//     int a = 1;
//     int b = 1;
//     print(a, b);
// }

#include <iostream>
using namespace std;

// Антиимпликация: x и не y (x ↛ y)
bool antiImpl(bool x, bool y) {
    return x && !y;
}

// Антирепликация: не x и y (обратная антиимпликация, y ↛ x)
bool antiRepl(bool x, bool y) {
    return !x && y;
}

// Штрих Шефера | (NAND): не (x и y)
bool nand(bool x, bool y) {
    return !(x && y);
}

// Стрелка Пирса ↓ (NOR): не (x или y)
bool nor(bool x, bool y) {
    return !(x || y);
}

int over(int a, int b) {
    if ((a + b) < 2)  {
        return a + b;
    } else {
        return 0;
    }
}

bool Const(int a){
    return (a <= a);
}

bool con(int a, int b){
    return over((a <= over(b, Const(a))),Const(a));
}

void print(int a, int b){
    a = 3;
    cout << "|x1 |x2 |x3 | 1 | 2 | 3 | 4 | 5 | 6 | 7 |" << endl;
    cout << "|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|:-:|" << endl;
    for (int x = 0; x < 2; ++x) {
        for (int y = 0; y < 2; ++y) {
            for (int z = 0; z < 2; ++z) {
                bool T1 = antiImpl(antiImpl(1, x), antiImpl(1, y));
                bool T2 = antiImpl(antiImpl(1, x), antiImpl(1, z));
                bool T3 = antiImpl(y, antiImpl(1, z));
                cout << "| " << x << " | " << y << " | " << z << " | "
                    << over(y, z) << " | " // 1
                    << con(y, z) << " | " // 2
                    << con(x, z) << " | " // 3                                 
                    << over(con(y, z), con(x, z)) << " | "  // 4
                    << con(x, y) << " | "  // 5
                    << over(over(con(y, z), con(x, z)), con(x, y)) << " | "  // 6
                    << over(over(y, z), over(over(con(y, z), con(x, z)), con(x, y))) << " | " << endl; // 7
            }
        }
    } 

}

int main()
{
    int a = 1;
    int b = 1;
    print(a, b);
}
