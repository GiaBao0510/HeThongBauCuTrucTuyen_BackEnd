using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.core.Entities;
using BackEnd.src.core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.src.infrastructure.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options): base(options){}

        //Thêm DbSet cho các thực thể khác, Mỗi DbSet là đại diện cho một tập các thực thể
        #region Dbset
        public DbSet<Roles> Roles{set; get;}    //Đại diện cho tập thực thể Roles, có thể thêm sửa xóa trực tiếp trên đây
        public DbSet<Account> Account{set;get;}
        public DbSet<Board> Board{set;get;}
        public DbSet<Candidate> Candidate{set;get;}
        public DbSet<CandidateNoticeDetails> CandidateNoticeDetails{set;get;}
        public DbSet<Constituency> Constituency{set;get;}
        public DbSet<District> District{set;get;}
        public DbSet<EducationLevel> EducationLevel{set;get;}
        public DbSet<EducationLevelDetails> EducationLevelDetails{set;get;}
        public DbSet<BallotDetails> BallotDetails{set;get;}
        public DbSet<ElectionDetails> ElectionDetails{set;get;}
        public DbSet<ElectionResults> ElectionResults{set;get;}  
        public DbSet<Elections> Elections{set;get;} 
        public DbSet<ElectionStatus> ElectionStatus{set;get;} 
        public DbSet<ListOfPositions> ListOfPositions{set;get;} 
        public DbSet<Lock> Lock{set;get;} 
        public DbSet<LoginHistory> LoginHistory{set;get;} 
        public DbSet<Notifications> Notifications{set;get;} 
        public DbSet<PermanentAddress> PermanentAddress{set;get;} 
        public DbSet<Province> Province{set;get;} 
        public DbSet<ResponseCandidate> ResponseCandidate{set;get;}
        public DbSet<ResponseVoter> ResponseVoter{set;get;}
        public DbSet<TemporaryAddress> TemporaryAddress{set;get;}
        public DbSet<Vote> Vote{set;get;}
        public DbSet<VoterNoticeDetails> VoterNoticeDetails{set;get;}
        public DbSet<Vouter> Vouter{set;get;}
        #endregion

        //Các lớp không có khóa chính thì đưa vô đây
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CandidateNoticeDetails>().HasNoKey();
            modelBuilder.Entity<EducationLevelDetails>().HasNoKey();
            modelBuilder.Entity<VoterNoticeDetails>().HasNoKey();
            modelBuilder.Entity<TemporaryAddress>().HasNoKey();
            modelBuilder.Entity<ResponseVoter>().HasNoKey();
            modelBuilder.Entity<ResponseCandidate>().HasNoKey();
            modelBuilder.Entity<LoginHistory>().HasNoKey();
            modelBuilder.Entity<ElectionStatus>().HasNoKey();
            modelBuilder.Entity<BallotDetails>().HasNoKey();
            modelBuilder.Entity<ElectionDetails>().HasNoKey();
            modelBuilder.Entity<PermanentAddress>().HasNoKey();
        }
    }
}