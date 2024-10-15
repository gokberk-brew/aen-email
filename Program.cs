using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdanaEnglishNights;

string applicantsFilePath = @"C:\Users\gokbe\OneDrive\Belgeler\Repos\AdanaEnglishNights\CSV\16.10.24.csv";
string rejectedFilePath = @"C:\Users\gokbe\OneDrive\Belgeler\Repos\AdanaEnglishNights\CSV\9.10.24-Rejected.csv";

List<Tuple<string, string>> bannedUsers = new List<Tuple<string, string>>()
{
    new("Faris HOCAOĞLU","faris.hocaoglu@hotmail.com"),
    new("Murat Uygun ","muratuygun1998@gmail.com"),
};

List<Applicant> resultApplicants = new List<Applicant>();

ReadCsv.ProcessAllApplicants(applicantsFilePath);
ReadCsv.GetRejectedEmails(rejectedFilePath);

// The desired number of applicants
int desiredWomenCount = 55;
int desiredMenCount = 30;

// Get the list of applicants
var applicants = ReadCsv.Applicants;
var lastWeekRejectedEmails = ReadCsv.RejectedEmails;

// Ensure distinct applicants based on EmailAddress
var distinctApplicants = applicants.DistinctBy(x => x.EmailAddress).ToList();

// Add rejected applicants to the results, ensuring they are not banned
resultApplicants.AddRange(distinctApplicants.Where(x => 
    lastWeekRejectedEmails.Contains(x.EmailAddress) && 
    !ReadCsv.IsBanned(x, bannedUsers)));

// Exclude banned users
var bannedExcluded = distinctApplicants.Where(x => 
    !ReadCsv.IsBanned(x, bannedUsers)).ToList();

// Filter internationals (not from Turkey or Turks and Caicos Islands or StringEmpty)
var internationals = bannedExcluded.Where(x => 
    x.Country != "Turkey" && x.Country != "Turks and Caicos Islands" && x.Country != String.Empty).ToList();
resultApplicants.AddRange(internationals);

// Filter out internationals from the remaining applicants
var internationalsExcluded = bannedExcluded.Where(x => 
    !internationals.Contains(x)).ToList();

// Separate by gender, excluding last week's rejected ones
var femaleApplicants = internationalsExcluded
    .Where(x => 
        !x.Gender.Trim().Equals("Male", StringComparison.OrdinalIgnoreCase) && 
        !lastWeekRejectedEmails.Contains(x.EmailAddress.Trim()))
    .ToList();

var maleApplicants = internationalsExcluded
    .Where(x => 
        x.Gender.Trim().Equals("Male", StringComparison.OrdinalIgnoreCase) && 
        !lastWeekRejectedEmails.Contains(x.EmailAddress.Trim()))
    .ToList();

var resWomanCount = resultApplicants.Count(x => !x.Gender.Trim().Equals("Male", StringComparison.OrdinalIgnoreCase));
var resMenCount = resultApplicants.Count(x => x.Gender.Trim().Equals("Male", StringComparison.OrdinalIgnoreCase));

var womanCount = desiredWomenCount - resWomanCount;
var menCount = desiredMenCount - resMenCount;

// Select desired number of female applicants
for (int i = 0; i < Math.Min(womanCount, femaleApplicants.Count); i++)
{
    resultApplicants.Add(femaleApplicants[i]);
}

// Select desired number of male applicants
for (int i = 0; i < Math.Min(menCount, maleApplicants.Count); i++)
{
    resultApplicants.Add(maleApplicants[i]);
}

// List rejected applicants (those who were not selected)
List<Applicant> rejectedApplicants = distinctApplicants.Where(x => 
    !resultApplicants.Contains(x)).ToList();

var acceptedApplicants = resultApplicants.DistinctBy(x => x.EmailAddress).ToList();

// Output the count of rejected applicants
Console.WriteLine("Number of total applicants " + distinctApplicants);
Console.WriteLine("Number of rejected applicants: " + rejectedApplicants.Count);
Console.WriteLine("Number of accepted applicants: " + acceptedApplicants.Count);

string filePathAccept= @"C:\Users\gokbe\OneDrive\Belgeler\Repos\AdanaEnglishNights\CSV\16.10.24_AcceptedApplications.csv";
string filePathRej = @"C:\Users\gokbe\OneDrive\Belgeler\Repos\AdanaEnglishNights\CSV\16.10.24_RejectedApplications.csv";


// Write Accepted
var res = acceptedApplicants.OrderBy(x => x.NameSurname).ToList();
StringBuilder csvContent = new StringBuilder();
csvContent.AppendLine("Count,NameSurname,Age,Occupation,Gender,EnglishLevel,EmailAddress,FieldOfWork,Country");
int count = 1;
foreach (var applicant in res)
{
    csvContent.AppendLine($"{count},{applicant.NameSurname},{applicant.Age},{applicant.Occupation},{applicant.Gender},{applicant.EnglishLevel},{applicant.EmailAddress},{applicant.FieldOfWork},{applicant.Country}");
    count++;
}
File.WriteAllText(filePathAccept, csvContent.ToString(), Encoding.UTF8);


// Write Rejected
var rejRes = rejectedApplicants.OrderBy(x => x.NameSurname).ToList();
StringBuilder csvContent2 = new StringBuilder();
csvContent2.AppendLine("Count,NameSurname,Age,Occupation,Gender,EnglishLevel,EmailAddress,FieldOfWork,Country");
int count2 = 1;
foreach (var applicant in rejRes)
{
    csvContent2.AppendLine($"{count2},{applicant.NameSurname},{applicant.Age},{applicant.Occupation},{applicant.Gender},{applicant.EnglishLevel},{applicant.EmailAddress},{applicant.FieldOfWork},{applicant.Country}");
    count2++;
}
File.WriteAllText(filePathRej, csvContent2.ToString(), Encoding.UTF8);