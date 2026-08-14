// hash_generator.cs — C# версия

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class HashGenerator {
    static void Main(string[] args) {
        string format = "hex";
        string filePath = null;
        string output = null;
        string compare = null;
        string input = null;

        for (int i = 0; i < args.Length; i++) {
            switch (args[i]) {
                case "--format":
                case "-f":
                    format = args[++i];
                    break;
                case "--file":
                case "-F":
                    filePath = args[++i];
                    break;
                case "--output":
                case "-o":
                    output = args[++i];
                    break;
                case "--compare":
                case "-c":
                    compare = args[++i];
                    break;
                default:
                    if (!args[i].StartsWith("-")) {
                        input = args[i];
                    }
                    break;
            }
        }

        Console.WriteLine("\u001B[36m🔐 Hash Generator (MD5) (C#)\u001B[0m");
        Console.WriteLine("Алгоритм: MD5");

        string hexDigest = "";
        if (filePath != null) {
            Console.WriteLine("📂 Хеширование файла: " + filePath);
            hexDigest = HashFile(filePath);
        } else if (input != null) {
            Console.WriteLine("📝 Входные данные: " + input);
            hexDigest = ComputeHash(Encoding.UTF8.GetBytes(input));
        } else {
            Console.WriteLine("📝 Чтение из STDIN (Ctrl+D для окончания)");
            string line;
            var sb = new StringBuilder();
            while ((line = Console.ReadLine()) != null) {
                sb.AppendLine(line);
            }
            string data = sb.ToString();
            if (string.IsNullOrEmpty(data)) {
                Console.WriteLine("\u001B[33m⚠️ Пустой ввод.\u001B[0m");
                Environment.Exit(1);
            }
            hexDigest = ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        string result;
        if (format == "base64") {
            byte[] bytes = HexStringToByteArray(hexDigest);
            result = Convert.ToBase64String(bytes);
        } else {
            result = hexDigest;
        }

        Console.WriteLine($"\u001B[32mХеш ({format}):\u001B[0m");
        Console.WriteLine(result);

        if (compare != null) {
            if (result == compare) {
                Console.WriteLine("\u001B[32m✅ Хеши совпадают!\u001B[0m");
            } else {
                Console.WriteLine("\u001B[31m❌ Хеши не совпадают!\u001B[0m");
            }
        }

        if (output != null) {
            try {
                File.WriteAllText(output, result + "\n");
                Console.WriteLine($"\u001B[32m💾 Сохранено в {output}\u001B[0m");
            } catch (Exception e) {
                Console.WriteLine($"\u001B[31m❌ Ошибка сохранения: {e.Message}\u001B[0m");
            }
        }
    }

    static string ComputeHash(byte[] data) {
        using (MD5 md5 = MD5.Create()) {
            byte[] digest = md5.ComputeHash(data);
            return BitConverter.ToString(digest).Replace("-", "").ToLower();
        }
    }

    static string HashFile(string filename) {
        using (var stream = File.OpenRead(filename)) {
            using (MD5 md5 = MD5.Create()) {
                long total = stream.Length;
                long processed = 0;
                byte[] buffer = new byte[8192];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0) {
                    md5.TransformBlock(buffer, 0, bytesRead, null, 0);
                    processed += bytesRead;
                    if (total > 1024 * 1024) {
                        double percent = (double)processed / total * 100;
                        Console.Error.Write($"\r⏳ Прогресс: {percent:F1}%");
                    }
                }
                md5.TransformFinalBlock(new byte[0], 0, 0);
                if (total > 1024 * 1024) Console.Error.WriteLine();
                byte[] digest = md5.Hash;
                return BitConverter.ToString(digest).Replace("-", "").ToLower();
            }
        }
    }

    static byte[] HexStringToByteArray(string hex) {
        int len = hex.Length;
        byte[] data = new byte[len / 2];
        for (int i = 0; i < len; i += 2) {
            data[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return data;
    }
}
