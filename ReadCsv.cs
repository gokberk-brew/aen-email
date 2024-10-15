using System.Text;

namespace AdanaEnglishNights
{
    using System.Collections.Generic;
    using System.IO;

    public static class ReadCsv
    {
        // A list to store all applicants
        public static List<Applicant> Applicants { get; private set; } = new List<Applicant>();
        public static List<string> RejectedEmails { get; private set; } = new List<string>();
        
        public static void ProcessAllApplicants(string filePath)
        {
            // Ensure the correct encoding is used for reading Turkish characters
            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                int i = 0;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    i += 1;

                    // Create a new Applicant object and populate its properties
                    var applicant = new Applicant
                    {
                        Id = i,
                        NameSurname = values[1],
                        Age = int.TryParse(values[2], out var age) ? age : 0,
                        Occupation = values[3],
                        Gender = values[4],
                        EnglishLevel = values[5],
                        EmailAddress = values[6],
                        FieldOfWork = values[7],
                        Country = values[8]
                    };

                    // Add the applicant to the list
                    Applicants.Add(applicant);
                }
            }
        }

        public static void GetRejectedEmails(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var email = reader.ReadLine()?.Trim(); // Read and trim each line
                    if (!string.IsNullOrEmpty(email)) // Ensure it's not empty
                    {
                        RejectedEmails.Add(email);
                    }
                }
            }

            // Output the number of rejected emails
            Console.WriteLine($"Number of rejected emails: {RejectedEmails.Count}");
        }
        
        
        public static bool IsBanned(Applicant applicant, List<Tuple<string, string>> bannedUsers)
        {
            // Check if the applicant is in the banned list, with culture-aware comparison
            return bannedUsers.Any(b => 
                b.Item1.Trim().Equals(applicant.NameSurname.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                b.Item2.Trim().Equals(applicant.EmailAddress.Trim(), StringComparison.CurrentCultureIgnoreCase));
        }
        
    }
}