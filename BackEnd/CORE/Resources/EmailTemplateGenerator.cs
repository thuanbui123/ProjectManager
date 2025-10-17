namespace CORE.Resources;

public static class EmailTemplateGenerator
{
    public static string GenerateWelcomeTemplate(string name, string verificationLink)
    {
        return $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                      <style>
                        body {{
                          font-family: Arial, sans-serif;
                        }}
                        .btn {{
                          display: inline-block;
                          padding: 10px 20px;
                          background-color: #4CAF50;
                          color: white !important;
                          text-decoration: none !important;
                          border-radius: 5px;
                          font-weight: bold;
                          font-size: 14px;
                          border: none;
                        }}
                        .btn:hover {{
                          background-color: #45a049;
                        }}
                      </style>
                    </head>
                    <body>
                      <h2>Xin chào {name},</h2>
                      <p>Chào mừng bạn đến với <strong>hệ thống quản lý tiến độ dự án</strong>.</p>
                      <p>Vui lòng xác minh tài khoản của bạn bằng cách nhấn vào nút bên dưới:</p>
                      <p>
                        <a href='{verificationLink}' class='btn'>Xác minh Email</a>
                      </p>
                      <p>Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>
                    </body>
                    </html>";
    }
}
