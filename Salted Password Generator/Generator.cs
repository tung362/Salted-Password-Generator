using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.IO;

namespace Salted_Password_Generator
{
    class Generator
    {
        public static string Password = "";
        public static int Iterations = 24000;
        public static int passwordByteCount = 24;

        public static void Start()
        {
            InputPassword();
            InputIterationCount();
            InputPasswordByteCount();
            Generate();
        }

        static void InputPassword()
        {
            Console.Write("Enter a password: ");
            Password = Console.ReadLine();
        }

        static void InputIterationCount()
        {
            Console.Write("Enter the amount of iterations (Default is 24000): ");
            string inputtedIterations = Console.ReadLine();
            if (string.IsNullOrEmpty(inputtedIterations)) return;
            if (!int.TryParse(inputtedIterations, out Iterations))
            {
                Console.Write("Invalid input You need to type a number! Press any key to try again.");
                Console.ReadKey();
                InputIterationCount();
            }
            if(Iterations <= 0)
            {
                Iterations = 24000;
                Console.Write("input cannot be below 1! Press any key to try again.");
                Console.ReadKey();
                InputIterationCount();
            }
        }

        static void InputPasswordByteCount()
        {
            Console.Write("Enter the amount of bytes to use for the password (Default is 24): ");
            string inputtedPasswordByteCount = Console.ReadLine();
            if (string.IsNullOrEmpty(inputtedPasswordByteCount)) return;
            if (!int.TryParse(inputtedPasswordByteCount, out passwordByteCount) && inputtedPasswordByteCount != "")
            {
                Console.Write("Invalid input You need to type a number! Press any key to try again.");
                Console.ReadKey();
                InputPasswordByteCount();
            }
            if (passwordByteCount <= 0)
            {
                passwordByteCount = 24;
                Console.Write("input cannot be below 1! Press any key to try again.");
                Console.ReadKey();
                InputPasswordByteCount();
            }
        }

        static void Generate()
        {
            //Generate randomized salt bytes
            byte[] generatedRawSalt = new byte[8];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(generatedRawSalt);
            }

            //Convert salt bytes to Base64 string
            string generatedSalt = Convert.ToBase64String(generatedRawSalt);

            //Generate password bytes
            byte[] generatedRawPassword;
            using (var pbkdf2 = new Rfc2898DeriveBytes(Password, Convert.FromBase64String(generatedSalt), Iterations))
                generatedRawPassword = pbkdf2.GetBytes(passwordByteCount);

            //Convert password bytes to Base64 string
            string generatedPassword = Convert.ToBase64String(generatedRawPassword);

            List<string> fileOutputs = new List<string>();
            fileOutputs.Add("Raw Salt: " + BitConverter.ToString(generatedRawSalt));
            fileOutputs.Add("Raw Password: " + BitConverter.ToString(generatedRawPassword));
            fileOutputs.Add("Salt: " + generatedSalt);
            fileOutputs.Add("Password: " + generatedPassword);
            File.WriteAllLines(Application.StartupPath + "/SaltedPassword.txt", fileOutputs);

            for (int i = 0; i < fileOutputs.Count; i++) Console.WriteLine(fileOutputs[i]);
            Console.WriteLine("Results saved to SaltedPassword.txt");
            Console.ReadKey();
        }
    }
}
