let arrFile = [
    "Account.cs", "Board.cs","Candidate.cs","CandidateNoticeDetails.cs",
    "Constituency.cs","District.cs", "EducationLevel.cs","EducationLevelDetails.cs",
    "ElectionDetails.cs","ElectionResults.cs","Elections.cs","ElectionStatus.cs",
    "ListOdPositions.cs", "LoginHistory.cs","Notifications.cs","PermanentAddress.cs",      
    "Provinces.cs","ResponseCandidate.cs","ResponseVoter.cs","Roles.cs",
    "TemporaryAddress.cs","Vote.cs","VoterNoticeDetails.cs", "Vouter.cs"
];

let result = "";
for(let i in arrFile){
    result += `"">I${arrFile[i]};`
}
console.log(result);