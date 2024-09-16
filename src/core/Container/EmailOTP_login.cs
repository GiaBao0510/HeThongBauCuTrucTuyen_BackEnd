using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeThongBauCuTrucTuyen_BackEnd.src.core.Container
{
    public class EmailOTP_login
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
                        :root {{
                            --primary-color: #3498db;
                            --secondary-color: #2ecc71;
                            --text-color: #333;
                            --background-color: #ecf0f1;
                        }}
                        body {{
                            font-family: 'Roboto', sans-serif;
                            background-color: var(--background-color);
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            min-height: 100vh;
                            margin: 0;
                            padding: 20px;
                            box-sizing: border-box;
                        }}
                        .container {{
                            background-color: white;
                            padding: 2rem;
                            border-radius: 15px;
                            box-shadow: 0 10px 20px rgba(0, 0, 0, 0.1);
                            text-align: center;
                            max-width: 400px;
                            width: 100%;
                        }}
                        h1 {{
                            color: var(--primary-color);
                            margin-bottom: 1rem;
                            font-size: 2.5rem;
                        }}
                        p {{
                            color: var(--text-color);
                            margin-bottom: 1.5rem;
                            line-height: 1.6;
                        }}
                        .otp-display {{
                            font-size: 2rem;
                            font-weight: bold;
                            color: var(--secondary-color);
                            padding: 10px;
                            background-color: #f7f7f7;
                            border-radius: 10px;
                            margin: 20px 0;
                            letter-spacing: 5px;
                        }}
                        .image-container {{
                            width: 100%;
                            max-width: 400px;
                            margin: 20px auto 0;
                            overflow: hidden;
                            border-radius: 10px;
                            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                        }}
                        .responsive-image {{
                            width: 100%;
                            height: auto;
                            display: block;
                            object-fit: cover;
                            object-position: center;
                        }}
                        @media (max-width: 600px) {{
                            .container {{
                                padding: 1.5rem;
                            }}
                            h1 {{
                                font-size: 2rem;
                            }}
                            .otp-display {{
                                font-size: 1.5rem;
                            }}
                            .image-container {{
                                max-width: 100%;
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