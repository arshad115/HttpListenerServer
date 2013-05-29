HttpListenerServer
===================
************
HttpListenerServer by **Arshad Mehmood** ( **arshad115** )


![Alt Program Screen]( http://a.fsdn.com/con/app/proj/httplistener/screenshots/httplistener.JPG)



**************************
Introduction
----------------------
****************

**HttpListenerServer** is a multithreaded simple webserver written in **C#,** made in **Visual Studio 2012**.This project uses the **HttpListener** Class (System.Net) to create a simple webserver. This class provides a simple HTTP protocol listener. The webserver is capable of listening to mutilple calls through multiple domains. The request data(JSON/Text) is also handled in the web server callback.This webserver listens for requests on the port 8000 on the localhost.

The code also includes commented code for response and for sending and receiving custom headers in the request.


##Background 
******************************
Web servers and web services are generally hosted on  IIS, Apache, or Tomcat. These often require complex installations and configuration. You can host your ASP.Net web applications or Php web sites.

A ASMX webservice can easily be created with an ASP.Net web application. You can also create a webservice endpoint using **[WebMethod]** before your method declaration. This method should be a static method in the code behind file of the web page.

This application is a part of another application which required a simple stand alone http web service listener. A simple web service was required in a windows forms application and HttpListener class met my requirements. 


##Using the code
*****************************

The project is a simple windows forms application with target .Net framework 4.0.

Now, lets get started with how the code works and how you can use it in your .Net C# application.

First of all define these variables in your class:
<br />
<pre>
<code>     
            static HttpListener listener;
            private Thread listenThread1;
            </code>
            </pre>

The Form load event method is as follows:
<pre>
<code> 
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

</code>
</pre>

Instantiate the HttpListener and add the prefixes on which the program should listen. If the request has the URI that matches this URI prefix then the program will handle that request. 
<pre>
<code> 
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8000/");
            listener.Prefixes.Add("http://127.0.0.1:8000/");

</code>
</pre>

####More on URI Prefixes (From MSDN):
-----

A URI prefix string is composed of a scheme (http or https), a host, an optional port, and an optional path. An example of a complete prefix string is "http://www.contoso.com:8080/customerData/". Prefixes must end in a forward slash ("/"). The HttpListener object with the prefix that most closely matches a requested URI responds to the request. Multiple HttpListener objects cannot add the same prefix; a Win32Exception exception is thrown if a HttpListener adds a prefix that is already in use.
When a port is specified, the host element can be replaced with "\*" to indicate that the HttpListener accepts requests sent to the port if the requested URI does not match any other prefix. For example, to receive all requests sent to port 8080 when the requested URI is not handled by any HttpListener, the prefix is "http://\*:8080/". Similarly, to specify that the HttpListener accepts all requests sent to a port, replace the host element with the "+" character, "https://+:8080". The "\*" and "+" characters can be present in prefixes that include paths.

--------
An HttpListener can require client authentication. The following line states that no authentication is required for the http request.
<pre>
<code> 
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            
</code>
</pre>

The following code starts the listener to listen on the specified prefixes. A new thread is created and passed the method 'startlistener' which is a blocking method and cant be run on the ui thread. We can also pass any object to the method in the thread but it is not my requirement so I havent passed anything to the thread Start() method.

<pre>
<code> 

            listener.Start();
            this.listenThread1 = new Thread(new ParameterizedThreadStart(startlistener));
            listenThread1.Start();
            
</code>
</pre>


The threads is blocked until a client send a request to the listener. 
The ListenerCalback function is called when a request is received and needs to be handled.



<pre>
<code> 
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
</code>
</pre>

BeginGetContext begins asynchronously retrieving an incoming request. We use AsyncWaitHandle.WaitOne() to prevent this thread from terminating while the asynchronous operation completes.

<br />
Our code to the ListenerCallback is as follows:

<pre>
<code> 
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
</code>
</pre>

The EndGetContext method completes an asynchronous operation to retrieve an incoming client request.
The context contains the HttpListenerRequest object named Request and the HttpListenerResponse named Response depending on the GET/POST request. 

The POST data is received via the InputStream. I am also specifying the encoding used to make things eaiser.
<pre>
<code>
            var data_text = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd();
</code>
</pre>

The following functions can be used to clean/decode/unescape the data.
I had to use the UrlDecode method to unescape the data.
For that to work you have to add a reference to the System.Web dll in your application. 
<pre>
<code> 
            //functions used to decode json encoded data.
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //var data1 = Uri.UnescapeDataString(data_text);
            //string da = Regex.Unescape(data_text);
            // var unserialized = js.Deserialize(data_text, typeof(String));

            var cleaned_data = System.Web.HttpUtility.UrlDecode(data_text);
</code>
</pre>

We are sending a POST request and do not require a response, but you can use these lines to send the response in the header:

<pre>
<code> 
            //use this line to get your custom header data in the request.
            //var headerText = context.Request.Headers["mycustomHeader"];

            //use this line to send your response in a custom header
            //context.Response.Headers["mycustomResponseHeader"] = "mycustomResponse";
</code>
</pre>

You can also send the response via the context.Response.OutputStream and flush the data using:

<pre>
<code> 
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer,0,buffer.Length);
</code>
</pre>
Where buffer is a byte array containing the response that you want to send.

Finally we send an OK response via these lines of code. 
<pre>
<code> 
            context.Response.StatusCode = 200;
            context.Response.StatusDescription = "OK";
            
</code>
</pre>

You can call the  listener.Stop() method in your application for the listener to stop listening on the specified port and prefixes. Keep in mind that only one listener can be listening to a port otherwise you will get an  exception.




##Points of Interest
*****************************
HttpListenerServer only implements a simple server for handling Ajax requests. You can also extend this code send a web page as a response or host your own web pages.

##Software Requirements
*****************************
<ul>
<li>Visual Studio 2012 (Can run on lower versions)</li >
<li>Administrator rights are required for HttpListener Class (System.Net). </li>
</ul>

##License
*****************************

<a rel="license" href="http://creativecommons.org/licenses/by/3.0/"><img alt="Creative Commons License" style="border-width:0" src="http://i.creativecommons.org/l/by/3.0/80x15.png" /></a><br />This work is licensed under a <a rel="license" href="http://creativecommons.org/licenses/by/3.0/">Creative Commons Attribution 3.0 Unported License</a>.

##Credits
*****************************
Copyright © Arshad Mehmood  
<br />[arshad115@hotmail.com ](mailto:arshad115@hotmail.com)
<br />[arshad@kookydroid.com](mailto:arshad@kookydroid.com)
<br />[Follow @arshad115](https://twitter.com/arshad115)
