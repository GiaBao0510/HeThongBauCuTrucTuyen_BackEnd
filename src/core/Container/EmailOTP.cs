
namespace BackEnd.src.core.Container
{
    public class EmailOTP
    {
        public string GenerateOtpEmail(string otp){
            var body = $@"
            <!DOCTYPE html>
            <html lang='vi'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>VoteSecure - Mã xác thực OTP</title>
                <style>
                    @import url('https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap');
                    
                    :root {{
                        --primary-color: #3498db;
                        --secondary-color: #2ecc71;
                        --accent-color: #e74c3c;
                        --text-color: #2c3e50;
                        --background-color: #ecf0f1;
                    }}
                    body {{
                        font-family: 'Roboto', sans-serif;
                        background: linear-gradient(120deg, #a1c4fd 0%, #c2e9fb 100%);
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        min-height: 100vh;
                        margin: 0;
                        padding: 20px;
                        box-sizing: border-box;
                    }}
                    .container {{
                        background: rgba(255, 255, 255, 0.9);
                        padding: 2.5rem;
                        border-radius: 20px;
                        box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
                        text-align: center;
                        max-width: 400px;
                        width: 100%;
                        backdrop-filter: blur(10px);
                    }}
                    h1 {{
                        color: var(--primary-color);
                        margin-bottom: 1rem;
                        font-size: 2.5rem;
                        text-transform: uppercase;
                        letter-spacing: 2px;
                        text-shadow: 2px 2px 4px rgba(0,0,0,0.1);
                    }}
                    p {{
                        color: var(--text-color);
                        margin-bottom: 1.5rem;
                        line-height: 1.6;
                        font-size: 1.1rem;
                    }}
                    .otp-display {{
                        font-size: 2.2rem;
                        font-weight: bold;
                        color: var(--accent-color);
                        padding: 15px;
                        background: linear-gradient(45deg, #f3f4f6, #fff);
                        border-radius: 15px;
                        margin: 25px 0;
                        letter-spacing: 8px;
                        box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
                        transition: all 0.3s ease;
                    }}
                    .otp-display:hover {{
                        transform: translateY(-5px);
                        box-shadow: 0 8px 20px rgba(0, 0, 0, 0.15);
                    }}
                    .image-container {{
                        width: 100%;
                        max-width: 300px;
                        margin: 25px auto 0;
                        overflow: hidden;
                        border-radius: 15px;
                        box-shadow: 0 8px 25px rgba(0, 0, 0, 0.2);
                    }}
                    .responsive-image {{
                        width: 100%;
                        height: auto;
                        display: block;
                        object-fit: cover;
                        object-position: center;
                        transition: transform 0.3s ease;
                    }}
                    .responsive-image:hover {{
                        transform: scale(1.05);
                    }}
                    @media (max-width: 600px) {{
                        .container {{
                            padding: 2rem;
                        }}
                        h1 {{
                            font-size: 2rem;
                        }}
                        .otp-display {{
                            font-size: 1.8rem;
                            letter-spacing: 6px;
                        }}
                        p {{
                            font-size: 1rem;
                        }}
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>VoteSecure</h1>
                    <p>Đây là mã xác thực đăng nhập của bạn. Vui lòng sử dụng mã này để hoàn thành bước đăng nhập. Mã có hiệu lực trong 5 phút.</p>
                    <div class='otp-display'>{otp}</div>
                    <div class='image-container'>
                        <img class='responsive-image' src='https://res.cloudinary.com/dkajnklq6/image/upload/v1726309469/NguoiDung/x101hxwcmigiywmodxlo.png' alt='Hình ảnh minh họa bầu cử'>
                    </div>
                </div>
            </body>
            </html>
            ";

            return body;
        }
    }
}