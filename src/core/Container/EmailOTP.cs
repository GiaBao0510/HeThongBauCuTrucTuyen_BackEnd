
namespace BackEnd.src.core.Container
{
    public class EmailOTP
    {
        public string GenerateOtpEmail(string otp)
        {
            return $@"
            <!DOCTYPE html>
            <html lang='vi'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>VoteSecure - Xác thực OTP</title>
                <style>
                    /* Sử dụng system fonts để tối ưu hiệu suất */
                    :root {{
                        --primary: #2563eb;
                        --secondary: #1e40af;
                        --background: #f8fafc;
                        --text: #0f172a;
                        --success: #059669;
                    }}
                    
                    * {{
                        margin: 0;
                        padding: 0;
                        box-sizing: border-box;
                    }}

                    body {{
                        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen-Sans, Ubuntu, Cantarell, sans-serif;
                        line-height: 1.6;
                        background-color: var(--background);
                        color: var(--text);
                        padding: 20px;
                    }}

                    .email-container {{
                        max-width: 600px;
                        margin: 0 auto;
                        background: white;
                        border-radius: 12px;
                        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
                        overflow: hidden;
                    }}

                    .header {{
                        background: var(--primary);
                        padding: 24px;
                        text-align: center;
                    }}

                    .header h1 {{
                        color: rgba(30, 176, 217, 1);
                        font-size: 24px;
                        font-weight: 600;
                        margin: 0;
                        letter-spacing: 0.5px;
                    }}

                    .content {{
                        padding: 32px 24px;
                        text-align: center;
                    }}

                    .message {{
                        margin-bottom: 24px;
                        font-size: 16px;
                        color: var(--text);
                    }}

                    .otp-code {{
                        font-family: monospace;
                        font-size: 32px;
                        font-weight: 700;
                        letter-spacing: 4px;
                        color: var(--primary);
                        background: #f1f5f9;
                        padding: 16px 24px;
                        border-radius: 8px;
                        margin: 24px 0;
                        border: 2px dashed var(--primary);
                    }}

                    .timer {{
                        display: inline-block;
                        color: var(--secondary);
                        font-size: 14px;
                        background: #e2e8f0;
                        padding: 6px 12px;
                        border-radius: 16px;
                        margin-top: 16px;
                    }}

                    .footer {{
                        background: #f8fafc;
                        padding: 16px;
                        text-align: center;
                        font-size: 12px;
                        color: #64748b;
                        border-top: 1px solid #e2e8f0;
                    }}

                    .security-notice {{
                        margin-top: 24px;
                        font-size: 13px;
                        color: #64748b;
                        padding: 12px;
                        background: #f1f5f9;
                        border-radius: 6px;
                    }}

                    .tech-badge {{
                        display: inline-block;
                        background: var(--success);
                        color: white;
                        padding: 4px 8px;
                        border-radius: 4px;
                        font-size: 12px;
                        margin-top: 8px;
                    }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <div class='header'>
                        <h1>🔐 VoteSecure</h1>
                    </div>
                    
                    <div class='content'>
                        <div class='message'>
                            <strong>Xin chào!</strong>
                            <p>Đây là mã xác thực OTP cho tài khoản của bạn:</p>
                        </div>

                        <div class='otp-code'>{otp}</div>

                        <div class='timer'>
                            ⏱️ Mã có hiệu lực trong 5 phút
                        </div>

                        <div class='security-notice'>
                            🛡️ Vì lý do bảo mật, tuyệt đối không chia sẻ mã này với bất kỳ ai.
                            <br>
                            Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.
                        </div>

                        <div class='tech-badge'>
                            Secured by VoteSecure Technology
                        </div>
                    </div>

                    <div class='footer'>
                        © 2024 VoteSecure - Hệ thống bảo mật hai lớp
                        <br>
                        Email này được gửi tự động, vui lòng không phản hồi.
                    </div>
                </div>
            </body>
            </html>";
        }
    }
}