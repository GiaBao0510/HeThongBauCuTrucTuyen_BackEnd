using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace BackEnd.src.infrastructure.Services
{
    public class MyHttpServer_1
    {
        private HttpListener listener;
        
        //Hàm khởi tạo, nhận vào một mảng các địa chỉ Http lắng nghe
        public MyHttpServer_1(params string[] prefixes){
            
            //Quăng ra lõi nếu không hỗ trợ HttpListener
            if(!HttpListener.IsSupported)
                throw new Exception("Máy chủ không hỗ trợ HttpListener");

            //Nêu mảng các địa chỉ http lắng nghe mà rỗng thì quăng lỗi
            if(prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");
            
            //Khởi tạo HttpListener
            listener = new HttpListener();
            foreach(string prefixe in  prefixes)
                listener.Prefixes.Add(prefixe);
        }

        //Khởi tạo
        public async Task Start(){
            //Bắt đầu lắng nghe kết nối HTTP
            listener.Start();
            do{
                try{
                    Console.WriteLine($"\n\t{DateTime.Now.ToString()} : waiting a client connect");

                    //Một clinet kết nối đến
                    HttpListenerContext context = await listener.GetContextAsync();
                    await ProcessRequest(context);

                }catch(Exception Ex){
                    Console.WriteLine($"Error: {Ex.Message}");
                }
            }while(listener.IsListening);
        }

        //Lắng nghe yêu cầu từ phía máy khách
        async Task ProcessRequest(HttpListenerContext context){
            HttpListenerRequest req = context.Request;
            HttpListenerResponse res = context.Response;
            Console.WriteLine($"{req.HttpMethod} - {req.RawUrl} - {req.Url.AbsolutePath}");

            //Lấy stream / gửi dữ liệu cho client
            var outputstream = res.OutputStream;

            //API - GET
            switch(req.Url.AbsolutePath){
                case"/json":{
                    res.Headers.Add("Content-Type","application/json");
                    var temp = new {
                        HoTen = "GiaBao",
                        MSSV = "B2016947",
                        Tuoi = 24
                    };

                    string jsonstring = JsonConvert.SerializeObject(temp);
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonstring);
                    res.ContentLength64 = buffer.Length;
                    await outputstream.WriteAsync(buffer, 0 , buffer.Length);
                }break;
                case"/api/stop":{
                    listener.Stop();
                    Console.WriteLine("Stop http");
                }break;
                default:{
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Not Found!");
                    res.ContentLength64 = buffer.Length;
                    await outputstream.WriteAsync(buffer,0,buffer.Length);
                }break;
            }

            //Đóng stream để hoàn thành gửi về client
            outputstream.Close();
        }
    }
}