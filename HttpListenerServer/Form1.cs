using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq; 
using System.Net;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace HttpListenerServer
{
    public partial class Form1 : Form
    {
        static HttpListener listener;
        private Thread listenThread1;
        public Form1()
        {
            InitializeComponent();
        }
        
        
        private void Form1_Load(object sender, EventArgs e)
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8000/");
            listener.Prefixes.Add("http://127.0.0.1:8000/");
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

            listener.Start();
            this.listenThread1 = new Thread(new ParameterizedThreadStart(startlistener));
            listenThread1.Start();
           
        }


        private void startlistener(object s)
        {

            while (true)
            {
               
                ////blocks until a client has connected to the server
                ProcessRequest();

            }

        }


        private void ProcessRequest()
        {

            var result = listener.BeginGetContext(ListenerCallback, listener);
            result.AsyncWaitHandle.WaitOne();

        }

        private void ListenerCallback(IAsyncResult result)
        {
            var context = listener.EndGetContext(result);
            Thread.Sleep(1000);
            var data_text = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd();

            //functions used to decode json encoded data.
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //var data1 = Uri.UnescapeDataString(data_text);
            //string da = Regex.Unescape(data_text);
            // var unserialized = js.Deserialize(data_text, typeof(String));

            var cleaned_data = System.Web.HttpUtility.UrlDecode(data_text);

            context.Response.StatusCode = 200;
            context.Response.StatusDescription = "OK";

            //use this line to get your custom header data in the request.
            //var headerText = context.Request.Headers["mycustomHeader"];

            //use this line to send your response in a custom header
            //context.Response.Headers["mycustomResponseHeader"] = "mycustomResponse";
            
            MessageBox.Show(cleaned_data);
            context.Response.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://arshad115.blogspot.com");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("mailto:arshad115@hotmail.com");
        }

    }
}
