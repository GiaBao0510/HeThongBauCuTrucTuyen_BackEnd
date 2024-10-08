using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.src.core.Entities;
using BackEnd.core.Entities;

namespace BackEnd.core.Entities
{
    [Table("UngCuVien")]
    public class Candidate
    {
        [Key]
        [DisplayName("Identify candidates")]
        public required string ID_ucv { set; get; }
        [Required]
        [DisplayName("Status")]
        [MaxLength(10)]
        public required string TrangThai { set; get; }

        //Khóa ngoại
        public string? ID_user { get; set; }
        [ForeignKey("ID_user")]
        public Users user {set;get;}

        //Truy xuất ngược
        public virtual ICollection<ElectionResults> electionResults{set;get;}
        public virtual ICollection<ResponseCandidate> responseCandidate{set;get;}
        public virtual ICollection<CandidateNoticeDetails> candidateNoticeDetails{set;get;}
        public virtual ICollection<BallotDetails> ballotDetails{set;get;}
        public virtual ICollection<CandidateDetails> candidateDetails{set;get;}
        public virtual ICollection<EducationLevelDetails> educationLevelDetails{set;get;}
    
        //Cho các lớp truy xuất ngược trả về mảng rỗng
        public Candidate(){
            electionResults = new List<ElectionResults>();
            responseCandidate = new List<ResponseCandidate>();
            candidateNoticeDetails = new List<CandidateNoticeDetails>();
            ballotDetails = new List<BallotDetails>();
            educationLevelDetails = new List<EducationLevelDetails>();
            candidateDetails = new List<CandidateDetails>();
        }
    }
}

