using System.Security.Cryptography;
using System.Text;

string fio = "Сеитбкиров,Руслан,Эмилевич";   // фамилия, имя, отчество через запятые

// 3. Строка -> байты (UTF-8, иначе кириллица закодируется неверно)
byte[] data = Encoding.UTF8.GetBytes(fio);

// 4. Хэш в виде массива байтов
byte[] md5 = MD5.HashData(data);
byte[] sha256 = SHA256.HashData(data);

// 5. Массив байтов -> строка из шестнадцатеричных символов
Console.WriteLine("MD5:    " + Convert.ToHexString(md5));
Console.WriteLine("SHA256: " + Convert.ToHexString(sha256));