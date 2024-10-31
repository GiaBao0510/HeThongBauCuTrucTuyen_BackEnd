
namespace BackEnd.src.infrastructure.Services
{
    public class GoogleDriveOptions
    {
        public const string Configuration = "GoogleDrive";
        public string credentialPath {set; get;}
        public string tokenPath {set; get;}

        //Kiểm tra tính hợp lệ
        public void Validate(){
            //Kiểm tra xem đường dẫn hợp lệ không
            if(string.IsNullOrEmpty(credentialPath))
                throw new ArgumentNullException(nameof(credentialPath), "CredentialsPath must be configured");

            if(string.IsNullOrEmpty(tokenPath))
                throw new ArgumentNullException(nameof(tokenPath), "TokenPath must be configured");

            if(!File.Exists(credentialPath))
                throw new FileNotFoundException("Credentials file not found", credentialPath);
        }
    }
}