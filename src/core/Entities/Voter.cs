using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.src.core.Entities;

namespace BackEnd.core.Entities
{
    [Table("CuTri")]
    public class Voter
    {
        [Key]
        [DisplayName("Identify candidates")]
        public string? ID_CuTri { set; get; }

        //Khoa ngoai
        public string? ID_user{get; set;}
        [ForeignKey("ID_user")]
        public Users user{set;get;}

        //Truy xuất ngược
        public virtual ICollection<ElectionStatus>electionStatus {set;get;}
        public virtual ICollection<ElectionDetails>electionDetails {set;get;}
        public virtual ICollection<ResponseVoter>responseVoter {set;get;}
        public virtual ICollection<BallotDetails>ballotDetails {set;get;}
        public virtual ICollection<VoterNoticeDetails> voterNoticeDetails {set;get;}
        public virtual ICollection<VoterDetails> voterDetails {set;get;}

        //Cho các lớp truy xuất ngược trả về mảng rỗng khi mảng rỗng
        public Voter(){
            ballotDetails = new List<BallotDetails>();
            electionStatus = new List<ElectionStatus>();
            electionDetails = new List<ElectionDetails>();
            responseVoter = new List<ResponseVoter>();
            voterNoticeDetails = new List<VoterNoticeDetails>();
            voterDetails = new List<VoterDetails>();
        }
    }
}