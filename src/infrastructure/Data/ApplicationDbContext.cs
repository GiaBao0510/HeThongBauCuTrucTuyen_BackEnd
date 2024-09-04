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
        public DbSet<Account> Accounts{set;get;}
        public DbSet<Board> Boards{set;get;}
        public DbSet<Candidate> Candidates{set;get;}
        public DbSet<CandidateNoticeDetails> CandidateNoticeDetails{set;get;}
        public DbSet<Constituency> Constituencys{set;get;}
        public DbSet<District> Districts{set;get;}
        public DbSet<EducationLevel> EducationLevels{set;get;}
        public DbSet<EducationLevelDetails> EducationLevelDetails{set;get;}
        public DbSet<BallotDetails> BallotDetails{set;get;}
        public DbSet<ElectionDetails> ElectionDetails{set;get;}
        public DbSet<ElectionResults> ElectionResults{set;get;}  
        public DbSet<Elections> Elections{set;get;}
        public DbSet<Ethnicity> Ethnicities{set;get;}
        public DbSet<Lock> Locks{set;get;}
        public DbSet<ElectionStatus> ElectionStatus{set;get;} 
        public DbSet<ListOfPositions> ListOfPositions{set;get;} 
        public DbSet<LoginHistory> LoginHistories{set;get;} 
        public DbSet<Notifications> Notifications{set;get;} 
        public DbSet<PermanentAddress> PermanentAddress{set;get;} 
        public DbSet<Province> Provinces{set;get;}
        public DbSet<PrivateKey> PrivateKey{set;get;}
        public DbSet<PublicKey> PublicKey{set;get;}
        public DbSet<ResponseCadre> ResponseCadres{set;get;}
        public DbSet<Position> Positions{set;get;}  
        public DbSet<ResponseCandidate> ResponseCandidates{set;get;}
        public DbSet<ResponseVoter> ResponseVoters{set;get;}
        public DbSet<TemporaryAddress> TemporaryAddress{set;get;}
        public DbSet<Vote> Votes{set;get;}
        public DbSet<VoterNoticeDetails> VoterNoticeDetails{set;get;}
        public DbSet<Voter> Vouters{set;get;}
        public DbSet<User> Users{set;get;}
        public DbSet<Cadre> Cadres{set;get;}
        public DbSet<WorkPlace> WorkPlaces{set;get;}
        public DbSet<CadreEducationLevelDetail> CadreEducationLevelDetails{set;get;}
        public DbSet<CadreNoiticeDetail> CadreNoiticeDetail{set;get;}
        #endregion

        /*
            Sử dụng các API thích hợp để định nghĩa ra mối quan hệ giữa các bảng
        Cần nạp chồng OnModelCreating của lớp DbContext.
        */
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Các Fluent API. Để định nghĩa mối quan hệ giữa các entity cho nó rõ ràng hơn
            
            modelBuilder.Entity<Roles>(entity => {
                entity.ToTable("VaiTro");
                entity.HasKey(e => e.RoleID);
                entity.Property(e => e.TenVaiTro).IsRequired().HasMaxLength(30);
            });
            modelBuilder.Entity<Account>(entity => {
                entity.ToTable("TaiKhoan");
                entity.HasKey(e => e.TaiKhoan);
                entity.Property(e => e.MatKhau).IsRequired();
                entity.Property(e => e.MatKhau).IsRequired();
                entity.Property(e => e.MatKhau).IsRequired();
                
                entity.Property(e => e.MatKhau).IsRequired();
            });
            
        }
    }
}