using System;
using System.Collections.Generic;
using System.Linq;
using AdanaEnglishNights;

string applicantsFilePath = @"C:\Users\gokbe\OneDrive\Belgeler\AEN\TestCsv.csv";
string rejectedFilePath = @"C:\Users\gokbe\OneDrive\Belgeler\AEN\Rejected_10.10.2024.csv";

List<Tuple<string, string>> bannedUsers = new List<Tuple<string, string>>()
{
    new Tuple<string, string>("Faris HOCAOĞLU", "faris.hocaoglu@hotmail.com"),
    new Tuple<string, string>("Murat Uygun", "muratuygun1998@gmail.com"),
};

List<Applicant> resultApplicants = new List<Applicant>();

ReadCsv.ProcessAllApplicants(applicantsFilePath);
ReadCsv.GetRejectedEmails(rejectedFilePath);

// The desired number of applicants
int desiredWomenCount = 55;
int desiredMenCount = 30;
int totalDesiredCount = desiredWomenCount + desiredMenCount;

// Get the list of applicants
var applicants = ReadCsv.Applicants;
var lastWeekRejectedEmails = ReadCsv.RejectedEmails;

// Ensure distinct applicants based on EmailAddress
var distinctApplicants = applicants.DistinctBy(x => x.EmailAddress).ToList();

// Add rejected applicants to the results, ensuring they are not banned
var rejectedApplicants = applicants.Where(x => 
    lastWeekRejectedEmails.Contains(x.EmailAddress) && 
    !ReadCsv.IsBanned(x, bannedUsers)).ToList();

resultApplicants.AddRange(rejectedApplicants);

// Exclude banned users from the distinct applicants
var bannedExcluded = distinctApplicants.Where(x => 
    !ReadCsv.IsBanned(x, bannedUsers)).ToList();

// Filter internationals (not from Turkey or Turks and Caicos Islands)
var internationals = bannedExcluded.Where(x => 
    x.Country != "Turkey" && x.Country != "Turks and Caicos Islands").ToList();

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

// Ensure distinct results from rejected and selected applicants
var finalResults = new List<Applicant>(resultApplicants);

// Add desired female applicants
for (int i = 0; i < femaleApplicants.Count && finalResults.Count < totalDesiredCount; i++)
{
    if (!finalResults.Contains(femaleApplicants[i]))
    {
        finalResults.Add(femaleApplicants[i]);
    }
}

// Add desired male applicants
for (int i = 0; i < maleApplicants.Count && finalResults.Count < totalDesiredCount; i++)
{
    if (!finalResults.Contains(maleApplicants[i]))
    {
        finalResults.Add(maleApplicants[i]);
    }
}

// Ensure the total accepted applicants does not exceed the desired count
var acceptedApplicants = finalResults.DistinctBy(x => x.EmailAddress).ToList();

// List rejected applicants (those who were not selected)
List<Applicant> allRejectedApplicants = applicants.Where(x => 
    !acceptedApplicants.Contains(x)).ToList();

// Output the count of rejected and accepted applicants
Console.WriteLine("Number of rejected applicants: " + allRejectedApplicants.Count);
Console.WriteLine("Number of accepted applicants: " + acceptedApplicants.Count);








// string applicantsFilePath = @"C:\Users\gokbe\OneDrive\Belgeler\AEN\TestCsv.csv";
// string rejectedFilePath = @"C:\Users\gokbe\OneDrive\Belgeler\AEN\Rejected_10.10.2024.csv";
//
// List<Tuple<string, string>> bannedUsers = new List<Tuple<string, string>>()
// {
//     new Tuple<string, string>("Faris HOCAOĞLU","faris.hocaoglu@hotmail.com"),
//     new Tuple<string, string>("Murat Uygun ","muratuygun1998@gmail.com"),
// };
//
// List<Applicant> resultApplicants = new List<Applicant>();
//
// ReadCsv.ProcessAllApplicants(applicantsFilePath);
// ReadCsv.GetRejectedEmails(rejectedFilePath);
//
// // The desired number of applicants
// int desiredWomenCount = 30;
// int desiredMenCount = 60;
//
// // Get the list of applicants
// var applicants = ReadCsv.Applicants;
// var lastWeekRejectedEmails = ReadCsv.RejectedEmails;
//
// // Ensure distinct applicants based on EmailAddress
// var distinctApplicants = applicants.DistinctBy(x => x.EmailAddress).ToList();
//
// // Add rejected applicants to the results, ensuring they are not banned
// resultApplicants.AddRange(applicants.Where(x => 
//     lastWeekRejectedEmails.Contains(x.EmailAddress) && 
//     !ReadCsv.IsBanned(x, bannedUsers)));
//
// // Exclude banned users
// var bannedExcluded = distinctApplicants.Where(x => 
//     !ReadCsv.IsBanned(x, bannedUsers)).ToList();
//
// // Filter internationals (not from Turkey or Turks and Caicos Islands)
// var internationals = bannedExcluded.Where(x => 
//     x.Country != "Turkey" && x.Country != "Turks and Caicos Islands").ToList();
// resultApplicants.AddRange(internationals);
//
// // Filter out internationals from the remaining applicants
// var internationalsExcluded = bannedExcluded.Where(x => 
//     !internationals.Contains(x)).ToList();
//
// // Separate by gender, excluding last week's rejected ones
// var femaleApplicants = internationalsExcluded
//     .Where(x => 
//         !x.Gender.Trim().Equals("Male", StringComparison.OrdinalIgnoreCase) && 
//         !lastWeekRejectedEmails.Contains(x.EmailAddress.Trim()))
//     .ToList();
//
// var maleApplicants = internationalsExcluded
//     .Where(x => 
//         x.Gender.Trim().Equals("Male", StringComparison.OrdinalIgnoreCase) && 
//         !lastWeekRejectedEmails.Contains(x.EmailAddress.Trim()))
//     .ToList();
//
// // Select desired number of female applicants
// for (int i = 0; i < Math.Min(femaleApplicants.Count, desiredWomenCount); i++)
// {
//     resultApplicants.Add(femaleApplicants[i]);
// }
//
// // Select desired number of male applicants
// for (int i = 0; i < Math.Min(maleApplicants.Count, desiredMenCount); i++)
// {
//     resultApplicants.Add(maleApplicants[i]);
// }
//
// // List rejected applicants (those who were not selected)
// List<Applicant> rejectedApplicants = applicants.Where(x => 
//     !resultApplicants.Contains(x)).ToList();
//
//
// var acceptedApplicants = resultApplicants.DistinctBy(x => x.EmailAddress).ToList();
//
// // Output the count of rejected applicants
// Console.WriteLine("Number of rejected applicants: " + rejectedApplicants.Count);
// Console.WriteLine("Number of accepted applicants: " + acceptedApplicants.Count);

