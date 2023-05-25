using System;
using System.Collections.Generic;
using System.IO;

namespace PhoneBook
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "persons.txt");
            var records = ReadRecordsFromFile(filePath);

            if (records.Count == 0)
            {
                Console.WriteLine("No records found in the file.");
                return;
            }

            Console.WriteLine("Please choose an ordering to sort: 'Ascending' or 'Descending'.");
            string ordering = Console.ReadLine();

            Console.WriteLine("Please choose criteria: 'Name', 'Surname' or 'PhoneNumberCode'.");
            string criteria = Console.ReadLine();

            SortAndShowRecords(records, ordering, criteria);
            ShowValidationMessages(records);

            Console.ReadLine();
        }

        static List<string> ReadRecordsFromFile(string filePath)
        {
            var records = new List<string>();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        records.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading the file: " + e.Message);
            }

            return records;
        }

        static void SortAndShowRecords(List<string> records, string ordering, string criteria)
        {
            Comparison<string> comparison;

            switch (criteria)
            {
                case "Name":
                    comparison = CompareByName;
                    break;
                case "Surname":
                    comparison = CompareBySurname;
                    break;
                case "PhoneNumberCode":
                    comparison = CompareByPhoneNumberCode;
                    break;
                default:
                    Console.WriteLine("Invalid criteria selected.");
                    return;
            }

            if (ordering.ToLower() == "descending")
            {
                Comparison<string> reversedComparison = (x, y) => comparison(y, x);
                records.Sort(reversedComparison);
            }

            records.Sort(comparison);

            Console.WriteLine("Sorted Records:");
            foreach (string record in records)
            {
                Console.WriteLine(record);
            }
        }

        static int CompareByName(string x, string y)
        {
            string[] partsX = x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] partsY = y.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return String.Compare(partsX[0], partsY[0], StringComparison.OrdinalIgnoreCase);
        }

        static int CompareBySurname(string x, string y)
        {
            string[] partsX = x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] partsY = y.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string surnameX = partsX.Length > 1 ? partsX[1] : "";
            string surnameY = partsY.Length > 1 ? partsY[1] : "";

            return String.Compare(surnameX, surnameY, StringComparison.OrdinalIgnoreCase);
        }

        static int CompareByPhoneNumberCode(string x, string y)
        {
            string[] partsX = x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] partsY = y.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string phoneNumberX = partsX.Length > 2 ? partsX[3] : "";
            string phoneNumberY = partsY.Length > 2 ? partsY[3] : "";

            return String.Compare(phoneNumberX.Substring(0, 3), phoneNumberY.Substring(0, 3), StringComparison.OrdinalIgnoreCase);
        }

        static void ShowValidationMessages(List<string> records)
        {
            Console.WriteLine("Validations:");

            for (int i = 0; i < records.Count; i++)
            {
                string record = records[i];
                var parts = record.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 3)
                {
                    Console.WriteLine($"Line {i + 1}: Incomplete record.");
                    continue;
                }

                string phoneNumber = parts[3];
                string surname = parts.Length > 1 ? parts[1] : "";

                var validationMessages = new List<string>();

                if (phoneNumber.Length != 9)
                {
                    validationMessages.Add("Phone number should be 9 digits.");

                    if (parts[2] != ":" && parts[2] != "-")
                    {
                        validationMessages.Add("Separator should be ':' or '-'.");
                    }
                }

                if (surname == "" && i < records.Count - 1)
                {
                    validationMessages.Add("Empty surname should appear in the last rows.");
                }

                if (validationMessages.Count > 0)
                {
                    Console.WriteLine($"Line {i + 1}: {string.Join(" ", validationMessages)}");
                }
            }
        }
    }
}